#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_accessibility_evidence.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_accessibility_evidence", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load accessibility evidence validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class AccessibilityEvidenceValidatorTests(unittest.TestCase):
    def test_repository_evidence_matrix_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_allowed_status_vocabulary_is_closed(self) -> None:
        validator = load_validator()
        self.assertEqual({"PASS", "FAIL", "BLOCKED", "NOT RUN"}, validator.ALLOWED_STATUSES)


if __name__ == "__main__":
    unittest.main()
