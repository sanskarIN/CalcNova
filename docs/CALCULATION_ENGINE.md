# CalcNova Calculation Engine

## Purpose

The calculation engine is intentionally independent of Avalonia UI so mathematical behavior can be tested without platform or presentation dependencies.

The current pipeline is:

```text
expression text
  -> Tokenizer
  -> Parser
  -> Expression syntax tree
  -> ExpressionEvaluator
  -> NumberValue or CalculationErrorCode
```

CalcNova does not evaluate expression text as C#, JavaScript, shell code, or another executable language.

## Expression syntax

Current syntax supports:

- integer literals;
- decimal literals;
- scientific notation;
- parentheses;
- unary `+` and `-`;
- `+`, `-`, `*`, `/`, `%`, `^`;
- function calls;
- comma-separated function arguments;
- constants.

The tokenizer also accepts display-friendly multiplication/division/minus symbols and normalizes them to calculation tokens.

## Operator precedence

From lower to higher precedence:

1. addition/subtraction;
2. multiplication/division/modulo;
3. unary sign handling around power semantics;
4. exponentiation;
5. primary values/functions/parentheses.

Exponentiation is right-associative:

```text
2 ^ 3 ^ 2 = 2 ^ (3 ^ 2) = 512
```

Unary minus is intentionally interpreted so:

```text
-2 ^ 2 = -(2 ^ 2) = -4
```

Negative exponents remain valid:

```text
2 ^ -2 = 0.25
```

These semantics are regression-tested and should not be changed accidentally by parser refactors.

## Numeric representation

`NumberValue` currently stores one of three representations.

### BigInteger

Used for exact integers and integer-preserving arithmetic. This supports values far beyond 64-bit limits as long as workload/memory constraints remain practical.

### decimal

Used for many finite decimal arithmetic paths. This makes ordinary decimal calculations such as `0.1 + 0.2` avoid the classic binary floating representation artifact.

### double

Used when transcendental functions or unsupported decimal ranges require BCL floating-point mathematics.

Only finite floating values are accepted as normal results. Non-finite outcomes are converted into typed errors.

## Exact vs approximate results

Integer operations are exact until an operation intentionally requires a non-integer or floating result.

Decimal operations use `decimal` while both values can be represented safely in that form.

Transcendental functions such as trigonometric functions, logarithms, and non-integer powers are approximate real-number computations based on BCL `double` math.

Future UI should distinguish approximation where doing so improves mathematical clarity.

## Rounding

The current `round` function uses midpoint-to-even behavior. Result-format precision controls and engineering/scientific notation are separate presentation concerns and must not silently change internal mathematical meaning.

## Angle units

Supported angle units:

- radians;
- degrees;
- gradians.

Trigonometric input is converted to radians before BCL evaluation. Inverse trigonometric output is converted from radians into the active angle unit.

The active angle unit must remain visible in scientific UI.

## Constants

Current constants include:

- `pi` / `π`;
- `e`;
- `tau` / `τ`.

## Current functions

The evaluator currently supports function families including:

- roots/powers: `sqrt`, `cbrt`, `sqr`, `cube`, `pow`, `root`;
- basic: `abs`, `reciprocal`, `percent`, `mod`;
- logarithmic/exponential: `ln`, `log`, `log10`, `log2`, `exp`;
- trigonometric: `sin`, `cos`, `tan`, `asin`, `acos`, `atan`;
- hyperbolic: `sinh`, `cosh`, `tanh`, `asinh`, `acosh`, `atanh`;
- rounding/order: `floor`, `ceil`, `round`, `trunc`, `sign`, `min`, `max`;
- integer/combinatoric: `factorial`, `gcd`, `lcm`, `comb`, `perm`.

Function aliases are defined in source and should be documented alongside any future user-facing function catalog.

## Error model

The engine returns typed error states for conditions including:

- empty expression;
- syntax error;
- divide by zero;
- domain error;
- numeric overflow;
- invalid argument;
- unsupported function;
- input too long;
- workload limit exceeded.

User interfaces should preserve the expression when evaluation fails and show a useful message rather than replacing the input with a fabricated result.

## Workload safeguards

`EvaluationOptions` currently provides configurable limits for:

- maximum expression length;
- maximum factorial input;
- maximum integer exponent.

Limits are intended to prevent an ordinary calculator input from creating unreasonable CPU/memory work.

Graphing will require separate sampling/time/workload budgets because a valid function may otherwise be evaluated thousands of times.

## Percentage semantics

The expression-language `%` operator currently means modulo/remainder.

Calculator-style contextual percentage behavior (for example, percentage-as-part-of-an-addition workflow) is a separate presentation/calculator-state feature and must not silently redefine expression-language modulo.

## Localization

Internal expression syntax remains culture-independent. Locale-aware decimal/grouping presentation should be handled at the input/presentation boundary so internal meaning is never changed by display formatting.

## Testing requirements

Changes to parser or numeric semantics should include tests for:

- precedence;
- associativity;
- unary operators;
- parentheses;
- scientific notation;
- decimal boundaries;
- very large integers;
- divide-by-zero;
- domain errors;
- negative zero;
- rounding boundaries;
- angle conversions;
- integer workload limits;
- equality/hash invariants;
- regression cases.

## Future numeric work

Potential improvements include:

- exact rational values;
- complex-number support;
- fraction display;
- configurable arbitrary decimal precision;
- recurring-decimal representation.

These features should only be added when their semantics, display behavior, serialization, and cross-platform tests are defined clearly.
