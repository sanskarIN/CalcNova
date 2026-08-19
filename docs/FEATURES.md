# CalcNova Features

This document distinguishes implemented source/shared-app integration from remaining product work. [`PROJECT_STATE.md`](../PROJECT_STATE.md) is the authoritative short-form continuation status.

## Standard calculator

### Implemented

- addition, subtraction, multiplication, division;
- modulo/remainder expression operator;
- decimal and scientific-notation input;
- parentheses and unary plus/minus;
- right-associative exponentiation and explicit precedence;
- typed divide-by-zero/syntax/domain/workload errors;
- clear, backspace, evaluate, and result reuse;
- calculator-style percentage transformation;
- repeated-equals session behavior;
- MC, MR, MS, M+, M- memory behavior;
- sanitized imported expression text with common calculator-glyph normalization;
- user-triggered sanitized clipboard paste;
- valid-result clipboard copy through a platform abstraction.

### Remaining product work

- cursor/selection-aware calculator editing;
- broader physical keyboard/numpad shortcuts;
- configurable result-presentation modes beyond current feature-specific formatting;
- target-platform clipboard validation.

## Scientific calculator

### Implemented

- square/cube/power;
- square/cube/nth root;
- reciprocal and absolute value;
- ln/log/log10/log2;
- exp;
- trigonometric and inverse-trigonometric functions;
- hyperbolic/inverse-hyperbolic functions;
- degree/radian/gradian modes;
- floor/ceil/round/truncate/sign;
- min/max;
- factorial;
- GCD/LCM;
- combinations/permutations;
- π, e, τ constants;
- shared scientific keypad controls.

### Remaining product work

- more compact/adaptive function grouping;
- function discovery/search;
- optional engineering/fraction presentation after numeric-design review.

## Programmer calculator

### Implemented

- base 2–36 parse/format and shared selector;
- arbitrary-precision radix conversion;
- binary/octal/decimal/hex synchronized representations;
- AND/OR/XOR/NOT;
- left/logical-right/arithmetic-right shifts;
- bounded word-size masking;
- signed/unsigned two's-complement interpretation;
- fixed-width bit-string visualization;
- full 8/16/32/64/128-bit interactive bit grid;
- byte-grouped shared presentation for large bit grids;
- explicit copy actions for binary/octal/decimal/hex/fixed-width bit representations;
- accessible bit-cell state names;
- fixed-width masked non-decimal output with signed decimal interpretation;
- Unicode scalar/code-point parsing, formatting, scalar-to-text, and bounded text inspection;
- visible shared Unicode code-point workflow;
- explicit copy actions for decoded code-point and inspected-text results.

### Remaining product work

- compact-layout/virtualization polish for 64/128-bit grids;
- screen-reader and keyboard validation on supported platforms.

## Unit converter

### Implemented

Offline fixed categories include length, area, volume, mass, speed, temperature, time, data/storage, frequency, pressure, energy, power, force, and angle.

Additional implemented source/app behavior:

- category safety;
- unit swapping;
- reusable validated conversion pairs;
- bounded recent-pair tracking;
- favorite pairs;
- visible pair restoration controls;
- 1–17 significant-digit output precision with shared presets;
- versioned pair persistence tokens;
- persisted precision/recent/favorite state through shared settings;
- automatic restore and deliberate-change autosave;
- category-scoped unit search;
- selected search-result assignment to From/To units;
- explicit result copy;
- change-aware clear-recents management.

### Remaining product work

- optional per-category default pairs;
- compact responsive layout refinement;
- target-platform clipboard/accessibility validation.

## Currency converter

### Implemented source/app integration

- replaceable provider interface;
- provider/cache integration;
- local cache support;
- offline fallback semantics;
- no embedded provider secret requirement;
- shared currency view model and shell integration;
- mocked/provider-focused tests.

### Remaining product work

- final provider/release policy;
- visible freshness/source semantics refinement;
- full network/privacy UX validation per platform.

## Date and duration utilities

### Implemented

- date difference;
- calendar arithmetic;
- business-day utilities;
- fixed-duration conversion;
- shared date/time view model and shell integration.

## History

### Implemented

- history repository abstraction;
- SQLite native implementation;
- browser-safe storage path;
- initialization/add/recent/search;
- favorite state;
- delete one/clear all;
- history-limit/settings integration;
- shared history UI/view model;
- bounded TXT/CSV/JSON export engine;
- explicit export-format selection;
- export preview for currently loaded/search-matching entries;
- explicit clipboard-copy export action.

### Remaining product work

- richer grouped/multi-select management where it improves usability;
- platform-specific file-save/share polish;
- additional UI/integration automation.

## Graphing

### Implemented

- `y = f(x)` sampling through the core expression engine;
- configurable bounded X range/sample count;
- invalid-sample/discontinuity segmentation;
- graph viewport model;
- interactive Avalonia plot control;
- deterministic accessible SVG export;
- explicit SVG generation/copy workflow;
- central-difference derivative approximation;
- bracketed bisection root finding;
- composite Simpson definite integration;
- bounded numerical-analysis options;
- visible derivative/root/integral controls with approximate-result labeling;
- nearest sampled-point tracing;
- bounded single-expression table-of-values CSV preview/copy;
- bounded newline-separated multi-expression sampling;
- stable generated series identities;
- identified multi-expression CSV preview/copy.

### Remaining product work

- deterministic multi-series color assignment by theme;
- final pan/zoom/reset controls and labels;
- compact/mobile graph-control refinement;
- expanded numerical edge-case coverage.

## Statistics

### Implemented source/app integration

- statistics module and shared view model;
- dataset analysis flows and tests;
- explicit clipboard copy for the current analysis summary.

### Later expansion

- covariance;
- correlation;
- regression;
- richer statistical visualizations where justified.

## Equations

### Implemented source/app integration

Equation-solving module and shared view model are present, including quadratic workflows exercised by app tests.

### Later expansion

- richer simultaneous-system UX;
- explicit exact-versus-approximate presentation where applicable;
- additional numeric root workflows integrated with graph analysis where useful.

## Matrices

### Implemented source/app integration

- determinant;
- inverse;
- rank;
- linear-system solving;
- shared view model/tests;
- explicit clipboard copy for the current matrix result.

### Later expansion

- richer matrix editing;
- additional vector operations;
- file import/export ergonomics.

## Settings and support

### Implemented source/app integration

- settings repository abstraction;
- shared settings view model;
- theme/angle/history settings integration;
- converter preference persistence;
- bounded native JSON and browser settings validation;
- About/support view model;
- external-link abstraction.

### Remaining product work

- final settings information architecture;
- onboarding/feature discovery;
- localization expansion;
- complete accessibility validation.

## Accessibility baseline

### Implemented source/UI measures

- global minimum 44-pixel heights for common interactive controls;
- 54-pixel standard calculator keys;
- keyboard Enter/Escape/Backspace handling for the primary calculator workflow;
- accessible names for programmer bit cells;
- byte-grouped programmer presentation;
- textual alternatives for bit patterns and graph-analysis results;
- graph CSV output and accessible SVG export paths;
- reduced-motion/high-contrast preference fields in settings;
- source-level shared XAML command/property contract validation;
- source-level Avalonia XML well-formedness validation.

### Remaining validation/polish

- complete keyboard traversal audit;
- screen-reader testing on target platforms;
- high-contrast/theme verification;
- large-text/narrow-window validation;
- compact mobile layout pass;
- real Avalonia UI automation and target accessibility validation.

## Privacy baseline

- local calculation;
- local fixed conversion;
- local history/settings paths;
- local history export generation;
- no account required for ordinary use;
- no advertising SDK by default;
- no behavioral analytics by default;
- user-triggered clipboard reads only;
- user-triggered clipboard writes only;
- network-enhanced features optional.

## Platforms

Desktop, Browser/WebAssembly, Android, and iOS source heads/composition exist. Platform source presence does not automatically imply a validated package/store build. See [`PLATFORM_SUPPORT.md`](PLATFORM_SUPPORT.md) and [`PROJECT_STATE.md`](../PROJECT_STATE.md) for validation status.
