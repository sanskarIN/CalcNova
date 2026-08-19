#!/usr/bin/env python3

from __future__ import annotations

import json
import tempfile
import unittest
from pathlib import Path

from tools.verify_release_evidence import load_evidence, required_check_ids, verify


class VerifyReleaseEvidenceTests(unittest.TestCase):
    def test_core_requirements_include_source_restore_format_build_test(self) -> None:
        self.assertEqual(
            ("source-hardening", "restore", "format", "build", "test"),
            required_check_ids("core", []),
        )

    def test_platform_requirement_is_appended_without_duplicates(self) -> None:
        ids = required_check_ids("source", ["browser", "browser", "android"])
        self.assertEqual(("source-hardening", "browser-publish", "android-build"), ids)

    def test_verify_accepts_explicit_pass_entries(self) -> None:
        data = {
            "checks": [
                {"id": "source-hardening", "status": "PASS"},
                {"id": "restore", "status": "PASS"},
            ]
        }

        self.assertEqual([], verify(data, ("source-hardening", "restore")))

    def test_verify_rejects_missing_blocked_and_not_run_required_entries(self) -> None:
        data = {
            "checks": [
                {"id": "source-hardening", "status": "PASS"},
                {"id": "restore", "status": "BLOCKED", "reason": "No SDK"},
                {"id": "browser-publish", "status": "NOT RUN", "reason": "Not requested"},
            ]
        }

        failures = verify(data, ("source-hardening", "restore", "browser-publish", "missing"))

        self.assertEqual(3, len(failures))
        self.assertTrue(any("restore" in failure and "BLOCKED" in failure for failure in failures))
        self.assertTrue(any("browser-publish" in failure and "NOT RUN" in failure for failure in failures))
        self.assertTrue(any("missing" in failure for failure in failures))

    def test_load_evidence_rejects_invalid_status_and_duplicate_ids(self) -> None:
        for checks in (
            [{"id": "one", "status": "MAYBE"}],
            [{"id": "same", "status": "PASS"}, {"id": "same", "status": "PASS"}],
        ):
            with self.subTest(checks=checks), tempfile.TemporaryDirectory() as directory:
                path = Path(directory) / "evidence.json"
                path.write_text(json.dumps({"schemaVersion": 1, "checks": checks}), encoding="utf-8")
                with self.assertRaises(ValueError):
                    load_evidence(path)

    def test_load_evidence_requires_schema_version_one(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "evidence.json"
            path.write_text(json.dumps({"schemaVersion": 2, "checks": []}), encoding="utf-8")
            with self.assertRaises(ValueError):
                load_evidence(path)


if __name__ == "__main__":
    unittest.main()
