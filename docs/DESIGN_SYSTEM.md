# CalcNova Design System

CalcNova should feel calm, precise, modern, and efficient rather than like a generic tutorial calculator. The design system exists to keep that quality consistent across modes and platforms.

## Principles

1. **Calculation first.** The current expression/result should always have clear visual priority.
2. **Progressive disclosure.** Advanced tools are discoverable without overcrowding Standard mode.
3. **Input confidence.** Pressed, focused, selected, disabled, and error states should be unambiguous.
4. **Adaptive, not stretched.** Tablet/desktop layouts should use additional space meaningfully.
5. **Accessible by default.** Typography, contrast, keyboard focus, semantics, and text scaling are design constraints.
6. **Minimal interruption.** Support/about/promotional surfaces must never block calculation work.

## Layout scale

Use a consistent spacing scale rather than arbitrary margins. Initial guidance:

```text
2  4  8  12  16  20  24  32  40  48
```

Use tighter values for internal icon/text spacing and larger values for page/section separation.

## Radius scale

Recommended semantic radius levels:

- small: compact chips/inline controls;
- medium: ordinary buttons/fields;
- large: cards/panels;
- extra-large: sheets/major surfaces when appropriate.

Exact pixel values should live in reusable Avalonia resources once the component library is established.

## Typography

Typography hierarchy should cover:

- display/result;
- expression;
- page title;
- section title;
- body;
- secondary/metadata;
- button/key label;
- code/programmer bit labels.

Results must remain legible at large text sizes and should adapt/wrap/scroll rather than clip silently.

## Semantic colors

Define resource tokens for roles rather than hard-coding feature-specific colors:

- background;
- surface;
- raised surface;
- primary text;
- secondary text;
- border/divider;
- accent;
- accent foreground;
- error;
- warning;
- success;
- focus;
- selected;
- disabled.

Graph series colors are a separate palette and must have non-color identifiers.

## Theme support

Required long-term themes:

- Light;
- Dark;
- System.

Possible later options:

- AMOLED dark;
- high-contrast preset;
- user accent selection.

Theme changes must not reduce text/focus contrast.

## Core reusable controls

Planned reusable components include:

### CalcButton

Primary calculator key with consistent sizing, focus/pressed semantics, automation/accessibility name, and responsive layout behavior.

### FunctionButton

Scientific/programmer function key with denser visual weight than digits but equal accessibility quality.

### ModeChip

Mode/angle/base selection control with explicit selected state.

### ExpressionDisplay

Editable mathematical expression surface with selection/cursor behavior and error preservation.

### ResultDisplay

High-priority result surface supporting long values, copy/reuse, and future exact/approximate indicators.

### HistoryItemView

Expression/result/time/favorite actions without exposing implementation storage details.

### SectionCard

Reusable settings/about/converter grouping container.

### SettingsItemView

Label/description/control pattern with keyboard and screen-reader semantics.

### SupportCard

Non-intrusive optional support surface. It must never interrupt calculations or imply payment is required.

### AdaptiveShell

Responsive mode navigation/workspace layout that changes structure at available-width thresholds.

## Calculator keys

Keys need consistent:

- minimum touch size;
- pressed/hover/focus state;
- typography;
- semantic label;
- spacing;
- keyboard equivalence where appropriate.

Operators/functions may use visual grouping but should not depend on color alone.

## Responsive structure

### Compact

- expression/result at top;
- keypad below;
- mode navigation through compact selector/navigation;
- history/settings as page/sheet.

### Medium

- larger keypad/work area;
- optional secondary panel;
- more scientific controls visible.

### Expanded

- persistent mode navigation;
- main calculation workspace;
- optional history/secondary pane;
- keyboard hints where useful;
- resizable content.

Breakpoints should derive from available layout width rather than device model names.

## Motion

Use motion to explain state/location changes, not as decoration.

- keep durations short;
- respect reduced-motion preference where possible;
- never delay startup artificially;
- do not animate a result in a way that slows repeated calculation.

## Icons and branding

CalcNova's future logo should combine a clean calculator/grid idea with a restrained nova/spark motif. It must remain recognizable at small sizes and be original.

Do not reuse another calculator app's icon or unlicensed brand assets.

## Error states

Errors should:

- preserve expression text;
- explain what can be corrected;
- remain visually distinct without relying only on red;
- avoid raw exception/stack output;
- avoid replacing the entire workspace.

## Design review checklist

Before merging a stable UI change, review:

- compact/medium/expanded widths;
- light/dark themes;
- keyboard focus;
- text scaling;
- touch targets;
- screen-reader labels;
- long localized strings;
- long expressions/results;
- error states;
- empty states;
- hover/pressed/disabled states.
