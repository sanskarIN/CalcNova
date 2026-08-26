#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest


ROOT = Path(__file__).resolve().parents[2]
MODULE_PATH = ROOT / "tools" / "release_identity.py"


def load_module():
    spec = importlib.util.spec_from_file_location("release_identity", MODULE_PATH)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load release identity helper")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class ReleaseIdentityTests(unittest.TestCase):
    def test_repository_release_identity_is_consistent(self) -> None:
        module = load_module()
        identity = module.load_release_identity(ROOT)
        self.assertEqual("2.9.7", identity.display_version)
        self.assertEqual("2.9.7", identity.semver_version)
        self.assertEqual("20907", identity.mobile_build_code)
        self.assertEqual("v2.9.7", identity.release_tag)
        self.assertEqual("2.9.7.0", identity.assembly_version)

    def test_mobile_build_code_supports_2_9_series(self) -> None:
        module = load_module()
        self.assertEqual("20900", module.mobile_build_code_for("2.9.0"))
        self.assertEqual("20905", module.mobile_build_code_for("2.9.5"))
        self.assertEqual("20906", module.mobile_build_code_for("2.9.6"))
        self.assertEqual("20907", module.mobile_build_code_for("2.9.7"))

    def test_display_version_normalization_removes_numeric_leading_zeroes(self) -> None:
        module = load_module()
        self.assertEqual("2.8.3", module.normalize_display_version("2.8.03"))
        self.assertEqual("2.9.7", module.normalize_display_version("2.9.7"))

    def test_mismatched_central_versions_fail_closed(self) -> None:
        module = load_module()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                """<Project><PropertyGroup>
<ProductDisplayVersion>2.9.7</ProductDisplayVersion>
<Version>2.9.6</Version>
<VersionPrefix>2.9.6</VersionPrefix>
<PackageVersion>2.9.6</PackageVersion>
<AssemblyVersion>2.9.6.0</AssemblyVersion>
<FileVersion>2.9.6.0</FileVersion>
<InformationalVersion>2.9.7</InformationalVersion>
</PropertyGroup></Project>""",
                encoding="utf-8",
            )
            with self.assertRaises(ValueError):
                module.load_release_identity(root)


if __name__ == "__main__":
    unittest.main()
