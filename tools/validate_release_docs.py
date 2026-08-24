#!/usr/bin/env python3
"""Validate CalcNova current release documentation/evidence contracts without SDKs."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    from tools.release_identity import ReleaseIdentity, load_release_identity
except ModuleNotFoundError:  # Direct execution via `python tools/validate_release_docs.py`.
    from release_identity import ReleaseIdentity, load_release_identity


def required_markers(identity: ReleaseIdentity) -> dict[str, tuple[str, ...]]:
    display = identity.display_version
    tag = identity.release_tag
    build = identity.mobile_build_code
    return {
        "docs/RELEASE.md": (
            f"# CalcNova {display} Release Process",
            f"python tools/release_preflight.py --tag {tag}",
            f"product/display version: `{display}`",
            f"release tag: `{tag}`",
            "PASS / FAIL / BLOCKED / NOT RUN",
            "tools/release_identity.py",
            "validate_completion_status.py",
            "validate_platform_support.py",
            "ACCESSIBILITY_TEST_MATRIX.md",
            "SETTINGS_MIGRATION.md",
            "artifact-metadata: write",
            "release-assets/**/*",
            "ARTIFACT_PROVENANCE.md",
        ),
        "docs/ARTIFACT_PROVENANCE.md": (
            "# CalcNova Release Artifact Provenance",
            "CycloneDX 1.7",
            "https://cyclonedx.org/schema/bom-1.7.schema.json",
            "tools/generate_sbom.py",
            ".sbom.cdx.json",
            "top-level format version `3`",
            "tools/tests/test_generate_sbom.py",
            "actions/attest@v4",
            "artifact-metadata: write",
            "release-assets/**/*",
            "duplicate-basename guard",
            "published asset **basenames**",
            "sha256sum -c SHA256SUMS.txt",
            "gh attestation verify PATH_TO_ARTIFACT -R sanskarIN/CalcNova",
        ),
        "docs/SECURITY_AUTOMATION.md": (
            "# CalcNova Security Automation",
            "<NuGetAudit>true</NuGetAudit>",
            "<NuGetAuditMode>all</NuGetAuditMode>",
            "<NuGetAuditLevel>moderate</NuGetAuditLevel>",
            "python tools/validate_dependency_security.py .",
            "python tools/validate_security_workflows.py .",
        ),
        "docs/RELEASE_READINESS_CHECKLIST.md": (
            f"# CalcNova {display} Release Evidence Checklist",
            f"Product: CalcNova {display} — COMPLETE",
            f"Release tag: {tag}",
            f"Mobile build code: {build}",
            "Source preflight: PASS / FAIL / BLOCKED / NOT RUN",
            ".NET restore/format/build/test: PASS / FAIL / BLOCKED / NOT RUN",
            "Windows x64: PASS / FAIL / BLOCKED / NOT RUN",
            "Windows ARM64: PASS / FAIL / BLOCKED / NOT RUN",
            "Linux x64: PASS / FAIL / BLOCKED / NOT RUN",
            "Linux ARM64: PASS / FAIL / BLOCKED / NOT RUN",
            "macOS x64: PASS / FAIL / BLOCKED / NOT RUN",
            "macOS ARM64: PASS / FAIL / BLOCKED / NOT RUN",
            "Browser: PASS / FAIL / BLOCKED / NOT RUN",
            "Android: PASS / FAIL / BLOCKED / NOT RUN",
            "iOS: PASS / FAIL / BLOCKED / NOT RUN",
            "Accessibility audit: PASS / FAIL / BLOCKED / NOT RUN",
            "Responsive-layout audit: PASS / FAIL / BLOCKED / NOT RUN",
            "Signing/store evidence: PASS / FAIL / BLOCKED / NOT RUN",
            "SBOM/checksum/provenance publication: PASS / FAIL / BLOCKED / NOT RUN",
            "Never convert `NOT RUN` or `BLOCKED` into PASS",
        ),
        "PROJECT_STATE.md": (
            f"**COMPLETE — CalcNova version {display}**",
            "NuGetAuditMode=all",
            "tools/release_identity.py",
            "artifact-metadata: write",
            "release-assets/**/*",
            "## Environment Verification Record",
            "`NOT RUN`",
            "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
        ),
        "what_changed.md": (
            "# What Changed",
            f"CalcNova {display} product scope: **COMPLETE**",
            "<NuGetAuditMode>all</NuGetAuditMode>",
            "artifact-metadata: write",
            "release-assets/**/*",
            "## Evidence policy",
            "`NOT RUN` or `BLOCKED`",
            "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
        ),
        "docs/VERSIONING.md": (
            f"**CalcNova {display}** is the public/product release version.",
            f"`{tag}`",
            f"ApplicationVersion = {build}",
            "tools/release_identity.py",
        ),
        "docs/PLATFORM_SUPPORT.md": (
            f"# CalcNova {display} Platform Support",
            f"display version: `{display}`",
            f"numeric build code: `{build}`",
            "validate_platform_support.py",
        ),
        "docs/releases/2.9.0.md": (
            "# CalcNova 2.9.0 Release Checkpoint",
            "Release tag: `v2.9.0`",
            "Android/iOS build code: `20900`",
            "advanced from this checkpoint to CalcNova **2.9.5**",
        ),
    }


def validate(root: Path) -> list[str]:
    failures: list[str] = []
    try:
        identity = load_release_identity(root)
    except ValueError as exception:
        return [str(exception)]

    for relative_path, markers in required_markers(identity).items():
        path = root / relative_path
        if not path.is_file():
            failures.append(f"Missing release evidence document: {relative_path}")
            continue

        source = path.read_text(encoding="utf-8")
        for marker in markers:
            if marker not in source:
                failures.append(f"{relative_path} is missing required release-evidence marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Validate CalcNova current release documentation/evidence contracts."
    )
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)
    if failures:
        print("Release documentation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    identity = load_release_identity(root)
    markers = required_markers(identity)
    checked_markers = sum(len(values) for values in markers.values())
    print(
        f"Validated {checked_markers} CalcNova {identity.display_version} release/evidence/security markers across "
        f"{len(markers)} documentation/state files."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
