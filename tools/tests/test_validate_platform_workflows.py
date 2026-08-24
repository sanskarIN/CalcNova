#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import shutil
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


def copy_fixture(destination: Path, validator) -> None:
    shutil.copy2(ROOT / "global.json", destination / "global.json")
    for relative_path in validator.WORKFLOW_CONTRACTS:
        source = ROOT / relative_path
        target = destination / relative_path
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(source, target)


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

    def test_checkout_v6_regression_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            copy_fixture(root, validator)
            workflow = root / ".github/workflows/build-desktop.yml"
            source = workflow.read_text(encoding="utf-8").replace(
                "actions/checkout@v7",
                "actions/checkout@v6",
            )
            workflow.write_text(source, encoding="utf-8")

            failures = validator.validate(root)

        self.assertTrue(any("actions/checkout@v7" in failure for failure in failures), failures)

    def test_missing_repository_reports_sdk_and_workflow_failures(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertGreaterEqual(len(failures), len(validator.WORKFLOW_CONTRACTS) + 1)


if __name__ == "__main__":
    unittest.main()
