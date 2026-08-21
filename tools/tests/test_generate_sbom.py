#!/usr/bin/env python3

from __future__ import annotations

import base64
import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
GENERATOR = ROOT / "tools" / "generate_sbom.py"


def load_generator():
    spec = importlib.util.spec_from_file_location("generate_sbom", GENERATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load SBOM generator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def sample_assets() -> dict:
    sha512 = base64.b64encode(bytes(range(64))).decode("ascii")
    return {
        "version": 3,
        "targets": {
            "net10.0": {
                "Package.B/2.0.0": {"type": "package"},
                "Package.A/1.2.3": {
                    "type": "package",
                    "dependencies": {"Package.B": "2.0.0"},
                },
                "CalcNova.Core/2.8.3": {"type": "project"},
            }
        },
        "libraries": {
            "Package.B/2.0.0": {"type": "package"},
            "Package.A/1.2.3": {"type": "package", "sha512": sha512},
            "CalcNova.Core/2.8.3": {"type": "project"},
        },
        "project": {
            "version": "2.8.3",
            "restore": {"projectName": "CalcNova.Desktop"},
            "frameworks": {
                "net10.0": {
                    "dependencies": {"Package.A": {"target": "Package", "version": "[1.2.3, )"}}
                }
            },
        },
    }


class GenerateSbomTests(unittest.TestCase):
    def test_build_sbom_is_deterministic_and_tracks_nuget_packages(self) -> None:
        generator = load_generator()
        first = generator.build_sbom(sample_assets())
        second = generator.build_sbom(sample_assets())

        self.assertEqual(first, second)
        self.assertEqual("CycloneDX", first["bomFormat"])
        self.assertEqual("1.7", first["specVersion"])
        self.assertEqual("https://cyclonedx.org/schema/bom-1.7.schema.json", first["$schema"])
        self.assertEqual("CalcNova.Desktop", first["metadata"]["component"]["name"])
        self.assertEqual("2.8.3", first["metadata"]["component"]["version"])
        self.assertEqual(["Package.A", "Package.B"], [item["name"] for item in first["components"]])
        self.assertTrue(first["serialNumber"].startswith("urn:uuid:"))

    def test_build_sbom_converts_nuget_sha512_and_emits_dependency_graph(self) -> None:
        generator = load_generator()
        sbom = generator.build_sbom(sample_assets())
        components = {item["name"]: item for item in sbom["components"]}

        self.assertEqual(bytes(range(64)).hex(), components["Package.A"]["hashes"][0]["content"])
        self.assertEqual("SHA-512", components["Package.A"]["hashes"][0]["alg"])

        root_ref = sbom["metadata"]["component"]["bom-ref"]
        package_a = components["Package.A"]["bom-ref"]
        package_b = components["Package.B"]["bom-ref"]
        dependencies = {item["ref"]: item["dependsOn"] for item in sbom["dependencies"]}

        self.assertEqual([package_a], dependencies[root_ref])
        self.assertEqual([package_b], dependencies[package_a])
        self.assertEqual([], dependencies[package_b])

    def test_build_sbom_uses_explicit_component_overrides(self) -> None:
        generator = load_generator()
        sbom = generator.build_sbom(
            sample_assets(),
            component_name="CalcNova-win-x64",
            component_version="2.8.3",
        )
        self.assertEqual("CalcNova-win-x64", sbom["metadata"]["component"]["name"])
        self.assertEqual("2.8.3", sbom["metadata"]["component"]["version"])

    def test_unsupported_assets_format_is_rejected(self) -> None:
        generator = load_generator()
        assets = sample_assets()
        assets["version"] = 4
        with self.assertRaisesRegex(ValueError, "Unsupported project.assets.json format version"):
            generator.build_sbom(assets)

    def test_invalid_assets_shape_is_rejected(self) -> None:
        generator = load_generator()
        assets = sample_assets()
        del assets["libraries"]
        with self.assertRaisesRegex(ValueError, "libraries"):
            generator.build_sbom(assets)

    def test_generator_metadata_records_assets_format_contract(self) -> None:
        generator = load_generator()
        sbom = generator.build_sbom(sample_assets())
        properties = {
            item["name"]: item["value"]
            for item in sbom["metadata"]["properties"]
        }
        self.assertEqual("3", properties["calcnova:nuget-assets-format-version"])
        self.assertEqual("2", properties["calcnova:sbom-generator-version"])

    def test_write_sbom_is_stable_json_with_trailing_newline(self) -> None:
        generator = load_generator()
        sbom = generator.build_sbom(sample_assets())
        with tempfile.TemporaryDirectory() as temp_dir:
            output = Path(temp_dir) / "nested" / "CalcNova.sbom.cdx.json"
            generator.write_sbom(sbom, output)
            first = output.read_text(encoding="utf-8")
            generator.write_sbom(sbom, output)
            second = output.read_text(encoding="utf-8")

        self.assertEqual(first, second)
        self.assertTrue(first.endswith("\n"))
        self.assertIn('"bomFormat": "CycloneDX"', first)
        self.assertIn('"$schema": "https://cyclonedx.org/schema/bom-1.7.schema.json"', first)


if __name__ == "__main__":
    unittest.main()
