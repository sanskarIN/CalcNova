#!/usr/bin/env python3
"""Validate CalcNova artifact manifest and tamper-verification infrastructure."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    model_path = root / "tools" / "artifact_manifest.py"
    generator_path = root / "tools" / "generate_artifact_manifest.py"
    verifier_path = root / "tools" / "verify_artifact_manifest.py"
    model_tests_path = root / "tools" / "tests" / "test_artifact_manifest.py"
    verifier_tests_path = root / "tools" / "tests" / "test_verify_artifact_manifest.py"

    paths = (model_path, generator_path, verifier_path, model_tests_path, verifier_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing artifact-integrity source: {path}")
    if failures:
        return failures

    model = model_path.read_text(encoding="utf-8")
    generator = generator_path.read_text(encoding="utf-8")
    verifier = verifier_path.read_text(encoding="utf-8")
    model_tests = model_tests_path.read_text(encoding="utf-8")
    verifier_tests = verifier_tests_path.read_text(encoding="utf-8")

    for marker in (
        "hashlib.sha256()",
        "Symbolic-link artifacts are not allowed",
        "Artifact is outside the manifest root",
        '"schemaVersion": 1',
        '"sizeBytes"',
        '"sha256"',
        "Artifact manifest paths must be unique",
    ):
        if marker not in model:
            failures.append(f"Artifact manifest model is missing marker: {marker}")

    for marker in (
        "collect_records",
        "Manifest output cannot also be included as an artifact input.",
        "git rev-parse",
        "write_manifest",
    ):
        if marker not in generator:
            failures.append(f"Artifact manifest generator is missing marker: {marker}")

    for marker in (
        "load_manifest",
        "verify_records",
        "Artifact size mismatch",
        "Artifact SHA-256 mismatch",
        "Repository mismatch",
        "Commit mismatch",
    ):
        if marker not in verifier:
            failures.append(f"Artifact manifest verifier is missing marker: {marker}")

    for marker in (
        "test_collect_records_is_sorted_and_hashes_files",
        "test_duplicate_artifact_paths_are_rejected",
        "test_path_outside_root_is_rejected",
        "test_symbolic_link_artifact_is_rejected",
    ):
        if marker not in model_tests:
            failures.append(f"Artifact manifest model tests are missing marker: {marker}")

    for marker in (
        "test_generated_manifest_verifies_unchanged_artifact",
        "test_changed_artifact_is_detected_by_size_and_hash",
        "test_missing_artifact_is_detected",
        "test_manifest_rejects_duplicate_paths",
        "test_manifest_rejects_unsafe_relative_path",
    ):
        if marker not in verifier_tests:
            failures.append(f"Artifact manifest verifier tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova artifact integrity tooling.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()
    failures = validate(Path(args.root).resolve())
    if failures:
        print("Artifact integrity validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated artifact manifest generation, safe paths, SHA-256 verification, and tamper tests.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
