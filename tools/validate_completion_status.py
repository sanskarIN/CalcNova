#!/usr/bin/env python3
"""Validate CalcNova's authoritative current-release completion contracts."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    from tools.release_identity import ReleaseIdentity, load_release_identity
except ModuleNotFoundError:  # Direct execution via `python tools/validate_completion_status.py`.
    from release_identity import ReleaseIdentity, load_release_identity


FORBIDDEN_CURRENT_STATUS_MARKERS = (
    "under active development",
    "active pre-release development",
    "## [unreleased]",
    "## [0.1.0] - planned",
    "remaining product/runtime work",
    "remaining high-priority work",
    "remaining work is evidence-dependent",
    "current phase",
    "first validated milestone will be created",
    "suggested development milestones",
    "development mobile display version",
    "## development environment",
)


def current_status_contracts(identity: ReleaseIdentity) -> dict[str, tuple[str, ...]]:
    display = identity.display_version
    tag = identity.release_tag
    return {
        "README.md": (
            f"**Current product version: {display}**",
            f"**Project status: Complete for version {display}.**",
            f"Normalized release tag: {tag}",
        ),
        "PROJECT_STATE.md": (
            f"**{display}**",
            f"**COMPLETE — CalcNova version {display}**",
            "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
        ),
        "CHANGELOG.md": (
            f"## [{display}] -",
            "**Status: Complete**",
            f"Normalized release tag: `{tag}`",
        ),
        "what_changed.md": (
            f"CalcNova {display} product scope: **COMPLETE**",
            "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
        ),
        "SECURITY.md": (
            f"**CalcNova {display} is the current completed and supported product baseline.**",
            f"| `{display}` | Yes |",
            f"normalized release tag is `{tag}`",
        ),
        "SUPPORT.md": (
            f"**Current supported product baseline: CalcNova {display}.**",
            f"Feature requests are optional post-{display} enhancements",
            "Support CalcNova maintenance and optional improvements",
        ),
        "CONTRIBUTING.md": (
            f"**CalcNova {display} is the completed product baseline.**",
            "## Contributor setup",
            "maintenance, correctness, security, compatibility, documentation, translations, tests, dependency updates, or explicitly proposed optional enhancements",
        ),
        "docs/README.md": (
            f"# CalcNova {display} Documentation",
            f"**Project status: Complete for version {display}.**",
            f"Normalized release tag: `{tag}`",
        ),
        "docs/FEATURES.md": (
            f"# CalcNova {display} Features",
            f"**Complete for version {display}.**",
            f"{display} feature scope: **COMPLETE**",
        ),
        "docs/ROADMAP.md": (
            f"# CalcNova {display} Completed Roadmap",
            f"**All milestones defined for CalcNova {display} are complete.**",
            f"No optional idea in this section is required for CalcNova {display} completion.",
        ),
        "docs/VERSIONING.md": (
            f"**CalcNova {display}** is the public/product release version.",
            f"| .NET/NuGet package version | `{identity.semver_version}` |",
            f"| Android/iOS numeric build code | `{identity.mobile_build_code}` |",
        ),
        "docs/RELEASE.md": (
            f"# CalcNova {display} Release Process",
            f"CalcNova {display} is the completed product baseline.",
            f"python tools/release_preflight.py --tag {tag}",
        ),
        "docs/RELEASE_READINESS_CHECKLIST.md": (
            f"# CalcNova {display} Release Evidence Checklist",
            f"CalcNova {display} is the completed product baseline.",
            f"Product: CalcNova {display} — COMPLETE",
        ),
        "docs/PLATFORM_SUPPORT.md": (
            f"# CalcNova {display} Platform Support",
            f"**Cross-platform source composition is complete for CalcNova {display}.**",
            "Desktop source composition: **COMPLETE**",
        ),
        "docs/SOURCE_PREFLIGHT.md": (
            f"# CalcNova {display} SDK-Independent Source Preflight",
            f"CalcNova {display} is the completed product baseline.",
            f"python tools/release_preflight.py --tag {tag}",
        ),
    }


def read_text(path: Path, failures: list[str]) -> str:
    if not path.is_file():
        failures.append(f"Missing authoritative completion file: {path}")
        return ""
    return path.read_text(encoding="utf-8")


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    try:
        identity = load_release_identity(root)
    except ValueError as exception:
        return [str(exception)]

    for relative_path, required_markers in current_status_contracts(identity).items():
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
        f'public string Version => "{identity.display_version}";',
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

    release_label = f"Version {identity.display_version} • Complete"
    about_test_path = root / "tests" / "CalcNova.App.Tests" / "AboutReleaseIdentityTests.cs"
    about_test = read_text(about_test_path, failures)
    if f'Assert.Equal("{release_label}", viewModel.ReleaseLabel);' not in about_test:
        failures.append("About release identity regression does not protect the current release label.")

    headless_test_path = root / "tests" / "CalcNova.App.Tests" / "AboutReleaseIdentityHeadlessTests.cs"
    headless_test = read_text(headless_test_path, failures)
    if f'"{release_label}"' not in headless_test:
        failures.append("About headless regression does not protect the visible current release label.")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova current-release completion contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)
    if failures:
        print("CalcNova completion-status validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    identity = load_release_identity(root)
    print(
        f"Validated CalcNova {identity.display_version} completed product status, normalized "
        f"{identity.semver_version} release identity, and in-app About release labeling."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
