# CalcNova Adaptive Layout Contract

CalcNova uses one shared Avalonia shell across desktop, browser, Android, and iOS. The layout therefore treats viewport width as a capability signal rather than assuming a desktop window.

## Width profiles

The shared `AdaptiveLayoutProfile` defines three source-level profiles:

- **Compact:** width up to 599 device-independent pixels. Intended for narrow windows and phone-class layouts. Horizontal mode scrolling is allowed where needed.
- **Medium:** width from 600 through 979 device-independent pixels. Intended for tablets and compact desktop windows.
- **Expanded:** width above 979 device-independent pixels. Intended for wider desktop/tablet surfaces.

Invalid, non-finite, or non-positive widths normalize to the compact profile so that uncertain startup measurements fail toward the safer narrow-screen behavior.

## Shared interaction requirements

Every primary mode must remain reachable in all profiles. The shared shell keeps vertical scrolling available inside modes, brings focused controls into view, and retains a minimum 44-DIP interactive target baseline for primary controls.

The current source-level contract covers these shared modes: Calculator, Programmer, Unicode, Converter, Statistics, Equations, Matrices, Graphing, Date/Duration, Currency, History, Settings, and About.

## Compact-layout rules

Compact mode reduces shell spacing and control padding without shrinking the global minimum interaction height below 44 DIPs. Calculator keypad buttons keep a larger minimum height because they are high-frequency touch targets. Tab headers are made denser and horizontal mode scrolling is enabled when the complete mode strip cannot fit.

Long values, programmer bit patterns, history exports, graph data, and other result-heavy surfaces must wrap or scroll instead of forcing the shell wider than the viewport.

## Validation

`tools/validate_adaptive_layout.py` performs SDK-independent source checks for:

- compact/medium/expanded style classes;
- adaptive width-change handling;
- compact fallback for invalid widths;
- focus bring-into-view behavior;
- touch-target baseline styles;
- vertical scrolling in the shared shell;
- presence of every primary mode header.

The validator runs in `.github/workflows/adaptive-layout-validate.yml` and has Python unit coverage under `tools/tests/test_validate_adaptive_layout.py`.

These checks reduce regression risk but do not replace runtime Avalonia UI tests, real-device touch checks, large-text checks, or platform screen-reader testing.

## Manual narrow-screen checklist

Before a release candidate is approved, validate at representative phone, tablet, compact desktop, and expanded desktop widths. Confirm that no primary action becomes unreachable, focused controls scroll into view, long text remains readable, the programmer bit grid remains operable, graph controls do not clip, history actions remain reachable, and onboarding can be completed without horizontal page panning.
