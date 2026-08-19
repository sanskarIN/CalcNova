#!/usr/bin/env python3
"""Validate bounded export-preview contracts without requiring the .NET SDK."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    formatter_path = root / "src" / "CalcNova.App" / "Infrastructure" / "ExportPreviewFormatter.cs"
    history_path = root / "src" / "CalcNova.App" / "ViewModels" / "HistoryViewModel.cs"
    graph_path = root / "src" / "CalcNova.App" / "ViewModels" / "GraphingViewModel.cs"
    formatter_tests_path = root / "tests" / "CalcNova.App.Tests" / "ExportPreviewFormatterTests.cs"
    history_tests_path = root / "tests" / "CalcNova.App.Tests" / "HistoryExportPreviewViewModelTests.cs"
    graph_tests_path = root / "tests" / "CalcNova.App.Tests" / "GraphExportPreviewViewModelTests.cs"

    paths = (
        formatter_path,
        history_path,
        graph_path,
        formatter_tests_path,
        history_tests_path,
        graph_tests_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing export-preview source: {path}")

    if failures:
        return failures

    formatter = formatter_path.read_text(encoding="utf-8")
    history = history_path.read_text(encoding="utf-8")
    graph = graph_path.read_text(encoding="utf-8")
    formatter_tests = formatter_tests_path.read_text(encoding="utf-8")
    history_tests = history_tests_path.read_text(encoding="utf-8")
    graph_tests = graph_tests_path.read_text(encoding="utf-8")

    for marker in (
        "public const int DefaultMaximumCharacters = 4_096;",
        "public const int DefaultMaximumLines = 80;",
        "preview truncated; full content is preserved for copy/export",
        "private static bool ExceedsLineLimit",
        "var hasWrittenLine = false;",
        "private static string SafePrefix",
        "char.IsHighSurrogate",
    ):
        if marker not in formatter:
            failures.append(f"ExportPreviewFormatter is missing contract marker: {marker}")

    for marker in (
        "private string _exportContent = string.Empty;",
        "public bool IsExportPreviewTruncated",
        "ExportPreview = ExportPreviewFormatter.Create(_exportContent);",
        "copy uses the full export",
        "ClipboardTextWriter.CopyAsync(_clipboardService, _exportContent, \"history export\")",
        "private void ClearExport()",
    ):
        if marker not in history:
            failures.append(f"HistoryViewModel is missing bounded-preview marker: {marker}")

    if "ClipboardTextWriter.CopyAsync(_clipboardService, ExportPreview" in history:
        failures.append("History export copy must use full export content, not the bounded preview.")

    for marker in (
        "private string _tableExportContent = string.Empty;",
        "private string _multiTableExportContent = string.Empty;",
        "private string _svgExportContent = string.Empty;",
        "TablePreview = ExportPreviewFormatter.Create(_tableExportContent);",
        "TableCsv = TablePreview;",
        "MultiTablePreview = ExportPreviewFormatter.Create(_multiTableExportContent);",
        "MultiTableCsv = MultiTablePreview;",
        "SvgPreview = ExportPreviewFormatter.Create(_svgExportContent);",
        "SvgExport = SvgPreview;",
        "ClipboardTextWriter.CopyAsync(_clipboardService, _tableExportContent, \"graph table\")",
        "ClipboardTextWriter.CopyAsync(_clipboardService, _multiTableExportContent, \"multi-expression graph table\")",
        "ClipboardTextWriter.CopyAsync(_clipboardService, _svgExportContent, \"SVG graph export\")",
    ):
        if marker not in graph:
            failures.append(f"GraphingViewModel is missing bounded-preview marker: {marker}")

    for forbidden in (
        "ClipboardTextWriter.CopyAsync(_clipboardService, TableCsv, \"graph table\")",
        "ClipboardTextWriter.CopyAsync(_clipboardService, MultiTableCsv, \"multi-expression graph table\")",
        "ClipboardTextWriter.CopyAsync(_clipboardService, SvgExport, \"SVG graph export\")",
    ):
        if forbidden in graph:
            failures.append("Graph copy commands must use full private export content, not bounded display text.")

    for marker in (
        "Create_ReturnsShortContentUnchanged",
        "Create_TruncatesContentAboveCharacterBudget",
        "Create_TruncatesContentAboveLineBudget",
        "Create_PreservesLeadingBlankLinesWhenTruncated",
        "Create_RecognizesCarriageReturnOnlyLineBoundaries",
        "Create_DoesNotSplitUtf16SurrogatePairAtCharacterBoundary",
    ):
        if marker not in formatter_tests:
            failures.append(f"Export preview formatter tests are missing marker: {marker}")

    for marker in (
        "GenerateExportCommand_BoundsLongPreviewWithoutChangingFullExport",
        "CopyExportCommand_CopiesFullContentWhenPreviewIsTruncated",
        "ExportFormatChange_ClearsTruncationStateAndCopyRegeneratesSelectedFormat",
        "Assert.NotEqual(viewModel.ExportPreview, clipboard.WrittenText)",
    ):
        if marker not in history_tests:
            failures.append(f"History export preview tests are missing marker: {marker}")

    for marker in (
        "SingleTablePreview_IsBoundedWhileCopyKeepsFullCsv",
        "MultiTablePreview_IsBoundedWhileCopyKeepsFullCsv",
        "SvgPreview_IsBoundedWhileCopyKeepsFullSvg",
        "GraphTableExporter.ToCsv(viewModel.TableRows)",
        "MultiGraphTableExporter.ToCsv(viewModel.MultiTableRows)",
        "new SvgGraphExporter().Export(viewModel.Segments)",
        "Assert.NotEqual(viewModel.TableCsv, clipboard.WrittenText)",
        "Assert.NotEqual(viewModel.MultiTableCsv, clipboard.WrittenText)",
        "Assert.NotEqual(viewModel.SvgExport, clipboard.WrittenText)",
    ):
        if marker not in graph_tests:
            failures.append(f"Graph export preview tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova bounded export-preview contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Export preview validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated bounded history/graph previews, newline-safe truncation, and preservation of full copy/export content.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
