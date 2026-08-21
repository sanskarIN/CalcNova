#!/usr/bin/env python3
"""Generate a deterministic CycloneDX SBOM from a restored .NET project.assets.json file."""

from __future__ import annotations

import argparse
import base64
import binascii
import json
import sys
import uuid
from pathlib import Path
from typing import Any
from urllib.parse import quote


CYCLONEDX_SPEC_VERSION = "1.5"
SBOM_GENERATOR_VERSION = "1"


def _package_identity(library_key: str) -> tuple[str, str] | None:
    """Split the NuGet library key (`Package.Id/1.2.3`) into name/version."""
    if "/" not in library_key:
        return None
    name, version = library_key.rsplit("/", 1)
    if not name or not version:
        return None
    return name, version


def _nuget_purl(name: str, version: str) -> str:
    return f"pkg:nuget/{quote(name, safe='')}@{quote(version, safe='')}"


def _sha512_hex(value: Any) -> str | None:
    """Convert NuGet's base64 SHA-512 value to CycloneDX's hexadecimal form."""
    if not isinstance(value, str) or not value:
        return None
    try:
        raw = base64.b64decode(value, validate=True)
    except (binascii.Error, ValueError):
        return None
    if len(raw) != 64:
        return None
    return raw.hex()


def _project_name(assets: dict[str, Any]) -> str:
    project = assets.get("project")
    if isinstance(project, dict):
        restore = project.get("restore")
        if isinstance(restore, dict):
            name = restore.get("projectName")
            if isinstance(name, str) and name.strip():
                return name.strip()
    return "CalcNova"


def _project_version(assets: dict[str, Any]) -> str:
    project = assets.get("project")
    if isinstance(project, dict):
        version = project.get("version")
        if isinstance(version, str) and version.strip():
            return version.strip()
    return "0.0.0"


def build_sbom(
    assets: dict[str, Any],
    *,
    component_name: str | None = None,
    component_version: str | None = None,
) -> dict[str, Any]:
    """Build a deterministic CycloneDX 1.5 JSON document from NuGet restore assets."""
    libraries = assets.get("libraries")
    if not isinstance(libraries, dict):
        raise ValueError("project.assets.json must contain a 'libraries' object")

    name = component_name.strip() if component_name and component_name.strip() else _project_name(assets)
    version = (
        component_version.strip()
        if component_version and component_version.strip()
        else _project_version(assets)
    )

    components: list[dict[str, Any]] = []
    package_refs: dict[str, str] = {}

    for library_key in sorted(libraries, key=str.casefold):
        metadata = libraries[library_key]
        if not isinstance(metadata, dict) or metadata.get("type") != "package":
            continue

        identity = _package_identity(library_key)
        if identity is None:
            continue
        package_name, package_version = identity
        purl = _nuget_purl(package_name, package_version)
        package_refs[library_key.casefold()] = purl

        component: dict[str, Any] = {
            "type": "library",
            "bom-ref": purl,
            "name": package_name,
            "version": package_version,
            "purl": purl,
        }

        hash_hex = _sha512_hex(metadata.get("sha512"))
        if hash_hex is not None:
            component["hashes"] = [{"alg": "SHA-512", "content": hash_hex}]

        components.append(component)

    components.sort(key=lambda item: (str(item["name"]).casefold(), str(item["version"])))

    root_ref = f"pkg:generic/{quote(name, safe='')}@{quote(version, safe='')}"
    direct_names: set[str] = set()
    project = assets.get("project")
    if isinstance(project, dict):
        frameworks = project.get("frameworks")
        if isinstance(frameworks, dict):
            for framework in frameworks.values():
                if not isinstance(framework, dict):
                    continue
                dependencies = framework.get("dependencies")
                if isinstance(dependencies, dict):
                    direct_names.update(str(dep).casefold() for dep in dependencies)

    dependency_edges: dict[str, set[str]] = {ref: set() for ref in package_refs.values()}
    direct_refs: set[str] = set()
    targets = assets.get("targets")
    if isinstance(targets, dict):
        for target in targets.values():
            if not isinstance(target, dict):
                continue

            target_by_name: dict[str, str] = {}
            for library_key, metadata in target.items():
                if not isinstance(metadata, dict) or metadata.get("type") != "package":
                    continue
                identity = _package_identity(library_key)
                if identity is None:
                    continue
                resolved_ref = package_refs.get(library_key.casefold())
                if resolved_ref is not None:
                    target_by_name[identity[0].casefold()] = resolved_ref
                    if identity[0].casefold() in direct_names:
                        direct_refs.add(resolved_ref)

            for library_key, metadata in target.items():
                if not isinstance(metadata, dict) or metadata.get("type") != "package":
                    continue
                source_ref = package_refs.get(library_key.casefold())
                if source_ref is None:
                    continue
                dependencies = metadata.get("dependencies")
                if not isinstance(dependencies, dict):
                    continue
                for dependency_name in dependencies:
                    dependency_ref = target_by_name.get(str(dependency_name).casefold())
                    if dependency_ref is not None and dependency_ref != source_ref:
                        dependency_edges[source_ref].add(dependency_ref)

    if direct_names and not direct_refs:
        # Some assets omit target-level package metadata. Fall back to matching the
        # unique restored package component by its case-insensitive NuGet ID.
        for component in components:
            if str(component["name"]).casefold() in direct_names:
                direct_refs.add(str(component["bom-ref"]))

    dependencies = [{"ref": root_ref, "dependsOn": sorted(direct_refs)}]
    dependencies.extend(
        {"ref": ref, "dependsOn": sorted(dependency_edges.get(ref, set()))}
        for ref in sorted(dependency_edges)
    )

    serial_seed = "|".join(
        [name, version, *(f"{item['name']}@{item['version']}" for item in components)]
    )
    serial = uuid.uuid5(uuid.NAMESPACE_URL, f"https://github.com/sanskarIN/CalcNova#sbom:{serial_seed}")

    return {
        "bomFormat": "CycloneDX",
        "specVersion": CYCLONEDX_SPEC_VERSION,
        "serialNumber": f"urn:uuid:{serial}",
        "version": 1,
        "metadata": {
            "component": {
                "type": "application",
                "bom-ref": root_ref,
                "name": name,
                "version": version,
            },
            "properties": [
                {"name": "calcnova:sbom-generator", "value": "tools/generate_sbom.py"},
                {"name": "calcnova:sbom-generator-version", "value": SBOM_GENERATOR_VERSION},
            ],
        },
        "components": components,
        "dependencies": dependencies,
    }


def load_assets(path: Path) -> dict[str, Any]:
    try:
        data = json.loads(path.read_text(encoding="utf-8"))
    except FileNotFoundError as exc:
        raise ValueError(f"Restore assets file does not exist: {path}") from exc
    except json.JSONDecodeError as exc:
        raise ValueError(f"Restore assets file is not valid JSON: {path}: {exc}") from exc

    if not isinstance(data, dict):
        raise ValueError("project.assets.json root must be a JSON object")
    return data


def write_sbom(sbom: dict[str, Any], output: Path) -> None:
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(json.dumps(sbom, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Generate a deterministic CycloneDX 1.5 SBOM from a .NET project.assets.json file."
    )
    parser.add_argument("--assets", required=True, type=Path, help="Path to project.assets.json")
    parser.add_argument("--output", required=True, type=Path, help="Output CycloneDX JSON path")
    parser.add_argument("--name", help="Override the root application component name")
    parser.add_argument("--version", help="Override the root application component version")
    args = parser.parse_args()

    try:
        assets = load_assets(args.assets)
        sbom = build_sbom(
            assets,
            component_name=args.name,
            component_version=args.version,
        )
        write_sbom(sbom, args.output)
    except (OSError, ValueError) as exc:
        print(f"SBOM generation failed: {exc}", file=sys.stderr)
        return 1

    print(
        f"Generated CycloneDX {CYCLONEDX_SPEC_VERSION} SBOM with "
        f"{len(sbom['components'])} NuGet package component(s): {args.output}"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
