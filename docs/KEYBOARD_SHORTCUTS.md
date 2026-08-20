# CalcNova 2.8.03 Keyboard Shortcuts

CalcNova supports keyboard-first workflows on keyboard-capable targets while deliberately avoiding broad shortcut interception that would conflict with text editing, browsers, operating systems, or assistive technology.

This document lists implemented shared mappings and separates optional future shortcut ideas from the completed 2.8.03 baseline.

## Shared shell navigation

When onboarding is not visible and the shortcut carries exactly the Control modifier:

| Key | Action |
|---|---|
| `Ctrl+PageDown` | Select next CalcNova mode, wrapping from last to first |
| `Ctrl+PageUp` | Select previous CalcNova mode, wrapping from first to last |
| `Ctrl+Home` | Select first CalcNova mode |
| `Ctrl+End` | Select final CalcNova mode |

Mode navigation is implemented through `MainViewModel` selection behavior rather than direct hard-coded tab mutation.

Out-of-range selection is normalized and previous/next navigation preserves wraparound semantics.

Using exactly Control prevents unrelated Control+Shift/Alt chords from being captured accidentally.

## Calculator action keys

When Calculator mode is active and focus is not inside an ordinary text editor, shared key handling includes:

| Key | Action |
|---|---|
| Enter / Return | Evaluate current expression |
| Escape | Clear calculator expression/result state |
| Backspace | Apply calculator selection/caret-aware Backspace behavior |

Inside an active `TextBox`, ordinary Avalonia/platform text editing remains authoritative so selection, caret, IME, and native editing behavior are not overridden by shell token injection.

## Unmodified calculator hardware mappings

Outside an active `TextBox`, Calculator mode recognizes supported unmodified keys including:

| Key | Token/action |
|---|---|
| Top-row `0`–`9` | matching digit |
| Numpad `0`–`9` | matching digit |
| Numpad `+` | `+` |
| Numpad `-` | `-` |
| Numpad `*` | `*` |
| Numpad `/` | `/` |
| Numpad decimal | canonical decimal token |
| Keyboard `-` | `-` where Avalonia reports the supported key |
| Keyboard `/` | `/` where Avalonia reports the supported key |
| Keyboard `.` | decimal punctuation/token path where supported |
| Keyboard `,` | comma token for function argument separation where supported |

These mappings use canonical parser tokens rather than localized display symbols.

## Shift-only top-row operator mappings

When the event carries exactly the Shift modifier, the implemented Calculator mapping recognizes the common top-row operator positions:

| Key | Token |
|---|---|
| Shift+`=` | `+` |
| Shift+`8` | `*` |
| Shift+`9` | `(` |
| Shift+`0` | `)` |
| Shift+`6` | `^` |
| Shift+`5` | `%` |

Control, Alt, and combined modifier chords are deliberately excluded from this mapping.

Keyboard-layout differences still require runtime verification, especially in Browser and non-US layouts. The source mapping being implemented does not mean every physical layout produces identical key codes.

## Calculator glyph normalization

For non-editor text-input paths, CalcNova also normalizes supported calculator-style Unicode glyphs that are not reliably represented by physical key enums:

- multiplication glyphs such as `×` / supported middle-dot form -> `*`;
- `÷` -> `/`;
- Unicode minus/dash forms -> `-`.

The text-symbol path intentionally avoids duplicating ordinary ASCII digit/operator insertion already handled through key events.

See [CALCULATOR_KEYBOARD_INPUT.md](CALCULATOR_KEYBOARD_INPUT.md).

## Selection/caret behavior

Hardware/keypad editing shares Calculator selection semantics rather than assuming all edits occur at the end of the string.

Supported behavior includes:

- replacement of selected text;
- insertion at caret when there is no selection;
- Backspace deleting selection or the character before the caret;
- clamped/reversed selection handling;
- selection-preserving function/parenthesis wrapping from keypad controls;
- predictable caret restoration after programmatic edits.

See [CALCULATOR_EDITING.md](CALCULATOR_EDITING.md).

## Graph keyboard controls

When the graph control has focus and no conflicting modifier is held:

| Key | Action |
|---|---|
| Left/Right Arrow | Pan horizontally |
| Up/Down Arrow | Pan vertically |
| Numpad `+` | Zoom in |
| Numpad `-` | Zoom out |
| Home | Reset default viewport |
| `F` | Fit viewport to finite sampled data |

Graph key handling deliberately avoids modified variants so browser/OS/application/accessibility shortcuts retain priority.

See [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md).

## Onboarding containment

While first-run onboarding is visible, shared Calculator/mode keyboard shortcuts are suppressed so a command cannot activate hidden background content.

After onboarding dismissal, focus is restored through the shared focus-handoff behavior.

See [ONBOARDING.md](ONBOARDING.md).

## Clipboard shortcuts versus clipboard actions

CalcNova has explicit user-triggered paste/copy commands and buttons through its shared clipboard abstraction.

Ordinary platform `Ctrl/Cmd+C` and `Ctrl/Cmd+V` behavior inside editable controls remains subject to native/Avalonia text editing. CalcNova should not globally intercept platform copy/paste chords unless a future mapping is carefully scoped and tested for browser/OS/accessibility conflicts.

Explicit calculator paste still sanitizes imported expression text and does not auto-evaluate it.

See [INPUT_SAFETY.md](INPUT_SAFETY.md).

## Focus and accessibility requirements

Keyboard support is more than key-event mapping.

The shared design contract also requires:

- logical Tab/Shift+Tab traversal;
- visible focus indication;
- expected control activation behavior;
- no keyboard traps;
- overlay/dialog focus containment/restoration;
- semantic accessible names for symbol-heavy controls;
- focus bring-into-view under scroll/adaptive layouts.

Source/headless contracts cover representative behavior, while actual keyboard layout, browser conflicts, focus rendering, and assistive-technology behavior remain target-runtime evidence.

## Browser considerations

Browser builds must not unnecessarily hijack important browser/system shortcuts.

CalcNova's modifier policies intentionally limit interception. Any future shortcut that conflicts with browser navigation, tabs, developer tools, accessibility software, or OS conventions should be redesigned or made appropriately configurable rather than captured globally.

## Optional post-2.8.03 shortcut ideas

Possible future power-user additions include:

- direct mode-selection shortcuts;
- a command palette;
- explicit focus-expression shortcut;
- configurable user shortcuts with conflict detection;
- additional locale-aware physical-key mappings after target validation.

These are optional enhancements, not missing 2.8.03 requirements.

## Validation

Relevant source validators/regressions cover:

- shell navigation policies;
- calculator physical/shift-only mappings;
- glyph normalization;
- calculator selection editing;
- graph keyboard behavior;
- onboarding shortcut suppression.

The integrated source gate is:

```bash
python tools/release_preflight.py
```

Target keyboard/layout/browser behavior is recorded independently using:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

## 2.8.03 classification

- Calculator Enter/Escape/Backspace: **COMPLETE**;
- top-row/numpad digit input: **COMPLETE**;
- numpad arithmetic input: **COMPLETE**;
- supported unmodified punctuation input: **COMPLETE**;
- Shift-only `+ * ( ) ^ %` mapping: **COMPLETE**;
- shared mode navigation: **COMPLETE**;
- graph keyboard pan/zoom/reset/fit: **COMPLETE**;
- onboarding shortcut suppression: **COMPLETE**;
- configurable/direct-mode extra shortcuts: **OPTIONAL POST-2.8.03**.
