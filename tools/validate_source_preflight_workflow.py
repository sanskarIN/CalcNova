#!/usr/bin/env python3
"""Validate the integrated Source Preflight workflow contract."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


WORKFLOW_PATH = Path(".github/workflows/source-preflight.yml")

REQUIRED_MARKERS = (
    "name: Source Preflight",
    "push:\n    branches: [main]",
    "pull_request:\n    branches: [main]",
    "workflow_dispatch:",
    "permissions:",
    "contents: read",
    "concurrency:",
    "cancel-in-progress: true",
    "runs-on: ubuntu-latest",
    "timeout-minutes: 8",
    "uses: actions/checkout@v6",
    "uses: actions/setup-python@v6",
    'python-version: "3.13"',
    "run: python tools/release_preflight.py",
)

FORBIDDEN_MARKERS = (
    "pull_request_target:",
    "contents: write",
    "actions: write",
    "    paths:",
    "    paths-ignore:",
)


def validate(root: Path) -> list[str]:
    workflow = root / WORKFLOW_PATH
    if not workflow.is_file():
        return [f"Missing source preflight workflow: {workflow}"]

    text = workflow.read_text(encoding="utf-8")
    failures: list[str] = []

    for marker in REQUIRED_MARKERS:
        if marker not in text:
            failures.append(f"Source preflight workflow is missing contract marker: {marker}")

    for marker in FORBIDDEN_MARKERS:
        if marker in text:
            failures.append(f"Source preflight workflow contains forbidden marker: {marker}")

    if text.count("branches: [main]") < 2:
        failures.append("Source preflight must target main for both push and pull_request.")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova Source Preflight workflow contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Source preflight workflow validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated always-run Source Preflight PR/push triggers, least privilege, concurrency, runner/toolchain, and integrated command.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
