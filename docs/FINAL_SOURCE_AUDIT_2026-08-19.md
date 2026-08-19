# CalcNova Final Source Audit — 2026-08-19

## Purpose

This record captures the final source-level hardening review performed against the current `main` branch on 2026-08-19.

It deliberately separates three different claims:

1. **implemented in source** — code/tests/docs/workflow contracts exist;
2. **validated by SDK-independent source contracts** — Python validators can inspect deterministic repository contracts without compiling .NET;
3. **observed runtime/release evidence** — a command, build, test, platform run, accessibility audit, signing step, artifact verification, or package validation actually executed and its result was observed.

A source audit must never convert an unexecuted .NET/platform check into PASS evidence.

## Concrete defects fixed during the final audit

### Exact-rational validator/source mismatch

The exact-rational source validator expected a magnitude-check marker that did not match the actual implementation even though the reduced bit-length guard was present.

The validator was aligned with the real `GetBitLength()` contract so its repository-validity regression no longer fails because of stale textual matching.

### Default `RationalNumber` invalid-state exposure

A C# value type can always be created with `default`. The original auto-property representation therefore allowed `default(RationalNumber)` to expose a zero denominator even though normal construction rejects denominator zero.

The type now stores backing fields and exposes a canonical denominator of `1` for the all-zero default backing state. Therefore:

- `default(RationalNumber)` behaves as exact zero;
- numerator is `0`;
- denominator is `1`;
- equality/hash/string/comparison/arithmetic behavior remains canonical;
- consumers do not observe a synthetic `0/0` state merely because the struct was default-initialized.

Dedicated regression and source-contract guards protect this behavior.

### Exact-rational whitespace workload-budget bypass

The exact-rational contract required the raw input-character budget to apply before trimming. The implementation instead checked the trimmed text length, allowing a small valid number to be surrounded by arbitrarily large whitespace and evade the intended input budget.

The parser now checks `text.Length` before trimming. A regression covers oversized whitespace-padded input and the source validator requires the pre-trim guard.

### Engineering-notation finite exponent contract mismatch

The engineering-notation validator/documentation required explicit finite engineering exponent bounds, but formatter/parser source and tests did not yet fully implement that contract.

The implementation now defines and enforces:

- minimum engineering exponent: `-324`;
- maximum engineering exponent: `306`.

Regression coverage includes out-of-range non-zero and zero-mantissa forms so an input such as `0e+309` cannot bypass the exponent workload contract merely because the mathematical value would still be zero.

### Engineering non-zero underflow silently becoming zero

A final numeric edge review found another real issue after the explicit exponent bounds were added.

`1e-324` uses a syntactically valid engineering exponent, but the value is below the minimum positive subnormal `double`. Chunked power-of-ten scaling therefore produces floating-point `0`.

Returning `0` for a non-zero canonical engineering input would silently change its meaning.

The parser now rejects the case when:

- the parsed mantissa is non-zero; and
- bounded scaling produces `0`.

It throws `OverflowException` with a below-supported-non-zero-range message. The regression suite includes `Parse_RejectsUnderflowingNonZeroEngineeringValue`, and the SDK-independent validator requires both the source guard and the regression scenario.

Representable extreme forms such as the formatter output for `double.Epsilon` remain part of round-trip coverage.

### Integrated release-preflight inventory gaps

Several focused validators existed but were not represented in the single SDK-independent release preflight.

The integrated inventory was expanded to include the current critical source contracts and their regression suites, including:

- exact rational arithmetic;
- engineering notation;
- artifact integrity infrastructure;
- structured release-evidence infrastructure;
- dynamic shared-control accessibility;
- exact-tag unsigned iOS simulator release workflow.

The preflight inventory regression was expanded simultaneously so these validators cannot silently fall out of the integrated gate.

### Documentation drift

The final review found multiple documentation files lagging behind source completed earlier on 2026-08-19. Completed features were still described as future work in some places, and newer release/integrity tooling was missing from public indexes.

The documentation set was synchronized so source state, roadmap, features, changelog, public README, preflight guide, numeric guides, project state, final audit, and continuation checkpoint describe the same current scope.

## Completed source capabilities confirmed in the final review

### Calculation and numeric utilities

- safe project-owned expression parser/evaluator;
- standard/scientific calculator behavior;
- repeated equals, percentage, and calculator memory workflows;
- selection/caret-aware calculator editing and selection-preserving wrapping;
- safe printable/shifted operator mappings outside active text-editing fields;
- bounded exact rational arithmetic with shared Calculator utility UI;
- bounded engineering-notation formatting/parsing with explicit finite exponent and non-zero-underflow protection;
- focused tests/source validators/workflows for the recent numeric utilities.

### Programmer and Unicode

- base 2–36 conversion;
- fixed-width signed/unsigned programmer behavior;
- bitwise operations and 8/16/32/64/128-bit grids;
- Unicode scalar parsing/inspection;
- local Unicode plane/category/UTF-8/UTF-16 metadata;
- explicit copy workflows.

### Conversion and utilities

- offline fixed-unit conversion;
- recent/favorite conversion pairs and persisted precision;
- category-scoped search and restore/swap workflows;
- optional provider/cache architecture for currency conversion;
- date/time/business-day/duration utilities.

### Statistics and advanced mathematics

- descriptive statistics;
- bounded paired X/Y parsing;
- covariance;
- Pearson correlation where defined;
- linear regression and prediction;
- equations;
- matrices;
- deterministic behavior for degenerate/non-finite/oversized paired datasets.

### Graphing and numerical analysis

- graph sampling, viewport interaction, and tracing;
- CSV/SVG export workflows;
- derivative/root/integration approximation with explicit workload bounds;
- deterministic non-color-only multi-series line patterns and text legend;
- combined-series fit-to-data;
- extreme-finite-value numerical-analysis hardening;
- bounded display previews with complete private copy payloads.

### Persistence and platform composition

- native SQLite history abstraction/implementation;
- Browser-safe storage paths;
- versioned settings schema and migration/validation architecture;
- Desktop, Browser/WebAssembly, Android, and iOS composition heads;
- shared clipboard abstraction;
- settings/culture/converter preference persistence contracts.

### Product quality

- compact/medium/expanded adaptive source profiles;
- shared touch-target and focus-visibility contracts;
- high-contrast/reduced-motion source state;
- dynamic graph-control accessibility regression coverage;
- English/Hindi semantic localization foundation and reviewed live migrated surfaces;
- first-run onboarding source behavior;
- runtime accessibility evidence vocabulary that distinguishes PASS, FAIL, BLOCKED, and NOT RUN.

### Release and integrity tooling

- repository/XAML/UI/navigation/keyboard/numerical/accessibility/localization/settings/platform/release source validators;
- focused Python regression suites;
- integrated `tools/release_preflight.py` source gate;
- artifact manifest generation/verification and SHA-256 integrity infrastructure;
- machine-readable release-evidence schema/model/runner/verifier;
- platform/release workflows including tag-first release logic and exact-tag unsigned iOS simulator validation.

## Documentation status

The main documentation index now links the current major contracts, including:

- `EXACT_RATIONALS.md`;
- `ENGINEERING_NOTATION.md`;
- `BIVARIATE_STATISTICS.md`;
- `UNICODE_METADATA.md`;
- `GRAPH_NUMERICAL_SAFETY.md`;
- `EXPORT_PREVIEWS.md`;
- `SOURCE_PREFLIGHT.md`;
- `VALIDATION_EVIDENCE.md`;
- this final source audit.

`FEATURES.md` distinguishes completed source features from remaining runtime/product work.

`ROADMAP.md` no longer lists exact rational arithmetic, engineering notation, covariance/correlation/regression, printable calculator operator mappings, deterministic multi-series differentiation, or already-added numerical edge hardening as future work.

`README.md`, `CHANGELOG.md`, `PROJECT_STATE.md`, and `what_changed.md` have been synchronized with the same source/evidence boundary.

The previous active continuation was preserved verbatim at:

`docs/history/what_changed_through_pre_final_audit_2026-08-19.md`.

## Source-validation entry points

The principal SDK-independent gate is:

```bash
python tools/release_preflight.py
```

Optional tag syntax/source validation can be included with:

```bash
python tools/release_preflight.py --tag v0.1.0
```

Structured source evidence can be collected with:

```bash
python tools/run_release_evidence.py --scope source
```

and evaluated through the repository evidence verifier according to `VALIDATION_EVIDENCE.md`.

## Observed GitHub status at the final audit boundary

The exact `main` commit was inspected through the GitHub connector during the final review.

A combined-status lookup returned no status contexts for the checked latest commit. A separate commit-workflow lookup available through the connector returned no pull-request-triggered workflow runs for that commit.

These empty results are **not** treated as CI success.

The connector available in this environment did not provide a general push-run listing for the branch through the allowed endpoint, so no unobserved push workflow is inferred either way.

## Runtime/release evidence still required

The final source review does **not** claim PASS for checks that were not observed. Before a release candidate can be called fully validated, real execution evidence is still required for at least:

- .NET 10 restore;
- formatting/analyzer validation;
- compiled solution/platform-head builds;
- full compiled unit/integration test suite;
- Avalonia compiled-XAML/headless tests;
- Desktop launch/persistence/clipboard/keyboard/scaling/package behavior on Windows, Linux, and macOS;
- Browser/WebAssembly publish/load/storage/clipboard/keyboard/accessibility behavior;
- Android workload build, emulator/device behavior, TalkBack/large text, signing, AAB, and store checks;
- iOS workload/simulator/device behavior, VoiceOver/Dynamic Type, signing/provisioning/archive/TestFlight/App Store checks;
- measured contrast, keyboard traversal, screen readers, large text, touch targets, reduced motion, and compact/orientation runtime accessibility audits;
- real artifact generation followed by manifest/checksum verification;
- release-candidate structured evidence generated from the exact commit being released.

## Local execution limitation

The active assistant execution environment does not provide the required .NET 10 SDK.

The complete final repository also could not be materialized locally from GitHub in the independent container path used during the continuation because that environment could not resolve `github.com`.

Accordingly:

- local .NET restore/build/test/headless execution is **NOT RUN**;
- the final integrated Python preflight is **NOT RUN locally against a materialized final tree**;
- source/workflow/test presence is not promoted to PASS evidence.

## Known source-level limitation that remains intentional

The semantic English/Hindi catalog and reviewed live mappings are implemented, but shared XAML still contains unmigrated hard-coded English.

CalcNova therefore must not claim complete Hindi UI localization yet. Remaining visible strings should be migrated in compile-verified increments and validated with real Devanagari/large-text/compact layouts.

## Final continuation rule

Future work should start with **observed execution evidence**, not by recreating already completed source modules.

When a real compiler, test, platform, accessibility, packaging, artifact, or signing failure is observed:

1. record the exact failing command/environment/commit;
2. fix the smallest concrete cause;
3. add or strengthen regression coverage where practical;
4. rerun the affected validation;
5. update structured/manual evidence without converting unavailable checks into PASS.

The source-level final audit is complete only in that limited sense: the known source/documentation inconsistencies and concrete numeric/release-gate defects found during this review were corrected, while runtime/platform release readiness remains evidence-dependent.
