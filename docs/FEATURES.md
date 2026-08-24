# CalcNova 2.9.5 Features

## Status

**Complete for version 2.9.5.**

This document is the completed feature inventory for CalcNova 2.9.5. Environment-specific verification results are evidence records, not unfinished feature requirements.

The 2.9 series preserves the completed calculator feature set while adding stronger release-identity and cross-platform source validation. The requested 2.9.0 checkpoint is preserved in [`releases/2.9.0.md`](releases/2.9.0.md); current source is 2.9.5.

## Standard calculator

- Addition, subtraction, multiplication, and division
- Modulo/remainder expression operator
- Decimal and scientific-notation input
- Parentheses and unary plus/minus
- Right-associative exponentiation and explicit precedence
- Typed syntax/domain/divide-by-zero/workload errors
- Clear, Backspace, evaluate, and result reuse
- Calculator-style percentage transformation
- Repeated-equals behavior
- MC, MR, MS, M+, and M- memory behavior
- Sanitized imported expression text and common calculator-glyph normalization
- User-triggered sanitized clipboard paste
- Explicit valid-result clipboard copy
- Top-row/numpad digit and arithmetic-key handling outside active text fields
- Safe printable/shifted operator mappings outside active text editing
- Tracked-caret insertion
- Forward/reversed selection replacement
- Selection deletion and Backspace-before-caret
- Selection-preserving function/parenthesis wrapping
- Final-expression workload enforcement after editing
- Shared TextBox selection/caret synchronization

## Scientific calculator

- Square, cube, and arbitrary power
- Square, cube, and nth roots
- Reciprocal and absolute value
- Natural/base-10/base-2 logarithms
- Exponential functions
- Trigonometric and inverse-trigonometric functions
- Hyperbolic and inverse-hyperbolic functions
- Degrees, radians, and gradians
- Floor, ceiling, round, truncate, and sign
- Min/max
- Factorial
- GCD/LCM
- Combinations/permutations
- π, e, and τ constants
- Shared scientific keypad controls

## Exact rational arithmetic

- Canonical `BigInteger` numerator/denominator representation
- Positive-denominator and greatest-common-divisor normalization
- Safe canonical `default(RationalNumber)` behavior
- Exact integer, fraction, finite-decimal, and decimal-scientific parsing
- Exact addition, subtraction, multiplication, division, negation, reciprocal, equality, hashing, and comparison
- Multiplication cross-cancellation
- Reduced-denominator addition
- 4,096-character raw input bound before trimming
- 10,000 decimal exponent/scale magnitude bound
- 65,536-bit reduced numerator/denominator bounds
- Calculator utility panel for normalize/add/subtract/multiply/divide
- Core, App, headless, focused-validator, and integrated-preflight regression coverage

See [`EXACT_RATIONALS.md`](EXACT_RATIONALS.md).

## Engineering notation

- Finite `double` formatting with exponents divisible by three
- 1–15 selectable significant digits
- Rounding normalization across the 1000-mantissa boundary
- Invariant-culture canonical parsing
- Engineering exponent range from -324 through 306
- Rejection of malformed/non-engineering exponent forms
- Rejection of non-zero values that underflow to floating-point zero
- Chunked power-of-ten scaling for extreme finite values
- Shared 4,096-character input bound across core parsing, Format action, and UI text entry
- Calculator utility panel
- Core, App, headless, focused-validator, and integrated-preflight regression coverage

See [`ENGINEERING_NOTATION.md`](ENGINEERING_NOTATION.md).

## Programmer calculator

- Base 2–36 parsing, formatting, and selector
- Arbitrary-precision radix conversion
- Binary/octal/decimal/hex synchronized representations
- 8/16/32/64/128-bit word-size presets
- Signed/unsigned two's-complement interpretation
- Correct fixed-width masked non-decimal output
- AND, OR, XOR, and NOT
- Left shift, logical right shift, and arithmetic right shift
- Fixed-width bit strings
- Full interactive bit grid
- Byte-grouped presentation
- Accessible bit-cell state names
- Copy actions for binary/octal/decimal/hex/fixed-width bit representations

## Unicode tools

- Unicode scalar/code-point parsing and formatting
- Scalar-to-text conversion
- Bounded text inspection using scalar semantics
- Local Unicode plane metadata
- Local .NET general-category metadata
- UTF-8 byte width and UTF-16 code-unit width
- Shared metadata presentation
- Explicit code-point/text/metadata copy actions
- Local-first metadata derivation without a network lookup

See [`PROGRAMMER_MODE.md`](PROGRAMMER_MODE.md) and [`UNICODE_METADATA.md`](UNICODE_METADATA.md).

## Unit converter

Offline fixed conversion categories include:

- length
- area
- volume
- mass
- speed
- temperature
- time
- data/storage
- frequency
- pressure
- energy
- power
- force
- angle

Additional completed converter features:

- Category safety
- Unit swapping
- Validated conversion-pair model
- Bounded recent-pair tracking
- Favorite pairs
- Versioned pair persistence tokens
- Persisted recent/favorite state
- 1–17 significant-digit precision
- Shared recents/favorites/precision controls
- Category-scoped unit search
- Search-result assignment to From/To
- Change-aware clear-recents
- Explicit result copy
- Converter default/preference/privacy source contracts

See [`CONVERTER_MODE.md`](CONVERTER_MODE.md).

## Currency converter

- Replaceable provider interface
- Provider/cache architecture
- Local rate cache
- Offline fallback semantics
- No embedded provider credentials
- Shared currency view model and shell integration
- Provider-focused regression source

## Date and duration utilities

- Date difference
- Calendar arithmetic
- Business-day utilities
- Fixed-duration conversion
- Shared date/time view model and shell integration

## Statistics

- Descriptive statistics
- Bounded numeric dataset parser
- Summary copy
- Paired X/Y analysis
- Population covariance
- Sample covariance
- Pearson correlation when mathematically defined
- Ordinary least-squares regression slope/intercept
- Coefficient of determination (`R²`) when defined
- Regression prediction
- Stale-model clearing after failed analysis
- Deterministic handling of mismatched, non-finite, oversized, constant-X, constant-Y, and single-pair datasets
- Shared paired-analysis panel and copy workflow
- Focused source validation and integrated preflight coverage

See [`BIVARIATE_STATISTICS.md`](BIVARIATE_STATISTICS.md).

## Equations

- Equation-solving module
- Shared equation view model
- Quadratic workflows
- App regression source

## Matrices

- Determinant
- Inverse
- Rank
- Linear-system solving
- Shared matrix view model
- Explicit result copy

## Graphing and numerical analysis

- `y = f(x)` sampling through the shared expression engine
- Configurable bounded X range/sample count
- Invalid-sample/discontinuity segmentation
- Explicit graph viewport
- Focusable Avalonia plot control
- Pointer drag panning
- Pointer-wheel zoom
- Double-tap/double-click fit-to-data
- Keyboard arrow-key panning
- Numpad Add/Subtract zoom
- Home reset
- `F` fit-to-data
- Nearest sampled-point tracing
- Bounded single-expression table CSV
- Bounded multi-expression sampling
- Stable generated series identities
- Identified multi-series CSV
- Deterministic line-pattern differentiation that does not rely on color alone
- Synchronized text legend
- Combined finite-data fit-to-view
- Deterministic accessible SVG export
- Bounded central-difference derivative approximation
- Bracketed bisection root finding
- Bounded Simpson integration
- Extreme-finite-value safeguards
- Explicit sampling/root/integration workload budgets
- Dedicated graph surface, presentation, numerical-safety, and workload validators

See [`GRAPH_INTERACTION.md`](GRAPH_INTERACTION.md), [`NUMERICAL_ANALYSIS.md`](NUMERICAL_ANALYSIS.md), and [`GRAPH_NUMERICAL_SAFETY.md`](GRAPH_NUMERICAL_SAFETY.md).

## History and exports

- History repository abstraction
- SQLite native implementation
- Browser-safe storage path
- Initialization/add/recent/search
- Favorites
- Delete one / clear all
- History-limit settings integration
- Shared history UI/view model
- Bounded TXT/CSV/JSON export
- Explicit export-format selection
- Bounded display preview
- Complete private clipboard payload separate from preview
- Character/line limits
- Newline normalization
- UTF-16 boundary safety
- Explicit copy-export action

See [`EXPORT_PREVIEWS.md`](EXPORT_PREVIEWS.md).

## Settings and persistence

- Settings repository abstraction
- Shared settings view model
- Theme/angle/history/accessibility settings
- Converter preference persistence
- Persisted culture preference
- Explicit settings schema version
- Legacy schema-zero migration
- Detection/migration of truly unversioned historical JSON
- Fail-closed negative/future schema rejection
- Shared JSON decoder for native and Browser paths
- Shared preference validator
- Bounded culture/precision/history/onboarding/converter settings

See [`SETTINGS_MIGRATION.md`](SETTINGS_MIGRATION.md).

## Accessibility and adaptive UI

- 44-DIP minimum interaction-target baseline
- 54-DIP standard calculator-key baseline
- Visible focus styling for common controls
- Stronger focus styling in CalcNova high contrast
- Compact/medium/expanded shell profiles
- Compact horizontal-overflow fallback
- Focus bring-into-view
- Ctrl+PageUp/PageDown/Home/End mode navigation
- Keyboard-operable graph viewport
- Accessible programmer bit-state names
- Text alternatives for non-color graph differentiation
- High-contrast and reduced-motion state
- Onboarding focus/shortcut behavior
- Dynamic graph-control focus/touch-target contracts
- Runtime accessibility evidence vocabulary: PASS / FAIL / BLOCKED / NOT RUN
- Source validators for accessibility markup, focus, dynamic controls, adaptive layout, touch targets, and evidence discipline

Runtime evidence remains recorded only when actually observed; this does not change the completed 2.9.5 implementation status.

## Localization

- Stable semantic localization keys
- Complete English semantic catalog for the current key set
- Complete Hindi semantic catalog for the current key set
- Regional English/Hindi culture selection
- Persisted culture preference
- Catalog completeness, duplicate, and unknown-key validation
- Live localization of reviewed shell, calculator, onboarding, settings, history, currency, About, and related surfaces
- Settings checkbox localization in the live capture/apply path

Additional languages or translation expansion are optional post-2.9.5 contributions.

## Platforms

Completed source composition exists for:

- Desktop — Windows/Linux/macOS targets
- Browser/WebAssembly/PWA
- Android
- iOS

Shared platform infrastructure includes:

- Application composition root
- Clipboard abstraction and Avalonia adapter
- External-link abstraction
- Native/Browser persistence composition
- Platform build workflows
- Cross-platform source validator/workflow
- Packaging metadata contracts

Maintained architecture/runtime source matrix:

- Windows: `win-x64`, `win-arm64`
- Linux: `linux-x64`, `linux-arm64`
- macOS: `osx-x64`, `osx-arm64`
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`

Android/iOS display version: `2.9.5`  
Android/iOS numeric build code: `20905`

See [`PLATFORM_SUPPORT.md`](PLATFORM_SUPPORT.md).

## Release identity and 2.9-series maintenance

- `Directory.Build.props` remains the shared release source of truth
- `tools/release_identity.py` validates display/package/assembly/file/informational identity
- Release tag derives as `v<Version>`
- Mobile build code derives as `MAJOR * 10000 + MINOR * 100 + PATCH`
- 2.9.0 checkpoint: `v2.9.0`, build code `20900`
- Current 2.9.5 baseline: `v2.9.5`, build code `20905`
- Packaging validator derives current version/build expectations instead of hardcoding release constants
- Completion-status validator derives current documentation/About expectations from central metadata
- Cross-platform source validator derives mobile build identity from central metadata
- Release-document validator derives current release/documentation markers from central metadata
- Release-identity regression coverage is integrated into Source Preflight
- Linux AppStream preserves stable entries for 2.8.03, 2.9.0, and 2.9.5

See [`VERSIONING.md`](VERSIONING.md) and [`releases/2.9.0.md`](releases/2.9.0.md).

## Release and validation infrastructure

- Repository/security validation
- XAML XML validation
- Shared UI/navigation/keyboard/editing contracts
- Graph surface/presentation/numerical contracts
- Unicode metadata validation
- Exact-rational validation
- Engineering-notation validation
- Export-preview validation
- Bivariate-statistics validation
- Headless UI source validation
- Accessibility/adaptive/localization/settings/onboarding validation
- Central release-identity consistency validation
- Packaging metadata validation
- Desktop/Browser/Android/iOS workflow validation
- Cross-platform source composition validation
- Exact-tag iOS simulator workflow validation
- Release-tag syntax validation
- Release workflow validation
- Release-documentation validation
- Source Preflight workflow self-validation
- CI hygiene validation
- Direct/transitive NuGet audit policy validation
- CodeQL/dependency-review workflow source validation
- Deterministic CycloneDX 1.7 release SBOM generation
- Release basename collision protection
- Flat published-name `SHA256SUMS.txt`
- `actions/attest@v4` release provenance
- Artifact manifest/checksum integrity infrastructure
- Structured release-evidence schema/model/runner/verifier
- Python regression suites
- Unified SDK-independent source preflight

See [`SOURCE_PREFLIGHT.md`](SOURCE_PREFLIGHT.md), [`VALIDATION_EVIDENCE.md`](VALIDATION_EVIDENCE.md), [`ARTIFACT_PROVENANCE.md`](ARTIFACT_PROVENANCE.md), and [`RELEASE_READINESS_CHECKLIST.md`](RELEASE_READINESS_CHECKLIST.md).

## Version identity

- Product/display version: `2.9.5`
- Package version: `2.9.5`
- Release tag: `v2.9.5`
- Assembly/file version: `2.9.5.0`
- Mobile build code: `20905`

See [`VERSIONING.md`](VERSIONING.md).

## Completion classification

- 2.9.5 feature scope: **COMPLETE**
- Shared application integration: **COMPLETE**
- Platform source composition: **COMPLETE**
- Cross-platform source validation: **COMPLETE**
- Release identity consistency: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation/release infrastructure: **COMPLETE**

Later changes are maintenance, compatibility/security updates, translations, tests, evidence collection, or optional enhancements rather than missing 2.9.5 features.
