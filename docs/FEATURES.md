# CalcNova Features

This document distinguishes implemented source from planned product scope. `PROJECT_STATE.md` is the authoritative short-form continuation status.

## Standard calculator

### Implemented source

- addition, subtraction, multiplication, division;
- modulo/remainder expression operator;
- decimal input;
- parentheses;
- unary plus/minus;
- exponentiation;
- operator precedence;
- scientific notation;
- clear/backspace/evaluate UI actions;
- result reuse;
- typed divide-by-zero/syntax/domain errors.

### Planned completion work

- calculator-style percentage semantics;
- repeated equals;
- memory buttons/state;
- richer cursor-aware editing;
- clipboard service;
- physical numpad routing beyond the initial key actions;
- history/favorites UI;
- configurable result formatting.

## Scientific calculator

### Implemented source

- square/cube/power;
- square/cube/nth root;
- reciprocal and absolute value;
- ln/log/log10/log2;
- exp;
- trigonometric functions;
- inverse trigonometric functions;
- hyperbolic/inverse hyperbolic functions;
- degree/radian/gradian modes;
- floor/ceil/round/truncate/sign;
- min/max;
- factorial;
- GCD/LCM;
- combinations/permutations;
- π, e, τ constants;
- initial scientific keypad controls.

### Planned completion work

- polished grouped keypad/layout;
- function discovery/search;
- additional numeric precision modes;
- optional engineering/fraction presentation after numeric review.

## Programmer calculator

### Implemented source

- base 2–36 parse/format;
- arbitrary-precision integers;
- AND/OR/XOR/NOT;
- left shift;
- logical/arithmetic right shift;
- word-size masking;
- signed/unsigned two's-complement interpretation;
- bit-string visualization.

### Planned UI

- binary/octal/decimal/hex synchronized display;
- custom-radix UI;
- word-size selector;
- signed/unsigned selector;
- bit toggle grid;
- optional character/code-point helper.

## Unit converter

### Implemented source

Offline fixed conversions include current definitions in these categories:

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

The engine supports search and prevents cross-category conversion.

### Planned UI

- searchable category/unit selectors;
- recent pairs;
- favorites;
- swap;
- copy;
- precision controls.

## Currency converter

Planned optional network-enhanced feature.

Requirements before implementation:

- replaceable provider interface;
- no embedded secret key;
- cached last successful rates;
- visible rate timestamp;
- offline handling;
- manual refresh;
- ability to disable network features;
- mocked provider tests.

## History

### Implemented source

Native SQLite repository supports:

- initialization;
- add;
- recent entries;
- search;
- favorite flag;
- delete one;
- clear all.

### Planned

- application service/composition;
- grouped history UI;
- multi-select delete;
- clear confirmation;
- history limits/auto-cleanup;
- export;
- browser-compatible storage implementation.

## Memory

Planned: MC, MR, M+, M−, MS with clear state indication. Multiple named slots are optional later work.

## Graphing

Planned module:

- `y = f(x)`;
- multiple expressions;
- show/hide;
- pan/zoom/reset;
- axes/grid/labels;
- discontinuity handling;
- workload safeguards;
- trace/table-of-values.

Advanced analysis such as roots, extrema, numerical derivative/integral, polar, and parametric plots remains later scope until core graphing is tested.

## Statistics

Planned:

- count/sum/mean/median/mode;
- min/max/range;
- variance;
- population/sample standard deviation;
- quartiles/percentiles;
- editable/pasteable dataset.

Optional later: covariance, correlation, regression.

## Equations

Planned:

- linear equation in one variable;
- simultaneous linear equations;
- quadratic equations;
- numeric root finding;
- matrix-backed systems where appropriate.

Exact/approximate results must be clearly distinguished.

## Matrices/vectors

Planned:

- creation/editing;
- add/subtract/multiply/scalar operations;
- transpose;
- determinant;
- inverse where defined;
- rank;
- linear-system solving;
- magnitude/dot/cross for supported vector dimensions.

## Settings

Planned settings categories:

- appearance;
- calculator;
- history;
- accessibility;
- advanced;
- privacy/network;
- help;
- support;
- about.

## Privacy

Product baseline:

- local calculation;
- local history;
- local settings;
- no account required for ordinary use;
- no advertising SDK by default;
- no behavioral analytics by default;
- network features optional.

## Platforms

See `docs/PLATFORM_SUPPORT.md` for implemented versus planned platform status.
