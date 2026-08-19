#!/usr/bin/env python3
"""Validate CalcNova engineering-notation source contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    formatter_path = root / "src" / "CalcNova.Core" / "Numerics" / "EngineeringNotationFormatter.cs"
    tests_path = root / "tests" / "CalcNova.Core.Tests" / "EngineeringNotationFormatterTests.cs"

    paths = (formatter_path, tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing engineering-notation source: {path}")

    if failures:
        return failures

    formatter = formatter_path.read_text(encoding="utf-8")
    tests = tests_path.read_text(encoding="utf-8")

    for marker in (
        "public static class EngineeringNotationFormatter",
        "public const int MinimumSignificantDigits = 1;",
        "public const int MaximumSignificantDigits = 15;",
        "public const int MinimumEngineeringExponent = -324;",
        "public const int MaximumEngineeringExponent = 306;",
        "public static string Format(double value, int significantDigits = 12)",
        "public static double Parse(string? text)",
        "if (exponent % 3 != 0)",
        "if (exponent is < MinimumEngineeringExponent or > MaximumEngineeringExponent)",
        "if (mantissa != 0d && value == 0d)",
        "Math.Floor(decimalExponent / 3d) * 3d",
        "private static double ScaleByPowerOfTen",
        "var step = Math.Clamp(remaining, -300, 300);",
        "if (!double.IsFinite(value))",
    ):
        if marker not in formatter:
            failures.append(f"EngineeringNotationFormatter is missing contract marker: {marker}")

    for marker in (
        "Format_UsesExponentMultiplesOfThree",
        "Format_RoundingAcrossThousandBoundary_AdvancesExponent",
        "Format_RejectsUnsupportedSignificantDigits",
        "Format_RejectsNonFiniteValues",
        "Parse_AcceptsCanonicalEngineeringNotation",
        "Parse_RejectsInvalidEngineeringNotation",
        "Parse_RejectsExponentOutsideFiniteEngineeringRange",
        "Parse_RejectsUnderflowingNonZeroEngineeringValue",
        "FormatAndParse_RoundTripRepresentativeFiniteValues",
        "double.Epsilon",
        "double.MaxValue",
    ):
        if marker not in tests:
            failures.append(f"Engineering notation tests are missing scenario: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova engineering-notation contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Engineering notation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated finite engineering formatting/parsing, exponent/underflow bounds, significant digits, and edge-case coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
