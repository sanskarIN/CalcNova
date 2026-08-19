# Graph Numerical Analysis

CalcNova graph analysis reuses the same project-owned expression engine used for normal calculations. The variable `x` is supplied through `EvaluationOptions.Variables`; graph analysis does not invoke arbitrary code.

## Scope

The current implementation provides three bounded numerical operations:

- central-difference first derivative approximation;
- bracketed root finding by bisection;
- definite integration by composite Simpson's rule.

These operations intentionally return approximate floating-point results. The shared Graph UI labels the analysis section and returned values as approximate.

## Derivative

`GraphNumericalAnalyzer.Derivative(...)` evaluates the expression at `x - h` and `x + h` and uses:

`(f(x + h) - f(x - h)) / (2h)`

The default finite-difference step is `1e-5`. A caller may provide a different positive finite step through `NumericalAnalysisOptions`.

A finite-difference derivative is sensitive to step size, floating-point rounding, rapidly changing functions, discontinuities, and domain boundaries. It is not symbolic differentiation.

## Root finding

`FindRoot(...)` uses bisection and therefore requires an interval with opposite signs at its endpoints unless an endpoint already satisfies the configured tolerance.

Defaults:

- root tolerance: `1e-10`;
- maximum iterations: `128`.

The method rejects invalid intervals and intervals that do not bracket a sign change. Failure to converge within the configured bound is reported rather than silently returning an unverified value.

This method is intentionally conservative. It does not claim to find even-multiplicity roots that do not create a sign change, nor every root in an interval.

## Integration

`Integrate(...)` uses composite Simpson's rule with an even number of intervals.

Defaults:

- integration intervals: `1000`;
- maximum allowed intervals: `100000`.

The options validator rejects odd interval counts, non-positive tolerances/steps, invalid limits, and excessive configured workloads.

Reversed bounds return the negative of the forward integral. Equal bounds return zero.

## Expression evaluation

Each sampled `x` value is converted to a CalcNova `NumberValue` and passed to a compiled expression through the variables dictionary. Evaluation errors are surfaced as analysis errors instead of converted into arbitrary numeric sentinel values.

## Shared UI integration

`GraphingViewModel` and the shared Graph tab expose:

- derivative at `AnalysisX`;
- root search across the current minimum/maximum X interval;
- integration across the current minimum/maximum X interval;
- a separate approximate-analysis result area;
- user-facing error text for invalid numeric input, expression failures, and unbracketed roots.

The numerical-analysis controls are now part of the shared XAML shell. Remaining work is interaction/accessibility validation, trace/table-of-values UX, multiple-expression workflows, and richer export controls.

## Testing expectations

Regression coverage includes:

- polynomial derivative approximation;
- bracketed square-root solving;
- rejection of unbracketed intervals;
- polynomial area integration;
- reversed integration bounds;
- Simpson interval validation;
- graph view-model command output/error behavior.

Additional future tests should cover trigonometric functions, domain boundaries, discontinuities, very small/large intervals, custom angle modes, and workload-limit behavior.

## Validation rule

Numerical-analysis source, view-model integration, shared controls, and tests are implemented. They are **NOT RUN locally in the current continuation environment** because the required .NET SDK is unavailable. Release documentation must only mark them PASS after an observed build/test result.
