# What Changed

## CalcNova 1.0.0 first release — 2026-09-17

CalcNova is released as **1.0.0**, the project's first release.

### Current release identity

- Product/display version: `1.0.0`
- .NET/NuGet package version: `1.0.0`
- Release tag contract: `v1.0.0`
- Assembly/file version: `1.0.0.0`
- Android/iOS numeric build code: `10000`
- Application id: `in.sanskar.calcnova`
- In-app About label: `Version 1.0.0 • Complete`

### 1.0.0 release scope

CalcNova 1.0.0 ships the complete calculator feature set with the following contracts held from source:

- graph viewport accessibility is now represented by one stable eight-action toolbar contract: pan left/right/up/down, zoom in/out, reset, and fit-to-data;
- graph toolbar commands, labels, tooltips, target sizes, and focus behavior are validated from source;
- graph action localization includes English and Hindi semantic labels;
- adaptive-layout validation now tests the compact/medium/expanded contract and the current primary mode labels used by the XAML shell;
- the shared release-identity helper remains the source of truth for version-aware validators;
- release, packaging, completion-status, and platform-support validators derive their expected version from `Directory.Build.props` rather than hardcoding it;
- Android source composition explicitly inventories ARM, ARM64, x86, and x64 runtime identifiers;
- Android Release builds package both the Google Play app bundle and the universal APK bundletool derives from it, and the Android build workflow asserts both shapes;
- the release workflow always builds and publishes Android; the signing secrets choose the key and the asset names rather than deciding whether Android ships at all;
- iOS source composition explicitly inventories ARM64 device and ARM64/x64 simulator identifiers;
- platform workflow contracts remain aligned with the current GitHub Actions checkout/setup baseline;
- deterministic CycloneDX 1.7 SBOM generation, checksum/provenance controls, dependency-security validation, and CodeQL/Dependency Review/Dependabot coverage remain protected;
- the public README, project state, and live change log now identify 1.0.0 consistently.

### 1.0.0 release-identity contract

The release identity remains centralized in `Directory.Build.props`:

```text
ProductDisplayVersion = 1.0.0
Version = 1.0.0
VersionPrefix = 1.0.0
PackageVersion = 1.0.0
AssemblyVersion = 1.0.0.0
FileVersion = 1.0.0.0
InformationalVersion = 1.0.0
MobileBuildCode = 10000
ReleaseTag = v1.0.0
```

The release-identity regression suite protects the numeric build mapping:

```text
1.0.0 -> 10000
```

### 1.0.0 graph accessibility contract

The graph surface now has a stable, inspectable viewport action vocabulary:

```text
Pan left
Pan right
Pan up
Pan down
Zoom in
Zoom out
Reset viewport
Fit to data
```

Each action is keyboard-focusable and uses the shared 44-DIP minimum interaction-target baseline. The source contract also protects English/Hindi semantic labels and focus restoration after toolbar interaction.

### 1.0.0 cross-platform source contract

The maintained source/release matrix remains:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

No maintained platform head was removed during the 1.0.0 maintenance work.

### 1.0.0 validation updates

The source-preflight regression set now includes the adaptive-layout validator test with the current shell mode labels. Release validators remain SDK-independent and fail closed when the source contract is inconsistent.

The preferred current source gate is:

```bash
python tools/release_preflight.py --tag v1.0.0
```

Focused checks include:

```bash
python -m unittest tools.tests.test_release_identity
python -m unittest tools.tests.test_validate_adaptive_layout
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/validate_platform_support.py .
```

### 1.0.0 documentation synchronization

The following current-state documents were advanced to 1.0.0 on the maintenance branch:

- `README.md`
- `PROJECT_STATE.md`
- `what_changed.md`
- `docs/releases/1.0.0.md` (existing release checkpoint)


### 1.0.0 maintenance commits

- `docs: advance README to CalcNova 1.0.0`
- `docs: advance project state to CalcNova 1.0.0`
- `test: align adaptive layout mode fixture with current labels`
- `docs: advance live change log to CalcNova 1.0.0`

## Evidence policy

CalcNova 1.0.0 product scope: **COMPLETE**

The repository continues to enforce dependency-audit policy through:

```xml
<NuGetAuditMode>all</NuGetAuditMode>
```

Release artifact workflows use the protected metadata and asset paths:

```text
artifact-metadata: write
release-assets/**/*
```

Environment execution evidence is conservative. A build, device, signing, store, or hosted-service operation is not called PASS merely because its source contract exists. It is recorded as **PASS**, **FAIL**, **BLOCKED**, or **NOT RUN** only when the relevant evidence is actually observed.

When an external environment is unavailable, the correct state is `NOT RUN` or `BLOCKED`.

Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**
