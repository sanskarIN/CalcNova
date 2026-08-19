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
- Unicode scalar/code-point conversion and text inspection workflow.
- Offline fixed-unit converter with recent pairs, favorites, selectable precision, and persisted converter preferences.
- Optional currency provider/cache architecture with offline fallback semantics.
- Date difference, calendar arithmetic, business-day, and duration utilities.
- Statistics, equation, and matrix modules with shared view models.
- Graph sampling, discontinuity segmentation, viewport/plot support, SVG export, derivative approximation, bracketed root finding, and Simpson integration.
- SQLite native history, browser-safe history/storage, history search/favorites/delete/clear, and TXT/CSV/JSON export.
- Shared settings, external-link, clipboard, and platform composition abstractions.
- Global minimum touch-target sizing and accessible programmer bit-state labels.
- GitHub Actions workflow foundations for build/test, formatting, docs, coverage, security, advanced utilities, platform builds, and release work.
- Detailed feature, input-safety, programmer, converter, numerical-analysis, accessibility, build, testing, platform, privacy, and release documentation.

### Changed

- Shared Avalonia shell expanded from the original calculator workspace to Calculator, Programmer, Code Points, Converter, Statistics, Equations, Matrices, Graph, Date/Time, Currency, History, Settings, and About modes.
- Programmer non-decimal output now consistently displays fixed-width masked values while decimal output follows signed/unsigned interpretation.
- Converter recent-pair recording now tracks deliberate conversion/swap/restoration actions instead of noisy intermediate selector changes.
- Converter significant-digit precision, recent pairs, and favorites now restore and autosave through shared settings.
- Settings validation now bounds converter precision and persisted pair-token counts/lengths.
- Project state, roadmap, README, and feature documentation were synchronized with the actual implementation rather than the earlier foundation phase.
- Package management remains centralized through `Directory.Packages.props`.
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings remain enabled centrally.

### Fixed

- Scientific-notation marker detection in numeric parsing uses valid APIs.
- Programmer radix parsing safely rejects separator-only and sign-only input.
- Numeric equality and hash-code behavior share a compatible cross-kind representation.
- Programmer arithmetic-right-shift and signed fixed-width result presentation preserve correct two's-complement bit patterns.
- Converter saved-pair selection can be reselected after application.
- Converter persistence restore preserves recency ordering, deduplicates entries, and respects capacity.
- Settings reset synchronizes converter state immediately rather than waiting for the next launch.
- Expression sanitizer uses evaluator-configured maximum length and preserves the previous expression when imported text is rejected.

### Security / Privacy

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer/numerical operations include bounded workload controls.
- Clipboard reads are explicit user actions; pasted text is sanitized and is not automatically evaluated.
- Fixed-unit conversion remains offline.
- Currency networking is optional and contains no embedded provider credential.
- Repository ignore rules exclude common signing credentials and local secret files.

### Validation note

The active continuation environment does not provide the required .NET SDK. Local restore, format, build, and test commands are therefore **NOT RUN** here. Platform packaging is also **NOT RUN** in this continuation. Source/test presence must not be interpreted as a validated release until actual CI/target-environment results are observed.

## [0.1.0] - Planned

The first validated milestone will be created only after the baseline build, analyzer, formatter, test, accessibility, and supported-platform release gates pass in suitable environments. It has not been released yet.
