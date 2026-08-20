#!/usr/bin/env python3
"""Validate repository-level NuGet vulnerability-audit policy."""

from __future__ import annotations

import argparse
import sys
import xml.etree.ElementTree as ET
from pathlib import Path


BUILD_PROPS = "Directory.Build.props"
EXPECTED_PROPERTIES = {
    "TreatWarningsAsErrors": "true",
    "NuGetAudit": "true",
    "NuGetAuditMode": "all",
    "NuGetAuditLevel": "moderate",
}


def validate(root: Path) -> list[str]:
    path = root / BUILD_PROPS
    if not path.is_file():
        return [f"Missing dependency security policy source: {BUILD_PROPS}"]

    failures: list[str] = []
    try:
        tree = ET.parse(path)
    except ET.ParseError as exception:
        return [f"Invalid XML in {BUILD_PROPS}: {exception}"]

    values: dict[str, list[str]] = {name: [] for name in EXPECTED_PROPERTIES}
    for element in tree.getroot().iter():
        if element.tag in values:
            values[element.tag].append((element.text or "").strip())

    for name, expected in EXPECTED_PROPERTIES.items():
        observed = values[name]
        if not observed:
            failures.append(f"{BUILD_PROPS} is missing required dependency security property: {name}")
            continue
        if len(observed) != 1:
            failures.append(f"{BUILD_PROPS} must define {name} exactly once; found {len(observed)} definitions")
            continue
        if observed[0].lower() != expected:
            failures.append(
                f"{BUILD_PROPS} must set {name}={expected}; found {observed[0] or '<empty>'}"
            )

    source = path.read_text(encoding="utf-8")
    forbidden_markers = (
        "<NuGetAudit>false</NuGetAudit>",
        "<NuGetAuditMode>direct</NuGetAuditMode>",
        "<WarningsNotAsErrors>NU1901",
        "<NoWarn>NU1901",
        "<NoWarn>NU1902",
        "<NoWarn>NU1903",
        "<NoWarn>NU1904",
    )
    for marker in forbidden_markers:
        if marker in source:
            failures.append(f"{BUILD_PROPS} contains forbidden NuGet-audit weakening marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova NuGet vulnerability-audit policy.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Dependency security validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated transitive NuGet audit, moderate severity threshold, and warnings-as-errors enforcement.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
