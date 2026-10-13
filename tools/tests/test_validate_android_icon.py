#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_android_icon.py"
FOREGROUND = ROOT / "src" / "CalcNova.Android" / "Resources" / "drawable" / "ic_launcher_foreground.xml"

VECTOR_TEMPLATE = """<?xml version="1.0" encoding="utf-8" ?>
<vector xmlns:android="http://schemas.android.com/apk/res/android"
        android:width="108dp"
        android:height="108dp"
        android:viewportWidth="108"
        android:viewportHeight="108">
  <path
      android:fillColor="#FFFFFF"
      android:pathData="{path}" />
</vector>
"""


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_android_icon", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load Android icon validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def validate_path_data(validator, path_data: str) -> list[str]:
    with tempfile.TemporaryDirectory() as directory:
        vector = Path(directory) / "ic_launcher_foreground.xml"
        vector.write_text(VECTOR_TEMPLATE.format(path=path_data), encoding="utf-8")
        return validator.validate_vector(vector)


class AndroidIconValidatorTests(unittest.TestCase):
    def test_repository_launcher_icon_is_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_artwork_inside_the_safe_circle_is_accepted(self) -> None:
        validator = load_validator()
        # A square whose corners sit 28.3dp from the centre, inside the 33dp radius.
        self.assertEqual([], validate_path_data(validator, "M34,34 L74,34 L74,74 L34,74 Z"))

    def test_artwork_outside_the_displayed_window_is_rejected(self) -> None:
        validator = load_validator()
        failures = validate_path_data(validator, "M79,12 L83,22 L93,26 L83,30 L79,40 L75,30 L65,26 L75,22 Z")
        self.assertTrue(any("cut off on every device" in failure for failure in failures), failures)

    def test_artwork_outside_the_mask_safe_circle_is_rejected(self) -> None:
        validator = load_validator()
        # Inside the 18..90 window, but its corners reach 44.5dp from the centre.
        failures = validate_path_data(validator, "M23,23 L85,23 L85,85 L23,85 Z")
        self.assertTrue(any("circular launcher masks" in failure for failure in failures), failures)
        self.assertFalse(any("cut off on every device" in failure for failure in failures), failures)

    def test_curve_bulge_outside_the_safe_circle_is_rejected(self) -> None:
        validator = load_validator()
        # Both endpoints sit 24dp from the centre and every sampled point stays inside the
        # 18..90 window; only the curve's apex, at (54, 19.5), leaves the 33dp circle. A
        # validator that trusted endpoints or control points would miss this.
        failures = validate_path_data(validator, "M30,54 Q54,-15 78,54 Z")
        self.assertTrue(any("circular launcher masks" in failure for failure in failures), failures)
        self.assertFalse(any("cut off on every device" in failure for failure in failures), failures)

    def test_non_standard_canvas_is_rejected(self) -> None:
        validator = load_validator()
        with tempfile.TemporaryDirectory() as directory:
            vector = Path(directory) / "ic_launcher_foreground.xml"
            vector.write_text(
                VECTOR_TEMPLATE.format(path="M34,34 L74,34 L74,74 L34,74 Z").replace("108dp", "48dp"),
                encoding="utf-8",
            )
            failures = validator.validate_vector(vector)
        self.assertTrue(any("must declare width=108dp" in failure for failure in failures), failures)

    def test_every_adaptive_icon_layer_references_the_validated_foreground(self) -> None:
        resources = ROOT / "src" / "CalcNova.Android" / "Resources"
        layers = sorted(resources.glob("mipmap-anydpi*/ic_launcher*.xml"))
        self.assertNotEqual([], layers)
        for layer in layers:
            with self.subTest(layer=layer.name):
                self.assertIn("@drawable/ic_launcher_foreground", layer.read_text(encoding="utf-8"))


if __name__ == "__main__":
    unittest.main()
