#!/usr/bin/env python3
"""Validate CalcNova's tag-first release workflow source contract."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    workflow_path = root / ".github" / "workflows" / "release.yml"
    if not workflow_path.is_file():
        return [f"Missing release workflow: {workflow_path}"]

    source = workflow_path.read_text(encoding="utf-8")
    failures: list[str] = []

    required_markers = (
        'RELEASE_TAG: ${{ github.event_name == \'workflow_dispatch\' && inputs.tag || github.ref_name }}',
        'git rev-parse -q --verify "refs/tags/$RELEASE_TAG^{commit}"',
        'git checkout --detach "$RELEASE_TAG"',
        'python tools/release_preflight.py --tag "$RELEASE_TAG"',
        'dotnet restore CalcNova.slnx',
        'dotnet format CalcNova.slnx --verify-no-changes --no-restore',
        'dotnet build CalcNova.slnx --configuration Release --no-restore',
        'dotnet test CalcNova.slnx --configuration Release --no-build',
        "needs: validate",
        "--verify-tag --generate-notes",
        "--clobber",
        'rm -f "$RUNNER_TEMP/calcnova-release.keystore"',
    )
    for marker in required_markers:
        if marker not in source:
            failures.append(f"Release workflow is missing required marker: {marker}")

    ref_marker = "ref: ${{ github.event_name == 'workflow_dispatch' && inputs.tag || github.ref }}"
    if source.count(ref_marker) < 4:
        failures.append("Desktop, Browser, Android, and release-publication jobs must all check out the release ref.")

    preflight_position = source.find('python tools/release_preflight.py --tag "$RELEASE_TAG"')
    checkout_position = source.find('git checkout --detach "$RELEASE_TAG"')
    restore_position = source.find('dotnet restore CalcNova.slnx')
    if not (checkout_position >= 0 and preflight_position > checkout_position and restore_position > preflight_position):
        failures.append("Tagged source preflight must run after tag checkout and before .NET restore/build validation.")

    forbidden_markers = (
        "gh release delete",
        "git tag -f",
        "git push --force",
        "AndroidSigningKeyPass>password",
    )
    for marker in forbidden_markers:
        if marker in source:
            failures.append(f"Release workflow contains forbidden destructive/insecure marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova release workflow contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Release workflow validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated tag-first release preflight, build, signing, and publication workflow contracts.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
