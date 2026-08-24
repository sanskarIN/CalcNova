#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_packaging_metadata.py"
IDENTITY_MODULE = ROOT / "tools" / "release_identity.py"


def load_module(path: Path, name: str):
    spec = importlib.util.spec_from_file_location(name, path)
    if spec is None or spec.loader is None:
        raise RuntimeError(f"Unable to load {name}")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class PackagingMetadataValidatorTests(unittest.TestCase):
    def test_repository_packaging_metadata_is_valid(self) -> None:
        validator = load_module(VALIDATOR, "validate_packaging_metadata")
        self.assertEqual([], validator.validate(ROOT))

    def test_release_identity_comes_from_central_build_properties(self) -> None:
        validator = load_module(VALIDATOR, "validate_packaging_metadata")
        identity_module = load_module(IDENTITY_MODULE, "release_identity_for_packaging_test")
        identity = identity_module.load_release_identity(ROOT)
        self.assertEqual("in.sanskar.calcnova", validator.APP_ID)
        self.assertEqual("CalcNova", validator.APP_NAME)
        self.assertTrue(identity.display_version)
        self.assertTrue(identity.semver_version)
        self.assertTrue(identity.mobile_build_code)

    def test_linux_appstream_records_current_stable_release(self) -> None:
        identity_module = load_module(IDENTITY_MODULE, "release_identity_for_appstream_test")
        identity = identity_module.load_release_identity(ROOT)
        tree = ET.parse(ROOT / "packaging" / "linux" / "in.sanskar.calcnova.metainfo.xml")
        matching = [
            release
            for release in tree.getroot().findall("./releases/release")
            if release.attrib.get("version") == identity.display_version
        ]
        self.assertEqual(1, len(matching))
        self.assertEqual("stable", matching[0].attrib.get("type"))
        self.assertIn(f"CalcNova {identity.display_version}", " ".join(matching[0].itertext()))

    def test_missing_metadata_is_reported(self) -> None:
        validator = load_module(VALIDATOR, "validate_packaging_metadata")
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                """<Project><PropertyGroup>
<ProductDisplayVersion>2.9.5</ProductDisplayVersion>
<Version>2.9.5</Version>
<VersionPrefix>2.9.5</VersionPrefix>
<PackageVersion>2.9.5</PackageVersion>
<AssemblyVersion>2.9.5.0</AssemblyVersion>
<FileVersion>2.9.5.0</FileVersion>
<InformationalVersion>2.9.5</InformationalVersion>
</PropertyGroup></Project>""",
                encoding="utf-8",
            )
            failures = validator.validate(root)
        self.assertTrue(any("Missing packaging metadata file" in failure for failure in failures))


if __name__ == "__main__":
    unittest.main()
