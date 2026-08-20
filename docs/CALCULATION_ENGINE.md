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

The tokenizer also accepts supported display-friendly multiplication/division/minus symbols and normalizes them to calculation tokens.

Imported/pasted expressions are sanitized and still pass through the normal tokenizer/parser limits.

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

These semantics are regression-protected and should not be changed accidentally by parser refactors.

## Numeric representation

`NumberValue` uses a mixed numeric strategy rather than forcing every operation through binary floating point.

### `BigInteger`

Used for exact integers and integer-preserving arithmetic. This supports values far beyond 64-bit limits while still respecting workload/memory safeguards.

### `decimal`

Used for many finite decimal arithmetic paths. This keeps ordinary decimal calculations such as `0.1 + 0.2` from inheriting the common binary floating-point representation artifact where decimal arithmetic can remain in range.

### finite `double`

Used when transcendental functions, numerical analysis, or unsupported decimal ranges require BCL floating-point mathematics.

Only finite floating values are accepted as normal results. Non-finite outcomes are converted into controlled error states.

## Exact rational utility

CalcNova 2.8.03 also includes a separate exact-rational utility based on bounded canonical `BigInteger` numerator/denominator arithmetic.

It supports exact integer, fraction, finite-decimal, and decimal-scientific parsing plus arithmetic/comparison/normalization without converting finite decimal input through `double` first.

This utility is intentionally documented separately from `NumberValue` expression evaluation because its exact-fraction representation and workload limits are distinct contracts.

See [EXACT_RATIONALS.md](EXACT_RATIONALS.md).

## Engineering notation utility

Engineering notation is also a dedicated bounded format/parse workflow rather than a change to core expression semantics.

It supports exponents in multiples of three, 1–15 significant digits, canonical parsing, explicit exponent limits, non-zero-underflow rejection, and the shared 4,096-character input budget.

See [ENGINEERING_NOTATION.md](ENGINEERING_NOTATION.md).

## Exact vs approximate results

Integer operations remain exact while their result fits the intended exact-integer semantics.

Decimal operations use `decimal` while values/operations can be represented safely in that form.

Transcendental functions such as trigonometric functions, logarithms, and non-integer powers are approximate real-number computations based on finite floating-point math.

Graph derivative/root/integration utilities are also approximate numerical analysis and are presented/documented as such.

The UI must not imply exactness for an approximate numerical result.

## Rounding

The current `round` function uses midpoint-to-even behavior.

Result-format precision controls, engineering notation, and scientific display formatting are presentation/utility concerns and must not silently change the mathematical meaning of the underlying evaluated value.

## Angle units

Supported angle units:

- radians;
- degrees;
- gradians.

Trigonometric input is converted to radians before BCL evaluation. Inverse trigonometric output is converted from radians into the active angle unit.

The active angle unit is application state and must remain clear to the user in scientific workflows.

## Constants

Current constants include:

- `pi` / `π`;
- `e`;
- `tau` / `τ`.

## Current functions

The evaluator supports function families including:

- roots/powers: `sqrt`, `cbrt`, `sqr`, `cube`, `pow`, `root`;
- basic: `abs`, `reciprocal`, `percent`, `mod`;
- logarithmic/exponential: `ln`, `log`, `log10`, `log2`, `exp`;
- trigonometric: `sin`, `cos`, `tan`, `asin`, `acos`, `atan`;
- hyperbolic: `sinh`, `cosh`, `tanh`, `asinh`, `acosh`, `atanh`;
- rounding/order: `floor`, `ceil`, `round`, `trunc`, `sign`, `min`, `max`;
- integer/combinatoric: `factorial`, `gcd`, `lcm`, `comb`, `perm`.

Function aliases are source-defined and should remain synchronized with any user-facing function reference.

## Error model

The engine returns typed/controlled error states for conditions including:

- empty expression;
- syntax error;
- divide by zero;
- domain error;
- numeric overflow/non-finite result;
- invalid argument;
- unsupported function;
- input too long;
- workload limit exceeded.

User interfaces should preserve useful input context when evaluation fails and show a meaningful error rather than fabricating a result or exposing an unhandled stack trace.

## Workload safeguards

`EvaluationOptions` provides configurable safeguards such as:

- maximum expression length;
- maximum factorial input;
- maximum integer exponent.

Related features have their own bounds rather than relying only on expression-engine limits. Examples include:

- exact-rational raw input/scale/bit-length limits;
- engineering notation input/exponent bounds;
- graph sampling and numerical-analysis budgets;
- statistics dataset bounds;
- export preview bounds;
- Unicode inspection limits.

These limits prevent ordinary calculator input from creating unreasonable CPU/memory work.

See [NUMERICAL_SAFETY.md](NUMERICAL_SAFETY.md) and [GRAPH_NUMERICAL_SAFETY.md](GRAPH_NUMERICAL_SAFETY.md).

## Percentage semantics

The expression-language `%` operator means modulo/remainder.

Calculator-style contextual percentage behavior is a separate calculator-session/presentation feature. It must not silently redefine expression-language modulo semantics.

## Calculator session behavior

The shared application layers calculator-specific state above the pure expression evaluator, including:

- repeated-equals behavior;
- MC/MR/MS/M+/M- memory operations;
- calculator percentage workflows;
- caret/selection-aware editing;
- selection-preserving function wrapping;
- user-triggered paste/copy workflows.

This separation keeps parser/evaluator semantics testable without embedding interactive UI state in the domain engine.

See [CALCULATOR_EDITING.md](CALCULATOR_EDITING.md) and [CALCULATOR_KEYBOARD_INPUT.md](CALCULATOR_KEYBOARD_INPUT.md).

## Localization

Internal expression syntax remains culture-independent.

Locale-aware display/culture behavior is handled at application/presentation boundaries so locale formatting does not silently change internal mathematical meaning.

See [LOCALIZATION.md](LOCALIZATION.md).

## Testing requirements

Changes to parser or numeric semantics should include coverage for:

- precedence;
- associativity;
- unary operators;
- parentheses;
- scientific notation;
- decimal boundaries;
- very large integers;
- divide-by-zero;
- domain errors;
- negative zero where relevant;
- rounding boundaries;
- angle conversions;
- integer workload limits;
- equality/hash invariants;
- regression cases.

Changes to exact-rational, engineering-notation, graph numerical, or other specialized numeric behavior must also run their focused regression/validator coverage.

## Optional post-2.8.03 numeric ideas

Possible future enhancements—not missing 2.8.03 requirements—include:

- complex-number workflows;
- broader fraction/exact-result integration in ordinary expression presentation;
- configurable arbitrary decimal precision;
- recurring-decimal representation;
- additional specialist mathematical utilities.

Any such enhancement requires clearly defined semantics, display behavior, workload bounds, serialization/persistence implications where applicable, and cross-platform regression coverage before it becomes part of a later release.
