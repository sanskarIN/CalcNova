#!/usr/bin/env python3
"""Validate CalcNova 2.8.03 release documentation/evidence contracts without SDKs."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


REQUIRED_MARKERS: dict[str, tuple[str, ...]] = {
    "docs/RELEASE.md": (
        "# CalcNova 2.8.03 Release Process",
        "python tools/release_preflight.py --tag v2.8.3",
        "product/display version: `2.8.03`",
        "normalized release tag: `v2.8.3`",
        "PASS / FAIL / BLOCKED / NOT RUN",
        "validate_completion_status.py",
        "ACCESSIBILITY_TEST_MATRIX.md",
        "SETTINGS_MIGRATION.md",
        "artifact-metadata: write",
        "release-assets/**/*",
        "ARTIFACT_PROVENANCE.md",
    ),
    "docs/ARTIFACT_PROVENANCE.md": (
        "# CalcNova Release Artifact Provenance",
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
        "# CalcNova 2.8.03 Release Evidence Checklist",
        "Product: CalcNova 2.8.03 — COMPLETE",
        "Normalized tag: v2.8.3",
        "Source preflight: PASS / FAIL / BLOCKED / NOT RUN",
        ".NET restore/format/build/test: PASS / FAIL / BLOCKED / NOT RUN",
        "Windows: PASS / FAIL / BLOCKED / NOT RUN",
        "Linux: PASS / FAIL / BLOCKED / NOT RUN",
        "macOS: PASS / FAIL / BLOCKED / NOT RUN",
        "Browser: PASS / FAIL / BLOCKED / NOT RUN",
        "Android: PASS / FAIL / BLOCKED / NOT RUN",
        "iOS: PASS / FAIL / BLOCKED / NOT RUN",
        "Accessibility audit: PASS / FAIL / BLOCKED / NOT RUN",
        "Responsive-layout audit: PASS / FAIL / BLOCKED / NOT RUN",
        "Signing/store evidence: PASS / FAIL / BLOCKED / NOT RUN",
        "Never convert `NOT RUN` or `BLOCKED` into PASS",
    ),
    "PROJECT_STATE.md": (
        "**COMPLETE — CalcNova version 2.8.03**",
        "NuGetAuditMode=all",
        "artifact-metadata: write",
        "release-assets/**/*",
        "## Environment Verification Record",
        "`NOT RUN`",
        "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
    ),
    "what_changed.md": (
        "# What Changed",
        "**CalcNova version 2.8.03 is complete.**",
        "<NuGetAuditMode>all</NuGetAuditMode>",
        "artifact-metadata: write",
        "release-assets/**/*",
        "## Evidence policy",
        "`NOT RUN` or `BLOCKED`",
        "Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**",
    ),
    "docs/VERSIONING.md": (
        "**CalcNova 2.8.03** is the public/product release version.",
        "`v2.8.3`",
        "ApplicationVersion = 20803",
    ),
}


def validate(root: Path) -> list[str]:
    failures: list[str] = []
    for relative_path, markers in REQUIRED_MARKERS.items():
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
        description="Validate CalcNova 2.8.03 release documentation/evidence contracts."
    )
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Release documentation validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    checked_markers = sum(len(markers) for markers in REQUIRED_MARKERS.values())
    print(
        f"Validated {checked_markers} CalcNova 2.8.03 release/evidence/security markers across "
        f"{len(REQUIRED_MARKERS)} documentation/state files."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
