# CalcNova Roadmap

This roadmap tracks planned work without promising fixed release dates. Implementation order may change when correctness, accessibility, platform constraints, or observed CI findings require it.

## Completed foundations

### Core and scientific calculation

- Safe project-owned tokenizer/parser/evaluator.
- Standard arithmetic and scientific functions.
- Degrees, radians, and gradians.
- Workload limits and typed calculation errors.
- Repeated-equals semantics.
- Calculator-style percentage transformation.
- MC, MR, MS, M+, and M- memory behavior.
- Sanitized external expression import.
- User-triggered sanitized clipboard paste and result copy.
- Shared Avalonia clipboard adapter composed on all current heads.

### Programmer mode

- Base 2–36 parsing/formatting.
- Full base 2–36 shared selector.
- Synchronized binary/octal/decimal/hex representations.
- Fixed-width signed/unsigned interpretation.
- Correct masked non-decimal and signed-decimal presentation.
- AND/OR/XOR/NOT and left/logical-right/arithmetic-right shifts.
- Fixed-width bit visualization.
- Full 8/16/32/64/128-bit interactive grid.
- Byte-grouped presentation for all supported word sizes.
- Shared copy actions for binary/octal/decimal/hex/fixed-width bit representations.
- Accessible bit-cell names.
- Unicode scalar/code-point helper backend and shared UI.
- Shared Unicode decode/inspection result copy actions.
- Local Unicode scalar metadata for general category, Unicode plane, UTF-8 byte width, and UTF-16 code-unit width.
- Shared Unicode metadata presentation and copy controls without a network lookup.

### Conversion and utility modules

- Offline fixed-unit conversion engine.
- Swap workflow.
- Conversion-pair model.
- Bounded recent conversion-pair state.
- Favorite conversion pairs.
- Versioned pair persistence tokens.
- Persisted recent/favorite pairs across launches.
- Selectable and persisted significant-digit display precision.
- Shared recents/favorites/precision controls.
- Shared category-scoped unit search workflow.
- Shared search-result assignment as From/To unit.
- Shared change-aware clear-recents workflow.
- Shared conversion-result clipboard copy.
- Optional currency-rate provider/cache architecture.
- Date difference, calendar arithmetic, business-day, and duration utilities.

### Advanced modes

- Statistics source module/view model and shared summary-copy action.
- Equation source module/view model.
- Matrix source module/view model and shared result-copy action.
- Graph sampling and discontinuity segmentation.
- Graph viewport and interactive plot control.
- Pointer pan/wheel zoom and fit-to-data interaction.
- Keyboard graph pan using arrow keys.
- Keyboard graph zoom using numpad Add/Subtract.
- Keyboard graph Home reset and `F` fit-to-data.
- SVG graph export engine plus shared generation/copy workflow.
- Bounded numerical derivative, root, and integral analysis.
- Shared derivative/root/integral controls with approximate-result labeling.
- Shared nearest sampled-point graph tracing.
- Shared bounded single-expression table-of-values CSV export/copy.
- Shared bounded multi-expression sampling and identified CSV export/copy.
- Newline-separated multi-expression parsing with stable generated series identities.
- Deterministic multi-series line-pattern differentiation that does not rely on color alone.
- Shared multi-series text legend synchronized with the active graph presentation.
- Extreme-bound numerical-analysis hardening plus dedicated edge-case and workload-budget regressions.
- Explicit graph sampling, root-iteration, and Simpson-integration workload-budget coverage.

### Persistence and platform architecture

- Native SQLite calculation history behind an abstraction.
- Browser-safe history/storage path.
- Search/favorite/delete/clear history flows.
- TXT/CSV/JSON history export engine.
- Shared history export-format selection, preview, and clipboard copy.
- Settings/preferences abstraction.
- Converter preference persistence through shared settings.
- Explicit settings schema version with legacy-v0 migration and future-schema rejection.
- Shared settings-schema normalization on native JSON and Browser storage paths.
- Desktop, Browser/WebAssembly, Android, and iOS heads/composition.
- Shared Avalonia `TopLevel` clipboard service attachment.
- Shared clipboard dependency injection into Calculator, Programmer, Unicode, Converter, Statistics, Matrices, Graphing, and History modes.

### Adaptive UI and accessibility source baseline

- Shared available-width profile with compact, medium, and expanded breakpoints.
- Resize-driven shell style-class updates.
- Compact-density styles that preserve common interaction-target baselines.
- Compact horizontal-overflow fallback for shared mode scroll containers.
- Focus-change bring-into-view configuration on shared mode scroll containers.
- Shared 44-DIP minimum touch-target contract.
- Explicit 3-DIP focused-state border emphasis on common keyboard controls.
- Explicit 4-DIP focused-state emphasis when CalcNova high contrast is enabled.
- Shared keyboard mode navigation using Ctrl+PageUp/PageDown/Home/End.
- Graph keyboard viewport controls.
- Onboarding shortcut suppression and focus-restoration source behavior.
- Runtime accessibility evidence matrix with conservative PASS/FAIL/BLOCKED/NOT RUN vocabulary.

### Localization foundation

- Stable semantic `AppStringKey` catalog.
- Complete English semantic catalog.
- Complete Hindi semantic catalog for the current key set.
- English/Hindi regional culture selection such as `en-IN` and `hi-IN`.
- Persisted culture preference.
- Multi-catalog completeness/duplicate validation.
- Runtime localization of shared shell headers, calculator prompts, and onboarding copy.
- Expanded semantic keys for settings, history, currency, About, and other product surfaces.

The shared XAML still contains hard-coded English outside migrated surfaces; semantic catalog support is therefore a foundation, not a claim of fully localized UI.

### Validation and release infrastructure

- Repository validation, formatting, build/test, coverage, security, advanced-utility, release, UI-contract, and platform workflow foundations.
- Source-level shared-XAML command/property contract validation.
- Source-level Avalonia XML well-formedness validation.
- Dedicated adaptive-layout, touch-target, keyboard, graph-keyboard, graph-surface, graph-series-presentation, numerical-analysis, Unicode-metadata, focus-visibility, accessibility-evidence, localization, settings-schema, onboarding, and packaging-metadata source validators/workflows.
- Dedicated graph workload-budget validation covering sample caps, root iteration limits, and integration limits.
- Python regression tests for the source validators added during hardening.
- Unified SDK-independent release preflight covering the current source-contract inventory.
- Versioned release/package identity metadata for supported heads/templates.

## Now — requires real execution evidence

### Build and test validation

- Observe actual `dotnet restore`, build, analyzer, formatter, and test results in a suitable .NET 10 environment.
- Fix every real compiler/analyzer/test failure found by those runs.
- Observe GitHub Actions/check results rather than inferring PASS from workflow/source presence.
- Add targeted Avalonia UI/integration automation where the test stack is stable and maintainable.

### Runtime adaptive design and accessibility

- Test compact/mobile structural behavior on actual target sizes and devices.
- Verify keyboard Tab/Shift+Tab order and visible focus rendering on Desktop/Browser.
- Verify screen-reader behavior for calculator results, programmer bit grids, graph analysis, long exports, settings, and onboarding.
- Verify 64/128-bit programmer interaction on narrow/mobile targets; introduce an alternative/virtualized presentation only if runtime evidence shows the current byte-group layout is inadequate.
- Verify large-text/display-scaling behavior.
- Measure representative contrast/focus/error/disabled/selected states in light, dark, and CalcNova high-contrast modes.
- Verify Browser shortcut conflicts and graph keyboard controls.
- Populate `ACCESSIBILITY_TEST_MATRIX.md` only with observed runtime evidence.

### Platform and packaging validation

- Validate Windows packaging on supported Windows tooling.
- Validate Linux packaging on supported Linux tooling.
- Validate macOS packaging/signing guidance on Apple tooling.
- Validate Android application bundle/release signing pipeline.
- Validate iOS signing/archive guidance on Apple tooling.
- Validate Browser/WebAssembly publish output and hosting guidance.
- Verify clipboard and settings persistence behavior on Desktop, Browser, Android, and iOS.

## Next — source/product polish after observed validation

### Localization UI migration

- Continue migrating remaining visible shared XAML strings to the semantic localization layer in compile-verified increments.
- Localize remaining accessibility names, settings, units/categories, date/time labels, empty states, and About/Support text.
- Validate Hindi long-string/Devanagari layout at compact widths and large text sizes.
- Add further reviewed language packs only when translation quality and layout can be validated.

### Interaction polish

- Improve cursor/selection-aware expression editing.
- Add locale-aware printable keyboard/operator shortcuts without breaking text-box editing, Browser conventions, or assistive technology.
- Improve empty/error/loading states without hiding calculation details.
- Refine long graph/history export previews for compact layouts while preserving workload bounds.
- Evaluate native file-save/share UX for history/graph exports after platform abstractions are validated.

### Converter/programmer polish

- Evaluate optional per-category converter default pairs after settings migration behavior is validated in real storage environments.
- Add visible persistence/privacy explanation for saved converter preferences.
- Evaluate custom programmer word sizes only if interaction and workload semantics remain clear.
- Evaluate richer Unicode names/properties only if a stable local data source can be versioned and validated without weakening the local-first contract.

### Graph presentation

- Improve axis/grid labels and optional explicit viewport controls after runtime graph interaction is validated.
- Extend numerical-analysis regression coverage when new real-world edge cases are observed.

## Later

### Product polish

- Finalize platform-density icon/splash assets after target package builds are observed.
- Consolidate reusable visual primitives only after real UI behavior identifies duplication worth abstracting.
- Add repository screenshots/social preview assets from validated builds.
- Consider accessibility presets only where they provide measurable value beyond platform/system settings.

### Extended mathematics

Only add these when their correctness contract, workload bounds, tests, and UX are clear:

- exact rational representation;
- recurring-decimal visualization;
- complex-number mode;
- engineering notation controls;
- covariance/correlation/regression expansion;
- richer vector workflows;
- saved formulas and user constants;
- deterministic local natural-language calculation patterns.

## Research

These ideas are not release promises:

- reusable high-performance numeric backends where profiling proves a need;
- local-only formula search and tagging;
- OS widgets and quick actions;
- richer graph analysis with carefully documented numerical methods;
- optional local backup/restore formats that remain privacy-first.

## Release gates

A milestone is not complete until its implementation, tests, documentation, supported validation checks, and relevant accessibility/privacy review are complete.

Unavailable checks must be recorded as `NOT RUN`, never as PASS. Platform-specific release readiness must be based on validation in the required target environment, not inferred from source presence alone.
