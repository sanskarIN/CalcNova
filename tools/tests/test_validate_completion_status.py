#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_completion_status.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_completion_status", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load completion-status validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class CompletionStatusValidatorTests(unittest.TestCase):
    def test_repository_completion_contract_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_release_identity_constants_are_stable(self) -> None:
        validator = load_validator()
        self.assertEqual("2.8.03", validator.DISPLAY_VERSION)
        self.assertEqual("2.8.3", validator.NORMALIZED_VERSION)
        self.assertEqual("20803", validator.MOBILE_BUILD_CODE)

    def test_authoritative_status_inventory_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {
                "README.md",
                "PROJECT_STATE.md",
                "CHANGELOG.md",
                "what_changed.md",
                "docs/README.md",
                "docs/FEATURES.md",
                "docs/ROADMAP.md",
                "docs/FINAL_SOURCE_AUDIT_2026-08-19.md",
                "docs/VERSIONING.md",
            },
            set(validator.CURRENT_STATUS_CONTRACTS),
        )

    def test_obsolete_status_phrases_remain_forbidden(self) -> None:
        validator = load_validator()
        forbidden = set(validator.FORBIDDEN_CURRENT_STATUS_MARKERS)
        self.assertIn("under active development", forbidden)
        self.assertIn("## [unreleased]", forbidden)
        self.assertIn("remaining product/runtime work", forbidden)
        self.assertIn("remaining high-priority work", forbidden)


if __name__ == "__main__":
    unittest.main()
