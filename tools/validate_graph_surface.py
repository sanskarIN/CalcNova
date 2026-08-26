#!/usr/bin/env python3
"""Validate CalcNova shared interactive graph-surface wiring without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    main_view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
    plot_path = root / "src" / "CalcNova.App" / "Controls" / "GraphPlotControl.cs"
    headless_path = root / "tests" / "CalcNova.App.Tests" / "MainViewHeadlessTests.cs"

    paths = (main_view_path, plot_path, headless_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing graph-surface source: {path}")

    if failures:
        return failures

    main_view = main_view_path.read_text(encoding="utf-8")
    plot = plot_path.read_text(encoding="utf-8")
    headless = headless_path.read_text(encoding="utf-8")

    for marker in (
        "private GraphPlotControl? _graphPlotControl;",
        "EnsureGraphPlot(viewModel.Graphing)",
        "private void EnsureGraphPlot(GraphingViewModel graphing)",
        "MinHeight = 300",
        "graphing.PropertyChanged += HandleGraphingPropertyChanged",
        "nameof(GraphingViewModel.Segments)",
        "nameof(GraphingViewModel.MultiSeries)",
        "_graphPlotControl.Segments = _graphPlotViewModel.Segments",
        "_graphPlotControl.Series = _graphPlotViewModel.MultiSeries",
        "ToolTip.SetTip(plot",
    ):
        if marker not in main_view:
            failures.append(f"MainView is missing shared graph-surface marker: {marker}")

    for marker in (
        "Focusable = true",
        "DoubleTapped += (_, _) => FitToData()",
        "protected override void OnPointerWheelChanged",
        "protected override void OnKeyDown",
        "public GraphViewport Viewport",
    ):
        if marker not in plot:
            failures.append(f"GraphPlotControl is missing interaction marker: {marker}")

    for marker in (
        "GraphMode_SurfacesInteractivePlotAndTracksSampledSegments",
        "OfType<GraphPlotControl>().Single()",
        "Assert.True(plot.Focusable)",
        "Assert.Same(viewModel.Graphing.Segments, plot.Segments)",
    ):
        if marker not in headless:
            failures.append(f"Headless UI tests are missing graph-surface marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova shared interactive graph surface.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Graph surface validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated shared interactive graph surface, single/multi-series synchronization, and headless coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
