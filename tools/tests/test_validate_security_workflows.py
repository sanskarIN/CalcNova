#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_security_workflows.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_security_workflows", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load security workflow validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class SecurityWorkflowValidatorTests(unittest.TestCase):
    def test_repository_security_workflows_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_security_workflow_inventory_is_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(".github/workflows/codeql.yml", validator.CODEQL_WORKFLOW)
        self.assertEqual(".github/workflows/dependency-review.yml", validator.DEPENDENCY_REVIEW_WORKFLOW)

    def test_missing_repository_reports_both_workflows(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertEqual(2, len(failures))

    def test_pull_request_target_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            codeql = root / validator.CODEQL_WORKFLOW
            dependency = root / validator.DEPENDENCY_REVIEW_WORKFLOW
            codeql.parent.mkdir(parents=True)
            codeql.write_text(
                "pull_request_target:\ncontents: read\nsecurity-events: write\nactions/checkout@v6\n"
                "github/codeql-action/init@v4\nlanguages: csharp\nbuild-mode: none\n"
                "github/codeql-action/analyze@v4\ncategory: \"/language:csharp\"\n",
                encoding="utf-8",
            )
            dependency.write_text(
                "pull_request_target:\ncontents: read\nactions/checkout@v6\n"
                "actions/dependency-review-action@v5\nfail-on-severity: moderate\n",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertTrue(any("pull_request_target:" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
