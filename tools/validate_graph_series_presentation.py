#!/usr/bin/env python3
"""Validate CalcNova deterministic multi-series graph presentation without .NET."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

EXPECTED_PATTERNS = (
    "Solid",
    "LongDash",
    "ShortDash",
    "Dot",
    "DashDot",
    "SparseDash",
    "DenseDash",
    "AlternatingDash",
)


def validate(root: Path) -> list[str]:
    enum_path = root / "src" / "CalcNova.Graphing" / "GraphSeriesLinePattern.cs"
    catalog_path = root / "src" / "CalcNova.Graphing" / "GraphSeriesLinePatternCatalog.cs"
    presentation_path = root / "src" / "CalcNova.Graphing" / "GraphSeriesPresentation.cs"
    plot_path = root / "src" / "CalcNova.App" / "Controls" / "GraphPlotControl.cs"
    main_view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
    catalog_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphSeriesLinePatternCatalogTests.cs"
    presentation_tests_path = root / "tests" / "CalcNova.Graphing.Tests" / "GraphSeriesPresentationTests.cs"
    headless_path = root / "tests" / "CalcNova.App.Tests" / "GraphMultiSeriesHeadlessTests.cs"

    paths = (
        enum_path,
        catalog_path,
        presentation_path,
        plot_path,
        main_view_path,
        catalog_tests_path,
        presentation_tests_path,
        headless_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing graph-series presentation source: {path}")

    if failures:
        return failures

    enum_source = enum_path.read_text(encoding="utf-8")
    catalog = catalog_path.read_text(encoding="utf-8")
    presentation = presentation_path.read_text(encoding="utf-8")
    plot = plot_path.read_text(encoding="utf-8")
    main_view = main_view_path.read_text(encoding="utf-8")
    catalog_tests = catalog_tests_path.read_text(encoding="utf-8")
    presentation_tests = presentation_tests_path.read_text(encoding="utf-8")
    headless = headless_path.read_text(encoding="utf-8")

    enum_match = re.search(r"enum\s+GraphSeriesLinePattern\s*\{(?P<body>.*?)\}", enum_source, re.DOTALL)
    if enum_match is None:
        failures.append("GraphSeriesLinePattern enum could not be parsed.")
    else:
        names = tuple(re.findall(r"^\s*([A-Za-z_][A-Za-z0-9_]*)\s*(?:=\s*\d+)?\s*,?\s*$", enum_match.group("body"), re.MULTILINE))
        if names != EXPECTED_PATTERNS:
            failures.append("GraphSeriesLinePattern must preserve the eight stable pattern identities in order.")

    for marker in (
        "Patterns = Enum.GetValues<GraphSeriesLinePattern>()",
        "ForSeriesIndex",
        "GetLabel",
        "ShouldDrawEdge",
        "seriesIndex % Patterns.Length",
    ):
        if marker not in catalog:
            failures.append(f"GraphSeriesLinePatternCatalog is missing marker: {marker}")

    for marker in (
        "GraphSeriesLinePatternCatalog.GetLabel(Pattern)",
        "GraphSeriesLinePatternCatalog.PatternCount",
        "GraphSeriesLinePatternCatalog.ForSeriesIndex(index)",
        "LegendText",
    ):
        if marker not in presentation:
            failures.append(f"GraphSeriesPresentation is missing shared catalog marker: {marker}")

    for marker in (
        "SeriesProperty",
        "GraphSeriesLinePatternCatalog.ForSeriesIndex(seriesIndex)",
        "GraphSeriesLinePatternCatalog.ShouldDrawEdge(pattern, edgeIndex)",
        "GraphSeriesLinePattern.Solid",
    ):
        if marker not in plot:
            failures.append(f"GraphPlotControl is missing multi-series pattern marker: {marker}")

    for marker in (
        'legend.Classes.Add("graph-series-legend")',
        "GraphPlotMode.Multiple",
        "_graphPlotControl.Series = _graphPlotViewModel.MultiSeries",
        "GraphSeriesPresentationFactory.Create(_graphPlotViewModel.MultiSeries)",
        "presentation.LegendText",
    ):
        if marker not in main_view:
            failures.append(f"MainView is missing multi-series legend marker: {marker}")

    for marker in (
        "Catalog_ContainsEightDistinctPatterns",
        "PatternMasks_AreUniqueAcrossRepresentativeWindow",
        "PatternLabels_AreNonEmptyAndUnique",
    ):
        if marker not in catalog_tests:
            failures.append(f"GraphSeriesLinePatternCatalogTests is missing test: {marker}")

    for marker in (
        "EightSeries_ReceiveEightDistinctPatternsInStableOrder",
        "Presentation_PreservesSeriesCountsAndIdentity",
        "MoreThanEightSeries_IsRejectedInsteadOfReusingAmbiguousPatterns",
    ):
        if marker not in presentation_tests:
            failures.append(f"GraphSeriesPresentationTests is missing test: {marker}")

    for marker in (
        "MultiSeriesPlot_UsesSeriesSurfaceAndTextPatternLegend",
        "ReturningToSinglePlot_ClearsMultiSeriesLegend",
        '"f1 [solid] — sin(x)"',
        '"f2 [long dash] — cos(x)"',
    ):
        if marker not in headless:
            failures.append(f"Graph multi-series headless tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova multi-series graph presentation contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Graph series presentation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated eight deterministic non-color graph patterns, text legends, renderer wiring, and headless coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
