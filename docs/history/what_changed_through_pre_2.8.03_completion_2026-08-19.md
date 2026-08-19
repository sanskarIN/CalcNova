# What Changed

## Final source-audit checkpoint — 2026-08-19

This is the live continuation checkpoint for CalcNova after the final source/documentation hardening pass.

Historical detail is preserved at:

- `docs/history/what_changed_through_full_source_hardening_2026-08-19.md` — earlier cumulative history;
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md` — the complete active continuation immediately before this final audit.

The detailed final review is:

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`.

## Final audit scope

The final pass inspected the actual current `main` source, tests, validators, release tooling, focused workflows, and documentation rather than recreating already completed modules.

Work concentrated on:

- concrete correctness defects found by comparing implementation, tests, validator contracts, and documentation;
- missing workload limits at numeric/UI boundaries;
- source validators that existed but were missing from the integrated release preflight;
- focused/master workflow path filters that could skip protected changes;
- stale public feature/roadmap/project-state documentation;
- conservative PASS / FAIL / BLOCKED / NOT RUN evidence semantics;
- creating a clean handoff from source hardening to real compiled/platform validation.

## Concrete defects fixed

### Exact-rational validator/source mismatch

`tools/validate_rational_numbers.py` expected a stale magnitude-check source marker even though the implementation contained the intended reduced bit-length guard in another form.

The validator now matches the real `GetBitLength()` contract.

### `default(RationalNumber)` canonicalization

Because `RationalNumber` is a C# value type, `default(RationalNumber)` can exist without invoking the validating constructor.

The previous representation could therefore expose denominator zero even though normal construction rejects zero denominators.

The type now uses backing fields and treats the zero-initialized denominator backing field as canonical denominator `1`.

`default(RationalNumber)` therefore behaves as exact zero instead of exposing synthetic `0/0` state.

Regression coverage verifies equality, numerator/denominator, integer classification, string representation, arithmetic, and comparison. The source validator protects the default-value contract.

### Exact-rational raw input budget bypass

The rational contract required the raw input-character budget to be enforced before trimming. The implementation checked the trimmed text length instead.

That allowed a tiny valid number wrapped in arbitrarily large whitespace to bypass the intended input budget.

The parser now checks `text.Length` before trimming. A dedicated regression rejects oversized whitespace-padded input, and the validator requires both the pre-trim guard and regression scenario.

### Engineering finite exponent contract

Engineering-notation documentation/validation required explicit finite engineering exponent boundaries, but formatter/parser source and tests did not yet fully enforce them.

The implementation now defines and enforces:

- minimum engineering exponent: `-324`;
- maximum engineering exponent: `306`.

Out-of-range zero and non-zero exponent forms are rejected so a value such as `0e+309` cannot bypass the workload contract simply because its mathematical result would remain zero.

### Engineering non-zero underflow becoming zero

`1e-324` is syntactically valid engineering notation, but it lies below the minimum positive subnormal `double`. Bounded power-of-ten scaling therefore produces floating-point zero.

Silently returning `0` for a non-zero input would change its mathematical meaning.

The parser now rejects the case when the mantissa is non-zero and scaling produces `0`, throwing `OverflowException` instead of silently returning zero.

Regression coverage and the SDK-independent validator protect this behavior.

### Engineering input text workload bound

The engineering core parser and Calculator Format action previously accepted arbitrary-length strings before numeric parsing, and the engineering TextBox had no feature-specific maximum length.

A single 4,096-character contract now applies end-to-end:

- `EngineeringNotationFormatter.MaximumInputCharacters = 4_096`;
- core `Parse` checks raw length before whitespace scanning/trimming/numeric parsing;
- oversized all-whitespace input is rejected before blank scanning;
- `EngineeringNotationViewModel.Format` checks the same bound before `double.TryParse`;
- `EngineeringNotationPanel` sets `TextBox.MaxLength` from the core constant;
- core, App, and headless regressions cover the boundary;
- `tools/validate_engineering_notation.py` validates the core formatter, App view model, panel, and regression scenarios.

### Focused engineering workflow path gap

The engineering source validator now protects App view-model/panel behavior, but the focused engineering workflow previously watched only the original core/test/validator paths.

`.github/workflows/engineering-notation-validate.yml` now watches:

- the core formatter;
- App engineering view model;
- App engineering panel;
- core tests;
- App view-model tests;
- App headless panel tests;
- the validator and its regression test;
- the workflow itself.

This keeps the focused gate aligned with what its validator actually checks.

### Integrated release-preflight inventory gaps

`tools/release_preflight.py` was audited against focused validators added during the wider continuation.

The integrated source gate now includes recent critical validator families and regression suites for:

- exact rational arithmetic;
- engineering notation;
- artifact manifest/integrity infrastructure;
- machine-readable release-evidence infrastructure;
- dynamic shared-control accessibility;
- exact-tag unsigned iOS simulator release workflow;
- the Source Preflight workflow itself.

`tools/tests/test_release_preflight.py` was expanded so these validators/test modules cannot silently disappear from the unified inventory.

### Master Source Preflight path-filter gap

The integrated Python preflight validates contracts across many domain libraries, tests, tools, docs, packaging files, and workflows. The earlier `.github/workflows/source-preflight.yml` path filter watched only a selected subset of those areas.

A domain-only change could therefore bypass the unified gate.

The master workflow now watches relevant changes across:

- `src/**`;
- `tests/**`;
- `tools/**`;
- `packaging/**`;
- `docs/**`;
- `.github/workflows/**`;
- `global.json`;
- `Directory.Build.props`;
- `Directory.Packages.props`;
- `CalcNova.slnx`;
- `CalcNova.All.slnx`;
- `.gitignore`;
- `README.md`;
- `CHANGELOG.md`;
- `PROJECT_STATE.md`;
- `what_changed.md`.

The workflow remains least-privilege with `contents: read`.

A new `tools/validate_source_preflight_workflow.py` validator and regression suite protect:

- push/PR broad source coverage;
- manual dispatch;
- read-only contents permission;
- Ubuntu runner;
- timeout;
- checkout/setup-python versions;
- Python 3.13;
- the integrated preflight command;
- rejection of `pull_request_target`, `contents: write`, and `actions: write` drift.

That workflow validator and its tests are themselves integrated into `tools/release_preflight.py`.

## Completed source capabilities synchronized into documentation

### Calculator and numeric utilities

- selection/caret-aware keypad editing;
- forward/reversed selection replacement;
- selection-aware Backspace;
- selection-preserving function/parenthesis wrapping;
- safe printable/shifted operator mappings outside active text fields;
- bounded exact rational arithmetic and Calculator utility panel;
- bounded engineering notation and Calculator utility panel;
- engineering input/exponent/non-zero-underflow protection.

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
- Source Preflight workflow self-validation and broad trigger coverage;
- artifact manifest generation/verification and SHA-256 integrity infrastructure;
- machine-readable validation evidence schema/model/runner/verifier;
- platform workflow source contracts;
- exact-tag unsigned iOS simulator release validation.

## Documentation completed/synchronized

### Added

- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`
- `docs/history/what_changed_through_pre_final_audit_2026-08-19.md`
- `tools/validate_source_preflight_workflow.py`
- `tools/tests/test_validate_source_preflight_workflow.py`

### Updated

- `README.md` — current public capability/evidence overview;
- `CHANGELOG.md` — current Unreleased feature/fix/security/validation inventory;
- `PROJECT_STATE.md` — authoritative current source/evidence status including engineering and preflight hardening;
- `docs/README.md` — indexes exact rationals, engineering notation, bivariate statistics, validation evidence, and final audit;
- `docs/FEATURES.md` — current implemented feature contracts including engineering input/underflow and master-preflight self-validation;
- `docs/ROADMAP.md` — completed numeric/statistics/keyboard/graph hardening moved out of future work;
- `docs/SOURCE_PREFLIGHT.md` — current integrated validator inventory, broad workflow triggers, least privilege, and self-validation;
- `docs/EXACT_RATIONALS.md` — default-value canonicalization and pre-trim input-budget enforcement;
- `docs/ENGINEERING_NOTATION.md` — finite exponent bounds, non-zero-underflow rejection, 4,096-character core/App/UI input contract, and focused workflow coverage;
- `.github/workflows/engineering-notation-validate.yml` — aligned with the full engineering contract surface;
- `.github/workflows/source-preflight.yml` — broad repository source/test/tool/docs/workflow trigger coverage;
- `what_changed.md` — this final live checkpoint.

## Key final-audit commits

The final audit intentionally used many focused commits so implementation, tests, validation contracts, workflows, and documentation remain independently traceable.

### Rational hardening

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

### Engineering hardening

- engineering exponent-bound implementation/test commits — explicit -324..306 parsing contract
- `8d8dd484` — fix(core): reject engineering underflow to zero
- `878be422` — test(core): cover engineering underflow rejection
- `5d4acdd3` — ci(core): guard engineering underflow rejection
- `39454bdb` — docs(core): document engineering underflow rejection
- `acb19ef9` — fix(core): bound engineering notation input text
- `a78a012c` — test(core): cover engineering input budget
- `6e4e5bd4` — fix(app): bound engineering format input
- `3bcaa463` — fix(app): cap engineering notation text entry
- `0c546e2b` — test(app): cover engineering format input budget
- `62507b1b` — test(app): cover engineering text entry budget
- `02b46189` — ci(core): validate engineering input budgets end to end
- `37b50dc9` — ci(core): watch engineering app contract paths
- `4d2f8b6e` — docs(core): document engineering text workload budget

### Release/source-gate hardening

- `3da7c93e` — ci(release): integrate remaining standalone validators
- `597fc261` — test(release): require standalone validator integration
- `adf80fd3` — ci(release): broaden source preflight path coverage
- `10bbaff2` — ci(release): validate source preflight workflow contract
- `adc9d910` — test(release): cover source preflight workflow validator
- `59b63071` — ci(release): self-validate source preflight workflow
- `acbc26e8` — test(release): require source preflight workflow validation
- `1bcc5e81` — docs(release): document preflight workflow self-validation

### Documentation/audit synchronization

- `a1856f78` — docs(release): synchronize complete source preflight inventory
- `9a12dde8` — docs: index completed math and release-evidence guides
- `50321d45` — docs: synchronize implemented feature inventory
- `9c8d1391` — docs: move completed math and keyboard work out of roadmap
- `e70161a5` — docs(audit): record final source hardening review
- `beb27a29` — docs: refresh public capability overview
- `5cbecf9c` — docs: finalize unreleased changelog
- `57474c57` — docs(history): archive pre-final continuation checkpoint
- `c3370c9b` — docs(state): finalize current CalcNova source status
- `315126b4` — docs: record final workload and preflight fixes
- `54ce3326` — docs: record final engineering and preflight hardening
- `08237d82` — docs(state): record final workload and preflight guards
- `03b45fbf` — docs(audit): finalize workload and CI trigger findings

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

The repository was audited and hardened through GitHub source/commit access.

The complete `python tools/release_preflight.py` command was **not re-executed locally against a materialized final repository tree** because the local execution environment could not materialize the repository from GitHub.

Therefore source/test/workflow presence is not recorded as release PASS evidence.

### GitHub status observation

An earlier status check during this final audit returned no combined GitHub status contexts and no pull-request-triggered workflow runs for the then-current `main` commit.

Those empty results were **not** treated as CI success.

Additional hardening commits have been pushed since that observation, so the exact final `main` commit must be checked again before recording any CI status.

### Commit identity

Inspected commit metadata uses:

`Sanskar <sanskarin@outlook.in>`

for author/committer identity on created commits.

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
