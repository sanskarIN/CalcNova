# CalcNova Release Artifact Provenance

CalcNova's stable release workflow generates cryptographic provenance attestations for packaged release artifacts in addition to SHA-256 checksum material.

This is a post-2.8.03 supply-chain hardening control. It does not change the CalcNova product version and it does not claim that an artifact is vulnerability-free.

## What is attested

The release publication job attests every file in the prepared `release-assets/` tree after checksum generation. For the current release topology, that includes:

- Windows x64 desktop ZIP;
- Windows ARM64 desktop ZIP;
- Linux x64 desktop ZIP;
- Linux ARM64 desktop ZIP;
- macOS Intel x64 desktop ZIP;
- macOS Apple Silicon ARM64 desktop ZIP;
- Browser/WebAssembly ZIP;
- Android AAB when signing secrets are configured;
- `SHA256SUMS.txt`.

The workflow uses:

```text
actions/attest@v4
```

with one inclusive subject glob:

```text
release-assets/**/*
```

Using one inclusive release-tree subject keeps optional artifacts such as the signed Android AAB conditional without requiring a separate path that may not exist in an unsigned release run.

## Why provenance matters

An artifact attestation allows a consumer to verify that the artifact is associated with GitHub build provenance for the CalcNova repository and release workflow. The provenance includes build identity such as repository/workflow context, commit identity, and triggering event.

An attestation does **not** prove that the artifact contains no defects or vulnerabilities. It proves provenance/integrity facts that can be checked independently.

## Least-privilege publication permissions

The release workflow's default token permission is:

```yaml
permissions:
  contents: read
```

Only the `publish-release` job receives the additional permissions needed to publish the GitHub Release and produce current `actions/attest@v4` artifact metadata/attestations:

```yaml
permissions:
  contents: write
  id-token: write
  attestations: write
  artifact-metadata: write
```

Build/validation jobs therefore do not inherit release-write, OIDC, attestation, or artifact-metadata write privileges.

`id-token: write` allows GitHub's attestation flow to establish workflow identity. `attestations: write` allows the attestation to be persisted. `artifact-metadata: write` allows the current attestation action to create the artifact storage metadata record. `contents: write` is required by the publication job to create/update GitHub Release assets.

## Release asset filename contract

GitHub Release assets are presented to users as flat filenames even though `actions/download-artifact` materializes prerequisite workflow artifacts under per-artifact subdirectories.

Before generating checksums, CalcNova validates that:

- at least one release asset exists;
- no two prepared files have the same basename;
- no build artifact is already named `SHA256SUMS.txt` because that name is reserved for the generated checksum manifest.

The duplicate-basename guard prevents two nested workflow artifacts from later collapsing to the same GitHub Release filename.

## Download-friendly checksum manifest

`SHA256SUMS.txt` contains the published asset **basenames**, not runner-local paths such as `release-assets/desktop-win-x64/...`.

A manifest entry therefore has the form:

```text
<sha256>  CalcNova-win-x64.zip
```

rather than a GitHub Actions workspace path.

After downloading `SHA256SUMS.txt` and the release files into the same directory on a system with GNU/coreutils-compatible `sha256sum`, users can run:

```bash
sha256sum -c SHA256SUMS.txt
```

The manifest deliberately excludes itself from checksum generation; it is then copied into the release-asset tree and covered by the provenance-attestation step.

## Release ordering

The publication flow intentionally follows this order:

1. download build artifacts from the prerequisite jobs;
2. reject duplicate/reserved release filenames and require at least one asset;
3. generate a flat, download-friendly SHA-256 manifest using published basenames;
4. copy `SHA256SUMS.txt` into the release-asset tree;
5. generate provenance attestations for the prepared release-asset tree;
6. create the GitHub Release if it does not already exist;
7. upload/replace the intended release assets.

This means the checksum manifest itself is included in the attested subject set.

## Verify a downloaded artifact

With a current GitHub CLI installation, verify a downloaded CalcNova release artifact using:

```bash
gh attestation verify PATH_TO_ARTIFACT -R sanskarIN/CalcNova
```

Example:

```bash
gh attestation verify CalcNova-win-x64.zip -R sanskarIN/CalcNova
```

For stronger policy binding, GitHub CLI also supports constraining verification to an expected signer workflow. Review the current GitHub CLI attestation options before introducing organization-level enforcement.

## Verify checksum content

Artifact provenance and checksums serve different purposes.

First verify the provenance of `SHA256SUMS.txt` if it is available as a release asset:

```bash
gh attestation verify SHA256SUMS.txt -R sanskarIN/CalcNova
```

Then, after downloading the release assets into the same directory, verify their SHA-256 values:

```bash
sha256sum -c SHA256SUMS.txt
```

On platforms without `sha256sum`, use the platform's SHA-256 tool and compare against the basename entry in `SHA256SUMS.txt`.

Do not treat a matching checksum from an untrusted source as equivalent to provenance verification; an attacker who can replace both an artifact and an unauthenticated checksum file could make both agree.

## Offline attestation verification

GitHub supports downloading attestation bundles and trusted-root material for later offline verification. The high-level flow is:

1. from an online system, download the attestation bundle for the artifact;
2. export current trusted-root material;
3. transfer the artifact, bundle, trusted-root file, and GitHub CLI to the offline environment;
4. run `gh attestation verify` with the downloaded bundle and trusted root.

Follow current GitHub documentation for the exact offline-verification command flags because CLI behavior can evolve over time.

## Source contract validation

`tools/validate_release_workflow.py` protects the release integrity/provenance contract by requiring:

- global read-only contents permission;
- exactly one `contents: write` grant;
- exactly one `id-token: write` grant;
- exactly one `attestations: write` grant;
- exactly one `artifact-metadata: write` grant;
- `actions/attest@v4`;
- the inclusive `release-assets/**/*` subject glob;
- a duplicate/reserved release-filename validation step;
- checksum entries written with `basename "$file"` rather than runner-local paths;
- checksum generation after filename validation;
- attestation after checksum generation and before release publication;
- rejection of the old nested-path `xargs -0 sha256sum > SHA256SUMS.txt` implementation;
- rejection of deprecated wrapper action references in the release workflow.

Regression source in `tools/tests/test_validate_release_workflow.py` locks the same permission/action/checksum/subject contract.

Both are part of the integrated source preflight:

```bash
python tools/release_preflight.py
```

## Evidence semantics

Use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Examples:

- release workflow source satisfies its validator after execution: source-contract `PASS`;
- `actions/attest` run succeeds and produces an attestation for a release ZIP: attestation service evidence `PASS`;
- `sha256sum -c SHA256SUMS.txt` succeeds against downloaded release assets: downloaded-checksum evidence `PASS`;
- GitHub Actions/OIDC is unavailable in a local environment: `NOT RUN` or `BLOCKED`, depending on context;
- `gh attestation verify` rejects a downloaded artifact: verification `FAIL` until the mismatch is understood.

Never claim an attestation or checksum verification succeeded merely because the workflow source exists.

## Related documentation

- [Release process](RELEASE.md)
- [Security automation](SECURITY_AUTOMATION.md)
- [Security engineering](SECURITY.md)
- [Artifact/release evidence](VALIDATION_EVIDENCE.md)
- [Release readiness checklist](RELEASE_READINESS_CHECKLIST.md)
