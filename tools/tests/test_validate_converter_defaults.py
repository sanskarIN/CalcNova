#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_converter_defaults.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_converter_defaults", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load converter default validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ConverterDefaultValidatorTests(unittest.TestCase):
    def test_repository_default_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_category_inventory_matches_current_scope(self) -> None:
        validator = load_validator()
        self.assertEqual(14, len(validator.EXPECTED_CATEGORIES))
        self.assertEqual("Length", validator.EXPECTED_CATEGORIES[0])
        self.assertEqual("Angle", validator.EXPECTED_CATEGORIES[-1])


if __name__ == "__main__":
    unittest.main()
