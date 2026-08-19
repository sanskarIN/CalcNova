#!/usr/bin/env python3

from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

from tools.artifact_manifest import collect_records, manifest_dict, normalize_artifact_path, record_file


class ArtifactManifestTests(unittest.TestCase):
    def test_collect_records_is_sorted_and_hashes_files(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "b.txt").write_text("beta", encoding="utf-8")
            (root / "a.txt").write_text("alpha", encoding="utf-8")

            records = collect_records(root, [Path("b.txt"), Path("a.txt")])

            self.assertEqual(["a.txt", "b.txt"], [record.path for record in records])
            self.assertTrue(all(len(record.sha256) == 64 for record in records))

    def test_directory_collection_recurses_deterministically(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            nested = root / "artifacts" / "nested"
            nested.mkdir(parents=True)
            (nested / "two.bin").write_bytes(b"two")
            (root / "artifacts" / "one.bin").write_bytes(b"one")

            records = collect_records(root, [Path("artifacts")])

            self.assertEqual(
                ["artifacts/nested/two.bin", "artifacts/one.bin"],
                [record.path for record in records],
            )

    def test_duplicate_artifact_paths_are_rejected(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "file.txt").write_text("x", encoding="utf-8")

            with self.assertRaises(ValueError):
                collect_records(root, [Path("file.txt"), Path("file.txt")])

    def test_path_outside_root_is_rejected(self) -> None:
        with tempfile.TemporaryDirectory() as directory, tempfile.TemporaryDirectory() as outside:
            root = Path(directory)
            external = Path(outside) / "file.txt"
            external.write_text("outside", encoding="utf-8")

            with self.assertRaises(ValueError):
                normalize_artifact_path(root, external)

    def test_symbolic_link_artifact_is_rejected(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            target = root / "target.txt"
            link = root / "link.txt"
            target.write_text("target", encoding="utf-8")
            try:
                link.symlink_to(target)
            except (OSError, NotImplementedError):
                self.skipTest("Symbolic links are unavailable on this test host.")

            with self.assertRaises(ValueError):
                record_file(root, link)

    def test_manifest_requires_nonempty_unique_records(self) -> None:
        with self.assertRaises(ValueError):
            manifest_dict("repo/name", "commit", [])


if __name__ == "__main__":
    unittest.main()
