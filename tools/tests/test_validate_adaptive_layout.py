#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_adaptive_layout.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_adaptive_layout", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load adaptive layout validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class AdaptiveLayoutValidatorTests(unittest.TestCase):
    def test_repository_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual(0, validator.main())

    def test_all_primary_modes_are_part_of_the_contract(self) -> None:
        validator = load_validator()
        expected = {
            "Calc",
            "Prog",
            "Code",
            "Convert",
            "Stats",
            "Equations",
            "Matrices",
            "Graph",
            "Date",
            "FX",
            "History",
            "Settings",
            "About",
        }
        self.assertEqual(expected, set(validator.REQUIRED_MODE_HEADERS))

    def test_validator_requires_all_three_adaptive_classes(self) -> None:
        validator = load_validator()
        self.assertEqual(("compact", "medium", "expanded"), validator.REQUIRED_STYLE_CLASSES)


if __name__ == "__main__":
    unittest.main()
