# CalcNova Versioning

## Current release

**CalcNova 2.8.03** is the public/product release version.

The repository uses two equivalent representations because strict Semantic Versioning does not allow leading zeroes in numeric identifiers:

| Purpose | Value |
| --- | --- |
| Product/display version | `2.8.03` |
| .NET/NuGet package version | `2.8.3` |
| Normalized Git release tag | `v2.8.3` |
| Assembly version | `2.8.3.0` |
| File version | `2.8.3.0` |
| Informational version | `2.8.03` |
| Android/iOS numeric build code | `20803` |

`2.8.03` and `2.8.3` identify the same CalcNova release. The normalized form exists only where package/tag tooling requires SemVer-compatible numeric identifiers.

## Source of truth

`Directory.Build.props` owns the shared release identity:

- `ProductDisplayVersion` = `2.8.03`;
- `Version`, `VersionPrefix`, and `PackageVersion` = `2.8.3`;
- `AssemblyVersion` and `FileVersion` = `2.8.3.0`;
- `InformationalVersion` = `2.8.03`.

Android and iOS use `$(ProductDisplayVersion)` for their visible application version and `20803` for the platform numeric build code.

## Release-tag contract

CalcNova's release workflow accepts strict `vMAJOR.MINOR.PATCH` Semantic Versioning tags. For this release the correct release tag is:

```text
v2.8.3
```

The visually formatted tag `v2.8.03` is intentionally rejected because a leading zero in a SemVer numeric patch identifier is invalid.

Before build and publication, the release workflow checks that the requested tag equals `v` plus the normalized `<Version>` stored in `Directory.Build.props`. This prevents publishing a tag whose package version does not match the source tree.

## Mobile release identity

The Android and iOS projects use:

```text
ApplicationDisplayVersion = 2.8.03
ApplicationVersion = 20803
```

Release publishing does not override those values from the GitHub run number or from the normalized tag. The source tree therefore remains the authoritative release identity.

## Packaging templates

Windows and macOS packaging templates remain parameterized because their package-generation steps have platform-specific version formats. Generated packages must use the release identity represented by this document while satisfying each platform's native version syntax.

## Verification

The SDK-independent packaging validator checks the shared version properties and the mobile source metadata. Release-workflow validation also protects tag/source consistency and prevents publish-time display-version drift.
