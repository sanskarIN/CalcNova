#!/usr/bin/env python3
"""Validate CalcNova release documentation/evidence contracts without SDKs."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


REQUIRED_MARKERS: dict[str, tuple[str, ...]] = {
    "docs/RELEASE.md": (
        "python tools/release_preflight.py --tag v0.1.0",
        "Manual release dispatch must reference an already-existing valid tag",
        "preserve the existing release/notes",
        "PASS / FAIL / BLOCKED / NOT RUN",
        "ACCESSIBILITY_TEST_MATRIX.md",
        "SETTINGS_MIGRATION.md",
    ),
    "docs/RELEASE_READINESS_CHECKLIST.md": (
        "Source preflight: PASS / FAIL / BLOCKED / NOT RUN",
        ".NET restore/format/build/test: PASS / FAIL / BLOCKED / NOT RUN",
        "Windows: PASS / FAIL / BLOCKED / NOT RUN",
        "Linux: PASS / FAIL / BLOCKED / NOT RUN",
        "macOS: PASS / FAIL / BLOCKED / NOT RUN",
        "Browser: PASS / FAIL / BLOCKED / NOT RUN",
        "Android: PASS / FAIL / BLOCKED / NOT RUN",
        "iOS: PASS / FAIL / BLOCKED / NOT RUN",
        "Accessibility audit: PASS / FAIL / BLOCKED / NOT RUN",
        "Responsive-layout audit: PASS / FAIL / BLOCKED / NOT RUN",
        "Settings-schema migration contracts pass.",
        "ACCESSIBILITY_TEST_MATRIX.md",
        "Never replace `NOT RUN` with `PASS`",
    ),
    "PROJECT_STATE.md": (
        "## Validation Status",
        "**NOT RUN**",
        "A check is never marked PASS unless",
    ),
    "what_changed.md": (
        "# What Changed",
        "Validation Status",
        "**NOT RUN**",
    ),
}


def validate(root: Path) -> list[str]:
    failures: list[str] = []
    for relative_path, markers in REQUIRED_MARKERS.items():
        path = root / relative_path
        if not path.is_file():
            failures.append(f"Missing release evidence document: {relative_path}")
            continue

        source = path.read_text(encoding="utf-8")
        for marker in markers:
            if marker not in source:
                failures.append(f"{relative_path} is missing required release-evidence marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Validate CalcNova release-readiness documentation contracts."
    )
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Release documentation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    checked_markers = sum(len(markers) for markers in REQUIRED_MARKERS.values())
    print(
        f"Validated {checked_markers} release evidence markers across "
        f"{len(REQUIRED_MARKERS)} documentation/state files."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
