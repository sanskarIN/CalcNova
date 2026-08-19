#!/usr/bin/env python3
"""Tests for the SDK-independent CalcNova release-tag validator."""

from __future__ import annotations

import importlib.util
import unittest
from pathlib import Path


MODULE_PATH = Path(__file__).resolve().parents[1] / "validate_release_tag.py"
SPEC = importlib.util.spec_from_file_location("validate_release_tag", MODULE_PATH)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"Could not load {MODULE_PATH}")

MODULE = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(MODULE)
is_valid_release_tag = MODULE.is_valid_release_tag


class ReleaseTagValidationTests(unittest.TestCase):
    def test_accepts_stable_semantic_versions(self) -> None:
        for tag in ("v0.1.0", "v1.0.0", "v2.8.3", "v10.25.300"):
            with self.subTest(tag=tag):
                self.assertTrue(is_valid_release_tag(tag))

    def test_product_display_version_2_8_03_uses_normalized_semver_tag(self) -> None:
        self.assertTrue(is_valid_release_tag("v2.8.3"))
        self.assertFalse(is_valid_release_tag("v2.8.03"))

    def test_accepts_semver_prerelease_versions(self) -> None:
        for tag in ("v1.0.0-alpha", "v1.0.0-alpha.1", "v2.4.0-rc.12"):
            with self.subTest(tag=tag):
                self.assertTrue(is_valid_release_tag(tag))

    def test_accepts_build_metadata(self) -> None:
        for tag in ("v1.0.0+build.7", "v1.2.3-rc.1+sha.abcdef"):
            with self.subTest(tag=tag):
                self.assertTrue(is_valid_release_tag(tag))

    def test_rejects_missing_v_prefix_or_components(self) -> None:
        for tag in ("1.2.3", "v1.2", "v1", "v"):
            with self.subTest(tag=tag):
                self.assertFalse(is_valid_release_tag(tag))

    def test_rejects_leading_zero_numeric_identifiers(self) -> None:
        for tag in ("v01.2.3", "v1.02.3", "v1.2.03", "v1.2.3-01"):
            with self.subTest(tag=tag):
                self.assertFalse(is_valid_release_tag(tag))

    def test_rejects_empty_or_malformed_identifiers(self) -> None:
        for tag in (
            "",
            "v1.2.3-",
            "v1.2.3-alpha..1",
            "v1.2.3+",
            "v1.2.3+build..7",
            "v1.2.3/evil",
            "v1.2.3 alpha",
        ):
            with self.subTest(tag=tag):
                self.assertFalse(is_valid_release_tag(tag))


if __name__ == "__main__":
    unittest.main()
