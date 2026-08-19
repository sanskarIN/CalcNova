#!/usr/bin/env python3
"""Reject release-critical incomplete-code markers in CalcNova source."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

SOURCE_SUFFIXES = {".cs", ".axaml", ".csproj"}
IGNORED_PARTS = {"bin", "obj", ".git"}
PATTERNS: tuple[tuple[str, re.Pattern[str]], ...] = (
    ("TODO marker", re.compile(r"\bTODO\b", re.IGNORECASE)),
    ("FIXME marker", re.compile(r"\bFIXME\b", re.IGNORECASE)),
    ("NotImplementedException", re.compile(r"\bNotImplementedException\b")),
    ("placeholder implementation", re.compile(r"\bplaceholder\s+(?:implementation|code)\b", re.IGNORECASE)),
    ("temporary implementation", re.compile(r"\btemporary\s+implementation\b", re.IGNORECASE)),
)


def validate(root: Path) -> list[str]:
    failures: list[str] = []
    scan_roots = (root / "src", root / "tests")

    for scan_root in scan_roots:
        if not scan_root.is_dir():
            failures.append(f"Missing source tree: {scan_root}")
            continue

        for path in scan_root.rglob("*"):
            if not path.is_file() or path.suffix not in SOURCE_SUFFIXES:
                continue
            if any(part in IGNORED_PARTS for part in path.parts):
                continue

            source = path.read_text(encoding="utf-8")
            for label, pattern in PATTERNS:
                for match in pattern.finditer(source):
                    line = source.count("\n", 0, match.start()) + 1
                    relative = path.relative_to(root)
                    failures.append(f"{relative}:{line}: forbidden {label}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Reject incomplete implementation markers in CalcNova source/tests.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Incomplete-code validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated source/test trees contain no forbidden incomplete-code markers.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
