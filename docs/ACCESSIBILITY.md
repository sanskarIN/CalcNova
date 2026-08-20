# CalcNova 2.8.03 Accessibility

Accessibility is part of the completed CalcNova 2.8.03 source baseline, not a post-release decoration.

Source implementation completeness and target-platform accessibility evidence are intentionally separate. CalcNova includes shared accessibility contracts and automated source/headless coverage, while screen-reader, text-scaling, contrast, touch, and device behavior must still be recorded as `PASS / FAIL / BLOCKED / NOT RUN` only after the relevant runtime check is actually performed.

## Completed shared baseline

The 2.8.03 shared source includes:

- a 44-DIP minimum interaction-target baseline for common controls;
- a 54-DIP standard calculator-key baseline;
- compact calculator keys that remain at least 50 DIPs tall;
- width-driven compact, medium, and expanded shell profiles;
- compact horizontal-overflow fallback for wide mode content;
- focus bring-into-view behavior;
- explicit visible keyboard focus styling;
- stronger focus/border styling under CalcNova high contrast;
- reduced-motion shell preference state;
- keyboard Enter/Escape/Backspace calculator behavior;
- deterministic top-row/numpad input mappings outside active text editing;
- `Ctrl+PageUp` / `Ctrl+PageDown` cyclic mode navigation;
- `Ctrl+Home` / `Ctrl+End` first/last mode navigation;
- automation names for symbol-heavy calculator/programmer controls;
- accessible programmer bit-state names;
- structural grouping and textual alternatives for large programmer bit representations;
- explicit programmer copy actions;
- keyboard-operable graph viewport controls;
- non-color-only graph series patterns plus text legend;
- textual graph sample/analysis/trace alternatives;
- accessible SVG export;
- onboarding shortcut suppression and focus restoration;
- scrollable mode surfaces;
- localization-aware reviewed shared surfaces;
- SDK-independent accessibility source validators;
- focused Avalonia headless regression coverage.

## Evidence boundary

A source contract proves that required markup/state/logic exists. It does not prove how every operating system, browser, screen reader, display scale, or input device presents that behavior.

Use the runtime evidence vocabulary:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Do not mark TalkBack, VoiceOver, Narrator, browser accessibility, measured contrast, large text, or touch behavior PASS merely because source validators exist.

See [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md) and [RUNTIME_VALIDATION_RUNBOOK.md](RUNTIME_VALIDATION_RUNBOOK.md).

## Adaptive layout baseline

The shared shell selects an available-width profile rather than relying on device names:

- **Compact:** up to 599 DIPs;
- **Medium:** 600–979 DIPs;
- **Expanded:** 980 DIPs and above.

Compact mode reduces non-essential spacing while preserving interaction-target baselines. Horizontal mode scrolling is available as a fallback for wide content. Focused controls request bring-into-view behavior where supported by the Avalonia container.

Long values, programmer grids, history/export content, localized labels, and graph controls must remain reachable through wrapping, scrolling, or adaptive structure rather than being silently clipped.

See [ADAPTIVE_LAYOUT.md](ADAPTIVE_LAYOUT.md).

## Source-level accessibility gates

`tools/validate_accessibility_markup.py` and related validators protect deterministic source requirements such as:

- automation names for symbol-heavy controls covered by the contract;
- common 44-DIP interaction-target styling;
- calculator-key sizing;
- CheckBox target styling;
- visible focus/high-contrast selectors;
- shared `high-contrast` and `reduced-motion` state classes.

Additional source contracts cover:

- focus visibility;
- adaptive layout;
- touch targets;
- dynamically inserted control accessibility;
- onboarding focus behavior;
- graph keyboard/surface behavior;
- programmer bit accessibility;
- localization/catalog integrity;
- accessibility evidence discipline.

These checks run through focused workflows and the integrated source preflight:

```bash
python tools/release_preflight.py
```

## Screen readers and semantics

Shared design requirements are:

- every essential control has an understandable accessible name or platform-derived label;
- symbol-only controls expose semantic names where the glyph alone is insufficient;
- important state such as mode, angle unit, signed/unsigned interpretation, word size, favorite state, and programmer bit state is available semantically rather than only visually;
- graph series have textual/non-color identifiers;
- dynamic controls remain understandable after state changes;
- overlays/onboarding do not leave focus behind hidden content.

Live announcements should be used selectively. Automatically announcing every result/status mutation can become disruptive, so live-region behavior requires platform assistive-technology validation rather than blanket implementation.

## Keyboard accessibility

On keyboard-capable targets, essential workflows should remain operable without a mouse.

The implemented shared baseline includes:

- visible keyboard focus;
- calculator key mappings;
- selection/caret-aware editing;
- mode navigation shortcuts;
- onboarding background-shortcut suppression;
- focus restoration after onboarding;
- keyboard-operable graph panning/zoom/reset/fit;
- focusable programmer bit controls.

Graph keyboard controls include:

- arrow-key panning;
- numpad Add/Subtract zoom;
- Home reset;
- `F` fit-to-data.

Target browsers/desktop environments must still be checked for shortcut conflicts and actual focus order.

## Touch targets

Common shared controls use a minimum 44-DIP height baseline. Standard calculator keys use 54 DIPs; compact calculator keys remain at least 50 DIPs tall.

Touch-target source contracts also cover dynamically inserted shared controls where applicable.

Real Android/iOS/touchscreen evidence should verify reachability, spacing, orientation behavior, and accidental-activation risk on representative devices.

## Text scaling and localization

Layouts should tolerate:

- platform text scaling;
- large text/Dynamic Type where supported;
- Hindi/English reviewed localized strings;
- long results and expressions;
- narrow portrait/landscape surfaces.

Important labels/actions must not disappear solely because text grows.

Onboarding remains scrollable so Skip/Start actions can stay reachable under larger text.

Runtime large-text evidence remains target-specific.

## Contrast and high contrast

CalcNova includes an application high-contrast preference that applies stronger focus/border styling across common interactive controls.

This implemented state is not the same as measured contrast evidence for every platform/theme/state.

Runtime review should measure or otherwise verify, as appropriate:

- representative foreground/background combinations;
- focus indicators;
- disabled/selected/error states;
- light/dark themes;
- interaction with supported system high-contrast modes;
- onboarding and graph presentation.

Visual inspection alone should not be recorded as a measured-contrast PASS when a measurement is required.

## Color independence

Essential information must not depend on color alone.

The completed source baseline provides examples such as:

- programmer textual bit representations and state labels;
- graph deterministic line patterns;
- graph text legend;
- textual graph sample/analysis output;
- explicit selected/state text where appropriate.

Future UI changes must preserve equivalent redundant cues.

## Reduced motion

The shared shell exposes a `reduced-motion` state class derived from preference state.

Current 2.8.03 shared UI does not require decorative animation to communicate essential meaning. Future transitions/animations must respect reduced-motion behavior where appropriate and must not make calculation workflows slower or inaccessible.

## Programmer accessibility

The current programmer surface includes:

- keyboard-focusable bit buttons;
- state-aware accessible bit labels;
- 8/16/32/64/128-bit presets;
- byte-grouped presentation for large word sizes;
- fixed-width textual representation;
- radix/fixed-width copy actions;
- signed/unsigned interpretation state.

Runtime evidence should verify actual screen-reader announcements, focus order, large-text behavior, narrow layouts, and high-contrast behavior on each claimed target.

See [PROGRAMMER_MODE.md](PROGRAMMER_MODE.md).

## Graph accessibility

The completed graph accessibility baseline includes:

- expression/series identities independent of color;
- deterministic non-color-only line patterns;
- synchronized text legend;
- textual sample/analysis/trace information;
- bounded CSV output;
- accessible SVG export;
- focusable graph control;
- pointer interaction;
- keyboard pan/zoom/reset/fit;
- explicit approximate numerical-analysis semantics.

Platform screen-reader/navigation behavior still requires runtime evidence.

See [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md), [GRAPH_SERIES_PRESENTATION.md](GRAPH_SERIES_PRESENTATION.md), and [GRAPH_VIEWPORT_CONTROLS.md](GRAPH_VIEWPORT_CONTROLS.md).

## Onboarding accessibility

The shared onboarding surface is short, scrollable, and dismissible through explicit text actions.

Implemented behavior includes:

- accessible Skip/Start actions;
- global calculator/mode shortcut suppression while onboarding is visible;
- queued focus into the onboarding action surface;
- focus restoration to calculator input after dismissal;
- persisted dismissal/completion state;
- ability to show the introduction again through settings.

Runtime validation should cover initial focus, assistive-technology traversal/context, large text, portrait/landscape, and focus restoration timing.

See [ONBOARDING.md](ONBOARDING.md).

## Clipboard accessibility and privacy

Clipboard workflows are explicit user actions.

Paste sanitizes imported expression text before evaluation. Copy actions report status through shared application state where implemented.

Target testing should verify that permission prompts/failures remain usable with keyboard and assistive technology and do not create traps.

See [PRIVACY.md](PRIVACY.md) and [INPUT_SAFETY.md](INPUT_SAFETY.md).

## Runtime validation matrix

Representative runtime checks include:

- keyboard-only workflows on keyboard targets;
- available screen readers;
- onboarding first launch/Skip/Start/reopen/focus restoration;
- large text/text scaling;
- light/dark/high-contrast states;
- reduced-motion preference;
- compact/medium/expanded widths;
- mobile portrait/landscape;
- 64/128-bit programmer grids;
- calculator selection editing;
- clipboard paste/copy and permission failure;
- graph pointer/keyboard workflows;
- converter saved/search controls;
- history/export preview.

Record results in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md), not as unqualified prose claims in this document.

## Contribution requirement

UI changes must preserve the shared accessibility/adaptive contracts. Pull requests should state relevant accessibility considerations and add/update deterministic automated coverage where practical.

A new UI capability that cannot be operated or understood through the project's supported input/accessibility patterns requires explicit design review before inclusion.

## 2.8.03 classification

- shared accessibility source baseline: **COMPLETE**;
- adaptive/touch/focus contracts: **COMPLETE**;
- programmer accessibility source behavior: **COMPLETE**;
- graph keyboard/non-color/text-alternative source behavior: **COMPLETE**;
- onboarding accessibility source behavior: **COMPLETE**;
- platform-specific runtime evidence: recorded independently as **PASS / FAIL / BLOCKED / NOT RUN**.

Runtime evidence gaps do not redefine the completed 2.8.03 source scope, and source completeness does not justify inventing runtime PASS results.
