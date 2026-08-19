# CalcNova Accessibility

Accessibility is a release requirement, not a post-release decoration.

## Current state

CalcNova now includes several accessibility-oriented implementation measures, but a full platform accessibility audit has **not** yet been completed. Do not describe the pre-release UI as fully accessible until the checks below have been exercised on supported platforms.

Current source/UI measures include:

- global minimum 44-pixel heights for Button, TextBox, ComboBox, CheckBox, TabItem, and ListBoxItem controls;
- 54-pixel minimum height for standard calculator keys, with compact-layout keys remaining at least 50 pixels tall;
- width-driven `compact`, `medium`, and `expanded` shared-shell profiles;
- compact-width horizontal scroll fallback for mode content that still contains wide fixed-column control groups;
- focus-change bring-into-view behavior on shared mode scroll containers;
- keyboard Enter/Escape/Backspace support in the primary calculator mode;
- `Ctrl+PageUp` / `Ctrl+PageDown` shared mode cycling;
- deterministic top-row/numpad digit and numpad arithmetic-key handling outside text fields;
- explicit automation names for symbol-heavy calculator controls including memory, angle, scientific, operator, digit, decimal, and evaluation keys;
- explicit automation names for programmer bitwise AND/OR/XOR/NOT controls;
- accessible state names for programmer bit cells such as `Bit 7, set`;
- textual programmer bit patterns in addition to the interactive bit grid;
- textual graph sampling/analysis output in addition to graphical presentation;
- a shared first-run surface with accessible Skip/Start actions;
- onboarding focus queued to the visible action surface, with focus returned to the calculator input after dismissal;
- an active `high-contrast` shell class that strengthens borders on common interactive controls when the preference is enabled;
- an active `reduced-motion` shell class that future motion styles/components can honor consistently;
- scrollable shared-mode layouts rather than fixed-height content clipping;
- source-level accessibility markup validation in the shared UI workflow.

## Adaptive layout baseline

The shared shell now selects an available-width profile instead of relying on device names:

- **Compact:** up to 599 logical pixels;
- **Medium:** 600–979 logical pixels;
- **Expanded:** 980 logical pixels and above.

Compact mode reduces non-essential padding while preserving minimum interactive target heights. It also enables horizontal scrolling inside shared mode scroll containers as a safe fallback for wide calculator/function/date grids that have not yet received a deeper structural reflow. Focus changes are configured to bring the focused control into view where the Avalonia scroll container supports that behavior.

This is an implementation baseline, not a claim that every mode is fully optimized for phones. The final mobile pass must still verify actual portrait/landscape behavior, text scaling, tab-header navigation, screen-reader order, and 64/128-bit programmer interaction on target devices.

## Source-level accessibility gate

`tools/validate_accessibility_markup.py` is wired into `UI Contract Validate` and currently checks deterministic source rules that do not require an accessibility runtime:

- symbol-heavy calculator/programmer buttons covered by the contract have an explicit `AutomationProperties.Name`;
- common control styles retain the shared 44-pixel minimum-height baseline;
- normal calculator keys retain the 54-pixel baseline;
- compact calculator keys retain at least a 50-pixel minimum height;
- CheckBox touch-target styling remains present;
- high-contrast style selectors remain present for Button, TextBox, ComboBox, TabItem, and ListBoxItem;
- the shared shell continues to apply both `high-contrast` and `reduced-motion` preference classes from settings.

This gate catches accidental source regressions only. It cannot prove screen-reader wording, focus order, measured contrast ratios, text scaling, or target-platform accessibility behavior.

## Requirements

### Screen readers and semantics

- Every control must have an understandable accessible name.
- Glyph-only buttons such as backspace, square root, or operators need semantic labels where the visual symbol is insufficient.
- Results and important calculation errors should be announced in a useful, non-disruptive way where platform APIs allow it.
- Mode, angle, signed/unsigned, word-size, and favorite state must be exposed semantically, not only visually.
- Dynamic bit states must remain understandable after toggling.

Dynamic live-region announcements should be added selectively. Making every result/status change a live announcement can become disruptive, so this needs targeted assistive-technology testing rather than a blanket source rule.

### Keyboard

- All essential desktop/browser functions must be reachable without a mouse.
- Tab order must follow the visual/logical workflow.
- Focus must always be visible.
- Dialogs/overlays must contain navigation appropriately while open and restore focus when closed.
- No calculator interaction may create an unrecoverable keyboard trap.
- Large programmer bit grids must preserve predictable focus order.
- Background calculator/mode shortcuts must not activate through the onboarding overlay.

The current onboarding implementation queues focus to its first visible action and returns focus to the calculator input after dismissal. This behavior still requires runtime verification on each supported keyboard target.

### Touch targets

Common shared controls now use a minimum 44-pixel height, while calculator keys use 54 pixels in normal layouts and at least 50 pixels in compact mode. This is a source-level baseline, not a substitute for device testing.

Dense scientific/programmer layouts should adapt rather than shrinking important targets below usable sizes. The 64/128-bit grid requires special compact-layout review on phones.

### Text scaling

- Text must remain readable with platform text scaling/large-text settings.
- Results may wrap or adapt rather than being clipped silently.
- Layouts should not depend on one fixed font size.
- Important labels must not disappear solely because text is enlarged.
- Tab/navigation presentation must remain usable when labels scale.
- Onboarding content must remain scrollable and keep Skip/Start reachable when text grows.

### Contrast

When the CalcNova high-contrast preference is enabled, the shared shell now applies a `high-contrast` class. Current styles strengthen borders on Button, TextBox, ComboBox, TabItem, and ListBoxItem controls, and TabItem text becomes semi-bold.

This is an implemented visual preference, not proof of platform-level high-contrast conformance. Before stable release:

- measure representative foreground/background and focus-state contrast in light/dark themes;
- verify disabled/selected/error states remain distinguishable;
- verify the preference composes correctly with system high-contrast modes where supported;
- verify the fixed onboarding color treatment remains readable and does not fight system accessibility settings.

### Color

Information must not be conveyed by color alone. Graph functions, error states, base indicators, bit states, favorites, and selected modes need labels, patterns, markers, or other redundant cues where appropriate.

### Motion

The reduced-motion setting now changes shared shell state by applying a `reduced-motion` class. The current shared UI does not contain decorative animation/transitions that require suppression, so there is no observable motion reduction to claim yet.

Future animation/transition styles must either honor the `reduced-motion` class or document why a motion effect is essential. No essential meaning should depend on animation.

### Error messages

Errors should be concise and understandable. Avoid raw stack traces in normal UI. Preserve user input when practical so errors can be corrected. Clipboard rejection, numerical-analysis failure, and converter/programmer validation errors must remain readable without relying on color alone.

## Programmer accessibility

The current bit grid is most-significant-bit first, keyboard-focusable through ordinary buttons, and exposes readable bit-state names. A fixed-width text pattern is also shown.

Before stable release:

- verify actual screen-reader announcements after toggles;
- verify focus order for 8/16/32/64/128-bit modes;
- add byte/nibble grouping without color-only semantics;
- evaluate grouping/virtualization for large grids on narrow screens;
- verify signed/unsigned state is announced clearly.

## Graph accessibility

Graphing requires:

- expression labels independent of line color;
- textual coordinate/value views;
- keyboard pan/zoom alternatives where practical;
- table-of-values support;
- clear focus for graph controls;
- explicit approximate labeling for numerical analysis;
- no rapid flashing or unnecessary animated effects.

The shared UI now provides textual sample/analysis output, nearest-point trace output, bounded table-of-values CSV, multi-expression CSV, and accessible SVG generation/copy. Full graph keyboard pan/zoom interaction and target-platform screen-reader validation remain future work.

## Onboarding accessibility

The shared onboarding overlay is implemented as a short, scrollable surface rather than a multi-page forced tour. Both dismissal actions are text buttons with explicit automation names. The shell suppresses its global calculator/mode shortcuts while onboarding is visible, then queues focus back to the Calculator input after dismissal.

Still validate:

- actual initial focus on Desktop and Browser keyboard targets;
- Android/iOS screen-reader traversal;
- focus restoration timing after the overlay collapses;
- compact landscape/portrait behavior;
- large text and display scaling;
- high-contrast behavior;
- whether assistive technology announces enough page context when the overlay appears.

## Clipboard accessibility and privacy

Clipboard operations are explicit buttons. Paste reads clipboard text only after user action, sanitizes it, and reports errors through calculator status text. Copy reports successful result copying through status text.

Target-platform testing must verify these status messages are discoverable to assistive technologies and that clipboard permission/browser prompts do not create keyboard traps.

## Testing checklist

Before a stable release, test representative workflows with:

- keyboard only;
- screen reader on available supported platforms;
- onboarding first run, Skip, Complete, and focus restoration;
- large text/text scaling;
- light theme;
- dark theme;
- CalcNova high-contrast preference plus supported system high-contrast settings;
- reduced-motion settings where supported;
- narrow mobile layout;
- landscape/tablet layout;
- desktop window resizing;
- 64/128-bit programmer grids;
- clipboard paste/copy;
- graph numerical-analysis controls;
- converter saved-pair controls.

Record platform-specific limitations here rather than hiding them.

## Contribution requirement

UI pull requests should state how accessibility was considered and add/update automated accessibility checks when the project tooling supports them.
