# What Changed

## 2026-08-19 — Continuation Pass

### Scope

This continuation resumed from the actual `main` branch instead of recreating features already present in the repository. The older `PROJECT_STATE.md`/roadmap descriptions were found to be behind the implementation, so source, tests, workflows, and shared UI were inspected first and the continuation proceeded from the real remaining gaps.

The work was intentionally split into many focused, meaningful commits rather than one large commit. The continuation added dozens of atomic feature/test/fix/documentation commits across Core, Programmer, Converter, Graphing, Platform, App, Persistence, tests, and documentation.

### Added — Safe Expression Import

- Added `CalcNova.Core.Parsing.ExpressionTextSanitizer`.
- Added normalization for calculator-style external text:
  - leading `=` removal;
  - `×`/`·` → `*`;
  - `÷` → `/`;
  - common Unicode minus/dash variants → `-`;
  - `π` → `pi`;
  - `τ` → `tau`;
  - superscript `²`/`³` → `^2`/`^3`;
  - CR/LF/TAB → safe spacing.
- Added rejection of unsupported controls/symbols.
- Added evaluator-aligned maximum-expression-length enforcement.
- Added `CalculatorViewModel.ImportExpression(...)` and `ImportExpressionCommand`.
- Rejected imports preserve the previous calculator expression and provide a user-facing status message.

### Added — Clipboard Architecture

- Added `CalcNova.Platform.Clipboard.IClipboardService`.
- Added `CalcNova.App.Services.AvaloniaClipboardService`.
- Added explicit `PasteCommand` / `PasteAsync` to the calculator.
- Added explicit `CopyResultCommand` / `CopyResultAsync`.
- Paste always goes through `ExpressionTextSanitizer` and does not auto-evaluate.
- Clipboard reads happen only after explicit user action.
- Clipboard service is attached to the active Avalonia `TopLevel` only while `MainView` is attached.
- Shared clipboard composition was added to:
  - Desktop;
  - Browser/WebAssembly;
  - Android;
  - iOS.
- Added visible `Paste expression` and `Copy result` actions to the Calculator tab.

### Added — Programmer Bit Tools

- Added bounded `BitwiseCalculator.IsBitSet(...)`.
- Added bounded `BitwiseCalculator.ToggleBit(...)`.
- Added bit-index validation against selected word size.
- Added `BitCellViewModel` with:
  - bit index;
  - set/clear state;
  - readable label;
  - accessible state label;
  - toggle command.
- Added full interactive bit collections for 8/16/32/64/128-bit presets.
- Added a most-significant-bit-first shared bit grid.
- Added accessible names such as `Bit 7, set`.
- Preserved a fixed-width textual bit pattern as an alternative representation.

### Added — Programmer Radix and Operations

- Exposed the complete base 2–36 range in the shared Programmer UI.
- Added input-base validation for 2–36.
- Added a second operand field.
- Added interactive commands for:
  - AND;
  - OR;
  - XOR;
  - NOT;
  - left shift;
  - logical right shift;
  - arithmetic right shift.
- Added a configurable shift count.
- Added shared UI controls for the bitwise/shift operations.
- Added a `LastOperation` status field.
- Added shift-count guardrails against values beyond the selected shared word size.

### Fixed — Programmer Signed/Unsigned Presentation

- Fixed fixed-width presentation semantics so binary/octal/hexadecimal remain masked to the selected word size.
- Decimal output now follows the selected signed/unsigned interpretation.
- Arithmetic right shift now preserves the expected two's-complement bit pattern and signed decimal interpretation.
- Applied results use signed decimal input only when decimal + signed mode is selected; non-decimal inputs remain fixed-width masked representations.

### Added — Unicode Code-Point Tools

- Added `UnicodeCodePointHelper`.
- Added parsing for forms such as:
  - `U+0041`;
  - `0x03C0`;
  - raw hexadecimal scalar values.
- Added Unicode scalar validation with `Rune` semantics.
- Added rejection of surrogate code points and invalid scalar ranges.
- Added canonical `U+XXXX` formatting.
- Added code-point-to-text conversion.
- Added bounded text inspection by Unicode scalar rather than UTF-16 code unit.
- Added `CodePointViewModel`.
- Added a dedicated shared `Code` tab for code-point-to-text and text-to-code-point workflows.

### Added — Converter Pair Model

- Added validated `ConversionPair` domain model.
- Added same-category validation.
- Added compact pair display names.
- Added pair swapping.
- Added `ConversionPairHistory`.
- Added bounded recent-pair tracking.
- Added duplicate suppression and most-recent-first ordering.
- Added favorite conversion pairs.
- Added restore support for persisted recent/favorite state.
- Added change-aware `Record(...)` behavior so already-most-recent pairs do not trigger redundant persistence.

### Added — Converter Precision

- Added selectable 1–17 significant-digit formatting.
- Added shared precision presets: 6, 9, 12, 15, 17.
- Changing precision immediately refreshes the displayed conversion.
- Added visible precision selector to the Converter tab.

### Added — Converter Recent/Favorite UI

- Added `CurrentPair`, `FavoriteToggleLabel`, `RecentPairs`, `FavoritePairs`, and saved-pair selection support.
- Added visible favorite toggle.
- Added recent-pair selector.
- Added favorite-pair selector.
- Saved-pair selection clears itself after application so the same pair can be selected again later.
- Deliberate Convert/Swap/Apply-pair actions record recents; intermediate selector changes no longer pollute recent history.

### Added — Converter Persistence

- Added versioned `ConversionPairToken` format (`v1:from>to`).
- Added safe token decode/validation through `UnitCatalog`.
- Added converter fields to shared `AppSettings`:
  - `ConverterSignificantDigits`;
  - `ConverterRecentPairs`;
  - `ConverterFavoritePairs`.
- Added native JSON settings validation for:
  - significant digits 1–17;
  - maximum 12 recent pairs;
  - maximum 100 favorites;
  - bounded non-empty token length.
- Added equivalent Browser settings validation.
- Added converter-state support to `SettingsViewModel`.
- Added serialized converter-state restore during `MainViewModel` initialization/settings application.
- Added autosave after deliberate converter preference changes.
- Added synchronization so Settings reset immediately resets in-memory converter preferences instead of waiting for another launch.

### Added — Graph Numerical Analysis

- Added `NumericalAnalysisOptions` with bounded configuration for:
  - derivative step;
  - root tolerance;
  - root iteration count;
  - Simpson integration interval count;
  - maximum integration intervals.
- Added `GraphNumericalAnalyzer`.
- Added central-difference first-derivative approximation.
- Added bracketed bisection root finding.
- Added composite Simpson definite integration.
- Added finite-value validation and bounded workloads.
- Reused compiled CalcNova expressions with `x` supplied through evaluator variables.
- Added `AnalysisX` and `AnalysisResult` to `GraphingViewModel`.
- Added derivative/root/integral commands.
- Added explicit approximate-result labeling.
- Added visible numerical-analysis controls to the shared Graph tab.

### Added — Shared UI Integration

The shared Avalonia shell now visibly includes:

- Calculator sanitized paste/copy actions;
- Programmer base 2–36 selector;
- Programmer AND/OR/XOR/NOT;
- Programmer left/logical-right/arithmetic-right shifts;
- full word-size interactive bit grid;
- Unicode code-point tools;
- Converter precision;
- Converter recent/favorite pair selectors;
- Converter favorite toggle;
- Graph derivative/root/integral controls.

### Added — Accessibility Baseline

- Added global minimum 44-pixel height for common Buttons.
- Added global minimum 44-pixel height for TextBoxes.
- Added global minimum 44-pixel height for ComboBoxes.
- Added global minimum 44-pixel height for TabItems.
- Added global minimum 44-pixel height for ListBoxItems.
- Preserved 54-pixel standard calculator keys.
- Added programmer bit-cell accessible names.
- Preserved textual alternatives for bit patterns and graph-analysis output.
- Updated accessibility documentation to distinguish implemented source measures from still-unvalidated platform accessibility behavior.

### Tests Added — Core / Clipboard

- Expression glyph normalization.
- Superscript normalization.
- Multiline whitespace normalization.
- Unsupported-character rejection.
- maximum imported-expression-length enforcement.
- empty imported text.
- calculator import/evaluation integration.
- rejected import preserving the prior expression.
- command-path import behavior.
- fake clipboard sanitized paste.
- unsafe clipboard rejection.
- valid result copy.
- unavailable clipboard behavior.

### Tests Added — Programmer

- bit inspection/toggle boundaries.
- view-model bit toggle synchronization.
- full bit-grid count/index ordering.
- bit-cell toggle behavior and accessible labels.
- 128-bit grid rebuild.
- complete base 2–36 selection.
- custom base-3 conversion.
- invalid radix rejection.
- AND operation.
- XOR masking.
- fixed-width NOT.
- shift operations.
- excessive shift-count rejection.
- signed 8-bit fixed-width output semantics.
- arithmetic-right-shift two's-complement behavior.
- signed decimal result behavior.
- Unicode scalar parse/format/text conversion.
- surrogate rejection.
- Unicode scalar enumeration.
- code-point inspection limit.
- CodePointViewModel decode/inspect/error behavior.

### Tests Added — Converter / Settings

- conversion-pair category mismatch.
- pair swap.
- recent de-duplication/order.
- recent-capacity enforcement.
- favorite add/remove.
- persisted pair-state restore.
- restore de-duplication/capacity.
- change-aware recent recording.
- versioned pair token round-trip.
- malformed/obsolete/invalid token rejection.
- converter pair-history/favorite app behavior.
- selectable-pair UI contract.
- favorite label state.
- significant-digit formatting and bounds.
- `MainViewModel` converter preference restore.
- converter precision autosave.
- favorite-pair autosave.
- native JSON settings round-trip for converter state.
- invalid converter precision rejection.
- oversized persisted recent-list rejection.

### Tests Added — Graphing

- polynomial derivative approximation.
- bracketed polynomial root solving.
- unbracketed-root rejection.
- polynomial Simpson integration.
- reversed integration bounds.
- odd Simpson interval rejection.
- derivative view-model command output.
- root view-model command output.
- integration view-model command output.
- view-model root error propagation.

### Fixed During Static Review

- Corrected the initial expression-sanitizer default maximum-length reference to use the existing `EvaluationOptions.Default` instance.
- Removed an unnecessary root-search assignment found during static review.
- Replaced potentially ambiguous collection-expression expectations in regression tests with explicit arrays where useful.
- Fixed saved conversion-pair reselection behavior.
- Corrected converter restore-capacity test expectations after reviewing most-recent-first de-duplication semantics.
- Fixed Settings reset synchronization for converter state.
- Fixed fixed-width signed programmer display/result semantics.

### Documentation Updated

- `README.md`
- `PROJECT_STATE.md`
- `CHANGELOG.md`
- `docs/README.md`
- `docs/FEATURES.md`
- `docs/ROADMAP.md`
- `docs/ACCESSIBILITY.md`
- `docs/INPUT_SAFETY.md`
- `docs/PROGRAMMER_MODE.md`
- `docs/CONVERTER_MODE.md`
- `docs/NUMERICAL_ANALYSIS.md`
- `what_changed.md`

Documentation now treats the shared clipboard/programmer/converter/graph controls and converter persistence as implemented, while keeping adaptive layout, accessibility validation, UI automation, packaging, localization, onboarding, and release validation in the remaining-work list.

### Validation Status for This Continuation

The active execution environment does not provide the required .NET SDK. Therefore:

- local `dotnet restore`: **NOT RUN**;
- local `dotnet format --verify-no-changes`: **NOT RUN**;
- local `dotnet build`: **NOT RUN**;
- local `dotnet test`: **NOT RUN**;
- Android package validation: **NOT RUN**;
- iOS package/archive validation: **NOT RUN**;
- Browser/WebAssembly publish validation: **NOT RUN**;
- Desktop OS package validation: **NOT RUN**.

GitHub's combined-status endpoint was checked during this continuation and exposed no status checks for the checked latest commit at that time. No CI PASS is inferred from an empty status list.

A check is never marked PASS merely because source code or tests exist.

### Remaining Work After This Continuation

Highest-priority continuation items are now:

1. adaptive/mobile layout pass across every mode;
2. complete keyboard/focus/screen-reader/high-contrast/large-text accessibility validation;
3. compact grouping/virtualization polish for 64/128-bit programmer grids;
4. observe actual GitHub Actions runs and fix real compile/analyzer/test failures;
5. stable shared-shell UI/integration automation;
6. converter unit/category search, clear-recents, and output-copy productivity actions;
7. programmer radix-output copy actions and bit grouping;
8. graph trace/cursor, table-of-values, multiple expressions, and richer export UI;
9. platform packaging/signing/store validation;
10. localization, onboarding, design-system consolidation, and final release-gate audit.

## 2026-08-18

### Added

- Apache-2.0 license.
- Repository formatting, line-ending, ignore, analyzer, nullable, deterministic-build, and central-package configuration.
- `.NET 10` SDK baseline through `global.json`.
- Modular `CalcNova.slnx` solution.
- Core calculation engine with typed errors and workload limits.
- Numeric value layer using arbitrary-precision integers, decimal arithmetic where possible, and bounded floating-point fallback for transcendental functions.
- Safe expression tokenizer and recursive-descent parser.
- Right-associative exponentiation and unary operator handling.
- Scientific functions including roots, logs, trigonometry, inverse trigonometry, hyperbolic functions, rounding, factorial, GCD, LCM, combinations, and permutations.
- Degree, radian, and gradian angle modes.
- Programmer radix conversion for bases 2 through 36.
- Programmer bounded word-size bitwise helpers and two's-complement interpretation.
- Offline fixed-unit conversion engine and searchable unit catalog.
- Initial Avalonia shared app with standard/scientific calculator workspace.
- Desktop Avalonia application entry point and basic Enter/Escape/Backspace keyboard actions.
- Native SQLite calculation-history repository behind `ICalculationHistoryRepository`.
- Core, programmer, converter, and SQLite persistence test source projects.
- `PROJECT_STATE.md` for deterministic multi-chat continuation.

### Changed

- Central package versions were expanded to cover native persistence and planned Android/iOS/Browser Avalonia platform packages.
- Solution was expanded to include persistence and persistence tests.

### Fixed

- Fixed number parsing code that used an invalid string API overload for detecting exponent markers.
- Fixed programmer radix parsing so separator-only or sign-only input is rejected without indexing an empty span.

### Tests Added

- Arithmetic and precedence cases:
  - `1 + 1`
  - `2 + 3 * 4`
  - `(2 + 3) * 4`
  - `2 ^ 3 ^ 2`
  - unary-minus precedence
  - negative exponents
- Decimal and large-integer cases:
  - `0.1 + 0.2`
  - `999999999999999999 + 1`
  - scientific notation
- Domain and typed-error cases:
  - divide by zero
  - square root of a negative real value
  - input length limit
- Scientific cases:
  - degree-mode trigonometry
  - factorial
  - GCD / LCM
  - combinations / permutations
- Programmer cases:
  - bases 2, 8, 10, 16, 36
  - large integer round trips
  - signed/two's-complement boundaries
  - fixed-width NOT and shifts
  - invalid radix input regressions
- Converter cases:
  - exact/known length, mass, data, and energy identities
  - Celsius/Fahrenheit/Kelvin affine conversions
  - category mismatch rejection
  - unit search
- SQLite history cases:
  - initialization
  - add/get
  - favorite update
  - search
  - delete
  - clear

### Documentation

- Added project continuation state and exact next-task list.
- Recorded unvalidated platform/build status explicitly instead of marking unavailable checks as PASS.

### Git

Atomic commits created during this work include:

- `3c9a1773` — chore: add .NET and IDE ignore rules
- `f7b00ca0` — chore: define repository line ending policy
- `6bca621e` — chore: configure C# formatting and analyzer defaults
- `45b27069` — build: pin .NET 10 SDK feature band
- `8fc21a1e` — build: enable nullable analysis and deterministic builds
- `a9915cae` — build: centralize stable package versions
- `e99411cf` — chore: add modular CalcNova solution
- `eed81e36` — chore(core): create calculation engine project
- `777a1364` — chore(scientific): create scientific feature project
- `97180487` — chore(programmer): create programmer feature project
- `6e64dad5` — chore(converter): create conversion feature project
- `a30bfdd8` — chore(app): create shared Avalonia application project
- `4da5a85d` — chore(desktop): create desktop host project
- `0c5007d2` — test(core): create core test project
- `3d06fd73` — test(programmer): create programmer test project
- `7b2f9596` — test(converter): create converter test project
- `f8290dbc` — feat(core): define calculation error codes
- `e42a8199` — feat(core): add typed calculation exception
- `8cadfb96` — feat(core): add angle unit model
- `829b9c7c` — feat(core): add evaluator options and safety limits
- `542cdec2` — feat(core): model successful and failed evaluations
- `8e5b704c` — feat(core): add mixed exact and floating numeric value type
- `594b48b8` — feat(parser): define expression token kinds
- `8f4fe375` — feat(parser): add immutable expression token model
- `ad78e78b` — feat(parser): implement safe expression tokenizer
- `70bc400d` — feat(parser): add expression syntax tree
- `b37ee948` — feat(parser): implement precedence-aware recursive descent parser
- `e7a66976` — feat(calc): implement deterministic expression evaluator
- `db83eb5b` — fix(core): use valid exponent marker checks
- `c988431a` — test(core): cover arithmetic precedence precision and domains
- `9297cf1e` — feat(scientific): add scientific calculator facade
- `540a9258` — feat(scientific): publish supported scientific function catalog
- `af1a8f47` — feat(programmer): add arbitrary precision radix conversion
- `efc3aca6` — feat(programmer): add bounded word bitwise operations
- `4b57bcec` — test(programmer): cover radix and bitwise boundaries
