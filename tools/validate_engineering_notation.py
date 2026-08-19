#!/usr/bin/env python3
"""Validate CalcNova engineering-notation source contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    formatter_path = root / "src" / "CalcNova.Core" / "Numerics" / "EngineeringNotationFormatter.cs"
    core_tests_path = root / "tests" / "CalcNova.Core.Tests" / "EngineeringNotationFormatterTests.cs"
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "EngineeringNotationViewModel.cs"
    panel_path = root / "src" / "CalcNova.App" / "Controls" / "EngineeringNotationPanel.cs"
    app_tests_path = root / "tests" / "CalcNova.App.Tests" / "EngineeringNotationViewModelTests.cs"
    panel_tests_path = root / "tests" / "CalcNova.App.Tests" / "EngineeringNotationPanelHeadlessTests.cs"

    paths = (
        formatter_path,
        core_tests_path,
        view_model_path,
        panel_path,
        app_tests_path,
        panel_tests_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing engineering-notation source: {path}")

    if failures:
        return failures

    formatter = formatter_path.read_text(encoding="utf-8")
    core_tests = core_tests_path.read_text(encoding="utf-8")
    view_model = view_model_path.read_text(encoding="utf-8")
    panel = panel_path.read_text(encoding="utf-8")
    app_tests = app_tests_path.read_text(encoding="utf-8")
    panel_tests = panel_tests_path.read_text(encoding="utf-8")

    for marker in (
        "public static class EngineeringNotationFormatter",
        "public const int MaximumInputCharacters = 4_096;",
        "public const int MinimumSignificantDigits = 1;",
        "public const int MaximumSignificantDigits = 15;",
        "public const int MinimumEngineeringExponent = -324;",
        "public const int MaximumEngineeringExponent = 306;",
        "public static string Format(double value, int significantDigits = 12)",
        "public static double Parse(string? text)",
        "if (text.Length > MaximumInputCharacters)",
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
        "Parse_RejectsInputAboveCharacterBudget",
        "Parse_RejectsOversizedWhitespaceBeforeScanningForBlankInput",
        "FormatAndParse_RoundTripRepresentativeFiniteValues",
        "double.Epsilon",
        "double.MaxValue",
    ):
        if marker not in core_tests:
            failures.append(f"Engineering notation core tests are missing scenario: {marker}")

    for marker in (
        "if (InputText.Length > EngineeringNotationFormatter.MaximumInputCharacters)",
        "EngineeringNotationFormatter.Parse(InputText)",
        "exception is ArgumentException or FormatException or OverflowException",
    ):
        if marker not in view_model:
            failures.append(f"Engineering notation view model is missing contract marker: {marker}")

    for marker in (
        "MaxLength = EngineeringNotationFormatter.MaximumInputCharacters",
        "nameof(EngineeringNotationViewModel.FormatCommand)",
        "nameof(EngineeringNotationViewModel.ParseCommand)",
    ):
        if marker not in panel:
            failures.append(f"Engineering notation panel is missing contract marker: {marker}")

    for marker in (
        "FormatCommand_RejectsInputAboveCharacterBudget",
        "EngineeringNotationFormatter.MaximumInputCharacters + 1",
    ):
        if marker not in app_tests:
            failures.append(f"Engineering notation app tests are missing scenario: {marker}")

    for marker in (
        "Panel_BoundsInteractiveInputToFormatterCharacterBudget",
        "Assert.Equal(EngineeringNotationFormatter.MaximumInputCharacters, input.MaxLength)",
    ):
        if marker not in panel_tests:
            failures.append(f"Engineering notation panel tests are missing scenario: {marker}")

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

    print(
        "Validated engineering formatting/parsing, text/exponent/underflow budgets, significant digits, and shared UI input bounds."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
