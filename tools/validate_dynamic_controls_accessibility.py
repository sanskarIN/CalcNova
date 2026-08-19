#!/usr/bin/env python3
"""Validate accessibility contracts for dynamically inserted shared controls."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.CheckBoxLocalization.cs"
    focus_path = root / "src" / "CalcNova.App" / "Controls" / "GraphPlotControl.cs"
    test_path = root / "tests" / "CalcNova.App.Tests" / "GraphViewportToolbarAccessibilityHeadlessTests.cs"

    paths = (view_path, focus_path, test_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing dynamic accessibility source: {path}")
    if failures:
        return failures

    view = view_path.read_text(encoding="utf-8")
    plot = focus_path.read_text(encoding="utf-8")
    tests = test_path.read_text(encoding="utf-8")

    for marker in (
        'Classes.Add("graph-viewport-toolbar")',
        "AddGraphViewportButton(AppStringKey.ActionGraphPanLeft, plot.PanLeft)",
        "AddGraphViewportButton(AppStringKey.ActionGraphZoomIn, plot.ZoomIn)",
        "AddGraphViewportButton(AppStringKey.ActionReset, plot.ResetViewport)",
    ):
        if marker not in view:
            failures.append(f"Dynamic graph toolbar is missing marker: {marker}")

    if "Focusable = true" not in plot:
        failures.append("GraphPlotControl must remain keyboard-focusable.")

    for marker in (
        "GraphViewportButtons_InheritTouchTargetAndKeyboardFocusBaseline",
        "Assert.True(button.Focusable)",
        "button.MinHeight >= 44d",
        "Assert.Equal(8, buttons.Length)",
    ):
        if marker not in tests:
            failures.append(f"Dynamic graph accessibility tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate dynamic shared-control accessibility contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()
    failures = validate(Path(args.root).resolve())
    if failures:
        print("Dynamic-control accessibility validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated dynamic graph controls retain focus and 44-DIP target regression coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
