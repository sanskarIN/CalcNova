# CalcNova Keyboard Shortcuts

CalcNova supports keyboard-first use on desktop and Browser targets through the shared Avalonia application layer. Text boxes still receive normal platform text editing; calculator-level shortcuts are routed only when CalcNova's calculator surface owns the key event.

The shared application header now includes a **Shortcuts** button that opens an in-app reference flyout for the primary calculator keyboard controls. Because it lives in `CalcNova.App`, the same reference is available to every platform head that hosts the shared view.

## Calculator shortcuts

| Key | Action |
|---|---|
| Enter / Return | Evaluate the current expression; repeated Enter uses repeated-equals semantics when available |
| Escape | Clear expression/result state |
| Backspace | Remove the final expression character when a text editor does not already own the key |
| F9 | Toggle the sign of the evaluated current value |
| `0`–`9` | Insert digits outside an active text editor |
| Numpad `0`–`9` | Insert digits |
| `.` | Insert decimal point |
| `+` | Addition |
| `-` | Subtraction |
| `*` | Multiplication |
| `/` | Division |
| `%` | Insert the explicit modulo operator when typed; the on-screen `%` button uses calculator-style contextual percentage semantics |
| `^` | Power |
| `(` / `)` | Parentheses |
| Ctrl+C / Cmd+C | Copy the current result, or the expression when the result is an error, when a text editor does not own the shortcut |
| Ctrl+V / Cmd+V | Paste a length-bounded expression from the system clipboard when a text editor does not own the shortcut |

The calculator's touch UI also includes explicit **Copy result**, **Copy expression**, and **Paste expression** actions for platforms where keyboard shortcuts are not convenient.

## In-app shortcut reference

Use the **Shortcuts** button in the CalcNova header to open the compact keyboard reference. It lists the primary evaluate, clear, backspace, sign-toggle, copy, paste, and numeric/operator controls without leaving the calculator.

The reference is intentionally informational: it does not intercept keystrokes or alter focus while closed. Normal text-field keyboard behavior remains unchanged.

## Numpad behavior

CalcNova handles physical numpad digits plus Add, Subtract, Multiply, Divide, and Decimal through the shared key router. This behavior is platform/framework dependent and remains subject to CI/manual validation on supported desktop environments.

## Text editing behavior

When a `TextBox` owns keyboard focus, CalcNova intentionally lets the platform text editor handle ordinary typing, selection, copy, paste, cursor movement, and deletion. This avoids duplicate input and preserves expected accessibility/IME behavior.

Pasted calculator expressions are capped at the same maximum expression length as direct calculator input. Pasted text is treated strictly as calculator expression text and is never executed as arbitrary code.

## Mode shortcuts

Dedicated mode-switch shortcuts are not yet assigned. They should avoid conflicts with operating-system/browser conventions and remain discoverable if introduced later.

## Focus and accessibility requirements

Keyboard support also depends on:

- logical Tab/Shift+Tab order;
- visible focus indication;
- expected button activation keys;
- no keyboard traps;
- predictable focus after dialogs/confirmation flows;
- screen-reader labels that describe the function rather than only a visual glyph.

The shared UI uses standard Avalonia controls wherever possible so platform accessibility and keyboard semantics are preserved.

## Browser considerations

Browser builds deliberately avoid overriding important browser/system shortcuts beyond normal copy/paste handling when CalcNova owns the surface. Browser navigation, developer-tool, tab-management, and accessibility shortcuts should remain under browser control.

## Later power-user work

Potential future additions include a searchable command palette, configurable shortcuts, and direct mode switching. Configurable shortcuts must detect duplicates/conflicts and offer reset-to-default behavior before they are considered complete.
