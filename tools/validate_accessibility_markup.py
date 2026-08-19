#!/usr/bin/env python3
"""Run lightweight accessibility-oriented checks against shared Avalonia markup.

This validator intentionally checks only deterministic source-level rules. It
cannot replace screen-reader, keyboard, contrast, scaling, or platform tests.
"""

from __future__ import annotations

import argparse
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


SYMBOL_BUTTONS_REQUIRING_NAMES = {
    "AC",
    "⌫",
    "÷",
    "×",
    "−",
    "+",
    "=",
    ".",
    "√",
    "x²",
    "xʸ",
    "π",
    "e",
    "(",
    ")",
    "%",
    "1/x",
    "fact",
    "MC",
    "MR",
    "MS",
    "M+",
    "M−",
    "AND",
    "OR",
    "XOR",
    "NOT",
}


def local_name(tag: str) -> str:
    return tag.rsplit("}", 1)[-1]


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate source-level accessibility markup rules.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    main_view = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml"
    app_xaml = root / "src" / "CalcNova.App" / "App.axaml"
    failures: list[str] = []

    try:
        tree = ET.parse(main_view)
    except (ET.ParseError, OSError) as exc:
        print(f"Unable to parse {main_view}: {exc}", file=sys.stderr)
        return 2

    checked = 0
    for element in tree.iter():
        if local_name(element.tag) != "Button":
            continue
        content = element.attrib.get("Content")
        if content not in SYMBOL_BUTTONS_REQUIRING_NAMES:
            continue
        checked += 1
        accessible_name = element.attrib.get("AutomationProperties.Name", "").strip()
        if not accessible_name:
            failures.append(f"Button '{content}' is missing AutomationProperties.Name.")

    try:
        app_text = app_xaml.read_text(encoding="utf-8")
    except OSError as exc:
        print(f"Unable to read {app_xaml}: {exc}", file=sys.stderr)
        return 2

    required_touch_target_fragments = (
        '<Style Selector="Button">',
        '<Setter Property="MinHeight" Value="44" />',
        '<Style Selector="TextBox">',
        '<Style Selector="ComboBox">',
        '<Style Selector="CheckBox">',
        '<Style Selector="TabItem">',
        '<Style Selector="ListBoxItem">',
        '<Style Selector="Button.calc-key">',
        '<Setter Property="MinHeight" Value="54" />',
        '<Style Selector="UserControl.compact Button.calc-key">',
        '<Setter Property="MinHeight" Value="50" />',
    )
    for fragment in required_touch_target_fragments:
        if fragment not in app_text:
            failures.append(f"App.axaml is missing accessibility baseline fragment: {fragment}")

    if failures:
        print("Accessibility markup validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        f"Validated semantic names for {checked} symbol-heavy buttons and shared/compact touch-target baselines."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
