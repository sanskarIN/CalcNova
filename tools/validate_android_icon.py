#!/usr/bin/env python3
"""Validate the Android adaptive launcher icon against Android's safe areas.

Android composites a 108x108dp adaptive-icon foreground behind a launcher-chosen
mask. Two boundaries follow from that:

* only the centre 72dp (18..90 on both axes) is ever displayed -- the outer ring
  exists so the launcher can animate the layers, and artwork there is never seen;
* of that, only the centre 66dp circle is guaranteed visible, because the mask
  shape is the launcher's choice (circle, squircle, rounded square, teardrop).

Artwork outside the window is a defect on every device. Artwork between the
circle and the window renders on some launchers and is cropped on others, which
is a defect for anything load-bearing. This validator holds the icon to the
circle, so the result is correct whatever mask a device applies.
"""

from __future__ import annotations

import argparse
import math
import re
import sys
from pathlib import Path
from xml.etree import ElementTree

ANDROID_NS = "{http://schemas.android.com/apk/res/android}"

CANVAS = 108.0
WINDOW_MIN = 18.0
WINDOW_MAX = 90.0
CENTRE = (54.0, 54.0)
SAFE_RADIUS = 33.0
TOLERANCE = 1e-6

CURVE_SAMPLES = 24

_TOKEN = re.compile(r"[MmLlHhVvQqCcZz]|-?\d*\.?\d+(?:[eE][-+]?\d+)?")


def flatten(path_data: str) -> list[tuple[float, float]]:
    """Sample a path into points, subdividing curves rather than only taking ends.

    A quadratic's bulge can leave the safe area while both endpoints sit inside
    it, so the control points alone would not catch a rounded corner that pushes
    past the mask.
    """
    tokens = _TOKEN.findall(path_data)
    points: list[tuple[float, float]] = []
    index = 0
    current = (0.0, 0.0)
    subpath_start = (0.0, 0.0)
    command = ""

    def take() -> float:
        nonlocal index
        value = float(tokens[index])
        index += 1
        return value

    while index < len(tokens):
        if re.fullmatch(r"[A-Za-z]", tokens[index]):
            command = tokens[index]
            index += 1
            if command in "Zz":
                current = subpath_start
                continue

        if not command:
            raise ValueError(f"path data starts without a command: {path_data!r}")

        relative = command.islower()
        kind = command.upper()
        origin_x, origin_y = current

        if kind == "M":
            x, y = take(), take()
            current = (origin_x + x, origin_y + y) if relative else (x, y)
            subpath_start = current
            points.append(current)
            command = "l" if relative else "L"
        elif kind == "L":
            x, y = take(), take()
            current = (origin_x + x, origin_y + y) if relative else (x, y)
            points.append(current)
        elif kind == "H":
            x = take()
            current = (origin_x + x if relative else x, origin_y)
            points.append(current)
        elif kind == "V":
            y = take()
            current = (origin_x, origin_y + y if relative else y)
            points.append(current)
        elif kind == "Q":
            cx, cy, x, y = take(), take(), take(), take()
            if relative:
                cx, cy, x, y = origin_x + cx, origin_y + cy, origin_x + x, origin_y + y
            for step in range(1, CURVE_SAMPLES + 1):
                t = step / CURVE_SAMPLES
                u = 1.0 - t
                points.append((
                    u * u * origin_x + 2 * u * t * cx + t * t * x,
                    u * u * origin_y + 2 * u * t * cy + t * t * y,
                ))
            current = (x, y)
        elif kind == "C":
            c1x, c1y, c2x, c2y, x, y = (take() for _ in range(6))
            if relative:
                c1x, c1y = origin_x + c1x, origin_y + c1y
                c2x, c2y = origin_x + c2x, origin_y + c2y
                x, y = origin_x + x, origin_y + y
            for step in range(1, CURVE_SAMPLES + 1):
                t = step / CURVE_SAMPLES
                u = 1.0 - t
                points.append((
                    u ** 3 * origin_x + 3 * u * u * t * c1x + 3 * u * t * t * c2x + t ** 3 * x,
                    u ** 3 * origin_y + 3 * u * u * t * c1y + 3 * u * t * t * c2y + t ** 3 * y,
                ))
            current = (x, y)
        else:
            raise ValueError(f"unsupported path command {command!r} in {path_data!r}")

    return points


def validate_vector(path: Path) -> list[str]:
    failures: list[str] = []
    root = ElementTree.parse(path).getroot()

    for attribute, expected in (("width", "108dp"), ("height", "108dp"),
                                ("viewportWidth", "108"), ("viewportHeight", "108")):
        actual = root.get(f"{ANDROID_NS}{attribute}")
        if actual != expected:
            failures.append(
                f"{path.name}: adaptive-icon foreground must declare {attribute}={expected}, found {actual}"
            )

    elements = root.findall("path")
    if not elements:
        failures.append(f"{path.name}: contains no <path> artwork")
        return failures

    for position, element in enumerate(elements):
        data = element.get(f"{ANDROID_NS}pathData")
        if not data:
            failures.append(f"{path.name}: path[{position}] has no pathData")
            continue

        fill = element.get(f"{ANDROID_NS}fillColor", "unspecified")
        worst_window: tuple[float, float] | None = None
        worst_radius = 0.0
        worst_radius_point: tuple[float, float] | None = None

        for x, y in flatten(data):
            if not (WINDOW_MIN - TOLERANCE <= x <= WINDOW_MAX + TOLERANCE
                    and WINDOW_MIN - TOLERANCE <= y <= WINDOW_MAX + TOLERANCE):
                worst_window = (x, y)
            distance = math.dist((x, y), CENTRE)
            if distance > worst_radius:
                worst_radius = distance
                worst_radius_point = (x, y)

        if worst_window is not None:
            failures.append(
                f"{path.name}: path[{position}] ({fill}) reaches "
                f"({worst_window[0]:g}, {worst_window[1]:g}), outside the "
                f"{WINDOW_MIN:g}..{WINDOW_MAX:g} window Android always displays, "
                "so that artwork is cut off on every device"
            )
        elif worst_radius > SAFE_RADIUS + TOLERANCE and worst_radius_point is not None:
            failures.append(
                f"{path.name}: path[{position}] ({fill}) reaches "
                f"({worst_radius_point[0]:g}, {worst_radius_point[1]:g}), "
                f"{worst_radius:.1f}dp from the centre, outside the {SAFE_RADIUS:g}dp "
                "safe radius, so it is cropped by circular launcher masks"
            )

    return failures


def validate(root: Path) -> list[str]:
    foreground = root / "src" / "CalcNova.Android" / "Resources" / "drawable" / "ic_launcher_foreground.xml"
    if not foreground.is_file():
        return [f"Missing adaptive-icon foreground: {foreground}"]
    return validate_vector(foreground)


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova's Android launcher icon.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Android launcher icon validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print(
        f"Validated the adaptive-icon foreground stays inside Android's {WINDOW_MAX - WINDOW_MIN:g}dp "
        f"display window and {SAFE_RADIUS * 2:g}dp mask-safe circle."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
