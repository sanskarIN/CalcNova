#!/usr/bin/env python3
"""Validate critical shared-XAML command/property contracts without .NET.

This is a lightweight source preflight for the shared CalcNova shell. It does
not understand Avalonia binding semantics and does not replace compilation.
It intentionally checks only explicit high-value contracts that are easy to
verify from source text and that have changed frequently during development.
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


CONTRACTS: dict[str, dict[str, tuple[str, ...]]] = {
    "CalculatorViewModel": {
        "commands": (
            "PasteCommand",
            "CopyResultCommand",
            "EvaluateCommand",
            "PercentageCommand",
            "MemoryClearCommand",
            "MemoryRecallCommand",
            "MemoryStoreCommand",
            "MemoryAddCommand",
            "MemorySubtractCommand",
        ),
        "properties": ("Expression", "Result", "StatusMessage", "AngleModeLabel"),
    },
    "ProgrammerViewModel": {
        "commands": (
            "ConvertCommand",
            "AndCommand",
            "OrCommand",
            "XorCommand",
            "NotCommand",
            "ShiftLeftCommand",
            "LogicalShiftRightCommand",
            "ArithmeticShiftRightCommand",
            "CopyRepresentationCommand",
        ),
        "properties": (
            "Input",
            "Operand",
            "InputBase",
            "SupportedBases",
            "WordSize",
            "WordSizes",
            "BitGroups",
            "BitPattern",
            "Binary",
            "Octal",
            "Decimal",
            "Hexadecimal",
        ),
    },
    "CodePointViewModel": {
        "commands": (
            "DecodeCodePointCommand",
            "InspectTextCommand",
            "CopyCodePointResultCommand",
            "CopyTextResultCommand",
        ),
        "properties": ("CodePointInput", "CodePointResult", "TextInput", "TextResult"),
    },
    "ConverterViewModel": {
        "commands": (
            "ConvertCommand",
            "SwapCommand",
            "ToggleFavoriteCommand",
            "CopyResultCommand",
            "UseSearchAsFromCommand",
            "UseSearchAsToCommand",
            "ClearRecentCommand",
        ),
        "properties": (
            "Categories",
            "SelectedCategory",
            "UnitSearchQuery",
            "SearchResults",
            "SelectedSearchUnit",
            "AvailableUnits",
            "FromUnit",
            "ToUnit",
            "SignificantDigits",
            "PrecisionOptions",
            "RecentPairs",
            "FavoritePairs",
            "Result",
        ),
    },
    "GraphingViewModel": {
        "commands": (
            "PlotCommand",
            "CopyPreviewCommand",
            "CopyTableCommand",
            "DerivativeCommand",
            "FindRootCommand",
            "IntegrateCommand",
            "CopyAnalysisResultCommand",
        ),
        "properties": (
            "Expression",
            "MinimumX",
            "MaximumX",
            "SampleCount",
            "Summary",
            "Preview",
            "TableCsv",
            "AnalysisX",
            "AnalysisResult",
        ),
    },
}

XAML_CONTEXT_NAMES = {
    "CalculatorViewModel": "Calculator",
    "ProgrammerViewModel": "Programmer",
    "CodePointViewModel": "CodePoint",
    "ConverterViewModel": "Converter",
    "GraphingViewModel": "Graphing",
}


def public_member_exists(source: str, member: str) -> bool:
    pattern = re.compile(rf"\bpublic\s+[^\n;{{}}]+\b{re.escape(member)}\b")
    return pattern.search(source) is not None


def xaml_binding_exists(xaml: str, member: str) -> bool:
    return re.search(rf"\{{Binding\s+{re.escape(member)}(?:\s*[,}}])", xaml) is not None


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate critical CalcNova shared UI contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    xaml_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml"
    if not xaml_path.is_file():
        print(f"Missing shared XAML: {xaml_path}", file=sys.stderr)
        return 2

    xaml = xaml_path.read_text(encoding="utf-8")
    failures: list[str] = []

    for view_model, groups in CONTRACTS.items():
        source_path = root / "src" / "CalcNova.App" / "ViewModels" / f"{view_model}.cs"
        if not source_path.is_file():
            failures.append(f"Missing view model source: {source_path}")
            continue

        source = source_path.read_text(encoding="utf-8")
        context_name = XAML_CONTEXT_NAMES[view_model]
        if f'DataContext="{{Binding {context_name}}}"' not in xaml:
            failures.append(f"MainView.axaml does not expose DataContext binding '{context_name}'.")

        for kind, members in groups.items():
            for member in members:
                if not public_member_exists(source, member):
                    failures.append(f"{view_model} is missing public {kind[:-1]} '{member}'.")
                if not xaml_binding_exists(xaml, member):
                    failures.append(f"MainView.axaml is missing binding '{member}' for {view_model}.")

    if failures:
        print("Shared UI contract validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    contract_count = sum(len(values) for groups in CONTRACTS.values() for values in groups.values())
    print(f"Validated {contract_count} critical shared UI command/property contracts.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
