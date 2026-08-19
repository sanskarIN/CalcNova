# CalcNova Numerical-Analysis Safety

CalcNova's derivative, root, and integration helpers are bounded numerical approximations. They must remain explicit about approximation and must fail safely when floating-point limits make a requested operation unreliable.

## Derivative safety

Central-difference derivative sampling validates that `x - step` and `x + step` are finite and actually differ from `x` in binary floating-point representation.

A derivative request is rejected when:

- either sample point becomes non-finite;
- the configured step is too small relative to the magnitude of `x` to create distinct sample points;
- the expression produces a non-finite sample;
- the final derivative result is non-finite.

This prevents a tiny default step at an enormous finite `x` from silently producing a meaningless zero-width difference.

## Root-search safety

Bracketed bisection requires a finite, strictly increasing interval with a sign change unless an endpoint is already within root tolerance.

The midpoint uses:

```text
(left / 2) + (right / 2)
```

instead of `(left + right) / 2`, avoiding intermediate overflow for large opposite-sign finite bounds.

If floating-point precision causes the midpoint to become equal to a bracket endpoint, the search returns the endpoint with the smaller absolute function value rather than looping without progress.

Expressions that become invalid/non-finite inside the bracket are rejected instead of being treated as a valid root.

## Integration safety

Composite Simpson integration validates even, bounded interval counts. It computes interval width without directly subtracting potentially extreme bounds before division:

```text
(maximum / intervals) - (minimum / intervals)
```

Interior sample locations use convex interpolation:

```text
minimum * (1 - fraction) + maximum * fraction
```

instead of `minimum + index * width`, avoiding avoidable intermediate overflow.

Non-finite sample points, expression results, or final integral results are rejected.

A zero-width integral returns exactly zero without evaluating the integrand, matching the mathematical interval-width contract.

## Workload bounds

`NumericalAnalysisOptions` constrains:

- derivative step to a finite positive value;
- root tolerance to a finite positive value;
- root iterations to at most 10,000;
- maximum integration intervals to at most 1,000,000;
- integration intervals to an even value within the configured maximum.

These are correctness and workload-safety limits, not performance guarantees for every target device.

## Validation

Run:

```bash
python tools/validate_numerical_analysis.py .
python -m unittest tools.tests.test_validate_numerical_analysis
```

The Graphing test project additionally covers baseline polynomial cases, endpoint roots, discontinuities, enormous finite bounds, representational derivative-step failure, and option boundaries.

Numerical outputs remain approximate and should continue to be labeled with approximation semantics in user-facing UI/export paths.
