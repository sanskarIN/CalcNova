#!/usr/bin/env python3
"""Validate CalcNova bivariate-statistics source contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    summary_path = root / "src" / "CalcNova.Statistics" / "BivariateStatisticsSummary.cs"
    calculator_path = root / "src" / "CalcNova.Statistics" / "BivariateStatisticsCalculator.cs"
    tests_path = root / "tests" / "CalcNova.Statistics.Tests" / "BivariateStatisticsCalculatorTests.cs"

    paths = (summary_path, calculator_path, tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing bivariate-statistics source: {path}")

    if failures:
        return failures

    summary = summary_path.read_text(encoding="utf-8")
    calculator = calculator_path.read_text(encoding="utf-8")
    tests = tests_path.read_text(encoding="utf-8")

    for marker in (
        "public sealed record BivariateStatisticsSummary",
        "double PopulationCovariance",
        "double? SampleCovariance",
        "double? PearsonCorrelation",
        "double? RegressionSlope",
        "double? RegressionIntercept",
        "double? RSquared",
        "public double Predict(double x)",
        "public bool HasLinearRegression",
    ):
        if marker not in summary:
            failures.append(f"BivariateStatisticsSummary is missing contract marker: {marker}")

    for marker in (
        "public const int MaximumPairCount = 100_000;",
        "using var xEnumerator = xValues.GetEnumerator();",
        "using var yEnumerator = yValues.GetEnumerator();",
        "if (hasX != hasY)",
        "if (count > MaximumPairCount)",
        "meanX += deltaX / count;",
        "meanY += deltaY / count;",
        "coMoment += deltaX * adjustedY;",
        "correlation = coMoment / Math.Sqrt(sumSquaredX) / Math.Sqrt(sumSquaredY);",
        "slope = coMoment / sumSquaredX;",
        "EnsureFiniteState",
    ):
        if marker not in calculator:
            failures.append(f"BivariateStatisticsCalculator is missing contract marker: {marker}")

    if "ToArray()" in calculator:
        failures.append("BivariateStatisticsCalculator must preserve bounded streaming pair enumeration instead of materializing input with ToArray().")

    for marker in (
        "Analyze_PerfectPositiveRelationship_ComputesRegressionAndCorrelation",
        "Analyze_PerfectNegativeRelationship_ReportsNegativeCorrelation",
        "Analyze_ConstantY_ProducesZeroSlopeButUndefinedCorrelation",
        "Analyze_ConstantX_MakesLinearRegressionUndefined",
        "Analyze_SinglePair_HasPopulationCovarianceButNoSampleCovariance",
        "Analyze_RejectsUnequalDatasetLengths",
        "Analyze_RejectsEmptyDatasets",
        "Analyze_RejectsNonFiniteValues",
        "Analyze_RejectsPairsAboveHardWorkloadBudget",
        "Predict_RejectsNonFiniteInput",
    ):
        if marker not in tests:
            failures.append(f"Bivariate statistics tests are missing scenario: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova bivariate-statistics contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Bivariate statistics validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated bounded streaming covariance, correlation, regression, prediction, and regression coverage contracts.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
