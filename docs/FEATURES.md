# CalcNova Features

This document distinguishes implemented source, shared-app integration, and remaining product work. [`PROJECT_STATE.md`](../PROJECT_STATE.md) is the authoritative short-form continuation status.

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
- sanitized imported expression text with common calculator-glyph normalization.

### Remaining product work

- cursor/selection-aware calculator editing;
- explicit sanitized clipboard service/event wiring;
- broader physical keyboard/numpad shortcuts;
- configurable result-presentation modes beyond current feature-specific formatting.

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

- base 2–36 parse/format;
- arbitrary-precision radix conversion;
- common binary/octal/decimal/hex representations in the app view model;
- AND/OR/XOR/NOT;
- left shift;
- logical/arithmetic right shift;
- bounded word-size masking;
- signed/unsigned two's-complement interpretation;
- fixed-width bit-string visualization;
- bounded bit inspection/toggling;
- app view-model bit-toggle command with synchronized representation refresh;
- Unicode scalar/code-point parsing, formatting, scalar-to-text, and bounded text inspection.

### Remaining product work

- visible accessible bit-toggle grid;
- visible Unicode code-point helper;
- custom radix selector UI for the full 2–36 range;
- accessible copy actions.

## Unit converter

### Implemented

Offline fixed categories include:

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

Additional implemented source/app behavior:

- category safety;
- unit swapping;
- reusable validated conversion pairs;
- bounded recent-pair tracking;
- favorite pairs;
- pair restoration command;
- 1–17 significant-digit output precision with common presets.

### Remaining product work

- visible recent/favorite-pair controls;
- visible precision selector;
- searchable unit/category selectors;
- result copy action;
- persistence semantics for pair state.

## Currency converter

### Implemented source

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
- TXT/CSV/JSON export.

### Remaining product work

- richer grouped/multi-select management where it improves usability;
- platform-specific export/share polish;
- additional UI/integration automation.

## Graphing

### Implemented

- `y = f(x)` sampling through the core expression engine;
- configurable bounded X range/sample count;
- invalid-sample/discontinuity segmentation;
- graph viewport model;
- interactive Avalonia plot control;
- deterministic SVG export;
- central-difference derivative approximation;
- bracketed bisection root finding;
- composite Simpson definite integration;
- bounded numerical-analysis options;
- graph-analysis commands in the shared view model.

### Remaining product work

- visible derivative/root/integral controls in shared XAML;
- multiple expression UX;
- trace/table-of-values;
- final pan/zoom/reset controls and labels;
- richer export UI;
- expanded numerical edge-case coverage.

## Statistics

### Implemented source/app integration

Statistics module and shared view model are present with dataset analysis flows and tests.

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

Matrix module and shared view model include determinant, inverse, rank, and linear-system solving workflows with tests.

### Later expansion

- richer matrix editing;
- additional vector operations;
- copy/import/export ergonomics.

## Settings and support

### Implemented source/app integration

- settings repository abstraction;
- shared settings view model;
- theme/angle/history-related settings integration;
- About/support view model;
- external-link abstraction.

### Remaining product work

- final settings information architecture;
- onboarding/feature discovery;
- localization expansion;
- complete accessibility review.

## Privacy baseline

- local calculation;
- local fixed conversion;
- local history/settings paths;
- no account required for ordinary use;
- no advertising SDK by default;
- no behavioral analytics by default;
- network-enhanced features optional.

## Platforms

Desktop, Browser/WebAssembly, Android, and iOS source heads/composition exist. Platform source presence does not automatically imply a validated package/store build. See [`PLATFORM_SUPPORT.md`](PLATFORM_SUPPORT.md) and [`PROJECT_STATE.md`](../PROJECT_STATE.md) for validation status.
