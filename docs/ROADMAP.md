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

### Programmer mode

- Base 2–36 parsing/formatting.
- Common synchronized radix representations.
- Word-size and signed/unsigned interpretation.
- Bitwise operations and shifts.
- Fixed-width bit visualization.
- Bounded bit inspection/toggle helpers.
- Unicode scalar/code-point helper backend.

### Conversion and utility modules

- Offline fixed-unit conversion engine.
- Swap workflow.
- Conversion-pair model.
- Recent/favorite conversion-pair state.
- Selectable significant-digit display precision.
- Optional currency-rate provider/cache architecture.
- Date difference, calendar arithmetic, business-day, and duration utilities.

### Advanced modes

- Statistics source module/view model.
- Equation source module/view model.
- Matrix source module/view model.
- Graph sampling and discontinuity segmentation.
- Graph viewport and interactive plot control.
- SVG graph export.
- Bounded numerical derivative, root, and integral analysis.

### Persistence and platform architecture

- Native SQLite calculation history behind an abstraction.
- Browser-safe history/storage path.
- Search/favorite/delete/clear history flows.
- TXT/CSV/JSON history export.
- Settings/preferences abstraction.
- Desktop, Browser/WebAssembly, Android, and iOS heads/composition.
- Repository validation, format, test/build, coverage, security, advanced-utility, release, and platform workflow foundations.

## Now

### Shared UI completion

- Add visible graph-analysis controls for derivative, root, and integral actions.
- Add a usable programmer bit-toggle grid.
- Add Unicode code-point inspection UI with accessible labels.
- Add converter precision control, recent-pair picker, and favorite-pair picker.
- Add sanitized clipboard paste/copy workflow behind platform-safe services.
- Improve expression editing and selection behavior.
- Keep every new control usable by keyboard and touch.

### Adaptive design and accessibility

- Replace remaining desktop-first assumptions with compact/mobile layouts.
- Audit focus order and visible focus states.
- Add screen-reader names/descriptions for non-text controls.
- Verify touch target sizes and high-contrast behavior.
- Add reduced-motion behavior where animation is introduced.
- Review graph and bit-grid accessibility alternatives.

### Validation hardening

- Observe actual GitHub Actions results for all shared-source changes.
- Fix compile/analyzer/format/test failures before release claims.
- Add targeted integration/UI automation where stable and maintainable.
- Keep vulnerability, repository, docs, and asset validation gates active.

## Next

### Converter persistence and productivity

- Decide and document persistence semantics for favorite/recent conversion pairs.
- Add searchable unit/category workflow.
- Add direct copy-result action.
- Add optional per-category default pair preferences.
- Keep physical conversions fully offline.

### Programmer productivity

- Add custom radix selector covering the full 2–36 range.
- Add bit-grid grouping for 8/16/32/64/128-bit layouts.
- Add code-point text inspection and scalar-to-text controls.
- Add accessible copy actions for radix representations.

### Graphing experience

- Wire numerical analysis into the visual graph tab.
- Add trace/cursor and table-of-values UX.
- Add multiple-expression model with deterministic color assignment by theme.
- Improve pan/zoom/reset controls and axis/grid labeling.
- Add export controls for SVG and tabular data.
- Continue workload-budget and discontinuity tests.

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
