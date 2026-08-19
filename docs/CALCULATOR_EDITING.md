# CalcNova Calculator Editing

CalcNova's shared calculator input supports both direct text editing and keypad-driven editing without forcing every keypad action to append at the end of the expression.

## Selection-aware keypad behavior

The calculator view model tracks the active expression selection as a pair of bounded indexes. Shared keypad commands use that selection as follows:

- inserting a digit, operator, constant, function, or parenthesis replaces the active selection;
- when there is no selection, insertion occurs at the current caret position;
- Backspace removes the active selection when one exists;
- otherwise Backspace removes the character immediately before the caret;
- Backspace at caret position `0` is a no-op;
- reversed selections are normalized before editing;
- out-of-range selection indexes are clamped to the current expression length;
- expression-length limits are evaluated after accounting for the text being replaced, so replacing a selection does not incorrectly fail because of the old expression length.

After a keypad edit, the view model requests the new caret position. The shared Avalonia view applies that request back to the calculator TextBox.

## Shared TextBox synchronization

`MainView` connects the calculator expression TextBox to the selection-aware editing contract without taking over ordinary text editing:

- keyboard selection/caret changes are synchronized after `KeyUp`;
- pointer selection/caret changes are synchronized after `PointerReleased`;
- the view model raises `SelectionRequested` after programmatic keypad edits;
- the view applies the requested `SelectionStart` / `SelectionEnd` values;
- subscriptions are removed when the shared view detaches from the visual tree.

Direct TextBox editing remains owned by Avalonia/platform text input behavior. CalcNova does not intercept ordinary character editing inside the TextBox merely to implement keypad selection semantics.

## Programmatic expression changes

Operations that replace the whole expression place the caret at a predictable location:

- Clear -> position `0`;
- imported/sanitized expression -> end of expression;
- result reuse -> end of expression;
- percentage transformation -> end of transformed expression;
- memory recall -> end of recalled value.

## Regression coverage

`CalculatorSelectionEditingTests` covers:

- forward selection replacement;
- reversed selection replacement;
- insertion at a middle caret position;
- selection deletion with Backspace;
- character-before-caret Backspace behavior;
- Backspace at the start of input;
- clamping invalid selection indexes;
- caret reset after Clear.

The Avalonia headless shared-shell suite additionally exercises keypad replacement and verifies that the resulting caret is restored on the real calculator TextBox.

`tools/validate_calculator_selection_editing.py` and its Python regression test protect the source wiring without requiring the .NET SDK. `.github/workflows/calculator-selection-validate.yml` provides a focused source-contract signal, while the App test project carries the compiled/headless behavior once `.NET 10` tests actually run.

## Validation boundary

Source contracts and test source are implemented. The active continuation environment still does not provide the required .NET SDK, so compiled/headless selection-editing tests remain `NOT RUN` locally here until a real CI or suitable local execution result is observed.

Target-specific IME, virtual-keyboard, accessibility, and unusual keyboard-layout behavior still require runtime checks on the supported platform rather than being inferred from source presence.
