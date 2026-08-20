#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_docs.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_release_docs", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load release documentation validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseDocumentationValidatorTests(unittest.TestCase):
    def test_repository_release_documentation_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_release_contract_tracks_four_state_evidence_vocabulary(self) -> None:
        validator = load_validator()
        release_markers = validator.REQUIRED_MARKERS["docs/RELEASE.md"]
        self.assertIn("PASS / FAIL / BLOCKED / NOT RUN", release_markers)

    def test_release_contract_tracks_2_8_03_identity(self) -> None:
        validator = load_validator()
        release_markers = validator.REQUIRED_MARKERS["docs/RELEASE.md"]
        self.assertIn("# CalcNova 2.8.03 Release Process", release_markers)
        self.assertIn("python tools/release_preflight.py --tag v2.8.3", release_markers)
        self.assertIn("normalized release tag: `v2.8.3`", release_markers)

    def test_release_contract_includes_versioning_guide(self) -> None:
        validator = load_validator()
        self.assertIn("docs/VERSIONING.md", validator.REQUIRED_MARKERS)

    def test_release_contract_protects_security_and_provenance_guides(self) -> None:
        validator = load_validator()
        self.assertIn("docs/ARTIFACT_PROVENANCE.md", validator.REQUIRED_MARKERS)
        self.assertIn("docs/SECURITY_AUTOMATION.md", validator.REQUIRED_MARKERS)
        provenance = validator.REQUIRED_MARKERS["docs/ARTIFACT_PROVENANCE.md"]
        security = validator.REQUIRED_MARKERS["docs/SECURITY_AUTOMATION.md"]
        self.assertIn("sha256sum -c SHA256SUMS.txt", provenance)
        self.assertIn("artifact-metadata: write", provenance)
        self.assertIn("<NuGetAuditMode>all</NuGetAuditMode>", security)
        self.assertIn("python tools/validate_dependency_security.py .", security)

    def test_current_state_and_handoff_track_security_release_contracts(self) -> None:
        validator = load_validator()
        project_state = validator.REQUIRED_MARKERS["PROJECT_STATE.md"]
        what_changed = validator.REQUIRED_MARKERS["what_changed.md"]
        self.assertIn("artifact-metadata: write", project_state)
        self.assertIn("release-assets/**/*", project_state)
        self.assertIn("<NuGetAuditMode>all</NuGetAuditMode>", what_changed)
        self.assertIn("artifact-metadata: write", what_changed)

    def test_missing_release_documents_are_reported(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            failures = validator.validate(Path(directory))
        self.assertEqual(len(validator.REQUIRED_MARKERS), len(failures))


if __name__ == "__main__":
    unittest.main()
