#!/usr/bin/env python3
"""Validate CalcNova cross-platform build workflow contracts without platform SDKs."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


WORKFLOW_CONTRACTS: dict[str, tuple[str, ...]] = {
    ".github/workflows/build-desktop.yml": (
        "os: [ubuntu-latest, windows-latest, macos-latest]",
        "actions/checkout@v7",
        "actions/setup-dotnet@v6",
        "dotnet-version: 10.0.x",
        "dotnet restore src/CalcNova.Desktop/CalcNova.Desktop.csproj",
        "dotnet build src/CalcNova.Desktop/CalcNova.Desktop.csproj --configuration Release --no-restore",
    ),
    ".github/workflows/build-browser.yml": (
        "runs-on: ubuntu-latest",
        "actions/checkout@v7",
        "actions/setup-dotnet@v6",
        "dotnet-version: 10.0.x",
        "dotnet workload install wasm-tools",
        "dotnet restore src/CalcNova.Browser/CalcNova.Browser.csproj",
        "dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj --configuration Release --no-restore --output artifacts/browser",
    ),
    ".github/workflows/build-android.yml": (
        "runs-on: ubuntu-latest",
        "actions/checkout@v7",
        "actions/setup-dotnet@v6",
        "actions/setup-java@v5",
        'java-version: "17"',
        "dotnet workload install android",
        "dotnet restore src/CalcNova.Android/CalcNova.Android.csproj",
        "dotnet build src/CalcNova.Android/CalcNova.Android.csproj --configuration Release --no-restore",
    ),
    ".github/workflows/build-ios.yml": (
        "runs-on: macos-latest",
        "actions/checkout@v7",
        "actions/setup-dotnet@v6",
        "dotnet workload install ios",
        "iossimulator-arm64",
        "iossimulator-x64",
        "dotnet restore src/CalcNova.iOS/CalcNova.iOS.csproj -p:RuntimeIdentifier=${{ env.IOS_RID }}",
        "dotnet build src/CalcNova.iOS/CalcNova.iOS.csproj --configuration Release --no-restore -p:RuntimeIdentifier=${{ env.IOS_RID }}",
    ),
}

FORBIDDEN_MARKERS = (
    "AndroidSigningKeyPass",
    "AndroidSigningStorePass",
    "CodesignKey",
    "CodesignProvision",
    "--password",
)


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    global_json = root / "global.json"
    if not global_json.is_file():
        failures.append("Missing global.json SDK policy.")
    else:
        source = global_json.read_text(encoding="utf-8")
        for marker in ('"version": "10.0.400"', '"rollForward": "latestFeature"', '"allowPrerelease": false'):
            if marker not in source:
                failures.append(f"global.json is missing SDK policy marker: {marker}")

    for relative_path, markers in WORKFLOW_CONTRACTS.items():
        path = root / relative_path
        if not path.is_file():
            failures.append(f"Missing platform workflow: {relative_path}")
            continue

        source = path.read_text(encoding="utf-8")
        for marker in markers:
            if marker not in source:
                failures.append(f"{relative_path} is missing platform-build marker: {marker}")

        if "permissions:\n  contents: read" not in source:
            failures.append(f"{relative_path} must keep read-only contents permission.")

        for marker in FORBIDDEN_MARKERS:
            if marker in source:
                failures.append(
                    f"{relative_path} contains signing/secret material that does not belong in validation builds: {marker}"
                )

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova platform build workflow contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Platform workflow validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated {len(WORKFLOW_CONTRACTS)} cross-platform build workflows and shared SDK policy.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
