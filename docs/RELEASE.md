# CalcNova Release Process

CalcNova releases must reflect actual validated repository state. A version number or source commit alone does not make a build production-ready.

## Versioning

CalcNova release tags use a `v`-prefixed Semantic Versioning form:

```text
vMAJOR.MINOR.PATCH
```

Examples:

```text
v0.1.0
v1.0.0
v1.2.0-rc.1
v1.2.0-rc.1+build.7
```

`tools/validate_release_tag.py` rejects malformed tags, missing `v` prefixes, incomplete versions, leading-zero numeric identifiers, and malformed prerelease/build identifiers. Its standard-library unit tests live in `tools/tests/test_validate_release_tag.py`.

Suggested development milestones are guidance, not obligations. Version boundaries may move when implementation or validation reality changes.

## Pre-release source gate

From the exact release-tag checkout, run the integrated SDK-independent source preflight first:

```bash
python tools/release_preflight.py --tag v0.1.0
```

The integrated preflight currently covers repository/security structure, XAML XML, shared UI/navigation, calculator/shell keyboard mappings, graph keyboard interaction, accessibility markup, visible focus, runtime-evidence discipline, adaptive layout, touch targets, English/Hindi localization catalogs, settings-schema migration, onboarding, package metadata, release documentation, release-tag validation, and regression tests for the source validators.

This command is the authoritative source-contract entry point. Focused validators remain independently runnable for diagnosis.

## .NET quality gate

After source preflight succeeds, run the compiled quality gate:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Then run the target-specific builds required for that release.

The Python validators are intentionally SDK-independent. They check repository release contracts, but they do **not** prove that an Android, iOS, Windows, Linux, macOS, or Browser package can be built, signed, installed, or accepted by a store.

A target that was not available must be listed as `NOT RUN`; it must not be presented as validated. Use the release evidence vocabulary `PASS / FAIL / BLOCKED / NOT RUN` and include enough environment detail to reproduce the result.

## Automated release flow

`.github/workflows/release.yml` supports both a pushed `v*` tag and manual `workflow_dispatch` with an existing tag name.

The workflow follows a tag-first safety contract:

1. fetch complete tag history;
2. validate the requested tag syntax;
3. verify that the tag exists in Git;
4. detach the validation job at that tag;
5. validate package metadata from the tagged source;
6. restore, format-check, build, and test the tagged source;
7. make every Desktop, Browser, and Android publish job check out the same release tag;
8. generate checksums for packaged artifacts;
9. create a GitHub Release only when one does not already exist;
10. on a rerun, preserve the existing release/notes and replace packaged assets with `--clobber` instead of deleting/recreating the release.

This prevents a manual release from accidentally building the branch head while publishing an older/different tag.

The release workflow does not create a missing tag automatically. Tag creation remains an explicit maintainer action after validation. Manual release dispatch must reference an already-existing valid tag; it must not be used as an implicit tag-creation mechanism.

## Packaging metadata contract

The current release-layer metadata uses these source identities:

- common mobile/package identifier: `in.sanskar.calcnova` where the platform format supports it;
- application display name: `CalcNova`;
- development mobile display version: `0.1.0-dev`;
- current mobile application/build version: `1`;
- desktop assembly: `CalcNova.Desktop`;
- browser assembly: `CalcNova.Browser`.

`tools/validate_packaging_metadata.py` cross-checks Android/iOS project metadata, iOS launch metadata, the Linux desktop/AppStream files, the macOS plist template, and the Windows Appx/MSIX manifest template. Its Python regression suite deliberately verifies the current identity constants and missing-metadata failure behavior.

Release-time values such as the macOS version/build placeholders and Windows publisher/MSIX version placeholders must be resolved by the release process. Do not commit a real signing identity, certificate password, keystore password, private key, provisioning profile, or other signing secret just to satisfy a package template.

## Settings migration gate

Preferences are schema-versioned. See [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

A release that changes settings must verify:

- current-schema round trips;
- every supported older-schema migration;
- representative preference preservation;
- safe rejection of corrupt or unsupported future schemas;
- native and Browser storage behavior.

An older build must not silently overwrite settings created by a newer unsupported schema.

## Accessibility evidence gate

Source accessibility checks do not replace runtime evidence. Use [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md) for Desktop, Browser, Android, and iOS evidence.

Do not mark a platform or accessibility scenario `PASS` merely because focus styles, automation names, keyboard mappings, or validators exist in source. Every PASS needs an observed target/runtime result.

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

## Interaction validation

For release-supported keyboard targets, verify:

- calculator Enter/Escape/Backspace and hardware-key mappings;
- Ctrl+PageUp/PageDown mode cycling;
- Ctrl+Home/End first/last mode navigation;
- graph arrow-key panning;
- graph numpad Add/Subtract zoom;
- graph Home reset and `F` fit-to-data;
- visible focus across representative controls;
- no background shortcut activation through onboarding.

Browser conflicts and assistive-technology interactions must be checked on actual target environments.

## Platform validation

Record each target separately:

```text
Windows: PASS / FAIL / BLOCKED / NOT RUN
Linux: PASS / FAIL / BLOCKED / NOT RUN
macOS: PASS / FAIL / BLOCKED / NOT RUN
Android: PASS / FAIL / BLOCKED / NOT RUN
iOS: PASS / FAIL / BLOCKED / NOT RUN
Browser: PASS / FAIL / BLOCKED / NOT RUN
```

Include relevant OS/toolchain versions in release evidence where useful.

Package metadata validation and platform validation are separate gates. For example, a correct Android application ID does not prove that an AAB was successfully produced or signed.

## Signing

Signing credentials must live outside Git.

Use platform-appropriate secure local configuration or GitHub Actions secrets. Never print signing passwords or private-key content into build logs.

The Android release workflow only produces a signed AAB when all required signing secrets are configured. Temporary signing material must be removed after use.

## Release artifacts

Attach only artifacts built from the release commit/tag through the documented release workflow or an equivalent recorded process.

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
