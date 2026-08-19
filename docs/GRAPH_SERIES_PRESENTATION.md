# CalcNova Multi-Series Graph Presentation

CalcNova distinguishes simultaneous graph series without relying on color alone.

## Stable presentation identities

The graph domain defines eight ordered line-pattern identities:

1. solid;
2. long dash;
3. short dash;
4. dotted;
5. dash-dot;
6. sparse dash;
7. dense dash;
8. alternating dash.

`GraphSeriesLinePatternCatalog` is the single source of truth for pattern order, human-readable labels, and the deterministic edge mask used by the Avalonia renderer.

The first eight series receive eight distinct patterns in stable input order. The rendering catalog can repeat deterministically after the catalog boundary, while `GraphSeriesPresentationFactory` rejects a request for more than eight simultaneously distinct legend identities instead of pretending additional unique patterns exist.

## Text legend

When the shared Graph mode is in multi-series mode, the UI shows an explicit text legend. A representative entry is:

```text
f1 [solid] — sin(x)
```

The legend therefore communicates:

- stable series label;
- line-pattern name;
- original expression.

A user does not need to infer series identity from color.

## Theme behavior

The current renderer deliberately uses the active theme foreground and varies line pattern. This keeps the distinction meaningful in light, dark, and high-contrast composition without tying mathematical identity to a fixed color palette.

Future color additions may supplement the patterns but must not replace the non-color distinction.

## Shared plot behavior

The interactive shared Graph surface supports:

- single-series `Segments` rendering;
- multi-series `Series` rendering;
- automatic fit-to-data when graph data changes;
- pointer drag pan;
- pointer-wheel zoom;
- keyboard arrow pan;
- numpad `+`/`-` zoom;
- `Home` reset;
- `F` fit-to-data;
- visible localized Pan/Zoom/Fit/Reset controls.

Switching back to a single plot clears the multi-series surface and hides the series legend.

## Validation

Relevant source gates:

```bash
python tools/validate_graph_surface.py .
python -m unittest tools.tests.test_validate_graph_surface
python tools/validate_graph_series_presentation.py .
python -m unittest tools.tests.test_validate_graph_series_presentation
```

Graphing-domain tests verify unique pattern masks and labels. Avalonia headless tests verify multi-series mode wiring, legend visibility, return-to-single behavior, and the visible viewport toolbar.

Real rendering quality, contrast, touch behavior, and assistive-technology output still require target runtime validation.
