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
- Copy actions for binary/octal/decimal/hex/fixed-width bit representations.
- Accessible bit-cell names.
- Unicode scalar/code-point helper backend and shared UI.
- Unicode decode/inspection result copy actions.

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
- Category-scoped unit search backend/view-model workflow.
- Search-result assignment as From/To unit.
- Change-aware clear-recents workflow.
- Conversion-result clipboard copy.
- Optional currency-rate provider/cache architecture.
- Date difference, calendar arithmetic, business-day, and duration utilities.

### Advanced modes

- Statistics source module/view model.
- Equation source module/view model.
- Matrix source module/view model.
- Graph sampling and discontinuity segmentation.
- Graph viewport and interactive plot control.
- SVG graph export engine and view-model generation/copy workflow.
- Bounded numerical derivative, root, and integral analysis.
- Shared derivative/root/integral controls with approximate-result labeling.
- Nearest sampled-point graph tracing backend/view-model workflow.
- Bounded single-expression table-of-values CSV export.
- Bounded multi-expression sampling and identified CSV export.
- Newline-separated multi-expression parsing with stable generated series identities.

### Persistence and platform architecture

- Native SQLite calculation history behind an abstraction.
- Browser-safe history/storage path.
- Search/favorite/delete/clear history flows.
- TXT/CSV/JSON history export.
- Settings/preferences abstraction.
- Converter preference persistence through shared settings.
- Desktop, Browser/WebAssembly, Android, and iOS heads/composition.
- Shared Avalonia `TopLevel` clipboard service attachment.
- Shared clipboard dependency injection into Calculator, Programmer, Unicode, Converter, and Graphing modes.
- Repository validation, format, test/build, coverage, security, advanced-utility, release, and platform workflow foundations.

## Now

### Adaptive design and accessibility

- Replace remaining desktop-first assumptions with compact/mobile layouts.
- Improve navigation when many modes are present on narrow screens.
- Audit focus order and visible focus states.
- Verify screen-reader behavior for the programmer bit grid, graph analysis, and dynamic results.
- Verify touch target sizes and high-contrast behavior.
- Add reduced-motion behavior where animation is introduced.
- Apply the existing byte grouping to the shared Programmer UI and evaluate virtualization for 64/128-bit layouts on compact devices.

### Validation hardening

- Observe actual GitHub Actions/check results for the new shared-source changes.
- Fix compile/analyzer/format/test failures before release claims.
- Add targeted integration/UI automation where stable and maintainable.
- Exercise clipboard behavior on Desktop, Browser, Android, and iOS target environments.
- Exercise settings migration/restore behavior on native and Browser storage paths.
- Keep vulnerability, repository, docs, and asset validation gates active.

### Interaction polish

- Improve cursor/selection-aware expression editing.
- Expose programmer radix-copy actions in the shared UI.
- Expose converter search, clear-recents, and result-copy actions in the shared UI.
- Expose graph trace, table, multi-expression, and SVG export actions in the shared UI.
- Add direct copy-result support to matrix/statistics outputs where useful.
- Improve empty/error/loading states without hiding calculation details.

## Next

### Converter productivity

- Add optional per-category default-pair preferences.
- Add visible persistence/privacy explanation for saved converter preferences.
- Keep physical conversions fully offline.

### Programmer productivity

- Add visible byte/nibble headings for large bit grids using the existing byte-group model.
- Evaluate user-selectable custom word sizes only if interaction and workload semantics remain clear.
- Add additional code-point metadata only from stable local data sources.

### Graphing experience

- Connect the existing trace/table/multiple-expression/export workflows to the shared UI.
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
