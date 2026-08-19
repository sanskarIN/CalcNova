#!/usr/bin/env python3
"""Validate CalcNova Avalonia XAML files as well-formed XML.

This is intentionally a lightweight preflight check. It catches malformed XML,
truncated XAML, duplicate XML attributes, and invalid entity usage before the
full Avalonia/.NET build runs. It does not replace Avalonia XAML compilation.
"""

from __future__ import annotations

import argparse
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


def find_xaml_files(root: Path) -> list[Path]:
    return sorted(
        path
        for path in root.rglob("*.axaml")
        if not any(part in {"bin", "obj", ".git"} for part in path.parts)
    )


def validate_file(path: Path) -> str | None:
    try:
        ET.parse(path)
    except (ET.ParseError, OSError) as exc:
        return f"{path}: {exc}"
    return None


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova .axaml files as XML.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root (default: current directory)")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    files = find_xaml_files(root)
    if not files:
        print("No .axaml files found.", file=sys.stderr)
        return 2

    failures = [error for path in files if (error := validate_file(path)) is not None]
    if failures:
        print("Avalonia XAML XML validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated {len(files)} Avalonia XAML file(s) as well-formed XML.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
