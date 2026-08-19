#!/usr/bin/env python3
"""Validate CalcNova's release-tag iOS simulator workflow contract."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    workflow_path = root / ".github" / "workflows" / "release-ios-validate.yml"
    if not workflow_path.is_file():
        return [f"Missing release iOS workflow: {workflow_path}"]

    source = workflow_path.read_text(encoding="utf-8")
    failures: list[str] = []

    required_markers = (
        "tags:\n      - 'v*'",
        "runs-on: macos-latest",
        "permissions:\n  contents: read",
        "git rev-parse -q --verify \"refs/tags/$RELEASE_TAG^{commit}\"",
        "git checkout --detach \"$RELEASE_TAG\"",
        "actions/setup-dotnet@v6",
        "dotnet-version: '10.0.x'",
        "dotnet workload install ios",
        "iossimulator-arm64",
        "iossimulator-x64",
        "dotnet restore src/CalcNova.iOS/CalcNova.iOS.csproj -p:RuntimeIdentifier=${{ env.IOS_RID }}",
        "dotnet build src/CalcNova.iOS/CalcNova.iOS.csproj --configuration Release --no-restore -p:RuntimeIdentifier=${{ env.IOS_RID }}",
        "does not claim device signing, provisioning, archive, notarization, or App Store readiness",
    )
    for marker in required_markers:
        if marker not in source:
            failures.append(f"Release iOS workflow is missing required marker: {marker}")

    for forbidden in (
        "CodesignKey",
        "CodesignProvision",
        "P12",
        "PROVISIONING_PROFILE",
        "certificate-password",
        "--force",
    ):
        if forbidden in source:
            failures.append(f"Unsigned simulator validation must not contain signing/destructive marker: {forbidden}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova release-tag iOS workflow contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Release iOS workflow validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated exact-tag unsigned iOS simulator release workflow contract.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
