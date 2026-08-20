# Exact Rational Arithmetic

CalcNova includes a bounded `BigInteger`-backed rational-number type and a shared Calculator utility for exact fraction arithmetic.

Unlike binary floating-point calculation, an exact rational stores a numerator and denominator. Finite decimal input is converted to its exact decimal fraction before reduction.

Examples:

- `0.1` -> `1/10`;
- `0.125` -> `1/8`;
- `6/8` -> `3/4`;
- `1.25e-3` -> `1/800`.

## Canonical representation

`RationalNumber` normalizes values to one canonical representation:

- denominator zero is rejected;
- denominator sign is made positive;
- numerator/denominator are divided by their greatest common divisor;
- zero is represented as `0/1` and displayed as `0`;
- `default(RationalNumber)` is treated as canonical zero rather than exposing an invalid `0/0` state;
- integer rationals are displayed without `/1`.

Canonical normalization makes equality and hash-code behavior stable across equivalent inputs such as `2/4`, `1/2`, and `0.5`.

## Accepted input

`RationalNumber.Parse` accepts:

- integers: `42`, `-7`;
- fractions: `3/4`, `-12/25`;
- finite decimals: `0.1`, `.5`, `5.`;
- decimal scientific notation: `1.25e3`, `1.25e-3`.

All decimal/scientific input is parsed from decimal digits into `BigInteger` numerator/denominator components. It is not first converted through `double`.

That means:

`0.1 + 0.2 = 3/10`

inside the exact-rational utility.

## Arithmetic

Implemented exact operators:

- addition;
- subtraction;
- multiplication;
- division;
- unary negation;
- reciprocal;
- exact comparison.

Multiplication cross-cancels numerator/denominator factors before the final products are formed. Addition reduces the denominator work by first finding the denominator greatest common divisor.

Every final rational is normalized again through the same constructor contract.

## Workload bounds

Exact arithmetic can grow very large, so CalcNova applies explicit limits:

- maximum input length: **4,096 characters**;
- maximum decimal exponent/scale magnitude: **10,000**;
- maximum reduced numerator bit length: **65,536 bits**;
- maximum reduced denominator bit length: **65,536 bits**.

The raw input-length check runs before trimming so oversized whitespace-wrapped input cannot bypass the text budget.

Magnitude checks apply to the reduced numerator and positive normalized denominator.

Arithmetic whose final reduced numerator or denominator exceeds the bit budget fails deterministically rather than allowing unbounded exact-number growth.

## Calculator utility

`RationalNumberViewModel` exposes:

- left input;
- right input;
- canonical left/right forms;
- exact result;
- operation summary;
- Normalize;
- Add;
- Subtract;
- Multiply;
- Divide.

`RationalNumberPanel` displays those controls in the shared Calculator mode.

The utility is intentionally separate from the main calculator evaluator. It provides an explicit exact-arithmetic workflow without changing existing floating/scientific expression semantics.

### Shared-shell integration

`MainView.BivariateStatistics.cs` currently owns the small feature-extension lifecycle used by supplemental shared-mode panels.

The extension locates the Calculator stack by the actual `CalculatorViewModel` data context and attaches:

- `EngineeringNotationPanel`;
- `RationalNumberPanel`.

It does not depend on a hard-coded tab index and avoids replacing the large shared XAML file while other UI work is active.

## Error behavior

The exact-rational implementation rejects:

- zero denominators;
- division by zero;
- invalid fraction/decimal syntax;
- multiple slash or exponent markers;
- non-integer decimal exponents;
- input above the text budget, including oversized whitespace padding;
- decimal exponent/scale outside the configured bound;
- final reduced values above the bit-length budget.

`TryParse` converts supported parse failures into a deterministic `false` result and outputs `RationalNumber.Zero`.

The Calculator utility clears stale canonical/result text after a failed operation so an old exact result is not presented as the result of invalid new input.

## Source contracts

Core:

- `src/CalcNova.Core/Numerics/RationalNumber.cs`

Application:

- `src/CalcNova.App/ViewModels/RationalNumberViewModel.cs`
- `src/CalcNova.App/Controls/RationalNumberPanel.cs`
- `src/CalcNova.App/Views/MainView.BivariateStatistics.cs`

Tests:

- `tests/CalcNova.Core.Tests/RationalNumberTests.cs`
- `tests/CalcNova.App.Tests/RationalNumberViewModelTests.cs`
- `tests/CalcNova.App.Tests/RationalNumberPanelHeadlessTests.cs`
- `tests/CalcNova.App.Tests/RationalNumberMainViewHeadlessTests.cs`

## SDK-independent validation

```bash
python tools/validate_rational_numbers.py .
python -m unittest tools.tests.test_validate_rational_numbers
```

The exact-rational validator and its regression suite are also part of `python tools/release_preflight.py`.

Focused workflow:

- `.github/workflows/rational-numbers-validate.yml`

## Evidence policy

Implementation, test source, source validation, focused workflow, panel, and shared-shell integration are present.

Compiled `.NET`/Avalonia execution is separate observed evidence. Record each actual run as:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Source presence does not prove a compiled PASS, and one environment's unavailable SDK does not make `NOT RUN` the permanent status of the completed exact-rational feature.
