# CalcNova Continuation Checkpoint — 2026-08-19

This checkpoint records source-side work completed after the earlier project-state snapshot. It is intentionally additive so concurrent edits to `PROJECT_STATE.md` and `what_changed.md` are not destructively overwritten.

## Completed in this continuation

### Live localization expansion

- shared live English/Hindi mode headers;
- Calculator title/subtitle/expression prompt;
- localized onboarding content and actions;
- Currency heading/privacy prompt/actions;
- History heading/search/management/export text;
- Settings language/precision/history labels, Save/Reset, and accessibility CheckBox labels;
- About/support text and persistent local-first footer;
- primary Programmer/Unicode/Converter/Statistics/Equations/Matrices/Graph/Date headings;
- localized converter preference/privacy notice;
- localized visible Graph Pan/Zoom/Fit/Reset controls;
- headless regression scenarios for the migrated Hindi surfaces;
- expanded localization source validator/workflow.

### Converter completion

- deterministic useful default pair for every fixed-unit category;
- category changes apply defaults;
- explicit restored recent/favorite pairs remain authoritative;
- default-pair domain/view-model tests;
- dedicated converter-default source validator/workflow;
- visible local preference notice explaining that precision/recents/favorites stay in local app settings and fixed unit conversion remains offline;
- dedicated preference-notice tests/validator/workflow.

### Graph shared UI completion

- real interactive `GraphPlotControl` surfaced in the shared Graph mode;
- single/multi-series synchronization through `GraphPlotMode`;
- deterministic eight-pattern non-color series distinction;
- explicit text legend containing series label, line-pattern name, and expression;
- one Graphing-domain source of truth for line-pattern order/labels/masks, with App compatibility forwarding;
- visible Pan left/right/up/down, Zoom in/out, Fit graph, and Reset controls;
- toolbar calls the same viewport operations as keyboard handling;
- Hindi viewport-control labels;
- headless graph surface, multi-series, legend, and toolbar interaction coverage;
- dedicated graph-surface and graph-series-presentation validators/workflows.

### Numerical-analysis hardening

- derivative rejects sample-point overflow and steps too small to produce distinct floating-point samples;
- bisection uses overflow-safe midpoint arithmetic;
- bisection handles floating-point bracket stagnation deterministically;
- Simpson integration avoids avoidable bound subtraction/multiplication overflow through stable width calculation and convex sample interpolation;
- non-finite sample/result rejection remains explicit;
- extreme finite-bound, discontinuity, endpoint-root, and option/workload boundary tests;
- dedicated numerical-analysis validator/workflow.

### Calculator keyboard/input polish

- additional unmodified OEM punctuation mappings outside text editors;
- Shift-only common top-row operators: `+`, `*`, `(`, `)`, `^`, `%`;
- strict non-capture of Control/Alt modifier combinations;
- non-ASCII calculator glyph normalization for multiplication/division/minus glyphs outside text editors;
- TextBox/onboarding/non-Calculator exclusions;
- unit/headless/source-contract coverage;
- focused printable-keyboard and text-symbol workflows.

### Repository quality hardening

- integrated source preflight expanded during the continuation with converter defaults/notice, graph surface/series presentation, numerical safety, and concurrent Unicode/export/budget gates;
- dedicated validator rejecting TODO/FIXME/`NotImplementedException`/placeholder implementation markers in source/test trees;
- focused incomplete-code workflow and regression test;
- standalone technical documentation added for converter defaults/privacy, graph series presentation, graph viewport controls, numerical safety, calculator keyboard input, and live localization.

## Validation boundary

The source, tests, validators, and workflows above are implemented. Do **not** convert them into PASS evidence merely because they exist.

Before release, observe the real results for:

- SDK-independent integrated source preflight;
- restore/format/build/full solution tests;
- Avalonia headless App tests;
- Desktop/Browser/Android/iOS target workflows;
- target runtime layout/accessibility/clipboard/persistence tests;
- Windows/macOS native runtime testing;
- Android device/emulator + TalkBack + signing/store checks;
- iOS simulator/device + VoiceOver + signing/archive/distribution checks.

## Remaining source/product work

The main remaining repository-side opportunities are incremental rather than missing core modules:

- continue migrating detailed hard-coded operation/help/status strings to semantic localization;
- refine compact/mobile layout only where observed runtime evidence identifies concrete problems;
- improve platform-native export/share UX after target abstractions are validated;
- continue statistical/vector/mathematical expansion only with explicit correctness/workload contracts;
- update release-facing screenshots/assets only from validated builds.

Runtime/device/package validation is now the dominant release-readiness blocker.
