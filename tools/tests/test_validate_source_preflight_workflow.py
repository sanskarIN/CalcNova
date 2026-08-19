#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_source_preflight_workflow.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_source_preflight_workflow", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load source-preflight workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class SourcePreflightWorkflowValidatorTests(unittest.TestCase):
    def test_repository_source_preflight_workflow_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_missing_workflow_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))

        self.assertEqual(1, len(failures))
        self.assertIn("Missing source preflight workflow", failures[0])

    def test_narrow_or_privileged_workflow_fails(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            workflow = root / validator.WORKFLOW_PATH
            workflow.parent.mkdir(parents=True)
            workflow.write_text(
                "name: Source Preflight\n"
                "on:\n"
                "  push:\n"
                "    branches: [main]\n"
                "    paths:\n"
                "      - \"tools/**\"\n"
                "  pull_request_target:\n"
                "permissions:\n"
                "  contents: write\n"
                "jobs:\n"
                "  source-preflight:\n"
                "    runs-on: ubuntu-latest\n"
                "    timeout-minutes: 8\n"
                "    steps:\n"
                "      - uses: actions/checkout@v6\n"
                "      - uses: actions/setup-python@v6\n"
                "        with:\n"
                "          python-version: \"3.13\"\n"
                "      - run: python tools/release_preflight.py\n",
                encoding="utf-8",
            )

            failures = validator.validate(root)

        self.assertTrue(any("src/**" in failure for failure in failures))
        self.assertTrue(any("pull_request_target" in failure for failure in failures))
        self.assertTrue(any("contents: write" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
