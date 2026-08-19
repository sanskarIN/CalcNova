#!/usr/bin/env python3
"""Validate CalcNova localization catalog and preference contracts without .NET."""

from __future__ import annotations

import argparse
import collections
import re
import sys
from pathlib import Path


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova localization catalog.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    key_path = root / "src" / "CalcNova.App" / "Localization" / "AppStringKey.cs"
    english_path = root / "src" / "CalcNova.App" / "Localization" / "EnglishAppStrings.cs"
    localizer_path = root / "src" / "CalcNova.App" / "Localization" / "AppLocalizer.cs"
    settings_model_path = root / "src" / "CalcNova.Platform" / "Settings" / "AppSettings.cs"
    settings_view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "SettingsViewModel.cs"

    failures: list[str] = []
    for path in (key_path, english_path, localizer_path, settings_model_path, settings_view_model_path):
        if not path.is_file():
            failures.append(f"Missing localization source: {path}")

    if failures:
        for failure in failures:
            print(failure, file=sys.stderr)
        return 2

    key_source = key_path.read_text(encoding="utf-8")
    english_source = english_path.read_text(encoding="utf-8")
    localizer_source = localizer_path.read_text(encoding="utf-8")
    settings_model_source = settings_model_path.read_text(encoding="utf-8")
    settings_view_model_source = settings_view_model_path.read_text(encoding="utf-8")

    enum_match = re.search(r"public\s+enum\s+AppStringKey\s*\{(?P<body>.*?)\}", key_source, re.DOTALL)
    if enum_match is None:
        failures.append("AppStringKey enum declaration could not be parsed.")
        keys: list[str] = []
    else:
        keys = re.findall(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*,?\s*$", enum_match.group("body"), re.MULTILINE)

    catalog_keys = re.findall(r"\[AppStringKey\.([A-Za-z_][A-Za-z0-9_]*)\]\s*=", english_source)
    catalog_counts = collections.Counter(catalog_keys)

    missing = sorted(set(keys) - set(catalog_keys))
    extra = sorted(set(catalog_keys) - set(keys))
    duplicates = sorted(key for key, count in catalog_counts.items() if count > 1)

    if missing:
        failures.append(f"English catalog is missing keys: {', '.join(missing)}")
    if extra:
        failures.append(f"English catalog contains unknown keys: {', '.join(extra)}")
    if duplicates:
        failures.append(f"English catalog contains duplicate keys: {', '.join(duplicates)}")

    for marker in (
        'CultureInfo.GetCultureInfo("en")',
        "TwoLetterISOLanguageName",
        "CultureNotFoundException",
        "CultureChanged?.Invoke",
    ):
        if marker not in localizer_source:
            failures.append(f"AppLocalizer is missing culture-safety marker: {marker}")

    for marker in (
        'public string CultureName { get; init; } = "en";',
    ):
        if marker not in settings_model_source:
            failures.append(f"AppSettings is missing localization preference marker: {marker}")

    for marker in (
        "SupportedCultureNames",
        "public string CultureName",
        "_localizer.TrySetCulture(CultureName)",
        "CultureName = normalizedCultureName",
        "ApplyCulture(settings.CultureName)",
    ):
        if marker not in settings_view_model_source:
            failures.append(f"SettingsViewModel is missing localization preference marker: {marker}")

    if failures:
        print("Localization validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        f"Validated {len(keys)} semantic localization keys, the English source catalog, "
        "and persisted culture preference contracts."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
