#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_platform_workflows.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_platform_workflows", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load platform workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class PlatformWorkflowValidatorTests(unittest.TestCase):
    def test_repository_platform_workflows_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_platform_workflow_inventory_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {
                ".github/workflows/build-desktop.yml",
                ".github/workflows/build-browser.yml",
                ".github/workflows/build-android.yml",
                ".github/workflows/build-ios.yml",
            },
            set(validator.WORKFLOW_CONTRACTS),
        )

    def test_missing_repository_reports_sdk_and_workflow_failures(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertGreaterEqual(len(failures), len(validator.WORKFLOW_CONTRACTS) + 1)


if __name__ == "__main__":
    unittest.main()
