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
- SQLite native history, browser-safe history/storage, history search/favorites/delete/clear, and TXT/CSV/JSON export engine.
- Explicit history export preview and clipboard-copy workflow for the currently visible/search-matching history set.
- Shared settings, external-link, clipboard, and platform composition abstractions.
- Global minimum touch-target sizing and accessible programmer bit-state labels.
- Keyboard-first shared-shell mode navigation with `Ctrl+PageUp`, `Ctrl+PageDown`, `Ctrl+Home`, and `Ctrl+End`.
- Dedicated keyboard shortcut mapping tests, SDK-independent keyboard contract validation, and a keyboard validation workflow.
- Shared-XAML command/property contract validation for Calculator, Programmer, Unicode, Converter, Statistics, Matrices, Graphing, and History.
- Avalonia XAML XML well-formedness validation in the shared UI contract workflow.
- GitHub Actions workflow foundations for build/test, formatting, docs, coverage, security, advanced utilities, platform builds, UI contracts, and release work.
- Detailed feature, input-safety, programmer, converter, numerical-analysis, accessibility, build, testing, platform, privacy, and release documentation.

### Changed

- Shared Avalonia shell expanded from the original calculator workspace to Calculator, Programmer, Code Points, Converter, Statistics, Equations, Matrices, Graph, Date/Time, Currency, History, Settings, and About modes.
- Shared Programmer UI now exposes byte-grouped bits and radix/fixed-width copy actions.
- Shared Converter UI now exposes unit search, search-result From/To assignment, result copy, and clear-recents actions.
- Shared Graph UI now exposes trace, single-series CSV, multi-expression sampling/CSV, and accessible SVG generation/copy controls.
- Shared Statistics and Matrix modes now expose copy actions for generated results.
- Shared History mode now exposes TXT/CSV/JSON preview/copy controls rather than leaving the export engine hidden behind source APIs.
- Shared mode selection now has explicit first/last/direct navigation APIs while cyclic next/previous behavior remains deterministic.
- Shell navigation shortcuts require exactly the Control modifier and remain suppressed while onboarding is visible.
- Programmer non-decimal output now consistently displays fixed-width masked values while decimal output follows signed/unsigned interpretation.
- Converter recent-pair recording now tracks deliberate conversion/swap/restoration actions instead of noisy intermediate selector changes.
- Converter significant-digit precision, recent pairs, and favorites now restore and autosave through shared settings.
- Settings validation now bounds converter precision and persisted pair-token counts/lengths.
- Project state, roadmap, changelog, and feature documentation are being synchronized with the actual implementation rather than the earlier foundation phase.
- Package management remains centralized through `Directory.Packages.props`.
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings remain enabled centrally.

### Fixed

- Scientific-notation marker detection in numeric parsing uses valid APIs.
- Programmer radix parsing safely rejects separator-only and sign-only input.
- Numeric equality and hash-code behavior share a compatible cross-kind representation.
- Programmer arithmetic-right-shift and signed fixed-width result presentation preserve correct two's-complement bit patterns.
- Stale programmer integration expectations now match signed two's-complement decimal presentation.
- Converter saved-pair selection can be reselected after application.
- Converter persistence restore preserves recency ordering, deduplicates entries, and respects capacity.
- Converter recent clearing reports whether persistent state actually changed.
- Settings reset synchronizes converter state immediately rather than waiting for the next launch.
- Expression sanitizer uses evaluator-configured maximum length and preserves the previous expression when imported text is rejected.
- Shared clipboard composition now reaches every currently copy-enabled Calculator, Programmer, Unicode, Converter, Statistics, Matrix, Graphing, and History workflow.
- Transient invalid two-way `TabControl` selection values no longer wrap to another CalcNova mode during initialization or rebinding.

### Security / Privacy

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer/numerical operations include bounded workload controls.
- Clipboard reads are explicit user actions; pasted text is sanitized and is not automatically evaluated.
- Clipboard writes occur only after explicit copy actions.
- History exports are generated locally from the currently loaded entries and are copied only after explicit user action.
- Fixed-unit conversion remains offline.
- Currency networking is optional and contains no embedded provider credential.
- Repository ignore rules exclude common signing credentials and local secret files.

### Validation note

The active continuation environment does not provide the required .NET SDK. Local restore, format, build, and test commands are therefore **NOT RUN** here. Platform packaging is also **NOT RUN** in this continuation. Source-level XAML/UI/keyboard validators and workflow definitions are implemented, but their mere presence is not treated as a passing run. Source/test presence must not be interpreted as a validated release until actual CI/target-environment results are observed.

## [0.1.0] - Planned

The first validated milestone will be created only after the baseline build, analyzer, formatter, test, accessibility, and supported-platform release gates pass in suitable environments. It has not been released yet.
