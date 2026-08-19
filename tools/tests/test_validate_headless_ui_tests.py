#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_headless_ui_tests.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_headless_ui_tests", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load headless UI-test validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class HeadlessUiTestValidatorTests(unittest.TestCase):
    def test_repository_headless_ui_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_avalonia_version_matches_repository_contract(self) -> None:
        validator = load_validator()
        self.assertEqual("12.1.1", validator.EXPECTED_AVALONIA_VERSION)


if __name__ == "__main__":
    unittest.main()
