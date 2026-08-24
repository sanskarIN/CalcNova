#!/usr/bin/env python3
"""Shared CalcNova release-identity helpers used by SDK-independent tooling."""

from __future__ import annotations

from dataclasses import dataclass
import re
import xml.etree.ElementTree as ET
from pathlib import Path


_SEMVER = re.compile(r"^(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)\.(0|[1-9][0-9]*)$")
_DISPLAY_VERSION = re.compile(r"^([0-9]+)\.([0-9]+)\.([0-9]+)$")


@dataclass(frozen=True)
class ReleaseIdentity:
    display_version: str
    semver_version: str
    mobile_build_code: str

    @property
    def release_tag(self) -> str:
        return f"v{self.semver_version}"

    @property
    def assembly_version(self) -> str:
        return f"{self.semver_version}.0"


def _required_property(properties: dict[str, str], name: str) -> str:
    value = properties.get(name, "").strip()
    if not value:
        raise ValueError(f"Directory.Build.props is missing a non-empty <{name}> property.")
    return value


def mobile_build_code_for(display_version: str) -> str:
    """Return CalcNova's MAJOR*10000 + MINOR*100 + PATCH mobile build code."""
    match = _DISPLAY_VERSION.fullmatch(display_version.strip())
    if match is None:
        raise ValueError(f"Invalid CalcNova display version: {display_version!r}")

    major, minor, patch = (int(part) for part in match.groups())
    if minor > 99 or patch > 99:
        raise ValueError("CalcNova mobile build codes require minor and patch values in the 0..99 range.")

    return str((major * 10000) + (minor * 100) + patch)


def normalize_display_version(display_version: str) -> str:
    match = _DISPLAY_VERSION.fullmatch(display_version.strip())
    if match is None:
        raise ValueError(f"Invalid CalcNova display version: {display_version!r}")
    return ".".join(str(int(part)) for part in match.groups())


def load_release_identity(root: Path) -> ReleaseIdentity:
    path = root / "Directory.Build.props"
    try:
        document = ET.parse(path)
    except (OSError, ET.ParseError) as exception:
        raise ValueError(f"Unable to parse release identity from {path}: {exception}") from exception

    properties: dict[str, str] = {}
    for property_group in document.getroot().findall("PropertyGroup"):
        for child in property_group:
            if child.text is not None:
                properties[child.tag] = child.text.strip()

    display_version = _required_property(properties, "ProductDisplayVersion")
    semver_version = _required_property(properties, "Version")

    if _SEMVER.fullmatch(semver_version) is None:
        raise ValueError(f"CalcNova package version must be stable SemVer MAJOR.MINOR.PATCH: {semver_version!r}")

    normalized_display = normalize_display_version(display_version)
    if normalized_display != semver_version:
        raise ValueError(
            "ProductDisplayVersion and Version disagree after normalizing display-version leading zeroes: "
            f"{display_version!r} -> {normalized_display!r}, Version={semver_version!r}"
        )

    expected = {
        "VersionPrefix": semver_version,
        "PackageVersion": semver_version,
        "AssemblyVersion": f"{semver_version}.0",
        "FileVersion": f"{semver_version}.0",
        "InformationalVersion": display_version,
    }
    for name, expected_value in expected.items():
        actual = _required_property(properties, name)
        if actual != expected_value:
            raise ValueError(
                f"Directory.Build.props <{name}> must be {expected_value!r}; found {actual!r}."
            )

    return ReleaseIdentity(
        display_version=display_version,
        semver_version=semver_version,
        mobile_build_code=mobile_build_code_for(display_version),
    )
