# CalcNova 2.9.5 Completed Roadmap

## Status

**All milestones defined for CalcNova 2.9.5 are complete.**

This document is a record of the completed product roadmap. Items added after 2.9.5 are maintenance or optional enhancement ideas and do not represent missing requirements for the completed release.

## Milestone 1 — Core calculation — Complete

- Project-owned tokenizer/parser/evaluator
- Standard arithmetic and precedence
- Scientific functions and constants
- Degree/radian/gradian modes
- Workload limits and typed errors
- Repeated equals
- Calculator percentage behavior
- Memory operations
- Sanitized external expression import
- Explicit paste/copy workflows
- Caret/selection-aware editing
- Printable and numpad keyboard handling

## Milestone 2 — Exact numeric utilities — Complete

### Exact rationals

- Canonical `BigInteger` rational representation
- Exact decimal/scientific parsing
- Exact arithmetic/comparison
- Default-value safety
- Input/scale/bit-length bounds
- Shared Calculator panel
- Tests and source validation

### Engineering notation

- Multiples-of-three exponents
- 1–15 significant digits
- Canonical parser
- Explicit finite exponent bounds
- Non-zero-underflow rejection
- 4,096-character core/App/UI input bound
- Shared Calculator panel
- Tests and source validation

## Milestone 3 — Programmer and Unicode — Complete

- Base 2–36 parsing/formatting
- Fixed-width signed/unsigned interpretation
- AND/OR/XOR/NOT
- Left/logical-right/arithmetic-right shifts
- 8/16/32/64/128-bit grids
- Accessible bit-cell states
- Radix and bit-pattern copy workflows
- Unicode scalar/code-point conversion
- Bounded scalar inspection
- Local general-category/plane/UTF-8/UTF-16 metadata
- Local-first Unicode metadata UI and copy workflows

## Milestone 4 — Conversion and utility modes — Complete

- Offline fixed-unit conversion
- Search, swap, recents, favorites, restoration, clear, and copy
- Persisted converter preferences
- 1–17 significant-digit precision
- Converter default/preference source contracts
- Replaceable currency provider/cache architecture
- Offline currency fallback
- Date difference
- Calendar arithmetic
- Business-day utilities
- Fixed-duration conversion

## Milestone 5 — Statistics, equations, and matrices — Complete

- Descriptive statistics
- Bounded dataset parsing
- Paired covariance/correlation/regression analysis
- `R²` and prediction when defined
- Deterministic degenerate/non-finite/oversized handling
- Shared paired-statistics UI and copy workflow
- Equation-solving workflows
- Matrix determinant/inverse/rank/linear-system solving
- Matrix result copy

## Milestone 6 — Graphing and numerical analysis — Complete

- Bounded graph sampling
- Discontinuity segmentation
- Interactive viewport
- Pointer/keyboard pan, zoom, reset, and fit
- Nearest-point trace
- Single/multi-expression CSV
- Stable multi-series identities
- Non-color-only line patterns and text legend
- Accessible SVG export
- Bounded derivative approximation
- Bisection root finding
- Simpson integration
- Extreme-finite-value safeguards
- Explicit numerical workload budgets

## Milestone 7 — History, persistence, and export — Complete

- Native SQLite history abstraction
- Browser-safe storage
- Recent/search/favorite/delete/clear flows
- TXT/CSV/JSON export
- Bounded previews with full private copy payloads
- UTF-16-safe preview formatting
- Settings repository/view model
- Versioned schema
- Legacy/unversioned migration
- Fail-closed future-schema handling
- Shared native/Browser settings decoder and validator

## Milestone 8 — Accessibility, adaptive UI, and onboarding — Complete

- 44-DIP interaction baseline
- 54-DIP calculator-key baseline
- Visible focus states
- Stronger high-contrast focus states
- Compact/medium/expanded profiles
- Compact overflow fallback
- Focus bring-into-view
- Keyboard mode navigation
- Accessible programmer bit states
- Dynamic graph-control focus/touch-target contracts
- Reduced-motion/high-contrast preference state
- Onboarding shortcut/focus behavior
- Conservative runtime evidence matrix

## Milestone 9 — Localization foundation and reviewed surfaces — Complete

- Stable semantic string keys
- English catalog
- Hindi catalog for current semantic key set
- Regional English/Hindi culture selection
- Persisted culture preference
- Catalog completeness/duplicate/unknown-key validation
- Reviewed live localization across shell, calculator, onboarding, settings, history, currency, About, and related surfaces

Additional languages and extra UI-string migration are optional translation improvements rather than incomplete 2.9.5 requirements.

## Milestone 10 — Cross-platform composition — Complete

- Desktop — Windows, Linux, macOS
- Browser/WebAssembly/PWA
- Android
- iOS
- Shared clipboard abstraction
- External-link abstraction
- Native/Browser persistence composition
- Windows/Linux/macOS x64 + ARM64 release targets
- Android ARM/ARM64/x86/x64 source runtime identifiers
- iOS ARM64 device and ARM64/x64 simulator source runtime identifiers
- Focused cross-platform source validator/workflow
- Mobile release identity `2.9.5` / build code `20905`

## Milestone 11 — Validation and release infrastructure — Complete

- Repository/security validation
- XAML and UI source contracts
- Keyboard/navigation/editing contracts
- Graph/numerical validators
- Unicode/exact-rational/engineering/statistics/export validators
- Accessibility/adaptive/localization/settings/onboarding validators
- Packaging/platform workflow validators
- Cross-platform source composition validator
- Source Preflight workflow self-validation
- Release workflow and exact-tag validation
- iOS simulator release-tag workflow contract
- Artifact manifest/checksum integrity tooling
- Structured release evidence model/runner/verifier/schema
- Deterministic CycloneDX 1.7 release SBOM generation
- Release checksum/provenance/least-privilege contracts
- Unified SDK-independent source preflight
- Focused GitHub Actions workflows

## Milestone 12 — Version 2.8.03 finalization — Complete historical baseline

- Public version `2.8.03`
- Strict SemVer/package equivalent `2.8.3`
- Normalized release tag `v2.8.3`
- Assembly/file version `2.8.3.0`
- Mobile build code `20803`
- Completed 2.8.03 product baseline

The 2.8.03 completion remains preserved in the changelog and historical audit documents.

## Milestone 13 — 2.9-series release consistency — Complete

### 2.9.0 checkpoint

- Central source version set to `2.9.0`
- Release tag contract `v2.9.0`
- Android/iOS build code `20900`
- About and release-label regressions updated to 2.9.0
- Linux AppStream 2.9.0 stable entry added
- Historical checkpoint preserved in `docs/releases/2.9.0.md`

### 2.9.5 current baseline

- Central source version set to `2.9.5`
- Release tag contract `v2.9.5`
- Android/iOS build code `20905`
- About and release-label regressions updated to 2.9.5
- Linux AppStream 2.9.5 stable entry added while retaining 2.9.0 and 2.8.03
- `tools/release_identity.py` centralizes SDK-independent release identity parsing and mobile build-code derivation
- packaging validation derives version/build expectations from central metadata
- completion-status validation derives documentation/About expectations from central metadata
- release-identity regression coverage is integrated into Source Preflight
- current documentation/status is synchronized to 2.9.5

## Environment Verification

Build, device, browser, signing, notarization, provisioning, accessibility-tool, and store checks require their corresponding environments and credentials. Results are recorded only when actually observed.

An environment-specific `NOT RUN` or `BLOCKED` record is evidence metadata, not an incomplete roadmap item.

## Optional Post-2.9.5 Ideas

The following may be considered later without changing the completed status of 2.9.5:

- additional language packs;
- optional UI refinements based on user feedback;
- additional export/share integrations;
- extra mathematical utilities;
- platform compatibility updates;
- additional distribution formats where there is a concrete support requirement;
- performance optimizations proven useful by profiling;
- dependency/security maintenance;
- additional automated regression coverage;
- runtime/device/browser evidence expansion.

No optional idea in this section is required for CalcNova 2.9.5 completion.
