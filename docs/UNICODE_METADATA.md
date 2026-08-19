# Unicode Scalar Metadata

CalcNova's Code mode includes a local-first Unicode scalar inspection workflow. The metadata implementation does not call a network service and does not require a Unicode lookup API credential.

## Supported metadata

For each valid Unicode scalar value, CalcNova can expose:

- canonical code-point text such as `U+0041` or `U+1F600`;
- the rendered scalar text;
- Unicode plane number;
- .NET Unicode general-category name;
- UTF-8 byte count;
- UTF-16 code-unit count.

The implementation lives in `CalcNova.Programmer` so the scalar rules remain independent of Avalonia UI code.

## Main source contracts

- `src/CalcNova.Programmer/UnicodeCodePointHelper.cs`
  - parses scalar input;
  - validates scalar ranges with `Rune` semantics;
  - formats canonical code-point strings;
  - enumerates text by scalar instead of splitting UTF-16 surrogate pairs;
  - derives stable local metadata.
- `src/CalcNova.Programmer/UnicodeScalarMetadata.cs`
  - immutable metadata record;
  - compact display summary.
- `src/CalcNova.App/ViewModels/CodePointViewModel.cs`
  - projects decoded-scalar metadata;
  - projects one metadata line per inspected scalar;
  - exposes explicit copy commands.
- `src/CalcNova.App/Controls/CodePointMetadataPanel.cs`
  - displays the metadata in the shared Code mode;
  - provides copy controls for scalar and inspected-text metadata.

## Scalar correctness

A Unicode scalar value is not the same thing as an arbitrary UTF-16 code unit. Surrogate values are therefore rejected as standalone code points. Supplementary-plane characters are enumerated as one scalar even though their UTF-16 representation uses two code units.

Examples:

| Input | Plane | General category | UTF-8 bytes | UTF-16 units |
| --- | ---: | --- | ---: | ---: |
| `U+0041` (`A`) | 0 | `UppercaseLetter` | 1 | 1 |
| `U+1F600` (`😀`) | 1 | `OtherSymbol` | 4 | 2 |

The general-category text follows the local .NET Unicode data available to the target runtime. CalcNova does not currently promise human-readable Unicode character names because no separately versioned local name database has been adopted.

## Workload limit

Text inspection uses the same bounded scalar-count contract as the existing code-point workflow. Callers may select a lower limit, but the helper rejects non-positive limits and rejects text that exceeds the requested scalar budget.

This prevents the Code mode from turning a simple inspection operation into an unbounded formatting workload.

## Clipboard behavior

Metadata copy remains explicit and user-triggered:

- **Copy scalar metadata** copies the currently decoded scalar metadata summary.
- **Copy inspected metadata** copies one metadata summary per inspected scalar.

Clipboard availability and failures are handled through the existing platform clipboard abstraction. No clipboard read is needed for metadata generation.

## Privacy

Metadata derivation uses local runtime APIs and encoding calculations only. The Unicode metadata implementation must not introduce HTTP clients, remote URLs, analytics, or provider credentials.

The dedicated source validator enforces the local-only implementation boundary for the metadata core.

## Tests

Relevant regression coverage includes:

- `tests/CalcNova.Programmer.Tests/UnicodeScalarMetadataTests.cs`;
- `tests/CalcNova.App.Tests/CodePointMetadataViewModelTests.cs`;
- `tests/CalcNova.App.Tests/CodePointCopyViewModelTests.cs`;
- `tests/CalcNova.App.Tests/CodePointMetadataPanelHeadlessTests.cs`;
- `tools/tests/test_validate_unicode_metadata.py`.

The headless tests are source-implemented but must not be reported as PASS until their compiled execution is observed in a suitable .NET/Avalonia environment.

## Validation

SDK-independent validation:

```bash
python tools/validate_unicode_metadata.py .
python -m unittest tools.tests.test_validate_unicode_metadata
```

The same contract is wired into `.github/workflows/unicode-metadata-validate.yml` and the integrated SDK-independent release preflight.

Compiled validation still requires the repository's .NET 10 toolchain and must be observed separately.

## Future extensions

Additional Unicode properties should be added only when all of the following are true:

1. the data source is stable and local;
2. its versioning/update policy is explicit;
3. package-size impact is acceptable across Desktop, Browser, Android, and iOS;
4. tests cover supplementary-plane and invalid-scalar behavior;
5. accessibility and compact-layout behavior are validated;
6. the extension does not weaken CalcNova's local-first privacy contract.
