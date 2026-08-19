#!/usr/bin/env python3
"""Deterministic artifact manifest primitives for CalcNova release outputs."""

from __future__ import annotations

import hashlib
import json
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


@dataclass(frozen=True, order=True)
class ArtifactRecord:
    path: str
    size_bytes: int
    sha256: str

    def validate(self) -> None:
        if not self.path or self.path.startswith("/") or ".." in Path(self.path).parts:
            raise ValueError(f"Artifact path must be a safe relative path: {self.path!r}")
        if self.size_bytes < 0:
            raise ValueError("Artifact size cannot be negative.")
        if len(self.sha256) != 64 or any(character not in "0123456789abcdef" for character in self.sha256):
            raise ValueError(f"Artifact SHA-256 is invalid for {self.path!r}.")


def sha256_file(path: Path, chunk_size: int = 1024 * 1024) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        while chunk := stream.read(chunk_size):
            digest.update(chunk)
    return digest.hexdigest()


def normalize_artifact_path(root: Path, path: Path) -> str:
    root = root.resolve()
    resolved = path.resolve()
    if not resolved.is_relative_to(root):
        raise ValueError(f"Artifact is outside the manifest root: {path}")
    relative = resolved.relative_to(root)
    if not relative.parts:
        raise ValueError("Manifest root itself is not an artifact file.")
    return relative.as_posix()


def record_file(root: Path, path: Path) -> ArtifactRecord:
    if path.is_symlink():
        raise ValueError(f"Symbolic-link artifacts are not allowed: {path}")
    if not path.is_file():
        raise ValueError(f"Artifact file does not exist: {path}")
    record = ArtifactRecord(
        path=normalize_artifact_path(root, path),
        size_bytes=path.stat().st_size,
        sha256=sha256_file(path),
    )
    record.validate()
    return record


def collect_records(root: Path, paths: Iterable[Path]) -> tuple[ArtifactRecord, ...]:
    root = root.resolve()
    records: list[ArtifactRecord] = []
    seen: set[str] = set()

    for input_path in paths:
        path = input_path if input_path.is_absolute() else root / input_path
        candidates = sorted(candidate for candidate in path.rglob("*") if candidate.is_file()) if path.is_dir() else [path]
        for candidate in candidates:
            record = record_file(root, candidate)
            if record.path in seen:
                raise ValueError(f"Duplicate artifact path: {record.path}")
            seen.add(record.path)
            records.append(record)

    return tuple(sorted(records))


def manifest_dict(repository: str, commit: str, records: Iterable[ArtifactRecord]) -> dict[str, object]:
    if not repository.strip() or not commit.strip():
        raise ValueError("Repository and commit must be non-empty.")
    items = tuple(sorted(records))
    if not items:
        raise ValueError("Artifact manifest cannot be empty.")
    paths = [record.path for record in items]
    if len(paths) != len(set(paths)):
        raise ValueError("Artifact manifest paths must be unique.")
    for record in items:
        record.validate()
    return {
        "schemaVersion": 1,
        "repository": repository,
        "commit": commit,
        "artifacts": [
            {
                "path": record.path,
                "sizeBytes": record.size_bytes,
                "sha256": record.sha256,
            }
            for record in items
        ],
    }


def write_manifest(path: Path, repository: str, commit: str, records: Iterable[ArtifactRecord]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(manifest_dict(repository, commit, records), indent=2) + "\n", encoding="utf-8")
