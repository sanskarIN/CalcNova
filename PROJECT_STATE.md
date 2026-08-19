# CalcNova Project State

## Current Version

`0.1.0-dev`

## Current Branch

`main`

## Current Phase

Broad feature implementation + cross-platform heads + validation hardening. The repository has moved well beyond the original foundation phase; remaining work is primarily UI completion, platform validation, accessibility, packaging/release polish, and integration of newer domain capabilities into the shared shell.

## Master Technical Direction

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Pure C# calculation/domain projects independent of Avalonia UI
- Platform-specific composition kept in thin platform heads
- Native SQLite persistence behind abstractions
- Browser-safe storage implementations separated from native SQLite
- Optional network-enhanced currency conversion with no embedded credentials
- Apache-2.0

## Implemented Source Foundations

### Core calculator

- Typed calculation errors and workload limits
- Mixed numeric representation using `BigInteger`, `decimal`, and bounded `double` fallback
- Safe tokenizer and recursive-descent parser
- Standard arithmetic, parentheses, unary operators, and right-associative exponentiation
- Scientific constants/functions and degree/radian/gradian angle modes
- Repeated-equals calculation session behavior
- Calculator-style percentage transformation separate from expression-language modulo
- Classic memory operations: MC, MR, MS, M+, M-
- Sanitized external expression text import with normalization for common calculator glyphs

### Programmer mode

- Base 2–36 parsing and formatting
- Binary/octal/decimal/hex synchronized representation support
- Bounded signed/unsigned word-size interpretation
- AND, OR, XOR, NOT, logical/arithmetic shifts
- Fixed-width bit-string visualization
- Bounded bit inspection/toggle helpers and view-model integration
- Unicode scalar/code-point parsing, formatting, text conversion, and bounded sequence inspection

### Conversion and utilities

- Offline fixed-unit conversion catalog across major physical/data categories
- Swap workflow
- Reusable conversion-pair model
- Bounded recent conversion-pair tracking
- Favorite conversion pairs
- User-selectable 1–17 significant-digit result precision
- Optional currency rate provider/cache architecture with offline fallback semantics
- Date difference, calendar arithmetic, business-day, and fixed-duration utilities

### Advanced mathematics

- Statistics module and shared view model
- Equation-solving module and shared view model
- Matrix utilities and shared view model
- Graph sampling with discontinuity segmentation
- Explicit graph viewport model
- Interactive Avalonia plot control
- Deterministic SVG graph export
- Bounded numerical derivative analysis
- Bracketed bisection root finding
- Simpson numerical integration with workload limits
- Shared graph-analysis view-model commands

### Persistence and application architecture

- Calculation-history repository abstraction
- SQLite-backed native history implementation
- Browser-compatible history/storage path
- Search, recent history, favorites, delete, and clear flows
- TXT/CSV/JSON history export
- Settings/preferences abstraction and shared settings view model
- Shared application composition root
- About/external-link abstraction
- Desktop composition
- Browser composition
- Android composition
- iOS composition

### Platform and repository infrastructure

- Desktop Avalonia head
- Browser/WebAssembly head
- Android head
- iOS head
- GitHub Actions workflows for core validation, formatting, docs, coverage, security, advanced utilities, and platform-target builds
- Dependency vulnerability audit workflow
- Repository/documentation/branding validation helpers
- Release workflow foundation
- Original branding asset source and verification helpers

## Shared UI Status

The shared Avalonia shell currently exposes these principal modes or utilities:

- Standard + Scientific calculator
- Programmer calculator
- Offline unit converter
- Statistics
- Equations
- Matrices
- Graphing
- Date/time utilities
- Currency conversion
- History
- Settings
- About/support

Some newly added capabilities are implemented in domain/view-model code but still need dedicated visible controls in the shared XAML shell, especially:

- programmer bit-toggle grid
- Unicode code-point helper UI
- converter favorite/recent-pair selectors and precision control
- graph derivative/root/integral controls
- explicit sanitized clipboard paste wiring

## Remaining High-Priority Work

1. Complete visible UI integration for capabilities that are currently domain/view-model ready.
2. Finish adaptive navigation and compact/mobile layouts across every mode.
3. Perform a full accessibility pass: keyboard traversal, screen-reader labels, focus visibility, contrast, target sizes, and reduced-motion behavior.
4. Add UI/integration automation where the selected Avalonia test stack is stable.
5. Validate all platform heads with their required SDK/workload environments and fix any target-specific compilation/packaging issues.
6. Complete Android/iOS/store packaging metadata and signed release documentation.
7. Complete Windows/Linux/macOS packaging validation and release artifact checks.
8. Expand localization infrastructure and reviewed language packs.
9. Complete onboarding/first-run polish and final design-system consolidation.
10. Run final release-gate audit covering build, tests, formatting, security, docs, assets, accessibility, privacy, and packaging.

## Known Issues / Risks

1. **The .NET SDK is unavailable in the active execution environment used for this continuation.** New changes made here have therefore not been locally compiled or tested.
2. GitHub's combined-status endpoint currently exposes no status checks for the latest continuation commit; no CI result is being inferred or fabricated.
3. Avalonia XAML and platform-specific package/workload integration must be validated by actual workflows or suitable target machines before release readiness is claimed.
4. Newly implemented converter favorites/recent pairs are currently in-memory view-model state; persistence across launches remains future work.
5. Sanitized expression import is available through the calculator view model, but native/shared clipboard event wiring still needs explicit UI integration.
6. Numerical graph analysis is intentionally approximate and bounded; UI must continue labeling results as approximate.

## Validation Status

### Source/test coverage present

- Core tests: IMPLEMENTED
- App/view-model tests: IMPLEMENTED
- Programmer tests: IMPLEMENTED
- Converter tests: IMPLEMENTED
- Persistence tests: IMPLEMENTED
- Graphing tests: IMPLEMENTED
- Statistics tests: IMPLEMENTED
- Equations tests: IMPLEMENTED
- Matrices tests: IMPLEMENTED
- Currency tests: IMPLEMENTED
- Date/time tests: IMPLEMENTED
- Platform abstraction tests: IMPLEMENTED

### This continuation execution

- Local restore: **NOT RUN — .NET SDK unavailable**
- Local formatting verification: **NOT RUN — .NET SDK unavailable**
- Local build: **NOT RUN — .NET SDK unavailable**
- Local tests: **NOT RUN — .NET SDK unavailable**
- Android build/package validation: **NOT RUN in this execution**
- iOS build/package validation: **NOT RUN in this execution**
- Browser/WebAssembly build validation: **NOT RUN in this execution**
- Desktop OS packaging validation: **NOT RUN in this execution**

A check is never marked PASS unless it actually ran and its result was observed.

## Continuation Priorities

The next development pass should begin with the first still-incomplete item below rather than recreating completed modules:

1. Wire the new graph numerical-analysis controls into the shared Graph tab.
2. Add visible programmer bit-toggle and code-point tooling.
3. Add converter precision/favorites/recent-pair controls and decide persistence semantics.
4. Add sanitized clipboard-paste/copy integration behind a platform-safe abstraction.
5. Expand adaptive/mobile UI behavior and accessibility metadata.
6. Inspect actual GitHub Actions results and fix any real compile/analyzer/test failures before release expansion.
7. Continue store/package/release hardening only after observed CI/platform validation.

## Important Paths

- `src/CalcNova.Core/`
- `src/CalcNova.Scientific/`
- `src/CalcNova.Programmer/`
- `src/CalcNova.Converter/`
- `src/CalcNova.Currency/`
- `src/CalcNova.DateTime/`
- `src/CalcNova.Graphing/`
- `src/CalcNova.Statistics/`
- `src/CalcNova.Equations/`
- `src/CalcNova.Matrices/`
- `src/CalcNova.Persistence/`
- `src/CalcNova.Platform/`
- `src/CalcNova.App/`
- `src/CalcNova.Desktop/`
- `src/CalcNova.Browser/`
- `src/CalcNova.Android/`
- `src/CalcNova.iOS/`
- `tests/`
- `.github/workflows/`
- `docs/`
- `what_changed.md`

## Continuation Rule

Before new development, read this file and `what_changed.md`, inspect current `main`, and continue the first incomplete task. Do not recreate completed files, reset the repository, or report unexecuted validation as passing.
