# CalcNova Accessibility

Accessibility is a release requirement, not a post-release decoration.

## Current state

The initial Avalonia calculator workspace uses ordinary interactive controls and a keyboard-capable desktop window, but a full accessibility audit has **not** yet been completed.

Do not describe the current pre-release UI as fully accessible until the checks below have been tested on supported platforms.

## Requirements

### Screen readers and semantics

- Every control must have an understandable accessible name.
- Glyph-only buttons such as backspace, square root, or operators need semantic labels where the visual symbol is insufficient.
- Results and important calculation errors should be announced in a useful, non-disruptive way where platform APIs allow it.
- Mode and angle state must be exposed semantically, not only visually.

### Keyboard

- All essential desktop/browser functions must be reachable without a mouse.
- Tab order must follow the visual/logical workflow.
- Focus must always be visible.
- Dialogs must trap focus only while open and restore focus when closed.
- No calculator interaction may create an unrecoverable keyboard trap.

### Touch targets

Touch-first layouts should provide sufficiently large controls with spacing that reduces accidental presses. Dense scientific/programmer layouts should adapt rather than shrinking important targets below usable sizes.

### Text scaling

- Text must remain readable with platform text scaling/large-text settings.
- Results may wrap or adapt rather than being clipped silently.
- Layouts should not depend on one fixed font size.
- Important labels must not disappear solely because text is enlarged.

### Contrast

- Light, dark, and any future AMOLED/accent themes must retain readable contrast.
- Focus states must remain visible in each theme.
- Disabled state must remain distinguishable without becoming unreadable.

### Color

Information must not be conveyed by color alone. Graph functions, error states, base indicators, and selected modes need labels, patterns, markers, or other redundant cues where appropriate.

### Motion

Motion should be purposeful and brief. Future transitions/graph interactions should respect reduced-motion preferences where practical. No essential meaning should depend on animation.

### Error messages

Errors should be concise and understandable. Avoid raw exception text/stack traces in normal UI. Preserve the user's expression so the error can be corrected.

## Graph accessibility

Graphing introduces additional requirements:

- expression labels independent of line color;
- textual coordinate/value views;
- keyboard pan/zoom alternatives where practical;
- table-of-values support;
- clear focus for graph controls;
- no rapid flashing or unnecessary animated effects.

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
- desktop window resizing.

Record platform-specific limitations here rather than hiding them.

## Contribution requirement

UI pull requests should state how accessibility was considered and add/update automated accessibility checks when the project tooling supports them.
