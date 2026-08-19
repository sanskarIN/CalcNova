#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_settings_schema.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_settings_schema", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load settings schema validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class SettingsSchemaValidatorTests(unittest.TestCase):
    def test_repository_settings_schema_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_current_schema_version_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(1, validator.CURRENT_SCHEMA_VERSION)


if __name__ == "__main__":
    unittest.main()
