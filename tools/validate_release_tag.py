#!/usr/bin/env python3
"""Validate CalcNova release tags before expensive build/signing jobs run."""

from __future__ import annotations

import argparse
import re
import sys


_RELEASE_TAG = re.compile(
    r"^v"
    r"(0|[1-9][0-9]*)\."
    r"(0|[1-9][0-9]*)\."
    r"(0|[1-9][0-9]*)"
    r"(?:-(?:"
    r"(?:0|[1-9][0-9]*|[A-Za-z-][0-9A-Za-z-]*)"
    r"(?:\.(?:0|[1-9][0-9]*|[A-Za-z-][0-9A-Za-z-]*))*"
    r"))?"
    r"(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?"
    r"$"
)


def is_valid_release_tag(tag: str) -> bool:
    """Return True for a v-prefixed SemVer 2.0-compatible release tag."""
    return bool(_RELEASE_TAG.fullmatch(tag.strip()))


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Validate a CalcNova release tag such as v1.2.3 or v1.2.3-rc.1."
    )
    parser.add_argument("tag", help="Release tag to validate")
    args = parser.parse_args()

    tag = args.tag.strip()
    if not is_valid_release_tag(tag):
        print(
            f"Invalid CalcNova release tag: {tag!r}. "
            "Expected vMAJOR.MINOR.PATCH with optional SemVer prerelease/build metadata.",
            file=sys.stderr,
        )
        return 1

    print(f"Validated release tag: {tag}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
