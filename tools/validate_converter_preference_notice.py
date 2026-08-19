#!/usr/bin/env python3
"""Validate CalcNova converter preference/privacy notice contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    key_path = root / "src" / "CalcNova.App" / "Localization" / "AppStringKey.cs"
    english_path = root / "src" / "CalcNova.App" / "Localization" / "EnglishAppStrings.cs"
    hindi_path = root / "src" / "CalcNova.App" / "Localization" / "HindiAppStrings.cs"
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.CheckBoxLocalization.cs"
    headless_path = root / "tests" / "CalcNova.App.Tests" / "ConverterPreferenceNoticeHeadlessTests.cs"

    paths = (key_path, english_path, hindi_path, view_path, headless_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing converter preference-notice source: {path}")
    if failures:
        return failures

    keys = key_path.read_text(encoding="utf-8")
    english = english_path.read_text(encoding="utf-8")
    hindi = hindi_path.read_text(encoding="utf-8")
    view = view_path.read_text(encoding="utf-8")
    headless = headless_path.read_text(encoding="utf-8")

    for marker in ("ConverterPreferencesTitle", "ConverterPreferencesBody"):
        if marker not in keys:
            failures.append(f"AppStringKey is missing converter notice key: {marker}")
        if f"AppStringKey.{marker}" not in english:
            failures.append(f"English catalog is missing converter notice key: {marker}")
        if f"AppStringKey.{marker}" not in hindi:
            failures.append(f"Hindi catalog is missing converter notice key: {marker}")

    for marker in (
        "EnsureConverterPreferenceNotice()",
        'Classes.Add("converter-preference-notice")',
        "ReferenceEquals(candidate.DataContext, converter)",
        "localizer[AppStringKey.ConverterPreferencesTitle]",
        "localizer[AppStringKey.ConverterPreferencesBody]",
    ):
        if marker not in view:
            failures.append(f"MainView converter notice wiring is missing marker: {marker}")

    for marker in (
        "ConverterMode_ShowsLocalPreferencePrivacyNotice",
        "HindiCulture_LocalizesConverterPreferencePrivacyNotice",
        '"Saved converter preferences"',
        '"सहेजी गई कन्वर्टर प्राथमिकताएँ"',
    ):
        if marker not in headless:
            failures.append(f"Converter preference notice headless coverage is missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova converter preference/privacy notice contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()
    failures = validate(Path(args.root).resolve())
    if failures:
        print("Converter preference-notice validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated converter local preference/privacy notice, localization, and headless coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
