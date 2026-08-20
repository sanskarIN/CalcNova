#!/usr/bin/env python3
"""Validate CalcNova security-automation workflow source contracts."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


CODEQL_WORKFLOW = ".github/workflows/codeql.yml"
DEPENDENCY_REVIEW_WORKFLOW = ".github/workflows/dependency-review.yml"


def _require_markers(source: str, path: str, markers: tuple[str, ...], failures: list[str]) -> None:
    for marker in markers:
        if marker not in source:
            failures.append(f"{path} is missing required security marker: {marker}")


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    codeql_path = root / CODEQL_WORKFLOW
    if not codeql_path.is_file():
        failures.append(f"Missing CodeQL workflow: {CODEQL_WORKFLOW}")
    else:
        source = codeql_path.read_text(encoding="utf-8")
        _require_markers(
            source,
            CODEQL_WORKFLOW,
            (
                "push:\n    branches: [main]",
                "pull_request:\n    branches: [main]",
                "schedule:",
                "workflow_dispatch:",
                "contents: read",
                "security-events: write",
                "actions/checkout@v6",
                "github/codeql-action/init@v4",
                "languages: csharp",
                "build-mode: none",
                "github/codeql-action/analyze@v4",
                'category: "/language:csharp"',
            ),
            failures,
        )
        for forbidden in (
            "pull_request_target:",
            "contents: write",
            "id-token: write",
            "actions: write",
            "packages: write",
        ):
            if forbidden in source:
                failures.append(f"{CODEQL_WORKFLOW} contains forbidden privilege/trigger marker: {forbidden}")

    dependency_path = root / DEPENDENCY_REVIEW_WORKFLOW
    if not dependency_path.is_file():
        failures.append(f"Missing dependency-review workflow: {DEPENDENCY_REVIEW_WORKFLOW}")
    else:
        source = dependency_path.read_text(encoding="utf-8")
        _require_markers(
            source,
            DEPENDENCY_REVIEW_WORKFLOW,
            (
                "pull_request:\n    branches: [main]",
                "contents: read",
                "actions/checkout@v6",
                "actions/dependency-review-action@v5",
                "fail-on-severity: moderate",
            ),
            failures,
        )
        for forbidden in (
            "pull_request_target:",
            "contents: write",
            "security-events: write",
            "id-token: write",
            "actions: write",
            "packages: write",
        ):
            if forbidden in source:
                failures.append(
                    f"{DEPENDENCY_REVIEW_WORKFLOW} contains forbidden privilege/trigger marker: {forbidden}"
                )

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova security workflow contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Security workflow validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated CodeQL and dependency-review workflow security contracts.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
