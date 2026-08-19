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
- accessible state names for programmer bit cells such as `Bit 7, set`;
- textual programmer bit patterns in addition to the interactive bit grid;
- textual graph sampling/analysis output in addition to graphical presentation;
- reduced-motion and high-contrast preference fields in settings;
- scrollable shared-mode layouts rather than fixed-height content clipping.

## Adaptive layout baseline

The shared shell now selects an available-width profile instead of relying on device names:

- **Compact:** up to 599 logical pixels;
- **Medium:** 600–979 logical pixels;
- **Expanded:** 980 logical pixels and above.

Compact mode reduces non-essential padding while preserving minimum interactive target heights. It also enables horizontal scrolling inside shared mode scroll containers as a safe fallback for wide calculator/function/date grids that have not yet received a deeper structural reflow. Focus changes are configured to bring the focused control into view where the Avalonia scroll container supports that behavior.

This is an implementation baseline, not a claim that every mode is fully optimized for phones. The final mobile pass must still verify actual portrait/landscape behavior, text scaling, tab-header navigation, screen-reader order, and 64/128-bit programmer interaction on target devices.

## Requirements

### Screen readers and semantics

- Every control must have an understandable accessible name.
- Glyph-only buttons such as backspace, square root, or operators need semantic labels where the visual symbol is insufficient.
- Results and important calculation errors should be announced in a useful, non-disruptive way where platform APIs allow it.
- Mode, angle, signed/unsigned, word-size, and favorite state must be exposed semantically, not only visually.
- Dynamic bit states must remain understandable after toggling.

### Keyboard

- All essential desktop/browser functions must be reachable without a mouse.
- Tab order must follow the visual/logical workflow.
- Focus must always be visible.
- Dialogs must trap focus only while open and restore focus when closed.
- No calculator interaction may create an unrecoverable keyboard trap.
- Large programmer bit grids must preserve predictable focus order.

### Touch targets

Common shared controls now use a minimum 44-pixel height, while calculator keys use 54 pixels in normal layouts and at least 50 pixels in compact mode. This is a source-level baseline, not a substitute for device testing.

Dense scientific/programmer layouts should adapt rather than shrinking important targets below usable sizes. The 64/128-bit grid requires special compact-layout review on phones.

### Text scaling

- Text must remain readable with platform text scaling/large-text settings.
- Results may wrap or adapt rather than being clipped silently.
- Layouts should not depend on one fixed font size.
- Important labels must not disappear solely because text is enlarged.
- Tab/navigation presentation must remain usable when labels scale.

### Contrast

- Light, dark, and any future AMOLED/accent themes must retain readable contrast.
- Focus states must remain visible in each theme.
- Disabled state must remain distinguishable without becoming unreadable.
- High-contrast preference behavior must be validated rather than inferred from a stored setting.

### Color

Information must not be conveyed by color alone. Graph functions, error states, base indicators, bit states, favorites, and selected modes need labels, patterns, markers, or other redundant cues where appropriate.

### Motion

Motion should be purposeful and brief. Future transitions/graph interactions should respect reduced-motion preferences where practical. No essential meaning should depend on animation.

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

The shared UI already provides textual sample/analysis output, but trace/table-of-values and full graph keyboard interaction remain future work.

## Clipboard accessibility and privacy

Clipboard operations are explicit buttons. Paste reads clipboard text only after user action, sanitizes it, and reports errors through calculator status text. Copy reports successful result copying through status text.

Target-platform testing must verify these status messages are discoverable to assistive technologies and that clipboard permission/browser prompts do not create keyboard traps.

## Testing checklist

Before a stable release, test representative workflows with:

- keyboard only;
- screen reader on available supported platforms;
- large text/text scaling;
- light theme;
- dark theme;
- high-contrast settings where supported;
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
