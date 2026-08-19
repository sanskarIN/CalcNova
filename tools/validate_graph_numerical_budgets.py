#!/usr/bin/env python3
"""Validate CalcNova graph numerical-analysis and workload-budget contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    analyzer_path = root / "src" / "CalcNova.Graphing" / "GraphNumericalAnalyzer.cs"
    options_path = root / "src" / "CalcNova.Graphing" / "NumericalAnalysisOptions.cs"
    sampler_path = root / "src" / "CalcNova.Graphing" / "GraphSampler.cs"
    edge_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphNumericalEdgeCaseTests.cs"
    budget_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphWorkloadBudgetTests.cs"

    paths = (analyzer_path, options_path, sampler_path, edge_tests_path, budget_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing graph numerical-budget source: {path}")

    if failures:
        return failures

    analyzer = analyzer_path.read_text(encoding="utf-8")
    options = options_path.read_text(encoding="utf-8")
    sampler = sampler_path.read_text(encoding="utf-8")
    edge_tests = edge_tests_path.read_text(encoding="utf-8")
    budget_tests = budget_tests_path.read_text(encoding="utf-8")

    for marker in (
        "MaximumRootIterations",
        "Root search did not converge within the configured iteration limit.",
        "ValidateFinite(minimumX, nameof(minimumX))",
        "ValidateFinite(maximumX, nameof(maximumX))",
        "if (minimumX == maximumX)",
    ):
        if marker not in analyzer:
            failures.append(f"GraphNumericalAnalyzer is missing budget/edge marker: {marker}")

    for marker in (
        "MaximumRootIterations is < 1 or > 10_000",
        "MaximumIntegrationIntervals is < 2 or > 1_000_000",
        "IntegrationIntervals > MaximumIntegrationIntervals",
        "(IntegrationIntervals & 1) != 0",
    ):
        if marker not in options:
            failures.append(f"NumericalAnalysisOptions is missing workload marker: {marker}")

    for marker in (
        "public const int MaximumSamples = 10_000;",
        "options.SampleCount is < 2 or > MaximumSamples",
        "options.MaximumAbsoluteY <= 0d",
        "options.DiscontinuityJumpThreshold <= 0d",
    ):
        if marker not in sampler:
            failures.append(f"GraphSampler is missing workload marker: {marker}")

    for marker in (
        "FindRoot_ReturnsLeftEndpointWithinTolerance",
        "FindRoot_ReturnsRightEndpointWithinTolerance",
        "FindRoot_StopsAtConfiguredIterationBudget",
        "Integrate_EqualBounds_ReturnsZero",
        "Derivative_RejectsNonFiniteX",
    ):
        if marker not in edge_tests:
            failures.append(f"Graph numerical edge-case tests are missing marker: {marker}")

    for marker in (
        "GraphSampler_RejectsSampleCountAboveHardBudget",
        "NumericalOptions_RejectRootIterationBudgetOutsideSupportedRange",
        "NumericalOptions_RejectIntegrationIntervalsAboveConfiguredMaximum",
        "NumericalOptions_RejectMaximumIntegrationBudgetAboveHardCap",
        "GraphSampler_RejectsInvalidMaximumAbsoluteY",
        "GraphSampler_RejectsInvalidDiscontinuityThreshold",
    ):
        if marker not in budget_tests:
            failures.append(f"Graph workload-budget tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova graph numerical workload contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Graph numerical workload validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated graph numerical edge cases, iteration limits, sampling caps, and integration budgets.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
