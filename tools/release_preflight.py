#!/usr/bin/env python3
"""Run CalcNova's deterministic release-source checks without a .NET SDK."""

from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path


CHECKS: tuple[tuple[str, tuple[str, ...]], ...] = (
    ("Repository structure/security", ("tools/scripts/validate_repository.py",)),
    ("Avalonia XAML XML", ("tools/validate_xaml.py", ".")),
    ("Shared UI contracts", ("tools/validate_ui_contracts.py", ".")),
    ("Navigation contracts", ("tools/validate_navigation_contracts.py", ".")),
    ("Keyboard contracts", ("tools/validate_keyboard_contracts.py", ".")),
    ("Calculator selection editing", ("tools/validate_calculator_selection_editing.py", ".")),
    ("Graph keyboard contracts", ("tools/validate_graph_keyboard.py", ".")),
    ("Graph surface contracts", ("tools/validate_graph_surface.py", ".")),
    ("Graph series presentation", ("tools/validate_graph_series_presentation.py", ".")),
    ("Numerical analysis safety", ("tools/validate_numerical_analysis.py", ".")),
    ("Graph workload budgets", ("tools/validate_graph_numerical_budgets.py", ".")),
    ("Unicode metadata contracts", ("tools/validate_unicode_metadata.py", ".")),
    ("Headless UI-test contracts", ("tools/validate_headless_ui_tests.py", ".")),
    ("Accessibility markup", ("tools/validate_accessibility_markup.py", ".")),
    ("Focus visibility", ("tools/validate_focus_visibility.py", ".")),
    ("Accessibility evidence", ("tools/validate_accessibility_evidence.py", ".")),
    ("Adaptive layout", ("tools/validate_adaptive_layout.py", ".")),
    ("Touch targets", ("tools/validate_touch_targets.py", ".")),
    ("Localization catalog", ("tools/validate_localization_catalog.py", ".")),
    ("Converter defaults", ("tools/validate_converter_defaults.py", ".")),
    ("Settings schema", ("tools/validate_settings_schema.py", ".")),
    ("Onboarding contracts", ("tools/validate_onboarding_contracts.py", ".")),
    ("Packaging metadata", ("tools/validate_packaging_metadata.py", ".")),
    ("Platform workflows", ("tools/validate_platform_workflows.py", ".")),
    ("Release workflow", ("tools/validate_release_workflow.py", ".")),
    ("Release documentation", ("tools/validate_release_docs.py", ".")),
    ("Release-tag validator tests", ("tools/tests/test_validate_release_tag.py",)),
    ("Release-workflow validator tests", ("-m", "unittest", "tools.tests.test_validate_release_workflow")),
    ("Release-documentation validator tests", ("-m", "unittest", "tools.tests.test_validate_release_docs")),
    ("Headless UI validator tests", ("-m", "unittest", "tools.tests.test_validate_headless_ui_tests")),
    ("Focus validator tests", ("-m", "unittest", "tools.tests.test_validate_focus_visibility")),
    ("Accessibility evidence validator tests", ("-m", "unittest", "tools.tests.test_validate_accessibility_evidence")),
    ("Keyboard validator tests", ("-m", "unittest", "tools.tests.test_validate_keyboard_contracts")),
    ("Calculator selection validator tests", ("-m", "unittest", "tools.tests.test_validate_calculator_selection_editing")),
    ("Graph keyboard validator tests", ("-m", "unittest", "tools.tests.test_validate_graph_keyboard")),
    ("Graph surface validator tests", ("-m", "unittest", "tools.tests.test_validate_graph_surface")),
    ("Graph-series presentation validator tests", ("-m", "unittest", "tools.tests.test_validate_graph_series_presentation")),
    ("Numerical-analysis validator tests", ("-m", "unittest", "tools.tests.test_validate_numerical_analysis")),
    ("Graph workload-budget validator tests", ("-m", "unittest", "tools.tests.test_validate_graph_numerical_budgets")),
    ("Unicode metadata validator tests", ("-m", "unittest", "tools.tests.test_validate_unicode_metadata")),
    ("Localization validator tests", ("-m", "unittest", "tools.tests.test_validate_localization_catalog")),
    ("Converter-default validator tests", ("-m", "unittest", "tools.tests.test_validate_converter_defaults")),
    ("Settings schema validator tests", ("-m", "unittest", "tools.tests.test_validate_settings_schema")),
    ("Adaptive validator tests", ("-m", "unittest", "tools.tests.test_validate_adaptive_layout")),
    ("Touch-target validator tests", ("-m", "unittest", "tools.tests.test_validate_touch_targets")),
    ("Packaging validator tests", ("-m", "unittest", "tools.tests.test_validate_packaging_metadata")),
    ("Platform-workflow validator tests", ("-m", "unittest", "tools.tests.test_validate_platform_workflows")),
    ("Preflight inventory tests", ("-m", "unittest", "tools.tests.test_release_preflight")),
)


def run_check(root: Path, label: str, arguments: tuple[str, ...]) -> bool:
    command = [sys.executable, *arguments]
    print(f"\n==> {label}", flush=True)
    completed = subprocess.run(command, cwd=root, check=False)
    if completed.returncode == 0:
        return True

    print(f"FAILED: {label} exited with code {completed.returncode}.", file=sys.stderr)
    return False


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Run CalcNova SDK-independent release-source validation."
    )
    parser.add_argument(
        "--tag",
        help="Optional release tag to validate in addition to the standard preflight checks",
    )
    parser.add_argument(
        "--root",
        default=".",
        help="Repository root (defaults to the current directory)",
    )
    args = parser.parse_args()

    root = Path(args.root).resolve()
    if not (root / "CalcNova.slnx").is_file():
        print(f"Not a CalcNova repository root: {root}", file=sys.stderr)
        return 2

    all_passed = True
    for label, command in CHECKS:
        all_passed = run_check(root, label, command) and all_passed

    if args.tag:
        all_passed = run_check(
            root,
            "Requested release tag",
            ("tools/validate_release_tag.py", args.tag),
        ) and all_passed

    if not all_passed:
        print("\nCalcNova SDK-independent release preflight FAILED.", file=sys.stderr)
        return 1

    print("\nCalcNova SDK-independent release preflight passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
