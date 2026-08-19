# CalcNova Keyboard Shortcuts

CalcNova is intended to support efficient keyboard-first use on desktop and browser targets. This file distinguishes currently implemented shortcuts from planned mappings.

## Current shared key handling

| Key | Current action |
|---|---|
| Enter / Return | Evaluate current expression while Calculator mode is active |
| Escape | Clear expression/result state while Calculator mode is active |
| Backspace | Remove the final expression character when Calculator mode is active and focus is not inside a text box |
| Ctrl+PageDown | Select the next CalcNova mode, wrapping from About to Calc |
| Ctrl+PageUp | Select the previous CalcNova mode, wrapping from Calc to About |

The expression text box also receives ordinary text entry according to Avalonia/platform text-input behavior. Backspace is deliberately left to an active text box instead of being intercepted by the shell so normal caret/selection editing can occur.

Mode cycling is implemented through `MainViewModel` rather than hard-coded tab mutations in the view. A repository validator checks that `MainViewModel.ModeCount` matches the number of shared XAML tabs and that the two keyboard shortcuts remain wired.

## Current calculator hardware mappings

When Calculator mode is active, focus is outside a text box, and no modifier is held, the shared shell maps these keys directly to canonical parser tokens:

| Key | Action/token |
|---|---|
| Top-row `0`–`9` | Insert matching digit |
| Numpad `0`–`9` | Insert matching digit |
| Numpad `+` | `+` |
| Numpad `-` | `-` |
| Numpad `*` | `*` |
| Numpad `/` | `/` |
| Numpad decimal | `.` canonical decimal token |

The mapping intentionally does not interpret shifted OEM punctuation keys yet. Those keys vary by keyboard layout and locale, so they should be handled only after locale/input-boundary behavior is defined and tested. The mapping is unit-tested and protected by a source-level CI contract validator.

## Planned calculator mappings

The following mappings are intended but must be tested before being marked implemented:

| Key | Intended action |
|---|---|
| Locale-aware decimal punctuation outside the numpad | Insert decimal separator safely |
| Top-row `+`, `-`, `*`, `/` punctuation variants | Arithmetic operators with keyboard-layout awareness |
| `%` | Percentage/modulo according to active context |
| `^` | Power |
| `(` / `)` | Parentheses |
| Delete | Clear selected/input content where appropriate |
| Ctrl/Cmd+C | Copy selected/result content |
| Ctrl/Cmd+V | Paste sanitized expression text |
| Ctrl/Cmd+K | Command palette if implemented |
| Ctrl/Cmd+L | Focus expression input if adopted without platform conflict |

## Mode shortcuts

`Ctrl+PageUp` and `Ctrl+PageDown` are the current shared mode-navigation shortcuts. Additional direct mode shortcuts are not finalized. Any future mappings should avoid conflicts with OS/browser conventions and remain discoverable in an in-app shortcut reference.

## Focus and accessibility requirements

Keyboard support is not complete merely because key events exist. Each interactive control must also support:

- logical Tab/Shift+Tab order;
- visible focus indication;
- activation using expected platform keys;
- no keyboard trap;
- dialogs that return focus predictably;
- screen-reader labels that describe function, not only visual glyphs.

## Browser considerations

Browser builds must avoid hijacking important browser/system shortcuts. Any shortcut that conflicts with navigation, developer tools, tabs, or accessibility software should be changed or made configurable after target-browser validation.

## Custom shortcuts

Configurable shortcuts are a later power-user feature. If implemented, the settings UI must detect duplicates/conflicts and provide a reset-to-default option.
