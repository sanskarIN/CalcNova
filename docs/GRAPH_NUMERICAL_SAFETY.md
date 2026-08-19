# Graph Numerical Safety and Workload Bounds

CalcNova graph analysis is intentionally approximate, bounded, and local. Derivatives, roots, and definite integrals are numerical tools rather than symbolic-math claims.

## Implemented methods

- First derivative: bounded central difference.
- Root finding: bracketed bisection.
- Definite integration: bounded composite Simpson integration.
- Graph rendering data: bounded expression sampling with discontinuity segmentation.

These methods reuse CalcNova's project-owned expression evaluator instead of executing arbitrary source code.

## Numerical-analysis options

`NumericalAnalysisOptions` defines explicit safety/workload controls:

- finite positive derivative step;
- finite positive root tolerance;
- root iterations from 1 through 10,000;
- integration interval count that is even and at least 2;
- configurable integration maximum;
- hard maximum integration budget of 1,000,000 intervals.

Invalid options fail before the requested numerical operation runs.

## Graph sampling budget

`GraphSampler.MaximumSamples` is 10,000. A graph request outside the supported sampling range is rejected before expression sampling begins.

Sampling options also require:

- finite ordered X bounds;
- finite positive maximum absolute Y;
- finite positive discontinuity-jump threshold.

These guards keep rendering work bounded and prevent non-finite configuration from silently entering graph data.

## Extreme finite bounds

Finite `double` values can still overflow intermediate calculations. Numerical routines therefore avoid unsafe arithmetic where practical.

Current protections include:

- rejecting derivative sample points that overflow or collapse onto the requested X value;
- overflow-safe root midpoint calculation;
- finite checks on evaluated expression values;
- interpolation-based Simpson sample points rather than an overflow-prone `minimum + width * index` form;
- finite integration-width and final-result checks;
- deterministic failure when a root cannot converge within the configured iteration budget.

## Root behavior

Root finding requires an ordered finite interval. If either endpoint is already within tolerance, that endpoint is returned immediately. Otherwise the interval must bracket a sign change.

Bisection stops when:

- the sampled midpoint is within root tolerance;
- the interval becomes sufficiently small;
- floating-point resolution makes the midpoint equal to an endpoint.

If the configured iteration budget is exhausted first, CalcNova reports a deterministic failure instead of continuing indefinitely.

## Integration behavior

Equal bounds return zero. Reversed bounds are normalized by evaluating the forward interval and negating the result.

Composite Simpson integration requires an even interval count. Every sampled expression value and the final integral must remain finite.

## Regression coverage

The graph numerical test suite includes baseline, extreme-bound, edge-case, and workload-budget coverage:

- `tests/CalcNova.Graphing.Tests/GraphNumericalAnalyzerTests.cs`;
- `tests/CalcNova.Graphing.Tests/GraphNumericalExtremeTests.cs`;
- `tests/CalcNova.Graphing.Tests/NumericalAnalysisOptionsTests.cs`;
- `tests/CalcNova.Graphing.Tests/GraphNumericalEdgeCaseTests.cs`;
- `tests/CalcNova.Graphing.Tests/GraphWorkloadBudgetTests.cs`.

Coverage includes endpoint roots, non-finite arguments, extreme finite ranges, discontinuities, iteration exhaustion, equal/reversed integration bounds, sample-count caps, root-iteration caps, and integration-budget limits.

## SDK-independent validation

Numerical-analysis source contracts are protected by two complementary validators:

```bash
python tools/validate_numerical_analysis.py .
python -m unittest tools.tests.test_validate_numerical_analysis

python tools/validate_graph_numerical_budgets.py .
python -m unittest tools.tests.test_validate_graph_numerical_budgets
```

The first protects extreme-value numerical-safety behavior. The second protects graph sampling and workload-budget regressions. Both are represented in focused GitHub Actions workflows and the integrated release-source preflight.

## Evidence policy

Source presence is not a compiled-test PASS. The repository still requires observed .NET 10 build/test results and real graph interaction validation on supported targets before release readiness can be claimed.

When a target toolchain is unavailable, record the result as **NOT RUN**, not PASS.
