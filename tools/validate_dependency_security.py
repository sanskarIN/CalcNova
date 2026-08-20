#!/usr/bin/env python3
"""Validate repository-level NuGet vulnerability-audit policy."""

from __future__ import annotations

import argparse
import re
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
AUDIT_WARNING_CODES = {"NU1901", "NU1902", "NU1903", "NU1904"}
SUPPRESSION_PROPERTIES = {"NoWarn", "WarningsNotAsErrors"}


def _local_name(tag: str) -> str:
    return tag.rsplit("}", 1)[-1]


def _warning_tokens(value: str) -> set[str]:
    return {
        token.upper()
        for token in re.split(r"[;,\s]+", value)
        if token and not token.startswith("$(")
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
    suppressed_audit_codes: set[str] = set()

    for element in tree.getroot().iter():
        name = _local_name(element.tag)
        text = (element.text or "").strip()
        if name in values:
            values[name].append(text)
        if name in SUPPRESSION_PROPERTIES:
            suppressed_audit_codes.update(_warning_tokens(text) & AUDIT_WARNING_CODES)

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

    if suppressed_audit_codes:
        failures.append(
            f"{BUILD_PROPS} must not suppress NuGet audit warnings through NoWarn/WarningsNotAsErrors: "
            + ", ".join(sorted(suppressed_audit_codes))
        )

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
