# CalcNova Calculator Editing

CalcNova's shared calculator input supports both direct text editing and keypad-driven editing without forcing every keypad action to append at the end of the expression.

## Selection-aware keypad behavior

The calculator view model tracks the active expression selection as a pair of bounded indexes. Shared keypad commands use that selection as follows:

- inserting a digit, operator, constant, or closing parenthesis replaces the active selection;
- when there is no selection, insertion occurs at the current caret position;
- a selected expression is preserved and wrapped when a function-prefix key ending in `(` is used;
- the open-parenthesis key also wraps a selected subexpression;
- Backspace removes the active selection when one exists;
- otherwise Backspace removes the character immediately before the caret;
- Backspace at caret position `0` is a no-op;
- reversed selections are normalized before editing;
- out-of-range selection indexes are clamped to the current expression length;
- expression-length limits are evaluated against the final edited expression.

After a keypad edit, the view model requests the new caret position. The shared Avalonia view applies that request back to the calculator TextBox.

## Function-key wrapping

Function buttons no longer discard a selected subexpression. The selection-aware token editor wraps the existing text and adds the matching closing parenthesis.

Examples:

- `1+25`, select `25`, press `sqrt(` -> `1+sqrt(25)`;
- `2+3*4`, select `3*4`, press `sin(` -> `2+sin(3*4)`;
- `1+2*3`, select `2*3`, press `(` -> `1+(2*3)`.

The caret is requested immediately after the generated closing parenthesis.

When there is no active selection, function insertion remains open for continued typing. For example, inserting `sin(` at a caret produces only the prefix and does not automatically add `)`.

The helper detects wrapper tokens by their trailing `(` rather than maintaining a duplicated hard-coded function-name list. New keypad function prefixes therefore inherit the same selection-preserving behavior automatically.

## Expression-length safety

`CalculatorSelectionEditor` evaluates the final result length before committing an edit.

Ordinary selection replacement removes the selected length before adding the new token. Function wrapping preserves the selected text and adds both the function/open-parenthesis prefix and a closing `)`.

If the final expression would exceed `EvaluationOptions.Default.MaximumExpressionLength`, the edit is rejected and the calculator reports `Expression limit reached.` instead of partially modifying the expression.

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

## Source separation

Selection math is isolated from Avalonia controls in:

- `src/CalcNova.App/Infrastructure/CalculatorSelectionEditor.cs`.

The helper returns the final expression plus the requested caret position. `CalculatorViewModel` owns command behavior, while `MainView` only synchronizes the real TextBox selection.

This keeps function wrapping, replacement, reversed-selection handling, clamping, and expression-limit behavior directly unit-testable without requiring a visual tree.

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

`CalculatorSelectionEditorTests` additionally covers:

- forward/reversed function wrapping;
- parenthesis wrapping;
- open function insertion at an empty selection;
- ordinary-token replacement;
- clamped selection indexes;
- final-expression length enforcement;
- wrapper-token detection.

`CalculatorFunctionSelectionViewModelTests` verifies the same wrapping behavior through the real calculator command surface and checks caret requests after a generated closing parenthesis.

The Avalonia headless shared-shell suite additionally exercises keypad replacement and verifies that the resulting caret is restored on the real calculator TextBox.

`tools/validate_calculator_selection_editing.py` and its Python regression test protect the source wiring without requiring the .NET SDK. `.github/workflows/calculator-selection-validate.yml` watches the helper, view model, view synchronization, and all focused selection-editing tests. The same validator/test pair remains part of the integrated release-source preflight.

## Validation boundary

Source contracts and test source are implemented. The active continuation environment still does not provide the required .NET SDK, so compiled/headless selection-editing tests remain `NOT RUN` locally here until a real CI or suitable local execution result is observed.

Target-specific IME, virtual-keyboard, accessibility, and unusual keyboard-layout behavior still require runtime checks on the supported platform rather than being inferred from source presence.
