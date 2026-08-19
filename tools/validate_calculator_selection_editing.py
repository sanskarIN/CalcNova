#!/usr/bin/env python3
"""Validate CalcNova selection-aware calculator editing contracts without .NET."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


def validate(root: Path) -> list[str]:
    view_model_path = root / "src" / "CalcNova.App" / "ViewModels" / "CalculatorViewModel.cs"
    view_path = root / "src" / "CalcNova.App" / "Views" / "MainView.axaml.cs"
    tests_path = root / "tests" / "CalcNova.App.Tests" / "CalculatorSelectionEditingTests.cs"

    failures: list[str] = []
    for path in (view_model_path, view_path, tests_path):
        if not path.is_file():
            failures.append(f"Missing calculator selection-editing source: {path}")

    if failures:
        return failures

    view_model = view_model_path.read_text(encoding="utf-8")
    view = view_path.read_text(encoding="utf-8")
    tests = tests_path.read_text(encoding="utf-8")

    for marker in (
        "public event Action<int, int>? SelectionRequested",
        "public void UpdateSelection(int selectionStart, int selectionEnd)",
        "var (start, end) = NormalizedSelection()",
        "Expression = Expression[..start] + token + Expression[end..]",
        "Expression = Expression.Remove(start, end - start)",
        "Expression = Expression.Remove(start - 1, 1)",
        "private void RequestSelection(int caretIndex)",
    ):
        if marker not in view_model:
            failures.append(f"CalculatorViewModel is missing selection-editing marker: {marker}")

    for marker in (
        "AttachCalculatorExpressionEditor(viewModel.Calculator)",
        "textBox.KeyUp += HandleCalculatorExpressionKeyUp",
        "textBox.PointerReleased += HandleCalculatorExpressionPointerReleased",
        "calculator.SelectionRequested += HandleCalculatorSelectionRequested",
        "_calculatorEditorViewModel.UpdateSelection(",
        "_calculatorExpressionTextBox.SelectionStart",
        "_calculatorExpressionTextBox.SelectionEnd",
    ):
        if marker not in view:
            failures.append(f"MainView is missing calculator selection synchronization marker: {marker}")

    expected_tests = (
        "AppendCommand_ReplacesForwardSelectionAndRequestsCaretAfterToken",
        "AppendCommand_ReplacesReversedSelection",
        "AppendCommand_AtCaret_InsertsInsteadOfAlwaysAppending",
        "Backspace_WithSelection_RemovesSelectedText",
        "Backspace_AtCaret_RemovesCharacterBeforeCaret",
        "Backspace_AtStart_DoesNothing",
        "UpdateSelection_ClampsOutOfRangeIndexes",
        "Clear_RequestsCaretAtStart",
    )
    for marker in expected_tests:
        if marker not in tests:
            failures.append(f"Calculator selection-editing tests are missing scenario: {marker}")

    return failures


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate CalcNova selection-aware calculator editing contracts.")
    parser.add_argument("root", nargs="?", default=".", help="Repository root")
    args = parser.parse_args()

    failures = validate(Path(args.root).resolve())
    if failures:
        print("Calculator selection-editing validation failed:", file=sys.stderr)
        for failure in failures:
            print(f"- {failure}", file=sys.stderr)
        return 1

    print("Validated selection-aware calculator keypad editing and TextBox synchronization contracts.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
