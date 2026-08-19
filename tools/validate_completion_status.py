#!/usr/bin/env python3
"""Validate CalcNova's authoritative 2.8.03 completion-status contracts."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


DISPLAY_VERSION = "2.8.03"
NORMALIZED_VERSION = "2.8.3"
MOBILE_BUILD_CODE = "20803"

CURRENT_STATUS_CONTRACTS: dict[str, tuple[str, ...]] = {
    "README.md": (
        "**Current product version: 2.8.03**",
        "**Project status: Complete for version 2.8.03.**",
        "Normalized release tag: v2.8.3",
    ),
    "PROJECT_STATE.md": (
        "**2.8.03**",
        "**COMPLETE — CalcNova version 2.8.03**",
        "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
    ),
    "CHANGELOG.md": (
        "## [2.8.03] - 2026-08-19",
        "**Status: Complete**",
        "Normalized release tag: `v2.8.3`",
    ),
    "what_changed.md": (
        "**CalcNova version 2.8.03 is complete.**",
        "CalcNova 2.8.03 product scope: **COMPLETE**",
        "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
    ),
    "docs/README.md": (
        "# CalcNova 2.8.03 Documentation",
        "**Project status: Complete for version 2.8.03.**",
        "Normalized release tag: `v2.8.3`",
    ),
    "docs/FEATURES.md": (
        "# CalcNova 2.8.03 Features",
        "**Complete for version 2.8.03.**",
        "2.8.03 feature scope: **COMPLETE**",
    ),
    "docs/ROADMAP.md": (
        "# CalcNova 2.8.03 Completed Roadmap",
        "**All milestones defined for CalcNova 2.8.03 are complete.**",
        "No optional idea in this section is required for CalcNova 2.8.03 completion.",
    ),
    "docs/FINAL_SOURCE_AUDIT_2026-08-19.md": (
        "# CalcNova 2.8.03 Final Completion Audit — 2026-08-19",
        "**CalcNova version 2.8.03 is complete for the defined product scope.**",
        "Version 2.8.03 product scope: **COMPLETE**",
    ),
    "docs/VERSIONING.md": (
        "**CalcNova 2.8.03** is the public/product release version.",
        "| .NET/NuGet package version | `2.8.3` |",
        "| Android/iOS numeric build code | `20803` |",
    ),
}

FORBIDDEN_CURRENT_STATUS_MARKERS = (
    "under active development",
    "0.1.0-dev",
    "## [unreleased]",
    "## [0.1.0] - planned",
    "remaining product/runtime work",
    "remaining high-priority work",
    "remaining work is evidence-dependent",
    "current phase",
    "first validated milestone will be created",
)


def read_text(path: Path, failures: list[str]) -> str:
    if not path.is_file():
        failures.append(f"Missing authoritative completion file: {path}")
        return ""
    return path.read_text(encoding="utf-8")


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    for relative_path, required_markers in CURRENT_STATUS_CONTRACTS.items():
        path = root / relative_path
        source = read_text(path, failures)
        if not source:
            continue

        for marker in required_markers:
            if marker not in source:
                failures.append(f"{relative_path} is missing completion marker: {marker}")

        lowered = source.lower()
        for forbidden in FORBIDDEN_CURRENT_STATUS_MARKERS:
            if forbidden in lowered:
                failures.append(
                    f"{relative_path} contains obsolete pre-completion marker: {forbidden}"
                )

    about_path = root / "src" / "CalcNova.App" / "ViewModels" / "AboutViewModel.cs"
    about_source = read_text(about_path, failures)
    for marker in (
        'public string Version => "2.8.03";',
        'public string CompletionStatus => "Complete";',
        'public string ReleaseLabel => $"Version {Version} • {CompletionStatus}";',
    ):
        if marker not in about_source:
            failures.append(f"AboutViewModel is missing release marker: {marker}")

    about_extension_path = root / "src" / "CalcNova.App" / "Views" / "MainView.BivariateStatistics.cs"
    about_extension = read_text(about_extension_path, failures)
    for marker in (
        "EnsureAboutReleaseIdentity();",
        "viewModel.About.ReleaseLabel",
        "DetachAboutReleaseIdentity();",
    ):
        if marker not in about_extension:
            failures.append(f"Shared About release identity is missing marker: {marker}")

    about_test_path = root / "tests" / "CalcNova.App.Tests" / "AboutReleaseIdentityTests.cs"
    about_test = read_text(about_test_path, failures)
    if 'Assert.Equal("Version 2.8.03 • Complete", viewModel.ReleaseLabel);' not in about_test:
        failures.append("About release identity regression does not protect the completed release label.")

    headless_test_path = root / "tests" / "CalcNova.App.Tests" / "AboutReleaseIdentityHeadlessTests.cs"
    headless_test = read_text(headless_test_path, failures)
    if '"Version 2.8.03 • Complete"' not in headless_test:
        failures.append("About headless regression does not protect the visible completed release label.")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova 2.8.03 completion-status contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("CalcNova completion-status validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        "Validated CalcNova 2.8.03 completed product status, normalized 2.8.3 release identity, "
        "and in-app About release labeling."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
