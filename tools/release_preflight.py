#!/usr/bin/env python3
"""Run CalcNova's deterministic release-source checks without a .NET SDK."""

from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path


CHECKS: tuple[tuple[str, tuple[str, ...]], ...] = (
    ("Repository structure/security", ("tools/scripts/validate_repository.py",)),
    ("Avalonia XAML XML", ("tools/validate_xaml.py", ".")),
    ("Shared UI contracts", ("tools/validate_ui_contracts.py", ".")),
    ("Navigation contracts", ("tools/validate_navigation_contracts.py", ".")),
    ("Keyboard contracts", ("tools/validate_keyboard_contracts.py", ".")),
    ("Accessibility markup", ("tools/validate_accessibility_markup.py", ".")),
    ("Localization catalog", ("tools/validate_localization_catalog.py", ".")),
    ("Onboarding contracts", ("tools/validate_onboarding_contracts.py", ".")),
    ("Packaging metadata", ("tools/validate_packaging_metadata.py", ".")),
    ("Release-tag validator tests", ("tools/tests/test_validate_release_tag.py",)),
)


def run_check(root: Path, label: str, arguments: tuple[str, ...]) -> bool:
    command = [sys.executable, *arguments]
    print(f"\n==> {label}", flush=True)
    completed = subprocess.run(command, cwd=root, check=False)
    if completed.returncode == 0:
        return True

    print(f"FAILED: {label} exited with code {completed.returncode}.", file=sys.stderr)
    return False


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Run CalcNova SDK-independent release-source validation."
    )
    parser.add_argument(
        "--tag",
        help="Optional release tag to validate in addition to the standard preflight checks",
    )
    parser.add_argument(
        "--root",
        default=".",
        help="Repository root (defaults to the current directory)",
    )
    args = parser.parse_args()

    root = Path(args.root).resolve()
    if not (root / "CalcNova.slnx").is_file():
        print(f"Not a CalcNova repository root: {root}", file=sys.stderr)
        return 2

    all_passed = True
    for label, command in CHECKS:
        all_passed = run_check(root, label, command) and all_passed

    if args.tag:
        all_passed = run_check(
            root,
            "Requested release tag",
            ("tools/validate_release_tag.py", args.tag),
        ) and all_passed

    if not all_passed:
        print("\nCalcNova SDK-independent release preflight FAILED.", file=sys.stderr)
        return 1

    print("\nCalcNova SDK-independent release preflight passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
