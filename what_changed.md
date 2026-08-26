# What Changed

## CalcNova 2.9.7 maintenance release preparation — 2026-08-26

CalcNova has been advanced from the preserved 2.9.6 checkpoint to the current **2.9.7** maintenance baseline.

### Current release identity

- Product/display version: `2.9.7`
- .NET/NuGet package version: `2.9.7`
- Release tag contract: `v2.9.7`
- Assembly/file version: `2.9.7.0`
- Android/iOS numeric build code: `20907`
- Application id: `in.sanskar.calcnova`
- In-app About label: `Version 2.9.7 • Complete`

### 2.9.7 maintenance scope

The 2.9.7 work preserves the completed calculator feature set and hardens the areas most likely to drift during release maintenance:

- graph viewport accessibility is now represented by one stable eight-action toolbar contract: pan left/right/up/down, zoom in/out, reset, and fit-to-data;
- graph toolbar commands, labels, tooltips, target sizes, and focus behavior are validated from source;
- graph action localization includes English and Hindi semantic labels;
- adaptive-layout validation now tests the compact/medium/expanded contract and the current primary mode labels used by the XAML shell;
- the shared release-identity helper remains the source of truth for version-aware validators;
- release, packaging, completion-status, and platform-support validators derive their expected version from `Directory.Build.props` rather than hardcoding historical release numbers;
- Android source composition explicitly inventories ARM, ARM64, x86, and x64 runtime identifiers;
- iOS source composition explicitly inventories ARM64 device and ARM64/x64 simulator identifiers;
- platform workflow contracts remain aligned with the current GitHub Actions checkout/setup baseline;
- deterministic CycloneDX 1.7 SBOM generation, checksum/provenance controls, dependency-security validation, and CodeQL/Dependency Review/Dependabot coverage remain protected;
- the public README, project state, and live change log now identify 2.9.7 consistently.

### 2.9.7 release-identity contract

The release identity remains centralized in `Directory.Build.props`:

```text
ProductDisplayVersion = 2.9.7
Version = 2.9.7
VersionPrefix = 2.9.7
PackageVersion = 2.9.7
AssemblyVersion = 2.9.7.0
FileVersion = 2.9.7.0
InformationalVersion = 2.9.7
MobileBuildCode = 20907
ReleaseTag = v2.9.7
```

The release-identity regression suite protects the numeric build mapping:

```text
2.9.0 -> 20900
2.9.5 -> 20905
2.9.6 -> 20906
2.9.7 -> 20907
```

### 2.9.7 graph accessibility contract

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

### 2.9.7 cross-platform source contract

The maintained source/release matrix remains:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

No maintained platform head was removed during the 2.9.7 maintenance work.

### 2.9.7 validation updates

The source-preflight regression set now includes the adaptive-layout validator test with the current shell mode labels. Release validators remain SDK-independent and fail closed when the source contract is inconsistent.

The preferred current source gate is:

```bash
python tools/release_preflight.py --tag v2.9.7
```

Focused checks include:

```bash
python -m unittest tools.tests.test_release_identity
python -m unittest tools.tests.test_validate_adaptive_layout
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/validate_platform_support.py .
```

### 2.9.7 documentation synchronization

The following current-state documents were advanced to 2.9.7 on the maintenance branch:

- `README.md`
- `PROJECT_STATE.md`
- `what_changed.md`
- `docs/releases/2.9.7.md` (existing release checkpoint)

Historical 2.9.6, 2.9.5, and 2.9.0 release checkpoints remain preserved rather than rewritten.

### 2.9.7 maintenance commits

- `docs: advance README to CalcNova 2.9.7`
- `docs: advance project state to CalcNova 2.9.7`
- `test: align adaptive layout mode fixture with current labels`
- `docs: advance live change log to CalcNova 2.9.7`

## Evidence policy

CalcNova 2.9.7 product scope: **COMPLETE**

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

## CalcNova 2.9.6 release preparation — 2026-08-24

CalcNova has been advanced from the preserved 2.9.5 checkpoint to the completed **2.9.6** source/release baseline.

### Previous 2.9 checkpoints preserved

Before the 2.9.6 bump, the completed 2.9.5 source/release state was preserved in `docs/releases/2.9.5.md`. The earlier requested 2.9.0 checkpoint remains preserved in `docs/releases/2.9.0.md`.

The release sequence remains auditable as:

```text
2.8.03 -> 2.9.0 -> 2.9.5 -> 2.9.6 -> 2.9.7
```

### Central version and mobile identities

The 2.9.6 baseline defined:

```text
ProductDisplayVersion = 2.9.6
Version = 2.9.6
VersionPrefix = 2.9.6
PackageVersion = 2.9.6
AssemblyVersion = 2.9.6.0
FileVersion = 2.9.6.0
InformationalVersion = 2.9.6
```

Android and iOS used visible version `2.9.6` and numeric build code `20906`.

### Version-aware source validation

The release preparation introduced/retained `tools/release_identity.py` as the SDK-independent source of current release expectations for packaging metadata, completion-status, cross-platform source, and release-document validation.

### Cross-platform source matrix

The maintained 2.9.6 matrix was Windows `win-x64`/`win-arm64`, Linux `linux-x64`/`linux-arm64`, macOS `osx-x64`/`osx-arm64`, Browser/WebAssembly/PWA, Android ARM/ARM64/x86/x64, and iOS ARM64 device plus ARM64/x64 simulator.

### Historical 2.9.6 continuation commits

- `03fa3210ea67df2525fe1c5a3326c29d20f720c0` — `docs: record CalcNova 2.9 series handoff`
- `c3fd3832b9cecd09668253f00f29d5c79aaf14ac` — `docs: preserve CalcNova 2.9.5 release checkpoint`
- `f16bf31c475125f7dad55dbfee6d5ba96d86d699` — `release: prepare CalcNova 2.9.6 identity`
- `46bd5d10a9c2f463e08d1222d05ad9f40519908a` — `release: set Android build code for 2.9.6`
- `4d818904d6889b02bf0ed3c494d02f6cbfcd65ea` — `release: set iOS build code for 2.9.6`
- `dea2f53a81ad7ba9c78475259ae3bace4f692cbe` — `release: show CalcNova 2.9.6 in About`
- `16deb005f60d492a2c6b4670e992fc0fe501eb2b` — `test: protect CalcNova 2.9.6 About identity`
- `8e53de2433dcd98c9b9bc4fe854ff69c6dc2dd68` — `test: protect visible CalcNova 2.9.6 identity`
- `470385e8abb0fcdbc4f6b2cbfdca8ffc6cdf4c16` — `test: set release identity baseline to 2.9.6`
- `60cae31229419ec8e5db54f1dfe021d70d2560e1` — `release: add CalcNova 2.9.6 AppStream entry`
- `b5857843b078fe06c62aecca877070f842fab2eb` — `test: align packaging fixture with 2.9.6`
- `3c26f054bf44f6cee2ee6ace43ccdb567f93e400` — `docs: support CalcNova 2.9.6 security baseline`
- `e71fdcc0488ef8e26c6d83f62af23462321f569c` — `docs: support CalcNova 2.9.6 baseline`
- `da21446b754491848feaed5ec425e48e1098999a` — `docs: advance contributor baseline to 2.9.6`
- `b5cf5e80b7a94920aa40afc4f8ce29823f6a18da` — `docs: define CalcNova 2.9.6 version mapping`
- `dfbe537d066b15070f75923deff8c232db420df3` — `docs: advance README to CalcNova 2.9.6`
- `a5b672bde6edf9ec3a13ef236cc44a9d3b38a93d` — `docs: advance project state to CalcNova 2.9.6`
- `1a80fca307c60d835533b346e00d5d999cf738a8` — `docs: advance documentation index to 2.9.6`
