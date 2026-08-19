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

    def test_every_configured_validator_exists(self) -> None:
        for label, arguments in MODULE.CHECKS:
            with self.subTest(label=label):
                self.assertTrue(arguments)
                self.assertTrue((ROOT / arguments[0]).is_file(), arguments[0])

    def test_integrated_inventory_contains_release_critical_contracts(self) -> None:
        configured_paths = {arguments[0] for _, arguments in MODULE.CHECKS}
        expected = {
            "tools/scripts/validate_repository.py",
            "tools/validate_xaml.py",
            "tools/validate_ui_contracts.py",
            "tools/validate_navigation_contracts.py",
            "tools/validate_keyboard_contracts.py",
            "tools/validate_accessibility_markup.py",
            "tools/validate_localization_catalog.py",
            "tools/validate_onboarding_contracts.py",
            "tools/validate_packaging_metadata.py",
            "tools/tests/test_validate_release_tag.py",
        }

        self.assertTrue(expected.issubset(configured_paths))


if __name__ == "__main__":
    unittest.main()
