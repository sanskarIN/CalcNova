#!/usr/bin/env python3
"""Validate bounded export-preview contracts without requiring the .NET SDK."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    formatter_path = root / "src" / "CalcNova.App" / "Infrastructure" / "ExportPreviewFormatter.cs"
    history_path = root / "src" / "CalcNova.App" / "ViewModels" / "HistoryViewModel.cs"
    formatter_tests_path = root / "tests" / "CalcNova.App.Tests" / "ExportPreviewFormatterTests.cs"
    history_tests_path = root / "tests" / "CalcNova.App.Tests" / "HistoryExportPreviewViewModelTests.cs"

    paths = (formatter_path, history_path, formatter_tests_path, history_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing export-preview source: {path}")

    if failures:
        return failures

    formatter = formatter_path.read_text(encoding="utf-8")
    history = history_path.read_text(encoding="utf-8")
    formatter_tests = formatter_tests_path.read_text(encoding="utf-8")
    history_tests = history_tests_path.read_text(encoding="utf-8")

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

    print("Validated bounded history previews, newline-safe truncation, and preservation of full copy/export content.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
