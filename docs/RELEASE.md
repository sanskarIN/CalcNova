# CalcNova Release Process

CalcNova releases must reflect actual validated repository state. A version number or source commit alone does not make a build production-ready.

## Versioning

The project intends to use semantic versioning:

```text
MAJOR.MINOR.PATCH
```

Pre-release identifiers may be used for alpha/beta/release-candidate builds.

Suggested development milestones are guidance, not obligations. Version boundaries may move when implementation or validation reality changes.

## Pre-release quality gate

Before tagging a release, verify at minimum:

```bash
python tools/validate_packaging_metadata.py .
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Then run the target-specific builds required for that release.

The package-metadata validator is intentionally SDK-independent. It checks repository identity/version contracts and structured release metadata, but it does **not** prove that an Android, iOS, Windows, Linux, macOS, or Browser package can be built, signed, installed, or accepted by a store.

A target that was not available must be listed as `NOT RUN`; it must not be presented as validated.

## Packaging metadata contract

The current release-layer metadata uses these source identities:

- common mobile/package identifier: `in.sanskar.calcnova` where the platform format supports it;
- application display name: `CalcNova`;
- development mobile display version: `0.1.0-dev`;
- current mobile application/build version: `1`;
- desktop assembly: `CalcNova.Desktop`;
- browser assembly: `CalcNova.Browser`.

`tools/validate_packaging_metadata.py` cross-checks Android/iOS project metadata, iOS launch metadata, the Linux desktop/AppStream files, the macOS plist template, and the Windows Appx/MSIX manifest template. The dedicated `Packaging Metadata Validate` workflow runs this preflight when relevant files change.

Release-time values such as the macOS version/build placeholders and Windows publisher/MSIX version placeholders must be resolved by the release process. Do not commit a real signing identity, certificate password, keystore password, private key, provisioning profile, or other signing secret just to satisfy a package template.

## Repository checks

Before release:

- `README.md` matches actual features;
- `PROJECT_STATE.md` is current;
- `what_changed.md` is current;
- `CHANGELOG.md` has the release changes;
- version/package identifiers are consistent;
- no release-critical placeholder implementation remains;
- release template placeholders are intentionally resolved only in generated release artifacts;
- no secrets/signing material are tracked;
- dependency alerts are reviewed;
- privacy/security docs match dependencies and network behavior;
- support/contact links are correct;
- license and third-party notices are complete;
- accessibility limitations are documented;
- known defects/limitations are disclosed.

## Mathematical validation

For a release affecting calculation behavior, verify the relevant manual/automated matrix:

- arithmetic precedence;
- power associativity;
- decimal/large integers;
- scientific domain boundaries;
- angle modes;
- programmer signed/base boundaries;
- fixed-unit identities;
- graph discontinuities/workload limits when graphing is included;
- advanced module degenerate cases when included.

## Platform validation

Record each target separately:

```text
Windows: PASS / FAIL / NOT RUN
Linux: PASS / FAIL / NOT RUN
macOS: PASS / FAIL / NOT RUN
Android: PASS / FAIL / NOT RUN
iOS: PASS / FAIL / NOT RUN
Browser: PASS / FAIL / NOT RUN
```

Include relevant OS/toolchain versions in release evidence where useful.

Package metadata validation and platform validation are separate gates. For example, a correct Android application ID does not prove that an AAB was successfully produced or signed.

## Signing

Signing credentials must live outside Git.

Use platform-appropriate secure local configuration or GitHub Actions secrets. Never print signing passwords or private-key content into build logs.

## Release artifacts

Attach only artifacts built from the release commit/tag through a documented process.

Potential artifacts include:

- Windows publish/package;
- Linux publish/package;
- macOS app/package;
- Android APK/AAB;
- iOS archive where distribution rules allow;
- Browser/PWA bundle;
- checksums.

Do not publish debug builds as stable release artifacts.

## Tagging

After all required checks pass and documentation is final:

```text
v0.1.0
v0.2.0
...
v1.0.0
```

Tag only validated milestones.

## Release notes

Release notes should include:

- major additions;
- important fixes;
- mathematical behavior changes;
- platform changes;
- security changes when appropriate;
- known limitations;
- upgrade/migration notes;
- documentation links;
- optional support note.

Do not claim universal compatibility or zero bugs.

## Rollback / hotfix

For a release-blocking regression:

1. reproduce and scope impact;
2. decide whether to disable/rollback or patch;
3. add regression coverage;
4. fix and run required validation;
5. update changelog/release notes;
6. issue a patch release.

Avoid destructive repository history rewrites for normal release corrections.
