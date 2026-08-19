#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_keyboard_contracts.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_keyboard_contracts", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load keyboard contract validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class KeyboardContractValidatorTests(unittest.TestCase):
    def test_repository_keyboard_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_shared_shell_shortcuts_cover_cyclic_and_boundary_navigation(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {
                "PageUp": "PreviousMode",
                "PageDown": "NextMode",
                "Home": "FirstMode",
                "End": "LastMode",
            },
            validator.EXPECTED_SHELL_SHORTCUTS,
        )

    def test_calculator_hardware_mapping_remains_bounded(self) -> None:
        validator = load_validator()
        self.assertEqual(25, len(validator.EXPECTED_KEY_TOKENS))


if __name__ == "__main__":
    unittest.main()
