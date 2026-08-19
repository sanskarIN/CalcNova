#!/usr/bin/env python3
"""Validate CalcNova calculator and shared-shell keyboard contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


EXPECTED_KEY_TOKENS = {
    "D0": "0",
    "D1": "1",
    "D2": "2",
    "D3": "3",
    "D4": "4",
    "D5": "5",
    "D6": "6",
    "D7": "7",
    "D8": "8",
    "D9": "9",
    "NumPad0": "0",
    "NumPad1": "1",
    "NumPad2": "2",
    "NumPad3": "3",
    "NumPad4": "4",
    "NumPad5": "5",
    "NumPad6": "6",
    "NumPad7": "7",
    "NumPad8": "8",
    "NumPad9": "9",
    "Add": "+",
    "Subtract": "-",
    "Multiply": "*",
    "Divide": "/",
    "Decimal": ".",
}

EXPECTED_SHELL_SHORTCUTS = {
    "PageUp": "PreviousMode",
    "PageDown": "NextMode",
    "Home": "FirstMode",
    "End": "LastMode",
}


def validate(root: Path) -> list[str]:
    mapping_path = root / "src" / "CalcNova.App" / "Infrastructure" / "CalculatorKeyboardInput.cs"
    shortcut_path = root / "src" / "CalcNova.App" / "Infrastructure" / "ShellKeyboardShortcut.cs"
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"

    failures: list[str] = []
    for path in (mapping_path, shortcut_path, view_path):
        if not path.is_file():
            failures.append(f"Missing keyboard source: {path}")

    if failures:
        return failures

    mapping_source = mapping_path.read_text(encoding="utf-8")
    shortcut_source = shortcut_path.read_text(encoding="utf-8")
    view_source = view_path.read_text(encoding="utf-8")

    for key, token in EXPECTED_KEY_TOKENS.items():
        direct_marker = f"Key.{key}"
        token_marker = f'=> "{token}"'
        if direct_marker not in mapping_source or token_marker not in mapping_source:
            failures.append(f"Calculator keyboard map is missing {key} -> {token!r}.")

    if "modifiers != KeyModifiers.Control" not in shortcut_source:
        failures.append("Shell shortcut policy must require exactly the Control modifier.")

    for key, action in EXPECTED_SHELL_SHORTCUTS.items():
        marker = f"Key.{key} => ShellNavigationAction.{action}"
        if marker not in shortcut_source:
            failures.append(f"Shell keyboard map is missing Ctrl+{key} -> {action}.")

    for marker in (
        "ShellKeyboardShortcut.GetNavigationAction",
        "ApplyShellNavigation(viewModel, navigationAction)",
        "CalculatorKeyboardInput.TryGetToken",
        "eventArgs.Source is not TextBox",
        "eventArgs.KeyModifiers == KeyModifiers.None",
        "viewModel.Calculator.AppendCommand.Execute(token)",
    ):
        if marker not in view_source:
            failures.append(f"MainView keyboard wiring is missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova keyboard contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)

    if failures:
        print("Keyboard contract validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        "Validated "
        f"{len(EXPECTED_KEY_TOKENS)} calculator hardware-key mappings, "
        f"{len(EXPECTED_SHELL_SHORTCUTS)} shared-shell shortcuts, and shell wiring."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
