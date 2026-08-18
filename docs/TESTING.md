# CalcNova Testing Strategy

CalcNova treats mathematical correctness, persistence integrity, cross-platform behavior, and bug-regression protection as release requirements.

## Baseline commands

From the repository root:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

A check is only **PASS** when it actually completes successfully. If the required SDK/workload/platform is unavailable, record **NOT RUN** and the reason.

The all-target graph is recorded in `CalcNova.All.slnx`, but Android/iOS/Browser workload builds are intentionally run in dedicated CI workflows.

## Current test projects

### CalcNova.Core.Tests

Protects:

- tokenizer/parser precedence and associativity;
- unary operators;
- decimal arithmetic;
- arbitrary-precision integer arithmetic;
- scientific notation;
- typed errors;
- angle modes;
- scientific functions;
- workload limits;
- numeric equality/hash invariants;
- compiled expressions and scoped variables;
- classic memory operations;
- repeated-equals sessions;
- contextual calculator percentage behavior.

### CalcNova.Programmer.Tests

Protects:

- bases 2–36;
- large integer round trips;
- signed/unsigned and two's-complement interpretation;
- fixed-width bitwise operations;
- shift behavior;
- separator/invalid radix input edge cases.

### CalcNova.Converter.Tests

Protects:

- known fixed-unit identities;
- affine temperature conversion;
- cross-category rejection;
- unit search;
- conversion round trips where appropriate.

### CalcNova.Statistics.Tests

Protects:

- compensated sum;
- mean/median/mode;
- min/max/range;
- sample/population variance and standard deviation;
- quartiles/percentiles;
- sorted data behavior;
- invalid/empty data cases.

### CalcNova.Equations.Tests

Protects:

- unique/no/infinite linear solutions;
- two real quadratic roots;
- repeated roots;
- complex roots;
- degenerate quadratic-to-linear behavior;
- bounded bisection convergence/error cases.

### CalcNova.Matrices.Tests

Protects:

- matrix arithmetic and dimensions;
- transpose;
- determinant;
- inverse;
- rank;
- singularity handling;
- linear-system solving;
- vector magnitude/dot/cross operations.

### CalcNova.Graphing.Tests

Protects:

- smooth-function sampling;
- divide-by-zero/domain discontinuities;
- invalid syntax;
- sample workload limits;
- viewport behavior;
- SVG export.

### CalcNova.DateTime.Tests

Protects:

- signed/reversed date ranges;
- leap-year behavior;
- month-end calendar addition;
- business-day direction;
- fixed-duration conversions;
- unsupported/ambiguous duration assumptions.

### CalcNova.Currency.Tests

Uses mocked providers/caches to protect:

- fresh provider rates;
- cache reuse;
- forced refresh;
- stale cached fallback;
- provider/base mismatch rejection;
- no-provider/no-cache failure behavior;
- timestamp/source semantics.

No live external rate service is required for the unit suite.

### CalcNova.Persistence.Tests

Protects native local persistence:

- SQLite history schema initialization;
- add/read/search;
- favorite/delete/clear lifecycle;
- native JSON settings persistence;
- native JSON currency-rate cache round trips;
- corrupt/missing cache handling.

Tests use isolated temporary storage.

### CalcNova.App.Tests

Protects application/MVVM behavior without requiring a full visual UI runner for every case:

- Programmer/Converter/Statistics/Equations/Matrices/Graphing view-model workflows;
- calculator percentage/repeated-equals/memory/sign-toggle behavior;
- history recording enable/disable;
- settings load/save propagation;
- result-formatting preference propagation;
- Date/Duration view-model validation;
- Currency view-model cached-rate behavior;
- History clear confirmation;
- deterministic History CSV formatting;
- display precision/grouping including Indian grouping semantics.

## Unit-test expectations

Every domain feature should cover:

1. representative valid cases;
2. boundary values;
3. invalid input;
4. deterministic failure behavior;
5. previous confirmed bug regressions;
6. workload-limit behavior where a calculation can become expensive.

Floating/numerical tests should use tolerances justified by the algorithm rather than tolerances broad enough to hide a defect.

## Property / invariant coverage

Useful invariants include:

- `a + 0 = a`;
- `a * 1 = a`;
- radix format/parse round trips;
- fixed-unit round trips within justified tolerance;
- deterministic parse/evaluate output;
- numeric equality/hash consistency;
- formatting leaves canonical calculator result state untouched;
- history persistence round trips;
- matrix identity relationships for supported dimensions.

A dedicated property-testing dependency is optional; deterministic generated loops are acceptable when they provide equivalent value with less dependency cost.

## Parser regression matrix

Maintain coverage for:

```text
1 + 1
-5 + 3
10 / 4
0 / 5
5 / 0
2 * (3 + 4)
-(-8)
2 + 3 * 4
(2 + 3) * 4
2 ^ 3 ^ 2
-2 ^ 2
2 ^ -2
0.1 + 0.2
1 / 3
999999999999999999 + 1
```

Also test malformed parentheses, invalid functions, invalid scientific notation, input limits, unsupported identifiers, unary/exponent boundaries, and scoped graph variables.

## Calculator-session tests

Session-level tests should keep explicit semantics for:

- repeated equals;
- editing resets repeat state;
- percentage context differs from explicit modulo syntax;
- MC/MR/MS/M+/M−;
- sign toggle;
- history labels for repeated operations;
- canonical result vs localized/formatted display result.

## Scientific tests

Use known reference inputs for:

- trig values in each angle mode;
- inverse functions;
- logarithm domains;
- roots;
- factorial limits;
- power overflow/workload limits;
- combination/permutation edge cases.

## Programmer tests

Include:

- binary/octal/decimal/hex;
- bases 2–36;
- negative values;
- word-size boundaries;
- logical/arithmetic shifts;
- signed min/max boundaries;
- format/parse round trips;
- invalid digits and separator-only input.

## Converter tests

Prefer exact physical-unit identities where possible. Affine conversions such as temperature require both offset and scale tests.

Every new unit category needs at least one known identity, validation case, and justified round-trip test.

## Persistence tests

Native tests use temporary paths and remove them after execution. Migration tests must be added before a persisted schema change ships.

Browser storage is implemented separately behind the same contracts; Browser publish/runtime smoke tests are therefore required in addition to native repository tests.

## UI/headless testing

Current source has strong view-model/domain coverage, but release work should expand Avalonia headless/UI tests for:

- modular mode view creation;
- keypad commands;
- mode switching;
- history selection/confirmation;
- settings controls;
- graph custom-control rendering/interaction;
- theme switching;
- keyboard focus order;
- accessibility labels and automation properties where tooling allows.

## Integration flows

Important end-to-end flows include:

- calculate -> save local history -> search/favorite/export;
- scientific calculation -> angle preference save -> restore;
- result precision/grouping preference -> visible formatting without canonical mutation;
- programmer base conversion;
- unit conversion;
- graph expression -> sample -> render -> pan/zoom/fit;
- native settings/history persistence across restart;
- Browser settings/history persistence across reload;
- offline startup with no currency provider;
- cached currency conversion with explicit timestamp/staleness state.

## CI validation

Independent workflows cover:

- formatting;
- core build/test matrix;
- code coverage;
- repository/docs checks;
- security/dependency checks;
- Desktop build;
- Android build;
- Browser/WebAssembly publish;
- iOS simulator build.

Platform workflow path filters include shared `src/**` changes so a change in domain/application code triggers the heads that consume it.

## Visual regression

Stable major layouts can gain screenshot/snapshot tests after the shared modular UI stabilizes. Visual snapshots supplement rather than replace semantic interaction tests.

## Manual release testing

Before a stable release, smoke-test at minimum:

- compact phone size;
- landscape phone;
- tablet/expanded layout;
- desktop resizing;
- Browser resize/install/offline behavior;
- keyboard-only/numpad input;
- clipboard copy/paste;
- high-DPI and text scaling;
- dark/light/system themes;
- local persistence;
- long expressions/results;
- error recovery;
- graph pan/zoom/fit;
- save-file History export where the platform storage provider supports it.

## Bug-fix rule

For a confirmed deterministic bug:

1. reproduce it;
2. add a regression test when practical;
3. fix the root cause;
4. run relevant formatter/analyzers/tests;
5. inspect adjacent behavior;
6. update user-visible documentation/state when needed;
7. commit the fix as a focused atomic change.
