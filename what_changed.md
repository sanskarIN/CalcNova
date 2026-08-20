# What Changed

## Release checksum manifest hardening — 2026-08-20

The stable release workflow received one additional integrity/usability fix after the NuGet-audit and attestation follow-up below.

### Downloaded checksum verification now matches GitHub Release filenames

Previously, checksum generation hashed files directly from the nested GitHub Actions download tree. That produced manifest entries containing runner-local paths such as:

```text
release-assets/desktop-win-x64/CalcNova-win-x64.zip
```

GitHub Release downloads are presented by flat filenames, so a user downloading the release assets into one directory could not reliably use a normal:

```bash
sha256sum -c SHA256SUMS.txt
```

The release workflow now writes checksum entries using each published asset basename, for example:

```text
<sha256>  CalcNova-win-x64.zip
```

### Release filename collision guard added

Before checksum generation, `publish-release` now:

- requires at least one prepared release file;
- rejects duplicate basenames across nested downloaded workflow artifacts;
- reserves `SHA256SUMS.txt` so no build artifact can collide with the generated manifest.

This matters because separate GitHub Actions artifacts can live in different subdirectories while GitHub Release assets ultimately share one flat filename namespace.

### Checksum/provenance ordering

The publication order is now:

1. download packaged workflow artifacts;
2. validate unique/reserved release filenames;
3. generate `SHA256SUMS.txt` using published basenames;
4. copy the checksum manifest into `release-assets/`;
5. attest the full `release-assets/**/*` tree with `actions/attest@v4`;
6. create/reuse the GitHub Release;
7. upload the prepared assets.

The checksum manifest is not included in its own checksum set, but it **is** covered by artifact provenance after being copied into the attested release tree.

### Regression and documentation protection

Updated:

- `.github/workflows/release.yml`;
- `tools/validate_release_workflow.py`;
- `tools/tests/test_validate_release_workflow.py`;
- `tools/validate_release_docs.py`;
- `tools/tests/test_validate_release_docs.py`;
- `docs/ARTIFACT_PROVENANCE.md`;
- `docs/README.md`;
- `CHANGELOG.md`;
- this live handoff record.

The release workflow validator now requires the filename-validation step, flat basename checksum generation, correct ordering, and rejects the previous nested-path `xargs -0 sha256sum > SHA256SUMS.txt` implementation.

The release-document validator now also protects the current security-automation and artifact-provenance guides, including the NuGet transitive-audit and release-attestation contracts.

### Evidence status

No release execution PASS is inferred from these source changes. Actual checksum validation, provenance generation, GitHub Release publication, .NET restore/build/test, CodeQL, Dependency Review, signing, and runtime results remain evidence only after those operations actually execute and are observed.

### Version/status unchanged

- Product/display version: `2.8.03`
- Normalized package version: `2.8.3`
- Normalized release tag: `v2.8.3`
- Mobile build code: `20803`
- Application id: `in.sanskar.calcnova`
- Product scope: **COMPLETE**
- This change: **POST-COMPLETION RELEASE-INTEGRITY MAINTENANCE**

## NuGet audit and attestation compatibility follow-up — 2026-08-20

This follow-up supersedes the security/provenance implementation details in the earlier 2026-08-20 checkpoint below while preserving that checkpoint as historical context.

### NuGet vulnerability auditing is now explicit and enforced

`Directory.Build.props` now defines:

```xml
<NuGetAudit>true</NuGetAudit>
<NuGetAuditMode>all</NuGetAuditMode>
<NuGetAuditLevel>moderate</NuGetAuditLevel>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

This makes CalcNova's .NET 10 restore path explicitly audit direct and transitive NuGet dependencies and keeps the enforced threshold aligned with the Dependency Review gate. When configured audit sources actually report a moderate, high, or critical advisory, the corresponding audit warning is expected to fail restore because warnings are treated as errors.

Added:

- `tools/validate_dependency_security.py`;
- `tools/tests/test_validate_dependency_security.py`.

The validator protects the audit-enabled state, transitive `all` mode, `moderate` threshold, warnings-as-errors enforcement, duplicate-policy drift, and `NU1901`–`NU1904` suppression through `NoWarn` / `WarningsNotAsErrors`, including composite warning lists.

Unrelated warning configuration remains allowed.

### Focused security validation strengthened

`.github/workflows/security-automation-validate.yml` now watches `Directory.Build.props` for both pushes and pull requests and runs:

```text
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python -m unittest tools.tests.test_validate_security_workflows
python -m unittest tools.tests.test_validate_dependency_security
```

`tools/validate_security_workflows.py` now also validates that focused workflow itself, including read-only permissions, path coverage, commands, and rejection of `pull_request_target`/unnecessary write permissions.

Both security validators and both regression suites are integrated into `tools/release_preflight.py`, and the preflight inventory tests require them.

### Final release-attestation contract corrected

The stable release workflow's current publication permissions are:

```yaml
permissions:
  contents: write
  id-token: write
  attestations: write
  artifact-metadata: write
```

Only `publish-release` receives those permissions. The workflow-level default remains:

```yaml
permissions:
  contents: read
```

The current `actions/attest@v4` step uses one inclusive subject:

```text
release-assets/**/*
```

This covers every prepared release asset, including desktop/Browser ZIP archives, `SHA256SUMS.txt`, and the Android AAB when signing secrets produce it, without requiring a separate potentially absent Android path.

`tools/validate_release_workflow.py` and `tools/tests/test_validate_release_workflow.py` now lock the four-permission publication contract, inclusive subject glob, ordering, and single-job privilege grants.

### Documentation synchronized

Updated current-facing documentation includes:

- `PROJECT_STATE.md`;
- `CHANGELOG.md`;
- root `SECURITY.md`;
- `docs/SECURITY.md`;
- `docs/SECURITY_AUTOMATION.md`;
- `docs/SOURCE_PREFLIGHT.md`;
- `docs/BUILDING.md`;
- `docs/RELEASE.md`;
- `docs/ARTIFACT_PROVENANCE.md`;
- this live `what_changed.md` record.

`PROJECT_STATE.md` now records the four attestation permissions and inclusive release-tree subject rather than the earlier three-permission snapshot.

### Evidence status

No execution result was fabricated. The available commit-status surface returned no legacy status records for the checked maintenance commits, which is not interpreted as either CI success or CI failure.

The assistant environment still did not provide a materialized .NET 10 repository execution path for the full restore/build/test suite. Therefore:

- security policy/source contracts are present on `main`;
- GitHub-hosted CodeQL/Dependency Review execution remains separately observable service evidence;
- online NuGet advisory-query success remains separate restore evidence;
- artifact-attestation execution remains separate release-service evidence;
- compiled/runtime/signing/store evidence remains environment dependent.

Use `PASS / FAIL / BLOCKED / NOT RUN` only from actual observed execution.

### Version/status unchanged

- Product/display version: `2.8.03`
- Normalized package version: `2.8.3`
- Normalized release tag: `v2.8.3`
- Mobile build code: `20803`
- Application id: `in.sanskar.calcnova`
- Product scope: **COMPLETE**
- This follow-up: **POST-COMPLETION SECURITY / DEPENDENCY / RELEASE-PROVENANCE MAINTENANCE**

## Security automation and release provenance maintenance — 2026-08-20

CalcNova 2.8.03 remains the completed product baseline. This continuation strengthened automated security review, release least privilege, and supply-chain provenance without changing the public version, normalized package version, release-tag mapping, application id, or mobile build code.

### Automated security gates added

The maintained `main` branch now contains:

- `.github/workflows/codeql.yml` — C# CodeQL analysis on pushes and pull requests to `main`, a weekly schedule, and manual dispatch;
- `.github/workflows/dependency-review.yml` — pull-request dependency review using `actions/dependency-review-action@v5` with `fail-on-severity: moderate`;
- `.github/workflows/security-automation-validate.yml` — focused read-only validation for the security workflow contracts;
- the existing `.github/dependabot.yml` — weekly NuGet and GitHub Actions update proposals.

CodeQL uses `github/codeql-action/init@v4` and `github/codeql-action/analyze@v4` for `csharp` with `build-mode: none`.

### Security workflow source contract added

Added:

- `tools/validate_security_workflows.py`;
- `tools/tests/test_validate_security_workflows.py`.

The validator protects:

- CodeQL push/PR/schedule/manual triggers;
- CodeQL Action major v4;
- C# language selection and source-analysis build mode;
- CodeQL result-publication permission without unnecessary repository/OIDC write privileges;
- Dependency Review Action major v5;
- moderate-or-higher vulnerability enforcement;
- read-only dependency-review permissions;
- rejection of `pull_request_target` for these workflows;
- rejection of unnecessary write/OIDC/package permissions.

The security validator and regression suite are integrated into `tools/release_preflight.py`, and the preflight inventory tests require that integration.

### Release least privilege hardened

`.github/workflows/release.yml` now defaults to:

```yaml
permissions:
  contents: read
```

Only `publish-release` receives:

```yaml
permissions:
  contents: write
  id-token: write
  attestations: write
```

Validation and build jobs therefore do not inherit release-write or OIDC privileges.

### Release provenance added

The release publication job now uses `actions/attest@v4` after checksum generation and before GitHub Release asset upload.

The attested release subject set includes:

- all packaged release ZIP files;
- the Android AAB when signing secrets produce one;
- `SHA256SUMS.txt`.

`tools/validate_release_workflow.py` now requires the provenance action, subject paths, ordering, global read-only permission, job-scoped publication permissions, and exactly one grant of each publication write/OIDC/attestation permission. It also rejects deprecated provenance-wrapper action references in the release workflow.

`tools/tests/test_validate_release_workflow.py` locks the same provenance and permission contract.

### Documentation synchronized

Added:

- `docs/SECURITY_AUTOMATION.md`;
- `docs/ARTIFACT_PROVENANCE.md`.

Updated:

- root `SECURITY.md`;
- `docs/SECURITY.md`;
- `docs/RELEASE.md`;
- `docs/SOURCE_PREFLIGHT.md`;
- `docs/README.md`;
- `CHANGELOG.md`;
- `PROJECT_STATE.md`;
- repository required-document validation;
- this live `what_changed.md` checkpoint.

The provenance guide documents `gh attestation verify PATH_TO_ARTIFACT -R sanskarIN/CalcNova`, checksum/provenance separation, least-privilege permissions, and offline-verification considerations.

### Evidence status

The assistant container still could not resolve `github.com` during the fresh-clone attempt, so a materialized local full-tree preflight and .NET build/test run were not executed there.

A checked maintenance commit exposed no legacy combined commit statuses through the available connector surface. That is not treated as proof that GitHub Actions passed or failed.

No CodeQL, Dependency Review, provenance-attestation, compiled, runtime, signing, or store-service PASS was invented. Their execution evidence remains `NOT RUN`/unobserved from this tool surface unless an actual service result is later retrieved.

The repository source contracts, workflow files, validator source, regression source, and documentation changes are present on `main`.

### Version/status unchanged

- Product/display version: `2.8.03`
- Normalized package version: `2.8.3`
- Normalized release tag: `v2.8.3`
- Mobile build code: `20803`
- Application id: `in.sanskar.calcnova`
- Product scope: **COMPLETE**
- This continuation: **POST-COMPLETION SECURITY / RELEASE-QUALITY MAINTENANCE**

## Native x64 + ARM64 desktop release maintenance — 2026-08-20

CalcNova 2.8.03 remains the completed product baseline. This continuation improved the release/distribution layer without changing the public version, normalized package version, release-tag mapping, application id, or mobile build code.

### Desktop release matrix expanded

`.github/workflows/release.yml` now publishes six self-contained desktop archives:

- Windows x64: `win-x64`;
- Windows ARM64: `win-arm64`;
- Linux x64: `linux-x64`;
- Linux ARM64: `linux-arm64`;
- macOS Intel: `osx-x64`;
- macOS Apple Silicon: `osx-arm64`.

Each RID keeps an independent `CalcNova-<rid>.zip` archive and `desktop-<rid>` workflow artifact. This improves first-class native release coverage for current ARM64 desktop systems instead of requiring architecture emulation where a native package can be produced.

### Release source contract hardened

`tools/validate_release_workflow.py` now requires:

- all six desktop runner/RID pairs;
- RID-specific self-contained publish output;
- RID-specific archive naming;
- RID-specific artifact naming;
- the existing exact-tag/source-version/preflight/release-safety contracts.

The validator's success message now explicitly covers x64/ARM64 desktop publication.

### Regression coverage expanded

`tools/tests/test_validate_release_workflow.py` now locks:

- the complete six-target desktop inventory;
- x64 + ARM64 coverage for Windows;
- x64 + ARM64 coverage for Linux;
- x64 + ARM64 coverage for macOS.

The existing repository-workflow validation test remains in place, so the real release workflow must satisfy the validator.

### Documentation synchronized

Updated current-facing documentation:

- `docs/BUILDING.md` — six release RIDs plus explicit publish commands for Windows/Linux/macOS x64 and ARM64;
- `docs/PLATFORM_SUPPORT.md` — native x64/ARM64 desktop release source contracts and architecture-specific evidence guidance;
- `docs/RELEASE.md` — six automated desktop artifact families and release-validator protections;
- `CHANGELOG.md` — post-2.8.03 maintenance record;
- `PROJECT_STATE.md` — authoritative current maintenance checkpoint;
- `what_changed.md` — this live continuation record.

### External support verification

Current Avalonia 12 documentation identifies x64 and ARM64 support across maintained Windows/macOS targets and x64/ARM64 support on representative maintained Linux distributions. The .NET runtime identifier catalog defines `win-arm64`, `linux-arm64`, and `osx-arm64` alongside their x64 equivalents.

That external platform information was used only to justify the source release targets. Actual CalcNova artifact execution remains evidence-based and must be recorded per target as `PASS`, `FAIL`, `BLOCKED`, or `NOT RUN`.

### Validation environment status

A fresh repository clone was attempted from the assistant container after this maintenance change. The container still could not resolve `github.com`, so a materialized full-tree local preflight could not run there.

No PASS result was invented. The repository now contains the strengthened release validator and regression source; CI/runtime/package execution remains separately observable evidence.

### Version/status unchanged

- Product/display version: `2.8.03`
- Normalized package version: `2.8.3`
- Normalized release tag: `v2.8.3`
- Mobile build code: `20803`
- Application id: `in.sanskar.calcnova`
- Product scope: **COMPLETE**
- This continuation: **POST-COMPLETION MAINTENANCE / RELEASE ENHANCEMENT**

## Documentation consistency pass — 2026-08-20

The maintained CalcNova documentation was audited against the completed 2.8.03 source tree. The product/version classification is unchanged: **CalcNova 2.8.03 remains COMPLETE**.

The authoritative record for this documentation pass is:

- `docs/DOCUMENTATION_AUDIT_2026-08-20.md`.

The documentation index now links that audit and provides a comprehensive current-guide map with explicit source-of-truth rules.

### Current-facing documentation corrected

This pass corrected stale development-era or environment-specific wording across:

- `docs/BUILDING.md`;
- `docs/README.md`;
- `docs/TROUBLESHOOTING.md`;
- `docs/ARCHITECTURE.md`;
- `docs/RUNTIME_VALIDATION_RUNBOOK.md`;
- `docs/PRIVACY.md`;
- `docs/SECURITY.md`;
- `docs/CALCULATION_ENGINE.md`;
- `docs/CALCULATOR_EDITING.md`;
- `docs/INPUT_SAFETY.md`;
- `docs/KEYBOARD_SHORTCUTS.md`;
- `docs/NUMERICAL_ANALYSIS.md`;
- `docs/BIVARIATE_STATISTICS.md`;
- `docs/EXACT_RATIONALS.md`;
- `docs/ENGINEERING_NOTATION.md`;
- `docs/CONVERTER_MODE.md`;
- `docs/PROGRAMMER_MODE.md`;
- `docs/ACCESSIBILITY.md`;
- `docs/DESIGN_SYSTEM.md`;
- `docs/LOCALIZATION.md`;
- `docs/LIVE_LOCALIZATION.md`;
- `docs/ONBOARDING.md`;
- `docs/UI_AUTOMATION.md`.

### Major reconciliations

- Desktop, Browser/WebAssembly, Android, and iOS are documented as present maintained source heads rather than future work.
- `BUILDING.md` now reflects the actual platform workflows, including Browser `wasm-tools`, Android workload + JDK 17, iOS workload/simulator RIDs, Android API 23, iOS 15.0, and current Desktop release RIDs.
- Android signed-AAB publication is documented with the current external CI secret contract without embedding credentials.
- Native SQLite history and Browser-safe settings/history composition are documented as current implementation.
- Optional network-enhanced currency provider/cache/offline behavior is documented consistently in architecture, privacy, and security guidance.
- Exact rational arithmetic, engineering notation, graph trace/multi-series/export, converter search/recents/favorites/copy, and programmer large-word grouping/copy are documented as completed 2.8.03 features rather than remaining work.
- English/Hindi semantic localization and reviewed live-localized onboarding/shared surfaces are documented as the completed 2.8.03 baseline; additional languages/detail migration remain optional enhancements.
- Shift-only top-row Calculator mappings for `+`, `*`, `(`, `)`, `^`, and `%` are documented as implemented rather than planned.
- Permanent feature documentation no longer freezes one assistant/environment's unavailable SDK state as a global `NOT RUN` status; execution evidence remains per-run and conservative.

### Evidence policy preserved

The documentation pass does **not** manufacture runtime results. Runtime/device/signing/store checks continue to use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Source completeness does not imply a runtime PASS, and an unexecuted runtime check does not redefine a completed source feature as unfinished.

### Historical records preserved

Dated 2026-08-19 continuation/source-audit records and `docs/history/` remain intact as historical evidence. Current authoritative documentation wins when a historical checkpoint describes an earlier implementation state.

### Product scope unchanged

No product-code behavior or release version was changed by this documentation pass.

- Product/display version: `2.8.03`
- Normalized package version: `2.8.3`
- Normalized release tag: `v2.8.3`
- Mobile build code: `20803`
- Application id: `in.sanskar.calcnova`
- Product/source scope: **COMPLETE**

## CalcNova 2.8.03 final completion checkpoint — 2026-08-19

**CalcNova version 2.8.03 is complete.**

This is the final live completion checkpoint for the defined 2.8.03 product baseline.

Historical source-hardening/continuation detail is preserved under `docs/history/`, including:

- `docs/history/what_changed_through_pre_2.8.03_completion_2026-08-19.md`;
- `docs/history/final_source_audit_pre_2.8.03_completion_2026-08-19.md`.

The authoritative completion audit is:

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`.

## Final release identity

- Product/display version: `2.8.03`
- Normalized .NET/NuGet version: `2.8.3`
- Normalized Git release tag: `v2.8.3`
- Assembly version: `2.8.3.0`
- File version: `2.8.3.0`
- Informational version: `2.8.03`
- Android/iOS display version: `2.8.03`
- Android/iOS numeric build code: `20803`
- Application id: `in.sanskar.calcnova`

Strict Semantic Versioning forbids leading zeroes in numeric version identifiers, so package/tag tooling uses normalized `2.8.3` / `v2.8.3` while CalcNova keeps the requested public product version `2.8.03`.

## Version source of truth

`Directory.Build.props` now centrally defines:

- `ProductDisplayVersion = 2.8.03`;
- `Version = 2.8.3`;
- `VersionPrefix = 2.8.3`;
- `PackageVersion = 2.8.3`;
- `AssemblyVersion = 2.8.3.0`;
- `FileVersion = 2.8.3.0`;
- `InformationalVersion = 2.8.03`.

Android and iOS use the shared display version plus numeric build code `20803`.

## Release workflow finalization

The release workflow now treats source version metadata as authoritative.

Before compiled validation/publication it:

1. validates strict tag syntax;
2. checks out the exact requested tag;
3. reads the normalized `<Version>` from `Directory.Build.props`;
4. requires `RELEASE_TAG = v + SOURCE_VERSION`;
5. runs tagged source preflight;
6. proceeds to .NET/platform publication steps only after those source checks.

For CalcNova 2.8.03 the normalized tag is `v2.8.3`.

The Android release path no longer replaces source-owned display/build versions from the tag text or GitHub run number.

## Mobile and packaging metadata

Android:

- `ApplicationDisplayVersion = $(ProductDisplayVersion)`;
- `ApplicationVersion = 20803`.

iOS:

- `ApplicationDisplayVersion = $(ProductDisplayVersion)`;
- `ApplicationVersion = 20803`.

Linux AppStream metadata now contains a stable dated release entry:

- version `2.8.03`;
- date `2026-08-19`;
- type `stable`;
- completed cross-platform baseline description.

`tools/validate_packaging_metadata.py` protects central/mobile/Linux/macOS/Windows identity and package metadata contracts, and its regression suite protects the 2.8.03 constants/AppStream release entry.

## In-app About release identity

`AboutViewModel` now exposes:

- `Version = 2.8.03`;
- `CompletionStatus = Complete`;
- `ReleaseLabel = Version 2.8.03 • Complete`.

The shared About surface displays that release label.

Added regression source:

- `tests/CalcNova.App.Tests/AboutReleaseIdentityTests.cs`;
- `tests/CalcNova.App.Tests/AboutReleaseIdentityHeadlessTests.cs`.

## Completed documentation state

Current-facing documentation now describes CalcNova 2.8.03 as complete.

Finalized documents include:

- `README.md` — completed product overview and 2.8.03 identity;
- `PROJECT_STATE.md` — authoritative complete classification;
- `CHANGELOG.md` — dated 2.8.03 release record;
- `docs/README.md` — 2.8.03 documentation index;
- `docs/FEATURES.md` — completed feature inventory;
- `docs/ROADMAP.md` — all defined 2.8.03 milestones complete;
- `docs/VERSIONING.md` — display/SemVer/build-code mapping;
- `docs/RELEASE.md` — 2.8.03 release process;
- `docs/RELEASE_READINESS_CHECKLIST.md` — release evidence checklist rather than an implementation-completion checklist;
- `docs/PLATFORM_SUPPORT.md` — completed platform source composition with external evidence recorded separately;
- `docs/SOURCE_PREFLIGHT.md` — 2.8.03 source gate and completion contract;
- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md` — final completion audit;
- `SECURITY.md` — 2.8.03 current completed/supported baseline;
- `SUPPORT.md` — 2.8.03 support/maintenance posture;
- `what_changed.md` — this final live checkpoint.

## Security/support status correction

The root security policy had retained an obsolete pre-release status statement.

It now identifies:

- CalcNova 2.8.03 as the current completed/supported baseline;
- normalized package/tag mapping;
- 2.8.03 security maintenance policy.

The Support guide now identifies 2.8.03 as the supported baseline and categorizes feature requests as optional post-release improvements unless they address correctness/security/compatibility defects.

## Completion-status source contract

Added:

- `tools/validate_completion_status.py`;
- `tools/tests/test_validate_completion_status.py`;
- `.github/workflows/completion-status-validate.yml`.

The completion validator protects current authoritative files against obsolete provisional-status wording and requires explicit 2.8.03 complete markers.

Protected documents include:

- root README/project state/changelog/live checkpoint/security/support;
- docs README/features/roadmap/final audit/versioning;
- release process/evidence checklist/platform support/source preflight.

It also protects the in-app About release identity and its regression source.

The validator rejects current-status phrases including:

- `under active development`;
- `active pre-release development`;
- an `Unreleased` top-level release posture;
- planned first-milestone wording;
- `remaining product/runtime work`;
- `remaining high-priority work`;
- `remaining work is evidence-dependent`.

Historical files under `docs/history/` remain preserved as historical records and do not define the current project state.

## Completion-focused workflow

`.github/workflows/completion-status-validate.yml` watches:

- central version metadata;
- authoritative completion/release/platform/security/support documentation;
- About release identity source/tests;
- Android/iOS version source;
- Linux AppStream release metadata;
- completion/packaging validators and tests.

The focused workflow runs:

```text
python tools/validate_completion_status.py .
python tools/validate_packaging_metadata.py .
python -m unittest tools.tests.test_validate_completion_status
python -m unittest tools.tests.test_validate_packaging_metadata
```

It uses read-only repository permissions.

## Integrated Source Preflight

`tools/release_preflight.py` now includes:

- the 2.8.03 completion-status validator;
- its Python regression suite.

`tools/tests/test_release_preflight.py` requires both entries so completion-status coverage cannot silently disappear.

The release-document validator was also updated from older tag/status wording to the 2.8.03 release/evidence contracts.

## Completed product scope

### Calculator and scientific — COMPLETE

Standard/scientific calculation, precedence, scientific functions/constants, angle modes, percentage, repeated equals, memory, sanitized import/paste/copy, keyboard mappings, and selection/caret-aware editing.

### Exact rational arithmetic — COMPLETE

Bounded canonical `BigInteger` rationals, exact decimal/scientific parsing, arithmetic/comparison, default-value safety, Calculator panel, tests, and source validation.

### Engineering notation — COMPLETE

Bounded engineering format/parse workflows, significant-digit formatting, finite exponent limits, non-zero-underflow rejection, shared 4,096-character input contract, Calculator panel, tests, and validation.

### Programmer and Unicode — COMPLETE

Base 2–36 tools, fixed-width signed/unsigned bitwise operations/shifts, 8/16/32/64/128-bit grids, accessible bit states, copy actions, Unicode scalar conversion/inspection, and local Unicode metadata.

### Converter, currency, and date/time — COMPLETE

Offline unit conversion, precision/search/recents/favorites/persistence/copy, converter preference/default contracts, replaceable currency provider/cache/offline fallback, and date/time/business-day/duration utilities.

### Statistics, equations, and matrices — COMPLETE

Descriptive statistics, paired covariance/correlation/regression/`R²`/prediction, deterministic edge handling, equation workflows, matrix determinant/inverse/rank/system solving, and copy workflows.

### Graphing/numerical analysis — COMPLETE

Bounded sampling, discontinuity handling, viewport interaction, multi-series presentation, non-color differentiation, trace/CSV/SVG, derivative/root/integration analysis, extreme-value safety, and workload budgets.

### History/settings/persistence/export — COMPLETE

Native/Browser storage composition, history operations, bounded exports/previews, settings schema/migration/validation, and preference persistence.

### Accessibility/adaptive/onboarding/localization baseline — COMPLETE

Interaction targets, focus/high-contrast behavior, adaptive profiles, keyboard navigation, dynamic-control accessibility, onboarding behavior, English/Hindi semantic catalogs, and reviewed live localized surfaces.

Additional language packs or translation expansion are optional post-release improvements.

### Platform source composition — COMPLETE

Desktop, Browser/WebAssembly, Android, and iOS source composition plus platform workflow/package contracts.

### Release/integrity/evidence infrastructure — COMPLETE

Integrated source preflight, focused validators/workflows, release-tag/workflow contracts, package metadata validation, artifact checksum/manifest tooling, and structured release-evidence schema/model/runner/verifier.

## Local validation attempt

A fresh clone was attempted from the assistant container to run the final integrated Python preflight against the materialized repository tree.

The container could not resolve `github.com`, so cloning stopped before the repository was materialized and the preflight command did not execute.

That attempt is recorded as an environment networking limitation, not as a CalcNova validation failure.

## Evidence policy

Product implementation completion and execution evidence remain intentionally separate.

A command/platform check is recorded as PASS only when it actually runs and the result is observed. When the required SDK/device/credential/tool/store service cannot be used in a particular environment, evidence is recorded as `NOT RUN` or `BLOCKED` instead of inventing success.

That does **not** make CalcNova 2.8.03 unfinished; it describes only whether a specific external verification operation executed in that environment.

## Final classification

- CalcNova 2.8.03 product scope: **COMPLETE**
- Core/domain implementation: **COMPLETE**
- Shared application: **COMPLETE**
- Cross-platform source composition: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**
- Current supported security baseline: **2.8.03**
- Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

## Handoff rule

Do not recreate completed 2.8.03 source work in later continuations. Continue only from a concrete observed defect, security/compatibility maintenance need, documentation correction, translation request, dependency update, test improvement, or explicitly requested optional enhancement.
