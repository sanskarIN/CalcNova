# CalcNova Focus Visibility Contract

Keyboard focus must remain visually discoverable in every shared CalcNova mode. Source-level focus styling is a release guard, not a substitute for runtime keyboard and assistive-technology testing.

## Implemented baseline

The shared Avalonia application styles now apply explicit focused-state border emphasis to:

- `Button`;
- `TextBox`;
- `ComboBox`;
- `CheckBox`;
- `TabItem`;
- `ListBoxItem`.

Normal focus uses a 3-DIP border emphasis. When CalcNova's high-contrast preference is active, the same focused controls use a 4-DIP border emphasis.

This supplements Fluent theme behavior so CalcNova does not depend entirely on theme defaults for focus discoverability.

## Regression validation

`tools/validate_focus_visibility.py` verifies that all six shared control categories keep both normal and high-contrast focus selectors and their expected border emphasis.

`tools/tests/test_validate_focus_visibility.py` regression-tests the validator itself.

`.github/workflows/focus-visibility-validate.yml` runs both checks when focus styles or their validation tooling change.

The unified `tools/release_preflight.py` also includes the focus validator and its regression suite.

## Runtime checks still required

Before stable release, verify on supported Desktop and Browser targets that:

- Tab and Shift+Tab reveal a visible focus indicator at every step;
- focused controls are not clipped by scroll containers;
- the focus indicator remains distinguishable in light and dark themes;
- high-contrast focus emphasis remains visible without obscuring control labels;
- focus remains visible on calculator keys and programmer bit cells;
- mode changes do not lose keyboard focus unexpectedly;
- onboarding restores focus predictably after dismissal;
- browser/OS focus rendering does not conflict with CalcNova styling.

Android/iOS switch-control and external-keyboard behavior must also be tested on target devices where supported.

## Non-goals

The source validator does not measure contrast ratios, prove screen-reader order, test keyboard traps, or confirm that Avalonia platform renderers display borders identically. Those remain runtime release evidence requirements.
