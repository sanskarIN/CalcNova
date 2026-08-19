# CalcNova Project State

## Current Version

`0.1.0-dev`

## Current Branch

`main`

## Current Phase

Source implementation and release-contract hardening are substantially complete for the current `0.1.0-dev` scope. Core calculation, scientific, programmer, converter, date/time, currency, statistics, equations, matrices, graphing, history, settings, onboarding, localization foundations, platform composition, source validation, release workflows, and focused Avalonia headless UI automation are implemented in source.

The dominant remaining blockers are no longer missing core modules. They are **observed execution evidence**: real .NET restore/build/test results, compiled Avalonia/headless results, target-platform builds, device/browser accessibility and adaptive-layout validation, signing/archive/store checks, and the broader visible-XAML localization migration.

## Master Technical Direction

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Project-owned parser/evaluator rather than arbitrary code execution
- Pure calculation/domain libraries kept independent of Avalonia where practical
- Thin Desktop, Browser/WebAssembly, Android, and iOS composition heads
- Native SQLite history behind abstractions
- Browser-safe history/settings storage separated from native SQLite
- Versioned local settings schema with explicit migration behavior
- Optional network-enhanced currency conversion with no embedded provider credentials
- Apache-2.0

## Implemented Source Foundations

### Core calculator and scientific mode

- Typed calculation errors and workload limits
- Mixed numeric representation using `BigInteger`, `decimal`, and bounded floating-point fallback
- Safe tokenizer and recursive-descent parser
- Standard arithmetic, parentheses, unary operators, and right-associative exponentiation
- Scientific constants/functions and degree/radian/gradian angle modes
- Calculator-style percentage transformation separate from expression-language modulo
- Repeated-equals calculation-session behavior
- Classic memory operations: MC, MR, MS, M+, M-
- Sanitized imported expression text with common calculator-glyph normalization
- User-triggered sanitized clipboard paste
- Explicit result copy
- Top-row/numpad digit and numpad arithmetic mappings outside active text fields
- Selection-aware keypad editing:
  - insertion at the tracked caret;
  - replacement of forward/reversed selections;
  - Backspace selection deletion;
  - Backspace before caret;
  - bounded/clamped selection indexes;
  - post-edit caret restoration to the shared TextBox
- Shared TextBox selection synchronization after keyboard and pointer selection changes

### Programmer and Unicode tools

- Base 2–36 parsing and formatting
- Full base 2–36 selector in the shared UI
- Binary/octal/decimal/hex synchronized representations
- Fixed-width signed/unsigned interpretation
- Correct masked non-decimal presentation with signed-decimal interpretation
- AND, OR, XOR, NOT
- Left, logical-right, and arithmetic-right shifts
- Fixed-width bit-string visualization
- Full 8/16/32/64/128-bit interactive bit grid
- Byte-grouped shared presentation
- Copy actions for binary/octal/decimal/hex/fixed-width bits
- Accessible bit-cell state labels
- Unicode scalar/code-point parsing, formatting, text conversion, and bounded sequence inspection
- Shared Unicode result-copy actions

### Converter and utility modules

- Offline fixed-unit conversion catalog across major physical/data categories
- Swap workflow
- Validated conversion-pair model
- Bounded recent-pair tracking
- Favorite conversion pairs
- Versioned pair-persistence tokens
- Persisted recent/favorite converter state
- User-selectable/persisted 1–17 significant-digit result precision
- Shared recents/favorites/precision restoration controls
- Category-scoped unit search
- Search-result assignment to From/To units
- Change-aware clear-recents action
- Conversion-result clipboard copy
- Optional currency provider/cache architecture with offline fallback semantics
- Date difference, calendar arithmetic, business-day, and fixed-duration utilities

### Advanced mathematics and graphing

- Statistics module and shared view model
- Statistics-summary copy action
- Equation-solving module and shared view model
- Matrix utilities and shared view model
- Matrix-result copy action
- Graph sampling with discontinuity segmentation
- Explicit graph viewport model
- Interactive/focusable Avalonia plot control
- Pointer drag pan, wheel zoom, and double-tap/double-click fit-to-data
- Keyboard arrow-key panning
- Keyboard numpad Add/Subtract zoom
- Keyboard Home reset
- Keyboard `F` fit-to-data
- Read-only viewport snapshot for deterministic UI assertions
- Deterministic accessible SVG export engine and copy workflow
- Bounded central-difference derivative analysis
- Bracketed bisection root finding
- Bounded Simpson numerical integration
- Shared approximate derivative/root/integral controls
- Nearest sampled-point tracing
- Bounded single-expression table-of-values CSV
- Bounded multi-expression sampling with stable generated identities
- Identified multi-expression CSV export/copy

### History and persistence

- Calculation-history repository abstraction
- SQLite-backed native history implementation
- Browser-compatible history/storage path
- Search, recent history, favorites, delete, and clear flows
- Bounded TXT/CSV/JSON history export engine
- Shared export-format selection, preview, and clipboard-copy workflow
- Settings repository abstraction
- Shared settings view model
- Persisted converter and culture preferences
- Explicit `AppSettingsSchema` version boundary
- Legacy schema-zero migration
- Detection/migration of truly historical JSON with **no** `schemaVersion` property
- Fail-closed rejection of corrupt negative and unsupported future schema versions
- Shared `AppSettingsJson` decoder used by native and Browser storage
- Shared `AppSettingsValidator` used by native and Browser storage
- Central validation of culture, decimal precision, history bounds, onboarding version, converter precision, and converter-token bounds
- Native JSON temporary-file replacement behavior retained

### Localization

- Stable semantic `AppStringKey` catalog
- Complete English semantic catalog for the current key set
- Complete Hindi semantic catalog for the current key set
- English/Hindi regional culture selection including forms such as `en-IN` and `hi-IN`
- Persisted culture preference
- Multi-catalog completeness/duplicate/unknown-key validation

The shared XAML remains predominantly English. The Hindi semantic catalog is therefore an implemented localization foundation, **not** a claim that the complete visible UI is already translated.

### Accessibility, adaptive layout, and onboarding

- Shared 44-DIP minimum interaction-target baseline
- 54-DIP standard calculator-key baseline
- Compact/medium/expanded available-width profiles
- Compact horizontal-overflow fallback for wide shared surfaces
- Focus-change bring-into-view behavior on shared scroll containers
- Explicit focused-state border emphasis for common keyboard controls
- Stronger focused-state emphasis under CalcNova high contrast
- High-contrast and reduced-motion shell state classes
- Shared keyboard mode navigation using Ctrl+PageUp/PageDown/Home/End
- Accessible symbol/button names and programmer bit-state names
- Onboarding shortcut suppression while the overlay is visible
- Onboarding focus handoff and calculator focus restoration source behavior
- Onboarding copy now documents cyclic and first/last keyboard mode navigation
- Runtime accessibility evidence matrix with PASS / FAIL / BLOCKED / NOT RUN vocabulary

### Application and platform architecture

- Shared application composition root
- About/external-link abstraction
- Shared clipboard abstraction and Avalonia adapter
- Clipboard dependency injection into current copy-enabled modes
- Desktop composition
- Browser/WebAssembly composition
- Android composition
- iOS composition
- Shared mode-selection API with cyclic normalization and deterministic first/last selection
- Transient invalid `TabControl` selection values ignored rather than wrapped into another mode

## UI Automation Status

Focused Avalonia headless UI automation is implemented in `CalcNova.App.Tests` using the repository-matched `Avalonia.Headless.XUnit` 12.1.1 package and xUnit v3.

Current headless source scenarios include:

- shared shell loads every primary mode;
- Calculator clear control executes its real bound command;
- selection-aware keypad replacement restores the TextBox caret;
- compact-width window applies the compact adaptive class;
- Ctrl+PageDown advances shared mode selection;
- high-contrast preference applies the shell class;
- onboarding is visible for a new/default state and hides after Skip;
- graph arrow-key panning updates viewport state;
- graph numpad zoom updates viewport span;
- graph Home resets the viewport;
- graph `F` fits finite sampled data.

A dedicated `.NET 10` workflow restores and runs the App test project. The headless test source and CI execution path are additionally protected by SDK-independent source validation.

These tests are **not considered PASS** merely because they exist. A compiled/headless execution result still needs to be observed.

## Repository and CI Infrastructure

Implemented SDK-independent validators/workflows cover:

- repository structure/security source checks;
- Avalonia XAML XML well-formedness;
- shared UI command/property contracts;
- navigation contracts;
- calculator/shared-shell keyboard contracts;
- calculator selection-editing contracts;
- graph keyboard contracts;
- headless UI-test setup/scenario/execution-path contracts;
- accessibility markup;
- focus visibility;
- accessibility runtime-evidence discipline;
- adaptive layout;
- touch targets;
- localization catalogs/preferences;
- settings schema/shared codec/shared validator architecture;
- onboarding persistence/visual/focus contracts;
- packaging metadata;
- Desktop/Browser/Android/iOS build-workflow contracts;
- tag-first release workflow contracts;
- release documentation/evidence contracts;
- release-tag validation;
- Python regression tests for the source validators;
- integrated SDK-independent source preflight inventory.

Additional release/platform source hardening includes:

- release workflow validates the exact detached release tag before .NET restore/build/test;
- Desktop release artifacts for Windows/Linux/macOS source paths;
- Browser release publish path;
- Android signed-AAB path only when external signing secrets are configured;
- temporary Android keystore cleanup;
- tag-specific unsigned iOS simulator validation workflow on macOS;
- focused contract validation for the iOS release-tag workflow;
- checksum generation for release artifacts;
- existing GitHub Release notes/history preserved on reruns while intended assets are replaced.

## Shared UI Status

The shared Avalonia shell exposes:

- Standard + Scientific calculator
- Sanitized paste and copy-result actions
- Selection-aware calculator keypad editing
- Programmer calculator with full radix selector, bitwise operations, shifts, grouped bit grid, and copy actions
- Unicode code-point tools
- Offline unit converter with precision/search/recents/favorites/clear/copy
- Statistics
- Equations
- Matrices
- Graphing with pointer/keyboard viewport interaction, tracing, numerical analysis, CSV, multi-expression export, and SVG export
- Date/time utilities
- Currency conversion
- History with search/favorite/delete/clear and TXT/CSV/JSON preview/copy
- Settings
- About/support
- First-run onboarding

## Remaining High-Priority Work

The following work cannot be honestly marked complete from source presence alone:

1. Observe real `.NET 10` restore, format, build, analyzer, and full test results and fix every concrete failure.
2. Observe the dedicated Avalonia headless UI workflow and fix any compiled-XAML/headless failures.
3. Validate Desktop on real Windows/Linux/macOS environments, including launch, clipboard, persistence, keyboard, scaling, and packaging.
4. Validate Browser/WebAssembly publish/load/storage/clipboard/keyboard/accessibility behavior in supported browsers.
5. Validate Android workload build, emulator/device launch, portrait/landscape/tablet layouts, persistence, clipboard, TalkBack/large text, and signed AAB/store checks.
6. Validate iOS workload/simulator/device behavior, Dynamic Type/VoiceOver/layout, persistence/clipboard, signing/provisioning/archive/distribution. The tag-time simulator workflow is source-implemented but still requires an observed run.
7. Perform the full runtime accessibility matrix: keyboard traversal, screen readers, focus visibility, measured contrast, target sizes, large text, reduced motion, and system accessibility composition.
8. Perform real compact/medium/expanded device/window validation, especially 64/128-bit programmer grids and long result/export surfaces.
9. Migrate predominantly English visible XAML to the semantic localization layer in compile-verified increments and validate Hindi long-string/Devanagari layouts.
10. Complete native file-save/share polish for exported history/graph data only after platform abstractions are runtime-validated.
11. Run the final release-candidate gate across source preflight, compiled tests, security, docs, assets, accessibility, privacy, platform packaging, signing, and store requirements.

## Known Issues / Risks

1. **The required .NET SDK is unavailable in the active assistant execution environment used for this continuation.** No local compiled build/test PASS is claimed.
2. Source/test/workflow presence is never treated as equivalent to observed execution.
3. Avalonia XAML and platform workload integration still require actual compiled runs.
4. Browser storage, native filesystem storage, clipboard APIs, and optional network behavior need target runtime evidence.
5. Numerical graph analysis is intentionally approximate and bounded; documentation/UI must keep that distinction visible.
6. Large 64/128-bit programmer grids may still need further compact-layout refinement if device testing proves the current grouped layout inadequate.
7. The Hindi semantic catalog exists, but the predominantly English shared XAML prevents any claim of complete Hindi UI localization.
8. Headless UI tests improve shared-shell confidence but do not emulate screen readers, real touch/IME behavior, native permission prompts, GPU rendering, mobile layout engines, signing, or package lifecycles.
9. Settings schema migration source tests now model truly unversioned historical JSON; native/Browser runtime storage migration must still be observed on target environments.
10. iOS release-tag simulator validation does not claim signing, provisioning, archive, TestFlight, App Store processing, or device readiness.

## Validation Status

### Implemented source/test coverage

- Core/domain tests: IMPLEMENTED
- Programmer tests: IMPLEMENTED
- Converter tests: IMPLEMENTED
- Persistence/history tests: IMPLEMENTED
- Shared settings schema/decoder/validator tests: IMPLEMENTED
- Graphing tests: IMPLEMENTED
- Statistics tests: IMPLEMENTED
- Equation tests: IMPLEMENTED
- Matrix tests: IMPLEMENTED
- Currency tests: IMPLEMENTED
- Date/time tests: IMPLEMENTED
- App/view-model tests: IMPLEMENTED
- Calculator selection-editing tests: IMPLEMENTED
- Avalonia headless shared-shell tests: IMPLEMENTED IN SOURCE
- Avalonia headless graph viewport tests: IMPLEMENTED IN SOURCE
- SDK-independent validator regression tests: IMPLEMENTED
- Platform/release workflow source contracts: IMPLEMENTED

### Execution evidence in this continuation

- Local `.NET restore`: **NOT RUN — .NET SDK unavailable**
- Local formatting verification: **NOT RUN — .NET SDK unavailable**
- Local compiled build: **NOT RUN — .NET SDK unavailable**
- Local compiled tests: **NOT RUN — .NET SDK unavailable**
- Local Avalonia headless tests: **NOT RUN — .NET SDK unavailable**
- Windows launch/package validation: **NOT RUN**
- Linux launch/package validation: **NOT RUN**
- macOS launch/sign/notarization validation: **NOT RUN**
- Browser/WebAssembly runtime validation: **NOT RUN**
- Android device/signed package validation: **NOT RUN**
- iOS simulator/device/sign/archive validation: **NOT RUN**
- Screen-reader/large-text/measured-contrast target audit: **NOT RUN**

The SDK-independent source preflight command was invoked against a downloaded `main` snapshot during this continuation, but its output is not being promoted here as release evidence. The authoritative release status remains conservative until an observable CI/local result is captured.

**A check is never marked PASS unless it actually ran and its result was observed.**

## Continuation Priorities

The next continuation must start from execution evidence rather than recreating completed source modules:

1. Inspect/observe GitHub Actions or run `.NET 10` locally and fix actual restore/build/test/headless failures.
2. Record real platform results in `docs/ACCESSIBILITY_TEST_MATRIX.md` and the release readiness checklist.
3. Fix target-specific adaptive/accessibility/platform issues discovered by those runs.
4. Continue visible-XAML localization only in compile-verified increments.
5. Add native file-save/share UX only after platform behavior is validated.
6. Complete signing/store/release-candidate work only with the required target tooling and external credentials.

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
- `tools/`
- `.github/workflows/`
- `docs/`
- `what_changed.md`

## Continuation Rule

Before new development, read this file and `what_changed.md`, inspect current `main`, and continue the first incomplete task. Do not recreate completed files, reset the repository, or report unavailable/unobserved validation as passing.
