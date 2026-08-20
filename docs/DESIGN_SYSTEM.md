# CalcNova 2.8.03 Design System

CalcNova's shared UI is designed to feel calm, precise, modern, efficient, and consistent across Desktop, Browser/WebAssembly, Android, and iOS.

This document describes the implemented 2.8.03 design baseline and the rules future maintenance/enhancement work must preserve.

## Principles

1. **Calculation first.** The active expression/result has clear visual priority.
2. **Progressive disclosure.** Advanced modes remain discoverable without overcrowding ordinary calculator workflows.
3. **Input confidence.** Focused, selected, disabled, error, and active states must be understandable.
4. **Adaptive, not stretched.** Layout responds to available width rather than device-name assumptions.
5. **Accessible by default.** Typography, interaction targets, focus, semantics, contrast, keyboard behavior, and text scaling are design constraints.
6. **Color is not the only signal.** Programmer/graph/state information uses text, patterns, labels, grouping, or other redundant cues.
7. **Minimal interruption.** About/support/donation/onboarding surfaces must not turn ordinary calculation into a promotional flow.
8. **Local-first trust.** UI must make network-enhanced behavior distinguishable from offline calculation where relevant.

## Shared interaction baseline

The shared Avalonia styles/contracts include:

- 44-DIP minimum interaction height for common controls;
- 54-DIP normal calculator-key height;
- at least 50-DIP calculator-key height in compact mode;
- explicit visible keyboard focus;
- stronger focus/border treatment under CalcNova high contrast;
- compact/medium/expanded shell classes;
- focus bring-into-view behavior;
- scrollable content for long/wide modes;
- reduced-motion state;
- keyboard mode navigation;
- accessible automation labels for symbol-heavy controls.

See [ACCESSIBILITY.md](ACCESSIBILITY.md) and [ADAPTIVE_LAYOUT.md](ADAPTIVE_LAYOUT.md).

## Adaptive width profiles

`AdaptiveLayoutProfile` is the source-level width classifier:

```text
Compact   <= 599 DIPs
Medium     600–979 DIPs
Expanded  >= 980 DIPs
```

The shell applies `compact`, `medium`, or `expanded` state from available layout width rather than checking a device model.

Invalid/non-finite/non-positive width input falls back to the compact profile so uncertain measurement fails toward the safer narrow layout.

### Compact

Compact mode:

- reduces non-essential shell spacing;
- preserves shared interaction-target baselines;
- keeps calculator keys touch-friendly;
- uses denser mode/header spacing;
- allows horizontal overflow where a wide control group cannot structurally fit;
- keeps vertical scrolling available;
- brings focused controls into view where supported.

### Medium

Medium mode gives additional working space while retaining a single shared application structure suitable for tablet/compact-window use.

### Expanded

Expanded mode uses wider workspace availability for desktop/tablet layouts without changing domain behavior.

Long values, graph content, programmer grids, history/export content, and localized labels must wrap, scroll, or otherwise remain reachable rather than forcing the application beyond the viewport.

## Spacing

Use a consistent spacing scale rather than arbitrary margins. The shared design language follows a compact progression such as:

```text
2  4  8  12  16  20  24  32  40  48
```

Smaller values suit icon/text or closely related controls; larger values separate sections/workspaces.

New screens should reuse existing application resources/styles before introducing one-off dimensions.

## Radius and surfaces

Use semantic corner/surface treatment consistently:

- small radius for compact inline controls;
- medium radius for ordinary controls;
- larger radius for cards/major surfaces where the existing UI language calls for it.

Do not duplicate arbitrary hard-coded values across feature views when a shared resource/style can express the same role.

## Typography hierarchy

The UI should maintain recognizable roles for:

- primary result/display;
- expression/input;
- page/mode title;
- section title;
- body text;
- secondary/metadata text;
- button/key labels;
- programmer/code/bit representations.

Results and metadata must remain readable under long values and text scaling. Wrapping/scrolling/adaptive structure is preferred to silent clipping.

## Semantic colors

Color should express roles rather than feature-specific arbitrary values.

Relevant semantic roles include:

- background;
- surface;
- raised surface;
- primary/secondary text;
- border/divider;
- accent/accent foreground;
- error/warning/success;
- focus;
- selected;
- disabled.

Graph series use a separate presentation identity that includes deterministic non-color line patterns and a text legend so line color is never the only series identifier.

## Theme and accessibility state

The application settings model includes persisted theme preference plus high-contrast and reduced-motion preferences.

The default theme preference is system-driven. Shared UI state reacts to theme/accessibility settings rather than treating them as unrelated per-view styling.

Theme/accessibility changes must not reduce focus visibility, interaction-target size, or semantic clarity.

Platform-specific theme/high-contrast rendering remains runtime evidence and should be tested on representative targets.

## Calculator interaction design

Calculator controls preserve:

- consistent key sizing;
- semantic labels/automation names;
- keyboard equivalence where appropriate;
- expression editing with caret/selection behavior;
- explicit paste/copy actions;
- clear result/error presentation;
- memory/percentage/repeated-equals workflows without changing expression-language semantics.

Operators and scientific functions may be visually grouped, but grouping must not depend only on color.

See [CALCULATOR_EDITING.md](CALCULATOR_EDITING.md) and [CALCULATOR_KEYBOARD_INPUT.md](CALCULATOR_KEYBOARD_INPUT.md).

## Programmer presentation

Programmer mode uses structural/textual redundancy:

- synchronized radix text;
- fixed-width bit patterns;
- interactive bit cells with state-aware accessible labels;
- grouping for large word sizes;
- signed/unsigned textual interpretation;
- explicit copy actions.

64/128-bit presentation must remain reachable under compact layouts rather than shrinking controls below shared target baselines.

See [PROGRAMMER_MODE.md](PROGRAMMER_MODE.md).

## Graph presentation

Graphing follows these design rules:

- series identity is stable;
- line differentiation is not color-only;
- text legend remains synchronized with the visual graph;
- focusable graph control exposes keyboard alternatives;
- trace/sample/analysis text remains available;
- numerical analysis is visibly approximate where appropriate;
- CSV/SVG export remains an explicit user action.

See [GRAPH_SERIES_PRESENTATION.md](GRAPH_SERIES_PRESENTATION.md) and [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md).

## Converter presentation

Converter mode keeps offline fixed-unit behavior visually distinct from optional network-enhanced currency behavior.

The fixed converter supports:

- category/default-pair selection;
- search and From/To assignment;
- swap;
- precision selection;
- recents/favorites;
- clear recents;
- result copy;
- local preference notice/state.

See [CONVERTER_MODE.md](CONVERTER_MODE.md).

## Onboarding

Onboarding is a short shared surface rather than a forced multi-page tour.

Design requirements include:

- explicit Skip/Start actions;
- scrollability under large text/narrow height;
- keyboard/shortcut containment while open;
- predictable focus entry/restoration;
- persisted completion;
- ability to reopen the introduction from settings.

It must not obscure the application's privacy/local-first posture.

See [ONBOARDING.md](ONBOARDING.md).

## Support and donation surfaces

Support/donation content is optional and must remain separate from core functionality.

It must not:

- block calculator use;
- imply payment is required;
- interrupt repeated calculation workflows;
- disguise an external link as a calculator action.

## Branding

CalcNova has a repository-owned geometric brand mark generated without external image/font dependencies by:

```text
tools/scripts/generate_brand_assets.py
```

The mark combines a calculator/grid form with a restrained nova/spark motif.

Generated asset targets include Browser/PWA icons and desktop packaging assets for Linux, Windows, and macOS. Platform-specific mobile resources are maintained through their platform source/packaging structure.

Do not substitute copied/unlicensed third-party calculator branding.

Brand assets should remain recognizable at small sizes, retain adequate contrast, and use source-controlled generation/metadata where practical.

## Motion

Motion should explain state/location changes rather than decorate routine calculation.

Rules:

- never delay startup artificially;
- never slow repeated calculation for animation;
- do not encode essential meaning only in motion;
- honor reduced-motion state for any future transitions that are not essential.

## Error states

Errors should:

- preserve useful expression/input context;
- explain what can be corrected;
- remain distinguishable without relying only on red;
- avoid raw exception/stack output;
- avoid replacing the entire workspace unnecessarily;
- remain accessible under keyboard/screen-reader/text-scaling workflows.

## Localization design

English and Hindi catalogs are part of the 2.8.03 baseline.

Layouts must tolerate:

- longer localized strings;
- regional English/Hindi culture selection;
- text scaling;
- narrow widths;
- no hard-coded layout assumptions tied to one language.

See [LOCALIZATION.md](LOCALIZATION.md) and [LIVE_LOCALIZATION.md](LIVE_LOCALIZATION.md).

## Design review checklist

For a UI maintenance/enhancement change, review as applicable:

- compact/medium/expanded widths;
- Desktop/Browser/mobile composition impact;
- light/dark/system theme behavior;
- CalcNova high contrast;
- reduced motion;
- keyboard focus/order;
- touch targets;
- screen-reader semantics;
- large text;
- English/Hindi strings;
- long expressions/results;
- error/empty/loading/offline states;
- hover/pressed/selected/disabled states;
- non-color information redundancy;
- clipboard/external-link behavior;
- privacy/network distinction.

Automated source/headless checks should be updated when a new deterministic design contract is introduced. Target-platform visual/accessibility behavior remains runtime evidence.

## 2.8.03 classification

- shared adaptive shell: **COMPLETE**;
- interaction-target/focus baseline: **COMPLETE**;
- theme/high-contrast/reduced-motion state: **COMPLETE**;
- programmer non-color/grouped presentation: **COMPLETE**;
- graph non-color/textual presentation: **COMPLETE**;
- onboarding design baseline: **COMPLETE**;
- repository-owned brand asset generation: **COMPLETE**.

Future design changes are maintenance or optional refinement rather than missing 2.8.03 product requirements.
