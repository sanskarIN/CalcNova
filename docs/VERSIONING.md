# CalcNova Versioning

## Current release

**CalcNova 2.9.7** is the public/product release version.

The current 2.9.7 release uses strict Semantic Versioning-compatible numeric components, so the public/product and .NET/NuGet versions are identical:

| Purpose | Value |
| --- | --- |
| Product/display version | `2.9.7` |
| .NET/NuGet package version | `2.9.7` |
| Normalized Git release tag | `v2.9.7` |
| Assembly version | `2.9.7.0` |
| File version | `2.9.7.0` |
| Informational version | `2.9.7` |
| Android/iOS numeric build code | `20907` |

Earlier 2.9-series checkpoints are preserved in [`releases/2.9.0.md`](releases/2.9.0.md), [`releases/2.9.5.md`](releases/2.9.5.md), and [`releases/2.9.6.md`](releases/2.9.6.md).

## Source of truth

`Directory.Build.props` owns the shared release identity:

- `ProductDisplayVersion` = `2.9.7`;
- `Version`, `VersionPrefix`, and `PackageVersion` = `2.9.7`;
- `AssemblyVersion` and `FileVersion` = `2.9.7.0`;
- `InformationalVersion` = `2.9.7`.

Android and iOS use `$(ProductDisplayVersion)` for their visible application version and `20907` for the platform numeric build code.

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
2.9.0 -> 20900
2.9.5 -> 20905
2.9.6 -> 20906
2.9.7 -> 20907
```

Minor and patch components above 99 are rejected by that mobile build-code contract instead of silently producing an ambiguous code.

## Release-tag contract

CalcNova's release workflow accepts strict `vMAJOR.MINOR.PATCH` Semantic Versioning tags. For this release the correct release tag is:

```text
v2.9.7
```

Before build and publication, the release workflow checks that the requested tag equals `v` plus the `<Version>` stored in `Directory.Build.props`. This prevents publishing a tag whose package version does not match the source tree.

## Mobile release identity

The Android and iOS projects use:

```text
ApplicationDisplayVersion = 2.9.7
ApplicationVersion = 20907
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

Linux AppStream metadata preserves stable entries for 2.8.03, 2.9.0, 2.9.5, 2.9.6, and the current 2.9.7 release. The packaging validator requires exactly one stable AppStream entry for the current source display version and validates that entry's release date and description.

## Verification

Current SDK-independent checks include:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/release_preflight.py --tag v2.9.7
```

The packaging/completion/platform/release-document validators derive their current version expectations from `Directory.Build.props`, reducing the risk of a future release bump leaving hardcoded validator constants behind.
