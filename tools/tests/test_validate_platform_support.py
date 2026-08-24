#!/usr/bin/env python3
"""Regression tests for CalcNova cross-platform source validation."""

from __future__ import annotations

import importlib.util
from pathlib import Path
import shutil
import tempfile
import unittest


ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_platform_support.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_platform_support", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load cross-platform source validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def copy_fixture(destination: Path, validator) -> None:
    paths = set(validator.FILE_MARKERS) | set(validator.REQUIRED_FILES)
    for relative_path in paths:
        source = ROOT / relative_path
        target = destination / relative_path
        if source.is_dir():
            target.mkdir(parents=True, exist_ok=True)
        else:
            target.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(source, target)


class PlatformSupportValidatorTests(unittest.TestCase):
    def test_repository_cross_platform_source_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_mobile_runtime_identifier_contracts_are_explicit(self) -> None:
        validator = load_validator()
        android = validator.FILE_MARKERS["src/CalcNova.Android/CalcNova.Android.csproj"]
        ios = validator.FILE_MARKERS["src/CalcNova.iOS/CalcNova.iOS.csproj"]
        self.assertIn(
            "<RuntimeIdentifiers>android-arm;android-arm64;android-x86;android-x64</RuntimeIdentifiers>",
            android,
        )
        self.assertIn(
            "<RuntimeIdentifiers>ios-arm64;iossimulator-arm64;iossimulator-x64</RuntimeIdentifiers>",
            ios,
        )

    def test_android_architecture_drift_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            copy_fixture(root, validator)
            project = root / "src/CalcNova.Android/CalcNova.Android.csproj"
            source = project.read_text(encoding="utf-8")
            source = source.replace(
                "android-arm;android-arm64;android-x86;android-x64",
                "android-arm64;android-x64",
            )
            project.write_text(source, encoding="utf-8")

            failures = validator.validate(root)

        self.assertTrue(any("RuntimeIdentifiers" in failure for failure in failures), failures)

    def test_mobile_build_code_drift_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            copy_fixture(root, validator)
            project = root / "src/CalcNova.Android/CalcNova.Android.csproj"
            source = project.read_text(encoding="utf-8")
            source = source.replace("<ApplicationVersion>20905</ApplicationVersion>", "<ApplicationVersion>20900</ApplicationVersion>")
            project.write_text(source, encoding="utf-8")

            failures = validator.validate(root)

        self.assertTrue(any("current mobile build marker" in failure for failure in failures), failures)

    def test_browser_offline_resource_drift_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            copy_fixture(root, validator)
            (root / "src/CalcNova.Browser/wwwroot/service-worker.js").unlink()

            failures = validator.validate(root)

        self.assertTrue(any("service-worker.js" in failure for failure in failures), failures)

    def test_missing_repository_reports_cross_platform_failures(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertGreaterEqual(len(failures), len(validator.FILE_MARKERS))


if __name__ == "__main__":
    unittest.main()
