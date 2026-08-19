#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_incomplete_code.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_incomplete_code", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load incomplete-code validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class IncompleteCodeValidatorTests(unittest.TestCase):
    def test_repository_has_no_forbidden_incomplete_markers(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_validator_detects_todo_and_not_implemented(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "src").mkdir()
            (root / "tests").mkdir()
            (root / "src" / "Example.cs").write_text(
                "// TODO finish\nthrow new NotImplementedException();\n",
                encoding="utf-8",
            )

            failures = validator.validate(root)

            self.assertTrue(any("TODO marker" in failure for failure in failures))
            self.assertTrue(any("NotImplementedException" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
