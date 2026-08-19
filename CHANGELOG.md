# Changelog

All notable user-visible changes to CalcNova are documented here.

The format is inspired by Keep a Changelog, and the project intends to use semantic versioning for validated releases.

## [Unreleased]

### Added

- Modular C#/.NET/Avalonia architecture with Desktop, Browser/WebAssembly, Android, and iOS heads.
- Project-owned tokenizer, parser, evaluator, typed calculation errors, and workload limits.
- Arbitrary-precision integer support, decimal-first arithmetic, and bounded floating-point fallbacks.
- Standard/scientific calculation with angle modes, percentage behavior, repeated equals, and classic calculator memory.
- Sanitized imported-expression pipeline with normalization for common calculator glyphs.
- Platform-safe clipboard abstraction with explicit paste and copy-result workflows.
- Selection-aware calculator keypad editing that inserts at the tracked caret, replaces selections, performs caret-aware Backspace, restores the requested TextBox caret, and supports selection-preserving function/parenthesis wrapping.
- Safe top-row/numpad and printable/shifted calculator operator mappings outside active text-editing fields.
- Bounded exact `BigInteger` rational arithmetic with canonical normalization, exact decimal/scientific parsing, arithmetic/comparison, workload limits, Calculator utility UI, tests, focused source validation, and release-preflight integration.
- Bounded engineering-notation formatter/parser with 1–15 significant digits, explicit -324..306 engineering exponent limits, extreme-finite-value scaling, Calculator utility UI, tests, focused source validation, and release-preflight integration.
- Programmer mode with full base 2–36 selection, signed/unsigned fixed-width interpretation, bitwise operations, shifts, and 8/16/32/64/128-bit interactive grids.
- Programmer byte-group presentation plus explicit binary/octal/decimal/hex/fixed-width bit copy actions.
- Unicode scalar/code-point conversion, bounded text inspection, local general-category/plane/UTF-8/UTF-16 metadata, and explicit result/metadata copy workflows.
- Offline fixed-unit converter with recent pairs, favorites, selectable precision, persisted converter preferences, category-scoped unit search, clear-recents, search-result assignment, and result copy.
- Optional currency provider/cache architecture with offline fallback semantics.
- Date difference, calendar arithmetic, business-day, and duration utilities.
- Descriptive statistics plus bounded paired X/Y analysis for population/sample covariance, Pearson correlation, ordinary least-squares regression, `R²`, and regression prediction.
- Shared bivariate-statistics panel with copy/prediction workflows, tests, focused validator/workflow, and integrated release-preflight coverage.
- Equation and matrix modules with shared view models; statistics summary and matrix-result clipboard copy workflows.
- Graph sampling, discontinuity segmentation, viewport/plot support, derivative approximation, bracketed root finding, and Simpson integration.
- Graph nearest-sample trace, bounded table-of-values CSV, bounded multi-expression sampling/identified CSV, and accessible SVG generation/copy workflows.
- Deterministic multi-series graph line patterns and synchronized text legend so series identification does not depend on color alone.
- Graph keyboard viewport interaction: arrow-key pan, numpad zoom, Home reset, and `F` fit-to-data.
- Extreme-finite-value graph numerical-analysis hardening plus explicit sampling/root/integration workload-budget regressions.
- Read-only graph viewport state for deterministic UI integration assertions.
- Reusable bounded export-preview formatter with line/character limits, newline normalization, UTF-16 boundary safety, and complete private copy payloads.
- SQLite native history, browser-safe history/storage, history search/favorites/delete/clear, and TXT/CSV/JSON export engine.
- Explicit history export preview and clipboard-copy workflow for the currently visible/search-matching history set.
- Shared settings, external-link, clipboard, and platform composition abstractions.
- Versioned settings schema with legacy migration and fail-closed future-schema rejection.
- Detection/migration of historical settings JSON that contains no `schemaVersion` property.
- Shared schema-aware settings JSON decoder and validator used by native and Browser storage.
- English and Hindi semantic localization catalogs for the current `AppStringKey` set, including regional English/Hindi culture selection and reviewed live product-surface mappings.
- Global minimum touch-target sizing, accessible programmer bit-state labels, and dynamic graph-control accessibility contracts.
- Explicit visible focus styling for common keyboard controls, with stronger focus emphasis under CalcNova high contrast.
- Keyboard-first shared-shell mode navigation with `Ctrl+PageUp`, `Ctrl+PageDown`, `Ctrl+Home`, and `Ctrl+End`.
- Runtime accessibility evidence matrix using PASS / FAIL / BLOCKED / NOT RUN status discipline.
- Avalonia headless xUnit v3 test foundation using the repository-matched `Avalonia.Headless.XUnit` package.
- Headless shared-shell tests for mode inventory, calculator command binding, selection-aware keypad editing, compact layout class, keyboard mode navigation, high-contrast state, onboarding, graph interaction, supplemental Calculator utilities, paired statistics, and dynamic controls.
- SDK-independent source validators for repository/XAML/UI/navigation/keyboard/calculator editing, graph interaction/presentation/numerical budgets, Unicode metadata, exact rationals, engineering notation, bounded exports, bivariate statistics, headless UI, accessibility/focus/dynamic controls/adaptive layout/touch targets, localization, converter preferences, settings schema, onboarding, packaging/platform workflows, iOS release validation, release workflow/documentation, artifact integrity, and structured release evidence.
- Python regression tests for release-critical source validators and release tooling.
- Integrated SDK-independent release/source preflight covering the current critical validation inventory.
- Cross-platform validation workflows for Desktop, Browser, Android, and iOS source heads.
- Cross-platform workflow source-contract validation protecting runner/workload/build configuration and keeping signing secrets out of normal validation builds.
- Tag-first release workflow validation that detaches at the requested release tag before source preflight and `.NET` validation.
- Exact-release-tag iOS simulator validation workflow that intentionally remains unsigned and does not claim device/App Store readiness.
- Artifact manifest generation/verification and SHA-256 integrity infrastructure.
- Machine-readable release-evidence schema/model/runner/verifier with explicit PASS/FAIL/BLOCKED/NOT RUN semantics.
- Detailed calculator-editing, exact-rational, engineering-notation, bivariate-statistics, Unicode-metadata, graph-interaction/numerical-safety, export-preview, UI-automation, focus/accessibility, settings-migration/storage, platform, source-preflight, validation-evidence, testing, privacy, release, and final-source-audit documentation.

### Changed

- Shared Avalonia shell expanded from the original calculator workspace to Calculator, Programmer, Code Points, Converter, Statistics, Equations, Matrices, Graph, Date/Time, Currency, History, Settings, and About modes, with supplemental exact-rational/engineering and paired-statistics panels integrated into their relevant modes.
- Shared Programmer UI exposes byte-grouped bits and radix/fixed-width copy actions plus local Unicode metadata presentation/copy controls.
- Shared Converter UI exposes unit search, search-result From/To assignment, result copy, clear-recents actions, and persisted recents/favorites/precision.
- Shared Graph UI exposes trace, single-series CSV, multi-expression sampling/CSV, deterministic multi-series presentation, accessible SVG generation/copy controls, numerical analysis, and keyboard viewport interaction.
- Shared Statistics mode exposes descriptive and paired-data covariance/correlation/regression workflows.
- Shared History mode exposes bounded TXT/CSV/JSON previews while preserving complete private copy payloads.
- Shared mode selection has explicit first/last/direct navigation APIs while cyclic next/previous behavior remains deterministic.
- Shell navigation shortcuts require exactly the Control modifier and remain suppressed while onboarding is visible.
- Calculator keypad edits honor the current expression caret/selection instead of always appending at the end.
- Calculator TextBox keyboard/pointer selection state is synchronized with the selection-aware keypad editor.
- Programmer non-decimal output consistently displays fixed-width masked values while decimal output follows signed/unsigned interpretation.
- Converter recent-pair recording tracks deliberate conversion/swap/restoration actions instead of noisy intermediate selector changes.
- Converter significant-digit precision, recent pairs, and favorites restore and autosave through shared settings.
- Native and Browser settings storage share one JSON legacy-detection contract and one preference validator rather than duplicated validation logic.
- Settings validation bounds converter precision and persisted pair-token counts/lengths through the shared Platform validator.
- The localization layer supports English and Hindi semantic catalogs with reviewed live mappings; visible shared XAML still contains unmigrated English and is not presented as fully localized.
- Accessibility documentation distinguishes source implementation from runtime/device evidence using an explicit test matrix.
- Platform support documentation reflects the implemented Browser, Android, and iOS heads/workflows instead of describing them as absent.
- Release documentation treats integrated source preflight as the authoritative first source gate and documents machine-readable validation evidence separately from runtime/manual proof.
- Release workflow validates tagged source before restore/format/build/test and preserves existing GitHub Release notes/history on reruns.
- Source-preflight inventory now includes recent rational, engineering, statistics, dynamic-control, iOS-release, artifact-integrity, and structured-evidence contracts instead of relying on focused workflows alone.
- Feature, roadmap, README, preflight, exact-rational, changelog, and audit documentation are synchronized with the current source scope.
- Package management remains centralized through `Directory.Packages.props`.
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings remain enabled centrally.

### Fixed

- Scientific-notation marker detection in numeric parsing uses valid APIs.
- Programmer radix parsing safely rejects separator-only and sign-only input.
- Numeric equality and hash-code behavior share a compatible cross-kind representation.
- Programmer arithmetic-right-shift and signed fixed-width result presentation preserve correct two's-complement bit patterns.
- Stale programmer integration expectations match signed two's-complement decimal presentation.
- Converter saved-pair selection can be reselected after application.
- Converter persistence restore preserves recency ordering, deduplicates entries, and respects capacity.
- Converter recent clearing reports whether persistent state actually changed.
- Settings reset synchronizes converter state immediately rather than waiting for the next launch.
- Expression sanitizer uses evaluator-configured maximum length and preserves the previous expression when imported text is rejected.
- Shared clipboard composition reaches every currently copy-enabled Calculator, Programmer, Unicode, Converter, Statistics, Matrix, Graphing, and History workflow.
- Transient invalid two-way `TabControl` selection values no longer wrap to another CalcNova mode during initialization or rebinding.
- Release-documentation validation uses the current four-state evidence vocabulary rather than stale exact wording.
- Historical pre-schema settings files that omit `schemaVersion` are explicitly recognized as legacy schema zero instead of accidentally inheriting the current C# default.
- Native and Browser settings validation drift risk is reduced by centralizing the shared validation implementation.
- Calculator keypad insertion/backspace correctly handles forward selections, reversed selections, middle-caret edits, and clamped invalid selection indexes.
- A potentially ambiguous span/string expression replacement implementation was replaced with compiler-safe string slicing.
- Exact-rational source validation no longer looks for a stale/nonexistent magnitude-check marker.
- `default(RationalNumber)` is treated as canonical zero instead of exposing an invalid zero denominator.
- Exact-rational raw input workload limits are enforced before trimming, closing an oversized whitespace-padding bypass.
- Engineering-notation source/tests now enforce the documented finite engineering exponent range, including zero-mantissa inputs.
- Integrated release preflight no longer omits the recent exact-rational, engineering-notation, artifact-integrity, structured-evidence, dynamic-control-accessibility, or exact-tag iOS workflow validators.
- Documentation no longer lists already-implemented exact rationals, engineering notation, covariance/correlation/regression, printable calculator operators, deterministic graph series differentiation, or numerical edge hardening as future features.

### Security / Privacy

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer/numerical operations include bounded workload controls.
- Exact-rational raw input, decimal exponent/scale, and reduced magnitude are bounded.
- Engineering-notation exponent input is explicitly bounded.
- Clipboard reads are explicit user actions; pasted text is sanitized and is not automatically evaluated.
- Clipboard writes occur only after explicit copy actions.
- History/graph exports are generated locally and copied only after explicit user action.
- Unicode metadata is derived locally without a network lookup.
- Fixed-unit conversion remains offline.
- Currency networking is optional and contains no embedded provider credential.
- Repository ignore rules exclude common signing credentials and local secret files.
- Ordinary Android/iOS validation workflows intentionally keep signing secrets out of source.
- Android release signing is conditional on external secrets and uses temporary signing material that is removed afterward.
- iOS release-tag validation is simulator-only unless a future externally secured signing/archive path is added.
- Unsupported future settings schemas fail closed instead of being silently overwritten by an older build.

### Validation note

The active assistant execution environment used for the final source review does not provide the required .NET 10 SDK. Local restore, format, compiled build, compiled tests, and Avalonia headless tests are therefore **NOT RUN** here. Platform runtime/package/signing/accessibility validation is also **NOT RUN** unless separately observed on the actual target environment.

The final current `main` tree was audited through GitHub source/commit inspection and its source contracts were hardened, but the complete integrated preflight was not re-executed locally against a materialized final repository tree in this environment. Source/test/workflow presence must not be interpreted as release PASS evidence. The authoritative next step is observed CI or local .NET/platform execution with structured/manual evidence recorded from the exact release-candidate commit.

## [0.1.0] - Planned

The first validated milestone will be created only after the required source, build, analyzer, formatter, test, accessibility, and supported-platform release gates pass in suitable environments. It has not been released yet.
