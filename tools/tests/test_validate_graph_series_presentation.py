#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_graph_series_presentation.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_graph_series_presentation", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load graph series presentation validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class GraphSeriesPresentationValidatorTests(unittest.TestCase):
    def test_repository_series_presentation_contract_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_pattern_inventory_is_eight_and_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(8, len(validator.EXPECTED_PATTERNS))
        self.assertEqual("Solid", validator.EXPECTED_PATTERNS[0])
        self.assertEqual("AlternatingDash", validator.EXPECTED_PATTERNS[-1])


if __name__ == "__main__":
    unittest.main()
