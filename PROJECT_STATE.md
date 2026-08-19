# CalcNova Project State

## Current Version

`0.1.0-dev`

## Current Branch

`main`

## Current Phase

Broad feature completion + cross-platform validation hardening. The main calculator, scientific, programmer, converter, graphing, utility, persistence, and platform-composition foundations are implemented. Recent continuation work repaired app-layer contracts, completed productivity backends, exposed them in shared Avalonia XAML, and strengthened source-level UI validation. Remaining work is now concentrated on adaptive/mobile polish, deeper accessibility validation, automated UI/integration coverage, observed build/test results, platform/package validation, localization, onboarding, and release hardening.

## Master Technical Direction

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Pure C# calculation/domain projects independent of Avalonia UI
- Thin platform-specific composition heads
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
- Sanitized imported expression text with normalization for common calculator glyphs
- Platform-safe clipboard contract
- User-triggered sanitized paste and result-copy commands
- Avalonia `TopLevel` clipboard adapter composed on Desktop, Browser, Android, and iOS

### Programmer mode

- Base 2–36 parsing and formatting
- Full base 2–36 selector in the shared UI
- Binary/octal/decimal/hex synchronized representation support
- Fixed-width signed/unsigned interpretation
- Correct two's-complement signed decimal display with masked non-decimal representations
- AND, OR, XOR, NOT
- Left, logical-right, and arithmetic-right shifts
- Fixed-width bit-string visualization
- Full word-size interactive bit grid for 8/16/32/64/128-bit presets
- Byte-grouped bit view-model collections for 8/16/32/64/128-bit presets
- Shared copy actions for binary/octal/decimal/hex/fixed-width bit representations
- Accessible bit-cell labels
- Unicode scalar/code-point parsing, formatting, text conversion, and bounded sequence inspection
- Shared Unicode decode/inspection copy actions
- Dedicated Unicode code-point shared UI

### Conversion and utilities

- Offline fixed-unit conversion catalog across major physical/data categories
- Swap workflow
- Reusable validated conversion-pair model
- Bounded recent conversion-pair tracking
- Favorite conversion pairs
- Versioned persisted conversion-pair tokens
- Persisted recent/favorite converter state across launches
- User-selectable and persisted 1–17 significant-digit result precision
- Shared UI for recents, favorites, precision, and pair restoration
- Shared category-scoped unit search workflow
- Shared search-result assignment to From/To units
- Shared change-aware clear-recents action
- Shared conversion-result clipboard copy action
- Optional currency-rate provider/cache architecture with offline fallback semantics
- Date difference, calendar arithmetic, business-day, and fixed-duration utilities

### Advanced mathematics

- Statistics module and shared view model
- Equation-solving module and shared view model
- Matrix utilities and shared view model
- Graph sampling with discontinuity segmentation
- Explicit graph viewport model
- Interactive Avalonia plot control
- Deterministic accessible SVG graph export engine
- Shared SVG generation/copy workflow
- Bounded central-difference derivative analysis
- Bracketed bisection root finding
- Bounded Simpson numerical integration
- Shared derivative/root/integral controls with approximate-result labeling
- Shared nearest sampled-point graph trace workflow
- Shared bounded graph table-of-values CSV workflow
- Shared bounded multi-expression sampling using stable generated series identities
- Shared identified multi-expression CSV export/copy workflow

### Persistence and application architecture

- Calculation-history repository abstraction
- SQLite-backed native history implementation
- Browser-compatible history/storage path
- Search, recent history, favorites, delete, and clear flows
- TXT/CSV/JSON history export engine
- Settings/preferences abstraction and shared settings view model
- Serialized converter preferences in shared settings
- Settings validation for converter precision/token bounds
- Shared application composition root
- About/external-link abstraction
- Shared clipboard abstraction and Avalonia adapter
- Shared clipboard dependency injection into Calculator, Programmer, Unicode, Converter, and Graphing modes
- Desktop composition
- Browser composition
- Android composition
- iOS composition

### Platform and repository infrastructure

- Desktop Avalonia head
- Browser/WebAssembly head
- Android head
- iOS head
- GitHub Actions workflows for core validation, formatting, docs, coverage, security, advanced utilities, release, UI contracts, and platform-target builds
- Dependency vulnerability audit workflow
- Repository/documentation/branding validation helpers
- Source-level Avalonia XML well-formedness validation
- Source-level shared command/property binding validation
- Release workflow foundation
- Original branding asset source and verification helpers

## Shared UI Status

The shared Avalonia shell currently exposes these principal modes or utilities:

- Standard + Scientific calculator
- Sanitized paste and copy-result actions
- Programmer calculator with full radix selector, bitwise operations, shifts, byte-grouped bit grid, and radix/fixed-width copy actions
- Unicode code-point tools with result-copy actions
- Offline unit converter with precision, search, search-result assignment, recents, favorites, clear-recents, and result copy
- Statistics
- Equations
- Matrices
- Graphing with sampling, preview/table copy, nearest-point trace, derivative/root/integral analysis, bounded multi-expression sampling/export, and accessible SVG generation/copy
- Date/time utilities
- Currency conversion
- History
- Settings
- About/support

The new productivity controls are source-wired and protected by the shared UI contract validator. They are **not** considered runtime-validated until Avalonia compilation/UI tests and target accessibility checks are observed.

## Remaining High-Priority Work

1. Finish adaptive navigation and compact/mobile layouts across every mode.
2. Perform a full accessibility pass: keyboard traversal, screen-reader behavior, focus visibility, contrast, target sizes, large text, and reduced-motion behavior.
3. Add stable UI/integration automation for the shared Avalonia shell.
4. Observe real GitHub Actions/build/test output from a suitable execution path and fix any compiler/analyzer/test failures.
5. Validate all platform heads with their required SDK/workload environments and fix any target-specific compilation/packaging issues.
6. Complete Android/iOS store packaging metadata, signing guidance, and release documentation.
7. Complete Windows/Linux/macOS packaging validation and release artifact checks.
8. Expand localization infrastructure and reviewed language packs.
9. Complete onboarding/first-run polish and final design-system consolidation.
10. Run the final release-gate audit covering build, tests, formatting, security, docs, assets, accessibility, privacy, and packaging.

## Known Issues / Risks

1. **The .NET SDK is unavailable in the active execution environment used for this continuation.** New changes made here have therefore not been locally compiled or tested.
2. A source change or test file is never treated as validated merely because it exists; actual workflow/build/test results still need to be observed.
3. Avalonia XAML and platform-specific package/workload integration must be validated by actual workflows or suitable target machines before release readiness is claimed.
4. Clipboard integration is explicitly user-triggered and attached to Avalonia `TopLevel`; target-specific runtime behavior still requires platform validation.
5. Numerical graph analysis is intentionally approximate and bounded; UI and documentation must continue labeling it as approximate.
6. Converter pair persistence uses versioned unit-ID tokens. Unknown/obsolete tokens are ignored by the converter restore layer, while settings storage also enforces count/length bounds.
7. Large 64/128-bit interactive grids may require additional compact-layout or virtualization polish on narrow/mobile screens even though byte grouping is now exposed in shared XAML.
8. Source-level XAML/XML and binding-contract checks reduce regression risk but do not replace Avalonia XAML compilation, real UI automation, or screen-reader/touch testing.

## Validation Status

### Source/test coverage present

- Core tests: IMPLEMENTED
- App/view-model tests: IMPLEMENTED
- Programmer tests: IMPLEMENTED
- Converter tests: IMPLEMENTED
- Persistence/settings tests: IMPLEMENTED
- Graphing tests: IMPLEMENTED
- Statistics tests: IMPLEMENTED
- Equations tests: IMPLEMENTED
- Matrices tests: IMPLEMENTED
- Currency tests: IMPLEMENTED
- Date/time tests: IMPLEMENTED
- Platform abstraction tests: IMPLEMENTED
- Shared XAML command/property contract validator: IMPLEMENTED
- Avalonia XAML XML well-formedness validator: IMPLEMENTED

### This continuation execution

- Local restore: **NOT RUN — .NET SDK unavailable**
- Local formatting verification: **NOT RUN — .NET SDK unavailable**
- Local build: **NOT RUN — .NET SDK unavailable**
- Local tests: **NOT RUN — .NET SDK unavailable**
- Local repository UI validators: **NOT RUN against a local checkout in this execution**
- Android build/package validation: **NOT RUN in this execution**
- iOS build/package validation: **NOT RUN in this execution**
- Browser/WebAssembly build validation: **NOT RUN in this execution**
- Desktop OS packaging validation: **NOT RUN in this execution**
- GitHub Actions result observation for current direct-push commits: **NOT CONFIRMED in this execution**

A check is never marked PASS unless it actually ran and its result was observed.

## Continuation Priorities

The next development pass should begin with the first still-incomplete item below rather than recreating completed modules:

1. Adaptive/mobile layout pass for Calculator, Programmer, Converter, Graph, Date, FX, and advanced modes.
2. Accessibility audit and screen-reader/focus/keyboard refinements, especially the large bit grid and graph experience.
3. Inspect actual GitHub Actions/check results and fix any real compile/analyzer/test failures.
4. Add reliable shared-shell UI/integration tests where the Avalonia test stack is stable.
5. Add remaining low-risk productivity polish such as export/copy actions only where the user flow and platform abstraction are clear.
6. Expand localization/onboarding only after core UI validation is stable.
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
