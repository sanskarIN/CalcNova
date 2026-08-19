# What Changed

## Final source-audit checkpoint — 2026-08-19

This file is the live continuation checkpoint for CalcNova after the final source/documentation hardening pass.

Historical detail is preserved verbatim at:

- `docs/history/what_changed_through_full_source_hardening_2026-08-19.md` — earlier cumulative history;
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md` — the complete active continuation immediately before this final audit.

The detailed final review is documented at:

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`.

## Final audit scope

The final pass inspected the current `main` source/documentation/release-contract state rather than recreating already completed modules.

Work concentrated on:

- concrete correctness bugs discovered by comparing source, tests, validators, and documentation;
- missing entries in the integrated SDK-independent release preflight;
- stale feature/roadmap/public documentation;
- preserving conservative validation/evidence semantics;
- leaving a clean continuation boundary for the first real compiled/platform evidence pass.

## Concrete bugs fixed

### Exact rational validator mismatch

`tools/validate_rational_numbers.py` expected a stale magnitude-check text marker that did not match the current implementation. The validator was aligned with the real `GetBitLength()` guard.

This prevents the validator's own repository-validity test from failing because of a stale source marker.

### `default(RationalNumber)` canonicalization

`RationalNumber` is a C# value type, so callers can always create `default(RationalNumber)` without invoking its validating constructor.

The previous auto-property representation therefore allowed a default value to expose denominator zero even though normal construction rejects zero denominators.

The type now uses backing fields and exposes denominator `1` for the zero-initialized backing state. The default value therefore behaves as canonical exact zero rather than an invalid synthetic `0/0` value.

Added regression coverage verifies:

- equality with `RationalNumber.Zero`;
- numerator `0`;
- denominator `1`;
- integer classification;
- string representation `0`;
- arithmetic with `One`;
- exact comparison with zero.

The source validator now protects this default-value contract.

### Exact-rational raw input budget bypass

The exact-rational documentation required the character workload limit to be applied before trimming input, but the parser checked `trimmed.Length`.

That allowed a tiny valid number to be surrounded by arbitrarily large whitespace and evade the intended raw-input budget.

The parser now checks `text.Length` before trimming.

Added regression coverage rejects oversized whitespace-padded rational input, and the source validator now requires both the pre-trim guard and its regression test.

### Engineering-notation exponent contract mismatch

The engineering-notation validator/documentation required explicit finite engineering exponent bounds while the formatter/parser and tests had not fully implemented that contract.

The implementation now defines:

- `MinimumEngineeringExponent = -324`;
- `MaximumEngineeringExponent = 306`.

Exponent-form parsing rejects values outside that range with `OverflowException`.

Regression coverage includes both non-zero and zero mantissas outside the allowed range so inputs such as `0e+309` cannot bypass the exponent workload policy simply because their mathematical result would be zero.

## Integrated release-preflight hardening

`tools/release_preflight.py` was audited against focused validators added during the broader continuation.

The integrated source gate now explicitly includes the recent critical validator families and regression suites for:

- exact rational arithmetic;
- engineering notation;
- artifact manifest/integrity infrastructure;
- machine-readable release-evidence infrastructure;
- dynamic shared-control accessibility;
- exact-tag unsigned iOS simulator release workflow.

The preflight inventory regression was expanded at the same time so these checks cannot silently disappear from the unified source gate.

The current preflight remains a source-contract gate. It does not replace compiled/runtime/platform validation.

## Completed capabilities synchronized into documentation

The public documentation now reflects source capabilities already implemented on `main`, including:

### Calculator and numeric utilities

- selection/caret-aware keypad editing and selection-preserving wrapping;
- safe printable/shifted operator mappings outside active text-editing fields;
- bounded exact rational arithmetic;
- Calculator exact-rational panel/workflow;
- bounded engineering-notation formatting/parsing;
- Calculator engineering-notation panel/workflow.

### Programmer and Unicode

- base 2–36 programmer workflows;
- 8/16/32/64/128-bit fixed-width tools;
- local Unicode scalar metadata;
- shared Unicode metadata/result copy workflows.

### Statistics

- bounded paired X/Y parsing;
- population/sample covariance;
- Pearson correlation when defined;
- ordinary least-squares regression;
- coefficient of determination when defined;
- regression prediction;
- degenerate/non-finite/oversized input handling;
- shared paired-statistics panel and copy workflow.

### Graphing

- deterministic multi-series line-pattern differentiation that does not rely on color alone;
- synchronized multi-series text legend;
- combined-series fit-to-data behavior;
- derivative/root/integration numerical hardening;
- explicit graph workload-budget regressions;
- bounded export previews with complete private copy payloads.

### Accessibility/adaptive/localization

- dynamic graph-control focus/touch-target regression/source contracts;
- compact/medium/expanded adaptive source profiles;
- reviewed English/Hindi runtime mappings beyond the earlier shell/calculator/onboarding subset;
- explicit conservative runtime accessibility evidence vocabulary.

### Release/integrity/evidence tooling

- broad SDK-independent source validators and Python regressions;
- unified source preflight;
- artifact manifest generation/verification and SHA-256 integrity infrastructure;
- machine-readable validation evidence schema/model/runner/verifier;
- platform workflow source contracts;
- exact-tag unsigned iOS simulator release validation.

## Documentation completed/synchronized in the final pass

### Added

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md`

### Updated

- `README.md` — current public capability/evidence overview;
- `CHANGELOG.md` — current Unreleased feature/fix/security/validation inventory;
- `docs/README.md` — indexes exact rationals, engineering notation, bivariate statistics, validation evidence, and final audit;
- `docs/FEATURES.md` — removes already-completed work from future lists and documents current feature contracts;
- `docs/ROADMAP.md` — moves exact rational, engineering notation, covariance/correlation/regression, printable keyboard operators, graph presentation, and numerical hardening to completed work;
- `docs/SOURCE_PREFLIGHT.md` — documents the current integrated validator inventory and its limits;
- `docs/EXACT_RATIONALS.md` — documents default-value canonicalization and pre-trim input-budget enforcement;
- `what_changed.md` — this final checkpoint.

## Key final-audit commits

The final audit deliberately used multiple focused commits. Important commits include:

- `cf2597fc` — fix(core): align rational source validator with magnitude guard
- `59d75449` — fix(core): canonicalize default rational values
- `20e2240a` — test(core): cover default rational canonicalization
- `a810fc2f` — ci(core): guard default rational canonicalization
- `d332aab1` — ci(release): include exact rational validation in preflight
- `6b9e0097` — test(release): require exact rational preflight coverage
- engineering-notation implementation/test commits enforcing explicit finite exponent bounds
- release-preflight commits integrating engineering notation, artifact integrity, structured evidence, dynamic controls accessibility, and iOS release workflow validation
- `a5339118` — fix(core): enforce rational budget before trimming
- `9c8d532d` — test(core): cover padded rational input budget
- `cab5e6ef` — ci(core): guard pre-trim rational workload budget
- `e76a427c` — docs(core): document rational default safety contract
- `a1856f78` — docs(release): synchronize complete source preflight inventory
- `9a12dde8` — docs: index completed math and release-evidence guides
- `50321d45` — docs: synchronize implemented feature inventory
- `9c8d1391` — docs: move completed math and keyboard work out of roadmap
- `e70161a5` — docs(audit): record final source hardening review
- `22a43f21` — docs(audit): index final source audit
- `beb27a29` — docs: refresh public capability overview
- `5cbecf9c` — docs: finalize unreleased changelog
- `57474c57` — docs(history): archive pre-final continuation checkpoint

## Validation evidence status

The evidence policy remains conservative.

### NOT RUN locally in the final audit environment

The required .NET 10 SDK/toolchains are unavailable here, so the following are **NOT RUN** locally:

- `dotnet restore`;
- `dotnet format --verify-no-changes`;
- compiled solution builds;
- compiled unit/integration tests;
- Avalonia compiled-XAML/headless execution;
- target-platform application/package builds;
- signing/notarization/provisioning/store submission checks;
- real device/browser accessibility and adaptive-layout audits.

### Integrated source preflight

The final current repository tree was inspected and its source contracts/preflight inventory were hardened through GitHub source/commit access.

The complete `python tools/release_preflight.py` command was **not re-executed locally against a materialized final repository tree** in this environment, because the environment used for local execution could not materialize the repository from GitHub.

Therefore this file does **not** record the final integrated source preflight as PASS solely because its source exists.

### GitHub/CI evidence

GitHub status/workflow results must be observed for the exact current commit before being recorded as PASS. An absent/empty status list is not a successful CI result.

### Commit identity

The repository commit metadata inspected during this continuation uses:

`Sanskar <sanskarin@outlook.in>`

for author/committer identity on the created commits.

## Remaining work is evidence-dependent

No new core source module should be recreated simply to make the project appear more complete.

The remaining high-priority work is:

1. observe real .NET 10 restore/format/analyzer/build/test output and fix any concrete failures;
2. observe the Avalonia headless workflow and fix any compiled-XAML/headless failures;
3. validate Windows/Linux/macOS Desktop launch, clipboard, persistence, keyboard, scaling, and packaging;
4. validate Browser/WebAssembly publish/load/storage/clipboard/keyboard/accessibility behavior;
5. validate Android workload builds, emulator/device layouts, persistence, clipboard, TalkBack/large text, signing, AAB, and store behavior;
6. validate iOS simulator/device behavior, Dynamic Type/VoiceOver, persistence/clipboard, signing/provisioning/archive/TestFlight/App Store behavior;
7. execute and populate the runtime accessibility matrix with observed keyboard/screen-reader/contrast/large-text/touch/reduced-motion evidence;
8. validate compact/medium/expanded layouts on real target sizes, especially wide programmer grids and long export/statistics/calculator-extension surfaces;
9. migrate the remaining hard-coded English shared XAML to the semantic localization layer in compile-verified increments and validate Hindi/Devanagari layouts;
10. verify real produced release artifacts with the artifact manifest/checksum tooling;
11. generate and verify structured release evidence from the exact release-candidate commit;
12. run the final release gate with actual packaging/signing/store requirements for each claimed platform.

## Continuation rule

The next continuation must begin from observed evidence rather than duplicating completed source work:

1. read `PROJECT_STATE.md`;
2. read this file;
3. read `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`;
4. inspect the actual current `main` commit;
5. inspect CI/workflow results or run the required commands in a suitable environment;
6. fix only concrete failures or clearly evidenced remaining product gaps;
7. never convert source presence, workflow presence, or unavailable checks into PASS evidence.
