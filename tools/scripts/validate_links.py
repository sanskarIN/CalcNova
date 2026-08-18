#!/usr/bin/env python3
"""Validate local Markdown links without making network requests."""

from __future__ import annotations

import re
import sys
from pathlib import Path
from urllib.parse import unquote, urlparse

ROOT = Path(__file__).resolve().parents[2]
MARKDOWN_LINK = re.compile(r"(?<!\!)\[[^\]]*\]\(([^)]+)\)")
RAW_HTML_LINK = re.compile(r"(?:href|src)=[\"']([^\"']+)[\"']", re.IGNORECASE)


def normalize_target(raw: str) -> str:
    target = raw.strip()
    if target.startswith("<") and target.endswith(">"):
        target = target[1:-1].strip()
    if " " in target and not target.startswith(("http://", "https://", "mailto:")):
        target = target.split(" ", 1)[0]
    return unquote(target)


def validate_file(path: Path) -> list[str]:
    text = path.read_text(encoding="utf-8")
    errors: list[str] = []
    candidates = [*MARKDOWN_LINK.findall(text), *RAW_HTML_LINK.findall(text)]

    for raw in candidates:
        target = normalize_target(raw)
        if not target or target.startswith("#"):
            continue

        parsed = urlparse(target)
        if parsed.scheme in {"http", "https", "mailto"}:
            continue
        if parsed.scheme:
            continue

        relative = parsed.path
        if not relative:
            continue

        resolved = (path.parent / relative).resolve()
        try:
            resolved.relative_to(ROOT)
        except ValueError:
            errors.append(f"{path.relative_to(ROOT)}: link escapes repository: {target}")
            continue

        if not resolved.exists():
            errors.append(f"{path.relative_to(ROOT)}: missing local link target: {target}")

    return errors


def main() -> int:
    errors: list[str] = []
    markdown_files = sorted(
        path for path in ROOT.rglob("*.md")
        if ".git" not in path.parts and "bin" not in path.parts and "obj" not in path.parts
    )

    for path in markdown_files:
        try:
            errors.extend(validate_file(path))
        except UnicodeDecodeError as exception:
            errors.append(f"{path.relative_to(ROOT)}: invalid UTF-8: {exception}")

    if errors:
        print("Markdown link validation failed:", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 1

    print(f"Validated {len(markdown_files)} Markdown files; local link targets are present.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
