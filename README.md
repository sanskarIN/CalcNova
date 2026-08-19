# CalcNova

**Fast. Precise. Private. Everywhere.**

CalcNova is an open-source, privacy-first, cross-platform calculator built with C#, .NET, and Avalonia UI. It combines a project-owned expression engine with scientific, exact-rational, engineering-notation, programmer, conversion, graphing, statistics, equation, matrix, date/time, currency, history, and settings modules while keeping ordinary calculations local.

> CalcNova is under active development. Source presence is not the same as validated release readiness. See [`PROJECT_STATE.md`](PROJECT_STATE.md) and the [final source audit](docs/FINAL_SOURCE_AUDIT_2026-08-19.md) for exact implementation and validation status.

## Status

[![Build and Test](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml)
[![Formatting](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml)
[![Documentation Check](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml)
[![Source Preflight](https://github.com/sanskarIN/CalcNova/actions/workflows/source-preflight.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/source-preflight.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

## Current capabilities

### Standard and scientific calculator

- Safe tokenizer and recursive-descent parser owned by the project
- Explicit operator precedence and right-associative exponentiation
- Parentheses and unary plus/minus
- Arbitrary-precision integers with `System.Numerics.BigInteger`
- Decimal arithmetic where appropriate with bounded floating-point fallback
- Typed calculation errors and workload limits
- Scientific notation parsing
- Square/cube/power/root operations
- Reciprocal, absolute value, logarithms, exponentials
- Trigonometric, inverse-trigonometric, hyperbolic, and inverse-hyperbolic functions
- Degrees, radians, and gradians
- Floor/ceiling/truncation/rounding/sign/min/max
- Factorial, GCD, LCM, combinations, and permutations
- Constants including π, e, and τ
- Calculator-style percentage handling separate from expression modulo
- Repeated-equals session behavior
- MC, MR, MS, M+, and M- memory operations
- Sanitized external expression import with common calculator-glyph normalization
- User-triggered sanitized clipboard paste and result copy
- Top-row/numpad digit and arithmetic-key support outside active text fields
- Safe printable/shifted operator mappings outside active text fields
- Caret-aware insertion, selection replacement/deletion, Backspace behavior, and selection-preserving wrapping

### Exact rational and engineering-notation utilities

- Canonical bounded `BigInteger` rational arithmetic
- Exact parsing of integers, fractions, finite decimals, and decimal scientific notation
- Exact add/subtract/multiply/divide, comparison, reciprocal, equality, and hashing
- Safe canonical behavior for `default(RationalNumber)`
- Raw input, decimal scale/exponent, and reduced bit-length workload limits
- Engineering notation with exponents divisible by three
- Engineering exponent range from -324 through 306
- Selectable 1–15 significant digits and extreme finite-value handling
- Shared Calculator panels, tests, focused source validators/workflows, and integrated preflight coverage

See [`docs/EXACT_RATIONALS.md`](docs/EXACT_RATIONALS.md) and [`docs/ENGINEERING_NOTATION.md`](docs/ENGINEERING_NOTATION.md).

### Programmer and Unicode tools

- Base 2 through base 36 parsing, formatting, and shared UI selection
- Binary/octal/decimal/hex representations
- 8/16/32/64/128-bit shared word-size presets
- Signed/unsigned two's-complement interpretation
- Correct fixed-width masked non-decimal displays
- AND, OR, XOR, NOT
- Left shift, logical right shift, arithmetic right shift
- Full interactive word-size bit grid with accessible bit labels
- Byte-grouped presentation for large word sizes
- Copy actions for radix and fixed-width bit representations
- Unicode scalar/code-point parsing, formatting, scalar-to-text, and bounded text inspection
- Local Unicode general-category, plane, UTF-8-byte-width, and UTF-16-unit metadata
- Dedicated shared Unicode UI and explicit metadata/result copy actions

### Offline unit conversion

Fixed conversion categories include length, area, volume, mass, speed, temperature, time, data/storage, frequency, pressure, energy, power, force, and angle.

The shared converter supports unit swapping, selectable 1–17 significant-digit result precision, category-scoped search, recent conversion pairs, favorite pairs, pair restoration, clear-recents, result copy, and persisted converter preferences across launches. Fixed physical/data conversions remain local and offline.

### Currency and date/time utilities

- Optional replaceable currency-rate provider architecture
- Local currency-rate cache and offline fallback semantics
- No embedded provider credentials
- Date difference calculations
- Calendar arithmetic
- Business-day helpers
- Fixed-duration conversion

### Graphing and numerical analysis

- `y = f(x)` sampling through the shared expression engine
- Workload-bounded sampling and discontinuity segmentation
- Explicit graph viewport model
- Focusable interactive Avalonia plot control
- Pointer drag pan, wheel zoom, and double-tap/double-click fit-to-data
- Keyboard arrow-key pan, numpad Add/Subtract zoom, Home reset, and `F` fit-to-data
- Nearest sampled-point trace
- Single- and multi-expression bounded CSV output
- Deterministic accessible SVG graph export
- Deterministic multi-series line-pattern differentiation that does not rely on color alone
- Synchronized multi-series text legend
- Bounded central-difference derivative approximation
- Bracketed bisection root finding
- Bounded Simpson numerical integration
- Extreme-finite-value numerical-analysis hardening and explicit workload-budget regressions
- Shared controls for derivative, root, and integral analysis

Numerical analysis is approximate by design and is labeled accordingly.

### Statistics and advanced mathematics

- Descriptive statistics and shared summary-copy workflow
- Bounded paired X/Y dataset parsing
- Population/sample covariance
- Pearson correlation when defined
- Ordinary least-squares regression, `R²`, and prediction when defined
- Deterministic degenerate/non-finite/oversized dataset behavior
- Shared paired-statistics panel and copy workflow
- Equation-solving module and shared view model
- Matrix utilities and shared view model with result copy

See [`docs/BIVARIATE_STATISTICS.md`](docs/BIVARIATE_STATISTICS.md).

### Local history and settings

- Calculation-history abstraction
- SQLite-backed native history
- Browser-safe storage path
- Search, favorites, delete, and clear workflows
- TXT/CSV/JSON history export with bounded display previews and full private copy payloads
- Settings/preferences abstraction and view model
- Persisted converter and culture preferences
- Explicit settings schema version
- Legacy/unversioned settings migration and fail-closed future-schema rejection
- Shared settings JSON decoder/validator architecture across native and Browser storage
- About/support external-link abstraction

### Accessibility, adaptive UI, and localization source foundations

- Shared 44-DIP minimum interaction-target baseline
- 54-DIP standard calculator-key baseline
- Explicit visible keyboard-focus emphasis, strengthened under CalcNova high contrast
- Compact/medium/expanded layout profiles and compact overflow fallback
- Ctrl+PageUp/PageDown cyclic mode navigation and Ctrl+Home/End first/last navigation
- Graph keyboard viewport interaction
- Focus bring-into-view behavior for shared scroll containers
- Dynamic graph controls covered by focus/touch-target regressions
- English semantic string catalog
- Hindi semantic string catalog for the current key set
- Regional English/Hindi culture selection such as `en-IN` and `hi-IN`
- Reviewed runtime localization of shell/calculator/onboarding/settings/history/currency/About surfaces
- Conservative runtime accessibility evidence matrix

The shared XAML still contains unmigrated English, so the Hindi semantic catalog is localization infrastructure rather than a claim that the complete UI is already translated.

## Shared application

The shared Avalonia shell currently exposes:

- Calculator, including exact-rational and engineering-notation utilities
- Programmer
- Unicode code points and local scalar metadata
- Unit conversion
- Statistics, including paired covariance/correlation/regression analysis
- Equations
- Matrices
- Graphing and numerical analysis
- Date/time
- Currency
- History
- Settings
- About/support

The remaining high-priority work is dominated by real compiled/runtime evidence: .NET restore/build/test/headless results, target-device accessibility/adaptive validation, platform runtime behavior, packaging, signing, and store checks. See [`docs/ROADMAP.md`](docs/ROADMAP.md).

## Platform source targets

The repository contains platform heads/composition for Desktop, Browser/WebAssembly, Android, and iOS. Clipboard composition is provided through the shared Avalonia adapter and attached only when the shared view has a `TopLevel` clipboard.

Platform-specific source existing in the repository does **not** by itself mean a store/package build has been validated. Exact build/packaging status is recorded in [`PROJECT_STATE.md`](PROJECT_STATE.md).

## Validation, artifact integrity, and release evidence

CalcNova has a broad SDK-independent source validation layer covering repository/XAML/UI/navigation/keyboard contracts, graph/numerical contracts, Unicode, exact rationals, engineering notation, exports, bivariate statistics, accessibility/adaptive behavior, localization/settings, packaging/platform workflows, exact-tag iOS simulator release logic, artifact integrity, and structured release evidence.

Run the integrated source gate:

```bash
python tools/release_preflight.py
```

Collect structured source evidence:

```bash
python tools/run_release_evidence.py --scope source
```

Artifact manifest/checksum tooling and the structured evidence verifier are documented alongside the release process. Source tooling does not replace real compiled/platform evidence.

See [`docs/SOURCE_PREFLIGHT.md`](docs/SOURCE_PREFLIGHT.md), [`docs/VALIDATION_EVIDENCE.md`](docs/VALIDATION_EVIDENCE.md), and [`docs/RELEASE_READINESS_CHECKLIST.md`](docs/RELEASE_READINESS_CHECKLIST.md).

## Architecture

CalcNova keeps mathematical/domain projects independent of Avalonia UI:

```text
Platform heads
     │
     ▼
CalcNova.App
     │
     ├─────────────┬─────────────┬─────────────┐
     ▼             ▼             ▼             ▼
CalcNova.Core   feature libs   platform     persistence
                               contracts    implementations
```

Platform heads stay thin and contain startup, lifecycle, packaging, permissions, browser/native storage composition, external-link composition, and unavoidable native integration.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Repository layout

```text
CalcNova/
├── .github/
│   └── workflows/
├── docs/
├── packaging/
├── src/
│   ├── CalcNova.App/
│   ├── CalcNova.Core/
│   ├── CalcNova.Scientific/
│   ├── CalcNova.Programmer/
│   ├── CalcNova.Converter/
│   ├── CalcNova.Currency/
│   ├── CalcNova.DateTime/
│   ├── CalcNova.Graphing/
│   ├── CalcNova.Statistics/
│   ├── CalcNova.Equations/
│   ├── CalcNova.Matrices/
│   ├── CalcNova.Platform/
│   ├── CalcNova.Persistence/
│   ├── CalcNova.Desktop/
│   ├── CalcNova.Browser/
│   ├── CalcNova.Android/
│   └── CalcNova.iOS/
├── tests/
├── tools/
├── CalcNova.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── PROJECT_STATE.md
└── what_changed.md
```

## Development prerequisites

The repository pins a .NET 10 SDK feature band through [`global.json`](global.json). Install the required stable .NET SDK and platform workloads for the target you intend to build.

Run the SDK-independent source gate first:

```bash
python tools/release_preflight.py
```

Then run the compiled verification sequence:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

See [`docs/SOURCE_PREFLIGHT.md`](docs/SOURCE_PREFLIGHT.md), [`docs/BUILDING.md`](docs/BUILDING.md), and [`docs/TESTING.md`](docs/TESTING.md) before platform-specific work.

## Run the desktop application

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

This requires a working .NET/Avalonia desktop environment.

## Correctness and validation policy

CalcNova does not execute arbitrary user code to evaluate expressions. Calculation syntax is tokenized, parsed, and evaluated by project-owned domain code.

A feature is not considered validated merely because source code or tests exist. Build/test/platform status must be based on commands or workflows that actually ran and whose results were observed. When an environment is unavailable, project state records `NOT RUN` or `BLOCKED` instead of inventing PASS.

See [`docs/CALCULATION_ENGINE.md`](docs/CALCULATION_ENGINE.md), [`docs/INPUT_SAFETY.md`](docs/INPUT_SAFETY.md), [`docs/ACCESSIBILITY_TEST_MATRIX.md`](docs/ACCESSIBILITY_TEST_MATRIX.md), and [`docs/TESTING.md`](docs/TESTING.md).

## Privacy

Core calculations, fixed conversions, exact rational arithmetic, engineering formatting, and Unicode metadata are designed to operate locally. CalcNova does not require an account for ordinary calculation features, and the open-source base does not intentionally include advertising or behavioral-tracking SDKs.

Clipboard text is read only after an explicit paste action in the calculator workflow; imported text is sanitized before evaluation. Network-enhanced features such as currency-rate refresh remain optional and independently controllable.

See [`docs/PRIVACY.md`](docs/PRIVACY.md).

## Contributing

Contributions are welcome. Please read:

- [`CONTRIBUTING.md`](CONTRIBUTING.md)
- [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md)
- [`SECURITY.md`](SECURITY.md)
- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- [`docs/TESTING.md`](docs/TESTING.md)

Small, focused pull requests with relevant tests and documentation are preferred.

## Security

Do not report sensitive vulnerabilities in a public issue. Follow [`SECURITY.md`](SECURITY.md) for private reporting guidance.

Support/security contact: **supportramsandesh@gmail.com**

## License

CalcNova is licensed under the [Apache License 2.0](LICENSE).

Third-party packages and assets remain subject to their respective licenses.

## Project links

- **Repository:** https://github.com/sanskarIN/CalcNova
- **GitHub profile:** https://www.github.com/sanskarIN
- **Business:** sanskarin@outlook.in
- **Business:** sanskarin.business@gmail.com
- **Support:** supportramsandesh@gmail.com

## Support CalcNova

If CalcNova is useful to you and you want to support continued open-source development:

**Buy Me a Coffee — @sanskarIN**  
https://buymeacoffee.com/sanskarIN

Support is optional. Core features must never be blocked behind donations or interrupted by donation prompts.

---

For exact development progress, recent commits, unresolved work, and continuation tasks, read [`what_changed.md`](what_changed.md), [`PROJECT_STATE.md`](PROJECT_STATE.md), [`docs/ROADMAP.md`](docs/ROADMAP.md), and the [final source audit](docs/FINAL_SOURCE_AUDIT_2026-08-19.md).
