#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_completion_status.py"
IDENTITY_MODULE = ROOT / "tools" / "release_identity.py"


def load_module(path: Path, name: str):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"Unable to load {name}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class CompletionStatusValidatorTests(unittest.TestCase):
    def test_repository_completion_contract_is_valid(self) -> None:
        validator = load_module(VALIDATOR, "validate_completion_status")
        self.assertEqual([], validator.validate(ROOT))

    def test_release_identity_is_loaded_from_central_metadata(self) -> None:
        validator = load_module(VALIDATOR, "validate_completion_status_identity")
        identity_module = load_module(IDENTITY_MODULE, "release_identity_for_completion_test")
        identity = identity_module.load_release_identity(ROOT)
        contracts = validator.current_status_contracts(identity)
        self.assertIn(identity.display_version, contracts["README.md"][0])
        self.assertIn(identity.release_tag, contracts["README.md"][2])
        self.assertIn(identity.mobile_build_code, contracts["docs/VERSIONING.md"][2])

    def test_authoritative_status_inventory_is_stable(self) -> None:
        validator = load_module(VALIDATOR, "validate_completion_status_inventory")
        identity_module = load_module(IDENTITY_MODULE, "release_identity_for_inventory_test")
        identity = identity_module.load_release_identity(ROOT)
        self.assertEqual(
            {
                "README.md",
                "PROJECT_STATE.md",
                "CHANGELOG.md",
                "what_changed.md",
                "SECURITY.md",
                "SUPPORT.md",
                "CONTRIBUTING.md",
                "docs/README.md",
                "docs/FEATURES.md",
                "docs/ROADMAP.md",
                "docs/VERSIONING.md",
                "docs/RELEASE.md",
                "docs/RELEASE_READINESS_CHECKLIST.md",
                "docs/PLATFORM_SUPPORT.md",
                "docs/SOURCE_PREFLIGHT.md",
            },
            set(validator.current_status_contracts(identity)),
        )

    def test_obsolete_status_phrases_remain_forbidden(self) -> None:
        validator = load_module(VALIDATOR, "validate_completion_status_forbidden")
        forbidden = set(validator.FORBIDDEN_CURRENT_STATUS_MARKERS)
        self.assertIn("under active development", forbidden)
        self.assertIn("active pre-release development", forbidden)
        self.assertIn("## [unreleased]", forbidden)
        self.assertIn("remaining product/runtime work", forbidden)
        self.assertIn("remaining high-priority work", forbidden)
        self.assertIn("suggested development milestones", forbidden)
        self.assertIn("development mobile display version", forbidden)
        self.assertIn("## development environment", forbidden)


if __name__ == "__main__":
    unittest.main()
