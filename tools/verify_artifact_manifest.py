#!/usr/bin/env python3
"""Verify CalcNova release artifacts against a deterministic SHA-256 manifest."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from tools.artifact_manifest import ArtifactRecord, record_file


def load_manifest(path: Path) -> tuple[str, str, tuple[ArtifactRecord, ...]]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exception:
        raise ValueError(f"Could not read artifact manifest: {exception}") from exception

    if data.get("schemaVersion") != 1:
        raise ValueError("Unsupported or missing artifact manifest schemaVersion.")
    repository = data.get("repository")
    commit = data.get("commit")
    artifacts = data.get("artifacts")
    if not isinstance(repository, str) or not repository.strip():
        raise ValueError("Artifact manifest repository is invalid.")
    if not isinstance(commit, str) or not commit.strip():
        raise ValueError("Artifact manifest commit is invalid.")
    if not isinstance(artifacts, list) or not artifacts:
        raise ValueError("Artifact manifest must contain a non-empty artifacts array.")

    records: list[ArtifactRecord] = []
    seen: set[str] = set()
    for item in artifacts:
        if not isinstance(item, dict):
            raise ValueError("Each artifact manifest entry must be an object.")
        path_value = item.get("path")
        size_value = item.get("sizeBytes")
        sha_value = item.get("sha256")
        if not isinstance(path_value, str) or not isinstance(size_value, int) or not isinstance(sha_value, str):
            raise ValueError("Artifact manifest entry has invalid field types.")
        record = ArtifactRecord(path_value, size_value, sha_value)
        record.validate()
        if record.path in seen:
            raise ValueError(f"Duplicate artifact path in manifest: {record.path}")
        seen.add(record.path)
        records.append(record)

    return repository, commit, tuple(records)


def verify_records(root: Path, records: tuple[ArtifactRecord, ...]) -> list[str]:
    failures: list[str] = []
    for expected in records:
        path = root / expected.path
        try:
            actual = record_file(root, path)
        except ValueError as exception:
            failures.append(str(exception))
            continue

        if actual.size_bytes != expected.size_bytes:
            failures.append(
                f"Artifact size mismatch for {expected.path}: expected {expected.size_bytes}, got {actual.size_bytes}."
            )
        if actual.sha256 != expected.sha256:
            failures.append(
                f"Artifact SHA-256 mismatch for {expected.path}: expected {expected.sha256}, got {actual.sha256}."
            )
    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Verify CalcNova artifacts against a manifest.")
    parser.add_argument("manifest", help="Artifact manifest JSON")
    parser.add_argument("--root", default=".", help="Artifact root")
    parser.add_argument("--repository", help="Optional required repository identity")
    parser.add_argument("--commit", help="Optional required commit identity")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    try:
        repository, commit, records = load_manifest(Path(args.manifest))
    except ValueError as exception:
        print(f"Artifact verification failed: {exception}", file=sys.stderr)
        return 2

    failures = verify_records(root, records)
    if args.repository and repository != args.repository:
        failures.append(f"Repository mismatch: expected {args.repository}, manifest records {repository}.")
    if args.commit and commit != args.commit:
        failures.append(f"Commit mismatch: expected {args.commit}, manifest records {commit}.")

    if failures:
        print("Artifact verification failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Verified {len(records)} artifact record(s) for {repository}@{commit}.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
