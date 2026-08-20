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
        self.assertEqual(
            ".github/workflows/security-automation-validate.yml",
            validator.SECURITY_VALIDATE_WORKFLOW,
        )

    def test_missing_repository_reports_all_security_workflows(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertEqual(3, len(failures))

    def test_pull_request_target_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            codeql = root / validator.CODEQL_WORKFLOW
            dependency = root / validator.DEPENDENCY_REVIEW_WORKFLOW
            focused = root / validator.SECURITY_VALIDATE_WORKFLOW
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
            focused.write_text(
                "pull_request_target:\ncontents: read\nactions/checkout@v6\nactions/setup-python@v6\n"
                "python tools/validate_security_workflows.py .\n"
                "python tools/validate_dependency_security.py .\n"
                "python -m unittest tools.tests.test_validate_security_workflows\n"
                "python -m unittest tools.tests.test_validate_dependency_security\n",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertTrue(any("pull_request_target:" in failure for failure in failures))

    def test_focused_workflow_watches_dependency_policy_on_push_and_pr(self) -> None:
        validator = load_validator()
        source = (ROOT / validator.SECURITY_VALIDATE_WORKFLOW).read_text(encoding="utf-8")
        self.assertGreaterEqual(source.count('      - "Directory.Build.props"'), 2)
        self.assertIn("python tools/validate_dependency_security.py .", source)
        self.assertIn("python -m unittest tools.tests.test_validate_dependency_security", source)


if __name__ == "__main__":
    unittest.main()
