#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_calculator_selection_editing.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_calculator_selection_editing", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load calculator selection-editing validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class CalculatorSelectionEditingValidatorTests(unittest.TestCase):
    def test_repository_selection_editing_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))


if __name__ == "__main__":
    unittest.main()
