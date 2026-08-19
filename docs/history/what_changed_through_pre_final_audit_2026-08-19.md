# What Changed

## Active continuation — 2026-08-19

This file is the current continuation checkpoint for CalcNova. The previous cumulative `what_changed.md` was preserved **verbatim** before this checkpoint at:

- `docs/history/what_changed_through_full_source_hardening_2026-08-19.md`

Nothing from the older cumulative log was discarded; it was archived unchanged so the active continuation file can remain practical to read and update while the repository continues to receive many small commits.

## Scope of this continuation

Work resumed from the actual current `main` branch and deliberately avoided recreating source modules already implemented. The continuation concentrated on remaining source/product polish that could be completed safely without pretending that unavailable platform/runtime evidence exists.

The main completed areas in this pass are:

- graph multi-series presentation hardening;
- local Unicode scalar metadata and its shared UI;
- Unicode metadata copy workflows;
- graph numerical-analysis extreme-value safety;
- graph workload-budget regressions;
- dedicated source validators/workflows and release-preflight integration;
- additional visible localization migration for reviewed product surfaces;
- documentation/roadmap synchronization.

## Graphing — deterministic multi-series presentation

### Added active plot presentation state

- Added `GraphPlotMode` to distinguish the active single-expression plot from the active multi-expression plot.
- `GraphingViewModel.PlotCommand` explicitly restores single-series presentation.
- `GraphingViewModel.PlotMultipleCommand` explicitly activates multi-series presentation after successful sampling.
- Failed single-series plotting keeps the single-series presentation contract deterministic.
- Multi-expression state remains separately represented by `MultiSeries`, `MultiTableRows`, and `MultiTableCsv`.

### Added multi-series rendering to the interactive plot

- Extended `GraphPlotControl` with a `Series` property for multiple `GraphExpressionSample` values.
- Multi-series rendering uses the graph-domain presentation catalog rather than depending on color alone.
- Single-series rendering retains a solid presentation.
- `FitToData()` includes finite points across every active series when multi-series data is supplied.
- The existing viewport interaction remains available:
  - pointer drag pan;
  - pointer-wheel zoom;
  - keyboard arrow-key pan;
  - numpad Add/Subtract zoom;
  - Home reset;
  - `F` fit-to-data;
  - double-tap/double-click fit behavior.

### Centralized non-color series differentiation

During concurrent graph work, the initial App-local pattern catalog was superseded by a graph-domain implementation. The duplicate App-local pattern adapter was removed so there is one canonical series-pattern contract.

Current graph-domain presentation includes:

- deterministic series-index-to-pattern mapping;
- distinct representative pattern masks;
- human-readable line-pattern labels;
- stable series identity/label/expression information;
- valid/invalid sampled-point counts in presentation metadata;
- text legend generation that does not require color perception.

### Shared graph synchronization

`MainView` now synchronizes the shared graph control with the active `GraphingViewModel` presentation:

- single mode supplies `Segments`;
- multi mode supplies `MultiSeries`;
- stale inactive plot data is removed from the renderer;
- the multi-series legend is shown only when multi-series presentation is active;
- graph property-change notifications keep the renderer and legend synchronized.

### Graph presentation tests and validation

Added or expanded coverage for:

- multi-series combined viewport fitting;
- deterministic pattern assignment;
- pattern-mask distinction;
- presentation legend content;
- active plot-mode switching;
- shared-shell multi-series graph integration;
- graph presentation source contracts.

Dedicated graph-series presentation validation and its regression tests are included in the SDK-independent release preflight.

## Unicode — local scalar metadata

### Added `UnicodeScalarMetadata`

Added a domain record containing:

- scalar integer value;
- canonical `U+XXXX` code-point text;
- rendered scalar text;
- Unicode plane number;
- .NET Unicode general category;
- UTF-8 byte count;
- UTF-16 code-unit count;
- compact human-readable summary.

### Extended `UnicodeCodePointHelper`

Added local-only metadata APIs:

- `Describe(int codePoint)`;
- `DescribeText(string?, int maximumRunes = 64)`.

The helper continues to use Unicode scalar semantics rather than treating UTF-16 code units as independent characters.

Important behavior:

- surrogate code points remain invalid standalone scalar values;
- supplementary-plane characters are enumerated as one scalar;
- UTF-8 byte width is derived locally;
- UTF-16 width reflects one or two code units as appropriate;
- the existing bounded inspection limit is shared by metadata inspection;
- non-positive inspection limits are rejected;
- metadata derivation does not require network access.

Representative regression expectations include:

- `U+0041` → plane 0, `UppercaseLetter`, UTF-8 1 byte, UTF-16 1 unit;
- `U+1F600` → plane 1, `OtherSymbol`, UTF-8 4 bytes, UTF-16 2 units.

### Exposed metadata through `CodePointViewModel`

Added:

- `CodePointMetadata`;
- `TextMetadata`;
- `CopyCodePointMetadataCommand`;
- `CopyTextMetadataCommand`.

Decode behavior now projects metadata while preserving the existing decode result format.

Text inspection now projects one metadata summary per Unicode scalar while preserving the existing code-point-list result format.

Invalid input clears stale metadata instead of leaving previous metadata visible.

### Added shared Unicode metadata UI

Added reusable `CodePointMetadataPanel` and injected it into the real shared Code mode.

The panel provides:

- local metadata explanation;
- decoded scalar metadata;
- inspected-text metadata;
- `Copy scalar metadata` action;
- `Copy inspected metadata` action.

The panel explicitly explains that the category/plane/encoding-width information is derived locally without a network lookup.

### Added Unicode metadata tests

Coverage now includes:

- Basic Latin metadata;
- supplementary-plane metadata;
- surrogate-pair-safe text inspection;
- inspection-limit enforcement;
- surrogate rejection;
- view-model metadata projection;
- stale-metadata clearing after invalid input;
- metadata clipboard commands;
- shared metadata-panel rendering;
- metadata-panel command bindings.

### Added Unicode metadata source validation

Added:

- `tools/validate_unicode_metadata.py`;
- `tools/tests/test_validate_unicode_metadata.py`;
- `.github/workflows/unicode-metadata-validate.yml`.

The validator protects:

- metadata record fields;
- helper APIs;
- local encoding/category derivation;
- view-model metadata projection;
- metadata copy commands;
- shared panel wiring;
- headless UI regression presence;
- the local-first core contract by rejecting HTTP client/URL markers from the metadata implementation.

The Unicode metadata validator and its regression test are included in `tools/release_preflight.py`.

## Graph numerical-analysis hardening

A parallel numerical-safety review found real extreme-finite-value risks in approximate graph analysis and hardened the implementation instead of merely adding tests around the previous arithmetic.

### Hardened derivative analysis

- Derivative sample points are required to remain finite.
- Sample points that collapse to the requested X value because the step is too small relative to floating-point magnitude are rejected.
- Non-finite derivative results are rejected deterministically.

### Hardened root analysis

- Added overflow-safe midpoint arithmetic.
- Endpoint roots are returned immediately when within tolerance.
- Midpoint collapse caused by floating-point resolution terminates deterministically using the better endpoint.
- Non-finite evaluated values remain rejected.
- Root search still fails when the interval does not bracket a sign change.
- Root search fails deterministically when `MaximumRootIterations` is exhausted.

### Hardened Simpson integration

- Equal bounds return zero.
- Reversed bounds preserve sign by evaluating the forward interval and negating the result.
- Integration interval width is computed with reduced intermediate-overflow risk.
- Sample positions use interpolation between the endpoints instead of an overflow-prone `minimum + width * index` form.
- Non-finite sample points/results are rejected.

### Added graph numerical edge-case regressions

Added `GraphNumericalEdgeCaseTests.cs` covering:

- left-endpoint root;
- right-endpoint root;
- non-finite root bounds;
- configured root-iteration exhaustion;
- equal integration bounds;
- non-finite integration bounds;
- non-finite derivative X.

### Added graph workload-budget regressions

Added `GraphWorkloadBudgetTests.cs` covering:

- graph sample count above `GraphSampler.MaximumSamples`;
- root iteration count below/above the supported range;
- Simpson interval count above the configured maximum;
- maximum integration budget above the hard cap;
- invalid `MaximumAbsoluteY` values;
- invalid discontinuity-jump thresholds.

These tests complement the existing numerical baseline, extreme-bound, and options-boundary test suites.

### Added graph workload source validation

Added:

- `tools/validate_graph_numerical_budgets.py`;
- `tools/tests/test_validate_graph_numerical_budgets.py`;
- `.github/workflows/graph-numerical-budgets-validate.yml`.

The validator protects:

- graph sample hard cap;
- root iteration range;
- integration hard/configured limits;
- Simpson even-interval requirement;
- maximum-absolute-Y guard;
- discontinuity-threshold guard;
- edge-case regression presence;
- workload regression presence.

This validator and its regression suite are included in the integrated SDK-independent release preflight alongside the separate extreme numerical-analysis safety validator.

## Localization — reviewed product surfaces

The localization continuation expanded the semantic catalog and live literal mapping beyond the earlier shell/calculator/onboarding foundation.

### Added semantic product-surface keys/catalog entries

The current branch added reviewed English/Hindi strings for settings, history, currency, About/support, and related product surfaces.

### Expanded live localization mapping

Reviewed shared product-surface literals are mapped into `ShellLocalization` so culture changes can update more visible content without duplicating translation logic in individual modes.

### Added settings checkbox localization

The live localization capture/apply path now includes settings checkbox content in addition to TextBlock/Button/TextBox watermark targets.

Regression coverage includes Hindi settings checkbox labels and Hindi product-surface mappings.

### Localization limitation remains explicit

CalcNova still does **not** claim that every visible XAML string has been migrated or that Hindi compact/large-text layout has been runtime validated. Remaining hard-coded English must continue to be migrated in compile-verified increments.

## Release-source preflight integration

The integrated preflight now includes the current graph/Unicode contracts in addition to the existing repository, UI, accessibility, settings, packaging, platform, localization, and release validators.

Newly represented checks include:

- graph-series presentation;
- numerical-analysis safety;
- graph workload budgets;
- Unicode metadata contracts;
- their Python validator regression suites.

`tools/tests/test_release_preflight.py` now requires these validators and test modules in the inventory so they cannot silently disappear from the release-source gate.

## Documentation added/updated

### Added

- `docs/UNICODE_METADATA.md`
- `docs/GRAPH_NUMERICAL_SAFETY.md`
- `docs/history/what_changed_through_full_source_hardening_2026-08-19.md` — exact archived copy of the previous cumulative change log.

### Updated

- `docs/README.md` — indexes the new Unicode and graph-safety guides.
- `docs/ROADMAP.md` — moves completed Unicode metadata, deterministic multi-series presentation, and graph workload hardening out of the future-work list.
- `what_changed.md` — current continuation checkpoint.

## Validation status

The evidence policy remains conservative.

### Not observed locally

The active assistant execution environment does not provide the required .NET 10 SDK, so the following are **NOT RUN** locally:

- `dotnet restore`;
- `dotnet format --verify-no-changes`;
- compiled builds;
- compiled unit/integration tests;
- Avalonia headless tests;
- target-platform package builds.

### Independent repository clone attempt

An independent container clone was attempted in order to run the Python validators against a fresh `main` snapshot. The container environment could not resolve `github.com`, so that clone/validator run did not execute and is **not** treated as PASS.

### GitHub status observation

A combined-status lookup performed earlier in this continuation exposed no status contexts for the checked commit. An empty status list is not treated as a successful CI result.

The source validators/workflows are implemented, but an observable successful run is still required before recording CI PASS evidence.

### Commit identity

Current live branch metadata reports recent commits with author/committer identity `Sanskar <sanskarin@outlook.in>`.

## Remaining high-priority work

The remaining work is increasingly dependent on observed execution rather than missing source modules:

1. observe real .NET 10 restore/format/build/analyzer/test results and fix every concrete failure;
2. observe the Avalonia headless workflow and fix compiled-XAML/headless failures;
3. validate Desktop launch/clipboard/persistence/keyboard/scaling/packaging on Windows, Linux, and macOS;
4. validate Browser publish/load/storage/clipboard/keyboard/accessibility behavior;
5. validate Android workload build, emulator/device layouts, clipboard/persistence, TalkBack/large text, signing, and AAB/store behavior;
6. validate iOS simulator/device layout, clipboard/persistence, Dynamic Type/VoiceOver, signing, archive, TestFlight, and App Store behavior;
7. perform the runtime accessibility matrix with observed evidence;
8. validate compact/medium/expanded layouts on real target sizes, especially programmer 64/128-bit surfaces and long exports;
9. continue remaining visible-XAML localization migration and validate Hindi Devanagari layout;
10. improve graph axis/grid labels and optional explicit viewport controls only after current interaction behavior is runtime validated;
11. add native file-save/share export UX only after target platform abstractions are validated;
12. run the final release-candidate gate with observed source, compiled, platform, accessibility, privacy, signing, and packaging evidence.

## Focused commits in this continuation

### Graph presentation

- `54b30045` — feat(graph): define deterministic series line patterns (initial App-local increment; later superseded by the graph-domain catalog).
- `27977d58` — test(graph): cover deterministic series patterns.
- `fba0e88f` — feat(graph): render multi-series with non-color patterns.
- `b116dfdd` — test(graph): cover multi-series plot viewport.
- `b960b246` — feat(graph): model active plot presentation mode.
- `9eb55d95` — feat(graph): track active single or multi-series plot.
- `7c82942b` — refactor(graph): remove superseded app-local pattern adapter.
- `8f42bc0d` and subsequent graph-domain commits — centralize deterministic line-pattern/presentation behavior.
- `72908853` — feat(graph): synchronize single and multi-series plot with text legend.
- `39127179` — headless graph presentation integration coverage.
- `40fea338` — test(release): require graph-series presentation in preflight.

### Unicode metadata

- `b5c41302` — feat(unicode): add local scalar metadata model.
- `9bec3855` — feat(unicode): describe scalar category and encodings locally.
- `2b26abb2` — test(unicode): cover local scalar metadata.
- `2f2dcf51` — feat(unicode): expose scalar metadata in code-point view model.
- `44692f5c` — test(unicode): cover metadata view-model projection.
- `a9c3742e` — ci(unicode): validate local scalar metadata contracts.
- `e39d4707` — test(unicode): cover scalar metadata source validator.
- `7f271e78` — ci(unicode): run scalar metadata source validation.
- `d53b0817` — ci(release): include Unicode metadata validation.
- `8d211e4f` — test(release): require Unicode metadata preflight coverage.
- `b042687f` — feat(unicode): add reusable scalar metadata panel.
- `75f255f0` — feat(unicode): surface local scalar metadata in shared UI.
- `a7f8766e` — test(unicode): cover shared metadata panel rendering.
- `0f2cf2e4` — ci(unicode): validate shared metadata panel wiring.
- `61d98914` — ci(unicode): watch metadata UI integration paths.
- `3ed07e3b` — feat(unicode): add metadata clipboard commands.
- `c39426f6` — test(unicode): cover metadata clipboard commands.
- `18c54d89` — feat(unicode): add metadata copy controls.
- `958abf8c` — test(unicode): cover metadata panel copy bindings.
- `08f618a8` — ci(unicode): guard metadata copy workflow.
- `2f03502f` — ci(unicode): watch metadata copy tests.

### Numerical analysis / workload hardening

- `0cda98e4` — fix(graph): harden numerical analysis against extreme finite bounds.
- `6e003ab3` — fix(graph): avoid overflow in numerical sample interpolation.
- `ac8e4af6` — test(graph): cover extreme numerical-analysis boundaries.
- `0ea5fbfd` — test(graph): cover numerical-analysis workload option boundaries.
- `9ace1754` — ci(graph): add numerical-analysis hardening validator.
- `3c2050a9` — test(graph): cover numerical-analysis validator.
- `a888d75f` — ci(graph): validate numerical-analysis safety contracts.
- `d29e35a9` — ci(release): include numerical-analysis safety validation.
- `df0907eb` — test(release): require numerical-analysis safety in preflight.
- `9db3d929` — test(graph): cover numerical analysis edge cases.
- `781432ad` — test(graph): cover graph workload budgets.
- `65618d2a` — ci(graph): validate numerical workload budget contracts.
- `8a3aa112` — test(graph): cover numerical budget source validator.
- `938969af` — ci(graph): run numerical budget validation.
- `eba84798` — ci(release): include graph workload budget validation.
- `a9ea2f08` — test(release): require graph workload budgets in preflight.

### Localization continuation

- `2a9e635b` — feat(localization): add settings/history/currency/About keys.
- `3a658326` — feat(localization): add English product surface strings.
- `635f4e97` — feat(localization): add Hindi product surface strings.
- `c6fb2f3f` — feat(localization): map reviewed product surface literals.
- `2737bb7c` — test(localization): cover Hindi currency/history/settings/About surfaces.
- `bacaa67a` — feat(localization): localize settings checkbox content live.
- `68df2f90` — test(localization): cover Hindi settings checkbox labels.
- `c3f7a0d2` — ci(localization): validate reviewed product surfaces and checkbox wiring.
- `871cac32` — ci(localization): trigger checks for product surface localization.

### Documentation/checkpoint

- `e28f3306` — docs(roadmap): mark Unicode and graph hardening milestones complete.
- `5451e498` — docs(unicode): document local scalar metadata workflow.
- `ca55bc6e` — docs(graph): document numerical safety and workload bounds.
- `6e5dfb28` — docs: index Unicode and graph safety guides.
- `7489a970` — docs(changelog): archive cumulative change history verbatim.

## Continuation rule

For the next continuation:

1. read `PROJECT_STATE.md`;
2. read this active `what_changed.md`;
3. use the archived historical change log only when older implementation detail is needed;
4. inspect current `main` before writing because parallel continuations may still be active;
5. do not recreate completed graph/Unicode/localization source work;
6. do not report unavailable or unobserved runtime checks as passing.
