#!/usr/bin/env python3

from __future__ import annotations

import json
import tempfile
import unittest
from pathlib import Path

from tools.release_evidence import EvidenceCheck, EvidenceStatus, create_evidence


class ReleaseEvidenceTests(unittest.TestCase):
    def test_valid_evidence_serializes_stable_schema(self) -> None:
        evidence = create_evidence(
            "sanskarIN/CalcNova",
            "abc123",
            [
                EvidenceCheck(
                    id="source-preflight",
                    label="Source preflight",
                    status=EvidenceStatus.PASS,
                    command="python tools/release_preflight.py",
                    exit_code=0,
                    duration_seconds=1.25,
                ),
                EvidenceCheck(
                    id="ios-runtime",
                    label="iOS runtime",
                    status=EvidenceStatus.NOT_RUN,
                    reason="No iOS target was requested.",
                ),
            ],
        )

        data = evidence.to_dict()

        self.assertEqual(1, data["schemaVersion"])
        self.assertEqual("sanskarIN/CalcNova", data["repository"])
        self.assertEqual("abc123", data["commit"])
        self.assertEqual("PASS", data["checks"][0]["status"])
        self.assertEqual("NOT RUN", data["checks"][1]["status"])

    def test_duplicate_check_ids_are_rejected(self) -> None:
        with self.assertRaises(ValueError):
            create_evidence(
                "repo/name",
                "commit",
                [
                    EvidenceCheck("same", "One", EvidenceStatus.PASS, exit_code=0),
                    EvidenceCheck("same", "Two", EvidenceStatus.PASS, exit_code=0),
                ],
            )

    def test_fail_requires_nonzero_exit_code(self) -> None:
        check = EvidenceCheck("build", "Build", EvidenceStatus.FAIL)
        with self.assertRaises(ValueError):
            check.validate()

    def test_not_run_and_blocked_require_reasons(self) -> None:
        for status in (EvidenceStatus.NOT_RUN, EvidenceStatus.BLOCKED):
            with self.subTest(status=status):
                check = EvidenceCheck("target", "Target", status)
                with self.assertRaises(ValueError):
                    check.validate()

    def test_write_json_creates_parent_directory_and_valid_json(self) -> None:
        evidence = create_evidence(
            "repo/name",
            "commit",
            [EvidenceCheck("test", "Tests", EvidenceStatus.PASS, exit_code=0)],
        )
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / "nested" / "evidence.json"

            evidence.write_json(output)

            data = json.loads(output.read_text(encoding="utf-8"))
            self.assertEqual("PASS", data["checks"][0]["status"])


if __name__ == "__main__":
    unittest.main()
