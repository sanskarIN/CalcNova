# Engineering Notation

CalcNova includes a bounded engineering-notation formatter/parser for finite `double` values.

Engineering notation is scientific-style notation whose decimal exponent is always a multiple of three. That convention keeps the exponent aligned with common SI-prefix steps.

## Formatting

`EngineeringNotationFormatter.Format(value, significantDigits)` accepts finite values and produces a mantissa plus an exponent that is divisible by three.

Examples:

| Value | Output |
| ---: | --- |
| `0` | `0` |
| `12.5` | `12.5` |
| `1234` | `1.234e+3` |
| `1200000` | `1.2e+6` |
| `0.0012` | `1.2e-3` |
| `-0.0000045` | `-4.5e-6` |

Exponent zero is omitted for ordinary-scale values.

## Significant digits

The formatter accepts **1 through 15 significant digits**.

The limit is deliberate: the utility targets finite IEEE-754 `double` values and avoids presenting more user-selected significant digits than the stable decimal formatter contract is intended to expose.

Rounding is performed before the final engineering representation is emitted. If rounding pushes a mantissa from a value just below `1000` to `1000`, CalcNova advances the exponent by three and renormalizes the mantissa.

Example:

`999999.9999999999` with 12 significant digits -> `1e+6`

## Parsing

`EngineeringNotationFormatter.Parse(text)` accepts:

- ordinary invariant-culture finite numbers such as `12.5`;
- canonical engineering exponent forms such as `1.234e+3` or `1.2E-3`.

When an exponent marker is present:

- exactly one `e`/`E` marker is allowed;
- the exponent must be an integer;
- the exponent must be divisible by three;
- the exponent must be within the finite `double` engineering range;
- a non-zero mantissa must have absolute value from `1` inclusive to `1000` exclusive.

## Exponent workload bounds

Accepted engineering exponents are bounded to:

- minimum: **-324**;
- maximum: **306**.

These are the engineering-step boundaries needed to represent the finite `double` domain. Rejecting exponents outside that range also prevents an input such as `0e+999999` from turning scaling into a needless long-running loop even though its mathematical result would still be zero.

## Extreme values

Power-of-ten scaling is performed in chunks of at most 300 decimal exponent steps. This avoids immediately overflowing/underflowing the scaling factor for values near:

- `double.Epsilon`;
- `double.MaxValue`.

The regression suite includes round trips for both extremes plus representative positive, negative, small, and large finite values.

## Error behavior

Formatting rejects:

- `NaN`;
- positive/negative infinity;
- significant-digit counts outside 1–15.

Parsing rejects:

- empty text;
- non-finite values;
- multiple exponent markers;
- non-integer exponents;
- exponent values not divisible by three;
- exponent values outside -324 through 306;
- non-canonical non-zero mantissas in exponent form;
- results that overflow the finite `double` range.

## Source contracts

Implementation:

- `src/CalcNova.Core/Numerics/EngineeringNotationFormatter.cs`

Regression tests:

- `tests/CalcNova.Core.Tests/EngineeringNotationFormatterTests.cs`

SDK-independent validation:

```bash
python tools/validate_engineering_notation.py .
python -m unittest tools.tests.test_validate_engineering_notation
```

Focused workflow:

- `.github/workflows/engineering-notation-validate.yml`

## Evidence policy

The implementation, test source, source validator, and focused workflow are present. Compiled `.NET` tests remain **NOT RUN** until their execution is observed in a suitable .NET 10 environment.
