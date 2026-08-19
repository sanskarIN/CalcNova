#!/usr/bin/env python3

from __future__ import annotations

import json
import tempfile
import unittest
from pathlib import Path

from tools.artifact_manifest import collect_records, write_manifest
from tools.verify_artifact_manifest import load_manifest, verify_records


class VerifyArtifactManifestTests(unittest.TestCase):
    def test_generated_manifest_verifies_unchanged_artifact(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            artifact = root / "app.zip"
            manifest = root / "manifest.json"
            artifact.write_bytes(b"calc-nova")
            write_manifest(manifest, "sanskarIN/CalcNova", "abc123", collect_records(root, [artifact]))

            repository, commit, records = load_manifest(manifest)
            failures = verify_records(root, records)

            self.assertEqual("sanskarIN/CalcNova", repository)
            self.assertEqual("abc123", commit)
            self.assertEqual([], failures)

    def test_changed_artifact_is_detected_by_size_and_hash(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            artifact = root / "app.zip"
            manifest = root / "manifest.json"
            artifact.write_bytes(b"original")
            write_manifest(manifest, "repo/name", "commit", collect_records(root, [artifact]))
            _, _, records = load_manifest(manifest)
            artifact.write_bytes(b"tampered-content")

            failures = verify_records(root, records)

            self.assertTrue(any("size mismatch" in failure for failure in failures))
            self.assertTrue(any("SHA-256 mismatch" in failure for failure in failures))

    def test_missing_artifact_is_detected(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            artifact = root / "app.zip"
            manifest = root / "manifest.json"
            artifact.write_bytes(b"original")
            write_manifest(manifest, "repo/name", "commit", collect_records(root, [artifact]))
            _, _, records = load_manifest(manifest)
            artifact.unlink()

            failures = verify_records(root, records)

            self.assertTrue(any("does not exist" in failure for failure in failures))

    def test_manifest_rejects_duplicate_paths(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "manifest.json"
            record = {"path": "app.zip", "sizeBytes": 1, "sha256": "0" * 64}
            path.write_text(
                json.dumps(
                    {
                        "schemaVersion": 1,
                        "repository": "repo/name",
                        "commit": "commit",
                        "artifacts": [record, record],
                    }
                ),
                encoding="utf-8",
            )

            with self.assertRaises(ValueError):
                load_manifest(path)

    def test_manifest_rejects_unsafe_relative_path(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "manifest.json"
            path.write_text(
                json.dumps(
                    {
                        "schemaVersion": 1,
                        "repository": "repo/name",
                        "commit": "commit",
                        "artifacts": [
                            {"path": "../escape.zip", "sizeBytes": 1, "sha256": "0" * 64}
                        ],
                    }
                ),
                encoding="utf-8",
            )

            with self.assertRaises(ValueError):
                load_manifest(path)


if __name__ == "__main__":
    unittest.main()
