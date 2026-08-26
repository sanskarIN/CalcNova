# CalcNova

**Fast. Precise. Private. Everywhere.**

**Current product version: 2.9.7**

CalcNova is a completed, open-source, privacy-first, cross-platform calculator built with C#, .NET, and Avalonia UI. It combines a project-owned expression engine with scientific, exact-rational, engineering-notation, programmer, Unicode, conversion, graphing, statistics, equation, matrix, date/time, currency, history, persistence, accessibility, localization, packaging, and release-validation capabilities while keeping ordinary calculations local.

> **Project status: Complete for version 2.9.7.** Future repository changes may provide maintenance, compatibility updates, security fixes, translations, or optional enhancements; they are not required to complete the 2.9.7 product scope.

## Status

[![Build and Test](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml)
[![Formatting](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml)
[![Documentation Check](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml)
[![Source Preflight](https://github.com/sanskarIN/CalcNova/actions/workflows/source-preflight.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/source-preflight.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

## Version 2.9.7

The public/product version is **2.9.7**, the .NET/NuGet package version is **2.9.7**, and the corresponding normalized release tag is **`v2.9.7`**. Android and iOS use numeric build code **`20907`**. See [`docs/VERSIONING.md`](docs/VERSIONING.md) for the complete mapping.

The earlier **2.9.0**, **2.9.5**, and **2.9.6** checkpoints are preserved in [`docs/releases/2.9.0.md`](docs/releases/2.9.0.md), [`docs/releases/2.9.5.md`](docs/releases/2.9.5.md), and [`docs/releases/2.9.6.md`](docs/releases/2.9.6.md). The repository then advanced to the current 2.9.7 maintenance baseline.

### 2.9-series release and platform maintenance

The 2.9 series adds release-consistency and cross-platform maintenance without removing the completed calculator feature set:

- centralized SDK-independent release identity parsing in `tools/release_identity.py`;
- fail-closed consistency checks for display, SemVer, package, assembly/file, informational, release-tag, and mobile-build identities;
- version-aware packaging, completion-status, platform-support, and release-document validators instead of hardcoded release constants;
- integrated release-identity regression coverage in Source Preflight;
- explicit Android ARM/ARM64/x86/x64 runtime identifiers;
- explicit iOS ARM64 device and ARM64/x64 simulator runtime identifiers;
- focused cross-platform source validation and GitHub Actions coverage;
- `actions/checkout@v7` alignment for platform workflow contracts;
- retained deterministic SBOM, checksum, provenance, dependency-security, CodeQL, and release-evidence controls.

## Core calculator

CalcNova includes a project-owned parser/evaluator rather than executing arbitrary user code.

Implemented capabilities include:

- safe tokenizer and recursive-descent parser;
- explicit operator precedence and right-associative exponentiation;
- parentheses and unary plus/minus;
- arbitrary-precision integers through `System.Numerics.BigInteger`;
- decimal arithmetic where appropriate with bounded floating-point fallback;
- typed calculation errors and workload limits;
- decimal and scientific-notation parsing;
- square, cube, arbitrary power, square/cube/nth root;
- reciprocal and absolute value;
- logarithmic and exponential functions;
- trigonometric, inverse-trigonometric, hyperbolic, and inverse-hyperbolic functions;
- degrees, radians, and gradians;
- floor, ceiling, truncation, rounding, sign, min, and max;
- factorial, GCD, LCM, combinations, and permutations;
- constants including π, e, and τ;
- calculator-style percentage handling separate from expression modulo;
- repeated-equals behavior;
- MC, MR, MS, M+, and M- memory operations;
- sanitized expression import and user-triggered clipboard paste;
- explicit copy-result workflow;
- top-row/numpad input and safe printable/shifted operator mappings outside active text editing;
- caret-aware insertion, forward/reversed selection replacement, selection deletion, Backspace-before-caret behavior, and selection-preserving wrapping.

## Exact rational arithmetic

The exact-rational utility provides bounded `BigInteger` fraction arithmetic without converting finite decimal input through `double` first.

It includes:

- canonical numerator/denominator normalization;
- positive-denominator normalization;
- stable zero/default value semantics;
- exact integer, fraction, finite-decimal, and decimal-scientific parsing;
- exact addition, subtraction, multiplication, division, negation, reciprocal, comparison, equality, and hashing;
- cross-cancellation before multiplication;
- bounded raw input length, decimal scale/exponent magnitude, and reduced bit length;
- Calculator panel workflows for normalize/add/subtract/multiply/divide;
- source, application, and headless regression coverage;
- focused and integrated SDK-independent validation.

See [`docs/EXACT_RATIONALS.md`](docs/EXACT_RATIONALS.md).

## Engineering notation

CalcNova includes a bounded engineering-notation formatter/parser for finite `double` values.

It provides:

- exponents in multiples of three;
- 1–15 significant digits;
- canonical invariant-culture parsing;
- explicit engineering exponent bounds from -324 through 306;
- non-zero-underflow rejection;
- chunked power-of-ten scaling for extreme finite values;
- a shared 4,096-character input budget across core parsing, formatting workflow, and UI text entry;
- Calculator panel format/parse workflows;
- focused validation and integrated source-preflight coverage.

See [`docs/ENGINEERING_NOTATION.md`](docs/ENGINEERING_NOTATION.md).

## Programmer and Unicode tools

Programmer features include:

- base 2 through base 36 parsing, formatting, and shared UI selection;
- binary/octal/decimal/hex synchronized representations;
- 8/16/32/64/128-bit word-size presets;
- signed/unsigned two's-complement interpretation;
- fixed-width masked non-decimal displays;
- AND, OR, XOR, and NOT;
- left shift, logical right shift, and arithmetic right shift;
- full interactive bit grids with accessible bit labels;
- byte-grouped display for large word sizes;
- copy actions for radix and fixed-width bit representations.

Unicode tools include:

- Unicode scalar/code-point parsing and formatting;
- scalar-to-text conversion;
- bounded Unicode text inspection;
- local general-category metadata;
- Unicode plane metadata;
- UTF-8 byte-width and UTF-16 code-unit metadata;
- local-first metadata derivation with no network lookup;
- explicit result and metadata copy actions.

See [`docs/PROGRAMMER_MODE.md`](docs/PROGRAMMER_MODE.md) and [`docs/UNICODE_METADATA.md`](docs/UNICODE_METADATA.md).

## Unit conversion

Offline fixed-unit categories include:

- length;
- area;
- volume;
- mass;
- speed;
- temperature;
- time;
- data/storage;
- frequency;
- pressure;
- energy;
- power;
- force;
- angle.

The shared converter also supports:

- validated conversion pairs;
- unit swapping;
- selectable 1–17 significant-digit precision;
- category-scoped unit search;
- search-result assignment to From/To;
- bounded recent pairs;
- favorite pairs;
- pair restoration;
- clear-recents;
- result copy;
- persisted converter preferences;
- source contracts for preference/default behavior.

Fixed physical/data conversion remains local and offline.

## Currency and date/time utilities

Currency infrastructure includes:

- replaceable rate-provider interface;
- local cache;
- offline fallback semantics;
- no embedded provider credentials.

Date/time utilities include:

- date difference;
- calendar arithmetic;
- business-day helpers;
- fixed-duration conversion.

## Statistics

CalcNova provides descriptive and paired statistics.

Implemented paired analysis includes:

- bounded X/Y dataset parsing;
- population covariance;
- sample covariance;
- Pearson correlation when mathematically defined;
- ordinary least-squares regression slope/intercept;
- coefficient of determination (`R²`) when defined;
- prediction from the latest valid regression model;
- stale-model clearing after failed analysis;
- deterministic handling of mismatched, non-finite, oversized, constant-X, constant-Y, and single-pair datasets;
- summary copy workflow.

See [`docs/BIVARIATE_STATISTICS.md`](docs/BIVARIATE_STATISTICS.md).

## Equations and matrices

Equation tools include shared equation-solving workflows, including quadratic analysis.

Matrix capabilities include:

- determinant;
- inverse;
- rank;
- linear-system solving;
- shared result presentation;
- explicit result copy.

## Graphing and numerical analysis

Graphing includes:

- `y = f(x)` sampling through the shared expression engine;
- workload-bounded sampling;
- discontinuity segmentation;
- explicit viewport model;
- focusable interactive Avalonia plot control;
- pointer drag panning;
- pointer-wheel zoom;
- double-tap/double-click fit-to-data;
- keyboard arrow-key panning;
- numpad Add/Subtract zoom;
- Home reset;
- `F` fit-to-data;
- accessible eight-action viewport toolbar for pan, zoom, reset, and fit;
- 44-DIP minimum toolbar action targets and keyboard focusability;
- nearest sampled-point trace;
- bounded single- and multi-expression CSV output;
- stable multi-series identities;
- deterministic non-color-only line patterns;
- synchronized multi-series text legend;
- accessible SVG export;
- bounded derivative approximation;
- bracketed bisection root finding;
- bounded Simpson integration;
- extreme-finite-value numerical safeguards;
- explicit numerical workload budgets.

Numerical analysis is approximate by design and is labeled accordingly.

See [`docs/GRAPH_INTERACTION.md`](docs/GRAPH_INTERACTION.md), [`docs/NUMERICAL_ANALYSIS.md`](docs/NUMERICAL_ANALYSIS.md), and [`docs/GRAPH_NUMERICAL_SAFETY.md`](docs/GRAPH_NUMERICAL_SAFETY.md).

## History, export, and persistence

History and persistence include:

- calculation-history abstraction;
- SQLite-backed native history;
- browser-safe storage path;
- recent/search/favorite/delete/clear workflows;
- bounded TXT/CSV/JSON export generation;
- bounded display previews with complete private copy payloads;
- reusable preview formatting with character/line limits, newline normalization, and UTF-16 boundary safety;
- settings repository abstraction;
- shared settings view model;
- persisted converter and culture preferences;
- explicit settings schema version;
- legacy and unversioned settings migration;
- fail-closed unsupported future-schema handling;
- shared JSON decoding/validation across native and Browser storage.

## Accessibility and adaptive UI

The shared accessibility/adaptive baseline includes:

- 44-DIP minimum interaction-target baseline;
- 54-DIP standard calculator-key baseline;
- explicit visible keyboard focus styling;
- stronger focus styling under CalcNova high contrast;
- compact/medium/expanded layout profiles;
- compact horizontal-overflow fallback;
- focus bring-into-view behavior;
- Ctrl+PageUp/PageDown cyclic mode navigation;
- Ctrl+Home/End first/last mode navigation;
- keyboard-operable graph viewport;
- accessible programmer bit-state names;
- textual alternatives for non-color graph differentiation;
- reduced-motion/high-contrast preference state;
- onboarding focus/shortcut contracts;
- dynamic-control touch-target/focus validation;
- runtime evidence vocabulary using PASS / FAIL / BLOCKED / NOT RUN.

Runtime/device evidence is recorded conservatively; a source contract is not mislabeled as an observed device test.

## Localization

CalcNova includes:

- stable semantic string keys;
- complete English semantic catalog for the current key set;
- complete Hindi semantic catalog for the current key set;
- regional English/Hindi culture selection such as `en-IN` and `hi-IN`;
- persisted culture preference;
- catalog completeness/duplicate/unknown-key validation;
- live localization for reviewed shell, calculator, onboarding, settings, history, currency, About, graph viewport actions, and related surfaces;
- Hindi labels for graph pan, zoom, reset, and fit controls.

Additional translations or further UI-string migration may be contributed as optional localization improvements; they are not required to define version 2.9.7 as complete.

## Platforms

CalcNova contains composition heads for:

- Desktop — Windows, Linux, and macOS targets;
- Browser/WebAssembly/PWA;
- Android — `android-arm`, `android-arm64`, `android-x86`, and `android-x64` source runtime identifiers;
- iOS — `ios-arm64`, `iossimulator-arm64`, and `iossimulator-x64` source runtime identifiers.

The product display version for Android and iOS is `2.9.7`, with numeric mobile build code `20907`.

Desktop release source targets remain `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`.

Platform package generation still depends on the appropriate external SDK/workload, signing identity, and store credentials. Those environment requirements are separate from project implementation completeness.

## Source validation and release evidence

CalcNova includes SDK-independent validators covering repository/security contracts, release identity, XAML, UI/navigation/keyboard behavior, graph/numerical contracts, Unicode, exact rationals, engineering notation, exports, bivariate statistics, accessibility/adaptive contracts, localization/settings, packaging metadata, platform workflows, cross-platform source composition, release workflows, artifact integrity, and structured release evidence.

Run the integrated source gate for the current release:

```bash
python tools/release_preflight.py --tag v2.9.7
```

Focused current-release checks include:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/validate_platform_support.py .
```

Collect structured evidence:

```bash
python tools/run_release_evidence.py --scope source
```

The Source Preflight workflow watches source, tests, tooling, documentation, packaging, workflows, and relevant root build/release metadata. It is also protected by its own source validator.

See [`docs/SOURCE_PREFLIGHT.md`](docs/SOURCE_PREFLIGHT.md), [`docs/VALIDATION_EVIDENCE.md`](docs/VALIDATION_EVIDENCE.md), and [`docs/RELEASE_READINESS_CHECKLIST.md`](docs/RELEASE_READINESS_CHECKLIST.md).

## Release identity

`Directory.Build.props` is the source of truth. The shared release-identity helper verifies the central version fields agree, derives the mobile build code, and exposes the expected release tag to SDK-independent validators.

For CalcNova 2.9.7:

```text
Product version: 2.9.7
Normalized package version: 2.9.7
Normalized release tag: v2.9.7
Assembly/file version: 2.9.7.0
Mobile build code: 20907
```

The release workflow verifies that the requested tag equals `v` plus the source `<Version>` before restore/build/test begins. The Android release job does not replace the product display version with tag text or GitHub run number.

See [`docs/VERSIONING.md`](docs/VERSIONING.md).

## Architecture

CalcNova keeps mathematical/domain projects independent of Avalonia UI where practical:

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

Platform heads remain thin and contain startup, lifecycle, packaging, permissions, browser/native storage composition, external-link composition, and unavoidable native integration.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Repository layout

```text
CalcNova/
├── .github/
│   └── workflows/
├── docs/
│   └── releases/
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

## Build prerequisites

The repository pins a .NET 10 SDK feature band through [`global.json`](global.json). Install the stable .NET SDK and platform workloads for the target you intend to build.

Run the SDK-independent source gate:

```bash
python tools/release_preflight.py --tag v2.9.7
```

Then run the compiled verification sequence in a suitable .NET environment:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

See [`docs/BUILDING.md`](docs/BUILDING.md) and [`docs/TESTING.md`](docs/TESTING.md).

## Run the desktop application

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

This requires a working .NET/Avalonia desktop environment.

## Correctness and evidence policy

A feature is not considered runtime-verified merely because source code or tests exist. Build/test/platform evidence must be based on commands or workflows that actually ran and whose results were observed. When an environment is unavailable, evidence records `NOT RUN` or `BLOCKED` instead of inventing PASS.

This evidence policy does not change the product-completion status of version 2.9.7; it preserves accuracy about where a particular verification command was or was not executed.

## Privacy

Core calculations, fixed conversions, exact rational arithmetic, engineering formatting, Unicode metadata, history, and settings are designed around local-first behavior. CalcNova does not require an account for ordinary calculation features, and the open-source base does not intentionally include advertising or behavioral-tracking SDKs.

Clipboard reads occur only after explicit user actions; imported expression text is sanitized. Network-enhanced currency refresh remains optional.

See [`docs/PRIVACY.md`](docs/PRIVACY.md).

## Contributing

Contributions for maintenance, compatibility, security, documentation, translations, tests, and optional enhancements are welcome. Read:

- [`CONTRIBUTING.md`](CONTRIBUTING.md)
- [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md)
- [`SECURITY.md`](SECURITY.md)
- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- [`docs/TESTING.md`](docs/TESTING.md)

## Security

Do not report sensitive vulnerabilities in a public issue. Follow [`SECURITY.md`](SECURITY.md) for private reporting guidance.

Support/security contact: **supportramsandesh@gmail.com**
