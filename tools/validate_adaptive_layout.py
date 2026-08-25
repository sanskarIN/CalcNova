#!/usr/bin/env python3
"""Validate CalcNova's source-level adaptive layout contracts without the .NET SDK."""

from __future__ import annotations

from pathlib import Path
import sys

ROOT = Path(__file__).resolve().parents[1]
APP_XAML = ROOT / "src" / "CalcNova.App" / "App.axaml"
MAIN_VIEW = ROOT / "src" / "CalcNova.App" / "Views" / "MainView.axaml"
MAIN_VIEW_CODE = ROOT / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
PROFILE = ROOT / "src" / "CalcNova.App" / "Infrastructure" / "AdaptiveLayoutProfile.cs"

REQUIRED_STYLE_CLASSES = ("compact", "medium", "expanded")
REQUIRED_MODE_HEADERS = (
    "Calc",
    "Prog",
    "Code",
    "Convert",
    "Stats",
    "Eq",
    "Matrix",
    "Graph",
    "Date",
    "FX",
    "History",
    "Settings",
    "About",
)


def require(text: str, needle: str, source: Path, errors: list[str]) -> None:
    if needle not in text:
        errors.append(f"{source.relative_to(ROOT)}: missing {needle!r}")


def main() -> int:
    errors: list[str] = []
    sources = [APP_XAML, MAIN_VIEW, MAIN_VIEW_CODE, PROFILE]
    for source in sources:
        if not source.is_file():
            errors.append(f"missing required file: {source.relative_to(ROOT)}")

    if errors:
        print("Adaptive layout validation failed:")
        print("\n".join(f"- {error}" for error in errors))
        return 1

    app_xaml = APP_XAML.read_text(encoding="utf-8")
    main_view = MAIN_VIEW.read_text(encoding="utf-8")
    main_view_code = MAIN_VIEW_CODE.read_text(encoding="utf-8")
    profile = PROFILE.read_text(encoding="utf-8")

    for style_class in REQUIRED_STYLE_CLASSES:
        require(app_xaml, f"UserControl.{style_class}", APP_XAML, errors)
        require(main_view_code, f'"{style_class}"', MAIN_VIEW_CODE, errors)

    require(main_view_code, "OnSizeChanged", MAIN_VIEW_CODE, errors)
    require(main_view_code, "ApplyAdaptiveLayout", MAIN_VIEW_CODE, errors)
    require(main_view_code, "BringIntoViewOnFocusChange = true", MAIN_VIEW_CODE, errors)
    require(main_view_code, "ScrollBarVisibility.Auto", MAIN_VIEW_CODE, errors)

    require(profile, "CompactMaximumWidth", PROFILE, errors)
    require(profile, "MediumMaximumWidth", PROFILE, errors)
    require(profile, "double.IsFinite(width)", PROFILE, errors)
    require(profile, "AllowHorizontalModeScrolling", PROFILE, errors)

    require(app_xaml, 'Selector="Button"', APP_XAML, errors)
    require(app_xaml, 'Property="MinHeight" Value="44"', APP_XAML, errors)
    require(app_xaml, 'Selector="TabItem"', APP_XAML, errors)

    require(main_view, 'VerticalScrollBarVisibility="Auto"', MAIN_VIEW, errors)
    for header in REQUIRED_MODE_HEADERS:
        require(main_view, f'Header="{header}"', MAIN_VIEW, errors)

    if errors:
        print("Adaptive layout validation failed:")
        print("\n".join(f"- {error}" for error in errors))
        return 1

    print("Adaptive layout contracts look consistent.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
