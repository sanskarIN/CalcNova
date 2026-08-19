#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_ios_workflow.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_release_ios_workflow", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load release iOS workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseIosWorkflowValidatorTests(unittest.TestCase):
    def test_repository_release_ios_workflow_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_missing_workflow_is_reported(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertEqual(1, len(failures))


if __name__ == "__main__":
    unittest.main()
