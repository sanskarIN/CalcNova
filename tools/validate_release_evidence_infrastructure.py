#!/usr/bin/env python3
"""Validate CalcNova structured release-evidence source and CI contracts."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    model_path = root / "tools" / "release_evidence.py"
    runner_path = root / "tools" / "run_release_evidence.py"
    verifier_path = root / "tools" / "verify_release_evidence.py"
    schema_path = root / "docs" / "release-evidence.schema.json"
    workflow_path = root / ".github" / "workflows" / "validation-evidence.yml"
    model_tests_path = root / "tools" / "tests" / "test_release_evidence.py"
    runner_tests_path = root / "tools" / "tests" / "test_run_release_evidence.py"
    verifier_tests_path = root / "tools" / "tests" / "test_verify_release_evidence.py"

    paths = (
        model_path,
        runner_path,
        verifier_path,
        schema_path,
        workflow_path,
        model_tests_path,
        runner_tests_path,
        verifier_tests_path,
    )
    failures: list[str] = []
    for path in paths:
        if not path.is_file():
            failures.append(f"Missing release-evidence source: {path}")
    if failures:
        return failures

    model = model_path.read_text(encoding="utf-8")
    runner = runner_path.read_text(encoding="utf-8")
    verifier = verifier_path.read_text(encoding="utf-8")
    workflow = workflow_path.read_text(encoding="utf-8")
    model_tests = model_tests_path.read_text(encoding="utf-8")
    runner_tests = runner_tests_path.read_text(encoding="utf-8")
    verifier_tests = verifier_tests_path.read_text(encoding="utf-8")

    try:
        schema = json.loads(schema_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as exception:
        failures.append(f"Release-evidence JSON schema is invalid JSON: {exception}")
        schema = {}

    if schema.get("properties", {}).get("schemaVersion", {}).get("const") != 1:
        failures.append("Release-evidence schema must require schemaVersion 1.")
    statuses = (
        schema.get("properties", {})
        .get("checks", {})
        .get("items", {})
        .get("properties", {})
        .get("status", {})
        .get("enum", [])
    )
    if statuses != ["PASS", "FAIL", "NOT RUN", "BLOCKED"]:
        failures.append("Release-evidence schema must preserve PASS/FAIL/NOT RUN/BLOCKED vocabulary in order.")

    for marker in (
        'PASS = "PASS"',
        'FAIL = "FAIL"',
        'NOT_RUN = "NOT RUN"',
        'BLOCKED = "BLOCKED"',
        "EvidenceStatus.NOT_RUN, EvidenceStatus.BLOCKED",
        '"schemaVersion": 1',
        "ids must be unique",
    ):
        if marker not in model:
            failures.append(f"Release evidence model is missing marker: {marker}")

    for marker in (
        '"source-hardening"',
        '"restore"',
        '"format"',
        '"build"',
        '"test"',
        '"desktop-publish"',
        '"browser-publish"',
        '"android-build"',
        '"ios-simulator-build"',
        '"runtime-accessibility"',
        '"responsive-layout-runtime"',
        '"signed-distribution"',
        "iOS simulator builds require a supported macOS/Xcode host.",
    ):
        if marker not in runner:
            failures.append(f"Release evidence runner is missing marker: {marker}")

    for marker in (
        'CORE_CHECKS = ("source-hardening", "restore", "format", "build", "test")',
        '"desktop": "desktop-publish"',
        '"browser": "browser-publish"',
        '"android": "android-build"',
        '"ios": "ios-simulator-build"',
        'if check.get("status") != "PASS"',
    ):
        if marker not in verifier:
            failures.append(f"Release evidence verifier is missing marker: {marker}")

    for job in ("core-desktop:", "browser:", "android:", "ios:"):
        if job not in workflow:
            failures.append(f"Validation-evidence workflow is missing job: {job}")
    for marker in (
        "run_release_evidence.py --scope core --platform desktop",
        "--scope core --require-platform desktop",
        "dotnet workload install wasm-tools",
        "--scope source --platform browser",
        "dotnet workload install android",
        "--scope source --platform android",
        "runs-on: macos-latest",
        "dotnet workload install ios",
        "--scope source --platform ios",
        "uses: actions/upload-artifact@v4",
        "if: always()",
    ):
        if marker not in workflow:
            failures.append(f"Validation-evidence workflow is missing marker: {marker}")

    for marker in (
        "test_valid_evidence_serializes_stable_schema",
        "test_duplicate_check_ids_are_rejected",
        "test_not_run_and_blocked_require_reasons",
    ):
        if marker not in model_tests:
            failures.append(f"Release evidence model tests are missing marker: {marker}")

    for marker in (
        "test_run_command_records_pass_and_log",
        "test_sequential_plan_blocks_downstream_checks_after_failure",
        "test_platform_plans_use_release_projects",
    ):
        if marker not in runner_tests:
            failures.append(f"Release evidence runner tests are missing marker: {marker}")

    for marker in (
        "test_core_requirements_include_source_restore_format_build_test",
        "test_verify_rejects_missing_blocked_and_not_run_required_entries",
        "test_load_evidence_rejects_invalid_status_and_duplicate_ids",
    ):
        if marker not in verifier_tests:
            failures.append(f"Release evidence verifier tests are missing marker: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova release-evidence infrastructure.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()
    failures = validate(Path(args.root).resolve())
    if failures:
        print("Release-evidence infrastructure validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated release-evidence model, runner, verifier, schema, tests, and platform CI jobs.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
