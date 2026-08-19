# What Changed

## Final source-audit checkpoint — 2026-08-19

This is the live continuation checkpoint for CalcNova after the final source/documentation hardening pass.

Historical detail is preserved verbatim at:

- `docs/history/what_changed_through_full_source_hardening_2026-08-19.md` — earlier cumulative history;
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md` — the complete active continuation immediately before the final audit.

The detailed final review is:

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`.

## Final audit scope

The final pass inspected the actual current `main` source, tests, validators, release tooling, and documentation rather than recreating already completed modules.

Work concentrated on:

- concrete correctness defects found by comparing implementation, tests, validator contracts, and documentation;
- source validators that existed but were missing from the integrated release preflight;
- stale public feature/roadmap/project-state documentation;
- preserving conservative PASS / FAIL / BLOCKED / NOT RUN evidence semantics;
- creating a clean handoff from source hardening to real compiled/platform validation.

## Concrete defects fixed

### Exact-rational validator/source mismatch

`tools/validate_rational_numbers.py` expected a stale magnitude-check source marker even though the implementation contained the intended reduced bit-length guard in another form.

The validator now matches the real `GetBitLength()` contract.

### `default(RationalNumber)` canonicalization

Because `RationalNumber` is a C# value type, `default(RationalNumber)` can exist without invoking the validating constructor.

The previous auto-property representation could therefore expose denominator zero even though normal construction rejects zero denominators.

The type now uses backing fields and treats the zero-initialized denominator backing field as canonical denominator `1`.

`default(RationalNumber)` therefore behaves as exact zero instead of exposing synthetic `0/0` state.

Regression coverage verifies:

- equality with `RationalNumber.Zero`;
- numerator `0`;
- denominator `1`;
- integer classification;
- string representation `0`;
- arithmetic with `One`;
- exact comparison with zero.

The source validator protects this contract.

### Exact-rational raw input budget bypass

The rational contract required the raw input-character budget to be enforced before trimming. The implementation checked the trimmed text length instead.

That allowed a tiny valid number wrapped in arbitrarily large whitespace to bypass the intended input budget.

The parser now checks `text.Length` before trimming.

A dedicated regression rejects oversized whitespace-padded input, and the validator requires both the pre-trim guard and regression scenario.

### Engineering finite exponent contract

Engineering-notation documentation/validation required explicit finite engineering exponent boundaries, but formatter/parser source and tests did not yet fully enforce them.

The implementation now defines and enforces:

- minimum engineering exponent: `-324`;
- maximum engineering exponent: `306`.

Out-of-range zero and non-zero exponent forms are rejected so a value such as `0e+309` cannot bypass the workload contract simply because its mathematical result would remain zero.

### Engineering non-zero underflow becoming zero

A final edge review found another real numeric correctness issue after exponent bounds were added.

`1e-324` is syntactically valid engineering notation, but it lies below the minimum positive subnormal `double`. Bounded power-of-ten scaling therefore produces floating-point zero.

Silently returning `0` for a non-zero input would change its mathematical meaning.

The parser now rejects the case when:

- the mantissa is non-zero; and
- bounded scaling produces `0`.

It throws `OverflowException` instead of silently returning zero.

Added regression:

- `Parse_RejectsUnderflowingNonZeroEngineeringValue`.

The engineering source validator now requires the underflow guard and regression scenario. `docs/ENGINEERING_NOTATION.md` documents the distinction between the -324 engineering step boundary and the actual minimum representable non-zero `double`.

## Integrated release-preflight hardening

The integrated source gate was audited against focused validators added during the wider continuation.

`tools/release_preflight.py` now includes the recent critical validator families and their regression suites for:

- exact rational arithmetic;
- engineering notation;
- artifact manifest/integrity infrastructure;
- machine-readable release-evidence infrastructure;
- dynamic shared-control accessibility;
- exact-tag unsigned iOS simulator release workflow.

`tools/tests/test_release_preflight.py` was expanded so these validators/test modules cannot silently disappear from the unified preflight inventory.

The preflight remains a source-contract gate; it does not replace compiled/runtime/platform evidence.

## Completed source capabilities synchronized into documentation

### Calculator and numeric utilities

- selection/caret-aware keypad editing;
- forward/reversed selection replacement;
- selection-aware Backspace;
- selection-preserving function/parenthesis wrapping;
- safe printable/shifted operator mappings outside active text fields;
- bounded exact rational arithmetic and Calculator utility panel;
- bounded engineering notation and Calculator utility panel;
- engineering exponent and non-zero-underflow protection.

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
- deterministic degenerate/non-finite/oversized input handling;
- shared paired-statistics panel and copy workflow.

### Graphing

- deterministic multi-series line-pattern differentiation that does not rely on color alone;
- synchronized multi-series text legend;
- combined-series fit-to-data behavior;
- derivative/root/integration extreme-value hardening;
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

## Documentation completed/synchronized

### Added

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md`

### Updated

- `README.md` — current public capability/evidence overview;
- `CHANGELOG.md` — current Unreleased feature/fix/security/validation inventory;
- `PROJECT_STATE.md` — authoritative current source/evidence status;
- `docs/README.md` — indexes exact rationals, engineering notation, bivariate statistics, validation evidence, and final audit;
- `docs/FEATURES.md` — removes already-completed work from future lists and documents current feature contracts;
- `docs/ROADMAP.md` — moves completed numeric/statistics/keyboard/graph hardening to completed work;
- `docs/SOURCE_PREFLIGHT.md` — documents the current integrated source validator inventory and limits;
- `docs/EXACT_RATIONALS.md` — documents default-value canonicalization and pre-trim input-budget enforcement;
- `docs/ENGINEERING_NOTATION.md` — documents explicit finite exponent bounds and non-zero-underflow rejection;
- `what_changed.md` — this final checkpoint.

## Key final-audit commits

Important commits from this last pass include:

- `cf2597fc` — fix(core): align rational source validator with magnitude guard
- `59d75449` — fix(core): canonicalize default rational values
- `20e2240a` — test(core): cover default rational canonicalization
- `a810fc2f` — ci(core): guard default rational canonicalization
- `d332aab1` — ci(release): include exact rational validation in preflight
- `6b9e0097` — test(release): require exact rational preflight coverage
- `a5339118` — fix(core): enforce rational budget before trimming
- `9c8d532d` — test(core): cover padded rational input budget
- `cab5e6ef` — ci(core): guard pre-trim rational workload budget
- `e76a427c` — docs(core): document rational default safety contract
- `a1856f78` — docs(release): synchronize complete source preflight inventory
- `50321d45` — docs: synchronize implemented feature inventory
- `9c8d1391` — docs: move completed math and keyboard work out of roadmap
- `e70161a5` / `2a8c91f9` — final source-audit documentation and final engineering-audit update
- `beb27a29` — docs: refresh public capability overview
- `5cbecf9c` — docs: finalize unreleased changelog
- `57474c57` — docs(history): archive pre-final continuation checkpoint
- `ac17e908` — docs: publish final source audit checkpoint
- `c3370c9b` — docs(state): finalize current CalcNova source status
- `8d8dd484` — fix(core): reject engineering underflow to zero
- `878be422` — test(core): cover engineering underflow rejection
- `5d4acdd3` — ci(core): guard engineering underflow rejection
- `39454bdb` — docs(core): document engineering underflow rejection

The final audit used many small commits intentionally so implementation, tests, validation contracts, and documentation remain independently traceable.

## Validation evidence status

The evidence policy remains conservative.

### NOT RUN locally

The required .NET 10 SDK/toolchains are unavailable in the active assistant execution environment, so the following are **NOT RUN** locally:

- `dotnet restore`;
- `dotnet format --verify-no-changes`;
- compiled solution/platform builds;
- compiled unit/integration tests;
- Avalonia compiled-XAML/headless execution;
- target-platform application/package builds;
- signing/notarization/provisioning/store checks;
- real device/browser accessibility and adaptive-layout audits.

### Final integrated source preflight

The final repository tree was audited through GitHub source/commit access and its source contracts/preflight inventory were hardened.

The complete `python tools/release_preflight.py` command was **not re-executed locally against a materialized final repository tree** because the local execution environment could not materialize the repository from GitHub.

Therefore source/test/workflow presence is not recorded as release PASS evidence.

### GitHub status observation

At the evidence check boundary, the latest inspected `main` commit returned:

- no combined GitHub status contexts;
- no pull-request-triggered workflow runs through the connector's commit-run lookup.

Those empty results are **not** treated as CI success.

The connector did not expose a general allowed branch push-run listing through the attempted endpoint, so no unobserved push workflow result is inferred.

### Commit identity

Inspected commit metadata uses:

`Sanskar <sanskarin@outlook.in>`

for author/committer identity on the created commits.

## Remaining work is evidence-dependent

No completed source module should be recreated simply to make the repository appear more complete.

The remaining high-priority work is:

1. observe real .NET 10 restore/format/analyzer/build/test output and fix concrete failures;
2. observe the Avalonia headless workflow and fix compiled-XAML/headless failures;
3. validate Windows/Linux/macOS Desktop launch, clipboard, persistence, keyboard, scaling, and packaging;
4. validate Browser/WebAssembly publish/load/storage/clipboard/keyboard/accessibility behavior;
5. validate Android workload builds, emulator/device layouts, persistence, clipboard, TalkBack/large text, signing, AAB, and store behavior;
6. validate iOS simulator/device behavior, Dynamic Type/VoiceOver, persistence/clipboard, signing/provisioning/archive/TestFlight/App Store behavior;
7. execute the runtime accessibility matrix with observed keyboard/screen-reader/contrast/large-text/touch/reduced-motion evidence;
8. validate compact/medium/expanded layouts on real target sizes, especially wide programmer grids and long export/statistics/Calculator-extension surfaces;
9. migrate remaining hard-coded English shared XAML to the semantic localization layer in compile-verified increments and validate Hindi/Devanagari layouts;
10. verify real produced release artifacts with artifact-manifest/checksum tooling;
11. generate and verify structured release evidence from the exact release-candidate commit;
12. run the final release gate with actual packaging/signing/store requirements for each claimed platform.

## Continuation rule

The next continuation must begin from observed evidence rather than duplicating completed source work:

1. read `PROJECT_STATE.md`;
2. read this file;
3. read `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`;
4. inspect the actual current `main` commit;
5. inspect CI/workflow results or run required commands in a suitable environment;
6. fix only concrete failures or clearly evidenced remaining product gaps;
7. never convert source presence, workflow presence, or unavailable checks into PASS evidence.
