# CalcNova Graph Numerical Analysis

CalcNova graph analysis reuses the same project-owned expression engine used for normal calculations. The variable `x` is supplied through evaluation variables; graph analysis does not invoke arbitrary code.

The 2.8.03 baseline contains bounded derivative, root-finding, integration, sampling, trace, multi-series, and export workflows. Numerical results are approximate by design and are presented as such.

## Scope

The numerical-analysis implementation provides three primary bounded operations:

- central-difference first-derivative approximation;
- bracketed root finding by bisection;
- definite integration by composite Simpson's rule.

The graphing system also provides bounded expression sampling and related presentation/export workflows used around those operations.

## Approximate-result contract

Derivative, root, and integration results use finite floating-point numerical methods. They are not symbolic mathematics or arbitrary-precision exact analysis.

The shared Graph UI distinguishes the approximate-analysis result area so a user is not led to interpret a numerical approximation as a symbolic proof or exact closed-form result.

Non-finite values and evaluation failures are surfaced as controlled analysis errors rather than arbitrary sentinel values.

## Derivative

`GraphNumericalAnalyzer.Derivative(...)` evaluates the expression at `x - h` and `x + h` and uses the central-difference approximation:

```text
(f(x + h) - f(x - h)) / (2h)
```

The default finite-difference step is `1e-5`. A caller may provide a different positive finite step through the numerical-analysis options.

A finite-difference derivative can be sensitive to:

- step size;
- floating-point rounding;
- rapidly changing functions;
- discontinuities;
- domain boundaries;
- extreme finite values.

It must not be described as symbolic differentiation.

## Root finding

`FindRoot(...)` uses bisection.

The interval must bracket a sign change unless an endpoint already satisfies the configured tolerance.

Current defaults include:

- root tolerance: `1e-10`;
- maximum iterations: `128`.

The method rejects invalid intervals and intervals that do not bracket a sign change. Failure to converge within the configured bound is reported instead of silently returning an unverified value.

Bisection is intentionally conservative. It does not claim to discover every root in an interval or even-multiplicity roots that do not create a sign change.

## Integration

`Integrate(...)` uses composite Simpson's rule with an even number of intervals.

Current defaults include:

- integration intervals: `1000`;
- maximum allowed intervals: `100000`.

Options validation rejects:

- odd interval counts;
- non-positive/invalid tolerances or steps;
- invalid bounds/options;
- excessive configured workloads.

Reversed bounds return the negative of the forward integral. Equal bounds return zero.

## Expression evaluation

Each sampled `x` value is converted to CalcNova's numerical representation and passed to the parsed/compiled expression through the variables mechanism.

Expression errors, domain failures, and non-finite values are propagated as controlled analysis/sampling failures where appropriate rather than being converted into fabricated numeric data.

## Sampling and discontinuities

Graph sampling uses explicit workload limits and segments invalid/non-finite regions so a plotted path does not visually connect across a discontinuity as though the function were continuous.

Sampling limits and numerical analysis limits are separate contracts because a valid expression can otherwise be evaluated many times during plotting.

See [GRAPH_NUMERICAL_SAFETY.md](GRAPH_NUMERICAL_SAFETY.md).

## Shared UI integration

`GraphingViewModel` and the shared Graph surface expose:

- derivative at the selected analysis X value;
- root search across the selected/current X interval;
- integration across the selected/current X interval;
- explicit approximate-analysis presentation;
- user-facing validation/evaluation errors;
- nearest sampled-point trace;
- table/sample data workflows;
- bounded single- and multi-expression CSV generation;
- accessible SVG export;
- multi-series presentation and legend integration;
- viewport pan/zoom/reset/fit interaction.

These are completed 2.8.03 source capabilities rather than remaining product work.

## Multi-series behavior

Graphing can sample/present multiple expressions with stable series identities.

Series presentation includes deterministic non-color-only line patterns and a synchronized text legend so differentiation does not depend only on color.

See [GRAPH_SERIES_PRESENTATION.md](GRAPH_SERIES_PRESENTATION.md).

## Viewport and trace behavior

The graph surface includes:

- pointer drag panning;
- pointer-wheel zoom;
- double-tap/double-click fit-to-data;
- arrow-key panning;
- numpad Add/Subtract zoom;
- Home reset;
- `F` fit-to-data;
- nearest sampled-point trace.

See [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md) and [GRAPH_VIEWPORT_CONTROLS.md](GRAPH_VIEWPORT_CONTROLS.md).

## Numerical safety

Numerical-analysis and graph-sampling code must remain bounded for pathological but syntactically valid expressions.

Safety concerns include:

- maximum sample counts;
- maximum integration intervals;
- root iteration limits;
- finite step/tolerance validation;
- extreme finite input handling;
- discontinuities/domain failures;
- output/export size bounds.

Any new numerical algorithm needs an explicit work bound before it is exposed through the shared application.

## Regression expectations

Coverage should protect representative and boundary cases such as:

- polynomial derivative approximation;
- bracketed square-root solving;
- rejection of unbracketed intervals;
- polynomial integration;
- reversed/equal integration bounds;
- Simpson interval validation;
- graph view-model command output/error behavior;
- discontinuity handling;
- extreme finite values;
- configured workload limits;
- sampling/export bounds;
- multi-series identity/presentation.

Additional maintenance tests may expand coverage for trigonometric/domain-specific cases when useful, but they are not missing 2.8.03 implementation requirements.

## Validation

SDK-independent numerical/graph validators and their regression suites are included in:

```bash
python tools/release_preflight.py
```

Compiled domain/application tests run through the normal .NET test gate documented in [TESTING.md](TESTING.md).

Source/test presence does not prove a particular target/runtime scenario passed. Record runtime evidence using:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

See [RUNTIME_VALIDATION_RUNBOOK.md](RUNTIME_VALIDATION_RUNBOOK.md).

## 2.8.03 classification

- bounded derivative approximation: **COMPLETE**;
- bounded bisection root finding: **COMPLETE**;
- bounded Simpson integration: **COMPLETE**;
- graph sampling/discontinuity handling: **COMPLETE**;
- trace: **COMPLETE**;
- multi-expression workflows: **COMPLETE**;
- CSV/SVG export workflows: **COMPLETE**;
- numerical workload safeguards: **COMPLETE**.

Future numerical additions are optional enhancements or maintenance, not unfinished 2.8.03 scope.
