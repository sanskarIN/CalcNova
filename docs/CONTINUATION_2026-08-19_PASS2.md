# CalcNova Continuation — 2026-08-19 Pass 2

## Scope

This pass resumed from the actual `main` branch and audited recently committed tests against the current implementation before adding more features. Several domain/test commits were present without matching app-layer implementation, which would have caused compile or test failures once a .NET runner executed the solution.

## Repaired app-layer contracts

- Completed `GraphingViewModel` support for graph table rows and CSV output.
- Completed nearest-sample graph tracing.
- Completed graph preview, analysis, trace, and table copy workflows.
- Completed `ConverterViewModel` unit search, selected-search-unit application, clear-recents, and result-copy workflows.
- Changed `ConversionPairHistory.ClearRecent()` to return whether state changed, matching its committed tests and avoiding redundant persistence writes.
- Completed `ProgrammerViewModel` result-copy commands, byte-grouped bit collections, and supported word-size validation.
- Completed Unicode code-point/text result copy commands.
- Injected the shared clipboard service into Programmer, Unicode, Converter, and Graphing modes through `MainViewModel`.
- Added a composition test proving all copy-enabled modes receive the same app clipboard dependency.
- Corrected an obsolete integration assertion that expected unsigned decimal output while signed two's-complement mode was selected.

## Added graph productivity features

- Added a bounded newline-separated graph expression-list parser.
- Supports up to the existing `MultiGraphSampler.MaximumExpressions` limit.
- Assigns stable generated identifiers (`series-1`, `series-2`, ...) and labels (`f1`, `f2`, ...).
- Integrated multi-expression sampling into `GraphingViewModel`.
- Added identified multi-series CSV table output and clipboard-copy support.
- Added multi-expression summary state with valid/invalid sample counts.
- Integrated the existing `SvgGraphExporter` into `GraphingViewModel`.
- Added explicit accessible SVG generation and copy state.
- SVG output is cleared whenever a new single graph is plotted so stale exports cannot be mistaken for the current graph.

## Tests added or repaired

- Shared clipboard composition across copy-enabled modes.
- Multi-expression list parsing, blank-line handling, stable IDs/labels, and maximum-expression enforcement.
- Multi-expression view-model sampling and identified CSV export.
- Multi-expression CSV clipboard copy.
- Accessible SVG generation and stale-export clearing.
- Signed programmer integration expectation aligned with the focused signed-display contract.

## Validation state

The active execution environment still does not provide the .NET SDK. Therefore the following have **not** been claimed as passing:

- `dotnet restore`
- `dotnet format --verify-no-changes`
- `dotnet build`
- `dotnet test`
- Android package/build validation
- iOS archive validation
- Browser/WebAssembly publish validation
- Desktop package/signing validation

The repository `Build and Test` workflow is configured for pushes and pull requests targeting `main`, across Ubuntu, Windows, and macOS. The GitHub connector available in this continuation exposes pull-request-associated workflow-run lookup only; no run/status checks were returned for the inspected commits. An empty status list is **not** treated as CI success.

## Remaining highest-priority work

1. Expose the completed Converter search/copy/clear-recents actions in the shared UI.
2. Expose Programmer radix-copy and byte-grouped bit presentation in the shared UI.
3. Expose Graph trace/table/multi-expression/SVG actions in the shared UI.
4. Complete adaptive/mobile layout work across all modes.
5. Complete keyboard/focus/screen-reader/high-contrast/large-text validation on real targets.
6. Add stable shared-shell UI/integration automation.
7. Observe actual CI runs from an Actions-capable environment and fix any compiler/analyzer/test failures.
8. Complete packaging/signing/store validation for supported targets.
9. Complete localization, onboarding, and design-system consolidation.
10. Run the final release-gate audit and update release documentation only from verified results.

## Commit discipline

This pass continued to use focused commits for feature, test, fix, composition, and documentation changes. No build, test, accessibility, packaging, or CI result is marked as passing without execution evidence.
