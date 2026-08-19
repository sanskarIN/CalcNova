# CalcNova Graph Viewport Controls

The shared Graph surface exposes the same viewport operations through pointer, keyboard, and visible buttons.

## Visible controls

The Graph toolbar contains eight actions:

- Pan left;
- Pan right;
- Pan up;
- Pan down;
- Zoom in;
- Zoom out;
- Fit graph;
- Reset.

The controls use normal shared `Button` styling, so the repository's 44-DIP minimum target baseline applies. Labels are supplied by the semantic English/Hindi localization catalogs.

## Interaction equivalence

The visible controls call the same public `GraphPlotControl` viewport methods used by keyboard handling:

- `PanLeft()` / `PanRight()` / `PanUp()` / `PanDown()`;
- `ZoomIn()` / `ZoomOut()`;
- `FitToData()`;
- `ResetViewport()`.

Keyboard equivalents remain:

- Arrow keys: pan;
- numpad Add/Subtract: zoom;
- `F`: fit data;
- `Home`: reset.

Pointer drag and wheel zoom remain available.

This avoids maintaining separate viewport math for toolbar, keyboard, and pointer interaction.

## Viewport contract

Reset restores:

```text
x: -10 .. 10
y: -10 .. 10
```

Fit-to-data computes a finite bounding rectangle around currently rendered single-series or multi-series points and applies bounded padding.

Zoom is centered on the viewport center for explicit keyboard/button actions and on the pointer position for wheel zoom.

## Localization

The toolbar labels are semantic `AppStringKey` entries. Hindi labels update immediately when the persisted culture changes.

## Validation

`GraphViewportToolbarHeadlessTests` verifies:

- all eight controls exist;
- Pan changes the viewport;
- Zoom reduces/increases the viewport span as expected;
- Reset restores the default range;
- Hindi toolbar labels are visible after culture switching.

Source contracts are protected by:

```bash
python tools/validate_graph_surface.py .
python -m unittest tools.tests.test_validate_graph_surface
```

Real touch ergonomics, platform focus visuals, screen-reader announcements, and mobile orientation behavior still require target-runtime testing.
