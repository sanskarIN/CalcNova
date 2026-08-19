# CalcNova

**Fast. Precise. Private. Everywhere.**

CalcNova is an open-source, privacy-first, cross-platform calculator built with C#, .NET, and Avalonia UI. It combines a project-owned expression engine with scientific, programmer, conversion, graphing, statistics, equation, matrix, date/time, currency, history, and settings modules while keeping ordinary calculations local.

> CalcNova is under active development. Source presence is not the same as validated release readiness. See [`PROJECT_STATE.md`](PROJECT_STATE.md) for exact implementation and validation status.

## Status

[![Build and Test](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml)
[![Formatting](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml)
[![Documentation Check](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml)
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

### Programmer tools

- Base 2 through base 36 parsing and formatting
- Binary/octal/decimal/hex representations
- Configurable word size and signed/unsigned interpretation
- AND, OR, XOR, NOT
- Left shift, logical right shift, arithmetic right shift
- Fixed-width bit strings
- Bounded bit inspection and toggling
- Unicode scalar/code-point parsing and formatting helpers
- Unicode scalar-to-text and bounded text-inspection utilities

### Offline unit conversion

Fixed conversion categories include:

- length
- area
- volume
- mass
- speed
- temperature
- time
- data/storage
- frequency
- pressure
- energy
- power
- force
- angle

The converter also has source support for swapping units, recent conversion pairs, favorite pairs, and selectable 1–17 significant-digit result precision.

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
- Workload-bounded sampling
- Discontinuity segmentation
- Explicit graph viewport model
- Interactive Avalonia plot control
- Deterministic SVG graph export
- Bounded central-difference derivative approximation
- Bracketed bisection root finding
- Bounded Simpson numerical integration

Numerical analysis is approximate by design and must be presented as such in UI/documentation.

### Advanced mathematics

- Statistics module and shared view model
- Equation-solving module and shared view model
- Matrix utilities and shared view model

See the source modules and [`PROJECT_STATE.md`](PROJECT_STATE.md) for the exact implemented surface.

### Local history and settings

- Calculation-history abstraction
- SQLite-backed native history
- Browser-safe storage path
- Search and recent-history workflows
- Favorite history entries
- Delete/clear operations
- TXT/CSV/JSON history export
- Settings/preferences abstraction and view model
- About/support external-link abstraction

## Shared application

The shared Avalonia shell contains principal modes for:

- Calculator
- Programmer
- Unit conversion
- Statistics
- Equations
- Matrices
- Graphing
- Date/time
- Currency
- History
- Settings
- About/support

Some newer domain/view-model capabilities still need dedicated visible controls in the shared XAML shell. Those tasks are tracked in [`docs/ROADMAP.md`](docs/ROADMAP.md).

## Platform source targets

The repository contains platform heads/composition for:

- Desktop
- Browser/WebAssembly
- Android
- iOS

Platform-specific source existing in the repository does **not** by itself mean a store/package build has been validated. Exact build/packaging status is recorded in [`PROJECT_STATE.md`](PROJECT_STATE.md).

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

Basic verification sequence:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

See [`docs/BUILDING.md`](docs/BUILDING.md) and [`docs/TESTING.md`](docs/TESTING.md) before platform-specific work.

## Run the desktop application

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

This requires a working .NET/Avalonia desktop environment.

## Correctness and validation policy

CalcNova does not execute arbitrary user code to evaluate expressions. Calculation syntax is tokenized, parsed, and evaluated by project-owned domain code.

A feature is not considered validated merely because source code or tests exist. Build/test/platform status must be based on commands or workflows that actually ran and whose results were observed. When an environment is unavailable, project state records `NOT RUN` instead of inventing PASS.

See [`docs/CALCULATION_ENGINE.md`](docs/CALCULATION_ENGINE.md) and [`docs/TESTING.md`](docs/TESTING.md).

## Privacy

Core calculations and fixed conversions are designed to operate locally. CalcNova does not require an account for ordinary calculation features, and the open-source base does not intentionally include advertising or behavioral-tracking SDKs.

Network-enhanced features such as currency-rate refresh remain optional and independently controllable.

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

For exact development progress, recent commits, unresolved work, and continuation tasks, read [`what_changed.md`](what_changed.md), [`PROJECT_STATE.md`](PROJECT_STATE.md), and [`docs/ROADMAP.md`](docs/ROADMAP.md).
