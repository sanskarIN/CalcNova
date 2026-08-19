#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_workflow.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_release_workflow", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load release workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseWorkflowValidatorTests(unittest.TestCase):
    def test_repository_release_workflow_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))


if __name__ == "__main__":
    unittest.main()
