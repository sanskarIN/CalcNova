#!/usr/bin/env python3
"""Validate CalcNova numerical-analysis safety contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    analyzer_path = root / "src" / "CalcNova.Graphing" / "GraphNumericalAnalyzer.cs"
    options_path = root / "src" / "CalcNova.Graphing" / "NumericalAnalysisOptions.cs"
    baseline_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphNumericalAnalyzerTests.cs"
    extreme_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphNumericalExtremeTests.cs"
    option_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "NumericalAnalysisOptionsTests.cs"

    paths = (analyzer_path, options_path, baseline_tests_path, extreme_tests_path, option_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing numerical-analysis source: {path}")

    if failures:
        return failures

    analyzer = analyzer_path.read_text(encoding="utf-8")
    options = options_path.read_text(encoding="utf-8")
    baseline_tests = baseline_tests_path.read_text(encoding="utf-8")
    extreme_tests = extreme_tests_path.read_text(encoding="utf-8")
    option_tests = option_tests_path.read_text(encoding="utf-8")

    for marker in (
        "if (!double.IsFinite(leftX) || !double.IsFinite(rightX))",
        "if (leftX == x || rightX == x || leftX == rightX)",
        "var middle = SafeMidpoint(left, right);",
        "return Math.Abs(leftValue) <= Math.Abs(rightValue) ? left : right;",
        "var midpoint = (left / 2d) + (right / 2d);",
        "var width = (maximumX / intervals) - (minimumX / intervals);",
        "var fraction = (double)index / intervals;",
        "var x = (minimumX * (1d - fraction)) + (maximumX * fraction);",
        "RequireFinite((width / 3d) * sum",
    ):
        if marker not in analyzer:
            failures.append(f"GraphNumericalAnalyzer is missing numeric-safety marker: {marker}")

    for marker in (
        "DerivativeStep",
        "RootTolerance",
        "MaximumRootIterations is < 1 or > 10_000",
        "MaximumIntegrationIntervals is < 2 or > 1_000_000",
        "IntegrationIntervals < 2",
        "(IntegrationIntervals & 1) != 0",
    ):
        if marker not in options:
            failures.append(f"NumericalAnalysisOptions is missing workload-bound marker: {marker}")

    for marker in (
        "Derivative_ApproximatesPolynomialSlope",
        "FindRoot_FindsBracketedPolynomialRoot",
        "FindRoot_RejectsIntervalWithoutSignChange",
        "Integrate_ApproximatesPolynomialArea",
        "Integrate_ReversedBoundsNegateResult",
    ):
        if marker not in baseline_tests:
            failures.append(f"GraphNumericalAnalyzerTests is missing baseline regression: {marker}")

    for marker in (
        "Derivative_HugeXWithDefaultTinyStep_IsRejected",
        "Derivative_SamplePointOverflow_IsRejected",
        "FindRoot_ExtremeSymmetricBounds_UsesOverflowSafeMidpoint",
        "FindRoot_EndpointRoot_ReturnsEndpointImmediately",
        "FindRoot_DiscontinuityAtMidpoint_IsRejected",
        "Integrate_ExtremeSymmetricBounds_ZeroFunctionAvoidsIntermediateOverflow",
        "Integrate_DiscontinuityAtSamplePoint_IsRejected",
    ):
        if marker not in extreme_tests:
            failures.append(f"GraphNumericalExtremeTests is missing edge regression: {marker}")

    for marker in (
        "DerivativeStep_MustBeFiniteAndPositive",
        "RootTolerance_MustBeFiniteAndPositive",
        "RootIterationLimit_IsBounded",
        "MaximumIntegrationIntervals_IsBounded",
        "IntegrationIntervals_MustBeEvenAndWithinConfiguredMaximum",
        "IntegrationIntervals_CannotExceedConfiguredMaximum",
        "BoundaryConfiguration_IsAccepted",
    ):
        if marker not in option_tests:
            failures.append(f"NumericalAnalysisOptionsTests is missing boundary regression: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova numerical-analysis safety contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Numerical-analysis validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated numerical derivative/root/integration safety, extreme-bound regressions, and workload limits.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
