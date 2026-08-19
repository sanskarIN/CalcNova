#!/usr/bin/env python3
"""Validate CalcNova Avalonia headless UI-test source contracts without .NET."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

EXPECTED_AVALONIA_VERSION = "12.1.1"


def validate(root: Path) -> list[str]:
    package_path = root / "Directory.Packages.props"
    project_path = root / "tests" / "CalcNova.App.Tests" / "CalcNova.App.Tests.csproj"
    builder_path = root / "tests" / "CalcNova.App.Tests" / "TestAppBuilder.cs"
    shell_tests_path = root / "tests" / "CalcNova.App.Tests" / "MainViewHeadlessTests.cs"
    graph_tests_path = root / "tests" / "CalcNova.App.Tests" / "GraphPlotControlHeadlessTests.cs"
    graph_control_path = root / "src" / "CalcNova.App" / "Controls" / "GraphPlotControl.cs"
    solution_path = root / "CalcNova.slnx"

    paths = (
        package_path,
        project_path,
        builder_path,
        shell_tests_path,
        graph_tests_path,
        graph_control_path,
        solution_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing headless UI-test source: {path}")

    if failures:
        return failures

    packages = package_path.read_text(encoding="utf-8")
    project = project_path.read_text(encoding="utf-8")
    builder = builder_path.read_text(encoding="utf-8")
    shell_tests = shell_tests_path.read_text(encoding="utf-8")
    graph_tests = graph_tests_path.read_text(encoding="utf-8")
    graph_control = graph_control_path.read_text(encoding="utf-8")
    solution = solution_path.read_text(encoding="utf-8")

    avalonia_match = re.search(r'<PackageVersion Include="Avalonia" Version="([^"]+)"', packages)
    headless_match = re.search(r'<PackageVersion Include="Avalonia\.Headless\.XUnit" Version="([^"]+)"', packages)
    if avalonia_match is None or headless_match is None:
        failures.append("Central package catalog must define Avalonia and Avalonia.Headless.XUnit versions.")
    else:
        if avalonia_match.group(1) != headless_match.group(1):
            failures.append("Avalonia.Headless.XUnit must match the repository Avalonia version.")
        if avalonia_match.group(1) != EXPECTED_AVALONIA_VERSION:
            failures.append(
                f"Headless UI contract expected Avalonia {EXPECTED_AVALONIA_VERSION}, found {avalonia_match.group(1)}."
            )

    for marker in (
        '<PackageReference Include="Avalonia.Headless.XUnit" />',
        '<PackageReference Include="Avalonia.Themes.Fluent" />',
        '<PackageReference Include="xunit.v3" />',
    ):
        if marker not in project:
            failures.append(f"App test project is missing headless UI package marker: {marker}")

    for marker in (
        "[assembly: AvaloniaTestApplication(typeof(CalcNova.App.Tests.TestAppBuilder))]",
        "AppBuilder.Configure<App>()",
        "UseHeadless(new AvaloniaHeadlessPlatformOptions())",
    ):
        if marker not in builder:
            failures.append(f"Headless test app builder is missing marker: {marker}")

    expected_shell_tests = (
        "SharedShell_LoadsEveryPrimaryMode",
        "CalculatorClearButton_ExecutesBoundCommand",
        "CompactWindow_AppliesCompactAdaptiveClass",
        "CtrlPageDown_AdvancesSharedModeSelection",
        "HighContrastPreference_AppliesShellClass",
        "NewUser_OnboardingOverlayIsVisibleAndSkipHidesIt",
    )
    for marker in expected_shell_tests:
        if marker not in shell_tests:
            failures.append(f"Headless shared-shell suite is missing test: {marker}")

    expected_graph_tests = (
        "KeyboardPanAndZoom_UpdateViewport",
        "HomeKey_ResetsViewportAfterKeyboardNavigation",
        "FitKey_FitsFiniteSampledData",
    )
    for marker in expected_graph_tests:
        if marker not in graph_tests:
            failures.append(f"Headless graph suite is missing test: {marker}")

    expected_fact_count = len(expected_shell_tests) + len(expected_graph_tests)
    if shell_tests.count("[AvaloniaFact]") + graph_tests.count("[AvaloniaFact]") < expected_fact_count:
        failures.append("Headless UI scenarios must remain AvaloniaFact tests.")

    for marker in (
        "window.Show()",
        "GetVisualDescendants()",
        'button.Content?.ToString(), "AC"',
        "window.KeyPressQwerty(PhysicalKey.PageDown, RawInputModifiers.Control)",
        'Assert.Contains("compact", view.Classes)',
        'Assert.Contains("high-contrast", view.Classes)',
        "Assert.True(overlay.IsVisible)",
    ):
        if marker not in shell_tests:
            failures.append(f"Headless shared-shell suite is missing interaction marker: {marker}")

    for marker in (
        "window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None)",
        "window.KeyPressQwerty(PhysicalKey.NumPadAdd, RawInputModifiers.None)",
        "window.KeyPressQwerty(PhysicalKey.Home, RawInputModifiers.None)",
        "window.KeyPressQwerty(PhysicalKey.F, RawInputModifiers.None)",
        "plot.Viewport",
    ):
        if marker not in graph_tests:
            failures.append(f"Headless graph suite is missing interaction marker: {marker}")

    if "public GraphViewport Viewport" not in graph_control:
        failures.append("GraphPlotControl must expose read-only viewport state for deterministic UI assertions.")

    if '<Project Path="tests/CalcNova.App.Tests/CalcNova.App.Tests.csproj" />' not in solution:
        failures.append("CalcNova.slnx must include the headless-enabled App test project.")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova Avalonia headless UI-test contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Headless UI-test validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated Avalonia headless xUnit configuration plus shared-shell and graph UI scenarios.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
