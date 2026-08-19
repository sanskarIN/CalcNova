#!/usr/bin/env python3
"""Reject obvious shared-XAML touch-target regressions without requiring Avalonia runtime."""

from __future__ import annotations

from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
APP_XAML = ROOT / "src" / "CalcNova.App" / "App.axaml"
VIEWS = ROOT / "src" / "CalcNova.App" / "Views"
MINIMUM_TARGET = 44.0

MIN_HEIGHT_PATTERN = re.compile(r'MinHeight="(?P<value>\d+(?:\.\d+)?)"')


def main() -> int:
    errors: list[str] = []

    if not APP_XAML.is_file():
        errors.append("missing src/CalcNova.App/App.axaml")
    if not VIEWS.is_dir():
        errors.append("missing src/CalcNova.App/Views")

    if errors:
        print("Touch-target validation failed:")
        print("\n".join(f"- {error}" for error in errors))
        return 1

    app_xaml = APP_XAML.read_text(encoding="utf-8")
    required_baselines = (
        'Style Selector="Button"',
        'Style Selector="TextBox"',
        'Style Selector="ComboBox"',
        'Style Selector="CheckBox"',
        'Style Selector="TabItem"',
        'Style Selector="ListBoxItem"',
    )
    for selector in required_baselines:
        if selector not in app_xaml:
            errors.append(f"App.axaml: missing interactive baseline {selector!r}")

    if app_xaml.count('Property="MinHeight" Value="44"') < len(required_baselines):
        errors.append("App.axaml: expected 44-DIP minimum-height baseline for all shared interactive control types")

    for xaml_file in sorted(VIEWS.rglob("*.axaml")):
        text = xaml_file.read_text(encoding="utf-8")
        for match in MIN_HEIGHT_PATTERN.finditer(text):
            value = float(match.group("value"))
            if value < MINIMUM_TARGET:
                line = text.count("\n", 0, match.start()) + 1
                errors.append(
                    f"{xaml_file.relative_to(ROOT)}:{line}: explicit MinHeight {value:g} is below {MINIMUM_TARGET:g}"
                )

    if errors:
        print("Touch-target validation failed:")
        print("\n".join(f"- {error}" for error in errors))
        return 1

    print("Shared touch-target source contracts look consistent.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
