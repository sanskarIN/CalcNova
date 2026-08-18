# CalcNova

**Fast. Precise. Private. Everywhere.**

CalcNova is an open-source, privacy-first, cross-platform calculator built with C#, .NET, and Avalonia UI. The project is being developed as a serious long-term utility with a tested calculation engine, scientific and programmer tooling, offline unit conversion, local history, responsive UI, accessibility, keyboard support, and broad platform goals.

> CalcNova is under active development. The repository does not claim that unvalidated builds or unfinished platform targets are production-ready. See [`PROJECT_STATE.md`](PROJECT_STATE.md) for exact status.

## Status

[![Build and Test](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml)
[![Formatting](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml)
[![Documentation Check](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/docs-check.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

## Current capabilities

### Calculation engine

- Safe tokenizer and deterministic recursive-descent parser
- Standard arithmetic with explicit operator precedence
- Right-associative exponentiation
- Parentheses and unary plus/minus
- Arbitrary-precision integers through `System.Numerics.BigInteger`
- Decimal arithmetic where appropriate
- Floating-point fallback for transcendental functions
- Typed calculation errors and workload limits
- Scientific notation parsing

### Scientific calculations

- Square, cube, arbitrary power, square root, cube root, and nth root
- Reciprocal and absolute value
- Natural, base-10, base-2, and arbitrary-base logarithms
- Exponential functions
- Trigonometric and inverse trigonometric functions
- Hyperbolic and inverse hyperbolic functions
- Degrees, radians, and gradians
- Floor, ceiling, truncation, rounding, sign, min, max
- Factorial, GCD, LCM, combinations, permutations
- Constants including π, e, and τ

### Programmer tools

- Base 2 through base 36 parsing and formatting
- Arbitrary-precision integer conversion
- AND, OR, XOR, NOT
- Left shift, logical right shift, arithmetic right shift
- Configurable word size
- Signed/unsigned two's-complement interpretation
- Fixed-width bit-string visualization

### Offline conversion

The current fixed-unit engine includes categories such as:

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

### User interface

- Initial shared Avalonia application
- Standard/scientific calculator workspace
- Angle-mode controls
- Touch/mouse calculator keypad
- Expression entry and result reuse
- Basic desktop keyboard actions
- System-aware Fluent theme foundation

### Local persistence

- SQLite-backed native calculation history implementation
- Search, recent history, favorite state, delete, and clear operations
- Persistence exposed behind an interface so other targets can use different storage backends

## Planned modes and platform work

The master project target includes Standard, Scientific, Programmer, Converter, Graphing, Statistics, Equations, Matrices/Vectors, History, Settings, and supporting utilities. Platform goals include Windows, Linux, macOS, Android, iOS, and Browser/WebAssembly where supported by the selected stable .NET/Avalonia toolchain.

Not every planned mode or platform head is complete yet. Exact implementation and validation status is tracked in [`PROJECT_STATE.md`](PROJECT_STATE.md) and [`docs/ROADMAP.md`](docs/ROADMAP.md).

## Architecture

CalcNova uses modular dependency direction:

```text
Platform heads
     │
     ▼
CalcNova.App
     │
     ├─────────────┬─────────────┐
     ▼             ▼             ▼
CalcNova.Core   feature libs   persistence abstractions/implementations
```

Mathematical/domain projects do not depend on Avalonia UI. Platform heads should stay thin and contain startup, packaging, lifecycle, permissions, and unavoidable native integration.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Repository layout

```text
CalcNova/
├── .github/
├── docs/
├── src/
│   ├── CalcNova.App/
│   ├── CalcNova.Core/
│   ├── CalcNova.Scientific/
│   ├── CalcNova.Programmer/
│   ├── CalcNova.Converter/
│   ├── CalcNova.Persistence/
│   └── CalcNova.Desktop/
├── tests/
├── tools/
├── CalcNova.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── PROJECT_STATE.md
└── what_changed.md
```

The structure will expand as additional stable modules and platform heads are implemented.

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

This command requires a working .NET/Avalonia desktop environment.

## Correctness and validation policy

CalcNova does not use arbitrary code execution to evaluate expressions. Calculation syntax is tokenized, parsed, and evaluated by project-owned domain code.

A feature is not considered validated merely because source code exists. Build/test/platform status must be based on commands that actually ran. When an environment is unavailable, project state records `NOT RUN` instead of a fabricated PASS.

See [`docs/CALCULATION_ENGINE.md`](docs/CALCULATION_ENGINE.md) and [`docs/TESTING.md`](docs/TESTING.md).

## Privacy

Core calculations are designed to operate locally. The project does not require an account for ordinary calculation features, and the open-source base does not intentionally include advertising or behavioral-tracking SDKs.

Network-enhanced features such as future currency rates must remain optional and independently controllable.

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

For exact development progress, recent commits, unresolved issues, and the next continuation tasks, read [`what_changed.md`](what_changed.md) and [`PROJECT_STATE.md`](PROJECT_STATE.md).
