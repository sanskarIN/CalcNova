# What Changed

## 2026-08-19 — Full Source Hardening Continuation

### Scope

This continuation resumed from the actual current `main` branch and continued the first incomplete items rather than recreating already-complete calculator/converter/graph modules. The work was split across many focused feature, fix, test, CI, and documentation commits.

The repository has now moved from broad source-feature completion into release-evidence hardening. Remaining blockers are dominated by real `.NET 10`, Avalonia, platform, accessibility, signing, and device/browser execution rather than missing core calculation modules.

### Added — Visible Keyboard Focus Contracts

- Strengthened shared focus styling for Button, TextBox, ComboBox, CheckBox, TabItem, and ListBoxItem controls.
- Added stronger focus emphasis when the CalcNova high-contrast preference is active.
- Added `tools/validate_focus_visibility.py`.
- Added `tools/tests/test_validate_focus_visibility.py`.
- Added `.github/workflows/focus-visibility-validate.yml`.
- Added `docs/FOCUS_VISIBILITY.md`.
- Kept focus styling separate from claims about real screen-reader/focus rendering on target platforms.

### Added — Hindi Localization Foundation

- Added a complete Hindi semantic catalog for the current `AppStringKey` set.
- Expanded `AppLocalizer` culture selection for English/Hindi and regional culture names such as `en-IN` and `hi-IN`.
- Added localization tests covering Hindi lookup/fallback behavior.
- Expanded `tools/validate_localization_catalog.py` to validate both English and Hindi catalogs.
- Expanded localization validator regression tests and workflow coverage.
- Updated localization documentation to explicitly state that the shared XAML is still predominantly English.

The Hindi catalog is an implemented localization **foundation**, not a claim of complete Hindi visible-UI translation.

### Added — Versioned Settings Migration

- Added explicit `AppSettingsSchema` versioning.
- Added current-schema normalization.
- Added legacy schema-zero migration.
- Added fail-closed rejection of negative/corrupt schema versions.
- Added fail-closed rejection of unsupported future schema versions to avoid unsafe downgrade/overwrite behavior.
- Added native JSON schema migration tests.
- Added settings-schema source validator, regression tests, and workflow.
- Added `docs/SETTINGS_MIGRATION.md`.

### Fixed — Truly Unversioned Historical Settings

A real migration edge case was found during static review: settings files created before schema versioning contain **no** `schemaVersion` field. Because new `AppSettings` instances default to the current schema, deserializing such historical JSON without inspecting the document could incorrectly make it look current.

The fix:

- added shared `AppSettingsJson` schema-aware JSON decoding;
- detects the schema property case-insensitively;
- treats a missing schema property as legacy schema `0`;
- preserves explicitly serialized schema values for normal schema-policy handling;
- updated native JSON migration tests to model the true historical format with no schema property;
- retained a separate explicit-schema-zero migration test.

### Refactored — Shared Settings Validation

- Added shared `AppSettingsValidator` in the Platform layer.
- Centralized validation for:
  - theme;
  - angle unit;
  - culture name;
  - decimal precision;
  - history limit;
  - onboarding version;
  - converter significant-digit precision;
  - recent/favorite converter-token counts and token lengths.
- Native `JsonSettingsRepository` now consumes `AppSettingsJson` + `AppSettingsValidator`.
- Browser `BrowserSettingsRepository` now consumes the same shared contracts.
- Removed duplicated native/Browser private validation implementations.
- Added Platform tests for shared JSON decoding and shared preference validation.
- Strengthened `validate_settings_schema.py` so future repositories cannot silently reintroduce duplicated validation.
- Updated the settings-schema workflow path coverage.
- Added `docs/SETTINGS_STORAGE_CONTRACT.md`.

### Added — Graph Keyboard Interaction

- Added typed graph keyboard navigation actions.
- Added keyboard mapping for:
  - arrow-key viewport panning;
  - numpad Add zoom-in;
  - numpad Subtract zoom-out;
  - Home viewport reset;
  - `F` fit-to-data.
- Wired keyboard actions into the focusable shared graph plot control.
- Added graph keyboard mapping tests.
- Added `tools/validate_graph_keyboard.py` and validator regression tests.
- Added dedicated graph-keyboard CI validation.
- Added graph keyboard checks to release-source preflight.
- Added `docs/GRAPH_INTERACTION.md`.
- Exposed a read-only graph viewport snapshot for deterministic headless UI assertions.

### Added — Runtime Accessibility Evidence Discipline

- Added `docs/ACCESSIBILITY_TEST_MATRIX.md`.
- Defined conservative evidence states: PASS / FAIL / BLOCKED / NOT RUN.
- Added source validation preventing unobserved runtime scenarios from being casually recorded as PASS.
- Added accessibility-evidence validator regression tests and focused workflow.
- Added the evidence contract to release-source preflight.

The matrix starts conservatively. Source implementation is never substituted for real keyboard/screen-reader/device evidence.

### Added — Avalonia Headless UI Automation Foundation

- Added centrally versioned `Avalonia.Headless.XUnit` matching Avalonia 12.1.1.
- Enabled the existing App test project for Avalonia headless xUnit v3 tests.
- Added `TestAppBuilder` with the headless CalcNova application bootstrap.
- Added shared-shell headless scenarios covering:
  - all primary mode tabs loading;
  - real Calculator clear-command binding;
  - selection-aware keypad replacement/caret restoration;
  - compact adaptive-class application;
  - Ctrl+PageDown shared mode navigation;
  - high-contrast shell-class application;
  - onboarding visibility and Skip behavior.
- Added graph headless scenarios covering:
  - keyboard pan;
  - keyboard zoom;
  - Home reset;
  - `F` fit-to-data.
- Added `tools/validate_headless_ui_tests.py`.
- Added validator regression tests.
- Added `.github/workflows/headless-ui-validate.yml`, which installs .NET 10, restores the App tests, and runs the App test project.
- Added headless source contracts to the integrated preflight.
- Added `docs/UI_AUTOMATION.md`.

The headless test source is implemented. It is **not** called PASS until a real `.NET` execution result is observed.

### Added — Selection-Aware Calculator Editing

- Added tracked expression selection state to `CalculatorViewModel`.
- Added bounded selection updates.
- Keypad insertion now replaces a selected range.
- Keypad insertion now occurs at the tracked caret when there is no selection.
- Reversed selections are normalized.
- Backspace removes the selected range when present.
- Backspace otherwise removes the character before the caret.
- Backspace at caret position zero is a no-op.
- Expression-length enforcement now accounts for text replaced by the selection.
- Programmatic expression replacement requests deterministic caret placement.
- Added `SelectionRequested` view-model contract.
- Shared `MainView` now synchronizes calculator TextBox selection after keyboard and pointer selection changes.
- Shared `MainView` restores requested TextBox selection after keypad edits.
- Added `CalculatorSelectionEditingTests` covering selection/caret boundaries.
- Added `tools/validate_calculator_selection_editing.py` and regression tests.
- Added `.github/workflows/calculator-selection-validate.yml`.
- Added calculator selection contracts to integrated source preflight.
- Added a headless keypad-selection integration scenario.
- Added `docs/CALCULATOR_EDITING.md`.

### Added — Unified SDK-Independent Source Preflight

The integrated `tools/release_preflight.py` inventory was expanded to cover current release-critical source contracts, including:

- repository/security structure;
- Avalonia XAML XML;
- shared UI contracts;
- navigation;
- calculator/shell keyboard mappings;
- calculator selection editing;
- graph keyboard behavior;
- headless UI-test configuration/scenarios/execution path;
- accessibility markup;
- focus visibility;
- accessibility evidence discipline;
- adaptive layout;
- touch targets;
- English/Hindi localization;
- settings schema/shared codec/shared validator;
- onboarding;
- packaging metadata;
- platform build-workflow contracts;
- release-workflow contracts;
- release-documentation contracts;
- release-tag validation;
- regression suites for the source validators;
- the preflight inventory test itself.

The Source Preflight workflow path filters were expanded so relevant App tests, Platform/Persistence settings tests, central package/SDK policy, platform workflows, headless workflow, release workflow, and validation documentation trigger the combined gate.

### Fixed — Release Documentation Contract Drift

Static review found that the release-documentation validator still required older exact wording and could reject the repository after documentation moved to the four-state PASS / FAIL / BLOCKED / NOT RUN evidence vocabulary.

- Modernized `tools/validate_release_docs.py`.
- Added a callable `validate(root)` contract.
- Added release-documentation validator regression tests.
- Updated required evidence markers to current documentation/state language.
- Added the regression suite to integrated preflight.

### Hardened — Tag-First Release Workflow

- Release validation now verifies the requested tag exists.
- Release validation detaches at the exact tag before source preflight.
- Integrated source preflight runs against the detached release tag.
- `.NET` restore/format/build/test follows tagged source preflight.
- Desktop, Browser, Android, and publication jobs check out the release ref.
- Existing release notes/history are preserved on reruns.
- Intended packaged assets are replaced with `--clobber` rather than deleting/recreating the release.
- Android signed AAB remains conditional on external signing secrets.
- Temporary Android signing material is removed even on failure paths.
- Added `tools/validate_release_workflow.py`.
- Added release-workflow validator regression tests.
- Added a focused release-workflow CI contract.
- Added release-workflow contracts to integrated preflight.
- Updated `docs/RELEASE.md` so integrated source preflight is the authoritative first source gate.
- Expanded `docs/RELEASE_READINESS_CHECKLIST.md` for current schema/accessibility/keyboard/platform evidence.

### Added — Cross-Platform Workflow Contracts

- Added `tools/validate_platform_workflows.py`.
- Protected the shared .NET 10 SDK policy.
- Protected Desktop validation across Windows/Linux/macOS runners.
- Protected Browser `wasm-tools` setup and publish command.
- Protected Android Java 17 + Android workload validation.
- Protected iOS macOS runner + iOS workload + simulator-RID validation.
- Required read-only contents permissions for validation workflows.
- Rejected signing-password/certificate markers from ordinary validation workflows.
- Added validator regression tests.
- Added `.github/workflows/platform-workflows-validate.yml`.
- Added platform-workflow contracts to integrated source preflight.

### Added — Release-Tag iOS Simulator Validation

The primary release workflow cannot honestly create a signed iOS archive without Apple signing/provisioning credentials. Instead of pretending that gap is solved, a separate exact-tag simulator validation path was added:

- added `.github/workflows/release-ios-validate.yml`;
- verifies and detaches at the requested existing release tag;
- uses `macos-latest` + .NET 10 + iOS workload;
- selects `iossimulator-arm64` or `iossimulator-x64` from runner architecture;
- restores and builds the tagged iOS head in Release configuration;
- explicitly states that simulator compilation does not prove signing/archive/App Store readiness;
- added `tools/validate_release_ios_workflow.py`;
- added validator regression tests;
- added focused release-iOS-workflow contract CI;
- added `docs/IOS_RELEASE_VALIDATION.md`.

### Documentation Synchronization

Documentation updated or added during this continuation includes:

- `README.md`
- `PROJECT_STATE.md`
- `docs/README.md`
- `docs/ROADMAP.md`
- `docs/FEATURES.md`
- `docs/TESTING.md`
- `docs/PLATFORM_SUPPORT.md`
- `docs/SOURCE_PREFLIGHT.md`
- `docs/FOCUS_VISIBILITY.md`
- `docs/GRAPH_INTERACTION.md`
- `docs/SETTINGS_MIGRATION.md`
- `docs/SETTINGS_STORAGE_CONTRACT.md`
- `docs/UI_AUTOMATION.md`
- `docs/ACCESSIBILITY_TEST_MATRIX.md`
- `docs/LOCALIZATION.md`
- `docs/ONBOARDING.md`
- `docs/RELEASE.md`
- `docs/RELEASE_READINESS_CHECKLIST.md`
- `docs/CALCULATOR_EDITING.md`
- `docs/IOS_RELEASE_VALIDATION.md`
- `what_changed.md`

A material stale-documentation issue was fixed in `PLATFORM_SUPPORT.md`: Android, iOS, and Browser heads/workflows already existed and are now described as implemented **source foundations** while runtime/package validation remains NOT RUN.

### Repository Sweep

Repository searches during this continuation did not surface ordinary `TODO`, `FIXME`, `NotImplementedException`, or placeholder implementation markers in the inspected current repository search results. This does not replace compilation/runtime validation and is not treated as proof that no defect exists.

### Validation Status

The active assistant execution environment still does not provide the required .NET SDK. Therefore:

- local `.NET restore`: **NOT RUN**;
- local `dotnet format`: **NOT RUN**;
- local compiled build: **NOT RUN**;
- local compiled unit/integration tests: **NOT RUN**;
- local Avalonia headless tests: **NOT RUN**;
- Windows package/runtime validation: **NOT RUN**;
- Linux package/runtime validation: **NOT RUN**;
- macOS package/sign/notarization validation: **NOT RUN**;
- Browser/WebAssembly runtime validation: **NOT RUN**;
- Android device/signed package validation: **NOT RUN**;
- iOS simulator/device/sign/archive validation: **NOT RUN**;
- target screen-reader/large-text/measured-contrast audit: **NOT RUN**.

The SDK-independent source preflight command was invoked against a downloaded current `main` snapshot during this continuation, but that invocation is not being promoted as release evidence here. An observable CI/local result is still required before marking the release gate PASS.

GitHub connector write actions used in this continuation do not expose a per-commit author-email override. Commits were therefore created through the authenticated repository identity rather than falsely claiming that `sanskarin@outlook.in` was applied as the commit author email.

### Remaining Work After This Continuation

The remaining high-priority work is now primarily execution/toolchain dependent:

1. observe real .NET 10 restore/format/build/test/headless results and fix concrete failures;
2. observe Desktop/Browser/Android/iOS workflow results;
3. perform actual platform launch/storage/clipboard/accessibility/adaptive-layout testing;
4. validate Android signing/store output with external credentials;
5. validate iOS signing/provisioning/archive/TestFlight/App Store behavior with Apple tooling/credentials;
6. migrate predominantly English visible XAML to the semantic localization layer in compile-verified increments;
7. add native file-save/share UX after target platform abstractions are validated;
8. run the final release-candidate audit with observed evidence.

## 2026-08-19 — Keyboard + Navigation Accessibility Continuation

### Added — Shared Shell Keyboard Navigation

- Added `ShellKeyboardShortcut` with a typed `ShellNavigationAction` contract.
- Added `Ctrl+Home` to select the first CalcNova mode.
- Added `Ctrl+End` to select the last CalcNova mode.
- Preserved `Ctrl+PageUp` / `Ctrl+PageDown` cyclic mode navigation.
- Required exactly the Control modifier for shared-shell navigation so extra modifier combinations do not accidentally trigger mode changes.
- Kept all global calculator/mode shortcuts suppressed while onboarding is visible.

### Changed — Mode Selection Safety

- Added explicit `SelectMode`, `SelectFirstMode`, and `SelectLastMode` APIs to `MainViewModel`.
- Preserved cyclic next/previous navigation through the shared normalization helper.
- Hardened the two-way `SelectedModeIndex` binding against transient invalid values such as `-1`, preventing a temporary Avalonia `TabControl` state from accidentally selecting the last mode during initialization.

### Added — Tests and CI

- Expanded `MainViewModelNavigationTests` with direct selection, boundary selection, normalization, and transient invalid-index coverage.
- Added `ShellKeyboardShortcutTests` for cyclic/boundary shortcuts and modifier collision behavior.
- Extended `tools/validate_keyboard_contracts.py` to validate both calculator hardware mappings and shared-shell navigation shortcuts.
- Added `tools/tests/test_validate_keyboard_contracts.py`.
- Added `.github/workflows/keyboard-validate.yml` for SDK-independent keyboard contract validation and regression tests.
- Updated `docs/KEYBOARD_SHORTCUTS.md` with the implemented shell navigation behavior and validation limitations.

### Validation Status

- GitHub workflow lookup for the latest keyboard-validator regression commit exposed no workflow runs at the time checked; no CI PASS is inferred from an empty run list.
- The active continuation environment still does not expose the required .NET SDK, so local restore/build/test claims remain unverified.
- The GitHub connector used for these commits does not expose a per-commit author-email override; authenticated repository identity was used rather than falsely claiming `sanskarin@outlook.in` was applied.

### Commits Added in This Continuation

- `09bb8075` — feat(navigation): harden shared mode selection semantics
- `bc8d41b3` — test(navigation): cover direct and boundary mode selection
- `b5b380f6` — feat(a11y): define shared keyboard mode navigation shortcuts
- `a9a56234` — test(a11y): cover shell keyboard navigation shortcuts
- `b598f145` — feat(a11y): wire keyboard-first shell navigation
- `827417c4` — docs(a11y): expand keyboard navigation reference
- `3a8a6e0c` — ci(a11y): extend keyboard contract validation to shell shortcuts
- `056f582e` — test(ci): cover keyboard contract validator
- `5f63850f` — ci(a11y): add dedicated keyboard contract workflow
- `ee6038fe` — fix(navigation): ignore transient invalid tab indexes
- `f6ad7fcc` — test(navigation): cover transient invalid tab indexes

## 2026-08-19 — Adaptive + Touch Validation Continuation

### Added — Adaptive Layout Validation

- Added `tools/validate_adaptive_layout.py`.
- Added SDK-independent checks for compact, medium, and expanded layout classes.
- Added checks for width-change handling and compact fallback behavior.
- Added checks that focused controls are brought into view.
- Added checks that every primary CalcNova mode remains present in the shared shell contract.
- Added `tools/tests/test_validate_adaptive_layout.py`.
- Added `.github/workflows/adaptive-layout-validate.yml`.
- Added `docs/ADAPTIVE_LAYOUT.md` with width-profile rules and manual narrow-screen validation guidance.

### Added — Touch Target Regression Validation

- Added `tools/validate_touch_targets.py`.
- Added source checks for the shared 44-DIP interactive-control baseline.
- Added detection for explicit view-level `MinHeight` values below the shared minimum.
- Added `tools/tests/test_validate_touch_targets.py`.
- Added `.github/workflows/touch-target-validate.yml`.

### Validation Status

- The GitHub combined-status endpoint was checked for the latest adaptive-layout documentation commit and returned no exposed statuses at that time.
- No CI PASS is inferred from an empty status list.
- The active continuation environment still does not expose the .NET SDK, so restore/build/test claims remain unverified locally.
- GitHub connector write actions used for this continuation do not expose a per-commit author-email override; commits were therefore created through the authenticated repository identity rather than falsely claiming a custom author email was applied.

### Commits Added in This Continuation

- `1b9bfd44` — ci(adaptive): add responsive layout contract validator
- `c9dedebc` — test(adaptive): cover responsive layout validator
- `964d547f` — ci(adaptive): validate responsive UI contracts
- `5fe0a967` — docs(adaptive): document responsive layout contract
- `9a4637fa` — ci(a11y): add touch target regression validator
- `ffeadd51` — test(a11y): cover touch target validator
- `414c0876` — ci(a11y): validate shared touch target contracts

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

Highest-priority continuation items were:

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

Many source-side items in this older list were completed by the later Full Source Hardening Continuation recorded at the top of this file.

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
