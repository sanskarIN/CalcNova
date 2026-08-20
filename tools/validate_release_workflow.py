#!/usr/bin/env python3
"""Validate CalcNova's tag-first release workflow source contract."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


DESKTOP_RELEASE_TARGETS: tuple[tuple[str, str], ...] = (
    ("windows-latest", "win-x64"),
    ("windows-latest", "win-arm64"),
    ("ubuntu-latest", "linux-x64"),
    ("ubuntu-latest", "linux-arm64"),
    ("macos-latest", "osx-x64"),
    ("macos-latest", "osx-arm64"),
)


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
        'SOURCE_VERSION="$(sed -n \'s:.*<Version>\\(.*\\)</Version>.*:\\1:p\' Directory.Build.props | head -n 1)"',
        'test "$RELEASE_TAG" = "v$SOURCE_VERSION"',
        'python tools/release_preflight.py --tag "$RELEASE_TAG"',
        'dotnet restore CalcNova.slnx',
        'dotnet format CalcNova.slnx --verify-no-changes --no-restore',
        'dotnet build CalcNova.slnx --configuration Release --no-restore',
        'dotnet test CalcNova.slnx --configuration Release --no-build',
        "needs: validate",
        "--verify-tag --generate-notes",
        "--clobber",
        'rm -f "$RUNNER_TEMP/calcnova-release.keystore"',
        'dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj --configuration Release --runtime ${{ matrix.rid }} --self-contained true --output publish/${{ matrix.rid }}',
        'name: desktop-${{ matrix.rid }}',
        'path: CalcNova-${{ matrix.rid }}.zip',
        "permissions:\n  contents: read",
        "actions/attest@v4",
        "subject-path: release-assets/**/*",
        "- name: Validate release asset filenames",
        "Duplicate release asset filenames are not allowed:",
        "SHA256SUMS.txt is a reserved release asset filename.",
        'printf \'%s  %s\\n\' "$digest" "$(basename "$file")" >> SHA256SUMS.txt',
    )
    for marker in required_markers:
        if marker not in source:
            failures.append(f"Release workflow is missing required marker: {marker}")

    for runner, rid in DESKTOP_RELEASE_TARGETS:
        target_marker = f"- os: {runner}\n            rid: {rid}"
        if target_marker not in source:
            failures.append(f"Release workflow is missing desktop release target: {runner} / {rid}")

    ref_marker = "ref: ${{ github.event_name == 'workflow_dispatch' && inputs.tag || github.ref }}"
    if source.count(ref_marker) < 4:
        failures.append("Desktop, Browser, Android, and release-publication jobs must all check out the release ref.")

    checkout_position = source.find('git checkout --detach "$RELEASE_TAG"')
    version_position = source.find('test "$RELEASE_TAG" = "v$SOURCE_VERSION"')
    preflight_position = source.find('python tools/release_preflight.py --tag "$RELEASE_TAG"')
    restore_position = source.find('dotnet restore CalcNova.slnx')
    if not (
        checkout_position >= 0
        and version_position > checkout_position
        and preflight_position > version_position
        and restore_position > preflight_position
    ):
        failures.append(
            "Release tag/source-version consistency and tagged source preflight must run after tag checkout and before .NET validation."
        )

    publish_position = source.find("  publish-release:")
    publication_permissions = (
        "    permissions:\n"
        "      contents: write\n"
        "      id-token: write\n"
        "      attestations: write\n"
        "      artifact-metadata: write"
    )
    permission_position = source.find(publication_permissions, publish_position)
    download_position = source.find("      - name: Download packaged artifacts", publish_position)
    filename_position = source.find("      - name: Validate release asset filenames", publish_position)
    checksum_position = source.find("      - name: Generate checksums", publish_position)
    attestation_position = source.find("      - name: Attest release artifacts", publish_position)
    release_position = source.find("      - name: Create or reuse GitHub Release", publish_position)
    if not (
        publish_position >= 0
        and permission_position > publish_position
        and download_position > permission_position
        and filename_position > download_position
        and checksum_position > filename_position
        and attestation_position > checksum_position
        and release_position > attestation_position
    ):
        failures.append(
            "Release publication must validate flat asset filenames, generate download-friendly checksums, attest artifacts, and only then publish the GitHub Release."
        )

    if source.count("contents: write") != 1:
        failures.append("Release workflow must grant contents: write only to the publication job.")
    if source.count("id-token: write") != 1:
        failures.append("Release workflow must grant id-token: write only once for artifact attestation.")
    if source.count("attestations: write") != 1:
        failures.append("Release workflow must grant attestations: write only once for artifact attestation.")
    if source.count("artifact-metadata: write") != 1:
        failures.append("Release workflow must grant artifact-metadata: write only once for artifact attestation.")

    forbidden_markers = (
        "gh release delete",
        "git tag -f",
        "git push --force",
        "AndroidSigningKeyPass>password",
        '-p:ApplicationDisplayVersion="$VERSION"',
        '-p:ApplicationVersion="${{ github.run_number }}"',
        "actions/attest-build-provenance@",
        "actions/attest-sbom@",
        "xargs -0 sha256sum > SHA256SUMS.txt",
    )
    for marker in forbidden_markers:
        if marker in source:
            failures.append(f"Release workflow contains forbidden destructive/insecure/version-drift/deprecated/checksum marker: {marker}")

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

    print(
        "Validated tag/source version alignment, x64/ARM64 publication, flat checksum manifests, least-privilege provenance attestation, signing, and release publication contracts."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
