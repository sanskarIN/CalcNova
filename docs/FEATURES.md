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
- valid-result clipboard copy through a platform abstraction;
- top-row/numpad digit and numpad arithmetic mappings outside active text fields.

### Remaining product work

- cursor/selection-aware calculator editing;
- locale-aware printable operator shortcuts without breaking text editing/browser conventions;
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

- more compact/adaptive function grouping if target-device evidence shows current horizontal fallback is insufficient;
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

- compact-layout/virtualization changes only if real narrow-device testing demonstrates a usability problem;
- screen-reader and keyboard traversal validation on supported platforms.

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

- optional per-category default pairs after real settings-migration/storage validation;
- compact responsive layout refinement based on target-device evidence;
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
- real shared-shell UI/integration automation.

## Graphing

### Implemented

- `y = f(x)` sampling through the core expression engine;
- configurable bounded X range/sample count;
- invalid-sample/discontinuity segmentation;
- graph viewport model;
- focusable interactive Avalonia plot control;
- pointer drag panning;
- pointer-wheel zoom around the pointer;
- double-tap/double-click fit-to-data;
- keyboard arrow-key panning;
- keyboard numpad Add/Subtract zoom;
- keyboard Home viewport reset;
- keyboard `F` fit-to-data;
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

- deterministic multi-series visual differentiation that never depends on color alone;
- compact/mobile graph-control refinement after target validation;
- expanded numerical edge-case coverage;
- runtime keyboard/focus/screen-reader validation.

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
- theme/angle/history/accessibility settings integration;
- converter preference persistence;
- persisted culture preference;
- explicit settings schema version;
- legacy schema-v0 normalization to the current schema;
- fail-closed rejection of corrupt negative and unsupported future schemas;
- shared schema normalization on native JSON and Browser storage paths;
- bounded native JSON and Browser settings validation;
- About/support view model;
- external-link abstraction.

### Remaining product work

- final settings information architecture after runtime testing;
- full visible-XAML localization migration;
- complete accessibility and platform persistence validation.

## Localization

### Implemented source foundation

- stable semantic localization keys;
- complete English catalog;
- complete Hindi catalog for the current semantic key set;
- regional English/Hindi culture selection;
- persisted culture preference;
- multi-catalog completeness, unknown-key, and duplicate-key validation.

### Remaining product work

- migrate the predominantly English shared XAML to localized bindings in compile-verified increments;
- localize accessibility names/onboarding/unit display/date labels/About text;
- validate Hindi long-string and large-text layouts on target sizes;
- add additional reviewed languages only after translation and layout review.

## Accessibility baseline

### Implemented source/UI measures

- global minimum 44-pixel heights for common interactive controls;
- 54-pixel standard calculator keys;
- explicit focused-state border emphasis for buttons, text boxes, combo boxes, check boxes, tabs, and list items;
- stronger focused-state emphasis under CalcNova high contrast;
- keyboard Enter/Escape/Backspace handling for the primary calculator workflow;
- Ctrl+PageUp/PageDown/Home/End shared mode navigation;
- focus bring-into-view on shared mode scroll containers;
- accessible names for programmer bit cells;
- byte-grouped programmer presentation;
- textual alternatives for bit patterns and graph-analysis results;
- keyboard-operable graph viewport;
- graph CSV output and accessible SVG export paths;
- reduced-motion/high-contrast preference fields in settings;
- onboarding focus/shortcut source contracts;
- runtime accessibility evidence matrix using PASS/FAIL/BLOCKED/NOT RUN states;
- source-level XAML, touch-target, focus, keyboard, graph-keyboard, adaptive-layout, and evidence validation.

### Remaining validation/polish

- actual Tab/Shift+Tab traversal audit;
- screen-reader testing on target platforms;
- measured high-contrast/theme verification;
- large-text/narrow-window target validation;
- mobile portrait/landscape validation;
- real Avalonia UI automation and target accessibility validation.

## Release/source validation

### Implemented

- repository/security source checks;
- XAML XML and shared-binding contracts;
- keyboard/navigation contracts;
- graph keyboard contracts;
- focus visibility and touch-target contracts;
- adaptive-layout contracts;
- accessibility evidence discipline;
- English/Hindi catalog validation;
- settings-schema migration contracts;
- onboarding contracts;
- cross-platform package metadata contracts;
- release documentation/tag validators;
- regression tests for the SDK-independent validators;
- unified SDK-independent release preflight.

### Still requires execution evidence

- .NET restore/build/format/test;
- Avalonia compiled-XAML/UI automation;
- Android/iOS workload builds and signed packages;
- Browser publish output;
- Windows/Linux/macOS packaging;
- target-device accessibility, clipboard, storage, and adaptive-layout behavior.

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

Desktop, Browser/WebAssembly, Android, and iOS source heads/composition exist. Platform source presence does not automatically imply a validated package/store build. See [`PLATFORM_SUPPORT.md`](PLATFORM_SUPPORT.md), [`ACCESSIBILITY_TEST_MATRIX.md`](ACCESSIBILITY_TEST_MATRIX.md), and [`PROJECT_STATE.md`](../PROJECT_STATE.md) for validation status.
