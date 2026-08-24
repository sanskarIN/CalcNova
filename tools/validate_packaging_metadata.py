#!/usr/bin/env python3
"""Validate CalcNova release/package identity metadata without platform SDKs."""

from __future__ import annotations

import argparse
from datetime import date
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

try:
    from tools.release_identity import load_release_identity
except ModuleNotFoundError:  # Direct execution via `python tools/validate_packaging_metadata.py`.
    from release_identity import load_release_identity


APP_ID = "in.sanskar.calcnova"
APP_NAME = "CalcNova"


def read(path: Path, failures: list[str]) -> str:
    if not path.is_file():
        failures.append(f"Missing packaging metadata file: {path}")
        return ""
    return path.read_text(encoding="utf-8")


def require(source: str, marker: str, label: str, failures: list[str]) -> None:
    if marker not in source:
        failures.append(f"{label} is missing required marker: {marker}")


def parse_xml(path: Path, failures: list[str]) -> ET.ElementTree | None:
    try:
        return ET.parse(path)
    except (ET.ParseError, OSError) as exception:
        failures.append(f"Invalid XML in {path}: {exception}")
        return None


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    try:
        identity = load_release_identity(root)
    except ValueError as exception:
        failures.append(str(exception))
        return failures

    display_version = identity.display_version
    semver_version = identity.semver_version
    mobile_build_code = identity.mobile_build_code

    build_props_path = root / "Directory.Build.props"
    android_path = root / "src" / "CalcNova.Android" / "CalcNova.Android.csproj"
    ios_project_path = root / "src" / "CalcNova.iOS" / "CalcNova.iOS.csproj"
    ios_plist_path = root / "src" / "CalcNova.iOS" / "Info.plist"
    desktop_project_path = root / "src" / "CalcNova.Desktop" / "CalcNova.Desktop.csproj"
    browser_project_path = root / "src" / "CalcNova.Browser" / "CalcNova.Browser.csproj"
    linux_desktop_path = root / "packaging" / "linux" / f"{APP_ID}.desktop"
    linux_meta_path = root / "packaging" / "linux" / f"{APP_ID}.metainfo.xml"
    macos_plist_path = root / "packaging" / "macos" / "Info.plist.template"
    windows_manifest_path = root / "packaging" / "windows" / "AppxManifest.xml.template"

    build_props = read(build_props_path, failures)
    android = read(android_path, failures)
    ios_project = read(ios_project_path, failures)
    ios_plist = read(ios_plist_path, failures)
    desktop_project = read(desktop_project_path, failures)
    browser_project = read(browser_project_path, failures)
    linux_desktop = read(linux_desktop_path, failures)
    linux_meta = read(linux_meta_path, failures)
    macos_plist = read(macos_plist_path, failures)
    windows_manifest = read(windows_manifest_path, failures)

    for marker in (
        f"<ProductDisplayVersion>{display_version}</ProductDisplayVersion>",
        f"<Version>{semver_version}</Version>",
        f"<VersionPrefix>{semver_version}</VersionPrefix>",
        f"<PackageVersion>{semver_version}</PackageVersion>",
        f"<AssemblyVersion>{identity.assembly_version}</AssemblyVersion>",
        f"<FileVersion>{identity.assembly_version}</FileVersion>",
        f"<InformationalVersion>{display_version}</InformationalVersion>",
    ):
        require(build_props, marker, "Directory.Build.props", failures)

    for label, source in (("Android project", android), ("iOS project", ios_project)):
        require(source, f"<ApplicationId>{APP_ID}</ApplicationId>", label, failures)
        require(source, f"<ApplicationTitle>{APP_NAME}</ApplicationTitle>", label, failures)
        require(source, f"<ApplicationVersion>{mobile_build_code}</ApplicationVersion>", label, failures)
        require(
            source,
            "<ApplicationDisplayVersion>$(ProductDisplayVersion)</ApplicationDisplayVersion>",
            label,
            failures,
        )
        if "-dev" in source.lower():
            failures.append(f"{label} still contains a development-version marker.")

    require(desktop_project, "<AssemblyName>CalcNova.Desktop</AssemblyName>", "Desktop project", failures)
    require(browser_project, "<AssemblyName>CalcNova.Browser</AssemblyName>", "Browser project", failures)

    require(ios_plist, "<string>CalcNova</string>", "iOS Info.plist", failures)
    require(ios_plist, "<string>LaunchScreen</string>", "iOS Info.plist", failures)

    require(linux_desktop, "Name=CalcNova", "Linux desktop entry", failures)
    require(linux_desktop, "Exec=CalcNova.Desktop", "Linux desktop entry", failures)
    require(linux_desktop, f"Icon={APP_ID}", "Linux desktop entry", failures)
    require(linux_meta, f"<id>{APP_ID}</id>", "Linux AppStream metadata", failures)
    require(linux_meta, "<name>CalcNova</name>", "Linux AppStream metadata", failures)

    linux_tree = parse_xml(linux_meta_path, failures) if linux_meta_path.is_file() else None
    if linux_tree is not None:
        matching_releases = [
            release
            for release in linux_tree.getroot().findall("./releases/release")
            if release.attrib.get("version") == display_version
        ]
        if len(matching_releases) != 1:
            failures.append(
                f"Linux AppStream metadata must contain exactly one release entry for {display_version}."
            )
        else:
            release = matching_releases[0]
            if release.attrib.get("type") != "stable":
                failures.append(f"Linux AppStream {display_version} release must be type=stable.")
            release_date = release.attrib.get("date", "")
            try:
                date.fromisoformat(release_date)
            except ValueError:
                failures.append(
                    f"Linux AppStream {display_version} release has an invalid ISO date: {release_date!r}."
                )
            description_text = " ".join(release.itertext())
            if f"CalcNova {display_version}" not in description_text:
                failures.append(
                    f"Linux AppStream {display_version} release description must identify CalcNova {display_version}."
                )

    require(macos_plist, f"<string>{APP_ID}</string>", "macOS plist template", failures)
    require(macos_plist, "<string>__VERSION__</string>", "macOS plist template", failures)
    require(macos_plist, "<string>__BUILD_NUMBER__</string>", "macOS plist template", failures)

    require(windows_manifest, f'Identity Name="{APP_ID}"', "Windows manifest template", failures)
    require(windows_manifest, 'Publisher="__PUBLISHER__"', "Windows manifest template", failures)
    require(windows_manifest, 'Version="__MSIX_VERSION__"', "Windows manifest template", failures)
    require(windows_manifest, "<DisplayName>CalcNova</DisplayName>", "Windows manifest template", failures)

    for path in (ios_plist_path, macos_plist_path, windows_manifest_path):
        if path.is_file():
            parse_xml(path, failures)

    forbidden_signing_patterns = (
        r"(?i)<AndroidSigningKeyPass>[^<$]",
        r"(?i)<AndroidSigningStorePass>[^<$]",
        r"(?i)<CodesignKey>[^<$]",
        r"(?i)certificate_password\s*=\s*[^_$]",
        r"(?i)keystore_password\s*=\s*[^_$]",
    )
    combined = "\n".join((android, ios_project, macos_plist, windows_manifest))
    for pattern in forbidden_signing_patterns:
        if re.search(pattern, combined):
            failures.append(f"Packaging metadata appears to contain an inline signing secret matching: {pattern}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova package metadata consistency.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)
    if failures:
        print("Packaging metadata validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    identity = load_release_identity(root)
    print(
        f"Validated CalcNova {identity.display_version} identity across shared, mobile, Desktop/Browser, "
        "Linux/macOS/Windows packaging contracts without requiring platform SDKs."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
