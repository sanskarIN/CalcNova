#!/usr/bin/env python3
"""Validate CalcNova local Unicode scalar metadata contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    metadata_path = root / "src" / "CalcNova.Programmer" / "UnicodeScalarMetadata.cs"
    helper_path = root / "src" / "CalcNova.Programmer" / "UnicodeCodePointHelper.cs"
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "CodePointViewModel.cs"
    panel_path = root / "src" / "CalcNova.App" / "Controls" / "CodePointMetadataPanel.cs"
    main_view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
    core_tests_path = root / "tests" / "CalcNova.Programmer.Tests" / "UnicodeScalarMetadataTests.cs"
    app_tests_path = root / "tests" / "CalcNova.App.Tests" / "CodePointMetadataViewModelTests.cs"
    headless_tests_path = root / "tests" / "CalcNova.App.Tests" / "CodePointMetadataPanelHeadlessTests.cs"

    paths = (
        metadata_path,
        helper_path,
        view_model_path,
        panel_path,
        main_view_path,
        core_tests_path,
        app_tests_path,
        headless_tests_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing Unicode metadata source: {path}")

    if failures:
        return failures

    metadata = metadata_path.read_text(encoding="utf-8")
    helper = helper_path.read_text(encoding="utf-8")
    view_model = view_model_path.read_text(encoding="utf-8")
    panel = panel_path.read_text(encoding="utf-8")
    main_view = main_view_path.read_text(encoding="utf-8")
    core_tests = core_tests_path.read_text(encoding="utf-8")
    app_tests = app_tests_path.read_text(encoding="utf-8")
    headless_tests = headless_tests_path.read_text(encoding="utf-8")

    for marker in (
        "public sealed record UnicodeScalarMetadata",
        "string GeneralCategory",
        "int Utf8ByteCount",
        "int Utf16CodeUnitCount",
        "public string CompactSummary",
    ):
        if marker not in metadata:
            failures.append(f"UnicodeScalarMetadata is missing contract marker: {marker}")

    for marker in (
        "public static UnicodeScalarMetadata Describe(int codePoint)",
        "CharUnicodeInfo.GetUnicodeCategory",
        "Encoding.UTF8.GetByteCount(text)",
        "public static IReadOnlyList<UnicodeScalarMetadata> DescribeText",
        "ValidateCodePoint(codePoint)",
    ):
        if marker not in helper:
            failures.append(f"UnicodeCodePointHelper is missing metadata marker: {marker}")

    for marker in (
        "public string CodePointMetadata",
        "public string TextMetadata",
        "UnicodeCodePointHelper.Describe(value).CompactSummary",
        "UnicodeCodePointHelper.DescribeText(TextInput)",
        "CodePointMetadata = string.Empty",
        "TextMetadata = string.Empty",
    ):
        if marker not in view_model:
            failures.append(f"CodePointViewModel is missing metadata marker: {marker}")

    for marker in (
        "public sealed class CodePointMetadataPanel",
        "nameof(CodePointViewModel.CodePointMetadata)",
        "nameof(CodePointViewModel.TextMetadata)",
        "derived locally without a network lookup",
    ):
        if marker not in panel:
            failures.append(f"CodePointMetadataPanel is missing UI marker: {marker}")

    for marker in (
        "private CodePointMetadataPanel? _codePointMetadataPanel;",
        "EnsureCodePointMetadataPanel(viewModel.CodePoint)",
        "private void EnsureCodePointMetadataPanel(CodePointViewModel codePoint)",
        "DataContext = codePoint",
        "DetachCodePointMetadataPanel()",
    ):
        if marker not in main_view:
            failures.append(f"MainView is missing Unicode metadata-panel marker: {marker}")

    for marker in (
        "Describe_BasicLatinLetter_ReportsCategoryAndEncodingWidths",
        "Describe_SupplementaryScalar_ReportsPlaneAndEncodingWidths",
        "DescribeText_EnumeratesScalarsWithoutSplittingSurrogatePairs",
        "Describe_RejectsSurrogateCodePoint",
    ):
        if marker not in core_tests:
            failures.append(f"Core Unicode metadata tests are missing marker: {marker}")

    for marker in (
        "DecodeCodePointCommand_ProjectsLocalScalarMetadata",
        "InspectTextCommand_ProjectsOneMetadataLinePerScalar",
        "InvalidScalar_ClearsPreviouslyProjectedMetadata",
    ):
        if marker not in app_tests:
            failures.append(f"App Unicode metadata tests are missing marker: {marker}")

    for marker in (
        "CodePointMode_SurfacesLocalScalarMetadataPanel",
        "OfType<CodePointMetadataPanel>().Single()",
        "Assert.Same(viewModel.CodePoint, panel.DataContext)",
        "UTF-8 4 byte",
    ):
        if marker not in headless_tests:
            failures.append(f"Headless Unicode metadata tests are missing marker: {marker}")

    forbidden_network_markers = ("HttpClient", "https://", "http://")
    for marker in forbidden_network_markers:
        if marker in helper or marker in metadata:
            failures.append(f"Unicode metadata must remain local-only; found forbidden marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova local Unicode scalar metadata contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Unicode metadata validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated local Unicode scalar metadata, shared UI projection, and regression coverage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
