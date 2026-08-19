# CalcNova Testing Strategy

CalcNova treats mathematical correctness and regression protection as primary engineering requirements.

## Baseline commands

From the repository root:

```bash
python tools/release_preflight.py
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

A check is only PASS when it actually completes successfully. If a required platform/environment is unavailable, record `NOT RUN` with the reason.

The Python preflight is SDK-independent source-contract validation. It does not replace the compiled `.NET` commands.

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
- numeric equality/hash invariants;
- imported-expression sanitization and normalization.

### CalcNova.Programmer.Tests

Protects:

- base 2–36 conversion;
- large integer round trips;
- signed/two's-complement interpretation;
- fixed-width bitwise operations;
- shifts;
- bit inspection/toggling;
- Unicode scalar/code-point helpers;
- invalid radix/input boundaries.

### CalcNova.Converter.Tests

Protects:

- known fixed-unit identities;
- affine temperature conversion;
- cross-category rejection;
- search;
- conversion-pair models;
- recent/favorite state;
- versioned pair tokens;
- significant-digit formatting.

### CalcNova.Persistence.Tests

Protects native persistence behavior:

- SQLite schema initialization;
- history add/read/search/favorites/delete/clear;
- settings JSON round trips;
- culture/converter/onboarding validation;
- legacy settings-schema migration;
- unsupported future settings-schema rejection.

### CalcNova.Platform.Tests

Protects shared platform-independent contracts, including the versioned `AppSettingsSchema` migration boundary.

### CalcNova.App.Tests

Protects shared application/view-model behavior, including:

- calculator/session workflows;
- clipboard behavior through fakes;
- programmer and Unicode view models;
- converter persistence/search/productivity behavior;
- statistics/equation/matrix/graph/date/currency/history/settings view models;
- localization behavior;
- navigation semantics;
- adaptive-layout profile rules;
- keyboard mappings;
- Avalonia headless shared-shell integration tests;
- Avalonia headless graph viewport keyboard tests.

The App test project uses `Avalonia.Headless.XUnit` with xUnit v3 for focused real-control integration scenarios. See [UI_AUTOMATION.md](UI_AUTOMATION.md).

## SDK-independent validator tests

`tools/tests/` regression-tests repository validators themselves. Current protected areas include:

- release preflight inventory;
- release tag/documentation/workflow contracts;
- XAML/source UI contracts;
- keyboard and graph-keyboard contracts;
- adaptive layout;
- touch targets;
- focus visibility;
- accessibility evidence discipline;
- localization catalogs;
- settings schema migration contracts;
- onboarding;
- packaging metadata;
- platform build workflow contracts;
- headless UI-test configuration/execution-path contracts.

These tests are intentionally standard-library Python where practical so they remain runnable without the .NET SDK.

## Unit-test expectations

Every domain feature should test:

1. representative valid cases;
2. boundaries;
3. invalid input;
4. previous bug regressions;
5. deterministic semantics.

Mathematical code should avoid assertion tolerances that are so broad they would hide a real correctness failure.

## Property / invariant tests

Useful invariants include:

- `a + 0 = a`;
- `a * 1 = a`;
- radix format/parse round trip;
- fixed-unit round trips within justified tolerance;
- deterministic parse/evaluate output;
- formatter/parser round trips for supported forms;
- programmer signed/unsigned conversion invariants;
- settings-schema migration preserves unaffected preferences.

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
- bit grid/string state;
- Unicode supplementary scalar behavior.

## Converter tests

Prefer exact published physical-unit identities where possible. Temperature conversions must test offsets as well as scale.

Every newly added unit category needs at least one known identity and one round-trip test.

## Persistence tests

Tests should use isolated temporary storage and remove it after execution. Migration tests must be added before persisted schema-breaking changes.

Native settings migration now has explicit tests. Browser storage shares the same schema normalization contract and still requires real Browser runtime/storage validation.

## Headless UI tests

Implemented Avalonia headless scenarios currently include:

- shared shell mode inventory;
- real Calculator clear-command binding;
- compact adaptive-class application;
- Ctrl+PageDown shell mode navigation;
- high-contrast class application;
- onboarding visibility and Skip behavior;
- graph arrow-key pan;
- graph keyboard zoom;
- graph Home reset;
- graph `F` fit-to-data.

`.github/workflows/headless-ui-validate.yml` restores and executes `CalcNova.App.Tests` under .NET 10 in addition to running SDK-independent source-contract checks.

The headless suite is not a replacement for target-platform accessibility, rendering, touch, clipboard-permission, storage, or packaging tests.

## Integration tests

Important end-to-end flows include:

- calculate -> store history -> recall expression;
- scientific function with angle mode;
- programmer base conversion;
- unit conversion and persisted pair state;
- preference persistence/migration;
- onboarding completion/skip;
- offline startup;
- graph keyboard navigation plus textual alternatives.

Add these to headless automation only when they can be asserted deterministically without depending on unavailable native services.

## Platform workflow validation

Dedicated build workflows exist for:

- Desktop on Windows/Linux/macOS runners;
- Browser/WebAssembly;
- Android;
- iOS simulator.

`tools/validate_platform_workflows.py` protects their source contracts, SDK/workload commands, runners, read-only permissions, and separation from signing secrets. A source-contract PASS does not mean those builds ran successfully.

## Visual regression

Stable major layouts can use screenshot/snapshot tests after the design system settles and the headless interaction suite is observed stable. Visual snapshots should not replace semantic/assertion-based interaction tests.

## Runtime/manual testing

Before release, exercise the evidence matrix in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md) and smoke-test:

- compact phone-like size;
- landscape/medium layout;
- desktop resizing;
- keyboard-only input;
- high-DPI/text scaling;
- dark/light themes;
- CalcNova high contrast;
- offline behavior;
- long expressions/results;
- 64/128-bit programmer grids;
- graph pointer/keyboard interaction;
- clipboard permission/failure flows;
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
