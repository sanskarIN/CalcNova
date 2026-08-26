# CalcNova Project State

## Current Version

**2.9.7**

Normalized .NET/NuGet version: `2.9.7`  
Normalized release tag: `v2.9.7`  
Mobile numeric build code: `20907`

See [`docs/VERSIONING.md`](docs/VERSIONING.md).

## Current Branch

`main`

## Completion Status

**COMPLETE — CalcNova version 2.9.7**

The defined 2.9.7 product scope is implemented in the repository. Core calculation, scientific functions, exact rational arithmetic, engineering notation, programmer and Unicode tools, converter/date-time/currency utilities, descriptive and bivariate statistics, equations, matrices, graphing/numerical analysis, history, persistence, settings, onboarding, localization infrastructure and reviewed localized surfaces, accessibility/adaptive contracts, Desktop/Browser/Android/iOS composition, source validation, dependency vulnerability policy, artifact integrity, structured release evidence, packaging metadata, release workflows, security automation, release provenance controls, deterministic release SBOM generation, cross-platform source-contract validation, centralized release-identity validation, and the 2.9.7 graph-accessibility maintenance work are present as completed source capabilities.

Future repository changes are classified as maintenance, compatibility updates, security fixes, documentation changes, translation additions, or optional enhancements. They are not required to define the 2.9.7 project as complete.

## Current Maintenance Enhancements — 2026-08-25/26

CalcNova 2.9.7 is a maintenance release over the preserved 2.9.6 baseline. The release keeps the completed calculator feature set and strengthens the graph interaction/accessibility surface plus release-document consistency.

### Graph accessibility and interaction

- the shared graph surface exposes eight explicit viewport actions for pan-left, pan-right, pan-up, pan-down, zoom-in, zoom-out, reset, and fit-to-data;
- the toolbar is generated from a stable action definition list so labels, icons, tooltips, commands, and keyboard access remain synchronized;
- every toolbar action uses the shared 44-DIP minimum interaction target baseline;
- graph viewport focus is restored after toolbar interaction and keyboard navigation remains available;
- English and Hindi semantic labels are supplied for graph actions;
- graph action labels are included in localization completeness checks;
- focused headless/source validation covers toolbar count, command wiring, accessibility labels, target sizing, and focus restoration.

### Release identity consistency

- `tools/release_identity.py` remains the SDK-independent parser for `Directory.Build.props` release identity;
- `ProductDisplayVersion`, SemVer/package/version-prefix identity, assembly/file version, informational version, release tag, and mobile build code are validated as one contract;
- mobile build code is derived as `MAJOR * 10000 + MINOR * 100 + PATCH`, producing `20907` for 2.9.7;
- release validators derive current-version expectations instead of retaining hardcoded historical release constants;
- the 2.9.0, 2.9.5, and 2.9.6 checkpoints remain preserved in release documentation;
- current release identity is `2.9.7` / `v2.9.7` / `20907`.

### Cross-platform source hardening

- Desktop remains shared across Windows, Linux, and macOS with x64 and ARM64 release targets;
- Browser/WebAssembly/PWA composition remains separately validated;
- Android explicitly declares `android-arm`, `android-arm64`, `android-x86`, and `android-x64` runtime identifiers;
- iOS explicitly declares `ios-arm64`, `iossimulator-arm64`, and `iossimulator-x64` runtime identifiers;
- focused platform validation and Source Preflight continue to cover shared platform composition and mobile architecture inventory;
- platform workflow contracts remain aligned with current GitHub Actions checkout/setup requirements.

### Deterministic release SBOMs and integrity

- `tools/generate_sbom.py` generates deterministic CycloneDX 1.7 JSON from restored NuGet dependency metadata;
- supported assets-format and generator versions are recorded in BOM metadata;
- Desktop and Browser release workflows publish matching SBOM files;
- signed Android publication emits an Android SBOM when the AAB is available;
- SBOMs participate in checksum and provenance coverage;
- release filenames are validated for uniqueness and reserved-name collisions before checksums/provenance/publication;
- `SHA256SUMS.txt` uses flat published basenames suitable for `sha256sum -c` after download;
- `actions/attest@v4` covers the prepared release-assets tree.

### Security automation

- direct/transitive NuGet Audit remains enabled with moderate-or-higher enforcement through warnings-as-errors;
- CodeQL C# scanning, Dependency Review, and Dependabot coverage remain enabled;
- focused security workflow validation protects action majors, permissions, triggers, severity thresholds, and unsafe-trigger drift;
- dependency-security validation protects against audit disablement, threshold weakening, and protected NU190x suppression drift.

## Product Identity

- Product name: CalcNova
- Public version: `2.9.7`
- SemVer/package equivalent: `2.9.7`
- Release tag equivalent: `v2.9.7`
- Android/iOS display version: `2.9.7`
- Android/iOS numeric build code: `20907`
- Assembly version: `2.9.7.0`
- File version: `2.9.7.0`
- Informational version: `2.9.7`
- Application id: `in.sanskar.calcnova`
- License: Apache-2.0
- Repository: `https://github.com/sanskarIN/CalcNova`

## Technical Foundation

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Project-owned parser/evaluator rather than arbitrary code execution
- Pure calculation/domain libraries separated from Avalonia where practical
- Thin Desktop, Browser/WebAssembly, Android, and iOS composition heads
- Native SQLite history behind abstractions
- Browser-safe history/settings storage
- Versioned local settings schema with explicit migration behavior
- Optional network-enhanced currency conversion with no embedded provider credentials
- Local-first ordinary calculation and metadata behavior
- Centralized package management
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings
- Direct/transitive NuGet vulnerability auditing
- Automated dependency review and CodeQL source scanning
- Deterministic CycloneDX 1.7 release SBOM generation
- Download-friendly SHA-256 release manifests
- Provenance-attested stable release artifact publication

## Completed Calculator Capabilities

### Standard and scientific calculation

- Typed calculation errors and workload limits
- Mixed numeric representation using `BigInteger`, `decimal`, and bounded floating-point fallback
- Safe tokenizer and recursive-descent parser
- Standard arithmetic, parentheses, unary operators, and right-associative exponentiation
- Decimal/scientific input
- Scientific constants/functions
- Degree/radian/gradian angle modes
- Calculator percentage semantics separate from expression modulo
- Repeated-equals session behavior
- MC, MR, MS, M+, M- memory operations
- Sanitized external expression import
- User-triggered clipboard paste and explicit result copy
- Top-row/numpad input
- Printable/shifted operator mappings outside active text editing
- Selection-aware editing, caret restoration, Backspace behavior, and selection-preserving wrapping

### Exact rational arithmetic

- Canonical `BigInteger` numerator/denominator representation
- Positive denominator and GCD normalization
- Exact integer, fraction, finite-decimal, and decimal-scientific parsing
- Exact arithmetic, reciprocal, comparison, equality, and hashing
- Cross-cancellation
- Bounded raw input, exponent/scale magnitude, and reduced bit length
- Calculator panel workflows and application/headless regression coverage

### Engineering notation

- Engineering exponents divisible by three
- 1–15 significant digits
- Canonical invariant-culture parsing
- Explicit exponent range `-324..306`
- Non-zero-underflow rejection
- Extreme finite-value chunked scaling
- Shared 4,096-character input bound
- Calculator panel and focused/integrated validation coverage

## Completed Programmer and Unicode Capabilities

- Base 2–36 parse/format
- Binary/octal/decimal/hex synchronized representations
- 8/16/32/64/128-bit word sizes
- Signed/unsigned two's-complement interpretation
- Fixed-width masking
- AND/OR/XOR/NOT and left/logical-right/arithmetic-right shifts
- Full interactive bit grids with accessible state labels
- Byte grouping and copy actions
- Unicode scalar/code-point conversion
- Bounded text inspection
- Local Unicode plane/general-category/UTF-8/UTF-16 metadata
- Shared Unicode metadata presentation and copy actions
- No network dependency for Unicode metadata

## Completed Conversion and Utility Capabilities

- Offline unit conversion across length, area, volume, mass, speed, temperature, time, data, frequency, pressure, energy, power, force, and angle
- Unit swapping and validated conversion-pair model
- Bounded recent pairs and favorites
- Persisted recent/favorite state
- 1–17 significant-digit precision
- Search and result assignment
- Clear-recents and result copy
- Converter default/preference/privacy contracts
- Replaceable currency provider/cache architecture
- Offline currency fallback semantics
- Date differences, calendar arithmetic, business-day utilities, and fixed-duration conversion

## Completed Statistics, Equations, Matrices, and Graphing

### Statistics

- Descriptive statistics
- Bounded dataset parser
- Population/sample covariance
- Pearson correlation when defined
- Ordinary least-squares regression
- `R²` when defined
- Regression prediction
- Deterministic degenerate/non-finite/oversized handling
- Stale-model clearing
- Shared paired-statistics panel and copy workflow

### Equations and matrices

- Equation-solving module and shared view model
- Quadratic workflows
- Matrix determinant, inverse, rank, and linear-system solving
- Matrix result copy

### Graphing and numerical analysis

- Bounded function sampling
- Discontinuity segmentation
- Explicit viewport model
- Pointer and keyboard interaction
- Eight-action accessible viewport toolbar
- Reset/fit controls
- Nearest-point trace
- Bounded CSV generation
- Multi-expression sampling
- Stable series identities
- Deterministic non-color-only line patterns
- Multi-series text legend
- Accessible SVG export
- Bounded derivative approximation
- Bracketed bisection root finding
- Bounded Simpson integration
- Extreme-finite-value safeguards
- Explicit graph numerical workload budgets

## Completed History, Export, Settings, Persistence, and Onboarding

- Calculation-history abstraction
- SQLite native history
- Browser-safe storage path
- Recent/search/favorite/delete/clear workflows
- Bounded TXT/CSV/JSON export
- Bounded display previews with complete private copy payloads
- UTF-16-safe preview boundaries and newline normalization
- Settings repository abstraction
- Shared settings view model
- Persisted converter/culture preferences
- Explicit settings schema
- Legacy/unversioned migration
- Fail-closed unsupported future-schema handling
- Shared native/Browser JSON decoding and validation
- Versioned onboarding persistence, deferred first-run display, complete/skip actions, shell attachment, and focus restoration

## Completed Accessibility, Adaptive UI, and Localization Baseline

- 44-DIP minimum interaction-target baseline
- 54-DIP calculator key baseline
- Compact/medium/expanded layout profiles
- Compact overflow fallback
- Focus bring-into-view
- Explicit focus styling and stronger CalcNova high-contrast focus styling
- High-contrast and reduced-motion shell state
- Ctrl+PageUp/PageDown/Home/End mode navigation
- Accessible programmer bit-state names
- Dynamic graph-control focus/touch-target contracts
- Eight-action graph toolbar with English/Hindi semantic labels
- Runtime evidence matrix with PASS / FAIL / BLOCKED / NOT RUN vocabulary
- Stable semantic string-key catalog
- Complete English semantic catalog for the current key set
- Complete Hindi semantic catalog for the current key set
- Regional English/Hindi culture selection
- Persisted culture preference
- Catalog completeness/duplicate/unknown-key validation
- Runtime localization for reviewed shell, calculator, onboarding, settings, history, currency, About, graph controls, and related surfaces

## Completed Platform Composition

- Shared application composition root
- Desktop composition
- Browser/WebAssembly composition
- Android composition
- iOS composition
- Shared clipboard abstraction and Avalonia adapter
- External-link abstraction
- Native/Browser settings and history composition
- x64 + ARM64 self-contained desktop release matrix for Windows, Linux, and macOS
- Explicit Android ARM/ARM64/x86/x64 source runtime identifiers
- Explicit iOS device ARM64 and simulator ARM64/x64 source runtime identifiers
- Focused cross-platform source validator/workflow and integrated preflight coverage

## Completed Validation, Security, and Release Infrastructure

SDK-independent source contracts cover:

- repository/security checks;
- XAML well-formedness;
- shared UI/navigation/keyboard contracts;
- calculator selection editing;
- graph keyboard/surface/presentation/numerical budgets and graph accessibility;
- Unicode metadata;
- exact rationals;
- engineering notation;
- export previews;
- bivariate statistics;
- headless UI source contracts;
- accessibility markup/focus/dynamic controls/evidence/adaptive layout/touch targets;
- localization;
- converter preference/default behavior;
- settings schema;
- onboarding;
- packaging metadata;
- current release identity and completion-status consistency;
- Desktop/Browser/Android/iOS workflow contracts;
- cross-platform source composition and mobile architecture contracts;
- security automation workflow contracts and focused-workflow self-validation;
- NuGet dependency-security policy contracts;
- Source Preflight workflow self-validation;
- exact-tag iOS simulator workflow;
- release workflow and documentation contracts;
- deterministic CycloneDX 1.7 SBOM generation and NuGet assets-format compatibility tests;
- Desktop/Browser/signed-Android SBOM publication ordering validation;
- six-target x64/ARM64 desktop release matrix validation;
- release filename collision/checksum usability validation;
- release least-privilege/provenance attestation validation;
- release-tag syntax;
- artifact manifest/checksum integrity;
- structured release-evidence model/runner/verifier;
- Python regression suites for source validators;
- integrated SDK-independent source preflight.

## Release-Version Safety

`Directory.Build.props` is the release-version and dependency-audit policy source of truth.

The release workflow:

1. validates strict SemVer tag syntax;
2. checks out the exact requested tag;
3. reads normalized `<Version>` from `Directory.Build.props`;
4. verifies the tag equals `v` plus that normalized source version;
5. runs tagged source preflight, including release-identity, dependency-security, and SBOM-generator regression coverage;
6. restores the .NET solution with configured direct/transitive vulnerability auditing when advisory sources are available;
7. proceeds to .NET validation and platform publication only after those checks;
8. publishes each target package and generates its matching CycloneDX 1.7 SBOM;
9. validates flat release filename uniqueness and the reserved checksum name;
10. generates a download-friendly basename checksum manifest covering package and SBOM assets;
11. attests the prepared release tree;
12. publishes stable GitHub Release assets.

The Android publication job does not replace source-owned display/build versions with tag text or GitHub run numbers.

## UI Automation Source Coverage

Focused Avalonia headless test source covers:

- primary mode inventory;
- Calculator commands and selection editing;
- compact layout class;
- keyboard mode navigation;
- high-contrast state;
- onboarding visibility and dismissal;
- graph keyboard viewport operations;
- eight-action graph toolbar command/label/target/focus contracts;
- multi-series presentation/legend integration;
- Unicode metadata panel;
- exact-rational panel;
- engineering-notation panel and input bound;
- paired-statistics panel;
- dynamic graph-control focus/touch-target behavior.

## Environment Verification Record

Product implementation completeness and environment execution evidence are separate concepts.

The repository records a check as PASS only when it actually executes and its result is observed. The available source pass verified the connected repository state through SDK-independent tooling; direct .NET/platform/device/store execution remains environment-dependent and must be recorded as `PASS`, `FAIL`, `BLOCKED`, or `NOT RUN` only after the relevant operation is observed.

No hosted workflow, runtime, signing, notarization, device, browser, TestFlight/App Store, Play Console, release-SBOM publication, checksum verification, or provenance PASS is inferred merely from source contracts.

Typical verification commands include:

```bash
python tools/release_preflight.py --tag v2.9.7
python -m unittest tools.tests.test_release_identity
python -m unittest tools.tests.test_generate_sbom
python tools/validate_platform_support.py .
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python tools/validate_release_workflow.py .
python tools/validate_release_docs.py .
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Platform signing, notarization, provisioning, hosted security scanning, online NuGet advisory lookup, checksum verification against published downloads, release SBOM generation from actual restored target assets, artifact attestation execution, and store processing additionally require their respective external environments/services/credentials.

## Historical Release Checkpoints

- [`docs/releases/2.9.6.md`](docs/releases/2.9.6.md) — preserved 2.9.6 release baseline
- [`docs/releases/2.9.5.md`](docs/releases/2.9.5.md) — preserved 2.9.5 release baseline
- [`docs/releases/2.9.0.md`](docs/releases/2.9.0.md) — preserved 2.9.0 release baseline

## Final Classification

- Product scope for 2.9.7: **COMPLETE**
- Core features: **COMPLETE**
- Shared application features: **COMPLETE**
- Platform source composition: **COMPLETE**
- Cross-platform source validation: **COMPLETE**
- x64/ARM64 desktop release source contract: **COMPLETE**
- Release identity consistency contract: **COMPLETE**
- Security automation source contract: **COMPLETE**
- NuGet dependency-security policy source contract: **COMPLETE**
- Deterministic release SBOM source contract: **COMPLETE**
- Release checksum/integrity source contract: **COMPLETE**
- Release provenance/least-privilege source contract: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release workflow infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**
- Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

For details, see:

- [`README.md`](README.md)
- [`CHANGELOG.md`](CHANGELOG.md)
- [`docs/VERSIONING.md`](docs/VERSIONING.md)
- [`docs/FEATURES.md`](docs/FEATURES.md)
- [`docs/ROADMAP.md`](docs/ROADMAP.md)
- [`docs/SECURITY_AUTOMATION.md`](docs/SECURITY_AUTOMATION.md)
- [`docs/ARTIFACT_PROVENANCE.md`](docs/ARTIFACT_PROVENANCE.md)
- [`docs/releases/2.9.7.md`](docs/releases/2.9.7.md)
- [`docs/releases/2.9.6.md`](docs/releases/2.9.6.md)
- [`docs/releases/2.9.5.md`](docs/releases/2.9.5.md)
- [`docs/releases/2.9.0.md`](docs/releases/2.9.0.md)
- [`docs/FINAL_SOURCE_AUDIT_2026-08-19.md`](docs/FINAL_SOURCE_AUDIT_2026-08-19.md)
- [`what_changed.md`](what_changed.md)
