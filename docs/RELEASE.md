# CalcNova 2.8.03 Release Process

## Release identity

CalcNova 2.8.03 uses:

- product/display version: `2.8.03`;
- normalized .NET/NuGet version: `2.8.3`;
- normalized release tag: `v2.8.3`;
- assembly/file version: `2.8.3.0`;
- Android/iOS display version: `2.8.03`;
- Android/iOS numeric build code: `20803`.

See [VERSIONING.md](VERSIONING.md).

Strict Semantic Versioning does not allow leading zeroes in numeric identifiers, so `v2.8.03` is intentionally invalid as a Git release tag. The correct normalized tag for the public 2.8.03 release is `v2.8.3`.

## Release evidence principle

CalcNova 2.8.03 is the completed product baseline. Release execution evidence is recorded independently.

A command, build, test, device check, signing operation, accessibility audit, security-service run, attestation, or store validation is marked PASS only after it actually executes and the result is observed. `NOT RUN` and `BLOCKED` are evidence states for unavailable environments/tools/credentials/services; they do not describe the product as unfinished.

## Source release gate

From the exact 2.8.03 release-tag checkout, run:

```bash
python tools/release_preflight.py --tag v2.8.3
```

The preflight validates repository/security structure, XAML/UI/navigation/keyboard contracts, calculator editing, graph/numerical contracts, Unicode metadata, exact rationals, engineering notation, bounded exports, bivariate statistics, accessibility/adaptive/localization contracts, settings/onboarding, package metadata, the 2.8.03 completion status, platform workflows, security-automation workflows, release workflows, artifact integrity, structured release evidence, and regression suites for the SDK-independent validators/tooling.

The preflight also rejects obsolete current-status wording in authoritative completion documents.

## .NET quality gate

In a suitable .NET 10 environment:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Target-specific builds then run for the platform artifacts included in the release.

The SDK-independent preflight and compiled quality gate are separate evidence layers.

## Automated security gates

The maintained branch also contains:

- `.github/workflows/codeql.yml` — C# CodeQL scanning;
- `.github/workflows/dependency-review.yml` — PR dependency vulnerability review at moderate severity or higher;
- `.github/dependabot.yml` — NuGet and GitHub Actions update proposals;
- `.github/workflows/security-automation-validate.yml` — focused security-workflow source validation.

See [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md).

A release maintainer should review relevant CodeQL, dependency-review, Dependabot, and repository security alerts before publication. The presence of workflow source is not equivalent to a successful service run.

## Automated release flow

`.github/workflows/release.yml` supports:

- a pushed `v*` tag;
- manual `workflow_dispatch` referencing an existing normalized SemVer tag.

For CalcNova 2.8.03, use `v2.8.3`.

The validation job follows this order:

1. check out workflow source with full tag history;
2. validate requested tag syntax;
3. verify the exact tag exists;
4. detach at the requested tag;
5. read the normalized `<Version>` from `Directory.Build.props`;
6. require the tag to equal `v` plus that source version;
7. run tagged source preflight;
8. set up .NET 10;
9. restore, format-check, build, and test the tagged source.

Desktop, Browser, Android, and release-publication jobs all check out the release ref rather than branch head.

The Desktop job publishes six self-contained architecture-specific archives:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`.

The release workflow validator requires all six target/runner pairs, the RID-specific archive naming contract, and the RID-specific artifact naming contract. This prevents a maintenance edit from silently dropping native ARM64 or x64 desktop publication.

## Release workflow permissions

The release workflow defaults to read-only repository contents permission:

```yaml
permissions:
  contents: read
```

Only `publish-release` receives the privileges needed to create/update the GitHub Release and generate provenance attestations:

```yaml
permissions:
  contents: write
  id-token: write
  attestations: write
```

This keeps release-write and OIDC privileges away from validation/build jobs.

`tools/validate_release_workflow.py` requires this least-privilege structure and rejects permission drift that would grant those write permissions more broadly.

## Source-owned version identity

`Directory.Build.props` is the release-version source of truth.

The Android release job intentionally does **not** replace:

- `ApplicationDisplayVersion` from the tag text;
- `ApplicationVersion` from `github.run_number`.

Android and iOS use the source-defined `2.8.03` display version and `20803` build code.

This prevents a rerun or tag-format difference from changing package identity.

## Package metadata contract

Current source identities are:

- common application identifier: `in.sanskar.calcnova` where supported;
- application display name: `CalcNova`;
- product display version: `2.8.03`;
- normalized package version: `2.8.3`;
- mobile application/build version: `20803`;
- desktop assembly: `CalcNova.Desktop`;
- browser assembly: `CalcNova.Browser`.

`tools/validate_packaging_metadata.py` cross-checks:

- central release version properties;
- Android/iOS project metadata;
- iOS launch metadata;
- Linux desktop/AppStream files;
- macOS plist template;
- Windows Appx/MSIX manifest template;
- signing-secret safety markers.

Windows/macOS templates remain parameterized where the native packaging format requires generated values.

## Completion-status contract

Run independently with:

```bash
python tools/validate_completion_status.py .
python -m unittest tools.tests.test_validate_completion_status
```

It protects:

- README completion status;
- project state;
- dated 2.8.03 changelog entry;
- completed roadmap;
- completed feature inventory;
- documentation index;
- final completion audit;
- versioning guide;
- live `what_changed.md` checkpoint;
- in-app About `Version 2.8.03 • Complete` label.

Historical records under `docs/history/` remain historical and do not define the current status.

## Settings migration gate

Preferences are schema-versioned. See [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

When a maintenance release changes settings, verify:

- current-schema round trips;
- supported older-schema migration;
- representative preference preservation;
- safe rejection of corrupt/unsupported future schemas;
- native and Browser storage behavior.

An older build must not silently overwrite settings created by an unsupported newer schema.

## Accessibility evidence gate

Source accessibility contracts are complete for 2.8.03. Runtime/device evidence is recorded separately in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).

Do not mark a runtime scenario PASS merely because focus styles, automation names, keyboard mappings, or validators exist in source.

Record:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

with enough target/tool context to reproduce the result where useful.

## Repository release checks

For the release commit/tag, confirm:

- `README.md` identifies version 2.8.03 as complete;
- `PROJECT_STATE.md` identifies version 2.8.03 as complete;
- `CHANGELOG.md` contains the dated 2.8.03 release entry;
- `docs/VERSIONING.md` maps 2.8.03 to normalized 2.8.3 / `v2.8.3`;
- `what_changed.md` contains the current maintenance checkpoint;
- package/version identifiers are consistent;
- source preflight is run or its evidence state is recorded;
- no release-critical placeholder implementation exists;
- generated packaging placeholders are resolved by the appropriate packaging step;
- no private signing material is tracked;
- dependency/security alerts are reviewed;
- CodeQL/dependency-review status is reviewed where applicable;
- privacy/security documentation matches dependencies/network behavior;
- contact/support links are correct;
- license/third-party notices are complete;
- release provenance/checksum behavior still satisfies the release workflow validator.

## Mathematical verification

For a release or maintenance patch affecting calculation behavior, verify the relevant matrix:

- arithmetic precedence;
- power associativity;
- decimal/large integers;
- scientific domain boundaries;
- exact rational boundaries;
- engineering-notation bounds;
- angle modes;
- programmer signed/base boundaries;
- fixed-unit identities;
- statistics degenerate cases;
- graph discontinuities and numerical workload limits.

## Interaction verification

For keyboard-capable targets, verify as applicable:

- calculator Enter/Escape/Backspace;
- top-row/numpad/printable mappings;
- selection/caret editing;
- Ctrl+PageUp/PageDown mode cycling;
- Ctrl+Home/End first/last navigation;
- graph arrow-key panning;
- graph numpad Add/Subtract zoom;
- graph Home reset and `F` fit;
- visible focus;
- onboarding shortcut suppression.

## Platform evidence

Record each target independently:

```text
Windows: PASS / FAIL / BLOCKED / NOT RUN
Linux: PASS / FAIL / BLOCKED / NOT RUN
macOS: PASS / FAIL / BLOCKED / NOT RUN
Android: PASS / FAIL / BLOCKED / NOT RUN
iOS: PASS / FAIL / BLOCKED / NOT RUN
Browser: PASS / FAIL / BLOCKED / NOT RUN
```

For desktop release evidence, record x64 and ARM64 separately when both architecture artifacts are evaluated.

Package metadata correctness and actual package execution are separate evidence layers.

## Signing

Signing credentials remain outside Git.

Use platform-appropriate secure local configuration or GitHub Actions secrets. Never print private-key content or signing passwords into logs.

The Android release workflow produces a signed AAB only when all required signing secrets are configured, and temporary keystore material is removed after use.

The iOS exact-tag simulator validation path is intentionally unsigned and does not claim App Store signing/provisioning.

## Release artifacts

Publish only artifacts built from the release tag through the documented workflow or an equivalent recorded process.

Current automated artifact families include:

- Windows x64 self-contained desktop archive;
- Windows ARM64 self-contained desktop archive;
- Linux x64 self-contained desktop archive;
- Linux ARM64 self-contained desktop archive;
- macOS Intel x64 self-contained desktop archive;
- macOS Apple Silicon ARM64 self-contained desktop archive;
- Browser bundle;
- Android AAB when signing secrets are configured;
- checksum material.

An iOS archive remains credential/provisioning dependent and is not represented by the unsigned simulator-validation workflow.

Do not publish debug builds as stable release artifacts.

## Checksums and provenance attestations

Release publication generates SHA-256 checksum material and then creates GitHub artifact provenance attestations before publishing the GitHub Release assets.

The attested subject set includes:

- all release ZIP archives;
- the Android AAB when present;
- `SHA256SUMS.txt`.

The workflow uses `actions/attest@v4`, not the older wrapper actions. Provenance attestation binds artifacts to GitHub workflow/repository/commit identity; it does not claim that the artifact is vulnerability-free.

See [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md) for verification guidance.

A downloaded artifact can be checked with a current GitHub CLI installation using:

```bash
gh attestation verify PATH_TO_ARTIFACT -R sanskarIN/CalcNova
```

Checksum verification remains useful as a separate integrity check. Verify both the provenance of the release material and the expected checksum when high-assurance distribution verification is required.

CalcNova also includes manifest generation/verification tooling that binds structured artifact evidence to repository/commit identity.

## GitHub Release behavior

The workflow:

- creates a GitHub Release only if one does not already exist;
- preserves existing release notes/history on rerun;
- generates checksums before provenance attestations;
- attests intended ZIP/AAB/checksum release files before upload;
- uploads intended packaged artifacts with `--clobber`;
- does not delete/recreate the release as a normal rerun strategy.

## Release notes

Release notes should identify:

- product version `2.8.03`;
- normalized tag `v2.8.3`;
- major capabilities;
- important fixes;
- platform changes;
- security changes where relevant;
- migration notes where relevant;
- documentation links;
- known runtime/security-service evidence limitations, if any.

Do not claim universal compatibility, zero defects, or successful provenance generation without observed evidence.

## Maintenance / hotfix process

For a post-2.8.03 defect:

1. reproduce and scope impact;
2. add regression coverage where practical;
3. fix the issue;
4. run applicable source/compiled/platform/security checks;
5. update changelog/release notes;
6. issue the appropriate normalized SemVer maintenance tag/version when publication is required.

Avoid destructive repository history rewrites for ordinary release corrections.
