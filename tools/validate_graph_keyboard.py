#!/usr/bin/env python3
"""Validate CalcNova graph keyboard-navigation source contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

EXPECTED_ACTIONS = {
    "Left": "PanLeft",
    "Right": "PanRight",
    "Up": "PanUp",
    "Down": "PanDown",
    "Add": "ZoomIn",
    "Subtract": "ZoomOut",
    "Home": "ResetViewport",
    "F": "FitToData",
}


def validate(root: Path) -> list[str]:
    mapping_path = root / "src" / "CalcNova.App" / "Infrastructure" / "GraphKeyboardInput.cs"
    control_path = root / "src" / "CalcNova.App" / "Controls" / "GraphPlotControl.cs"

    failures: list[str] = []
    for path in (mapping_path, control_path):
        if not path.is_file():
            failures.append(f"Missing graph keyboard source: {path}")

    if failures:
        return failures

    mapping = mapping_path.read_text(encoding="utf-8")
    control = control_path.read_text(encoding="utf-8")

    if "modifiers != KeyModifiers.None" not in mapping:
        failures.append("Graph keyboard mapping must reject modified shortcuts.")

    for key, action in EXPECTED_ACTIONS.items():
        marker = f"Key.{key} => GraphKeyboardAction.{action}"
        if marker not in mapping:
            failures.append(f"Missing graph keyboard mapping: {marker}")

    for marker in (
        "Focusable = true",
        "protected override void OnKeyDown(KeyEventArgs eventArgs)",
        "GraphKeyboardInput.GetAction(eventArgs.Key, eventArgs.KeyModifiers)",
        "ApplyKeyboardAction(action)",
        "PanViewport(-KeyboardPanFraction, 0d)",
        "PanViewport(KeyboardPanFraction, 0d)",
        "ZoomAround(ViewportCenter(), 0.82d)",
        "ZoomAround(ViewportCenter(), 1.22d)",
        "ResetViewport()",
        "FitToData()",
    ):
        if marker not in control:
            failures.append(f"GraphPlotControl is missing keyboard-navigation marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate graph keyboard-navigation contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Graph keyboard validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated {len(EXPECTED_ACTIONS)} graph keyboard actions and plot-control wiring.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
