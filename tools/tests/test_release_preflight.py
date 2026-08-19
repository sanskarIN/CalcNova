#!/usr/bin/env python3
"""Contract tests for the integrated SDK-independent CalcNova preflight."""

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[2]
MODULE_PATH = ROOT / "tools" / "release_preflight.py"
SPEC = importlib.util.spec_from_file_location("release_preflight", MODULE_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Could not load {MODULE_PATH}")

MODULE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(MODULE)


class ReleasePreflightContractTests(unittest.TestCase):
    def test_check_labels_are_unique(self) -> None:
        labels = [label for label, _ in MODULE.CHECKS]
        self.assertEqual(len(labels), len(set(labels)))

    def test_every_configured_validator_or_test_module_exists(self) -> None:
        for label, arguments in MODULE.CHECKS:
            with self.subTest(label=label):
                self.assertTrue(arguments)
                if arguments[0] == "-m":
                    self.assertGreaterEqual(len(arguments), 3)
                    self.assertEqual("unittest", arguments[1])
                    module_path = ROOT / (arguments[2].replace(".", "/") + ".py")
                    self.assertTrue(module_path.is_file(), module_path)
                else:
                    self.assertTrue((ROOT / arguments[0]).is_file(), arguments[0])

    def test_integrated_inventory_contains_release_critical_contracts(self) -> None:
        configured_files = {arguments[0] for _, arguments in MODULE.CHECKS if arguments[0] != "-m"}
        expected = {
            "tools/scripts/validate_repository.py",
            "tools/validate_xaml.py",
            "tools/validate_ui_contracts.py",
            "tools/validate_navigation_contracts.py",
            "tools/validate_keyboard_contracts.py",
            "tools/validate_calculator_selection_editing.py",
            "tools/validate_graph_keyboard.py",
            "tools/validate_graph_surface.py",
            "tools/validate_graph_series_presentation.py",
            "tools/validate_numerical_analysis.py",
            "tools/validate_graph_numerical_budgets.py",
            "tools/validate_unicode_metadata.py",
            "tools/validate_rational_numbers.py",
            "tools/validate_engineering_notation.py",
            "tools/validate_export_previews.py",
            "tools/validate_bivariate_statistics.py",
            "tools/validate_headless_ui_tests.py",
            "tools/validate_accessibility_markup.py",
            "tools/validate_focus_visibility.py",
            "tools/validate_accessibility_evidence.py",
            "tools/validate_dynamic_controls_accessibility.py",
            "tools/validate_adaptive_layout.py",
            "tools/validate_touch_targets.py",
            "tools/validate_localization_catalog.py",
            "tools/validate_converter_defaults.py",
            "tools/validate_converter_preference_notice.py",
            "tools/validate_settings_schema.py",
            "tools/validate_onboarding_contracts.py",
            "tools/validate_packaging_metadata.py",
            "tools/validate_platform_workflows.py",
            "tools/validate_source_preflight_workflow.py",
            "tools/validate_release_ios_workflow.py",
            "tools/validate_release_workflow.py",
            "tools/validate_release_docs.py",
            "tools/validate_artifact_integrity.py",
            "tools/validate_release_evidence_infrastructure.py",
            "tools/tests/test_validate_release_tag.py",
        }
        self.assertTrue(expected.issubset(configured_files))

    def test_preflight_runs_regression_suites_for_source_validators(self) -> None:
        configured_modules = {
            arguments[2]
            for _, arguments in MODULE.CHECKS
            if len(arguments) >= 3 and arguments[0:2] == ("-m", "unittest")
        }
        expected_modules = {
            "tools.tests.test_validate_release_workflow",
            "tools.tests.test_validate_release_docs",
            "tools.tests.test_validate_source_preflight_workflow",
            "tools.tests.test_validate_release_ios_workflow",
            "tools.tests.test_validate_headless_ui_tests",
            "tools.tests.test_validate_focus_visibility",
            "tools.tests.test_validate_accessibility_evidence",
            "tools.tests.test_validate_dynamic_controls_accessibility",
            "tools.tests.test_validate_keyboard_contracts",
            "tools.tests.test_validate_calculator_selection_editing",
            "tools.tests.test_validate_graph_keyboard",
            "tools.tests.test_validate_graph_surface",
            "tools.tests.test_validate_graph_series_presentation",
            "tools.tests.test_validate_numerical_analysis",
            "tools.tests.test_validate_graph_numerical_budgets",
            "tools.tests.test_validate_unicode_metadata",
            "tools.tests.test_validate_rational_numbers",
            "tools.tests.test_validate_engineering_notation",
            "tools.tests.test_validate_export_previews",
            "tools.tests.test_validate_bivariate_statistics",
            "tools.tests.test_validate_localization_catalog",
            "tools.tests.test_validate_converter_defaults",
            "tools.tests.test_validate_converter_preference_notice",
            "tools.tests.test_validate_settings_schema",
            "tools.tests.test_validate_adaptive_layout",
            "tools.tests.test_validate_touch_targets",
            "tools.tests.test_validate_packaging_metadata",
            "tools.tests.test_validate_platform_workflows",
            "tools.tests.test_artifact_manifest",
            "tools.tests.test_verify_artifact_manifest",
            "tools.tests.test_validate_artifact_integrity",
            "tools.tests.test_release_evidence",
            "tools.tests.test_run_release_evidence",
            "tools.tests.test_verify_release_evidence",
            "tools.tests.test_validate_release_evidence_infrastructure",
        }
        self.assertTrue(expected_modules.issubset(configured_modules))


if __name__ == "__main__":
    unittest.main()
