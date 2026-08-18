# CalcNova Features

This document distinguishes **implemented source** from **remaining product work**. Source presence is not the same as a validated release; actual build/test/platform status is recorded in `PROJECT_STATE.md` and `docs/PLATFORM_SUPPORT.md`.

## Standard calculator

### Implemented source

- addition, subtraction, multiplication, division;
- contextual calculator-style percentage through the on-screen `%` command;
- explicit modulo/remainder expression operator;
- positive/negative toggle based on evaluated numeric value;
- decimal input and scientific notation;
- parentheses;
- unary plus/minus;
- right-associative exponentiation;
- operator precedence;
- repeated-equals session behavior;
- backspace, clear, result reuse;
- copy result/expression and paste expression through Avalonia clipboard APIs;
- keyboard and numpad routing;
- typed divide-by-zero, syntax, domain, overflow, and workload errors;
- local calculation history when enabled;
- classic memory state: MC, MR, MS, M+, M−.

### Remaining polish

- richer cursor-aware calculator editing beyond standard `TextBox` behavior;
- configurable result formatting wired from settings into every mode;
- optional auto-copy behavior;
- dedicated shortcut/help dialog.

## Scientific calculator

### Implemented source

- square, cube, arbitrary power;
- square root, cube root, nth root;
- reciprocal and absolute value;
- natural, base-10, base-2, and arbitrary-base logarithms;
- exponential function;
- trigonometric and inverse trigonometric functions;
- hyperbolic and inverse hyperbolic functions;
- degrees, radians, and gradians;
- floor, ceiling, truncate, round, sign;
- min/max;
- factorial;
- GCD/LCM;
- combinations/permutations;
- π, e, and τ constants;
- shared scientific keypad UI.

### Remaining polish

- searchable function catalog/command palette;
- engineering/fraction/recurring-decimal presentation after numeric review;
- richer accessibility descriptions for specialized mathematical functions.

## Numeric correctness layer

Implemented numeric strategy includes:

- arbitrary-precision integers with `System.Numerics.BigInteger`;
- decimal arithmetic where values fit the decimal domain;
- finite binary floating-point fallback for transcendental operations;
- deterministic invariant-culture parser internals;
- explicit negative-zero normalization for display;
- workload limits for expensive integer powers/factorials/combinatorics;
- typed domain/overflow/divide-by-zero failures;
- compiled expressions with scoped variables for graphing;
- regression coverage for large integers, decimal precision, rounding boundaries, parser precedence, scientific functions, and error states.

See `docs/CALCULATION_ENGINE.md`.

## Programmer calculator

### Implemented source/UI

- base 2 through base 36 parsing and formatting;
- arbitrary-precision integer conversion;
- binary/octal/decimal/hex display;
- AND/OR/XOR/NOT;
- left shift;
- logical/arithmetic right shift;
- configurable fixed word sizes;
- signed/unsigned two's-complement interpretation;
- fixed-width bit-string visualization;
- shared Programmer mode UI.

### Remaining work

- interactive bit-toggle grid;
- custom-radix selector beyond common-base UI;
- optional Unicode/code-point helper.

## Unit converter

### Implemented source/UI

Offline fixed conversions include definitions for:

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

The engine validates categories, supports unit search, and does not require a network connection. The shared UI includes category/unit selectors, source/target swapping, input, result, and validation messages.

### Remaining work

- favorites/recent conversion pairs;
- copy action directly in converter mode;
- typography/fuel-economy/digital-transfer categories where definitions are reviewed;
- precision settings wired from global preferences.

## Currency converter

### Implemented architecture/UI

- optional `ICurrencyRateProvider` abstraction;
- `ICurrencyRateCache` abstraction;
- no embedded provider key or secret;
- timestamped `CurrencyRateSnapshot` values;
- freshness/staleness detection;
- cached fallback when a configured provider fails;
- manual refresh command;
- native JSON cache implementation;
- Browser `localStorage` cache implementation;
- mocked provider/cache tests;
- shared Currency UI with explicit rate source/time/status.

CalcNova intentionally ships **without a hard-coded live-rate provider** until a provider can be selected whose license/terms and credential model are suitable for an open-source client application. Cached rates can still be consumed when another trusted host/composition layer supplies them.

## Statistics

### Implemented source/UI

- count;
- compensated sum;
- mean;
- median;
- mode where meaningful;
- minimum/maximum/range;
- population/sample variance;
- population/sample standard deviation;
- quartiles and percentiles;
- sorted data model;
- comma/semicolon/whitespace/newline dataset parsing;
- shared Statistics UI;
- domain regression tests.

### Later scope

- covariance;
- correlation;
- simple linear regression;
- saved datasets after explicit user request.

## Equation solver

### Implemented source/UI

- linear equations with unique/no/infinite-solution states;
- quadratic equations;
- repeated roots;
- complex quadratic roots;
- bounded numerical bisection root finder;
- clear degenerate-case handling;
- shared equation UI;
- regression tests.

### Later scope

- richer simultaneous-system UI backed by matrices;
- higher-degree polynomial root utilities after numeric behavior is reviewed;
- explanatory steps only where the engine can produce them reliably.

## Matrices and vectors

### Implemented source/UI

- matrix model and dimension validation;
- addition/subtraction;
- matrix multiplication;
- scalar multiplication;
- transpose;
- determinant with partial pivoting;
- inverse when defined;
- rank;
- linear-system solving;
- vector magnitude;
- dot product;
- 3D cross product;
- shared matrix editor/results UI;
- regression tests for numerical and singular cases.

### Remaining UI work

- dedicated vector workspace;
- richer matrix cell editor instead of text-only matrix entry;
- copy/export helpers.

## Graphing

### Implemented source/UI

- safe `y = f(x)` sampling through the shared expression engine;
- one compiled expression per sampled function;
- bounded sample count;
- invalid-domain/discontinuity segmentation;
- jump splitting safeguards;
- automatic data viewport calculation;
- SVG graph exporter;
- interactive Avalonia `GraphPlotControl`;
- axes/grid rendering;
- pointer drag panning;
- wheel zoom;
- double-tap fit-to-data;
- shared Graph mode with visual plot, sample controls, summary, and point preview;
- regression tests for smooth/discontinuous/invalid-domain/workload cases and SVG export.

### Remaining advanced graph work

- multiple simultaneously styled expressions;
- point trace/crosshair surfaced in the main UI;
- roots/intercepts/extrema helpers;
- numerical derivative/integral;
- polar/parametric plots;
- PNG export/share in addition to SVG.

## Date and duration utilities

### Implemented source/UI

- signed date difference using `DateOnly`;
- absolute day count;
- whole weeks + remaining days;
- Monday–Friday business-day difference;
- add/subtract years, months, weeks, and days in documented calendar order;
- leap-year/month-end behavior through .NET calendar rules;
- fixed-duration conversion between supported fixed units;
- strict `yyyy-MM-dd` UI parsing to avoid locale ambiguity;
- shared Date/Duration mode;
- leap-year, reversed-range, month-end, and duration regression tests.

No timezone is silently assumed for `DateOnly` calculations.

## History

### Implemented native/browser behavior

- chronological local history;
- search;
- result/expression reuse through calculator history storage flow;
- favorite flag;
- delete selected entry;
- clear-all with explicit confirmation;
- configurable history limit;
- optional history enable/disable;
- SQLite-backed native persistence;
- Browser `localStorage` persistence;
- application composition through platform-neutral repository contracts;
- shared History UI.

### Remaining work

- date grouping in the UI;
- multi-select delete;
- auto-cleanup policy setting;
- TXT/CSV export/save flow.

## Settings

### Implemented

- Light/Dark/System theme preference;
- angle-unit preference;
- decimal precision preference with validation;
- grouping-separator preference;
- haptics preference flag;
- history enable/limit;
- reduced-motion preference;
- high-contrast preference;
- reset-to-defaults;
- native atomic JSON persistence;
- Browser `localStorage` persistence;
- shared Settings UI;
- immediate shared theme/angle application when saved.

Some preference flags such as haptics/high-contrast/reduced-motion still require additional platform-specific behavior beyond persistence.

## About / support

Implemented shared About view includes:

- CalcNova name/tagline;
- repository link;
- GitHub profile;
- Apache-2.0/open-source statement;
- business contacts;
- support contact;
- optional Buy Me a Coffee link;
- platform-safe external-link service abstractions.

Support actions are non-blocking and never gate calculator functionality.

## Privacy baseline

Implemented architecture is local-first:

- calculations run locally;
- history is local;
- settings are local;
- Browser persistence uses browser storage;
- native history/settings/currency cache use local application storage;
- no account is required for ordinary use;
- no advertising SDK is included by default;
- no behavioral analytics SDK is included by default;
- no live currency provider or API secret is embedded;
- network-enhanced functionality is optional.

## Platforms

Source heads exist for:

- Windows/Linux/macOS desktop through `CalcNova.Desktop`;
- Android;
- iOS;
- Browser/WebAssembly + installable PWA shell.

All heads reuse the shared modular Avalonia UI/application state and platform-neutral calculation libraries. Exact CI/manual validation status is tracked separately in `docs/PLATFORM_SUPPORT.md` and `PROJECT_STATE.md`.
