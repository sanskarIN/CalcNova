# CalcNova Final Source Audit — 2026-08-19

## Purpose

This record captures the final source-level hardening review performed against the current `main` branch on 2026-08-19.

It deliberately separates three different claims:

1. **implemented in source** — code/tests/docs/workflow contracts exist;
2. **validated by SDK-independent source contracts** — Python validators can inspect deterministic repository contracts without compiling .NET;
3. **observed runtime/release evidence** — a command, build, test, platform run, accessibility audit, signing step, or package validation actually executed and its result was observed.

Only the first two layers can be completed from source review alone. A source audit must not convert an unexecuted .NET/platform check into PASS evidence.

## Concrete defects fixed during the final audit

### Exact-rational validator/source mismatch

The exact-rational source validator expected a magnitude-check marker that was not present in the implementation even though the actual bit-length guard existed in another form.

The validator was aligned with the real source contract so its repository-validity regression no longer fails because of a stale textual marker.

### Default `RationalNumber` invalid-state exposure

A C# value type can always be created with `default`. The original auto-property representation therefore allowed `default(RationalNumber)` to expose a zero denominator even though normal construction rejects denominator zero.

The type now stores backing fields and exposes a canonical denominator of `1` when the backing denominator is the all-zero default. As a result:

- `default(RationalNumber)` behaves as exact zero;
- equality/hash/string/comparison/arithmetic behavior remains canonical;
- consumers do not observe a synthetic `0/0` state simply because the struct was default-initialized.

A dedicated regression and source-contract guard protect this behavior.

### Exact-rational whitespace workload-budget bypass

The exact-rational documentation required the input-character budget to apply before trimming. The implementation instead checked the trimmed text length, allowing a very small valid number to be wrapped in arbitrarily large whitespace and bypass the intended raw-input budget.

The parser now checks `text.Length` before trimming. A dedicated regression covers oversized whitespace-padded input and the source validator requires the pre-trim guard.

### Engineering-notation exponent contract mismatch

The engineering-notation validator/documentation required explicit finite engineering exponent bounds, but the formatter/parser source and tests did not yet contain that complete contract.

The implementation now defines and enforces:

- minimum engineering exponent: `-324`;
- maximum engineering exponent: `306`.

Regression coverage includes out-of-range non-zero and zero-mantissa forms so an input such as a zero multiplied by an enormous power of ten cannot bypass the exponent workload contract.

### Integrated release-preflight inventory gaps

Several focused validators existed but were not represented in the single SDK-independent release preflight. The integrated inventory was expanded to include the current critical source contracts and their regression suites, including:

- exact rational arithmetic;
- engineering notation;
- artifact integrity infrastructure;
- structured release-evidence infrastructure;
- dynamic shared-control accessibility;
- exact-tag iOS simulator release workflow.

The preflight inventory regression was expanded at the same time so these validators cannot silently fall out of the integrated gate.

### Documentation drift

The final review found multiple documentation files lagging behind source completed earlier on 2026-08-19. The documentation set was synchronized so completed features are not still advertised as future ideas.

Updated areas include:

- documentation index;
- feature inventory;
- roadmap;
- SDK-independent preflight guide;
- exact-rational contract;
- this final source-audit record;
- continuation/change checkpoint.

## Completed source capabilities confirmed in the final review

The current source tree contains the following major capability groups.

### Calculation and numeric utilities

- safe project-owned expression parser/evaluator;
- standard/scientific calculator behavior;
- repeated equals, percentage, and calculator memory workflows;
- selection/caret-aware calculator editing and wrapping;
- safe printable/shifted operator mappings outside active text-editing fields;
- bounded exact rational arithmetic with shared Calculator utility UI;
- bounded engineering notation formatting/parsing with shared Calculator utility UI.

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
- graph sampling, viewport interaction, tracing, CSV/SVG export;
- derivative/root/integration approximation with explicit workload bounds;
- deterministic non-color-only multi-series line patterns and text legend;
- extreme-finite-value numerical-analysis hardening.

### Persistence and platform composition

- native SQLite history abstraction/implementation;
- Browser-safe storage paths;
- versioned settings schema and migration/validation architecture;
- Desktop, Browser/WebAssembly, Android, and iOS composition heads;
- shared clipboard abstraction;
- bounded export previews with full private copy payloads.

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

The documentation index now links the current major contracts, including:

- `EXACT_RATIONALS.md`;
- `ENGINEERING_NOTATION.md`;
- `BIVARIATE_STATISTICS.md`;
- `UNICODE_METADATA.md`;
- `GRAPH_NUMERICAL_SAFETY.md`;
- `EXPORT_PREVIEWS.md`;
- `SOURCE_PREFLIGHT.md`;
- `VALIDATION_EVIDENCE.md`.

`FEATURES.md` distinguishes completed source features from remaining runtime/product work.

`ROADMAP.md` no longer lists exact rational arithmetic, engineering notation, covariance/correlation/regression, printable calculator operator mappings, deterministic multi-series differentiation, or already-added numerical edge hardening as future work.

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

## Known source-level limitation that remains intentional

The semantic English/Hindi catalog is implemented, but the shared XAML still contains unmigrated hard-coded English. CalcNova therefore must not claim complete Hindi UI localization yet. Remaining visible strings should be migrated in compile-verified increments and validated with real Devanagari/large-text/compact layouts.

## Final continuation rule

Future work should start with **observed execution evidence**, not by recreating already completed source modules.

When a real compiler, test, platform, accessibility, packaging, or signing failure is observed:

1. record the exact failing command/environment;
2. fix the smallest concrete cause;
3. add or strengthen regression coverage where practical;
4. rerun the affected validation;
5. update structured/manual evidence without converting unavailable checks into PASS.

The source-level final audit is complete only in that limited sense: the known source/documentation inconsistencies found during this review were corrected, while runtime/platform release readiness remains evidence-dependent.
