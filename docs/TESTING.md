# CalcNova Testing Strategy

CalcNova treats mathematical correctness and regression protection as primary engineering requirements.

## Baseline commands

From the repository root:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

A check is only PASS when it actually completes successfully. If a required platform/environment is unavailable, record `NOT RUN` with the reason.

## Current test projects

### CalcNova.Core.Tests

Protects:

- precedence and associativity;
- unary operators;
- decimal arithmetic;
- arbitrary-precision integer arithmetic;
- scientific notation;
- typed errors;
- angle modes;
- scientific functions;
- workload limits;
- numeric equality/hash invariants.

### CalcNova.Programmer.Tests

Protects:

- base conversion;
- large integer round trips;
- signed/two's-complement interpretation;
- fixed-width bitwise operations;
- invalid radix input.

### CalcNova.Converter.Tests

Protects:

- known fixed-unit identities;
- affine temperature conversion;
- cross-category rejection;
- search.

### CalcNova.Persistence.Tests

Protects native SQLite history behavior:

- schema initialization;
- add/read;
- search;
- favorites;
- delete;
- clear.

## Unit-test expectations

Every domain feature should test:

1. representative valid cases;
2. boundaries;
3. invalid input;
4. previous bug regressions;
5. deterministic semantics.

Mathematical code should avoid assertion tolerances that are so broad they would hide a real correctness failure.

## Property / invariant tests

Planned invariant coverage includes properties such as:

- `a + 0 = a`;
- `a * 1 = a`;
- radix format/parse round trip;
- fixed-unit round trips within justified tolerance;
- deterministic parse/evaluate output;
- formatter/parser round trips for supported forms;
- programmer signed/unsigned conversion invariants.

Property-based tooling should only be added if its dependency cost is justified; deterministic generated test loops are also acceptable.

## Parser regression matrix

Maintain tests for:

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

Also test malformed parentheses, invalid function calls, invalid scientific notation, input-length limits, and unsupported identifiers.

## Scientific tests

Use known reference inputs for:

- trigonometric angles in each mode;
- inverse functions;
- logarithm domains;
- roots;
- factorial boundaries;
- power overflow/workload limits;
- combination/permutation edge cases.

## Programmer tests

Include:

- decimal/binary/octal/hex conversions;
- bases 2–36;
- negative values;
- word-size boundaries;
- shift behavior;
- two's-complement min/max;
- bit grid/string state.

## Converter tests

Prefer exact published physical-unit identities where possible. Temperature conversions must test offsets as well as scale.

Every newly added unit category needs at least one known identity and one round-trip test.

## Persistence tests

Tests should use isolated temporary storage and remove it after execution. Migration tests must be added before persisted schema-breaking changes.

Browser storage requires a separate browser-compatible test strategy.

## UI tests

Planned Avalonia UI/headless tests should cover:

- keypad input;
- expression display;
- result/error state;
- mode switching;
- history;
- settings;
- dialogs;
- theme switching;
- accessibility labels/focus where tooling allows it.

## Integration tests

Important end-to-end flows include:

- calculate -> store history -> recall expression;
- scientific function with angle mode;
- programmer base conversion;
- unit conversion;
- preference persistence;
- offline startup.

## Visual regression

Stable major layouts can use screenshot/snapshot tests after the design system settles. Visual snapshots should not replace semantic/assertion-based interaction tests.

## Manual testing

Before release, smoke-test:

- compact phone-like size;
- landscape/medium layout;
- desktop resizing;
- keyboard-only input;
- high-DPI/text scaling;
- dark/light themes;
- offline behavior;
- long expressions/results;
- error recovery.

## Bug-fix rule

For a confirmed deterministic bug:

1. reproduce it;
2. add a regression test when practical;
3. fix the root cause;
4. run relevant tests/analyzers;
5. inspect adjacent behavior;
6. document user-visible impact;
7. commit the fix as a focused change.
