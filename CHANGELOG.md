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
- Selection-aware calculator keypad editing that inserts at the tracked caret, replaces selections, performs caret-aware Backspace, and restores the requested TextBox caret after programmatic edits.
- Programmer mode with full base 2–36 selection, signed/unsigned fixed-width interpretation, bitwise operations, shifts, and 8/16/32/64/128-bit interactive grids.
- Programmer byte-group presentation plus explicit binary/octal/decimal/hex/fixed-width bit copy actions.
- Unicode scalar/code-point conversion, text inspection, and explicit result-copy workflows.
- Offline fixed-unit converter with recent pairs, favorites, selectable precision, persisted converter preferences, category-scoped unit search, clear-recents, search-result assignment, and result copy.
- Optional currency provider/cache architecture with offline fallback semantics.
- Date difference, calendar arithmetic, business-day, and duration utilities.
- Statistics, equation, and matrix modules with shared view models.
- Statistics summary and matrix-result clipboard copy workflows.
- Graph sampling, discontinuity segmentation, viewport/plot support, derivative approximation, bracketed root finding, and Simpson integration.
- Graph nearest-sample trace, bounded table-of-values CSV, bounded multi-expression sampling/identified CSV, and accessible SVG generation/copy workflows.
- Graph keyboard viewport interaction: arrow-key pan, numpad zoom, Home reset, and `F` fit-to-data.
- Read-only graph viewport state for deterministic UI integration assertions.
- SQLite native history, browser-safe history/storage, history search/favorites/delete/clear, and TXT/CSV/JSON export engine.
- Explicit history export preview and clipboard-copy workflow for the currently visible/search-matching history set.
- Shared settings, external-link, clipboard, and platform composition abstractions.
- Versioned settings schema with legacy migration and fail-closed future-schema rejection.
- Detection/migration of historical settings JSON that contains no `schemaVersion` property.
- Shared schema-aware settings JSON decoder used by native and Browser storage.
- Shared settings validator used by native and Browser storage for culture, precision, history, onboarding, and converter preference bounds.
- English and Hindi semantic localization catalogs for the current `AppStringKey` set, including regional English/Hindi culture selection.
- Global minimum touch-target sizing and accessible programmer bit-state labels.
- Explicit visible focus styling for common keyboard controls, with stronger focus emphasis under CalcNova high contrast.
- Keyboard-first shared-shell mode navigation with `Ctrl+PageUp`, `Ctrl+PageDown`, `Ctrl+Home`, and `Ctrl+End`.
- Runtime accessibility evidence matrix using PASS / FAIL / BLOCKED / NOT RUN status discipline.
- Avalonia headless xUnit v3 test foundation using the repository-matched `Avalonia.Headless.XUnit` package.
- Headless shared-shell tests for mode inventory, calculator command binding, selection-aware keypad editing, compact layout class, keyboard mode navigation, high-contrast state, and onboarding.
- Headless graph tests for keyboard pan/zoom/reset/fit viewport behavior.
- SDK-independent source validators for shared UI, navigation, keyboard mappings, calculator selection editing, graph keyboard interaction, headless UI contracts, accessibility markup, visible focus, accessibility evidence, adaptive layout, touch targets, localization, settings schema, onboarding, packaging metadata, platform workflows, release workflow, and release documentation.
- Python regression tests for release-critical source validators.
- Integrated SDK-independent release/source preflight covering the current validation inventory.
- Cross-platform validation workflows for Desktop, Browser, Android, and iOS source heads.
- Cross-platform workflow source-contract validation protecting runner/workload/build configuration and keeping signing secrets out of normal validation builds.
- Tag-first release workflow validation that detaches at the requested release tag before source preflight and `.NET` validation.
- Exact-release-tag iOS simulator validation workflow that intentionally remains unsigned and does not claim device/App Store readiness.
- Detailed calculator-editing, UI-automation, graph-interaction, focus-visibility, accessibility-evidence, settings-migration/storage, platform, source-preflight, iOS-release-validation, testing, privacy, and release documentation.

### Changed

- Shared Avalonia shell expanded from the original calculator workspace to Calculator, Programmer, Code Points, Converter, Statistics, Equations, Matrices, Graph, Date/Time, Currency, History, Settings, and About modes.
- Shared Programmer UI exposes byte-grouped bits and radix/fixed-width copy actions.
- Shared Converter UI exposes unit search, search-result From/To assignment, result copy, and clear-recents actions.
- Shared Graph UI exposes trace, single-series CSV, multi-expression sampling/CSV, accessible SVG generation/copy controls, and keyboard viewport interaction.
- Shared Statistics and Matrix modes expose copy actions for generated results.
- Shared History mode exposes TXT/CSV/JSON preview/copy controls rather than leaving the export engine hidden behind source APIs.
- Shared mode selection has explicit first/last/direct navigation APIs while cyclic next/previous behavior remains deterministic.
- Shell navigation shortcuts require exactly the Control modifier and remain suppressed while onboarding is visible.
- Calculator keypad edits now honor the current expression caret/selection instead of always appending at the end.
- Calculator TextBox keyboard/pointer selection state is synchronized with the selection-aware keypad editor.
- Programmer non-decimal output consistently displays fixed-width masked values while decimal output follows signed/unsigned interpretation.
- Converter recent-pair recording tracks deliberate conversion/swap/restoration actions instead of noisy intermediate selector changes.
- Converter significant-digit precision, recent pairs, and favorites restore and autosave through shared settings.
- Native and Browser settings storage now share one JSON legacy-detection contract and one preference validator rather than duplicated validation logic.
- Settings validation now bounds converter precision and persisted pair-token counts/lengths through the shared Platform validator.
- The localization layer now supports English and Hindi semantic catalogs; visible shared XAML remains predominantly English and is not presented as fully localized.
- Accessibility documentation now distinguishes source implementation from runtime/device evidence using an explicit test matrix.
- Platform support documentation now reflects the already-implemented Browser, Android, and iOS heads/workflows instead of describing them as absent.
- Release documentation now treats integrated source preflight as the authoritative first source gate.
- Release workflow validates tagged source before restore/format/build/test and preserves existing GitHub Release notes/history on reruns.
- Source-preflight path coverage now reacts to App tests, settings migration sources/tests, central SDK/package policy, headless UI workflow, platform workflows, release workflow, and validation documentation.
- Project state, roadmap, changelog, testing, platform, and feature documentation are synchronized with the actual implementation rather than the earlier foundation phase.
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
- Release-documentation validation now uses the current four-state evidence vocabulary rather than stale exact wording.
- Release preflight now regression-tests its release-documentation, release-workflow, accessibility, keyboard, localization, settings, packaging, platform-workflow, and headless-UI source contracts.
- Historical pre-schema settings files that omit `schemaVersion` are explicitly recognized as legacy schema zero instead of accidentally inheriting the current C# default.
- Native and Browser settings validation drift risk is reduced by centralizing the shared validation implementation.
- Calculator keypad insertion/backspace now correctly handles forward selections, reversed selections, middle-caret edits, and clamped invalid selection indexes.
- A potentially ambiguous span/string expression replacement implementation was replaced with compiler-safe string slicing.

### Security / Privacy

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer/numerical operations include bounded workload controls.
- Clipboard reads are explicit user actions; pasted text is sanitized and is not automatically evaluated.
- Clipboard writes occur only after explicit copy actions.
- History exports are generated locally from the currently loaded entries and are copied only after explicit user action.
- Fixed-unit conversion remains offline.
- Currency networking is optional and contains no embedded provider credential.
- Repository ignore rules exclude common signing credentials and local secret files.
- Ordinary Android/iOS validation workflows intentionally keep signing secrets out of source.
- Android release signing is conditional on external secrets and uses temporary signing material that is removed afterward.
- iOS release-tag validation is simulator-only unless a future externally secured signing/archive path is added.
- Unsupported future settings schemas fail closed instead of being silently overwritten by an older build.

### Validation note

The active assistant execution environment does not provide the required .NET SDK. Local restore, format, compiled build, compiled tests, and Avalonia headless tests are therefore **NOT RUN** here. Platform runtime/package/signing validation is also **NOT RUN** in this continuation unless separately observed on the actual target environment.

The SDK-independent source preflight command was invoked against a downloaded current `main` snapshot during this continuation, but that invocation is not being promoted to release PASS evidence here. Source/test/workflow presence must not be interpreted as a validated release until actual CI/target-environment results are observed.

## [0.1.0] - Planned

The first validated milestone will be created only after the required source, build, analyzer, formatter, test, accessibility, and supported-platform release gates pass in suitable environments. It has not been released yet.
