# CalcNova Calculator Keyboard Input

CalcNova separates calculator keyboard handling from ordinary text editing and global shell navigation.

## Unmodified physical-key mappings

Outside a `TextBox`, Calculator mode recognizes:

- top-row and numpad digits `0`–`9`;
- numpad `+`, `-`, `*`, `/`, decimal;
- keyboard `-`, `/`, `.`, and `,` punctuation keys where Avalonia reports the corresponding OEM key.

The comma token remains available for expression-function argument separation.

## Shift-only operator mappings

When the event carries exactly the Shift modifier, Calculator mode recognizes the common top-row operator positions:

- Shift+`=` → `+`;
- Shift+`8` → `*`;
- Shift+`9` → `(`;
- Shift+`0` → `)`;
- Shift+`6` → `^`;
- Shift+`5` → `%`.

Control, Alt, and combined modifier chords are deliberately not captured by this mapping. This protects browser, OS, accessibility-tool, and application shortcut behavior.

## Calculator glyph text normalization

For non-editor text input, CalcNova also normalizes calculator-style Unicode glyphs that physical-key handling does not cover:

- `×` → `*`;
- `÷` → `/`;
- Unicode minus/en dash/em dash → `-`;
- middle-dot multiplication glyphs → `*`.

Ordinary ASCII operators and digits are not handled by this text-symbol layer, preventing duplicate insertion with the physical-key path.

## TextBox safety

When a `TextBox` owns the input event, the shared calculator-level keyboard handlers do not inject tokens. Native Avalonia text editing, selection, IME/input-method behavior, and the calculator's selection-aware expression synchronization remain authoritative.

The calculator's keypad buttons separately respect the tracked selection/caret: insertion replaces the selected range, and Backspace removes the selected range or the previous character.

## Shared shell shortcuts

Exactly-Control shell navigation remains independent of calculator tokens:

- Ctrl+PageUp: previous mode;
- Ctrl+PageDown: next mode;
- Ctrl+Home: first mode;
- Ctrl+End: last mode.

Those shortcuts are suppressed while onboarding is visible.

## Validation

Run:

```bash
python tools/validate_keyboard_contracts.py .
python -m unittest tools.tests.test_validate_keyboard_contracts
```

Headless UI coverage exercises Shift-only operator insertion. Glyph normalization has deterministic unit/source-contract coverage because a stable public headless Unicode text-input helper is not currently assumed by the project.

Actual keyboard-layout behavior still requires target testing, especially Browser and non-US hardware layouts.
