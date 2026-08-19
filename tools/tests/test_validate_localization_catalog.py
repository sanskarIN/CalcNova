#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_localization_catalog.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_localization_catalog", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load localization validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class LocalizationCatalogValidatorTests(unittest.TestCase):
    def test_repository_localization_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_catalogs_are_registered(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {"English": "EnglishAppStrings.cs", "Hindi": "HindiAppStrings.cs"},
            validator.CATALOG_FILES,
        )

    def test_duplicate_catalog_keys_are_rejected(self) -> None:
        validator = load_validator()
        source = """
            [AppStringKey.AppName] = \"A\",
            [AppStringKey.AppName] = \"B\",
        """
        failures = validator.validate_catalog("Test", ["AppName"], source)
        self.assertTrue(any("duplicate" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
