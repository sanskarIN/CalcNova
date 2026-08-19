#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_touch_targets.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_touch_targets", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load touch-target validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class TouchTargetValidatorTests(unittest.TestCase):
    def test_repository_touch_targets_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual(0, validator.main())

    def test_minimum_target_is_44_dip(self) -> None:
        validator = load_validator()
        self.assertEqual(44.0, validator.MINIMUM_TARGET)

    def test_min_height_pattern_reads_numeric_values(self) -> None:
        validator = load_validator()
        match = validator.MIN_HEIGHT_PATTERN.search('<Button MinHeight="48" />')
        self.assertIsNotNone(match)
        self.assertEqual("48", match.group("value"))


if __name__ == "__main__":
    unittest.main()
