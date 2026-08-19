#!/usr/bin/env python3

from __future__ import annotations

import importlib.util
from pathlib import Path
import unittest

ROOT = Path(__file__).resolve().parents[2]
VALIDATOR = ROOT / "tools" / "validate_graph_keyboard.py"


def load_validator():
    spec = importlib.util.spec_from_file_location("validate_graph_keyboard", VALIDATOR)
    if spec is None or spec.loader is None:
        raise RuntimeError("Unable to load graph keyboard validator")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


class GraphKeyboardValidatorTests(unittest.TestCase):
    def test_repository_graph_keyboard_contracts_are_valid(self) -> None:
        validator = load_validator()
        self.assertEqual([], validator.validate(ROOT))

    def test_mapping_covers_pan_zoom_reset_and_fit(self) -> None:
        validator = load_validator()
        self.assertEqual(
            {
                "Left": "PanLeft",
                "Right": "PanRight",
                "Up": "PanUp",
                "Down": "PanDown",
                "Add": "ZoomIn",
                "Subtract": "ZoomOut",
                "Home": "ResetViewport",
                "F": "FitToData",
            },
            validator.EXPECTED_ACTIONS,
        )


if __name__ == "__main__":
    unittest.main()
