# CalcNova Project State

## Current Version

**2.9.7**

Normalized .NET/NuGet version: `2.9.7`  
Normalized release tag: `v2.9.7`  
Mobile numeric build code: `20907`

See [`docs/VERSIONING.md`](docs/VERSIONING.md).

## Current Branch

`main`

## Completion Status

**COMPLETE — CalcNova version 2.9.7**

The defined 2.9.7 product scope is implemented in the repository. Core calculation, scientific functions, exact rational arithmetic, engineering notation, programmer and Unicode tools, converter/date-time/currency utilities, descriptive and bivariate statistics, equations, matrices, graphing/numerical analysis, history, persistence, settings, onboarding, localization, accessibility/adaptive UI, Desktop/Browser/Android/iOS composition, source validation, dependency-security policy, artifact integrity, structured release evidence, packaging metadata, release workflows, security automation, release provenance, deterministic release SBOM generation, cross-platform source validation, centralized release identity, and graph accessibility maintenance are present as completed source capabilities.

Future repository changes are classified as maintenance, compatibility updates, security fixes, documentation changes, translation additions, dependency updates, tests, or optional enhancements. They are not required to define the 2.9.7 project as complete.

## Current Maintenance Enhancements — 2026-08-26

### Graph accessibility and localization

- stable eight-action graph viewport toolbar: pan left, pan right, pan up, pan down, zoom in, zoom out, reset, fit to data;
- toolbar actions reuse the existing graph viewport commands instead of duplicating graph state;
- every action is keyboard-focusable and uses the shared 44-DIP minimum interaction target;
- focus restoration after toolbar interaction is protected by source contracts;
- English and Hindi graph-action semantic labels are part of localization validation;
- dynamic graph controls remain in the shared shell localization map.

### Adaptive layout validation

- compact/medium/expanded layout classes remain required;
- current primary mode headers are validated as `Calc`, `Prog`, `Code`, `Convert`, `Stats`, `Eq`, `Matrix`, `Graph`, `Date`, `FX`, `History`, `Settings`, and `About`;
- compact overflow and focus bring-into-view contracts remain protected;
- adaptive-layout regression fixtures now match the current shell labels.

### Release identity

`Directory.Build.props` is the source of truth for current release identity. `tools/release_identity.py` parses and validates it without requiring the .NET SDK.

```text
ProductDisplayVersion = 2.9.7
Version = 2.9.7
VersionPrefix = 2.9.7
PackageVersion = 2.9.7
AssemblyVersion = 2.9.7.0
FileVersion = 2.9.7.0
InformationalVersion = 2.9.7
ReleaseTag = v2.9.7
MobileBuildCode = 20907
```

Protected mobile mappings include:

```text
2.9.0 -> 20900
2.9.5 -> 20905
2.9.6 -> 20906
2.9.7 -> 20907
```

### Cross-platform source hardening

The maintained source matrix is:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS: `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

Platform-support validation, platform workflow validation, and Source Preflight protect this matrix.

### Security and supply-chain controls

The repository keeps direct/transitive NuGet auditing enabled:

```xml
<NuGetAudit>true</NuGetAudit>
<NuGetAuditMode>all</NuGetAuditMode>
<NuGetAuditLevel>moderate</NuGetAuditLevel>
```

Warnings-as-errors keep moderate-or-higher NuGet audit warnings actionable when advisory data is available. CodeQL, Dependency Review, Dependabot, focused security-workflow validation, and dependency-policy validation remain enabled.

### Deterministic SBOM and provenance

- `tools/generate_sbom.py` emits deterministic CycloneDX 1.7 JSON from restored NuGet dependency metadata;
- supported assets-format and generator versions are recorded in BOM metadata;
- Desktop, Browser, and signed-Android publication workflows publish matching SBOMs when their artifacts exist;
- release filenames are checked for duplicate basenames and reserved checksum-name collisions;
- `SHA256SUMS.txt` uses published basenames and is usable with `sha256sum -c` after download;
- `actions/attest@v4` covers the prepared release tree;
- publication uses the protected permission `artifact-metadata: write` only where required;
- provenance subjects use the protected release asset tree `release-assets/**/*`.

### Release workflow safety

The release workflow validates strict tag syntax, checks the exact tag source, verifies the tag against `<Version>`, runs source preflight and dependency-security checks, restores/builds the requested targets, generates SBOM/checksum/provenance evidence, and publishes stable assets only after the earlier gates pass.

The Android publication job keeps the source-owned product display version and numeric build code rather than replacing them with GitHub run metadata.

## Product Identity

- Product name: CalcNova
- Public version: `2.9.7`
- Package version: `2.9.7`
- Release tag: `v2.9.7`
- Assembly/file version: `2.9.7.0`
- Mobile build code: `20907`
- Application id: `in.sanskar.calcnova`
- License: Apache-2.0
- Repository: `https://github.com/sanskarIN/CalcNova`

## Completed Feature Baseline

### Calculator and scientific engine

- safe tokenizer and recursive-descent parser;
- explicit precedence and right-associative exponentiation;
- parentheses, unary operators, decimal/scientific input;
- `BigInteger`, decimal arithmetic, bounded floating-point fallback;
- typed calculation errors and workload limits;
- scientific constants and trigonometric/inverse/hyperbolic functions;
- degrees, radians, gradians;
- percentage semantics separate from modulo;
- repeated equals and calculator memory operations;
- sanitized expression import and explicit clipboard actions;
- selection-aware editing and bounded keyboard input.

### Exact rational and engineering notation

- canonical `BigInteger` numerator/denominator fractions;
- exact integer/fraction/finite-decimal/scientific parsing;
- exact arithmetic, comparison, reciprocal, hashing, and cross-cancellation;
- bounded input/scale/exponent/reduced-bit-size contracts;
- engineering exponents in multiples of three;
- 1–15 significant digits;
- invariant parsing and exponent range `-324..306`;
- extreme finite-value scaling and underflow rejection;
- shared 4,096-character input bound.

### Programmer and Unicode tools

- base 2–36 parsing/formatting;
- binary/octal/decimal/hex synchronized views;
- 8/16/32/64/128-bit word sizes;
- signed/unsigned two's-complement interpretation;
- AND/OR/XOR/NOT and shift operations;
- accessible bit grids and fixed-width masking;
- Unicode scalar/code-point conversion;
- bounded Unicode text inspection;
- local Unicode category/plane/UTF-8/UTF-16 metadata;
- no network dependency for Unicode metadata.

### Converter, currency, and date/time

- offline conversion across length, area, volume, mass, speed, temperature, time, data, frequency, pressure, energy, power, force, and angle;
- validated conversion pairs, swapping, search, favorites, recents, precision, and result copy;
- persisted converter preferences;
- replaceable currency provider and local cache with offline fallback;
- date difference, calendar arithmetic, business-day helpers, and fixed-duration conversion.

### Statistics, equations, matrices, and graphing

- descriptive and paired statistics;
- population/sample covariance, Pearson correlation, OLS regression, `R²`, and prediction;
- bounded degenerate/non-finite handling and stale-model clearing;
- equation-solving and quadratic workflows;
- matrix determinant, inverse, rank, and linear-system solving;
- bounded graph sampling and discontinuity segmentation;
- pointer/keyboard pan and zoom;
- accessible eight-action graph toolbar;
- nearest-point trace and bounded CSV export;
- stable multi-series identities and textual legend;
- non-color-only line differentiation and accessible SVG export;
- bounded derivative, bisection root, and Simpson integration;
- explicit numerical workload budgets and extreme-finite-value safeguards.

### History, persistence, settings, and onboarding

- calculation-history abstraction and native SQLite history;
- Browser-safe storage;
- recent/search/favorite/delete/clear workflows;
- bounded TXT/CSV/JSON export and UTF-16-safe previews;
- settings repository and shared settings view model;
- versioned settings schema, legacy migration, and fail-closed future-schema handling;
- shared native/Browser JSON validation;
- versioned onboarding persistence, deferred first-run behavior, dismissal/skip actions, and focus restoration.

### Accessibility and localization

- 44-DIP minimum interaction-target baseline;
- 54-DIP calculator-key baseline;
- visible keyboard focus and stronger high-contrast focus;
- compact/medium/expanded adaptive profiles and compact overflow;
- focus bring-into-view and Ctrl+PageUp/PageDown/Home/End mode navigation;
- accessible programmer bit-state names;
- dynamic-control touch/focus validation;
- stable semantic string keys;
- complete English and Hindi catalogs for the current key set;
- regional `en-IN`/`hi-IN` culture selection and persistence;
- catalog completeness/duplicate/unknown-key validation;
- reviewed live localization including graph controls;
- evidence vocabulary using PASS / FAIL / BLOCKED / NOT RUN.

## Completed Validation and Release Infrastructure

SDK-independent validation covers repository/security contracts, XAML, navigation, keyboard input, adaptive layout, graph surface and numerical budgets, Unicode, exact rationals, engineering notation, exports, statistics, headless UI, accessibility, localization, converter/settings/onboarding contracts, packaging metadata, release identity, completion status, platform support, cross-platform composition, security automation, dependency security, release workflow, artifact integrity, release evidence, SBOM generation, checksums, and provenance.

The integrated gate is:

```bash
python tools/release_preflight.py --tag v2.9.7
```

Focused checks include:

```bash
python -m unittest tools.tests.test_release_identity
python -m unittest tools.tests.test_validate_adaptive_layout
python -m unittest tools.tests.test_generate_sbom
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
python tools/validate_platform_support.py .
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python tools/validate_release_workflow.py .
python tools/validate_release_docs.py .
```

## Environment Verification Record

Product implementation completeness and environment execution evidence are separate concepts.

The repository records PASS only when an operation actually executes and its result is observed. Hosted CI, compiled .NET validation, downloaded artifact execution, browser/device testing, Android/iOS signing, notarization, store processing, SBOM publication, checksum verification, and provenance verification must not be inferred from source contracts.

Unavailable external operations are recorded as `NOT RUN` or `BLOCKED`, never invented as PASS.

## Historical Release Checkpoints

- [`docs/releases/2.9.7.md`](docs/releases/2.9.7.md) — current 2.9.7 checkpoint
- [`docs/releases/2.9.6.md`](docs/releases/2.9.6.md) — preserved 2.9.6 checkpoint
- [`docs/releases/2.9.5.md`](docs/releases/2.9.5.md) — preserved 2.9.5 checkpoint
- [`docs/releases/2.9.0.md`](docs/releases/2.9.0.md) — preserved 2.9.0 checkpoint

## Final Classification

- Product scope for 2.9.7: **COMPLETE**
- Core features: **COMPLETE**
- Shared application features: **COMPLETE**
- Platform source composition: **COMPLETE**
- Cross-platform source validation: **COMPLETE**
- Release identity consistency: **COMPLETE**
- Security/dependency policy: **COMPLETE**
- SBOM/checksum/provenance source contracts: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release workflow infrastructure: **COMPLETE**
- Release evidence infrastructure: **COMPLETE**
- Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**
