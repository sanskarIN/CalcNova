<p align="center">
  <img src="assets/branding/calcnova-logo.svg" alt="CalcNova logo" width="180" />
</p>

# CalcNova

<p align="center"><strong>Fast. Precise. Private. Everywhere.</strong></p>

CalcNova is an open-source, privacy-first cross-platform calculator built with **C#**, **.NET**, and **Avalonia UI**. It combines everyday calculation, scientific mathematics, programmer tooling, graphing, unit conversion, statistics, equation solving, matrix/vector utilities, date/duration tools, local history, settings, and an optional credential-free currency architecture in one modular application.

> CalcNova is under active validation. Source implementation does not automatically mean a platform/release is production-ready. Exact build/test status and limitations are recorded in [`PROJECT_STATE.md`](PROJECT_STATE.md) and [`docs/PLATFORM_SUPPORT.md`](docs/PLATFORM_SUPPORT.md).

## Status

[![Build and Test](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-test.yml)
[![Formatting](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/format.yml)
[![Desktop Build](https://github.com/sanskarIN/CalcNova/actions/workflows/build-desktop.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-desktop.yml)
[![Android Build](https://github.com/sanskarIN/CalcNova/actions/workflows/build-android.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-android.yml)
[![Browser Build](https://github.com/sanskarIN/CalcNova/actions/workflows/build-browser.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-browser.yml)
[![iOS Simulator Build](https://github.com/sanskarIN/CalcNova/actions/workflows/build-ios.yml/badge.svg)](https://github.com/sanskarIN/CalcNova/actions/workflows/build-ios.yml)
[![License](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)

## Highlights

- **Safe expression engine** — project-owned tokenizer/parser/evaluator; no arbitrary-code `eval` path.
- **Numeric strategy** — arbitrary-precision integers, decimal arithmetic where appropriate, and finite floating-point evaluation for transcendental functions.
- **Standard + Scientific** — precedence, parentheses, powers, roots, logarithms, trig/inverse/hyperbolic functions, constants, factorial/combinatorics, contextual percentage, repeated equals, ±, and classic memory.
- **Programmer** — bases 2–36, binary/octal/decimal/hex views, two's-complement interpretation, word sizes, bitwise and shift operations.
- **Offline Unit Converter** — fixed physical conversions across major categories without network access.
- **Graphing** — safe expression sampling, discontinuity segmentation, axes/grid, pan/zoom, fit/reset, coordinate display, SVG export engine.
- **Statistics** — descriptive statistics, quartiles/percentiles, compensated sum, editable pasted datasets.
- **Equations** — linear/quadratic solving, complex roots, degenerate cases, bounded numerical bisection.
- **Matrices + Vectors** — determinant, inverse, rank, systems, transpose, arithmetic, magnitude/dot/cross operations.
- **Date + Duration** — timezone-free `DateOnly` calculations, calendar arithmetic, business days, fixed-duration conversion.
- **History** — local persistence, search, favorites, delete, confirm-before-clear, and user-initiated CSV export.
- **Settings** — Light/Dark/System, angle mode, result precision/grouping, history controls, reduced-motion/high-contrast/haptics preferences.
- **Optional Currency Architecture** — replaceable provider/cache interfaces, rate timestamps, stale-cache fallback, no embedded API key.
- **Privacy-first** — no account requirement for ordinary calculations, no ad SDK, no behavioral analytics SDK, and local persistence by default.
- **Cross-platform shared UI** — one modular Avalonia `MainView` reused by Desktop, Android, iOS, and Browser heads.

For the detailed implementation matrix, see [`docs/FEATURES.md`](docs/FEATURES.md).

## Calculator input

The shared calculator supports mouse/touch input plus keyboard/numpad routing. Important shortcuts include:

| Input | Action |
|---|---|
| Enter | Evaluate / repeated equals |
| Escape | Clear |
| Backspace | Remove last character when the calculator surface owns the key |
| F9 | Toggle sign |
| Numpad digits/operators | Calculator input |
| Ctrl/Cmd+C | Copy result (outside an active text editor) |
| Ctrl/Cmd+V | Paste a length-bounded expression (outside an active text editor) |

The touch UI also exposes copy-result, copy-expression, and paste-expression actions. See [`docs/KEYBOARD_SHORTCUTS.md`](docs/KEYBOARD_SHORTCUTS.md).

## Supported source targets

CalcNova currently contains source heads for:

- **Windows** desktop
- **Linux** desktop
- **macOS** desktop
- **Android**
- **iOS**
- **Browser/WebAssembly + PWA**

The platform heads stay thin. Ordinary calculations and mathematical modules are shared. Native targets use local SQLite history plus JSON settings/currency cache; Browser uses `localStorage` implementations behind the same platform-neutral contracts.

Platform source availability is not the same as a validated stable release. See [`docs/PLATFORM_SUPPORT.md`](docs/PLATFORM_SUPPORT.md).

## Architecture

```text
CalcNova.Desktop ─┐
CalcNova.Android ─┤
CalcNova.iOS ─────┤
CalcNova.Browser ─┤
                  ▼
             CalcNova.App
                  │
        ┌─────────┼───────────────────────┐
        ▼         ▼                       ▼
 CalcNova.Core  feature libraries   CalcNova.Platform
                                      │
                                      ▼
                              persistence implementations
```

Domain projects do not depend on Avalonia. The shared UI is split into focused mode views under `src/CalcNova.App/Views/Modes/`, while platform heads contain startup, local storage composition, packaging, and unavoidable native integration.

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md).

## Repository layout

```text
CalcNova/
├── .github/
│   ├── ISSUE_TEMPLATE/
│   └── workflows/
├── assets/
│   ├── branding/
│   └── icons/
├── docs/
├── packaging/
├── src/
│   ├── CalcNova.App/
│   ├── CalcNova.Core/
│   ├── CalcNova.Scientific/
│   ├── CalcNova.Programmer/
│   ├── CalcNova.Converter/
│   ├── CalcNova.Statistics/
│   ├── CalcNova.Equations/
│   ├── CalcNova.Matrices/
│   ├── CalcNova.Graphing/
│   ├── CalcNova.DateTime/
│   ├── CalcNova.Currency/
│   ├── CalcNova.Platform/
│   ├── CalcNova.Persistence/
│   ├── CalcNova.Desktop/
│   ├── CalcNova.Android/
│   ├── CalcNova.iOS/
│   └── CalcNova.Browser/
├── tests/
├── tools/
├── CalcNova.slnx
├── CalcNova.All.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── PROJECT_STATE.md
└── what_changed.md
```

## Toolchain

The repository pins a stable .NET 10 SDK feature band in [`global.json`](global.json) and centrally manages package versions in [`Directory.Packages.props`](Directory.Packages.props).

Core verification sequence:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

The full all-target graph is also recorded in `CalcNova.All.slnx`, but platform workload builds are intentionally validated by separate workflows so an unavailable mobile/browser workload does not hide core results.

See [`docs/BUILDING.md`](docs/BUILDING.md), [`docs/TESTING.md`](docs/TESTING.md), and [`docs/VALIDATION_BASELINE.md`](docs/VALIDATION_BASELINE.md).

## Run desktop

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

This requires a compatible .NET/Avalonia desktop environment.

## Platform builds

Examples:

```bash
# Desktop
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj -c Release

# Android workload required
dotnet build src/CalcNova.Android/CalcNova.Android.csproj -c Release

# Browser wasm-tools workload required
dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj -c Release
```

iOS builds require macOS plus the iOS workload/Xcode environment. Signing, notarization, Play Store/App Store credentials, keystores, and private keys are intentionally not stored in Git.

## Original branding and packaging

CalcNova branding is project-owned and stored under `assets/`. The repository includes:

- master CalcNova SVG logo;
- support badge artwork;
- social-preview source;
- Android adaptive icon/splash resources;
- Browser/PWA icons;
- deterministic icon/raster generation tooling;
- Windows/Linux/macOS packaging metadata and helper scripts.

See [`assets/ASSET_LICENSES.md`](assets/ASSET_LICENSES.md) and [`docs/DESIGN_SYSTEM.md`](docs/DESIGN_SYSTEM.md).

## Correctness policy

CalcNova does not claim to be mathematically bug-free forever. Milestones target:

- no known release-blocking defects;
- explicit numeric limitations;
- typed failures instead of plausible-looking invalid results;
- regression tests for confirmed bugs;
- formatter/analyzer/build/test validation;
- platform status based on commands that actually ran.

When an environment is unavailable, state files use **NOT RUN** rather than a fabricated PASS.

See [`docs/CALCULATION_ENGINE.md`](docs/CALCULATION_ENGINE.md).

## Privacy

Core calculations, fixed-unit conversion, graph sampling, statistics, equations, matrices, date utilities, settings, and history are designed to work locally. CalcNova's open-source base includes no advertising SDK and no behavioral tracking SDK by default.

Currency is isolated as an optional network-enhanced architecture. A live rate provider is not hard-coded into the open-source client, and no secret API key is embedded. Cached rates are timestamped and surfaced as cached/stale when appropriate.

See [`docs/PRIVACY.md`](docs/PRIVACY.md) and [`docs/SECURITY.md`](docs/SECURITY.md).

## Contributing

Contributions are welcome. Start with:

- [`CONTRIBUTING.md`](CONTRIBUTING.md)
- [`CODE_OF_CONDUCT.md`](CODE_OF_CONDUCT.md)
- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)
- [`docs/TESTING.md`](docs/TESTING.md)
- [`docs/ROADMAP.md`](docs/ROADMAP.md)

Small, focused changes with relevant tests, accessibility consideration, and documentation updates are preferred.

## Security

Do not disclose sensitive vulnerabilities in a public issue. Follow [`SECURITY.md`](SECURITY.md) for reporting guidance.

**Support/security contact:** `supportramsandesh@gmail.com`

## License

CalcNova is licensed under the [Apache License 2.0](LICENSE). Third-party packages/assets remain subject to their respective licenses and notices.

## Project links

- **Repository:** https://github.com/sanskarIN/CalcNova
- **GitHub profile:** https://www.github.com/sanskarIN
- **Business:** sanskarin@outlook.in
- **Business:** sanskarin.business@gmail.com
- **Support:** supportramsandesh@gmail.com

## Support CalcNova

<p>
  <img src="assets/branding/buy-me-a-coffee-support.svg" alt="Buy Me a Coffee — @sanskarIN" width="260" />
</p>

**Buy Me a Coffee:** https://buymeacoffee.com/sanskarIN

Support is optional. CalcNova core functionality must never be blocked, degraded, or interrupted by donation prompts.

---

For current implementation/validation status and exact continuation tasks, read [`what_changed.md`](what_changed.md) and [`PROJECT_STATE.md`](PROJECT_STATE.md).
