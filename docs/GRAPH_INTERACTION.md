# CalcNova Graph Interaction

The shared graph plot supports pointer and keyboard interaction while preserving textual graph-analysis and export alternatives.

## Pointer interaction

When the plot is available:

- left-button drag pans the viewport;
- pointer-wheel movement zooms around the pointer coordinate;
- double-tap/double-click fits the viewport to finite sampled data;
- pointer movement updates coordinate text.

## Keyboard interaction

The graph control is focusable. With focus on the plot and no modifiers held:

| Key | Action |
| --- | --- |
| Left Arrow | Pan left by 10% of the current horizontal span |
| Right Arrow | Pan right by 10% of the current horizontal span |
| Up Arrow | Pan up by 10% of the current vertical span |
| Down Arrow | Pan down by 10% of the current vertical span |
| Numpad `+` | Zoom in around the viewport center |
| Numpad `-` | Zoom out around the viewport center |
| Home | Reset to the default `-10..10` viewport |
| F | Fit to finite sampled data |

Modified versions of these keys are not intercepted by the graph mapping. This reduces collisions with shell, browser, operating-system, and assistive-technology shortcuts.

## Accessibility alternatives

Keyboard pan/zoom is not the only graph access path. CalcNova also provides textual sample output, nearest-sample trace output, bounded table-of-values CSV, identified multi-expression CSV, approximate derivative/root/integral output, and accessible SVG generation/copy.

Visual graph lines must not become the sole carrier of mathematical information.

## Workload and viewport bounds

Zoom spans remain bounded by the graph control's minimum/maximum span limits. Keyboard operations reuse the same viewport math as pointer interaction instead of creating an unbounded secondary path.

## Validation

- `GraphKeyboardInputTests` cover deterministic key-to-action mapping.
- `tools/validate_graph_keyboard.py` protects the source wiring between the mapping and `GraphPlotControl`.
- `tools/tests/test_validate_graph_keyboard.py` regression-tests that validator.
- `.github/workflows/graph-keyboard-validate.yml` runs the SDK-independent graph keyboard contracts.
- the unified release preflight includes both the graph validator and its Python regression suite.

Actual keyboard focus rendering, Browser shortcut conflicts, screen-reader discoverability, and platform input behavior still require runtime validation before stable release.
