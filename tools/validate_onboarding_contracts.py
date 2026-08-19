#!/usr/bin/env python3
"""Validate CalcNova onboarding persistence and shared visual contracts without .NET."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova onboarding contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    settings_path = root / "src" / "CalcNova.Platform" / "Settings" / "AppSettings.cs"
    policy_path = root / "src" / "CalcNova.App" / "Infrastructure" / "OnboardingPolicy.cs"
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "SettingsViewModel.cs"
    overlay_path = root / "src" / "CalcNova.App" / "Views" / "OnboardingOverlay.axaml"
    overlay_code_path = root / "src" / "CalcNova.App" / "Views" / "OnboardingOverlay.axaml.cs"
    main_view_code_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"

    failures: list[str] = []
    for path in (
        settings_path,
        policy_path,
        view_model_path,
        overlay_path,
        overlay_code_path,
        main_view_code_path,
    ):
        if not path.is_file():
            failures.append(f"Missing onboarding source: {path}")

    if failures:
        for failure in failures:
            print(failure, file=sys.stderr)
        return 2

    settings_source = settings_path.read_text(encoding="utf-8")
    policy_source = policy_path.read_text(encoding="utf-8")
    view_model_source = view_model_path.read_text(encoding="utf-8")
    overlay_source = overlay_path.read_text(encoding="utf-8")
    overlay_code_source = overlay_code_path.read_text(encoding="utf-8")
    main_view_code_source = main_view_code_path.read_text(encoding="utf-8")

    if "public int CompletedOnboardingVersion" not in settings_source:
        failures.append("AppSettings is missing CompletedOnboardingVersion.")

    version_match = re.search(r"public\s+const\s+int\s+CurrentVersion\s*=\s*(\d+)\s*;", policy_source)
    if version_match is None:
        failures.append("OnboardingPolicy.CurrentVersion is missing or unparsable.")
    elif int(version_match.group(1)) < 1:
        failures.append("OnboardingPolicy.CurrentVersion must be at least 1.")

    for marker in (
        "ShouldShow(int completedVersion)",
        "MarkCurrentVersionCompleted()",
        "NormalizeCompletedVersion(int completedVersion)",
    ):
        if marker not in policy_source:
            failures.append(f"OnboardingPolicy is missing marker: {marker}")

    for marker in (
        "public bool IsLoaded",
        "ShouldShowOnboarding => _isLoaded &&",
        "CompleteOnboardingCommand",
        "SkipOnboardingCommand",
        "CompleteOnboardingAsync",
        "SkipOnboardingAsync",
        "CompletedOnboardingVersion = OnboardingPolicy.NormalizeCompletedVersion",
    ):
        if marker not in view_model_source:
            failures.append(f"SettingsViewModel is missing onboarding marker: {marker}")

    for marker in (
        'IsVisible="{Binding Settings.ShouldShowOnboarding}"',
        'Command="{Binding Settings.SkipOnboardingCommand}"',
        'Command="{Binding Settings.CompleteOnboardingCommand}"',
        'AutomationProperties.Name="Skip CalcNova introduction"',
        'AutomationProperties.Name="Complete CalcNova introduction and start calculating"',
        "No account is required",
    ):
        if marker not in overlay_source:
            failures.append(f"OnboardingOverlay.axaml is missing visual contract marker: {marker}")

    for marker in (
        "partial class OnboardingOverlay",
        "InitializeComponent();",
    ):
        if marker not in overlay_code_source:
            failures.append(f"OnboardingOverlay code-behind is missing marker: {marker}")

    for marker in (
        "AttachOnboardingOverlay();",
        "new OnboardingOverlay()",
        "Grid.SetRowSpan",
        "viewModel.Settings.ShouldShowOnboarding",
        "QueueOnboardingFocus();",
        "QueueCalculatorFocus();",
        "Dispatcher.UIThread.Post",
        "ReferenceEquals(textBox.DataContext, calculator)",
    ):
        if marker not in main_view_code_source:
            failures.append(f"MainView onboarding integration is missing marker: {marker}")

    if failures:
        print("Onboarding contract validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        "Validated versioned onboarding persistence, deferred first-run display, shared visual surface, "
        "complete/skip actions, shell attachment, and focus restoration contracts."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
