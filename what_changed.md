# What Changed

## CalcNova 2.9.6 release preparation — 2026-08-24

CalcNova has been advanced from the preserved 2.9.5 checkpoint to the current completed **2.9.6** source/release baseline.

### Current release identity

- Product/display version: `2.9.6`
- .NET/NuGet package version: `2.9.6`
- Release tag contract: `v2.9.6`
- Assembly/file version: `2.9.6.0`
- Android/iOS numeric build code: `20906`
- Application id: `in.sanskar.calcnova`
- In-app About label: `Version 2.9.6 • Complete`

### Previous 2.9 checkpoints preserved

Before this bump, the completed 2.9.5 source/release state was preserved in `docs/releases/2.9.5.md`. The earlier requested 2.9.0 checkpoint remains preserved in `docs/releases/2.9.0.md`.

The release sequence now remains auditable as:

```text
2.8.03 -> 2.9.0 -> 2.9.5 -> 2.9.6
```

Linux AppStream metadata also preserves stable entries for all four release identities.

### Central version and mobile identities advanced

`Directory.Build.props` now defines:

```text
ProductDisplayVersion = 2.9.6
Version = 2.9.6
VersionPrefix = 2.9.6
PackageVersion = 2.9.6
AssemblyVersion = 2.9.6.0
FileVersion = 2.9.6.0
InformationalVersion = 2.9.6
```

Android and iOS retain `$(ProductDisplayVersion)` for their visible version and now use numeric build code `20906`.

The release-identity regression suite now explicitly protects:

```text
2.9.0 -> 20900
2.9.5 -> 20905
2.9.6 -> 20906
```

### In-app release identity synchronized

`AboutViewModel` now exposes `2.9.6`, and both the direct view-model regression and Avalonia headless shell regression protect the visible `Version 2.9.6 • Complete` label.

### Version-aware source validation retained

The release preparation continues to use `tools/release_identity.py` as the SDK-independent source of current release expectations for:

- packaging metadata validation;
- completion-status validation;
- cross-platform source validation;
- release-document validation.

This means those validators follow `Directory.Build.props` rather than requiring a hand-edited hardcoded release constant for each future bump.

### Cross-platform source matrix retained

The maintained source/release matrix remains:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

No existing maintained platform head was removed during the 2.9.6 preparation.

### Current documentation synchronized

The current 2.9.6 baseline is synchronized across:

- `README.md`;
- `PROJECT_STATE.md`;
- `CHANGELOG.md`;
- `SECURITY.md`;
- `SUPPORT.md`;
- `CONTRIBUTING.md`;
- `docs/README.md`;
- `docs/FEATURES.md`;
- `docs/ROADMAP.md`;
- `docs/VERSIONING.md`;
- `docs/PLATFORM_SUPPORT.md`;
- `docs/SOURCE_PREFLIGHT.md`;
- `docs/RELEASE.md`;
- `docs/RELEASE_READINESS_CHECKLIST.md`;
- this live handoff.

### Commits in the 2.9.6 continuation

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
- `c428aad89f23a7f4b9cc57e49b28f3c9ac65fa48` — `docs: advance feature inventory to CalcNova 2.9.6`
- `23ca6906b82fdb52e35895283c35f216ce6d419b` — `docs: complete CalcNova 2.9.6 roadmap`
- `13470bf49806f80fa3a228523c827280ca26ecfa` — `docs: advance platform support to CalcNova 2.9.6`
- `b2e9dc28bb1c6221ff6a2256059d64e2ce1c2f24` — `docs: advance source preflight to CalcNova 2.9.6`
- `48470d476232868eebcc234208f4fd79a3bdbefb` — `docs: define CalcNova 2.9.6 release process`
- `d5c38788fc5dc604c60d79a78c0e45af1a230117` — `docs: advance release evidence checklist to 2.9.6`
- `965282c98b461f870f3352f63689010effd92507` — `docs: add CalcNova 2.9.6 release record`

### Evidence policy

Source/release preparation and execution evidence remain separate.

No hosted CI, .NET restore/build/test, runtime, physical-device, representative-browser, signing, notarization, TestFlight/App Store, Play Console, release-SBOM publication, checksum verification, or provenance PASS is inferred unless the operation actually runs and the result is observed.

Use only:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

for external execution evidence.

### Current remaining operational work

- observe hosted Source Preflight and focused platform/security/release workflow results;
- execute representative Windows/Linux/macOS x64/ARM64 artifacts;
- run representative Browser install/offline/storage/clipboard/accessibility checks;
- run Android emulator/physical-device smoke and accessibility checks;
- run iOS simulator/physical-device smoke and accessibility checks;
- perform Android signing/Play Console validation where credentials are available;
- perform macOS signing/notarization where the chosen distribution path requires it;
- perform iOS signing/provisioning/TestFlight/App Store validation where credentials are available;
- verify actual generated SBOMs, `SHA256SUMS.txt`, and provenance attestations from a real release execution.

CalcNova 2.9.6 product scope: **COMPLETE**

- Current release source identity: **2.9.6**
- Cross-platform maintained source matrix: **COMPLETE**
- Release identity consistency: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Runtime/signing/store evidence: **SEPARATE EXTERNAL EVIDENCE**
- Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

## CalcNova 2.9.0 → 2.9.5 release preparation — 2026-08-24

CalcNova advanced from the completed 2.8.03 baseline through a preserved 2.9.0 checkpoint and then to the completed 2.9.5 source/release baseline.

### Current release identity at that checkpoint

- Product/display version: `2.9.5`
- .NET/NuGet package version: `2.9.5`
- Release tag contract: `v2.9.5`
- Assembly/file version: `2.9.5.0`
- Android/iOS numeric build code: `20905`
- Application id: `in.sanskar.calcnova`
- In-app About label: `Version 2.9.5 • Complete`

### 2.9.0 prepared first and preserved

The repository was intentionally advanced to 2.9.0 first. That checkpoint used:

- display/package version `2.9.0`;
- tag contract `v2.9.0`;
- assembly/file version `2.9.0.0`;
- mobile build code `20900`.

The checkpoint is preserved in `docs/releases/2.9.0.md`, Linux AppStream history, and the changelog before the source tree was intentionally advanced to 2.9.5.

### Central release-identity infrastructure

Added `tools/release_identity.py` and regression coverage so SDK-independent tooling derives release identity from `Directory.Build.props` rather than duplicating hardcoded release constants.

The helper validates and derives:

- display version;
- normalized SemVer/package version;
- release tag;
- assembly/file version;
- mobile build code using `MAJOR * 10000 + MINOR * 100 + PATCH`.

Regression coverage explicitly protects `2.9.0 -> 20900` and `2.9.5 -> 20905` and fails closed when central version properties disagree.

### Release validators made current-version aware

The following source gates were converted away from hardcoded 2.8.03 release assumptions:

- `tools/validate_packaging_metadata.py`;
- `tools/validate_completion_status.py`;
- `tools/validate_platform_support.py`;
- `tools/validate_release_docs.py`.

Their regression suites now derive current release expectations from the central release identity. This prevents a valid future version bump from failing only because an SDK-independent validator retained an old release number.

A Python importlib/dataclass compatibility edge case in the new identity helper was also fixed so the repository's importlib-based regression harness can load it reliably.

### Cross-platform release identity synchronized

Android and iOS used build code `20905` while continuing to inherit the visible application version from `$(ProductDisplayVersion)`.

The maintained architecture/source contracts remained:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

Linux AppStream metadata preserved stable entries for 2.8.03, 2.9.0, and 2.9.5.

### Completion workflow modernized

`.github/workflows/completion-status-validate.yml` is no longer named for a single release. It uses the release-neutral name `CalcNova Current Release Completion Validate`, watches the shared release-identity helper and release checkpoints, runs release-identity tests before completion/package validation, limits pull requests to `main`, and cancels superseded runs.

### Current documentation synchronized at that checkpoint

The maintained current-facing baseline was advanced to 2.9.5 across README, project state, changelog, security/support/contributor policy, documentation index, feature inventory, roadmap, versioning, platform support, Source Preflight, release process, and release evidence checklist.

Historical 2.8.03 and 2.9.0 records remained historical instead of being rewritten as if those releases never existed.

### Major commits in the 2.9 preparation

- `5bc17a4cc7550b98fe04dc90e9404f4a72de6cfa` — `release: prepare CalcNova 2.9.0 identity`
- `148f8f3476fc4cd915735d22c00839cfb912ff3a` — `release: set Android build code for 2.9.0`
- `4bea94c3da1df50f533b265c4c8976ff1f7e27fc` — `release: set iOS build code for 2.9.0`
- `eade08ea6cf3a6258a8be3aad7713d38ab530f88` — `release: show CalcNova 2.9.0 in About`
- `c68a956b8a5c5ed0bdff5acb45824ae8807a1920` — `docs: record CalcNova 2.9.0 release checkpoint`
- `1f3a88d2cda38c446937e94281179a1ef4c50a7b` — `release: prepare CalcNova 2.9.5 identity`
- `7bece8a69a327a7b6c3b60556a950fdc242f10e0` — `release: set Android build code for 2.9.5`
- `0d4010d3dcce8969fe67840910b3500462f66f15` — `release: set iOS build code for 2.9.5`
- `6ee8dff80cf643b832c0c9515c09fe53c39641fc` — `release: show CalcNova 2.9.5 in About`
- `f0e568336d594acd8a1c333c978240c11e78978e` — `release: add CalcNova 2.9.5 AppStream entry`
- `3544ed263b8620c179861e4a367ed29512beb7c1` — `docs: advance project state to CalcNova 2.9.5`
- `e64cb22a2187e5b7b4e0924b4c44380b7124e5bd` — `docs: advance README to CalcNova 2.9.5`
- `c0e077cbd85fc02b368bbf7a58241a9f26116cf3` — `docs: define CalcNova 2.9.5 version mapping`
- `138408f20b04453d54afa4f9c744356b2246201f` — `docs: advance platform support to CalcNova 2.9.5`
- `759aee9c9f91c35a771c0315dffba24b02463e4c` — `docs: define CalcNova 2.9.5 release process`
- `c686ae3b08215b2f109326ce0e444ece7c6e2b9f` — `fix: make platform support validation version-aware`
- `270714a09b3e25c6da715fd4d616ee38799110ff` — `fix: make release documentation validation version-aware`
- `0a6060571fa56844342f10824095fc7415dd8f56` — `fix: keep release identity importlib-safe`
- `6c92b0a5d61b585438c3f032fffc0f64251f1370` — `docs: add CalcNova 2.9.0 and 2.9.5 releases`
- `65861a9e381c85e4bc75f20940678388b1915bcd` — `ci: make completion gate current-release aware`

### Evidence boundary and remaining operational work

Source/release preparation was complete for the 2.9.5 baseline. Hosted CI, .NET execution, downloaded artifacts, representative browsers, mobile devices, signing/notarization, store processing, and actual release supply-chain publication remained separately observed evidence.

No PASS was inferred for an operation that was not actually observed.

### 2.9.5 classification

- Product scope: **COMPLETE**
- Release source identity: **2.9.5**
- Cross-platform maintained source matrix: **COMPLETE**
- Version-aware release validation: **COMPLETE**
- Documentation synchronization: **COMPLETE**
- Runtime/signing/store evidence: **SEPARATE EXTERNAL EVIDENCE**

## Cross-platform source hardening — 2026-08-24

CalcNova 2.8.03 was the completed product baseline at this checkpoint. This continuation strengthened the maintained Windows, Linux, macOS, Browser/WebAssembly/PWA, Android, and iOS source contracts without changing that checkpoint's public product version or redefining completed calculator scope.

### Platform workflow validator drift fixed

A concrete post-maintenance defect was found in `tools/validate_platform_workflows.py`: the real platform workflows had already moved to `actions/checkout@v7`, while the validator still required `actions/checkout@v6`.

Because `tools/release_preflight.py` includes that validator, the stale marker could make the integrated source gate reject the newer valid workflows.

The validator now requires `actions/checkout@v7` for Desktop, Browser, Android, and iOS workflows. Its regression suite explicitly mutates a platform workflow back to checkout v6 and requires that drift to fail validation.

### Android architecture contract made explicit

`src/CalcNova.Android/CalcNova.Android.csproj` declared:

```xml
<RuntimeIdentifiers>android-arm;android-arm64;android-x86;android-x64</RuntimeIdentifiers>
```

The Android identity at that checkpoint was target framework `net10.0-android`, application id `in.sanskar.calcnova`, display version `2.8.03`, build code `20803`, and minimum supported Android platform source contract API 23.

The Android head continued to use app-local native storage, SQLite calculation history, JSON settings/currency cache, shared clipboard composition, and Android external-link integration.

### iOS device/simulator architecture contract made explicit

`src/CalcNova.iOS/CalcNova.iOS.csproj` declared:

```xml
<RuntimeIdentifiers>ios-arm64;iossimulator-arm64;iossimulator-x64</RuntimeIdentifiers>
```

The iOS identity at that checkpoint was target framework `net10.0-ios`, application id `in.sanskar.calcnova`, display version `2.8.03`, build code `20803`, and minimum supported iOS platform source contract iOS 15.0.

The iOS head continued to use native local-data storage with a documents fallback, SQLite calculation history, JSON settings/currency cache, shared clipboard composition, and iOS external-link integration.

### Cross-platform composition validator added

Added `tools/validate_platform_support.py` and `tools/tests/test_validate_platform_support.py` to protect maintained platform source itself rather than only workflow YAML.

The validator verifies Desktop/Avalonia composition, Browser/PWA resources and Browser-safe services, Android/iOS target/application/runtime metadata and native services, shared platform contracts, and the platform-support document.

### Focused platform-support CI gate added

Added `.github/workflows/platform-support-validate.yml` with read-only permissions, concurrency cancellation, checkout v7, Python 3.13, source validation, and regression execution.

### Integrated Source Preflight strengthened

`tools/release_preflight.py` gained cross-platform source validation and its regression suite, and `tools/tests/test_release_preflight.py` protects their presence.

### Platform-support documentation expanded

`docs/PLATFORM_SUPPORT.md` gained the maintained Windows/Linux/macOS x64/ARM64, Browser/PWA, Android four-RID, and iOS device/simulator matrix and separated source completeness from runtime evidence.

### Commits created in this continuation

1. `5ef60dd46b5678e329a94aa57b49fac049cbc2c9` — `fix: align platform validator with checkout v7`
2. `f4b9ac7d8fbd5b519d8dc6eccc6a04bd1d0a2922` — `build: declare Android architecture support`
3. `3197b6ae9d29a9be0b9a9cc2b383084193f87b82` — `build: declare iOS device and simulator RIDs`
4. `6431ed4da48a9293f73e655f4d06849615e3a6d7` — `ci: add cross-platform source contract validator`
5. `38e3c985c10be31dde8a26b15c24038020fb202d` — `test: cover cross-platform source contracts`
6. `7b9399d41bb8e6bc26f8add8513a238f2b982915` — `ci: add focused cross-platform validation gate`
7. `a098450603b1b49a437e001f8332ee16ccb47bd8` — `ci: integrate cross-platform contracts into preflight`
8. `acd199dfe9c1b08a9fb9095a06204fc05751c578` — `test: require cross-platform checks in preflight`
9. `33b83574217ffe28ad1fafb21fe9140d6d398de9` — `docs: define complete cross-platform support matrix`
10. `5fedaacee1a740e02bc70d26322430aab91aab5e` — `test: prevent platform checkout contract regressions`

### Evidence and operational follow-up

Source inspection confirmed the maintained Desktop, Browser, Android, iOS, and shared-platform files contained the markers required by the new validator. The solution intentionally kept platform-workload heads out of the general `CalcNova.slnx` build and validated Browser/Android/iOS through dedicated workflows.

The available legacy combined commit-status surface returned no statuses for the checked maintenance head. That was not interpreted as GitHub Actions success or failure.

No hosted build, physical-device, signed package, notarization, TestFlight/App Store, Play Console, or representative-browser PASS was inferred from source changes alone.

## CI hygiene and stale workflow cleanup — 2026-08-23

CalcNova 2.8.03 was the completed product baseline at this checkpoint. This continuation focused on post-completion CI correctness, repository hygiene, regression protection, governance documentation, and stale pull-request cleanup without changing the product version or redefining already completed feature scope.

### Canonical GitHub Actions upgraded

The active repository-wide workflows were updated to use `actions/checkout@v7`:

- `.github/workflows/build-test.yml`;
- `.github/workflows/format.yml`;
- `.github/workflows/docs-check.yml`.

Build and Test / Formatting continued to use `actions/setup-dotnet@v6` and the repository's .NET 10.0.x baseline.

### Obsolete GitHub starter workflows removed

Removed generic `.github/workflows/dotnet.yml` and `.github/workflows/dotnet-desktop.yml` starter templates that targeted unrelated .NET 8/WPF/MSIX flows and contained unresolved starter placeholders.

### CI hygiene source contract added

Added `tools/validate_ci_hygiene.py`, its tests, `.github/workflows/ci-hygiene-validate.yml`, and `docs/CI_HYGIENE.md` to protect canonical workflows, action majors, SDK baseline, read-only permissions, retired-template absence, and starter-placeholder absence.

### Integrated Source Preflight strengthened

The CI-hygiene validator and tests were integrated into `tools/release_preflight.py` and protected by the preflight inventory tests.

### Required documentation protection expanded

The documentation check made Source Preflight, branch protection, security automation, artifact provenance, CI hygiene, validation evidence, and release readiness mandatory non-empty repository documents.

### Branch-protection evidence refreshed

The GitHub branch metadata checked on 2026-08-23 reported branch protection disabled. That was recorded conservatively and no branch-protection PASS was claimed.

### Stale pull-request queue cleaned

Previously open stale/superseded CalcNova pull requests were reviewed and closed after supersession notes rather than being merged into the completed current baseline.

### Evidence status

A fresh local clone could not resolve `github.com`, so the materialized repository preflight and .NET restore/build/test sequence were not executed in that container. No hosted/runtime PASS was inferred from source presence.

## Deterministic CycloneDX release SBOM hardening — 2026-08-21

CalcNova 2.8.03 was the completed product baseline at this checkpoint. This continuation added deterministic per-platform release SBOM generation, integrated validation, checksum/provenance coverage, and current documentation.

### Deterministic CycloneDX generator added

Added `tools/generate_sbom.py` and `tools/tests/test_generate_sbom.py`.

The standard-library-only generator reads restored NuGet dependency graphs from `project.assets.json`, emits deterministic CycloneDX 1.7 JSON declaring:

```text
https://cyclonedx.org/schema/bom-1.7.schema.json
```

and records package names/versions, Package URLs, valid NuGet SHA-512 hashes, dependency edges, a deterministic UUIDv5 serial, and generator/assets-format metadata without a wall-clock timestamp.

### NuGet restore-format drift fails closed

The generator requires top-level assets format version `3` and expected `libraries`, `targets`, and `project` objects. Unsupported future format changes abort generation for explicit review.

### Release workflow publishes SBOMs beside packages

The release workflow produces per-RID Desktop SBOMs, a Browser SBOM, and an Android SBOM when signed Android publication is enabled. These files inherit flat filename collision protection, checksum inclusion, provenance attestation, and GitHub Release upload behavior.

### Release/source contracts strengthened

Release workflow/document/preflight validators and regression tests protect SBOM generation, filenames, ordering, schema identity, generator path, and supported NuGet assets format.

### Validation evidence

A focused Python generator smoke execution observed PASS for deterministic output, CycloneDX 1.7 identity, package/dependency inventory, hash conversion, and unsupported-format rejection. This did not substitute for full repository/release execution.

## Release checksum manifest hardening — 2026-08-20

The stable release workflow was corrected so `SHA256SUMS.txt` uses the flat basenames users actually download from a GitHub Release rather than runner-local nested paths.

A duplicate/reserved basename guard was added before checksum generation. The publication order became artifact download, filename validation, checksum generation, checksum copy into the release tree, provenance attestation, release creation/reuse, then asset upload.

No release execution PASS was inferred from these source changes.

## NuGet audit and attestation compatibility follow-up — 2026-08-20

The current repository-level dependency policy introduced here remains:

```xml
<NuGetAudit>true</NuGetAudit>
<NuGetAuditMode>all</NuGetAuditMode>
<NuGetAuditLevel>moderate</NuGetAuditLevel>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

The dependency-security validator/tests protect direct/transitive audit, severity threshold, warnings-as-errors, duplicate policy drift, and protected NU190x suppression markers.

The stable release publication permission contract introduced here remains:

```yaml
permissions:
  contents: write
  id-token: write
  attestations: write
  artifact-metadata: write
```

on `publish-release`, while the workflow-level default remains `contents: read`.

The current provenance subject remains:

```text
release-assets/**/*
```

No execution result was fabricated; service/runtime evidence remained separate.

## Security automation and release provenance maintenance — 2026-08-20

This checkpoint added CodeQL, dependency review, focused security validation, release least-privilege separation, artifact provenance, and related documentation/source validators.

No CodeQL, Dependency Review, provenance-attestation, compiled, runtime, signing, or store-service PASS was invented merely from source presence.

## Native x64 + ARM64 desktop release maintenance — 2026-08-20

This checkpoint expanded the release workflow to six self-contained Desktop archives:

- Windows `win-x64`, `win-arm64`;
- Linux `linux-x64`, `linux-arm64`;
- macOS `osx-x64`, `osx-arm64`.

The release validator and regression suite protect the complete six-target inventory and RID-specific naming.

Actual CalcNova artifact execution remains evidence-based per target.

## Documentation consistency pass — 2026-08-20

The maintained documentation was audited against the then-current completed 2.8.03 source tree. Platform heads, build requirements, storage composition, currency behavior, completed mathematical tools, localization, keyboard input, and evidence wording were reconciled with source.

Dated 2026-08-19 continuation/source-audit records and `docs/history/` remained intact as historical evidence.

## CalcNova 2.8.03 final completion checkpoint — 2026-08-19

**CalcNova version 2.8.03 is complete.**

This is the final live completion checkpoint for the defined 2.8.03 product baseline at that release point.

Historical source-hardening/continuation detail is preserved under `docs/history/`, including:

- `docs/history/what_changed_through_pre_2.8.03_completion_2026-08-19.md`;
- `docs/history/final_source_audit_pre_2.8.03_completion_2026-08-19.md`.

The authoritative completion audit is `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`.

## 2.8.03 release identity

- Product/display version: `2.8.03`
- Normalized .NET/NuGet version: `2.8.3`
- Normalized Git release tag: `v2.8.3`
- Assembly/file version: `2.8.3.0`
- Informational version: `2.8.03`
- Android/iOS display version: `2.8.03`
- Android/iOS numeric build code: `20803`
- Application id: `in.sanskar.calcnova`

Strict Semantic Versioning forbids leading zeroes in numeric version identifiers, so package/tag tooling used normalized `2.8.3` / `v2.8.3` while CalcNova kept the requested public product version `2.8.03`.

## Evidence policy

Product implementation completion and execution evidence remain intentionally separate.

A command/platform check is recorded as PASS only when it actually runs and the result is observed. When a required SDK/device/credential/tool/store service cannot be used in a particular environment, evidence is `NOT RUN` or `BLOCKED` instead of invented success.

## Historical 2.8.03 final classification

- CalcNova 2.8.03 product scope: **COMPLETE**
- Core/domain implementation: **COMPLETE**
- Shared application: **COMPLETE**
- Cross-platform source composition: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**

The current baseline is defined by the newest section at the top of this file and by `PROJECT_STATE.md`.
