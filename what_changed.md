# What Changed

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
