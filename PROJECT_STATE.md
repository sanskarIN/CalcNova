# CalcNova Project State

## Current Version

**2.8.03**

Normalized .NET/NuGet version: `2.8.3`  
Normalized release tag: `v2.8.3`  
Mobile numeric build code: `20803`

See [`docs/VERSIONING.md`](docs/VERSIONING.md).

## Current Branch

`main`

## Completion Status

**COMPLETE — CalcNova version 2.8.03**

The defined 2.8.03 product scope is implemented in the repository. Core calculation, scientific functions, exact rational arithmetic, engineering notation, programmer and Unicode tools, converter/date-time/currency utilities, descriptive and bivariate statistics, equations, matrices, graphing/numerical analysis, history, persistence, settings, onboarding, localization infrastructure and reviewed localized surfaces, accessibility/adaptive contracts, Desktop/Browser/Android/iOS composition, source validation, dependency vulnerability policy, artifact integrity, structured release evidence, packaging metadata, release workflows, security automation, release provenance controls, and deterministic release SBOM generation are present as completed source capabilities.

Future repository changes are classified as maintenance, compatibility updates, security fixes, documentation changes, translation additions, or optional enhancements. They are not required to define the 2.8.03 project as complete.

## Current Maintenance Enhancements — 2026-08-21

The completed 2.8.03 baseline now includes stronger cross-platform release and supply-chain controls.

### Deterministic CycloneDX release SBOMs

- `tools/generate_sbom.py` generates deterministic CycloneDX 1.7 JSON from restored NuGet dependency metadata;
- the emitted document declares `https://cyclonedx.org/schema/bom-1.7.schema.json`;
- package components include NuGet package names/versions, Package URLs, available NuGet SHA-512 package hashes, and resolved dependency edges;
- the BOM serial is deterministic rather than time-dependent;
- no wall-clock timestamp is emitted, so identical restore metadata and release identity produce stable JSON;
- the generator fails closed unless `project.assets.json` uses the explicitly supported top-level format version `3` and provides the expected `libraries`, `targets`, and `project` objects;
- the supported assets-format version and generator version are recorded in BOM metadata properties;
- all six Desktop release targets publish a matching `CalcNova-<rid>.sbom.cdx.json` next to their ZIP archive;
- Browser publishes `CalcNova-browser.sbom.cdx.json` next to its ZIP archive;
- signed Android publication emits `CalcNova-android.sbom.cdx.json` next to the AAB when signing configuration is available;
- release asset basename-collision validation applies to SBOMs as well as packages;
- SBOM files are included in `SHA256SUMS.txt` and in the existing `actions/attest@v4` `release-assets/**/*` provenance subject set;
- `tools/tests/test_generate_sbom.py` protects determinism, CycloneDX schema identity, package/hash/dependency output, format-version fail-closed behavior, generator metadata, and stable JSON serialization;
- `tools/validate_release_workflow.py` protects Desktop/Browser/Android SBOM generation and ordering before artifact upload;
- generator regression tests are integrated into `tools/release_preflight.py`, and the preflight inventory test requires that integration;
- `docs/ARTIFACT_PROVENANCE.md` documents SBOM generation, verification, checksum/provenance coverage, evidence semantics, and NuGet format-drift handling.

### Native desktop release coverage

- Windows self-contained archives for `win-x64` and `win-arm64`;
- Linux self-contained archives for `linux-x64` and `linux-arm64`;
- macOS self-contained archives for `osx-x64` and `osx-arm64`;
- RID-specific archive and artifact names;
- release-workflow source validation requiring all six target/runner pairs;
- regression tests that lock x64 + ARM64 coverage for every desktop operating system;
- build/platform/release documentation synchronized with the six-target release matrix.

### Automated security maintenance

- repository-level NuGet Audit explicitly enabled for direct and transitive packages with `NuGetAudit=true`, `NuGetAuditMode=all`, and `NuGetAuditLevel=moderate`;
- warnings-as-errors retained so moderate-or-higher NuGet audit warnings fail restore/build gates when observed;
- `tools/validate_dependency_security.py` protects the NuGet audit policy against disablement, direct-only drift, threshold weakening, duplicate definitions, and protected NU190x suppression markers;
- regression tests protect the dependency-security validator, including composite `NoWarn` / `WarningsNotAsErrors` lists;
- C# CodeQL scanning on pushes and pull requests to `main`, weekly schedule, and manual dispatch;
- pull-request dependency review with `moderate` vulnerability severity enforcement;
- existing Dependabot coverage for NuGet and GitHub Actions retained;
- `tools/validate_security_workflows.py` protects action majors, triggers, language/build mode, severity threshold, permissions, unsafe-trigger drift, and the focused security workflow itself;
- focused `Security Automation Validate` workflow watches `Directory.Build.props` and runs both security validators plus their regression suites with read-only repository permission;
- security workflow and dependency-policy validation are integrated into the SDK-independent release preflight.

### Release provenance, checksums, and least privilege

- release workflow defaults to `contents: read`;
- only the publication job receives `contents: write`, `id-token: write`, `attestations: write`, and `artifact-metadata: write`;
- publication validates that prepared release assets exist, have unique basenames, and do not preempt the reserved `SHA256SUMS.txt` name;
- `SHA256SUMS.txt` records the flat published basenames users download rather than runner-local `release-assets/<artifact>/...` paths;
- the flat checksum manifest can be used with `sha256sum -c SHA256SUMS.txt` after release files are downloaded into one directory;
- `actions/attest@v4` generates provenance attestations for the prepared `release-assets/**/*` tree, covering desktop/Browser ZIP archives and SBOMs, the Android AAB/SBOM when present, and `SHA256SUMS.txt`;
- the checksum manifest is excluded from its own hash set and then included in the attested release tree;
- the inclusive release-tree subject keeps optional Android output conditional without requiring a separate potentially absent AAB path;
- filename validation occurs before checksum generation, which occurs before provenance generation and GitHub Release asset upload;
- release workflow validator requires the permission/filename/checksum/SBOM/attestation/order/subject contract and rejects deprecated provenance wrappers plus the old nested-path checksum implementation;
- release-workflow regression tests lock the provenance action, flat checksum behavior, inclusive subject set, SBOM publication, filename guards, and permission counts;
- release-document validation protects the security automation, provenance/SBOM guide, current state, and live handoff documentation contracts;
- `docs/SECURITY_AUTOMATION.md` and `docs/ARTIFACT_PROVENANCE.md` document operation, evidence semantics, and verification guidance.

These are post-completion maintenance improvements. They do not change the public product version, normalized package version, release tag mapping, or mobile build code.

## Product Identity

- Product name: CalcNova
- Public version: `2.8.03`
- SemVer/package equivalent: `2.8.3`
- Release tag equivalent: `v2.8.3`
- Android/iOS display version: `2.8.03`
- Android/iOS numeric build code: `20803`
- Assembly version: `2.8.3.0`
- File version: `2.8.3.0`
- Informational version: `2.8.03`
- Application id: `in.sanskar.calcnova`
- License: Apache-2.0
- Repository: `https://github.com/sanskarIN/CalcNova`

The public `2.8.03` format is intentionally preserved. Strict SemVer tooling uses `2.8.3` because numeric SemVer components cannot contain leading zeroes.

## Technical Foundation

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Project-owned parser/evaluator rather than arbitrary code execution
- Pure calculation/domain libraries separated from Avalonia where practical
- Thin Desktop, Browser/WebAssembly, Android, and iOS composition heads
- Native SQLite history behind abstractions
- Browser-safe history/settings storage
- Versioned local settings schema with explicit migration behavior
- Optional network-enhanced currency conversion with no embedded provider credentials
- Local-first ordinary calculation and metadata behavior
- Centralized package management
- Nullable reference types, analyzers, warnings-as-errors, deterministic build settings, and explicit moderate-or-higher direct/transitive NuGet vulnerability auditing
- Automated dependency review and CodeQL source scanning
- Deterministic CycloneDX 1.7 release SBOM generation with fail-closed NuGet assets-format compatibility checks
- Download-friendly SHA-256 release manifests with filename collision protection
- Provenance-attested stable release artifact publication

## Completed Calculator Capabilities

### Standard and scientific calculation

- Typed calculation errors and workload limits
- Mixed numeric representation using `BigInteger`, `decimal`, and bounded floating-point fallback
- Safe tokenizer and recursive-descent parser
- Standard arithmetic, parentheses, unary operators, and right-associative exponentiation
- Decimal/scientific input
- Scientific constants/functions
- Degree/radian/gradian angle modes
- Calculator percentage semantics separate from expression modulo
- Repeated-equals session behavior
- MC, MR, MS, M+, M- memory operations
- Sanitized external expression import
- User-triggered clipboard paste and explicit result copy
- Top-row/numpad input
- Printable/shifted operator mappings outside active text editing
- Selection-aware editing, caret restoration, Backspace behavior, and selection-preserving wrapping

### Exact rational arithmetic

- Canonical `BigInteger` numerator/denominator representation
- Positive denominator and GCD normalization
- Safe default-value/canonical-zero semantics
- Exact integer, fraction, finite-decimal, and decimal-scientific parsing
- Exact arithmetic, reciprocal, comparison, equality, and hashing
- Cross-cancellation
- 4,096-character raw input bound before trimming
- 10,000 decimal exponent/scale magnitude bound
- 65,536-bit reduced numerator/denominator bound
- Shared Calculator panel and application workflows
- Core/application/headless regression source
- Focused validator/workflow and integrated preflight coverage

### Engineering notation

- Engineering exponents divisible by three
- 1–15 significant digits
- Canonical invariant-culture parsing
- 4,096-character shared core/App/UI input bound
- Explicit exponent range `-324..306`
- Non-zero-underflow rejection
- Extreme finite-value chunked scaling
- Shared Calculator panel
- Core/application/headless regression source
- Focused validator/workflow and integrated preflight coverage

## Completed Programmer and Unicode Capabilities

- Base 2–36 parse/format
- Binary/octal/decimal/hex synchronized representations
- 8/16/32/64/128-bit word sizes
- Signed/unsigned two's-complement interpretation
- Fixed-width masking
- AND/OR/XOR/NOT
- Left/logical-right/arithmetic-right shifts
- Full interactive bit grids
- Byte grouping
- Accessible bit-cell state labels
- Copy actions for radix/fixed-width representations
- Unicode scalar/code-point conversion
- Bounded text inspection
- Local Unicode plane/general-category/UTF-8/UTF-16 metadata
- Shared Unicode metadata presentation and copy actions
- No network dependency for Unicode metadata

## Completed Conversion and Utility Capabilities

- Offline unit conversion across major physical/data categories
- Unit swapping
- Validated conversion-pair model
- Bounded recent pairs
- Favorites
- Versioned persistence tokens
- Persisted recent/favorite state
- 1–17 significant-digit precision
- Search and result assignment
- Clear-recents and result copy
- Converter default/preference/privacy source contracts
- Replaceable currency provider/cache architecture
- Offline currency fallback semantics
- Date differences
- Calendar arithmetic
- Business-day utilities
- Fixed-duration conversion

## Completed Statistics, Equations, Matrices, and Graphing

### Statistics

- Descriptive statistics
- Bounded dataset parser
- Population/sample covariance
- Pearson correlation when defined
- Ordinary least-squares regression
- `R²` when defined
- Regression prediction
- Deterministic degenerate/non-finite/oversized handling
- Stale-model clearing
- Shared paired-statistics panel and copy workflow

### Equations and matrices

- Equation-solving module and shared view model
- Quadratic workflows
- Matrix determinant
- Matrix inverse
- Matrix rank
- Linear-system solving
- Matrix result copy

### Graphing and numerical analysis

- Bounded function sampling
- Discontinuity segmentation
- Explicit viewport model
- Pointer and keyboard interaction
- Reset/fit controls
- Nearest-point trace
- Bounded CSV generation
- Multi-expression sampling
- Stable series identities
- Deterministic non-color-only line patterns
- Multi-series text legend
- Accessible SVG export
- Bounded derivative approximation
- Bracketed bisection root finding
- Bounded Simpson integration
- Extreme-finite-value safeguards
- Explicit graph numerical workload budgets

## Completed History, Export, Settings, and Persistence

- Calculation-history abstraction
- SQLite native history
- Browser-safe storage path
- Recent/search/favorite/delete/clear workflows
- Bounded TXT/CSV/JSON export
- Bounded display previews with complete private copy payloads
- UTF-16-safe preview boundaries and newline normalization
- Settings repository abstraction
- Shared settings view model
- Persisted converter/culture preferences
- Explicit settings schema
- Legacy/unversioned migration
- Fail-closed unsupported future-schema handling
- Shared native/Browser JSON decoding and validation

## Completed Accessibility, Adaptive UI, and Onboarding Baseline

- 44-DIP minimum interaction-target baseline
- 54-DIP calculator key baseline
- Compact/medium/expanded layout profiles
- Compact overflow fallback
- Focus bring-into-view
- Explicit focus styling
- Stronger CalcNova high-contrast focus styling
- High-contrast and reduced-motion shell state
- Ctrl+PageUp/PageDown/Home/End mode navigation
- Accessible programmer bit-state names
- Dynamic graph control focus/touch-target contracts
- Onboarding shortcut suppression and focus restoration
- Runtime evidence matrix with PASS / FAIL / BLOCKED / NOT RUN vocabulary

## Completed Localization Baseline

- Stable semantic string-key catalog
- Complete English semantic catalog for the current key set
- Complete Hindi semantic catalog for the current key set
- Regional English/Hindi culture selection
- Persisted culture preference
- Catalog completeness/duplicate/unknown-key validation
- Runtime localization for reviewed shell, calculator, onboarding, settings, history, currency, About, and related surfaces

Additional language packs or further localization expansion are optional post-release contributions, not completion requirements for 2.8.03.

## Completed Platform Composition

- Shared application composition root
- Desktop composition
- Browser/WebAssembly composition
- Android composition
- iOS composition
- Shared clipboard abstraction and Avalonia adapter
- External-link abstraction
- Settings/history composition appropriate to native and Browser environments
- x64 + ARM64 self-contained desktop release matrix for Windows, Linux, and macOS

Android and iOS source metadata uses display version `2.8.03` and numeric build code `20803`.

Desktop release source packages `win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, and `osx-arm64`. Runtime/package evidence for those architecture artifacts remains separately recorded.

## Completed Validation, Security, and Release Infrastructure

SDK-independent source contracts cover:

- repository/security checks;
- XAML well-formedness;
- shared UI/navigation/keyboard contracts;
- calculator selection editing;
- graph keyboard/surface/presentation/numerical budgets;
- Unicode metadata;
- exact rationals;
- engineering notation;
- export previews;
- bivariate statistics;
- headless UI source contracts;
- accessibility markup/focus/dynamic controls/evidence/adaptive layout/touch targets;
- localization;
- converter preference/default behavior;
- settings schema;
- onboarding;
- packaging metadata;
- Desktop/Browser/Android/iOS workflow contracts;
- security automation workflow contracts and focused-workflow self-validation;
- NuGet dependency-security policy contracts;
- Source Preflight workflow self-validation;
- exact-tag iOS simulator workflow;
- release workflow and documentation contracts;
- deterministic CycloneDX 1.7 SBOM generation and NuGet assets-format compatibility tests;
- Desktop/Browser/signed-Android SBOM publication ordering validation;
- six-target x64/ARM64 desktop release matrix validation;
- release filename collision/checksum usability validation;
- release least-privilege/provenance attestation validation;
- release-tag syntax;
- artifact manifest/checksum integrity;
- structured release-evidence model/runner/verifier;
- Python regression suites for source validators;
- integrated SDK-independent source preflight.

Maintained security controls include:

- explicit direct/transitive NuGet Audit with moderate-or-higher enforcement through warnings-as-errors;
- CodeQL Action v4 C# source scanning;
- Dependency Review Action v5 with moderate-or-higher enforcement;
- Dependabot NuGet/GitHub Actions updates;
- focused security automation and dependency-policy source validation.

Stable release publication includes:

- per-platform deterministic CycloneDX 1.7 SBOMs beside package assets;
- release filename uniqueness/reserved-name validation;
- SHA-256 checksum generation using flat published basenames, including SBOM files;
- provenance attestations using `actions/attest@v4` for the prepared release-assets tree, including SBOM files;
- job-scoped release `contents: write`, OIDC, attestation, and artifact-metadata permissions.

## Release-Version Safety

`Directory.Build.props` is the release-version and dependency-audit policy source of truth.

The release workflow:

1. validates strict SemVer tag syntax;
2. checks out the exact requested tag;
3. reads the normalized `<Version>` from `Directory.Build.props`;
4. verifies the tag equals `v` plus that normalized source version;
5. runs tagged source preflight, including dependency-security/SBOM-generator regression coverage;
6. restores the .NET solution, which executes the configured NuGet direct/transitive vulnerability audit when advisory sources are available;
7. proceeds to .NET validation and platform publication only after those checks;
8. publishes each target package and generates its matching CycloneDX 1.7 SBOM from the restored dependency graph;
9. validates flat release filename uniqueness and the reserved checksum name;
10. generates a download-friendly basename checksum manifest covering package and SBOM assets;
11. attests the prepared release tree;
12. publishes stable GitHub Release assets.

The Android publication job does not replace source-owned display/build versions with the tag text or GitHub run number.

## UI Automation Source Coverage

Focused Avalonia headless test source covers the shared shell and key product scenarios, including:

- primary mode inventory;
- Calculator commands and selection editing;
- compact layout class;
- keyboard mode navigation;
- high-contrast state;
- onboarding visibility and dismissal;
- graph keyboard viewport operations;
- multi-series presentation/legend integration;
- Unicode metadata panel;
- exact-rational panel;
- engineering-notation panel and input bound;
- paired-statistics panel;
- dynamic graph-control focus/touch-target behavior.

## Environment Verification Record

Product implementation completeness and environment execution evidence are separate concepts.

The repository records a check as PASS only when it actually executes and its result is observed. In the assistant environment used for the source pass, the required .NET 10/platform toolchains were not available for direct execution, so local compiled/platform/NuGet-audit execution evidence remains recorded conservatively as `NOT RUN` rather than being invented.

A fresh-clone attempt also could not resolve `github.com`, so the updated repository could not be materialized in that container for a local full-tree preflight. The current maintenance work was therefore validated through repository source contracts, regression-source updates, GitHub repository reads/writes, documentation consistency checks, and a focused local Python SBOM smoke execution built from the committed generator logic.

The focused SBOM smoke execution observed `PASS` for deterministic output, CycloneDX 1.7 schema identity, package inventory, direct/transitive dependency edges, SHA-512 conversion, and rejection of an unsupported assets-format version. This is focused generator evidence only; it is not a substitute for the repository's full Source Preflight workflow or a real release run.

The commit-status endpoint exposed no legacy commit statuses for the checked maintenance head. That is not treated as proof that GitHub Actions checks passed or failed; no Actions service PASS was inferred from it.

Compiled/runtime/GitHub-hosted security-service/NuGet advisory-query/release-SBOM publication evidence remains environment/service dependent. A real `.sbom.cdx.json` release asset is `PASS` evidence only after the release workflow actually creates and publishes it.

This evidence notation does **not** mean CalcNova 2.8.03 is incomplete. It means a particular command or service check was not observed in that environment/tool surface.

Typical environment-specific verification commands include:

```bash
python tools/release_preflight.py
python -m unittest tools.tests.test_generate_sbom
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python tools/validate_release_workflow.py .
python tools/validate_release_docs.py .
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Platform signing, notarization, provisioning, GitHub-hosted security scanning, online NuGet advisory lookup, checksum verification against published downloads, release SBOM generation from actual restored target assets, artifact attestation execution, and store processing additionally require their respective external environments/services/credentials.

## Final Classification

- Product scope for 2.8.03: **COMPLETE**
- Core features: **COMPLETE**
- Shared application features: **COMPLETE**
- Platform source composition: **COMPLETE**
- x64/ARM64 desktop release source contract: **COMPLETE**
- Security automation source contract: **COMPLETE**
- NuGet dependency-security policy source contract: **COMPLETE**
- Deterministic release SBOM source contract: **COMPLETE**
- Release checksum/integrity source contract: **COMPLETE**
- Release provenance/least-privilege source contract: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release workflow infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**
- Future repository changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

For details, see:

- [`README.md`](README.md)
- [`CHANGELOG.md`](CHANGELOG.md)
- [`docs/VERSIONING.md`](docs/VERSIONING.md)
- [`docs/FEATURES.md`](docs/FEATURES.md)
- [`docs/ROADMAP.md`](docs/ROADMAP.md)
- [`docs/SECURITY_AUTOMATION.md`](docs/SECURITY_AUTOMATION.md)
- [`docs/ARTIFACT_PROVENANCE.md`](docs/ARTIFACT_PROVENANCE.md)
- [`docs/FINAL_SOURCE_AUDIT_2026-08-19.md`](docs/FINAL_SOURCE_AUDIT_2026-08-19.md)
- [`what_changed.md`](what_changed.md)
