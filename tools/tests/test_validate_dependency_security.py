#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_dependency_security.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_dependency_security", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load dependency security validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class DependencySecurityValidatorTests(unittest.TestCase):
    def test_repository_dependency_security_policy_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_expected_policy_values_are_stable(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {
                "TreatWarningsAsErrors": "true",
                "NuGetAudit": "true",
                "NuGetAuditMode": "all",
                "NuGetAuditLevel": "moderate",
            },
            validator.EXPECTED_PROPERTIES,
        )
        self.assertEqual({"NU1901", "NU1902", "NU1903", "NU1904"}, validator.AUDIT_WARNING_CODES)

    def test_weakened_audit_policy_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                "<Project><PropertyGroup>"
                "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>"
                "<NuGetAudit>false</NuGetAudit>"
                "<NuGetAuditMode>direct</NuGetAuditMode>"
                "<NuGetAuditLevel>high</NuGetAuditLevel>"
                "</PropertyGroup></Project>",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertGreaterEqual(len(failures), 3)
        self.assertTrue(any("NuGetAudit=true" in failure for failure in failures))
        self.assertTrue(any("NuGetAuditMode=all" in failure for failure in failures))
        self.assertTrue(any("NuGetAuditLevel=moderate" in failure for failure in failures))

    def test_nuget_audit_warning_suppression_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                "<Project><PropertyGroup>"
                "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>"
                "<NuGetAudit>true</NuGetAudit>"
                "<NuGetAuditMode>all</NuGetAuditMode>"
                "<NuGetAuditLevel>moderate</NuGetAuditLevel>"
                "<NoWarn>$(NoWarn);NU1903,NU1904</NoWarn>"
                "<WarningsNotAsErrors>NU1605;NU1902;$(WarningsNotAsErrors)</WarningsNotAsErrors>"
                "</PropertyGroup></Project>",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertTrue(any("NU1902" in failure for failure in failures))
        self.assertTrue(any("NU1903" in failure for failure in failures))
        self.assertTrue(any("NU1904" in failure for failure in failures))

    def test_unrelated_warning_suppression_remains_allowed(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                "<Project><PropertyGroup>"
                "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>"
                "<NuGetAudit>true</NuGetAudit>"
                "<NuGetAuditMode>all</NuGetAuditMode>"
                "<NuGetAuditLevel>moderate</NuGetAuditLevel>"
                "<NoWarn>$(NoWarn);NU1605</NoWarn>"
                "</PropertyGroup></Project>",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertEqual([], failures)


if __name__ == "__main__":
    unittest.main()
