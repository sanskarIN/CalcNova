# CalcNova Roadmap

This roadmap tracks planned work without promising fixed release dates. Implementation order may change when correctness, accessibility, platform constraints, or CI findings require it.

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
- Byte-grouped bit view-model collections for all supported word sizes.
- Shared byte-group presentation.
- Shared copy actions for binary/octal/decimal/hex/fixed-width bit representations.
- Accessible bit-cell names.
- Unicode scalar/code-point helper backend and shared UI.
- Shared Unicode decode/inspection result copy actions.

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
- SVG graph export engine plus shared generation/copy workflow.
- Bounded numerical derivative, root, and integral analysis.
- Shared derivative/root/integral controls with approximate-result labeling.
- Shared nearest sampled-point graph tracing.
- Shared bounded single-expression table-of-values CSV export/copy.
- Shared bounded multi-expression sampling and identified CSV export/copy.
- Newline-separated multi-expression parsing with stable generated series identities.

### Persistence and platform architecture

- Native SQLite calculation history behind an abstraction.
- Browser-safe history/storage path.
- Search/favorite/delete/clear history flows.
- TXT/CSV/JSON history export engine.
- Shared history export-format selection, preview, and clipboard copy.
- Settings/preferences abstraction.
- Converter preference persistence through shared settings.
- Desktop, Browser/WebAssembly, Android, and iOS heads/composition.
- Shared Avalonia `TopLevel` clipboard service attachment.
- Shared clipboard dependency injection into Calculator, Programmer, Unicode, Converter, Statistics, Matrices, Graphing, and History modes.
- Repository validation, format, test/build, coverage, security, advanced-utility, release, UI-contract, and platform workflow foundations.
- Source-level shared-XAML command/property contract validation.
- Source-level Avalonia XML well-formedness validation.

## Now

### Adaptive design and accessibility

- Replace remaining desktop-first assumptions with compact/mobile layouts.
- Improve navigation when many modes are present on narrow screens.
- Audit focus order and visible focus states.
- Verify screen-reader behavior for the programmer bit grid, graph analysis, long exports, and dynamic results.
- Verify touch target sizes, large-text behavior, and high-contrast behavior.
- Add reduced-motion behavior where animation is introduced.
- Evaluate virtualization or alternative compact presentation for 64/128-bit programmer grids on narrow devices.

### Validation hardening

- Observe actual GitHub Actions/check results for the new shared-source changes.
- Fix compile/analyzer/format/test failures before release claims.
- Add targeted integration/UI automation where stable and maintainable.
- Exercise clipboard behavior on Desktop, Browser, Android, and iOS target environments.
- Exercise settings migration/restore behavior on native and Browser storage paths.
- Keep vulnerability, repository, XAML, UI-contract, docs, and asset validation gates active.

### Interaction polish

- Improve cursor/selection-aware expression editing.
- Add broader physical keyboard/numpad shortcuts.
- Improve empty/error/loading states without hiding calculation details.
- Refine graph controls and large export previews for compact layouts without weakening workload bounds.
- Evaluate native file-save/share UX for history/graph exports after platform abstractions are validated.

## Next

### Converter productivity

- Add optional per-category default-pair preferences.
- Add visible persistence/privacy explanation for saved converter preferences.
- Keep physical conversions fully offline.

### Programmer productivity

- Evaluate user-selectable custom word sizes only if interaction and workload semantics remain clear.
- Add additional code-point metadata only from stable local data sources.

### Graphing experience

- Add deterministic color assignment by theme for multiple expressions.
- Improve pan/zoom/reset controls and axis/grid labeling.
- Continue workload-budget and discontinuity tests.
- Add additional numerical-analysis edge-case coverage.

### Platform packaging

- Validate Windows packaging on supported Windows tooling.
- Validate Linux packaging on supported Linux tooling.
- Validate macOS packaging/signing guidance on Apple tooling.
- Validate Android application bundle/release signing pipeline.
- Validate iOS signing/archive guidance on Apple tooling.
- Validate Browser/WebAssembly publish output and hosting guidance.

## Later

### Product polish

- Finalize the original CalcNova icon/splash asset set for every platform density/format.
- Add optional onboarding and feature discovery.
- Consolidate the design system and reusable view primitives.
- Add reviewed localization infrastructure and language packs.
- Add accessibility presets where they provide real value.
- Add repository screenshots and final social preview assets.

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
