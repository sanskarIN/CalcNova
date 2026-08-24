# Changelog

All notable CalcNova changes are recorded here.

## [2.9.6] - 2026-08-24

**Status: Complete**

Public/product version: `2.9.6`  
Package version: `2.9.6`  
Normalized release tag: `v2.9.6`  
Assembly/file version: `2.9.6.0`  
Mobile build code: `20906`

### Release preparation and identity

- Preserved the completed 2.9.5 source/release checkpoint in `docs/releases/2.9.5.md` before advancing current source.
- Advanced `Directory.Build.props` to 2.9.6 and retained the existing direct/transitive NuGet Audit policy.
- Set Android/iOS numeric build code to `20906` while preserving source-owned display-version inheritance.
- Updated the in-app About identity to `Version 2.9.6 • Complete`.
- Updated compiled/headless About regressions to protect the visible 2.9.6 identity.
- Updated release-identity regression expectations to `2.9.6`, `v2.9.6`, `2.9.6.0`, and `20906` while retaining checks for earlier 2.9-series build-code mappings.
- Added a stable Linux AppStream 2.9.6 entry while preserving 2.9.5, 2.9.0, and 2.8.03 history.

### Current baseline synchronization

- Advanced the maintained security baseline to 2.9.6 and marked 2.9.5 as superseded.
- Advanced support and contributor guidance to 2.9.6.
- Updated the README, authoritative project state, documentation index, feature inventory, roadmap, versioning guide, platform-support guide, Source Preflight guide, release process, and release evidence checklist to 2.9.6.
- Preserved earlier 2.9.0 and 2.9.5 checkpoint documents rather than rewriting them as current.
- Updated the live `what_changed.md` handoff so current status no longer stops at the earlier 2.8.03 maintenance record.

### Release-validation consistency

- Retained centralized release identity parsing through `tools/release_identity.py`.
- Retained version-aware packaging, completion-status, platform-support, and release-document validators.
- Retained integrated release-identity regression coverage in Source Preflight.
- Retained explicit Android ARM/ARM64/x86/x64 and iOS device/simulator RID contracts.
- Retained current-release completion validation through the release-neutral focused workflow.

### Security, supply chain, and cross-platform infrastructure retained

- Direct/transitive NuGet auditing at moderate-or-higher severity with warnings-as-errors.
- CodeQL and Dependency Review source automation contracts.
- Windows/Linux/macOS x64 + ARM64 Desktop release source matrix.
- Browser/WebAssembly/PWA composition.
- Deterministic CycloneDX 1.7 SBOM generation.
- Flat release filename collision protection and `SHA256SUMS.txt` generation.
- `actions/attest@v4` provenance over `release-assets/**/*`.
- Job-scoped release write/OIDC/attestation/artifact-metadata permissions.
- Structured PASS / FAIL / BLOCKED / NOT RUN release evidence model.

### Evidence boundary

This release record documents completed source/release preparation. Hosted CI, compiled .NET validation, downloaded package execution, representative Browser testing, Android/iOS device tests, signing, notarization, TestFlight/App Store, Play Console, SBOM/checksum publication, and provenance execution are PASS only after those operations actually run and are observed.

## [2.9.5] - 2026-08-24

**Status: Complete**

Public/product version: `2.9.5`  
Package version: `2.9.5`  
Normalized release tag: `v2.9.5`  
Assembly/file version: `2.9.5.0`  
Mobile build code: `20905`

### Release identity consistency

- Added `tools/release_identity.py` as the shared SDK-independent release identity parser.
- Added deterministic release tag, display-version normalization, assembly/file-version, and mobile build-code derivation.
- Defined the mobile build code as `MAJOR * 10000 + MINOR * 100 + PATCH`, with explicit tests for `2.9.0 -> 20900` and `2.9.5 -> 20905`.
- Made packaging validation derive current release/build expectations from `Directory.Build.props` instead of hardcoded 2.8.03 constants.
- Made completion-status validation derive current documentation, tag, build code, and About expectations from central release identity.
- Made cross-platform source validation derive the current Android/iOS build code rather than pinning `20803`.
- Made release-document validation derive current documentation/evidence markers from central release identity.
- Integrated release-identity regression coverage into the SDK-independent Source Preflight.
- Fixed importlib compatibility for the release-identity dataclass regression harness.

### Cross-platform maintenance

- Retained native x64/ARM64 Desktop release targets for Windows, Linux, and macOS.
- Retained Browser/WebAssembly/PWA source composition and offline resources.
- Retained explicit Android source runtime identifiers: `android-arm`, `android-arm64`, `android-x86`, `android-x64`.
- Retained explicit iOS source runtime identifiers: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.
- Strengthened the cross-platform source validator to check mobile build identity against central release metadata.
- Added regression coverage that rejects Android mobile build-code drift.
- Retained platform workflow contracts aligned with `actions/checkout@v7`.

### Current product identity at that checkpoint

- Advanced `Directory.Build.props` to 2.9.5.
- Set Android/iOS numeric build code to `20905`.
- Updated the in-app About identity to `Version 2.9.5 • Complete`.
- Updated About unit/headless regressions to protect the visible 2.9.5 identity.
- Added Linux AppStream stable release metadata for 2.9.5 while preserving 2.9.0 and 2.8.03 entries.
- Updated the maintained security/support baseline to 2.9.5 at that checkpoint.
- Updated README, project state, versioning, feature inventory, roadmap, platform support, Source Preflight, release process, release evidence checklist, contributor guidance, and documentation index to 2.9.5.
- Preserved this completed checkpoint in `docs/releases/2.9.5.md` before advancing to 2.9.6.

### Existing release/security infrastructure retained

- Direct/transitive NuGet auditing at moderate-or-higher severity with warnings-as-errors.
- CodeQL and Dependency Review source automation contracts.
- Deterministic CycloneDX 1.7 SBOM generation.
- Flat release filename collision protection and `SHA256SUMS.txt` generation.
- `actions/attest@v4` provenance over `release-assets/**/*`.
- Job-scoped release write/OIDC/attestation/artifact-metadata permissions.
- Structured PASS / FAIL / BLOCKED / NOT RUN release evidence model.

### Evidence boundary

This release entry records completed source/release preparation. Hosted CI, compiled .NET validation, downloaded package execution, Browser runtime checks, Android/iOS device tests, signing, notarization, TestFlight/App Store, and Play Console results remain separate evidence and are PASS only when actually executed and observed.

## [2.9.0] - 2026-08-24

**Status: Complete checkpoint; superseded by later 2.9 releases**

Public/product version: `2.9.0`  
Package version: `2.9.0`  
Normalized release tag: `v2.9.0`  
Assembly/file version: `2.9.0.0`  
Mobile build code: `20900`

### Prepared first as requested

- Advanced central release identity to 2.9.0 before beginning later 2.9-series preparation.
- Set Android/iOS build code to `20900`.
- Updated in-app About and compiled/headless release identity regressions to 2.9.0.
- Added a stable Linux AppStream 2.9.0 entry while preserving the 2.8.03 release entry.
- Established centralized release identity parsing and version-aware packaging/completion validation as the foundation for the 2.9 series.
- Preserved the full checkpoint in `docs/releases/2.9.0.md` before intentionally advancing current source.

## Post-2.8.03 maintenance - 2026-08-20

### Cross-platform release architecture

- Expanded the stable desktop release matrix from x64-only artifacts to native x64 and ARM64 self-contained archives for Windows, Linux, and macOS.
- Added `win-arm64`, `linux-arm64`, and `osx-arm64` alongside the existing `win-x64`, `linux-x64`, and `osx-x64` release targets.
- Kept every desktop architecture as an independent RID-specific archive/artifact so native packages remain unambiguous.
- Hardened `tools/validate_release_workflow.py` so all six desktop target/runner pairs and RID-specific archive/artifact contracts are required by source validation.
- Expanded release-workflow regression tests to lock the six-target inventory and require both x64 and ARM64 for each desktop operating system.
- Updated build, platform-support, and release documentation to distinguish source publication support from separately observed runtime/package evidence.

### Security automation and dependency auditing

- Added `.github/workflows/codeql.yml` for C# CodeQL scanning on pushes and pull requests to `main`, weekly scheduled scans, and manual runs.
- Added `.github/workflows/dependency-review.yml` to reject pull-request dependency changes that introduce known vulnerabilities at moderate severity or higher.
- Added `.github/workflows/security-automation-validate.yml` as a focused read-only contract-validation workflow.
- Added `tools/validate_security_workflows.py` and regression tests to protect CodeQL/Dependency Review action majors, triggers, language/build mode, vulnerability threshold, least-privilege permissions, rejection of `pull_request_target` drift, and the focused security workflow itself.
- Added explicit repository-level NuGet vulnerability auditing in `Directory.Build.props`: `NuGetAudit=true`, `NuGetAuditMode=all`, and `NuGetAuditLevel=moderate`.
- Kept `TreatWarningsAsErrors=true` so moderate-or-higher NuGet audit warnings fail restore/build gates when actually reported by the configured audit sources.
- Added `tools/validate_dependency_security.py` and regression tests to protect direct/transitive audit coverage, severity threshold, warnings-as-errors enforcement, duplicate-policy drift, and protected NU190x suppression markers.
- Updated the focused security workflow to watch `Directory.Build.props` and run both security validators plus both regression suites.
- Integrated security-workflow and dependency-security source validation into `tools/release_preflight.py` and its inventory tests.
- Added/updated `docs/SECURITY_AUTOMATION.md` and synchronized the security/preflight documentation.

### Release provenance and least privilege

- Changed the release workflow default permission from `contents: write` to `contents: read`.
- Scoped `contents: write`, `id-token: write`, `attestations: write`, and `artifact-metadata: write` only to the `publish-release` job.
- Added `actions/attest@v4` provenance generation for the prepared `release-assets/**/*` tree, covering desktop/Browser ZIP files, the Android AAB when present, and `SHA256SUMS.txt`.
- Used one inclusive release-tree subject so optional Android output remains conditional without requiring a separate potentially absent AAB path.
- Ordered provenance generation after checksum creation and before GitHub Release publication.
- Hardened `tools/validate_release_workflow.py` to enforce attestation action/subject/order plus single-job contents/OIDC/attestation/artifact-metadata permission grants.
- Expanded release-workflow regression tests to lock the current provenance and permission contract.
- Added `docs/ARTIFACT_PROVENANCE.md` with online/offline verification guidance and evidence semantics, then synchronized it with the current `actions/attest@v4` metadata-permission requirements.
- Updated the release/security/documentation index and made the security/provenance guides required repository documentation.

### Release checksum usability and collision safety

- Fixed `SHA256SUMS.txt` so entries use the flat basenames users actually download from a GitHub Release instead of GitHub Actions runner-local `release-assets/<artifact>/...` paths.
- Added a pre-publication duplicate-basename guard so two nested workflow artifacts cannot collapse to the same GitHub Release filename.
- Reserved `SHA256SUMS.txt` so a build artifact cannot collide with the generated checksum manifest.
- Required at least one prepared release asset before checksum generation.
- Kept the checksum manifest outside its own hash set, then copied it into `release-assets/` so the manifest itself is covered by provenance attestation.
- Hardened `tools/validate_release_workflow.py` and its regression tests to require flat basename checksum entries, filename validation ordering, and rejection of the old nested-path `xargs -0 sha256sum` implementation.
- Expanded release-document validation to protect the artifact-provenance and security-automation guides as current release contracts.
- Documented direct verification with `sha256sum -c SHA256SUMS.txt` after downloading release assets into one directory.

The product/display version remains `2.8.03` for this historical maintenance checkpoint; these were repository maintenance/security/release-quality enhancements before the later 2.9-series preparation.

## [2.8.03] - 2026-08-19

**Status: Complete**

Public/product version: `2.8.03`  
Normalized package version: `2.8.3`  
Normalized release tag: `v2.8.3`  
Mobile build code: `20803`

### Calculator

- Added the project-owned tokenizer, parser, evaluator, typed calculation errors, and workload limits.
- Added standard arithmetic, explicit precedence, right-associative exponentiation, parentheses, unary operators, decimal/scientific input, and result reuse.
- Added scientific functions, constants, degrees/radians/gradians, factorial, GCD/LCM, combinations/permutations, logarithmic/exponential functions, and trigonometric/hyperbolic functions.
- Added calculator percentage semantics, repeated-equals behavior, and MC/MR/MS/M+/M- memory operations.
- Added sanitized expression import, user-triggered paste, explicit copy result, top-row/numpad handling, and safe printable/shifted operator mappings.
- Added selection-aware editing, caret restoration, forward/reversed selection replacement, Backspace semantics, and selection-preserving function/parenthesis wrapping.

### Exact rational arithmetic

- Added canonical bounded `BigInteger` rational representation.
- Added exact parsing for integers, fractions, finite decimals, and decimal scientific notation.
- Added exact arithmetic, negation, reciprocal, comparison, equality, and hashing.
- Added multiplication cross-cancellation and denominator reduction.
- Added safe canonical behavior for `default(RationalNumber)`.
- Added a 4,096-character raw input bound enforced before trimming.
- Added 10,000 decimal scale/exponent and 65,536-bit reduced-value workload bounds.
- Added Calculator panel workflows, regression source, focused validation, and integrated preflight coverage.

### Engineering notation

- Added finite engineering-notation format/parse workflows with exponents divisible by three.
- Added 1–15 significant-digit formatting.
- Added canonical invariant-culture parsing.
- Added explicit `-324..306` engineering exponent bounds.
- Added rejection of non-zero inputs that would underflow to floating-point zero.
- Added extreme finite-value scaling safeguards.
- Added one 4,096-character input contract across core parser, Format action, and shared TextBox.
- Added Calculator panel, core/App/headless regression source, focused validation, and integrated preflight coverage.

### Programmer and Unicode

- Added base 2–36 parsing/formatting.
- Added binary/octal/decimal/hex synchronized representations.
- Added 8/16/32/64/128-bit signed/unsigned two's-complement workflows.
- Added AND/OR/XOR/NOT plus left/logical-right/arithmetic-right shifts.
- Added full interactive bit grids, byte grouping, accessible bit names, and representation copy actions.
- Added Unicode scalar/code-point conversion and bounded text inspection.
- Added local Unicode plane/general-category/UTF-8/UTF-16 metadata and metadata copy workflows without a network lookup.

### Conversion, date/time, and currency

- Added offline fixed-unit conversion across major physical/data categories.
- Added validated pairs, swapping, search, recent pairs, favorites, restoration, clear-recents, copy, and persisted converter preferences.
- Added selectable 1–17 significant-digit conversion precision.
- Added source contracts for converter defaults and persisted-preference/privacy behavior.
- Added replaceable currency provider/cache architecture with offline fallback and no embedded provider credentials.
- Added date-difference, calendar arithmetic, business-day, and fixed-duration utilities.

### Statistics, equations, and matrices

- Added descriptive statistics and summary copy.
- Added bounded paired X/Y parsing.
- Added population/sample covariance, Pearson correlation, ordinary least-squares regression, `R²`, and regression prediction when mathematically defined.
- Added deterministic handling of mismatched, non-finite, oversized, constant-X, constant-Y, and single-pair datasets.
- Added stale regression-state clearing after failed analysis.
- Added shared paired-statistics panel and source validation.
- Added equation-solving workflows.
- Added matrix determinant, inverse, rank, linear-system solving, and result copy.

### Graphing and numerical analysis

- Added workload-bounded graph sampling and discontinuity segmentation.
- Added explicit viewport model and focusable interactive Avalonia plot control.
- Added pointer pan/wheel zoom/double-click fit and keyboard pan/zoom/reset/fit controls.
- Added nearest sampled-point tracing.
- Added bounded single- and multi-expression CSV output.
- Added stable multi-series identities, deterministic non-color-only line patterns, and synchronized text legend.
- Added accessible SVG generation/copy.
- Added bounded derivative approximation, bisection root finding, and Simpson integration.
- Added extreme finite-value safety and explicit sampling/root/integration workload budgets.

### History, export, settings, and persistence

- Added native SQLite history behind an abstraction and Browser-safe storage.
- Added recent/search/favorite/delete/clear history workflows.
- Added bounded TXT/CSV/JSON export with bounded display previews and complete private copy payloads.
- Added UTF-16-safe preview boundaries and newline normalization.
- Added shared settings abstraction/view model.
- Added persisted converter/culture preferences.
- Added explicit settings schema version, legacy/unversioned migration, and fail-closed future-schema handling.
- Added shared native/Browser settings JSON decoding and validation.

### Accessibility, adaptive UI, onboarding, and localization

- Added 44-DIP minimum interaction-target and 54-DIP calculator-key baselines.
- Added visible focus styling and stronger CalcNova high-contrast focus styling.
- Added compact/medium/expanded layout profiles and compact overflow fallback.
- Added focus bring-into-view and Ctrl+PageUp/PageDown/Home/End mode navigation.
- Added accessible programmer bit-state naming and non-color graph differentiation.
- Added dynamic-control focus/touch-target contracts.
- Added high-contrast and reduced-motion preference state.
- Added onboarding focus/shortcut behavior.
- Added English and Hindi semantic catalogs for the current key set, regional culture selection, persisted preference, catalog validation, and live localization of reviewed surfaces.

### Platforms

- Added Desktop composition for Windows/Linux/macOS targets.
- Added Browser/WebAssembly composition.
- Added Android composition.
- Added iOS composition.
- Added shared clipboard/external-link abstractions and appropriate native/Browser storage composition.
- Added a stable Linux AppStream release entry for version `2.8.03` dated `2026-08-19`.

### Version 2.8.03 release identity

- Centralized product/display version `2.8.03` in `Directory.Build.props`.
- Set normalized .NET/NuGet version `2.8.3`.
- Set assembly/file version `2.8.3.0` and informational version `2.8.03`.
- Set Android/iOS display version from `ProductDisplayVersion`.
- Set Android/iOS numeric build code to `20803`.
- Updated package metadata validation to reject obsolete provisional mobile-version markers.
- Documented that strict SemVer uses normalized tag `v2.8.3`; `v2.8.03` is intentionally invalid under SemVer numeric rules.
- Added in-app About identity: `Version 2.8.03 • Complete`.
- Added view-model and headless-shell regression source for the About release identity.

### Validation and release infrastructure

- Added repository/security, XAML, UI, navigation, keyboard, calculator-editing, graph, Unicode, exact-rational, engineering, export, statistics, accessibility, localization, settings, onboarding, packaging, platform-workflow, release-workflow, artifact-integrity, and structured-evidence validators.
- Added Python regression tests for SDK-independent source validators/tooling.
- Added unified source preflight.
- Added Source Preflight workflow self-validation and broad path coverage.
- Added focused platform and feature validation workflows.
- Added tag-first release validation.
- Added exact-tag unsigned iOS simulator validation contract.
- Added release artifact checksum/manifest integrity infrastructure.
- Added structured PASS / FAIL / BLOCKED / NOT RUN evidence model, runner, verifier, and schema.
- Added release source-version consistency check so a release tag must equal `v` plus the normalized source `<Version>`.
- Removed Android release-time display/build version overrides so publication cannot drift from the source-owned 2.8.03 identity.
- Added `tools/validate_completion_status.py` and its regression suite to protect the completed 2.8.03 status across authoritative current-facing files and in-app About metadata.
- Integrated the completion-status validator and regression suite into the unified source preflight.
- Added the focused `CalcNova 2.8.03 Completion Validate` workflow with read-only permissions.
- Updated release-document validation to require the 2.8.03 release/evidence/versioning contracts.
- Expanded package validation to require the stable 2.8.03 Linux AppStream release entry.

### Documentation and support policy

- Completed architecture, build, testing, UI automation, troubleshooting, security, privacy, accessibility, adaptive layout, localization, platform, feature, numerical, converter, persistence, release, source-preflight, release-evidence, versioning, and audit documentation.
- Added the authoritative 2.8.03 completion state in `PROJECT_STATE.md`.
- Added `docs/VERSIONING.md` for public/display versus normalized SemVer mapping.
- Updated the root README to identify CalcNova 2.8.03 as complete.
- Closed the 2.8.03 roadmap and replaced provisional feature-status sections with a completed feature inventory.
- Converted release readiness documentation into a release **evidence** checklist so unexecuted environment checks are not described as missing product implementation.
- Updated platform support documentation to classify Desktop, Browser, Android, and iOS source composition as complete while recording runtime/device/signing results separately.
- Updated `SECURITY.md` so CalcNova 2.8.03 is the current completed and supported security baseline at that release point.
- Updated `SUPPORT.md` so feature requests were categorized as optional post-2.8.03 enhancements unless they addressed correctness, security, or compatibility.
- Updated `CONTRIBUTING.md` to describe the completed 2.8.03 baseline and contributor setup/maintenance work instead of a provisional project posture.
- Preserved earlier audit/continuation records under `docs/history/` for historical traceability.

### Evidence policy

CalcNova records an execution result as PASS only when the command or platform check actually ran and was observed. `NOT RUN`/`BLOCKED` records describe execution evidence in a particular environment; they do not change the completed implementation status of version 2.8.03.

A final fresh-clone attempt from the assistant container could not resolve `github.com`, so the materialized final-tree Python preflight did not execute there. This is recorded as an environment networking limitation rather than a CalcNova failure.

## Maintenance policy

CalcNova 2.9.6 is the current completed product baseline. Later repository changes may contain security fixes, compatibility maintenance, documentation corrections, translations, test improvements, dependency updates, evidence improvements, or optional features.
