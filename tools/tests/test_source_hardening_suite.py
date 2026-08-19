#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
MODULE_PATH = ROOT / "tools" / "source_hardening_suite.py"


def load_suite():
    spec = importlib.util.spec_from_file_location("source_hardening_suite", MODULE_PATH)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load source hardening suite")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class SourceHardeningSuiteTests(unittest.TestCase):
    def test_check_labels_are_unique(self) -> None:
        module = load_suite()
        labels = [label for label, _ in module.CHECKS]
        self.assertEqual(len(labels), len(set(labels)))

    def test_suite_contains_additive_release_critical_gates(self) -> None:
        module = load_suite()
        commands = {arguments for _, arguments in module.CHECKS}
        self.assertIn(("tools/release_preflight.py",), commands)
        self.assertIn(("tools/validate_incomplete_code.py", "."), commands)
        self.assertIn(("tools/validate_dynamic_controls_accessibility.py", "."), commands)


if __name__ == "__main__":
    unittest.main()
