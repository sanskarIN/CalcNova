#!/usr/bin/env python3
"""Perform deterministic repository checks that do not require a .NET build."""

from __future__ import annotations

import json
import re
import sys
import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]

REQUIRED_FILES = [
    "README.md",
    "LICENSE",
    "CHANGELOG.md",
    "CONTRIBUTING.md",
    "CODE_OF_CONDUCT.md",
    "SECURITY.md",
    "SUPPORT.md",
    "PROJECT_STATE.md",
    "what_changed.md",
    "docs/ARCHITECTURE.md",
    "docs/BUILDING.md",
    "docs/CALCULATION_ENGINE.md",
    "docs/FEATURES.md",
    "docs/KEYBOARD_SHORTCUTS.md",
    "docs/ACCESSIBILITY.md",
    "docs/PRIVACY.md",
    "docs/SECURITY.md",
    "docs/TESTING.md",
    "docs/RELEASE.md",
    "docs/PLATFORM_SUPPORT.md",
    "docs/TROUBLESHOOTING.md",
    "docs/DESIGN_SYSTEM.md",
    "docs/LOCALIZATION.md",
    "docs/ROADMAP.md",
    "assets/branding/calcnova-logo.svg",
    "assets/icons/calcnova-icon.svg",
]

FORBIDDEN_FILE_SUFFIXES = {
    ".keystore",
    ".jks",
    ".p12",
    ".pfx",
    ".mobileprovision",
    ".cer",
    ".der",
    ".key",
}

SENSITIVE_NAME_PATTERNS = [
    re.compile(r"(^|[._-])secrets?([._-]|$)", re.IGNORECASE),
    re.compile(r"service[-_]?account", re.IGNORECASE),
    re.compile(r"credentials?", re.IGNORECASE),
]


def error(message: str, errors: list[str]) -> None:
    errors.append(message)


def validate_required_files(errors: list[str]) -> None:
    for relative in REQUIRED_FILES:
        path = ROOT / relative
        if not path.is_file():
            error(f"Missing required repository file: {relative}", errors)


def validate_xml(errors: list[str]) -> None:
    for pattern in ("*.xml", "*.axaml", "*.plist", "*.storyboard"):
        for path in ROOT.rglob(pattern):
            if any(part in {"bin", "obj", ".git"} for part in path.parts):
                continue
            try:
                ET.parse(path)
            except ET.ParseError as exception:
                error(f"Invalid XML in {path.relative_to(ROOT)}: {exception}", errors)


def validate_json(errors: list[str]) -> None:
    for path in ROOT.rglob("*.json"):
        if any(part in {"bin", "obj", ".git"} for part in path.parts):
            continue
        try:
            json.loads(path.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, UnicodeDecodeError) as exception:
            error(f"Invalid JSON in {path.relative_to(ROOT)}: {exception}", errors)


def validate_no_sensitive_files(errors: list[str]) -> None:
    for path in ROOT.rglob("*"):
        if not path.is_file() or ".git" in path.parts:
            continue
        if path.suffix.lower() in FORBIDDEN_FILE_SUFFIXES:
            error(f"Sensitive/signing file must not be tracked: {path.relative_to(ROOT)}", errors)
            continue

        lower_name = path.name.lower()
        if lower_name in {".env", ".env.local", ".env.production", ".env.development"}:
            error(f"Environment-secret file must not be tracked: {path.relative_to(ROOT)}", errors)
            continue

        if any(pattern.search(path.name) for pattern in SENSITIVE_NAME_PATTERNS):
            allowed = path.as_posix().endswith("/.github/ISSUE_TEMPLATE/config.yml") or path.name.lower().endswith(".example")
            if not allowed:
                error(f"Credential-like filename requires manual review: {path.relative_to(ROOT)}", errors)


def validate_contact_links(errors: list[str]) -> None:
    canonical = {
        "repository": "https://github.com/sanskarIN/CalcNova",
        "profile": "https://www.github.com/sanskarIN",
        "coffee": "https://buymeacoffee.com/sanskarIN",
        "business": "sanskarin@outlook.in",
        "business_secondary": "sanskarin.business@gmail.com",
        "support": "supportramsandesh@gmail.com",
    }

    combined = "\n".join(
        path.read_text(encoding="utf-8", errors="replace")
        for path in [ROOT / "README.md", ROOT / "SUPPORT.md", ROOT / "SECURITY.md"]
        if path.is_file()
    )
    for label, value in canonical.items():
        if value not in combined:
            error(f"Canonical {label} contact/link is missing from top-level project documentation: {value}", errors)


def main() -> int:
    errors: list[str] = []
    validate_required_files(errors)
    validate_xml(errors)
    validate_json(errors)
    validate_no_sensitive_files(errors)
    validate_contact_links(errors)

    if errors:
        print("Repository validation failed:", file=sys.stderr)
        for item in errors:
            print(f"- {item}", file=sys.stderr)
        return 1

    print("CalcNova repository metadata, structured files, required docs, and secret-file guards are valid.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
