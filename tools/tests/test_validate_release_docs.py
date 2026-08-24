#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_release_docs.py"
IDENTITY_MODULE = ROOT / "tools" / "release_identity.py"


def load_module(path: Path, name: str):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"Unable to load {name}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseDocumentationValidatorTests(unittest.TestCase):
    def current_markers(self):
        validator = load_module(VALIDATOR, "validate_release_docs")
        identity_module = load_module(IDENTITY_MODULE, "release_identity_for_release_docs")
        identity = identity_module.load_release_identity(ROOT)
        return validator, identity, validator.required_markers(identity)

    def test_repository_release_documentation_is_valid(self) -> None:
        validator, _, _ = self.current_markers()
        self.assertEqual([], validator.validate(ROOT))

    def test_release_contract_tracks_four_state_evidence_vocabulary(self) -> None:
        _, _, markers = self.current_markers()
        self.assertIn("PASS / FAIL / BLOCKED / NOT RUN", markers["docs/RELEASE.md"])

    def test_release_contract_tracks_current_identity(self) -> None:
        _, identity, markers = self.current_markers()
        release_markers = markers["docs/RELEASE.md"]
        self.assertIn(f"# CalcNova {identity.display_version} Release Process", release_markers)
        self.assertIn(f"python tools/release_preflight.py --tag {identity.release_tag}", release_markers)
        self.assertIn(f"release tag: `{identity.release_tag}`", release_markers)

    def test_release_contract_includes_versioning_and_2_9_0_checkpoint(self) -> None:
        _, _, markers = self.current_markers()
        self.assertIn("docs/VERSIONING.md", markers)
        self.assertIn("docs/releases/2.9.0.md", markers)

    def test_release_contract_protects_security_and_provenance_guides(self) -> None:
        _, _, markers = self.current_markers()
        self.assertIn("docs/ARTIFACT_PROVENANCE.md", markers)
        self.assertIn("docs/SECURITY_AUTOMATION.md", markers)
        provenance = markers["docs/ARTIFACT_PROVENANCE.md"]
        security = markers["docs/SECURITY_AUTOMATION.md"]
        self.assertIn("CycloneDX 1.7", provenance)
        self.assertIn("https://cyclonedx.org/schema/bom-1.7.schema.json", provenance)
        self.assertIn("tools/generate_sbom.py", provenance)
        self.assertIn(".sbom.cdx.json", provenance)
        self.assertIn("top-level format version `3`", provenance)
        self.assertIn("tools/tests/test_generate_sbom.py", provenance)
        self.assertIn("sha256sum -c SHA256SUMS.txt", provenance)
        self.assertIn("artifact-metadata: write", provenance)
        self.assertIn("<NuGetAuditMode>all</NuGetAuditMode>", security)
        self.assertIn("python tools/validate_dependency_security.py .", security)

    def test_current_state_and_handoff_track_security_release_contracts(self) -> None:
        _, _, markers = self.current_markers()
        project_state = markers["PROJECT_STATE.md"]
        what_changed = markers["what_changed.md"]
        self.assertIn("artifact-metadata: write", project_state)
        self.assertIn("release-assets/**/*", project_state)
        self.assertIn("<NuGetAuditMode>all</NuGetAuditMode>", what_changed)
        self.assertIn("artifact-metadata: write", what_changed)

    def test_readiness_contract_tracks_desktop_architectures(self) -> None:
        _, _, markers = self.current_markers()
        readiness = markers["docs/RELEASE_READINESS_CHECKLIST.md"]
        self.assertIn("Windows x64: PASS / FAIL / BLOCKED / NOT RUN", readiness)
        self.assertIn("Windows ARM64: PASS / FAIL / BLOCKED / NOT RUN", readiness)
        self.assertIn("Linux ARM64: PASS / FAIL / BLOCKED / NOT RUN", readiness)
        self.assertIn("macOS ARM64: PASS / FAIL / BLOCKED / NOT RUN", readiness)
        self.assertIn("SBOM/checksum/provenance publication: PASS / FAIL / BLOCKED / NOT RUN", readiness)

    def test_missing_release_documents_are_reported(self) -> None:
        validator, _, markers = self.current_markers()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text((ROOT / "Directory.Build.props").read_text(encoding="utf-8"), encoding="utf-8")
            failures = validator.validate(root)
        self.assertEqual(len(markers), len(failures))


if __name__ == "__main__":
    unittest.main()
