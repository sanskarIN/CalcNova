# CalcNova Versioning

## Current release

**CalcNova 1.0.0** is the public/product release version.

The current 1.0.0 release uses strict Semantic Versioning-compatible numeric components, so the public/product and .NET/NuGet versions are identical:

| Purpose | Value |
| --- | --- |
| Product/display version | `1.0.0` |
| .NET/NuGet package version | `1.0.0` |
| Normalized Git release tag | `v1.0.0` |
| Assembly version | `1.0.0.0` |
| File version | `1.0.0.0` |
| Informational version | `1.0.0` |
| Android/iOS numeric build code | `10000` |

## Why 1.0.0 follows the 2.x development numbers

CalcNova used 2.x version numbers throughout development and none of them were ever released: the repository has no release tag and no published artifact for any of them. Continuing from 2.9.7 would have asserted a release history that does not exist.

1.0.0 states the real position — this is CalcNova's first release — and it is the number that version comparators in package managers, app stores and software centres will rank correctly from here on.

The development record is preserved, not renumbered. [`CHANGELOG.md`](../CHANGELOG.md) keeps every earlier entry under a pre-release heading, and the checkpoints remain in [`releases/2.9.0.md`](releases/2.9.0.md), [`releases/2.9.5.md`](releases/2.9.5.md), [`releases/2.9.6.md`](releases/2.9.6.md) and [`releases/2.9.7.md`](releases/2.9.7.md).

Linux AppStream metadata is the one place the earlier numbers were removed rather than relabelled: a software centre reading them would have offered users a 2.9.7 newer than the release they actually have.

## Source of truth

`Directory.Build.props` owns the shared release identity:

- `ProductDisplayVersion` = `1.0.0`;
- `Version`, `VersionPrefix`, and `PackageVersion` = `1.0.0`;
- `AssemblyVersion` and `FileVersion` = `1.0.0.0`;
- `InformationalVersion` = `1.0.0`.

Android and iOS use `$(ProductDisplayVersion)` for their visible application version and `10000` for the platform numeric build code.

## SDK-independent release identity contract

`tools/release_identity.py` parses the central build properties and fails closed when the release fields disagree.

It validates:

- display-version syntax;
- stable `MAJOR.MINOR.PATCH` SemVer package syntax;
- display-version normalization against `<Version>`;
- `VersionPrefix` and `PackageVersion` equality;
- assembly/file version equality to `<Version>.0`;
- informational version equality to the product display version;
- deterministic release tag `v<Version>`;
- deterministic mobile build code using `MAJOR * 10000 + MINOR * 100 + PATCH`.

The mobile calculation therefore gives:

```text
1.0.0 -> 10000
2.9.0 -> 20900
2.9.5 -> 20905
2.9.6 -> 20906
2.9.7 -> 20907
```

The 2.9 rows are retained because the formula has to keep producing the same codes for the checkpoints that recorded them; only the first row describes a release.

Minor and patch components above 99 are rejected by that mobile build-code contract instead of silently producing an ambiguous code.

## Release-tag contract

CalcNova's release workflow accepts strict `vMAJOR.MINOR.PATCH` Semantic Versioning tags. For this release the correct release tag is:

```text
v1.0.0
```

Before build and publication, the release workflow checks that the requested tag equals `v` plus the `<Version>` stored in `Directory.Build.props`. This prevents publishing a tag whose package version does not match the source tree.

## Mobile release identity

The Android and iOS projects use:

```text
ApplicationDisplayVersion = 1.0.0
ApplicationVersion = 10000
```

Release publishing does not override those values from the GitHub run number or from the tag. The source tree therefore remains the authoritative release identity.

Android currently declares source runtime identifiers:

```text
android-arm;android-arm64;android-x86;android-x64
```

iOS currently declares source runtime identifiers:

```text
ios-arm64;iossimulator-arm64;iossimulator-x64
```

## Packaging templates

Windows and macOS packaging templates remain parameterized because their package-generation steps have platform-specific version formats. Generated packages must use the release identity represented by this document while satisfying each platform's native version syntax.

Linux AppStream metadata lists exactly one stable entry, for the current 1.0.0 release, because no earlier version was ever published. The packaging validator requires exactly one stable AppStream entry for the current source display version and validates that entry's release date and description.

## Verification

Current SDK-independent checks include:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/release_preflight.py --tag v1.0.0
```

The packaging/completion/platform/release-document validators derive their current version expectations from `Directory.Build.props`, reducing the risk of a future release bump leaving hardcoded validator constants behind.
