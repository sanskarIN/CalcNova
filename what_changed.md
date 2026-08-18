# What Changed

## 2026-08-18

### Added — Repository / Architecture

- Established CalcNova as a modular **C# + .NET 10 + Avalonia UI** open-source project.
- Added repository-wide formatting, line-ending, ignore, nullable, analyzer, warnings-as-errors, deterministic-build, and central-package configuration.
- Added `global.json` SDK pinning and central NuGet version management.
- Added `CalcNova.slnx` for the normal core/application/domain/desktop/test validation graph.
- Added `CalcNova.All.slnx` to record the all-target graph including Android, iOS, and Browser heads.
- Added `CalcNova.Platform` for platform-neutral history/settings/external-link contracts.
- Added `CalcNova.Persistence` for native SQLite/JSON persistence implementations.
- Added atomic `AppComposition` dependency publication so thin platform heads configure shared application services without duplicating application logic.
- Added a dedicated full-baseline validation branch and PR:
  - branch: `ci/full-baseline-validation`
  - PR: `#6 — ci: validate full cross-platform CalcNova baseline`
- Added `docs/VALIDATION_BASELINE.md` with factual PASS/FAIL/QUEUED/NOT RUN rules.

### Added — Core Calculation Engine

- Added typed calculation error codes/exceptions.
- Added safe tokenizer and recursive-descent expression parser.
- Added expression syntax-tree models.
- Added deterministic expression evaluation without arbitrary code execution.
- Added right-associative exponentiation.
- Added unary plus/minus and parentheses.
- Added scientific notation parsing.
- Added mixed numeric representation using:
  - `BigInteger` for arbitrary-precision integer paths;
  - `decimal` where representable;
  - finite `double` fallback for transcendental operations.
- Added typed divide-by-zero, domain, overflow, invalid-argument, unsupported-function, input-length, and workload-limit failures.
- Added compiled expressions with scoped variables for graphing.
- Added angle-unit model for degrees/radians/gradians.
- Added calculation-session state for repeated-equals behavior.
- Added contextual calculator-style percentage transformation while explicit `%` expression syntax remains modulo/remainder.
- Added classic calculator memory domain model: MC, MR, MS, M+, M−.
- Added evaluated positive/negative toggle behavior.
- Added numeric equality/hash cross-kind contract handling.

### Added — Scientific Calculator

- Added square, cube, arbitrary power.
- Added square root, cube root, nth root.
- Added reciprocal and absolute value.
- Added natural/base-10/base-2/arbitrary-base logarithms.
- Added exponential function.
- Added trig/inverse trig.
- Added hyperbolic/inverse hyperbolic functions.
- Added floor, ceiling, truncate, round, sign.
- Added min/max.
- Added factorial.
- Added GCD/LCM.
- Added combinations/permutations.
- Added π, e, and τ constants.
- Added workload guards for expensive powers/factorials/combinatorics.

### Added — Programmer Calculator

- Added base 2–36 parsing and formatting.
- Added arbitrary-precision programmer values.
- Added binary/octal/decimal/hex output.
- Added fixed word-size interpretation.
- Added signed/unsigned two's-complement behavior.
- Added AND/OR/XOR/NOT.
- Added left shift and logical/arithmetic right shifts.
- Added bit-pattern visualization.
- Added Programmer view model and modular shared UI.

### Added — Offline Unit Converter

- Added fixed-unit categories and affine/multiplicative conversion model.
- Added local/offline definitions for:
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
- Added category validation and unit search.
- Added source/target swapping and modular shared Converter UI.

### Added — Statistics

- Added count.
- Added compensated sum.
- Added mean/median/mode.
- Added minimum/maximum/range.
- Added sample/population variance.
- Added sample/population standard deviation.
- Added quartiles and percentiles.
- Added sorted-data result model.
- Added editable comma/semicolon/whitespace/newline dataset parsing.
- Added modular shared Statistics UI.

### Added — Equations

- Added linear equation solving with unique/no/infinite-solution states.
- Added quadratic solving.
- Added repeated-root handling.
- Added complex quadratic roots.
- Added degenerate quadratic-to-linear behavior.
- Added bounded numerical bisection root finding.
- Added modular shared Equation UI.

### Added — Matrices / Vectors

- Added matrix model and dimension validation.
- Added matrix addition/subtraction.
- Added matrix multiplication.
- Added scalar multiplication.
- Added transpose.
- Added determinant using partial pivoting.
- Added inverse when defined.
- Added rank.
- Added linear-system solving.
- Added vector magnitude.
- Added dot product.
- Added supported 3D cross product.
- Added modular shared Matrix UI.

### Added — Graphing

- Added graph-safe expression sampling through the shared expression engine.
- Added one compiled expression per sampled function.
- Added bounded sampling workload.
- Added invalid-domain/discontinuity segmentation.
- Added large-jump splitting safeguards.
- Added automatic viewport calculation.
- Added deterministic SVG export.
- Added Avalonia `GraphPlotControl`.
- Added axes/grid rendering.
- Added pointer drag panning.
- Added wheel zoom.
- Added coordinate text.
- Added double-tap fit-to-data.
- Added explicit Fit Data and Reset View actions.
- Added modular shared Graph mode with a real visual plot rather than only text previews.

### Added — Date / Duration Utilities

- Added `DateOnly` signed difference.
- Added absolute day count.
- Added whole weeks + remaining days.
- Added Monday–Friday business-day calculation.
- Added calendar add/subtract for years/months/weeks/days in documented order.
- Added fixed-duration conversion that does not silently treat months/years as fixed durations.
- Added strict `yyyy-MM-dd` shared UI parsing to avoid locale ambiguity.
- Added modular Date/Duration UI.

### Added — Currency Architecture / UI

- Added `ICurrencyRateProvider`.
- Added `ICurrencyRateCache`.
- Added currency-code normalization.
- Added timestamped validated currency-rate snapshots.
- Added freshness/staleness behavior.
- Added forced provider refresh.
- Added cached fallback after provider failure.
- Added native JSON currency-rate cache.
- Added Browser `localStorage` currency-rate cache.
- Added modular Currency UI showing amount, source/target ISO codes, result, rate timestamp, rate source, freshness/staleness, and refresh action.
- Intentionally did **not** embed a private live-rate API key/provider into the open-source client.

### Added — History / Persistence

- Added platform-neutral `HistoryEntry` and history-repository contracts.
- Added native SQLite history implementation.
- Added Browser `localStorage` history repository.
- Added chronological history UI.
- Added search.
- Added favorite/un-favorite.
- Added selected-entry delete.
- Added configurable history limit.
- Added optional history enable/disable.
- Added clear-all confirm/cancel flow.
- Added user-initiated CSV export through Avalonia's storage-provider save-file picker.
- Added deterministic CSV export formatting with UTC timestamps and correct CSV quoting.

### Added — Settings / Formatting

- Added platform-neutral settings model/repository contract.
- Added atomic native JSON settings repository.
- Added Browser `localStorage` settings repository.
- Added Light/Dark/System theme preference.
- Added angle-unit preference.
- Added decimal precision preference.
- Added grouping-separator preference.
- Added history enable/limit preferences.
- Added haptics preference flag.
- Added reduced-motion preference.
- Added high-contrast preference.
- Added reset-to-defaults flow.
- Added locale-aware calculator display formatter separated from canonical parser-safe result state.
- Added culture-specific grouping support including Indian digit grouping.
- Added propagation from saved Settings into active calculator angle mode and visible result formatting.

### Added — Input / Clipboard

- Added shared keyboard routing for calculator mode.
- Added Enter/repeated-equals.
- Added Escape clear.
- Added Backspace when calculator surface owns the key.
- Added physical numpad digits/operators.
- Added layout-aware `KeySymbol` routing for calculator characters.
- Added F9 sign toggle.
- Added Ctrl/Cmd+C calculator-level copy.
- Added Ctrl/Cmd+V length-bounded expression paste.
- Added touch actions for:
  - Copy result;
  - Copy expression;
  - Paste expression.
- Preserved normal platform text-editing behavior when a `TextBox` owns the key/clipboard shortcut.

### Added — Shared Modular Avalonia UI

- Replaced the large monolithic shared XAML with focused mode views under `src/CalcNova.App/Views/Modes/`:
  - `CalculatorModeView`
  - `ProgrammerModeView`
  - `ConverterModeView`
  - `StatisticsModeView`
  - `EquationsModeView`
  - `MatricesModeView`
  - `GraphingModeView`
  - `DateTimeModeView`
  - `CurrencyModeView`
  - `HistoryModeView`
  - `SettingsModeView`
  - `AboutModeView`
- Reduced `MainView` to navigation/composition.
- Changed Desktop `MainWindow` to host the same shared `MainView` used by Android/iOS/Browser single-view heads.

### Added — About / Support

- Added modular About view.
- Added CalcNova name/tagline/open-source statement.
- Added repository link.
- Added GitHub profile link.
- Added business emails.
- Added support email.
- Added optional Buy Me a Coffee link.
- Added safe external-link abstraction and Desktop/Android/iOS/Browser implementations.
- Kept support actions optional and non-blocking.

### Added — Platform Heads

#### Desktop

- Added Avalonia Desktop startup.
- Added native local history/settings/currency-cache composition.
- Added safe external-link service.
- Added deterministic project-owned icon generation.
- Added Windows/Linux/macOS packaging metadata and helper scripts.

#### Android

- Added Avalonia Android head.
- Added application identity `in.sanskar.calcnova`.
- Added native local persistence composition.
- Added Android-safe external-link service.
- Added adaptive launcher icon resources.
- Added splash/theme resources.
- Added permission-minimal manifest baseline.
- Added Android workload CI.
- Added release signing path through repository secrets rather than committed keystore/passwords.

#### iOS

- Added Avalonia iOS head.
- Added native local persistence composition.
- Added iOS-safe external-link service.
- Added launch metadata/screen.
- Added generated AppIcon asset catalog based on project-owned artwork.
- Added iOS simulator CI.

#### Browser / WebAssembly / PWA

- Added Avalonia Browser/WebAssembly head.
- Added Browser `localStorage` history/settings/currency cache.
- Added safe Browser external-link interop.
- Added PWA manifest.
- Added service worker/offline app-shell baseline.
- Added favicon/icon/social assets.
- Added Browser publish CI.

### Added — Branding / Packaging

- Added master CalcNova SVG logo.
- Added original calculator/nova icon source.
- Added optional support badge artwork.
- Added social-preview artwork.
- Added Android adaptive icon/splash source.
- Added Browser/PWA icons.
- Added deterministic dependency-free brand/raster generator.
- Added asset ownership/license documentation.
- Added Linux desktop/AppStream metadata and package helper.
- Added macOS bundle metadata and package helper.
- Added Windows packaging template/helper.
- Added release workflow foundation with checksums and secret-based signing inputs.

### Added — Tests

Created/expanded test source for:

- `CalcNova.Core.Tests`
- `CalcNova.Programmer.Tests`
- `CalcNova.Converter.Tests`
- `CalcNova.Statistics.Tests`
- `CalcNova.Equations.Tests`
- `CalcNova.Matrices.Tests`
- `CalcNova.Graphing.Tests`
- `CalcNova.DateTime.Tests`
- `CalcNova.Currency.Tests`
- `CalcNova.Persistence.Tests`
- `CalcNova.App.Tests`

Important regression coverage now includes:

- parser precedence and associativity;
- unary/exponent edge cases;
- large integers and decimal arithmetic;
- scientific notation;
- scientific function domains/angle modes;
- workload limits;
- numeric equality/hash behavior;
- compiled/scoped graph variables;
- memory behavior;
- repeated equals;
- contextual percentage;
- sign toggle;
- bases 2–36;
- two's-complement/shift boundaries;
- invalid radix separator/sign-only input;
- exact/affine unit conversions;
- descriptive statistics;
- linear/quadratic/complex/bisection equation behavior;
- matrix determinant/inverse/rank/system solving;
- vector operations;
- graph discontinuities/workload/viewport/SVG export;
- Date/Duration leap-year/reversed-range/month-end/duration behavior;
- currency provider/cache/freshness/stale fallback;
- SQLite history lifecycle;
- native JSON settings;
- native JSON currency cache including corrupt/missing files;
- advanced-mode view models;
- history enable/disable;
- settings load/save propagation;
- history clear confirmation;
- history CSV formatting;
- result formatting/grouping, including `en-IN` grouping;
- Date/Duration/Currency view-model workflows.

### Added — CI / Repository Automation

- Added formatting workflow.
- Added build/test workflow.
- Added code coverage workflow.
- Added repository-validation workflow.
- Added documentation checks.
- Added security/dependency audit workflow.
- Added Desktop build workflow.
- Added Android workload build workflow.
- Added Browser/WebAssembly publish workflow.
- Added iOS simulator build workflow.
- Added release workflow.
- Updated Android/Browser/iOS path filters so all shared `src/**` changes trigger consuming platform heads.
- Removed legacy GitHub template workflow targeting .NET 8.
- Removed legacy placeholder WPF/WAP desktop-packaging workflow.

### Changed

- Moved persistence contracts out of native SQLite implementation so Browser builds never require the native database package.
- Changed platform heads to compose shared app dependencies through `AppDependencies` / `AppComposition`.
- Made dependency publication/reset atomic.
- Changed Desktop from a separate duplicated XAML workspace to the same shared `MainView` as other targets.
- Refactored the shared UI into focused mode views.
- Changed calculator result presentation so localization/grouping/precision never mutates canonical math result state.
- Changed History clear-all from immediate destructive action to explicit confirmation.
- Changed History export to use platform storage-provider capability rather than direct machine-specific file paths.
- Changed Graphing from text-preview-only presentation to real interactive rendered graph UI.
- Changed public documentation to distinguish source availability from validated release status.

### Fixed

- Fixed numeric parsing code that previously used an invalid exponent-marker API usage.
- Fixed programmer radix parsing so separator-only/sign-only input is rejected without empty-span indexing.
- Fixed `NumberValue` equality/hash consistency across internal numeric kinds.
- Fixed calculator touch/XAML bindings that were ahead of the stale `CalculatorViewModel` implementation.
- Fixed Android startup composition that was constructing `AppDependencies` with an obsolete positional signature.
- Fixed Browser startup composition with the same obsolete dependency signature.
- Wired Desktop/Android/iOS/Browser external-link services through the current composition contract.
- Wired native/Browser currency cache implementations through the current composition contract.
- Fixed currency-rate cache dictionary copying by using explicit enumerable conversion instead of relying on constructor overload assumptions.
- Corrected an incorrect Date/Duration regression-test expectation to match documented calendar-add order.
- Fixed History export so it does not require a resizable output stream.
- Fixed saved decimal-precision/grouping settings so they affect visible calculator output instead of remaining persistence-only fields.
- Fixed stale README/feature/platform/roadmap/build/testing/architecture/keyboard documentation so it matches actual source.

### Documentation

Updated/created:

- `README.md`
- `PROJECT_STATE.md`
- `what_changed.md`
- `CHANGELOG.md`
- `CONTRIBUTING.md`
- `CODE_OF_CONDUCT.md`
- `SECURITY.md`
- `SUPPORT.md`
- `docs/README.md`
- `docs/ARCHITECTURE.md`
- `docs/BUILDING.md`
- `docs/CALCULATION_ENGINE.md`
- `docs/FEATURES.md`
- `docs/KEYBOARD_SHORTCUTS.md`
- `docs/ACCESSIBILITY.md`
- `docs/PRIVACY.md`
- `docs/SECURITY.md`
- `docs/TESTING.md`
- `docs/RELEASE.md`
- `docs/PLATFORM_SUPPORT.md`
- `docs/TROUBLESHOOTING.md`
- `docs/DESIGN_SYSTEM.md`
- `docs/LOCALIZATION.md`
- `docs/ROADMAP.md`
- `docs/VALIDATION_BASELINE.md`
- asset-license documentation;
- packaging metadata/scripts;
- GitHub issue forms/PR template/Dependabot/workflows.

The README now presents the implemented modes/platform heads/privacy model while explicitly warning that source implementation does not automatically equal a validated stable release.

### Git / Atomic Development

The repository was developed through a large number of small Conventional-Commit-style commits covering individual project creation, domain models, parser stages, functions, feature engines, tests, persistence contracts, platform heads, UI modes, documentation, CI, packaging, and fixes.

Examples from the atomic history include:

- `e7a66976` — feat(calc): implement deterministic expression evaluator
- `c988431a` — test(core): cover arithmetic precedence precision and domains
- `efc3aca6` — feat(programmer): add bounded word bitwise operations
- `fea67947` — fix(programmer): reject separator-only radix input safely
- `c62644c9` — feat(converter): implement offline fixed-unit conversion engine
- `1f2c8dce` — feat(history): implement SQLite calculation history repository
- `dc575df2` — test(history): cover SQLite history lifecycle
- `974baf91` — test(settings): verify calculator formatting preferences propagate
- `fb876dab` — docs(changelog): record current unreleased implementation baseline
- `711c7cd1` — docs(build): document implemented target build and packaging paths

Use GitHub/git history for the complete commit sequence. Missing hashes are not invented merely to make this log look longer.

### Validation

#### Repository access / identity

- GitHub repository exists and is writable — **PASS**.
- Repository owner has push/admin permissions through the connected GitHub tool — **VERIFIED**.
- Requested author email `sanskarin@outlook.in` — **VERIFIED on the repository's initial commit metadata**.

#### Local active execution environment

- `dotnet restore` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet format --verify-no-changes` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet build --configuration Release --no-restore` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet test --configuration Release --no-build` — **NOT RUN — .NET SDK unavailable in active execution environment**.

No local PASS has been fabricated.

#### GitHub Actions / PR validation

Validation branch:

`ci/full-baseline-validation`

Pull request:

`#6 — ci: validate full cross-platform CalcNova baseline`

At the latest confirmed GitHub workflow query during this work segment, these PR-triggered jobs were **QUEUED** rather than concluded:

- Repository Validation
- Code Coverage
- Security Audit
- Formatting
- Build and Test
- Documentation Check

Desktop, Android, Browser, and iOS simulator workflows were also triggered through shared source changes during the validation work. Their final conclusion has not yet been observed through the connector and therefore must not be reported as PASS.

#### Platform status

- Core solution — **source implemented; GitHub validation queued; local dotnet validation NOT RUN**.
- Windows Desktop — **source implemented; validation pending/queued**.
- Linux Desktop — **source implemented; validation pending/queued**.
- macOS Desktop — **source implemented; validation pending/queued; signing/notarization NOT RUN**.
- Android — **source implemented; validation pending/queued; signed store artifact NOT RUN**.
- iOS simulator — **source implemented; validation pending/queued**.
- iOS device/archive/App Store — **NOT RUN — Apple environment/signing required**.
- Browser/WebAssembly/PWA — **source implemented; publish validation pending/queued; manual browser install/offline matrix pending**.

### Repository Audit

Targeted repository index searches during this development segment returned no indexed occurrences for:

- `TODO`
- `FIXME`
- `HACK`
- `placeholder`

This is useful but does **not** replace compiler/analyzer/tests/manual platform validation.

### Remaining / Next Exact Work

1. Let the current PR-triggered GitHub Actions runs conclude.
2. Inspect and fix all concrete formatter/compiler/XAML/analyzer/test/platform failures.
3. Re-run until required automated validation is green.
4. Expand Avalonia headless/UI tests for modular views, mode switching, graph rendering, history confirmation, theme/focus/accessibility semantics.
5. Perform manual accessibility and responsive-form-factor review.
6. Validate Desktop packaging on representative Windows/Linux/macOS environments.
7. Validate Android APK/AAB installation and store signing with external secrets.
8. Validate iOS simulator, then real-device/archive signing in an appropriate Apple environment.
9. Validate Browser PWA install/offline/update behavior across supported browsers/hosting paths.
10. Wire persisted high-contrast/reduced-motion/haptics preferences into concrete target behavior.
11. Continue remaining non-release-blocking roadmap items such as programmer bit-toggle grid, converter favorites, multiple graph expressions, richer matrix/vector UI, and command palette only after baseline stability.
12. Update this file, `PROJECT_STATE.md`, and `CHANGELOG.md` with real CI/manual results before creating `v0.1.0`.
