#!/usr/bin/env python3
"""Validate CalcNova localization catalog and preference contracts without .NET."""

from __future__ import annotations

import argparse
import collections
import re
import sys
from pathlib import Path

CATALOG_FILES = {
    "English": "EnglishAppStrings.cs",
    "Hindi": "HindiAppStrings.cs",
}


def parse_catalog_keys(source: str) -> list[str]:
    return re.findall(r"\[AppStringKey\.([A-Za-z_][A-Za-z0-9_]*)\]\s*=", source)


def validate_catalog(name: str, keys: list[str], source: str) -> list[str]:
    catalog_keys = parse_catalog_keys(source)
    counts = collections.Counter(catalog_keys)
    failures: list[str] = []

    missing = sorted(set(keys) - set(catalog_keys))
    extra = sorted(set(catalog_keys) - set(keys))
    duplicates = sorted(key for key, count in counts.items() if count > 1)

    if missing:
        failures.append(f"{name} catalog is missing keys: {', '.join(missing)}")
    if extra:
        failures.append(f"{name} catalog contains unknown keys: {', '.join(extra)}")
    if duplicates:
        failures.append(f"{name} catalog contains duplicate keys: {', '.join(duplicates)}")

    return failures


def validate(root: Path) -> list[str]:
    key_path = root / "src" / "CalcNova.App" / "Localization" / "AppStringKey.cs"
    localizer_path = root / "src" / "CalcNova.App" / "Localization" / "AppLocalizer.cs"
    settings_model_path = root / "src" / "CalcNova.Platform" / "Settings" / "AppSettings.cs"
    settings_view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "SettingsViewModel.cs"
    catalog_paths = {
        name: root / "src" / "CalcNova.App" / "Localization" / filename
        for name, filename in CATALOG_FILES.items()
    }

    failures: list[str] = []
    required_paths = [key_path, localizer_path, settings_model_path, settings_view_model_path, *catalog_paths.values()]
    for path in required_paths:
        if not path.is_file():
            failures.append(f"Missing localization source: {path}")

    if failures:
        return failures

    key_source = key_path.read_text(encoding="utf-8")
    localizer_source = localizer_path.read_text(encoding="utf-8")
    settings_model_source = settings_model_path.read_text(encoding="utf-8")
    settings_view_model_source = settings_view_model_path.read_text(encoding="utf-8")

    enum_match = re.search(r"public\s+enum\s+AppStringKey\s*\{(?P<body>.*?)\}", key_source, re.DOTALL)
    if enum_match is None:
        failures.append("AppStringKey enum declaration could not be parsed.")
        keys: list[str] = []
    else:
        keys = re.findall(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*,?\s*$", enum_match.group("body"), re.MULTILINE)

    for name, path in catalog_paths.items():
        failures.extend(validate_catalog(name, keys, path.read_text(encoding="utf-8")))

    for marker in (
        'CultureInfo.GetCultureInfo("en")',
        'CultureInfo.GetCultureInfo("hi")',
        "TwoLetterISOLanguageName",
        "CultureNotFoundException",
        "CultureChanged?.Invoke",
        'ValidateCatalog("English", EnglishAppStrings.Values)',
        'ValidateCatalog("Hindi", HindiAppStrings.Values)',
    ):
        if marker not in localizer_source:
            failures.append(f"AppLocalizer is missing culture-safety marker: {marker}")

    if 'public string CultureName { get; init; } = "en";' not in settings_model_source:
        failures.append('AppSettings is missing the default localization preference marker.')

    for marker in (
        "SupportedCultureNames",
        "public string CultureName",
        "_localizer.TrySetCulture(CultureName)",
        "CultureName = normalizedCultureName",
        "ApplyCulture(settings.CultureName)",
    ):
        if marker not in settings_view_model_source:
            failures.append(f"SettingsViewModel is missing localization preference marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova localization catalog.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)

    if failures:
        print("Localization validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    key_source = (root / "src" / "CalcNova.App" / "Localization" / "AppStringKey.cs").read_text(encoding="utf-8")
    enum_match = re.search(r"public\s+enum\s+AppStringKey\s*\{(?P<body>.*?)\}", key_source, re.DOTALL)
    key_count = 0 if enum_match is None else len(re.findall(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*,?\s*$", enum_match.group("body"), re.MULTILINE))
    print(
        f"Validated {key_count} semantic localization keys across {len(CATALOG_FILES)} catalogs "
        "and persisted culture preference contracts."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
