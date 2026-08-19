#!/usr/bin/env python3
"""Validate CalcNova runtime accessibility evidence status discipline."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

ALLOWED_STATUSES = {"PASS", "FAIL", "BLOCKED", "NOT RUN"}
STATUS_PATTERN = re.compile(r"\b(PASS|FAIL|BLOCKED|NOT RUN)\b")


def validate(root: Path) -> list[str]:
    matrix_path = root / "docs" / "ACCESSIBILITY_TEST_MATRIX.md"
    if not matrix_path.is_file():
        return [f"Missing accessibility evidence matrix: {matrix_path}"]

    source = matrix_path.read_text(encoding="utf-8")
    failures: list[str] = []

    for status in ALLOWED_STATUSES:
        if f"`{status}`" not in source:
            failures.append(f"Evidence rules do not document allowed status: {status}")

    table_rows = [
        line.strip()
        for line in source.splitlines()
        if line.strip().startswith("|") and "---" not in line and "Check" not in line and "Status" not in line
    ]
    evidence_rows = [row for row in table_rows if STATUS_PATTERN.search(row)]
    if len(evidence_rows) < 20:
        failures.append("Accessibility evidence matrix has too few executable evidence rows.")

    for row in evidence_rows:
        cells = [cell.strip() for cell in row.strip("|").split("|")]
        status_cells = [cell for cell in cells[1:] if cell in ALLOWED_STATUSES]
        if not status_cells:
            failures.append(f"Evidence row has no valid status: {row}")

    forbidden_claims = (
        "all accessibility tests passed",
        "fully accessible",
        "100% accessible",
    )
    lowered = source.lower()
    for claim in forbidden_claims:
        if claim in lowered:
            failures.append(f"Accessibility evidence matrix contains an unsupported blanket claim: {claim}")

    for marker in (
        "platform and OS version",
        "commit SHA or release tag",
        "assistive-technology version",
        "linked issue",
    ):
        if marker not in source:
            failures.append(f"Evidence recording guidance is missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate accessibility runtime evidence discipline.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Accessibility evidence validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated accessibility runtime evidence matrix structure and status vocabulary.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
