#!/usr/bin/env python3
"""Validate CalcNova per-category converter default-pair contracts without .NET."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

EXPECTED_CATEGORIES = (
    "Length",
    "Area",
    "Volume",
    "Mass",
    "Speed",
    "Temperature",
    "Time",
    "Data",
    "Frequency",
    "Pressure",
    "Energy",
    "Power",
    "Force",
    "Angle",
)


def validate(root: Path) -> list[str]:
    defaults_path = root / "src" / "CalcNova.Converter" / "ConversionDefaults.cs"
    catalog_path = root / "src" / "CalcNova.Converter" / "UnitCatalog.cs"
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "ConverterViewModel.cs"
    domain_tests_path = root / "tests" / "CalcNova.Converter.Tests" / "ConversionDefaultsTests.cs"
    app_tests_path = root / "tests" / "CalcNova.App.Tests" / "ConverterDefaultPairViewModelTests.cs"

    paths = (defaults_path, catalog_path, view_model_path, domain_tests_path, app_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing converter-default source: {path}")

    if failures:
        return failures

    defaults = defaults_path.read_text(encoding="utf-8")
    catalog = catalog_path.read_text(encoding="utf-8")
    view_model = view_model_path.read_text(encoding="utf-8")
    domain_tests = domain_tests_path.read_text(encoding="utf-8")
    app_tests = app_tests_path.read_text(encoding="utf-8")

    category_entries = re.findall(r"\[UnitCategory\.([A-Za-z_][A-Za-z0-9_]*)\]\s*=\s*new\(\"([^\"]+)\",\s*\"([^\"]+)\"\)", defaults)
    category_names = [name for name, _, _ in category_entries]
    if tuple(category_names) != EXPECTED_CATEGORIES:
        failures.append(
            "ConversionDefaults must define exactly one deterministic pair for every UnitCategory in enum order."
        )

    known_unit_ids = set(re.findall(r'new\("([^\"]+)",\s*"', catalog))
    for category, from_id, to_id in category_entries:
        if from_id == to_id:
            failures.append(f"Default pair for {category} must use two distinct units.")
        for unit_id in (from_id, to_id):
            if unit_id not in known_unit_ids:
                failures.append(f"Default pair for {category} references unknown unit id: {unit_id}")

    for marker in (
        "ConversionDefaults.ForCategory(_selectedCategory)",
        "ApplyDefaultPair(value)",
        "private void ApplyDefaultPair(UnitCategory category)",
        "ConversionDefaults.ForCategory(category)",
    ):
        if marker not in view_model:
            failures.append(f"ConverterViewModel is missing default-pair integration marker: {marker}")

    for marker in (
        "EveryUnitCategory_HasAValidDefaultPair",
        "RepresentativeCategories_UseExpectedDefaults",
        "UnknownCategory_IsRejected",
    ):
        if marker not in domain_tests:
            failures.append(f"ConversionDefaultsTests is missing regression test: {marker}")

    for marker in (
        "InitialCategory_UsesLengthDefaults",
        "ChangingCategory_AppliesDeterministicDefaults",
        "RestoringExplicitPair_StillOverridesCategoryDefaults",
    ):
        if marker not in app_tests:
            failures.append(f"ConverterDefaultPairViewModelTests is missing integration test: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova converter category default pairs.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Converter default-pair validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated deterministic converter defaults for {len(EXPECTED_CATEGORIES)} categories.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
