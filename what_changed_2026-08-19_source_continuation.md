# What Changed — CalcNova Source Continuation — 2026-08-19

This file is an additive continuation log for work completed while `main` was also receiving concurrent repository updates. It avoids replacing the historical `what_changed.md` from a partial snapshot.

## Localization commits/work

- added semantic shell-localization registry;
- localized all mode tab headers live;
- localized Calculator heading/subtitle/expression watermark;
- localized onboarding content and actions;
- localized Currency, History, Settings, About, footer, and accessibility preference controls;
- localized primary mode headings for Programmer, Unicode, Converter, Statistics, Equations, Matrices, Graphing, and Date/Duration;
- added dedicated CheckBox localization lifecycle for Settings options;
- added reviewed English/Hindi converter preference notice strings;
- added reviewed English/Hindi Graph viewport-control labels;
- expanded headless Hindi UI coverage;
- expanded localization validator/workflow contracts.

## Converter commits/work

- added one deterministic useful default pair per fixed-unit category;
- applied category defaults in `ConverterViewModel`;
- preserved explicit restored recent/favorite pairs;
- added domain and App tests;
- added converter-default validator/test/workflow;
- added visible local preference/privacy notice;
- added converter-notice headless tests and source validator/workflow.

## Graph commits/work

- surfaced the real interactive plot in shared Graph mode;
- synchronized sampled single-series segments with the plot;
- synchronized multi-series samples through `GraphPlotMode`;
- reconciled concurrent App-layer pattern work into a Graphing-domain source of truth;
- defined eight deterministic non-color line patterns;
- added human-readable pattern labels;
- added explicit multi-series text legend;
- added single↔multi transition headless tests;
- added visible localized viewport toolbar;
- exposed public Pan/Zoom viewport operations and reused them from keyboard handling;
- added graph-surface and graph-series validators/tests/workflows;
- integrated graph gates into the unified source preflight while preserving concurrent Unicode/export/numerical-budget gates.

## Numerical analysis commits/work

- reject derivative samples that overflow finite range;
- reject derivative steps that collapse to the same floating-point `x`;
- use overflow-safe root midpoint arithmetic;
- handle bisection endpoint stagnation deterministically;
- use overflow-resistant Simpson width calculation;
- use convex interpolation for integration sample points;
- preserve explicit non-finite rejection;
- added extreme-bound/discontinuity/endpoint-root tests;
- added option/workload-bound tests;
- added numerical-analysis source validator/test/workflow;
- integrated it into release preflight.

## Calculator input commits/work

- expanded safe unmodified physical-key punctuation mappings;
- added exact-Shift top-row operator mappings;
- explicitly reject Control/Alt/combined modifier capture in the Shift mapper;
- added headless Shift-key insertion coverage;
- added Unicode calculator glyph normalization (`×`, `÷`, Unicode minus/dash variants, middle-dot multiplication variants);
- wired glyph normalization through non-editor `OnTextInput` only;
- kept TextBox/onboarding/non-Calculator exclusions;
- expanded keyboard source validator;
- added focused printable-keyboard and calculator-text-symbol workflows.

## Repository hardening commits/work

- added incomplete implementation marker validator for source/tests;
- rejects TODO, FIXME, `NotImplementedException`, placeholder implementation, and temporary implementation markers;
- added validator regression tests;
- added focused incomplete-code workflow;
- added extended source-preflight workflow covering `src`, `tests`, `tools`, `docs`, workflows, and state/changelog paths;
- added standalone documentation for new converter, graph, numerical, keyboard, and localization contracts;
- added additive continuation checkpoint for safe state handoff.

## Validation boundary for this continuation

New source/test/workflow presence is **not** treated as proof that the newest batch has compiled or passed runtime validation.

Required observed validation remains:

- integrated Python source preflight;
- .NET restore/format/build/test;
- Avalonia headless tests;
- target Desktop/Browser/Android/iOS workflows;
- real target accessibility/layout/clipboard/persistence checks;
- signing/archive/store validation where applicable.

If a check has not been observed, record it as `NOT RUN` rather than PASS.
