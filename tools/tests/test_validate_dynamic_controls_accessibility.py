#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_dynamic_controls_accessibility.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_dynamic_controls_accessibility", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load dynamic-controls accessibility validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class DynamicControlsAccessibilityValidatorTests(unittest.TestCase):
    def test_repository_dynamic_control_accessibility_contract_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))


if __name__ == "__main__":
    unittest.main()
