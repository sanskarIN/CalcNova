# Changelog

All notable user-visible changes to CalcNova are documented here.

The format is inspired by Keep a Changelog. Semantic version tags are reserved for validated milestones/releases.

## [Unreleased]

### Added

- Modular C#/.NET/Avalonia solution with domain, application, platform, persistence, and test projects.
- Safe tokenizer, recursive-descent parser, expression tree, evaluator, compiled expressions, scoped variables, and typed calculation errors.
- Numeric layer using arbitrary-precision integers, decimal arithmetic where representable, and finite floating-point paths for transcendental operations.
- Standard calculator with precedence, parentheses, repeated equals, calculator-style percentage, positive/negative toggle, result reuse, copy/paste, keyboard/numpad input, and classic MC/MR/MS/M+/M− memory.
- Scientific functions including roots/powers, logarithms, trig/inverse/hyperbolic functions, rounding transforms, constants, GCD/LCM, factorial, combinations, and permutations.
- Programmer calculator domain/UI with base 2–36 parsing/formatting, two's-complement word-size interpretation, bitwise operations, shifts, and bit-pattern display.
- Offline fixed-unit converter with category-safe conversions and searchable definitions.
- Statistics engine/UI covering count, compensated sum, descriptive statistics, variance/standard-deviation variants, quartiles, and percentiles.
- Equation solver/UI for linear/quadratic equations, complex roots, degenerate cases, and bounded bisection.
- Matrix/vector engine/UI covering arithmetic, transpose, determinant, inverse, rank, linear systems, magnitude, dot product, and supported cross product.
- Graphing engine with compiled-expression sampling, workload limits, domain/discontinuity segmentation, viewport calculation, SVG export, and interactive Avalonia plot rendering with pan/zoom/fit/reset/coordinates.
- Date/Duration utilities for signed date differences, business days, calendar arithmetic, and fixed-duration conversion without silent timezone assumptions.
- Optional Currency architecture/UI with replaceable provider/cache interfaces, timestamped rate snapshots, freshness/stale fallback behavior, native JSON cache, Browser `localStorage` cache, and no embedded provider secret.
- Local History UI with search, favorites, selected-entry deletion, confirm-before-clear, configurable limit, optional enable/disable, and user-initiated CSV export.
- Native SQLite history persistence and atomic JSON settings persistence behind platform-neutral contracts.
- Browser `localStorage` implementations for history/settings/currency cache.
- Settings UI/persistence for theme, angle mode, decimal precision, grouping, history, haptics preference, reduced-motion preference, and high-contrast preference.
- Locale-aware display-result formatter separated from canonical parser-safe calculator result state.
- Modular shared Avalonia mode views reused by Desktop, Android, iOS, and Browser heads.
- Desktop host plus Android, iOS, and Browser/WebAssembly/PWA platform heads.
- Safe platform-specific external-link services for project/support actions.
- Browser PWA manifest, service worker/offline shell, favicon/icon/social assets.
- Original CalcNova logo, support badge, social preview source, Android adaptive icon/splash resources, and deterministic raster/icon generation tooling.
- Windows/Linux/macOS packaging metadata and helper scripts.
- Release workflow foundation with desktop/browser artifacts, Android signing-through-secrets support, checksums, and GitHub Release creation.
- Comprehensive test projects for core, programmer, converter, statistics, equations, matrices, graphing, date/time, currency, persistence, and application/view-model behavior.
- Application regression coverage for repeated equals, percentage, memory, sign toggle, history confirmation/export formatting, settings propagation, result formatting, Date/Duration, Currency, and advanced mode view models.
- GitHub Actions workflows for formatting, build/test, code coverage, repository/docs validation, security audit, Desktop, Android, Browser, iOS simulator, and release packaging.
- Contributor/community files, issue forms, PR template, security/support/privacy documentation, platform/build/testing/design/localization/troubleshooting docs, roadmap, validation baseline, `PROJECT_STATE.md`, and `what_changed.md`.

### Changed

- Package management is centralized through `Directory.Packages.props`.
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings are enabled centrally.
- Platform heads now use `AppDependencies`/`AppComposition` instead of duplicating application logic.
- Native persistence contracts were moved out of the SQLite implementation so Browser/WebAssembly remains free of the native database package.
- `MainWindow` now hosts the same shared `MainView` used by single-view platform heads.
- The shared UI was split from one monolithic XAML file into focused mode views under `src/CalcNova.App/Views/Modes/`.
- Calculator display formatting is now independent of canonical numeric result strings so grouping/precision preferences do not break history, copy, repeat-equals, or result reuse.
- Platform CI path filters now validate shared `src/**` changes for Android, Browser, and iOS heads.
- Repository README, architecture, feature matrix, platform support, testing, building, keyboard shortcuts, and roadmap documentation were updated to match actual source.
- Legacy GitHub template workflows targeting .NET 8/WPF placeholder packaging were removed from the validation branch.

### Fixed

- Scientific-notation marker detection in numeric parsing now uses valid APIs.
- Programmer radix parsing safely rejects separator-only and sign-only input.
- Numeric equality and hash-code behavior now share a compatible cross-kind representation.
- Android and Browser startup were corrected to use the current `AppDependencies` composition contract.
- Desktop/iOS/Android/Browser composition now supplies the appropriate external-link and currency-cache implementations.
- Currency-rate caches copy read-only rate dictionaries through explicit enumerable conversion rather than relying on constructor overload assumptions.
- A Date/Duration view-model regression expectation was corrected to the actual documented calendar-add order.
- History clear-all now requires explicit confirmation.
- History export avoids requiring a resizable output stream.
- Calculator touch/XAML bindings were reconciled with the implemented memory/percentage/session commands.
- Result precision/grouping settings now affect visible calculator output instead of remaining persistence-only settings.

### Security

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer operations include configurable workload limits.
- Graph sampling has explicit sample/workload limits and separates invalid/non-finite domains.
- Pasted expressions are length-bounded and treated only as calculator input text.
- Browser/native persistence remains local by default.
- No advertising SDK or behavioral analytics SDK is included by default.
- Currency providers remain optional and no API secret is embedded in the open-source client.
- Repository ignore/validation rules exclude common signing credentials and local secret files.
- Android/Apple/store signing material is expected through local/CI secret stores, not Git.

## [0.1.0] - Planned

The first validated milestone will be created only after the required formatter, analyzer, automated-test, platform-build, accessibility/manual-smoke, and release-documentation gates have actually completed. It has not been released yet.
