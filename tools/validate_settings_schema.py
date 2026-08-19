#!/usr/bin/env python3
"""Validate CalcNova settings-schema migration contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

CURRENT_SCHEMA_VERSION = 1


def validate(root: Path) -> list[str]:
    schema_path = root / "src" / "CalcNova.Platform" / "Settings" / "AppSettingsSchema.cs"
    model_path = root / "src" / "CalcNova.Platform" / "Settings" / "AppSettings.cs"
    native_path = root / "src" / "CalcNova.Persistence" / "Settings" / "JsonSettingsRepository.cs"
    browser_path = root / "src" / "CalcNova.Browser" / "Services" / "BrowserSettingsRepository.cs"
    platform_tests_path = root / "tests" / "CalcNova.Platform.Tests" / "AppSettingsSchemaTests.cs"
    native_tests_path = root / "tests" / "CalcNova.Persistence.Tests" / "JsonSettingsRepositoryTests.cs"

    paths = (schema_path, model_path, native_path, browser_path, platform_tests_path, native_tests_path)
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing settings-schema source: {path}")

    if failures:
        return failures

    schema = schema_path.read_text(encoding="utf-8")
    model = model_path.read_text(encoding="utf-8")
    native = native_path.read_text(encoding="utf-8")
    browser = browser_path.read_text(encoding="utf-8")
    platform_tests = platform_tests_path.read_text(encoding="utf-8")
    native_tests = native_tests_path.read_text(encoding="utf-8")

    expected_schema_markers = (
        f"public const int CurrentVersion = {CURRENT_SCHEMA_VERSION};",
        "0 => settings with { SchemaVersion = CurrentVersion }",
        "CurrentVersion => settings",
        "settings.SchemaVersion} is newer than supported version",
    )
    for marker in expected_schema_markers:
        if marker not in schema:
            failures.append(f"AppSettingsSchema is missing migration marker: {marker}")

    if "public int SchemaVersion { get; init; } = AppSettingsSchema.CurrentVersion;" not in model:
        failures.append("AppSettings does not default SchemaVersion to the current schema.")

    for label, source in (("native settings repository", native), ("browser settings repository", browser)):
        if "settings = AppSettingsSchema.Normalize(settings);" not in source:
            failures.append(f"{label} does not normalize settings schema before validation.")

    for marker in (
        "Normalize_LegacyUnversionedSettings_MigratesToCurrentVersion",
        "Normalize_FutureSchema_RejectsUnsafeDowngrade",
        "Normalize_NegativeSchema_RejectsCorruptState",
    ):
        if marker not in platform_tests:
            failures.append(f"AppSettingsSchemaTests is missing boundary test: {marker}")

    for marker in (
        "Load_LegacySchemaVersionZero_MigratesToCurrentVersion",
        "Load_FutureSchemaVersion_IsRejected",
    ):
        if marker not in native_tests:
            failures.append(f"JsonSettingsRepositoryTests is missing migration test: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova settings schema contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Settings schema validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(f"Validated settings schema v{CURRENT_SCHEMA_VERSION} migration contracts across native and Browser storage.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
