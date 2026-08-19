#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_packaging_metadata.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_packaging_metadata", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load packaging metadata validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class PackagingMetadataValidatorTests(unittest.TestCase):
    def test_repository_packaging_metadata_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_release_identity_constants_are_stable(self) -> None:
        validator = load_validator()
        self.assertEqual("in.sanskar.calcnova", validator.APP_ID)
        self.assertEqual("CalcNova", validator.APP_NAME)
        self.assertEqual("2.8.03", validator.DISPLAY_VERSION)
        self.assertEqual("2.8.3", validator.SEMVER_VERSION)
        self.assertEqual("20803", validator.MOBILE_BUILD_CODE)

    def test_linux_appstream_records_completed_release(self) -> None:
        source = (ROOT / "packaging" / "linux" / "in.sanskar.calcnova.metainfo.xml").read_text(
            encoding="utf-8"
        )
        self.assertIn('<release version="2.8.03" date="2026-08-19" type="stable">', source)
        self.assertIn("CalcNova 2.8.03 completed cross-platform product baseline.", source)

    def test_missing_metadata_is_reported(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertTrue(any("Missing packaging metadata file" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
