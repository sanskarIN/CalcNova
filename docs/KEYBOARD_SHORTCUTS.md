# CalcNova Keyboard Shortcuts

CalcNova is intended to support efficient keyboard-first use on desktop and browser targets. This file distinguishes currently implemented shortcuts from planned mappings.

## Current desktop key handling

| Key | Current action |
|---|---|
| Enter / Return | Evaluate current expression |
| Escape | Clear expression/result state |
| Backspace | Remove the final expression character |

The expression text box also receives ordinary text entry according to Avalonia/platform text-input behavior.

## Planned calculator mappings

The following mappings are intended but must be tested before being marked implemented:

| Key | Intended action |
|---|---|
| `0`–`9` | Insert digit |
| Numpad `0`–`9` | Insert digit |
| `.` / locale-aware decimal input | Insert decimal separator safely |
| `+` | Addition |
| `-` | Subtraction |
| `*` | Multiplication |
| `/` | Division |
| `%` | Percentage/modulo according to active context |
| `^` | Power |
| `(` / `)` | Parentheses |
| Delete | Clear selected/input content where appropriate |
| Ctrl/Cmd+C | Copy selected/result content |
| Ctrl/Cmd+V | Paste sanitized expression text |
| Ctrl/Cmd+K | Command palette if implemented |
| Ctrl/Cmd+L | Focus expression input if adopted without platform conflict |

## Mode shortcuts

Mode-switch shortcuts are not finalized. They should avoid conflicts with OS/browser conventions and remain discoverable in an in-app shortcut dialog.

## Focus and accessibility requirements

Keyboard support is not complete merely because key events exist. Each interactive control must also support:

- logical Tab/Shift+Tab order;
- visible focus indication;
- activation using expected platform keys;
- no keyboard trap;
- dialogs that return focus predictably;
- screen-reader labels that describe function, not only visual glyphs.

## Browser considerations

Browser builds must avoid hijacking important browser/system shortcuts. Any shortcut that conflicts with navigation, developer tools, tabs, or accessibility software should be changed or made configurable.

## Custom shortcuts

Configurable shortcuts are a later power-user feature. If implemented, the settings UI must detect duplicates/conflicts and provide a reset-to-default option.
