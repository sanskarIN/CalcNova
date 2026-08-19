# Bounded Export Previews

CalcNova keeps long text exports usable on compact layouts without silently shortening the data copied by the user.

The shared contract is:

1. full export content is generated first;
2. the UI receives a bounded preview;
3. a truncation notice is appended only when the preview is shortened;
4. explicit copy/export commands use the full generated content rather than the bounded display text.

## Default preview limits

`ExportPreviewFormatter` currently uses these defaults:

- maximum characters: **4,096**;
- maximum lines: **80**.

The limits are presentation bounds, not export-data bounds.

When content is truncated, the preview ends with:

`… preview truncated; full content is preserved for copy/export.`

## History exports

History TXT/CSV/JSON generation remains owned by `HistoryExportService`.

`HistoryViewModel` stores the complete generated export separately from `ExportPreview`. The shared History UI shows only the bounded preview when the generated output is large.

`Copy export` always copies the complete generated payload.

Changing the selected export format clears stale preview/full-content state so the next copy regenerates the requested format.

## Graph exports

The shared Graph UI already binds to `TableCsv`, `MultiTableCsv`, and `SvgExport`. To avoid a risky shared-XAML rewrite while preserving the existing binding contract, long graph outputs use those properties as bounded display text.

The complete payloads are retained privately for:

- single-expression table CSV copy;
- multi-expression table CSV copy;
- SVG copy.

Small graph exports remain byte-for-byte unchanged in their display properties because `ExportPreviewFormatter` returns content unchanged when it is within both limits.

## Newline handling

Preview truncation uses `StringReader` rather than splitting only on `\n`. This keeps line budgeting consistent for:

- LF (`\n`);
- CRLF (`\r\n`);
- CR-only (`\r`) input.

Leading blank lines are preserved when a preview is truncated.

## Unicode safety

Character-budget truncation avoids ending a preview prefix with an unmatched UTF-16 high surrogate. This prevents the formatter from cutting a supplementary-plane character in half at the truncation boundary.

This is a display-safety rule only; the complete export payload is never altered by preview truncation.

## Source contracts

Key implementation paths:

- `src/CalcNova.App/Infrastructure/ExportPreviewFormatter.cs`;
- `src/CalcNova.App/ViewModels/HistoryViewModel.cs`;
- `src/CalcNova.App/ViewModels/GraphingViewModel.cs`.

Relevant regression tests:

- `tests/CalcNova.App.Tests/ExportPreviewFormatterTests.cs`;
- `tests/CalcNova.App.Tests/HistoryExportPreviewViewModelTests.cs`;
- `tests/CalcNova.App.Tests/GraphExportPreviewViewModelTests.cs`.

## SDK-independent validation

```bash
python tools/validate_export_previews.py .
python -m unittest tools.tests.test_validate_export_previews
```

The validator protects both sides of the contract:

- bounded display text exists for long History/Graph exports;
- copy commands use complete private payloads, not bounded preview text.

The validator is also included in `.github/workflows/export-previews-validate.yml` and the integrated source release preflight.

## Evidence policy

The source implementation and regression tests are implemented, but compiled tests are not considered PASS until their execution is observed in a suitable .NET/Avalonia environment.
