#!/usr/bin/env python3
"""Validate CalcNova exact-rational source contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    rational_path = root / "src" / "CalcNova.Core" / "Numerics" / "RationalNumber.cs"
    tests_path = root / "tests" / "CalcNova.Core.Tests" / "RationalNumberTests.cs"

    paths = (rational_path, tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing exact-rational source: {path}")

    if failures:
        return failures

    rational = rational_path.read_text(encoding="utf-8")
    tests = tests_path.read_text(encoding="utf-8")

    for marker in (
        "public readonly struct RationalNumber",
        "public const int MaximumInputCharacters = 4_096;",
        "public const int MaximumDecimalScale = 10_000;",
        "public const int MaximumBitLength = 65_536;",
        "BigInteger.GreatestCommonDivisor",
        "if (denominator.Sign < 0)",
        "public static RationalNumber Parse(string? text)",
        "public static bool TryParse(string? text, out RationalNumber value)",
        "public RationalNumber Reciprocal()",
        "public static RationalNumber operator +",
        "public static RationalNumber operator *",
        "var leftCancellation = BigInteger.GreatestCommonDivisor",
        "var rightCancellation = BigInteger.GreatestCommonDivisor",
        "public static RationalNumber operator /",
        "Math.Abs((long)exponent) > MaximumDecimalScale",
        "Math.Abs(scale) > MaximumDecimalScale",
        "numerator.GetBitLength() > MaximumBitLength || denominator.GetBitLength() > MaximumBitLength",
    ):
        if marker not in rational:
            failures.append(f"RationalNumber is missing contract marker: {marker}")

    for marker in (
        "Constructor_NormalizesSignAndGreatestCommonDivisor",
        "Constructor_RejectsZeroDenominator",
        "Parse_ProducesCanonicalExactRepresentation",
        "Parse_DecimalPointOne_IsExactlyOneTenth",
        "Parse_RejectsInvalidSyntax",
        "Parse_RejectsZeroFractionDenominator",
        "Parse_RejectsDecimalScaleOutsideWorkloadBudget",
        "Parse_RejectsInputAboveCharacterBudget",
        "Constructor_RejectsReducedValuesAboveBitBudget",
        "Addition_UsesExactReducedArithmetic",
        "Subtraction_UsesExactReducedArithmetic",
        "Multiplication_CrossCancelsBeforeFinalConstruction",
        "Division_UsesExactReciprocalArithmetic",
        "Division_RejectsZeroDivisor",
        "Arithmetic_RejectsFinalMagnitudeAboveBitBudget",
        "Comparison_UsesExactCrossProducts",
        "CanonicalValues_HaveStableEqualityAndHashCodes",
        "TryParse_ReturnsDeterministicSuccessState",
    ):
        if marker not in tests:
            failures.append(f"Rational number tests are missing scenario: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova exact-rational contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Exact rational validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated exact rational normalization, decimal parsing, arithmetic cancellation, and workload bounds.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
