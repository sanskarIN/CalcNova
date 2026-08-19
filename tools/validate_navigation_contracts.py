#!/usr/bin/env python3
"""Validate shared mode-count and keyboard-navigation contracts without .NET."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


def fail(message: str) -> int:
    print(f"Navigation contract validation failed: {message}", file=sys.stderr)
    return 1


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova shared navigation contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    xaml_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml"
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "MainViewModel.cs"

    for path in (xaml_path, view_path, view_model_path):
        if not path.is_file():
            return fail(f"missing source file: {path}")

    xaml = xaml_path.read_text(encoding="utf-8")
    view_source = view_path.read_text(encoding="utf-8")
    view_model_source = view_model_path.read_text(encoding="utf-8")

    tab_count = len(re.findall(r"<TabItem\b", xaml))
    mode_count_match = re.search(r"public\s+const\s+int\s+ModeCount\s*=\s*(\d+)\s*;", view_model_source)
    if mode_count_match is None:
        return fail("MainViewModel.ModeCount constant is missing")

    mode_count = int(mode_count_match.group(1))
    if tab_count != mode_count:
        return fail(f"MainView contains {tab_count} tabs but MainViewModel.ModeCount is {mode_count}")

    for marker in (
        "SelectNextMode()",
        "SelectPreviousMode()",
        "NormalizeModeIndex",
    ):
        if marker not in view_model_source:
            return fail(f"MainViewModel is missing navigation marker: {marker}")

    for marker in (
        "Key.PageDown",
        "Key.PageUp",
        "KeyModifiers.Control",
        "SelectNextMode()",
        "SelectPreviousMode()",
    ):
        if marker not in view_source:
            return fail(f"MainView keyboard navigation is missing marker: {marker}")

    print(
        f"Validated {tab_count} shared modes and Ctrl+PageUp/PageDown keyboard navigation contracts."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
