#!/usr/bin/env python3
"""Verify that requested CalcNova release-evidence checks are explicitly PASS."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

CORE_CHECKS = ("source-hardening", "restore", "format", "build", "test")
SOURCE_CHECKS = ("source-hardening",)
PLATFORM_CHECKS = {
    "desktop": "desktop-publish",
    "browser": "browser-publish",
    "android": "android-build",
    "ios": "ios-simulator-build",
}
VALID_STATUSES = {"PASS", "FAIL", "NOT RUN", "BLOCKED"}


def load_evidence(path: Path) -> dict[str, object]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except (OSError, json.JSONDecodeError) as exception:
        raise ValueError(f"Could not read release evidence: {exception}") from exception

    if data.get("schemaVersion") != 1:
        raise ValueError("Unsupported or missing release-evidence schemaVersion.")
    checks = data.get("checks")
    if not isinstance(checks, list):
        raise ValueError("Release evidence must contain a checks array.")

    seen: set[str] = set()
    for check in checks:
        if not isinstance(check, dict):
            raise ValueError("Every evidence check must be an object.")
        check_id = check.get("id")
        status = check.get("status")
        if not isinstance(check_id, str) or not check_id.strip():
            raise ValueError("Every evidence check must have a non-empty id.")
        if check_id in seen:
            raise ValueError(f"Duplicate evidence check id: {check_id}")
        seen.add(check_id)
        if status not in VALID_STATUSES:
            raise ValueError(f"Evidence check '{check_id}' has invalid status: {status}")

    return data


def required_check_ids(scope: str, platforms: list[str]) -> tuple[str, ...]:
    base = SOURCE_CHECKS if scope == "source" else CORE_CHECKS
    requested = [PLATFORM_CHECKS[platform] for platform in platforms]
    return tuple(dict.fromkeys([*base, *requested]))


def verify(data: dict[str, object], required_ids: tuple[str, ...]) -> list[str]:
    checks = data["checks"]
    assert isinstance(checks, list)
    by_id = {check["id"]: check for check in checks if isinstance(check, dict)}
    failures: list[str] = []

    for check_id in required_ids:
        check = by_id.get(check_id)
        if check is None:
            failures.append(f"Required evidence check is missing: {check_id}")
            continue
        if check.get("status") != "PASS":
            reason = check.get("reason")
            suffix = f" ({reason})" if reason else ""
            failures.append(f"Required evidence check '{check_id}' is {check.get('status')}{suffix}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Require explicit PASS evidence for requested CalcNova gates.")
    parser.add_argument("evidence", help="Path to release-evidence JSON")
    parser.add_argument("--scope", choices=("source", "core"), default="core")
    parser.add_argument(
        "--require-platform",
        action="append",
        choices=tuple(PLATFORM_CHECKS),
        default=[],
        help="Require a platform command to be PASS; may be repeated",
    )
    args = parser.parse_args()

    try:
        data = load_evidence(Path(args.evidence))
        failures = verify(data, required_check_ids(args.scope, args.require_platform))
    except ValueError as exception:
        print(f"Release evidence verification failed: {exception}", file=sys.stderr)
        return 2

    if failures:
        print("Release evidence is incomplete:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Required release-evidence checks are explicit PASS entries.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
