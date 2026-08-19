#!/usr/bin/env python3
"""Validate CalcNova release/package identity metadata without platform SDKs."""

from __future__ import annotations

import argparse
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

APP_ID = "in.sanskar.calcnova"
APP_NAME = "CalcNova"
DEV_DISPLAY_VERSION = "0.1.0-dev"


def read(path: Path, failures: list[str]) -> str:
    if not path.is_file():
        failures.append(f"Missing packaging metadata file: {path}")
        return ""
    return path.read_text(encoding="utf-8")


def require(source: str, marker: str, label: str, failures: list[str]) -> None:
    if marker not in source:
        failures.append(f"{label} is missing required marker: {marker}")


def parse_xml(path: Path, failures: list[str]) -> None:
    try:
        ET.parse(path)
    except (ET.ParseError, OSError) as exception:
        failures.append(f"Invalid XML in {path}: {exception}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova package metadata consistency.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures: list[str] = []

    android_path = root / "src" / "CalcNova.Android" / "CalcNova.Android.csproj"
    ios_project_path = root / "src" / "CalcNova.iOS" / "CalcNova.iOS.csproj"
    ios_plist_path = root / "src" / "CalcNova.iOS" / "Info.plist"
    desktop_project_path = root / "src" / "CalcNova.Desktop" / "CalcNova.Desktop.csproj"
    browser_project_path = root / "src" / "CalcNova.Browser" / "CalcNova.Browser.csproj"
    linux_desktop_path = root / "packaging" / "linux" / f"{APP_ID}.desktop"
    linux_meta_path = root / "packaging" / "linux" / f"{APP_ID}.metainfo.xml"
    macos_plist_path = root / "packaging" / "macos" / "Info.plist.template"
    windows_manifest_path = root / "packaging" / "windows" / "AppxManifest.xml.template"

    android = read(android_path, failures)
    ios_project = read(ios_project_path, failures)
    ios_plist = read(ios_plist_path, failures)
    desktop_project = read(desktop_project_path, failures)
    browser_project = read(browser_project_path, failures)
    linux_desktop = read(linux_desktop_path, failures)
    linux_meta = read(linux_meta_path, failures)
    macos_plist = read(macos_plist_path, failures)
    windows_manifest = read(windows_manifest_path, failures)

    for label, source in (("Android project", android), ("iOS project", ios_project)):
        require(source, f"<ApplicationId>{APP_ID}</ApplicationId>", label, failures)
        require(source, f"<ApplicationTitle>{APP_NAME}</ApplicationTitle>", label, failures)
        require(source, "<ApplicationVersion>1</ApplicationVersion>", label, failures)
        require(
            source,
            f"<ApplicationDisplayVersion>{DEV_DISPLAY_VERSION}</ApplicationDisplayVersion>",
            label,
            failures,
        )

    require(desktop_project, "<AssemblyName>CalcNova.Desktop</AssemblyName>", "Desktop project", failures)
    require(browser_project, "<AssemblyName>CalcNova.Browser</AssemblyName>", "Browser project", failures)

    require(ios_plist, "<string>CalcNova</string>", "iOS Info.plist", failures)
    require(ios_plist, "<string>LaunchScreen</string>", "iOS Info.plist", failures)

    require(linux_desktop, "Name=CalcNova", "Linux desktop entry", failures)
    require(linux_desktop, "Exec=CalcNova.Desktop", "Linux desktop entry", failures)
    require(linux_desktop, f"Icon={APP_ID}", "Linux desktop entry", failures)
    require(linux_meta, f"<id>{APP_ID}</id>", "Linux AppStream metadata", failures)
    require(linux_meta, "<name>CalcNova</name>", "Linux AppStream metadata", failures)

    require(macos_plist, f"<string>{APP_ID}</string>", "macOS plist template", failures)
    require(macos_plist, "<string>__VERSION__</string>", "macOS plist template", failures)
    require(macos_plist, "<string>__BUILD_NUMBER__</string>", "macOS plist template", failures)

    require(windows_manifest, f'Identity Name="{APP_ID}"', "Windows manifest template", failures)
    require(windows_manifest, 'Publisher="__PUBLISHER__"', "Windows manifest template", failures)
    require(windows_manifest, 'Version="__MSIX_VERSION__"', "Windows manifest template", failures)
    require(windows_manifest, "<DisplayName>CalcNova</DisplayName>", "Windows manifest template", failures)

    for path in (ios_plist_path, linux_meta_path, macos_plist_path, windows_manifest_path):
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

    if failures:
        print("Packaging metadata validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        "Validated Android/iOS application metadata and Desktop/Browser/Linux/macOS/Windows "
        "release identity contracts without requiring platform SDKs."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
