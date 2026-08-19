#!/usr/bin/env python3
"""Validate CalcNova calculator keyboard mapping/wiring contracts without .NET."""

from __future__ import annotations

import argparse
import re
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


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova keyboard contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    mapping_path = root / "src" / "CalcNova.App" / "Infrastructure" / "CalculatorKeyboardInput.cs"
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"

    failures: list[str] = []
    for path in (mapping_path, view_path):
        if not path.is_file():
            failures.append(f"Missing keyboard source: {path}")

    if failures:
        for failure in failures:
            print(failure, file=sys.stderr)
        return 2

    mapping_source = mapping_path.read_text(encoding="utf-8")
    view_source = view_path.read_text(encoding="utf-8")

    parsed_pairs = dict(
        re.findall(r"Key\.([A-Za-z0-9_]+)(?:\s+or\s+Key\.[A-Za-z0-9_]+)?\s*=>\s*\"([^\"]+)\"", mapping_source)
    )

    for key, token in EXPECTED_KEY_TOKENS.items():
        direct_marker = f"Key.{key}"
        token_marker = f'=> "{token}"'
        if direct_marker not in mapping_source or token_marker not in mapping_source:
            failures.append(f"Calculator keyboard map is missing {key} -> {token!r}.")

    for marker in (
        "CalculatorKeyboardInput.TryGetToken",
        "eventArgs.Source is not TextBox",
        "eventArgs.KeyModifiers == KeyModifiers.None",
        "viewModel.Calculator.AppendCommand.Execute(token)",
    ):
        if marker not in view_source:
            failures.append(f"MainView keyboard wiring is missing marker: {marker}")

    if failures:
        print("Keyboard contract validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated {len(EXPECTED_KEY_TOKENS)} calculator hardware-key mappings and shell wiring.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
