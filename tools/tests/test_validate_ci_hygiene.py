#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest


ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_ci_hygiene.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_ci_hygiene", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load CI hygiene validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def write_canonical_workflows(root: Path, validator) -> None:
    workflow_dir = root / validator.WORKFLOW_DIR
    workflow_dir.mkdir(parents=True, exist_ok=True)
    for filename, markers in validator.CANONICAL_WORKFLOWS.items():
        (workflow_dir / filename).write_text("\n".join(markers) + "\n", encoding="utf-8")


class CiHygieneValidatorTests(unittest.TestCase):
    def test_repository_ci_hygiene_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_retired_template_workflow_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            write_canonical_workflows(root, validator)
            workflow = root / validator.WORKFLOW_DIR / "dotnet.yml"
            workflow.write_text("name: generic template\n", encoding="utf-8")
            failures = validator.validate(root)

        self.assertTrue(any("Retired generic workflow" in failure for failure in failures))

    def test_starter_placeholder_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            write_canonical_workflows(root, validator)
            workflow = root / validator.WORKFLOW_DIR / "custom.yml"
            workflow.write_text("env:\n  Solution_Name: your-solution-name\n", encoding="utf-8")
            failures = validator.validate(root)

        self.assertTrue(any("starter-template marker" in failure for failure in failures))

    def test_obsolete_checkout_or_setup_dotnet_major_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            write_canonical_workflows(root, validator)
            workflow = root / validator.WORKFLOW_DIR / "custom.yml"
            workflow.write_text(
                "steps:\n"
                "  - uses: actions/checkout@v4\n"
                "  - uses: actions/setup-dotnet@v5\n",
                encoding="utf-8",
            )
            failures = validator.validate(root)

        self.assertTrue(any("actions/checkout@v4" in failure for failure in failures))
        self.assertTrue(any("actions/setup-dotnet@v5" in failure for failure in failures))

    def test_canonical_sdk_downgrade_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            write_canonical_workflows(root, validator)
            build = root / validator.WORKFLOW_DIR / "build-test.yml"
            build.write_text(
                build.read_text(encoding="utf-8").replace("dotnet-version: 10.0.x", "dotnet-version: 8.0.x"),
                encoding="utf-8",
            )
            failures = validator.validate(root)

        self.assertTrue(any("build-test.yml" in failure and "10.0.x" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
