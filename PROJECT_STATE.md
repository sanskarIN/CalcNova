# CalcNova Project State

## Current Version

**2.8.03**

Normalized .NET/NuGet version: `2.8.3`  
Normalized release tag: `v2.8.3`  
Mobile numeric build code: `20803`

See [`docs/VERSIONING.md`](docs/VERSIONING.md).

## Current Branch

`main`

## Completion Status

**COMPLETE — CalcNova version 2.8.03**

The defined 2.8.03 product scope is implemented in the repository. Core calculation, scientific functions, exact rational arithmetic, engineering notation, programmer and Unicode tools, converter/date-time/currency utilities, descriptive and bivariate statistics, equations, matrices, graphing/numerical analysis, history, persistence, settings, onboarding, localization infrastructure and reviewed localized surfaces, accessibility/adaptive contracts, Desktop/Browser/Android/iOS composition, source validation, artifact integrity, structured release evidence, packaging metadata, and release workflows are present as completed source capabilities.

Future repository changes are classified as maintenance, compatibility updates, security fixes, documentation changes, translation additions, or optional enhancements. They are not required to define the 2.8.03 project as complete.

## Product Identity

- Product name: CalcNova
- Public version: `2.8.03`
- SemVer/package equivalent: `2.8.3`
- Release tag equivalent: `v2.8.3`
- Android/iOS display version: `2.8.03`
- Android/iOS numeric build code: `20803`
- Assembly version: `2.8.3.0`
- File version: `2.8.3.0`
- Informational version: `2.8.03`
- Application id: `in.sanskar.calcnova`
- License: Apache-2.0
- Repository: `https://github.com/sanskarIN/CalcNova`

The public `2.8.03` format is intentionally preserved. Strict SemVer tooling uses `2.8.3` because numeric SemVer components cannot contain leading zeroes.

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
- Safe default-value/canonical-zero semantics
- Exact integer, fraction, finite-decimal, and decimal-scientific parsing
- Exact arithmetic, reciprocal, comparison, equality, and hashing
- Cross-cancellation
- 4,096-character raw input bound before trimming
- 10,000 decimal exponent/scale magnitude bound
- 65,536-bit reduced numerator/denominator bound
- Shared Calculator panel and application workflows
- Core/application/headless regression source
- Focused validator/workflow and integrated preflight coverage

### Engineering notation

- Engineering exponents divisible by three
- 1–15 significant digits
- Canonical invariant-culture parsing
- 4,096-character shared core/App/UI input bound
- Explicit exponent range `-324..306`
- Non-zero-underflow rejection
- Extreme finite-value chunked scaling
- Shared Calculator panel
- Core/application/headless regression source
- Focused validator/workflow and integrated preflight coverage

## Completed Programmer and Unicode Capabilities

- Base 2–36 parse/format
- Binary/octal/decimal/hex synchronized representations
- 8/16/32/64/128-bit word sizes
- Signed/unsigned two's-complement interpretation
- Fixed-width masking
- AND/OR/XOR/NOT
- Left/logical-right/arithmetic-right shifts
- Full interactive bit grids
- Byte grouping
- Accessible bit-cell state labels
- Copy actions for radix/fixed-width representations
- Unicode scalar/code-point conversion
- Bounded text inspection
- Local Unicode plane/general-category/UTF-8/UTF-16 metadata
- Shared Unicode metadata presentation and copy actions
- No network dependency for Unicode metadata

## Completed Conversion and Utility Capabilities

- Offline unit conversion across major physical/data categories
- Unit swapping
- Validated conversion-pair model
- Bounded recent pairs
- Favorites
- Versioned persistence tokens
- Persisted recent/favorite state
- 1–17 significant-digit precision
- Search and result assignment
- Clear-recents and result copy
- Converter default/preference/privacy source contracts
- Replaceable currency provider/cache architecture
- Offline currency fallback semantics
- Date differences
- Calendar arithmetic
- Business-day utilities
- Fixed-duration conversion

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
- Matrix determinant
- Matrix inverse
- Matrix rank
- Linear-system solving
- Matrix result copy

### Graphing and numerical analysis

- Bounded function sampling
- Discontinuity segmentation
- Explicit viewport model
- Pointer and keyboard interaction
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

## Completed History, Export, Settings, and Persistence

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

## Completed Accessibility, Adaptive UI, and Onboarding Baseline

- 44-DIP minimum interaction-target baseline
- 54-DIP calculator key baseline
- Compact/medium/expanded layout profiles
- Compact overflow fallback
- Focus bring-into-view
- Explicit focus styling
- Stronger CalcNova high-contrast focus styling
- High-contrast and reduced-motion shell state
- Ctrl+PageUp/PageDown/Home/End mode navigation
- Accessible programmer bit-state names
- Dynamic graph control focus/touch-target contracts
- Onboarding shortcut suppression and focus restoration
- Runtime evidence matrix with PASS / FAIL / BLOCKED / NOT RUN vocabulary

## Completed Localization Baseline

- Stable semantic string-key catalog
- Complete English semantic catalog for the current key set
- Complete Hindi semantic catalog for the current key set
- Regional English/Hindi culture selection
- Persisted culture preference
- Catalog completeness/duplicate/unknown-key validation
- Runtime localization for reviewed shell, calculator, onboarding, settings, history, currency, About, and related surfaces

Additional language packs or further localization expansion are optional post-release contributions, not completion requirements for 2.8.03.

## Completed Platform Composition

- Shared application composition root
- Desktop composition
- Browser/WebAssembly composition
- Android composition
- iOS composition
- Shared clipboard abstraction and Avalonia adapter
- External-link abstraction
- Settings/history composition appropriate to native and Browser environments

Android and iOS source metadata uses display version `2.8.03` and numeric build code `20803`.

## Completed Validation and Release Infrastructure

SDK-independent source contracts cover:

- repository/security checks;
- XAML well-formedness;
- shared UI/navigation/keyboard contracts;
- calculator selection editing;
- graph keyboard/surface/presentation/numerical budgets;
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
- Desktop/Browser/Android/iOS workflow contracts;
- Source Preflight workflow self-validation;
- exact-tag iOS simulator workflow;
- release workflow and documentation contracts;
- release-tag syntax;
- artifact manifest/checksum integrity;
- structured release-evidence model/runner/verifier;
- Python regression suites for source validators;
- integrated SDK-independent source preflight.

## Release-Version Safety

`Directory.Build.props` is the release-version source of truth.

The release workflow:

1. validates strict SemVer tag syntax;
2. checks out the exact requested tag;
3. reads the normalized `<Version>` from `Directory.Build.props`;
4. verifies the tag equals `v` plus that normalized source version;
5. runs tagged source preflight;
6. proceeds to .NET validation and platform publication only after those checks.

The Android publication job does not replace source-owned display/build versions with the tag text or GitHub run number.

## UI Automation Source Coverage

Focused Avalonia headless test source covers the shared shell and key product scenarios, including:

- primary mode inventory;
- Calculator commands and selection editing;
- compact layout class;
- keyboard mode navigation;
- high-contrast state;
- onboarding visibility and dismissal;
- graph keyboard viewport operations;
- multi-series presentation/legend integration;
- Unicode metadata panel;
- exact-rational panel;
- engineering-notation panel and input bound;
- paired-statistics panel;
- dynamic graph-control focus/touch-target behavior.

## Environment Verification Record

Product implementation completeness and environment execution evidence are separate concepts.

The repository records a check as PASS only when it actually executes and its result is observed. In the assistant environment used for the final source pass, the required .NET 10/platform toolchains were not available for direct execution, so local compiled/platform evidence remains recorded conservatively as `NOT RUN` rather than being invented.

This evidence notation does **not** mean CalcNova 2.8.03 is incomplete. It means a particular command was not executed in that particular environment.

Typical environment-specific verification commands include:

```bash
python tools/release_preflight.py
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Platform signing, notarization, provisioning, and store processing additionally require external platform credentials/services.

## Final Classification

- Product scope for 2.8.03: **COMPLETE**
- Core features: **COMPLETE**
- Shared application features: **COMPLETE**
- Platform source composition: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release workflow infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**
- Future repository changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

For details, see:

- [`README.md`](README.md)
- [`CHANGELOG.md`](CHANGELOG.md)
- [`docs/VERSIONING.md`](docs/VERSIONING.md)
- [`docs/FEATURES.md`](docs/FEATURES.md)
- [`docs/ROADMAP.md`](docs/ROADMAP.md)
- [`docs/FINAL_SOURCE_AUDIT_2026-08-19.md`](docs/FINAL_SOURCE_AUDIT_2026-08-19.md)
- [`what_changed.md`](what_changed.md)
