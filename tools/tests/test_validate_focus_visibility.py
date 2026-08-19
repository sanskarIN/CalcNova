#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / 'tools' / 'validate_focus_visibility.py'


def load_validator():
    spec = importlib.util.spec_from_file_location('validate_focus_visibility', VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError('Unable to load focus visibility validator')
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class FocusVisibilityValidatorTests(unittest.TestCase):
    def test_repository_focus_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_shared_control_coverage_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(6, len(validator.BASE_FOCUS_SELECTORS))
        self.assertEqual(6, len(validator.HIGH_CONTRAST_FOCUS_SELECTORS))


if __name__ == '__main__':
    unittest.main()
