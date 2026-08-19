#!/usr/bin/env python3
"""Structured release-validation evidence primitives for CalcNova."""

from __future__ import annotations

import dataclasses
import datetime as dt
import json
import platform
from enum import StrEnum
from pathlib import Path
from typing import Iterable


class EvidenceStatus(StrEnum):
    PASS = "PASS"
    FAIL = "FAIL"
    NOT_RUN = "NOT RUN"
    BLOCKED = "BLOCKED"


@dataclasses.dataclass(frozen=True)
class EvidenceCheck:
    id: str
    label: str
    status: EvidenceStatus
    command: str | None = None
    exit_code: int | None = None
    reason: str | None = None
    duration_seconds: float | None = None
    log_file: str | None = None

    def validate(self) -> None:
        if not self.id.strip():
            raise ValueError("Evidence check id cannot be empty.")
        if not self.label.strip():
            raise ValueError("Evidence check label cannot be empty.")
        if self.status is EvidenceStatus.PASS and self.exit_code not in (None, 0):
            raise ValueError("PASS evidence cannot carry a non-zero exit code.")
        if self.status is EvidenceStatus.FAIL and self.exit_code in (None, 0):
            raise ValueError("FAIL evidence must carry a non-zero exit code.")
        if self.status in (EvidenceStatus.NOT_RUN, EvidenceStatus.BLOCKED) and not self.reason:
            raise ValueError(f"{self.status} evidence must include a reason.")
        if self.duration_seconds is not None and self.duration_seconds < 0:
            raise ValueError("Evidence duration cannot be negative.")


@dataclasses.dataclass(frozen=True)
class ReleaseEvidence:
    repository: str
    commit: str
    generated_at_utc: str
    host_os: str
    host_architecture: str
    checks: tuple[EvidenceCheck, ...]

    def validate(self) -> None:
        if not self.repository.strip():
            raise ValueError("Repository name cannot be empty.")
        if not self.commit.strip():
            raise ValueError("Commit cannot be empty.")
        ids = [check.id for check in self.checks]
        if len(ids) != len(set(ids)):
            raise ValueError("Evidence check ids must be unique.")
        for check in self.checks:
            check.validate()

    def to_dict(self) -> dict[str, object]:
        self.validate()
        return {
            "schemaVersion": 1,
            "repository": self.repository,
            "commit": self.commit,
            "generatedAtUtc": self.generated_at_utc,
            "host": {
                "os": self.host_os,
                "architecture": self.host_architecture,
            },
            "checks": [
                {
                    "id": check.id,
                    "label": check.label,
                    "status": check.status.value,
                    **({"command": check.command} if check.command else {}),
                    **({"exitCode": check.exit_code} if check.exit_code is not None else {}),
                    **({"reason": check.reason} if check.reason else {}),
                    **(
                        {"durationSeconds": round(check.duration_seconds, 3)}
                        if check.duration_seconds is not None
                        else {}
                    ),
                    **({"logFile": check.log_file} if check.log_file else {}),
                }
                for check in self.checks
            ],
        }

    def write_json(self, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(self.to_dict(), indent=2) + "\n", encoding="utf-8")


def create_evidence(repository: str, commit: str, checks: Iterable[EvidenceCheck]) -> ReleaseEvidence:
    now = dt.datetime.now(dt.timezone.utc).replace(microsecond=0).isoformat().replace("+00:00", "Z")
    evidence = ReleaseEvidence(
        repository=repository,
        commit=commit,
        generated_at_utc=now,
        host_os=platform.system() or "Unknown",
        host_architecture=platform.machine() or "Unknown",
        checks=tuple(checks),
    )
    evidence.validate()
    return evidence
