#!/usr/bin/env python3
"""Validate CalcNova's first-party GitHub Actions hygiene contract."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


WORKFLOW_DIR = Path(".github/workflows")
CANONICAL_WORKFLOWS = {
    "build-test.yml": (
        "name: Build and Test",
        "uses: actions/checkout@v7",
        "uses: actions/setup-dotnet@v6",
        "dotnet-version: 10.0.x",
        "dotnet restore CalcNova.slnx",
        "dotnet build CalcNova.slnx --configuration Release --no-restore",
        "dotnet test CalcNova.slnx --configuration Release --no-build",
        "permissions:\n  contents: read",
    ),
    "format.yml": (
        "name: Formatting",
        "uses: actions/checkout@v7",
        "uses: actions/setup-dotnet@v6",
        "dotnet-version: 10.0.x",
        "dotnet restore CalcNova.slnx",
        "dotnet format CalcNova.slnx --verify-no-changes --no-restore",
        "permissions:\n  contents: read",
    ),
    "docs-check.yml": (
        "name: Documentation Check",
        "uses: actions/checkout@v7",
        "permissions:\n  contents: read",
    ),
}

RETIRED_TEMPLATE_WORKFLOWS = (
    "dotnet.yml",
    "dotnet-desktop.yml",
)

FORBIDDEN_TEMPLATE_MARKERS = (
    "your-solution-name",
    "your-test-project-path",
    "your-wap-project-directory-name",
    "your-wap-project-path",
)

OBSOLETE_ACTION_PATTERNS = (
    re.compile(r"actions/checkout@v[1-5](?:\b|$)"),
    re.compile(r"actions/setup-dotnet@v[1-5](?:\b|$)"),
)


def _workflow_files(root: Path) -> list[Path]:
    directory = root / WORKFLOW_DIR
    if not directory.is_dir():
        return []
    return sorted((*directory.glob("*.yml"), *directory.glob("*.yaml")))


def validate(root: Path) -> list[str]:
    failures: list[str] = []
    workflow_dir = root / WORKFLOW_DIR
    if not workflow_dir.is_dir():
        return [f"Missing GitHub workflow directory: {workflow_dir}"]

    for filename, markers in CANONICAL_WORKFLOWS.items():
        path = workflow_dir / filename
        if not path.is_file():
            failures.append(f"Missing canonical workflow: {path}")
            continue

        text = path.read_text(encoding="utf-8")
        for marker in markers:
            if marker not in text:
                failures.append(f"{filename} is missing CI hygiene marker: {marker}")

    for filename in RETIRED_TEMPLATE_WORKFLOWS:
        path = workflow_dir / filename
        if path.exists():
            failures.append(
                f"Retired generic workflow must not return: {path}. "
                "CalcNova uses its dedicated .NET 10/Avalonia workflows."
            )

    for path in _workflow_files(root):
        text = path.read_text(encoding="utf-8")
        relative = path.relative_to(root)

        for marker in FORBIDDEN_TEMPLATE_MARKERS:
            if marker in text:
                failures.append(f"{relative} contains unresolved starter-template marker: {marker}")

        for pattern in OBSOLETE_ACTION_PATTERNS:
            match = pattern.search(text)
            if match:
                failures.append(f"{relative} uses obsolete action major: {match.group(0)}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova GitHub Actions CI hygiene.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)
    if failures:
        print("CI hygiene validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        "Validated canonical .NET 10 workflows, retired-template absence, "
        "starter-placeholder absence, and minimum checkout/setup-dotnet action majors."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
