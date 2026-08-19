# CalcNova Features

This document distinguishes implemented source/shared-app integration from remaining product and runtime-validation work. [`PROJECT_STATE.md`](../PROJECT_STATE.md) is the authoritative short-form continuation status.

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
- top-row/numpad digit and numpad arithmetic mappings outside active text fields;
- safe printable/shifted calculator operator mappings outside active text fields;
- tracked caret insertion and forward/reversed selection replacement;
- selection deletion and Backspace-before-caret behavior;
- selection-preserving function/parenthesis wrapping;
- final-expression workload enforcement after editing/wrapping;
- shared TextBox caret/selection synchronization after keyboard and pointer selection changes.

### Remaining product/runtime work

- target-platform clipboard validation;
- Browser/IME/assistive-technology validation of printable shortcuts;
- additional result-presentation choices only where they have a clear correctness and UX contract.

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

### Remaining product/runtime work

- more compact/adaptive function grouping if target-device evidence shows the current horizontal fallback is insufficient;
- optional function discovery/search.

## Exact rational utility

### Implemented

- canonical `BigInteger` numerator/denominator representation;
- positive-denominator and greatest-common-divisor normalization;
- safe canonical behavior for `default(RationalNumber)`;
- exact integer, fraction, finite-decimal, and decimal-scientific parsing;
- exact addition, subtraction, multiplication, division, negation, reciprocal, equality, hashing, and comparison;
- multiplication cross-cancellation and reduced-denominator addition;
- explicit input-length, decimal-scale/exponent, and reduced-bit-length workload bounds;
- raw input-length enforcement before trimming, including oversized whitespace padding;
- Calculator utility view model and shared panel for normalize/add/subtract/multiply/divide workflows;
- source validator, focused workflow, regression tests, and integrated release-preflight coverage.

See [`EXACT_RATIONALS.md`](EXACT_RATIONALS.md).

## Engineering notation utility

### Implemented

- finite `double` formatting with exponents divisible by three;
- 1–15 selectable significant digits;
- rounding normalization across the 1000-mantissa boundary;
- invariant-culture parsing of canonical engineering forms;
- explicit 4,096-character raw input budget before whitespace/numeric parsing;
- the same 4,096-character budget on the shared Format action and engineering input `TextBox`;
- explicit engineering exponent range from -324 through 306;
- rejection of non-finite values, malformed exponents, non-engineering exponents, and non-canonical exponent-form mantissas;
- rejection of non-zero inputs that would underflow to floating-point zero;
- chunked power-of-ten scaling for extreme finite values;
- Calculator utility view model and shared panel;
- core/app/headless regression coverage for input bounds and numeric edge cases;
- source validator, focused workflow, regression tests, and integrated release-preflight coverage;
- focused workflow path coverage for the core formatter, App view model/panel, and their tests.

See [`ENGINEERING_NOTATION.md`](ENGINEERING_NOTATION.md).

## Programmer calculator and Unicode tools

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
- local Unicode scalar metadata for Unicode plane, general category, UTF-8 byte width, and UTF-16 code-unit width;
- visible shared Unicode metadata presentation;
- explicit copy actions for decoded code-point, inspected-text, and metadata results;
- local-first metadata derivation without a network lookup.

### Remaining product/runtime work

- compact-layout/virtualization changes only if real narrow-device testing demonstrates a usability problem;
- screen-reader and keyboard traversal validation on supported platforms;
- richer Unicode names/properties only if a stable local versioned data source is justified.

See [`PROGRAMMER_MODE.md`](PROGRAMMER_MODE.md) and [`UNICODE_METADATA.md`](UNICODE_METADATA.md).

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
- change-aware clear-recents management;
- source contracts for default-pair behavior and preference/privacy notice behavior.

### Remaining product/runtime work

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

### Remaining product/runtime work

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

## History and exports

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
- bounded display preview for currently loaded/search-matching entries;
- complete private clipboard payload retained separately from the bounded preview;
- reusable preview formatter with character/line limits, newline normalization, and UTF-16 boundary safety;
- explicit clipboard-copy export action.

### Remaining product/runtime work

- richer grouped/multi-select management only where it improves usability;
- platform-specific file-save/share polish;
- target-platform persistence/clipboard validation.

See [`EXPORT_PREVIEWS.md`](EXPORT_PREVIEWS.md).

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
- bounded numerical-analysis options and explicit workload caps;
- extreme-finite-value numerical hardening and interpolation/midpoint safeguards;
- visible derivative/root/integral controls with approximate-result labeling;
- nearest sampled-point tracing;
- bounded single-expression table-of-values CSV preview/copy;
- bounded newline-separated multi-expression sampling;
- stable generated series identities;
- identified multi-expression CSV preview/copy;
- deterministic multi-series line patterns that do not depend on color alone;
- synchronized text legend for active multi-series plots;
- combined finite-data fit-to-view across active graph series;
- dedicated graph surface, presentation, numerical-safety, and workload-budget source validation.

### Remaining product/runtime work

- compact/mobile graph-control refinement after target validation;
- axis/grid label polish and optional explicit viewport controls after runtime interaction evidence;
- additional numerical regressions when real-world edge cases are observed;
- runtime keyboard/focus/screen-reader validation.

See [`GRAPH_INTERACTION.md`](GRAPH_INTERACTION.md), [`NUMERICAL_ANALYSIS.md`](NUMERICAL_ANALYSIS.md), and [`GRAPH_NUMERICAL_SAFETY.md`](GRAPH_NUMERICAL_SAFETY.md).

## Statistics

### Implemented source/app integration

- descriptive statistics module and shared view model;
- bounded dataset parsing;
- explicit clipboard copy for analysis summaries;
- paired X/Y bivariate analysis;
- population and sample covariance;
- Pearson correlation when mathematically defined;
- ordinary least-squares regression slope/intercept;
- coefficient of determination when defined;
- regression prediction with stale-model clearing after failed analysis;
- deterministic handling of constant-X, constant-Y, single-pair, mismatched, non-finite, and oversized datasets;
- shared paired-analysis panel and clipboard workflow;
- source validator, focused workflow, regression tests, and integrated release-preflight coverage.

### Later expansion

- richer statistical visualizations where justified by validated UX requirements.

See [`BIVARIATE_STATISTICS.md`](BIVARIATE_STATISTICS.md).

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
- detection/migration of truly unversioned historical JSON;
- fail-closed rejection of corrupt negative and unsupported future schemas;
- shared JSON decoding and validation on native JSON and Browser storage paths;
- bounded native JSON and Browser settings validation;
- About/support view model;
- external-link abstraction.

### Remaining product/runtime work

- final settings information architecture after runtime testing;
- complete accessibility and platform persistence validation.

## Localization

### Implemented source foundation and reviewed surfaces

- stable semantic localization keys;
- complete English catalog;
- complete Hindi catalog for the current semantic key set;
- regional English/Hindi culture selection;
- persisted culture preference;
- multi-catalog completeness, unknown-key, and duplicate-key validation;
- runtime localization of shared shell headers, calculator prompts, onboarding copy, and reviewed settings/history/currency/About/product surfaces;
- settings checkbox localization in the live capture/apply path.

### Remaining product/runtime work

- migrate remaining hard-coded English shared XAML in compile-verified increments;
- localize remaining accessibility names, units/categories, date/time labels, and empty states;
- validate Hindi long-string/Devanagari and large-text layouts on target sizes;
- add additional reviewed languages only after translation and layout review.

## Accessibility and adaptive-layout baseline

### Implemented source/UI measures

- global minimum 44-DIP heights for common interactive controls;
- 54-DIP standard calculator keys;
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
- compact/medium/expanded shell profiles with compact overflow fallback;
- dynamic graph viewport toolbar controls protected by focus/touch-target headless and source contracts;
- runtime accessibility evidence matrix using PASS/FAIL/BLOCKED/NOT RUN states;
- source-level XAML, touch-target, focus, keyboard, graph-keyboard, dynamic-control, adaptive-layout, and evidence validation.

### Remaining validation/polish

- actual Tab/Shift+Tab traversal audit;
- screen-reader testing on target platforms;
- measured high-contrast/theme verification;
- large-text/narrow-window target validation;
- mobile portrait/landscape validation;
- observed compiled Avalonia headless execution and target accessibility validation.

## Release/source validation and evidence

### Implemented

- repository/security source checks;
- XAML XML and shared-binding contracts;
- navigation, keyboard, calculator-selection, graph-keyboard, graph-surface, and graph-series contracts;
- numerical-analysis and graph workload-budget contracts;
- Unicode metadata, exact-rational, engineering-notation, bounded-export, and bivariate-statistics contracts;
- focus visibility, touch-target, dynamic-control accessibility, adaptive-layout, and accessibility-evidence contracts;
- English/Hindi localization contracts;
- converter default/preference-notice contracts;
- settings-schema migration contracts;
- onboarding contracts;
- cross-platform package metadata and Desktop/Browser/Android/iOS workflow contracts;
- exact-tag unsigned iOS simulator workflow contracts;
- Source Preflight workflow self-validation for broad trigger coverage, least-privilege permissions, Python setup, and integrated command execution;
- master Source Preflight triggers across `src/**`, `tests/**`, `tools/**`, `docs/**`, packaging, workflows, and release/build root metadata;
- release documentation/tag/workflow validators;
- artifact manifest generation/verification and SHA-256 integrity validation infrastructure;
- machine-readable release-evidence model, runner, verifier, schema, and source validation;
- regression tests for SDK-independent validators/tooling;
- unified SDK-independent release preflight covering the current critical inventory.

### Still requires observed execution evidence

- .NET restore/build/format/analyzer/test;
- Avalonia compiled-XAML/headless execution;
- Android/iOS workload builds and signed/device packages;
- Browser publish/runtime behavior;
- Windows/Linux/macOS launch and packaging;
- signing/notarization/provisioning/store acceptance;
- target-device accessibility, clipboard, storage, and adaptive-layout behavior.

See [`SOURCE_PREFLIGHT.md`](SOURCE_PREFLIGHT.md), [`VALIDATION_EVIDENCE.md`](VALIDATION_EVIDENCE.md), and [`RELEASE_READINESS_CHECKLIST.md`](RELEASE_READINESS_CHECKLIST.md).

## Privacy baseline

- local calculation;
- local fixed conversion;
- local Unicode metadata;
- local history/settings paths;
- local history/graph export generation;
- no account required for ordinary use;
- no advertising SDK by default;
- no behavioral analytics by default;
- user-triggered clipboard reads only;
- user-triggered clipboard writes only;
- network-enhanced features optional.

## Platforms

Desktop, Browser/WebAssembly, Android, and iOS source heads/composition exist. Platform source presence does not automatically imply a validated package/store build. See [`PLATFORM_SUPPORT.md`](PLATFORM_SUPPORT.md), [`ACCESSIBILITY_TEST_MATRIX.md`](ACCESSIBILITY_TEST_MATRIX.md), and [`PROJECT_STATE.md`](../PROJECT_STATE.md) for validation status.
