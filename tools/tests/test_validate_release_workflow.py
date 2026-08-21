#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_workflow.py"
RELEASE_WORKFLOW = ROOT / ".github" / "workflows" / "release.yml"


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

    def test_release_artifacts_include_cyclonedx_sboms(self) -> None:
        source = RELEASE_WORKFLOW.read_text(encoding="utf-8")
        self.assertIn(
            "python tools/generate_sbom.py --assets src/CalcNova.Desktop/obj/project.assets.json",
            source,
        )
        self.assertIn("CalcNova-${{ matrix.rid }}.sbom.cdx.json", source)
        self.assertIn(
            "python tools/generate_sbom.py --assets src/CalcNova.Browser/obj/project.assets.json",
            source,
        )
        self.assertIn("CalcNova-browser.sbom.cdx.json", source)
        self.assertIn(
            "python tools/generate_sbom.py --assets src/CalcNova.Android/obj/project.assets.json",
            source,
        )
        self.assertIn("CalcNova-android.sbom.cdx.json", source)
        self.assertGreaterEqual(source.count("actions/setup-python@v6"), 4)

    def test_release_provenance_uses_current_attest_action_and_scoped_permissions(self) -> None:
        source = RELEASE_WORKFLOW.read_text(encoding="utf-8")
        self.assertIn("permissions:\n  contents: read", source)
        self.assertIn("actions/attest@v4", source)
        self.assertEqual(1, source.count("contents: write"))
        self.assertEqual(1, source.count("id-token: write"))
        self.assertEqual(1, source.count("attestations: write"))
        self.assertEqual(1, source.count("artifact-metadata: write"))
        self.assertIn("subject-path: release-assets/**/*", source)

    def test_release_checksums_use_published_flat_filenames(self) -> None:
        source = RELEASE_WORKFLOW.read_text(encoding="utf-8")
        self.assertIn("- name: Validate release asset filenames", source)
        self.assertIn("Duplicate release asset filenames are not allowed:", source)
        self.assertIn("SHA256SUMS.txt is a reserved release asset filename.", source)
        self.assertIn('printf \'%s  %s\\n\' "$digest" "$(basename "$file")" >> SHA256SUMS.txt', source)
        self.assertNotIn("xargs -0 sha256sum > SHA256SUMS.txt", source)


if __name__ == "__main__":
    unittest.main()
