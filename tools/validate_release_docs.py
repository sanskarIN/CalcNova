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
        "PASS / FAIL / NOT RUN",
    ),
    "docs/RELEASE_READINESS_CHECKLIST.md": (
        "Source preflight: PASS / FAIL / NOT RUN",
        ".NET restore/format/build/test: PASS / FAIL / NOT RUN",
        "Windows: PASS / FAIL / NOT RUN",
        "Linux: PASS / FAIL / NOT RUN",
        "macOS: PASS / FAIL / NOT RUN",
        "Browser: PASS / FAIL / NOT RUN",
        "Android: PASS / FAIL / NOT RUN",
        "iOS: PASS / FAIL / NOT RUN",
        "Accessibility audit: PASS / FAIL / NOT RUN",
        "Responsive-layout audit: PASS / FAIL / NOT RUN",
        "Never replace `NOT RUN` with `PASS`",
    ),
    "PROJECT_STATE.md": (
        "## Current validation evidence",
        "**NOT RUN**",
        "GitHub Actions result for the latest direct-push checkpoint",
        "never convert unexecuted checks into PASS",
    ),
    "what_changed.md": (
        "## Validation boundary for this continuation",
        "**NOT RUN**",
        "Their existence is not treated here as proof that GitHub Actions has passed.",
    ),
}


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Validate CalcNova release-readiness documentation contracts."
    )
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures: list[str] = []
    checked_markers = 0

    for relative_path, markers in REQUIRED_MARKERS.items():
        path = root / relative_path
        if not path.is_file():
            failures.append(f"Missing release evidence document: {relative_path}")
            continue

        source = path.read_text(encoding="utf-8")
        for marker in markers:
            checked_markers += 1
            if marker not in source:
                failures.append(f"{relative_path} is missing required release-evidence marker: {marker}")

    if failures:
        print("Release documentation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        f"Validated {checked_markers} release evidence markers across "
        f"{len(REQUIRED_MARKERS)} documentation/state files."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
