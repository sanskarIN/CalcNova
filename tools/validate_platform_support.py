#!/usr/bin/env python3
"""Validate CalcNova's cross-platform source composition without platform SDKs."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

try:
    from tools.release_identity import load_release_identity
except ModuleNotFoundError:  # Direct execution via `python tools/validate_platform_support.py`.
    from release_identity import load_release_identity


FILE_MARKERS: dict[str, tuple[str, ...]] = {
    "src/CalcNova.Desktop/CalcNova.Desktop.csproj": (
        "<TargetFramework>net10.0</TargetFramework>",
        '<PackageReference Include="Avalonia.Desktop" />',
        '<ProjectReference Include="../CalcNova.App/CalcNova.App.csproj" />',
        '<ProjectReference Include="../CalcNova.Persistence/CalcNova.Persistence.csproj" />',
    ),
    "src/CalcNova.Desktop/Program.cs": (
        ".UsePlatformDetect()",
        "SqliteCalculationHistoryRepository",
        "JsonSettingsRepository",
        "DesktopExternalLinkService",
        "AvaloniaClipboardService",
        "JsonCurrencyRateCache",
    ),
    "src/CalcNova.Browser/CalcNova.Browser.csproj": (
        '<Project Sdk="Microsoft.NET.Sdk.Browser">',
        "<TargetFramework>net10.0-browser</TargetFramework>",
        '<PackageReference Include="Avalonia.Browser" />',
        '<ProjectReference Include="../CalcNova.App/CalcNova.App.csproj" />',
    ),
    "src/CalcNova.Browser/Program.cs": (
        "BrowserHistoryRepository",
        "BrowserSettingsRepository",
        "BrowserExternalLinkService",
        "AvaloniaClipboardService",
        "BrowserCurrencyRateCache",
        'StartBrowserAppAsync("out")',
    ),
    "src/CalcNova.Android/CalcNova.Android.csproj": (
        "<TargetFramework>net10.0-android</TargetFramework>",
        "<RuntimeIdentifiers>android-arm;android-arm64;android-x86;android-x64</RuntimeIdentifiers>",
        "<ApplicationId>in.sanskar.calcnova</ApplicationId>",
        "<ApplicationDisplayVersion>$(ProductDisplayVersion)</ApplicationDisplayVersion>",
        '<PackageReference Include="Avalonia.Android" />',
        '<ProjectReference Include="../CalcNova.App/CalcNova.App.csproj" />',
        '<ProjectReference Include="../CalcNova.Persistence/CalcNova.Persistence.csproj" />',
    ),
    "src/CalcNova.Android/MainActivity.cs": (
        "FilesDir?.AbsolutePath",
        "SqliteCalculationHistoryRepository",
        "JsonSettingsRepository",
        "AndroidExternalLinkService",
        "AvaloniaClipboardService",
        "JsonCurrencyRateCache",
    ),
    "src/CalcNova.iOS/CalcNova.iOS.csproj": (
        "<TargetFramework>net10.0-ios</TargetFramework>",
        "<RuntimeIdentifiers>ios-arm64;iossimulator-arm64;iossimulator-x64</RuntimeIdentifiers>",
        "<ApplicationId>in.sanskar.calcnova</ApplicationId>",
        "<ApplicationDisplayVersion>$(ProductDisplayVersion)</ApplicationDisplayVersion>",
        '<PackageReference Include="Avalonia.iOS" />',
        '<ProjectReference Include="../CalcNova.App/CalcNova.App.csproj" />',
        '<ProjectReference Include="../CalcNova.Persistence/CalcNova.Persistence.csproj" />',
    ),
    "src/CalcNova.iOS/AppDelegate.cs": (
        "Environment.SpecialFolder.LocalApplicationData",
        "SqliteCalculationHistoryRepository",
        "JsonSettingsRepository",
        "IosExternalLinkService",
        "AvaloniaClipboardService",
        "JsonCurrencyRateCache",
    ),
    "src/CalcNova.Platform/CalcNova.Platform.csproj": (
        "<TargetFramework>net10.0</TargetFramework>",
        '<ProjectReference Include="../CalcNova.Core/CalcNova.Core.csproj" />',
    ),
}

REQUIRED_FILES: tuple[str, ...] = (
    "Directory.Build.props",
    "src/CalcNova.Platform/Clipboard/IClipboardService.cs",
    "src/CalcNova.Platform/External/IExternalLinkService.cs",
    "src/CalcNova.Platform/History/ICalculationHistoryRepository.cs",
    "src/CalcNova.Platform/Settings/ISettingsRepository.cs",
    "src/CalcNova.Browser/wwwroot/index.html",
    "src/CalcNova.Browser/wwwroot/manifest.webmanifest",
    "src/CalcNova.Browser/wwwroot/service-worker.js",
    "src/CalcNova.Browser/wwwroot/icons",
    "docs/PLATFORM_SUPPORT.md",
)


def validate(root: Path) -> list[str]:
    failures: list[str] = []

    try:
        identity = load_release_identity(root)
    except ValueError as exception:
        identity = None
        failures.append(str(exception))

    for relative_path, markers in FILE_MARKERS.items():
        path = root / relative_path
        if not path.is_file():
            failures.append(f"Missing cross-platform source file: {relative_path}")
            continue

        source = path.read_text(encoding="utf-8")
        for marker in markers:
            if marker not in source:
                failures.append(f"{relative_path} is missing cross-platform marker: {marker}")

        if identity is not None and relative_path in {
            "src/CalcNova.Android/CalcNova.Android.csproj",
            "src/CalcNova.iOS/CalcNova.iOS.csproj",
        }:
            version_marker = f"<ApplicationVersion>{identity.mobile_build_code}</ApplicationVersion>"
            if version_marker not in source:
                failures.append(
                    f"{relative_path} is missing current mobile build marker: {version_marker}"
                )

    for relative_path in REQUIRED_FILES:
        path = root / relative_path
        if not path.exists():
            failures.append(f"Missing required cross-platform resource: {relative_path}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova cross-platform source contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    failures = validate(root)
    if failures:
        print("Cross-platform source validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    identity = load_release_identity(root)
    print(
        "Validated Desktop, Browser/PWA, Android, iOS, shared platform abstractions, "
        f"mobile runtime identifiers/build code {identity.mobile_build_code}, persistence, "
        "clipboard, and external-link composition."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
