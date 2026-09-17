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
        self.assertEqual("1.0.0", identity.display_version)
        self.assertEqual("1.0.0", identity.semver_version)
        self.assertEqual("10000", identity.mobile_build_code)
        self.assertEqual("v1.0.0", identity.release_tag)
        self.assertEqual("1.0.0.0", identity.assembly_version)

    def test_mobile_build_code_covers_multi_digit_components(self) -> None:
        module = load_module()
        self.assertEqual("30400", module.mobile_build_code_for("3.4.0"))
        self.assertEqual("30405", module.mobile_build_code_for("3.4.5"))
        self.assertEqual("71209", module.mobile_build_code_for("7.12.9"))
        self.assertEqual("10000", module.mobile_build_code_for("1.0.0"))

    def test_display_version_normalization_removes_numeric_leading_zeroes(self) -> None:
        module = load_module()
        self.assertEqual("3.4.5", module.normalize_display_version("3.4.05"))
        self.assertEqual("1.0.0", module.normalize_display_version("1.0.0"))

    def test_mismatched_central_versions_fail_closed(self) -> None:
        module = load_module()
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            (root / "Directory.Build.props").write_text(
                """<Project><PropertyGroup>
<ProductDisplayVersion>1.0.0</ProductDisplayVersion>
<Version>3.4.5</Version>
<VersionPrefix>3.4.5</VersionPrefix>
<PackageVersion>3.4.5</PackageVersion>
<AssemblyVersion>3.4.5.0</AssemblyVersion>
<FileVersion>3.4.5.0</FileVersion>
<InformationalVersion>1.0.0</InformationalVersion>
</PropertyGroup></Project>""",
                encoding="utf-8",
            )
            with self.assertRaises(ValueError):
                module.load_release_identity(root)


if __name__ == "__main__":
    unittest.main()
