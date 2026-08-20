#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_workflow.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_release_workflow", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load release workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseWorkflowValidatorTests(unittest.TestCase):
    def test_repository_release_workflow_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_desktop_release_target_inventory_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(
            (
                ("windows-latest", "win-x64"),
                ("windows-latest", "win-arm64"),
                ("ubuntu-latest", "linux-x64"),
                ("ubuntu-latest", "linux-arm64"),
                ("macos-latest", "osx-x64"),
                ("macos-latest", "osx-arm64"),
            ),
            validator.DESKTOP_RELEASE_TARGETS,
        )

    def test_each_desktop_os_has_x64_and_arm64_release_targets(self) -> None:
        validator = load_validator()
        targets = set(validator.DESKTOP_RELEASE_TARGETS)
        self.assertTrue({("windows-latest", "win-x64"), ("windows-latest", "win-arm64")} <= targets)
        self.assertTrue({("ubuntu-latest", "linux-x64"), ("ubuntu-latest", "linux-arm64")} <= targets)
        self.assertTrue({("macos-latest", "osx-x64"), ("macos-latest", "osx-arm64")} <= targets)


if __name__ == "__main__":
    unittest.main()
