# CalcNova Documentation Consistency Audit — 2026-08-20

## Scope

This audit reconciles CalcNova's maintained documentation with the completed 2.8.03 repository state.

The primary goal was to remove development-era wording that incorrectly described implemented source as future/planned work while preserving the project's strict distinction between:

- **implemented source capability**;
- **automated source/test infrastructure**;
- **observed runtime/release evidence**.

The authoritative product classification remains:

- product/display version: `2.8.03`;
- normalized package version: `2.8.3`;
- normalized release tag: `v2.8.3`;
- mobile build code: `20803`;
- application id: `in.sanskar.calcnova`;
- product scope: **COMPLETE**;
- Desktop/Browser/Android/iOS source composition: **COMPLETE**.

## Source-of-truth hierarchy used

Conflicts were resolved against the current repository in this order:

1. `PROJECT_STATE.md` for product/completion classification;
2. `Directory.Build.props` and `docs/VERSIONING.md` for release identity;
3. actual `src/CalcNova.*/*.csproj` files for target frameworks/platform metadata;
4. actual source/application/test files for implemented capability;
5. `.github/workflows/build-*.yml` and `.github/workflows/release.yml` for automated build/release behavior;
6. `docs/PLATFORM_SUPPORT.md` for platform-source composition status;
7. runtime/evidence documents for observed `PASS / FAIL / BLOCKED / NOT RUN` status.

A dated historical checkpoint was not allowed to override a newer maintained source-of-truth document.

## Platform facts reconciled

The documentation now consistently describes these maintained heads:

| Target | Project / target |
|---|---|
| Windows/Linux/macOS | `src/CalcNova.Desktop` / `net10.0` Avalonia Desktop |
| Browser/WebAssembly | `src/CalcNova.Browser` / `net10.0-browser` |
| Android | `src/CalcNova.Android` / `net10.0-android` |
| iOS/iPadOS | `src/CalcNova.iOS` / `net10.0-ios` |

Current mobile metadata documented consistently:

- Android minimum platform/API: 23;
- iOS minimum platform version: 15.0;
- Android CI JDK: 17;
- Android/iOS display version: `2.8.03`;
- Android/iOS numeric build code: `20803`.

Current automated release artifact families documented consistently:

- Desktop `win-x64`;
- Desktop `linux-x64`;
- Desktop `osx-x64`;
- Browser/WebAssembly publish bundle;
- signed Android AAB when external signing secrets are configured;
- checksum material;
- iOS simulator validation kept separate from signed App Store distribution.

## Maintained documents corrected in this pass

The following current-facing guides contained stale, incomplete, contradictory, or environment-specific wording and were updated.

### Cross-platform/build/release navigation

- `docs/BUILDING.md`
  - replaced claims that Android/iOS/Browser heads were absent;
  - documented current workloads/toolchains;
  - documented Desktop, Browser, Android, and iOS build paths;
  - documented current release RIDs;
  - documented Android signed-AAB secret contract;
  - documented iOS simulator RIDs;
  - separated build-source availability from runtime/signing/store evidence.

- `docs/README.md`
  - expanded into a comprehensive documentation index;
  - linked previously unindexed maintained guides;
  - added documentation source-of-truth rules;
  - separated current guides from dated historical records.

- `docs/TROUBLESHOOTING.md`
  - removed obsolete statements that Browser/Android heads were unimplemented;
  - added current Browser/Android/iOS/Desktop diagnostics;
  - documented workload/JDK/Xcode/signing distinctions.

- `docs/RUNTIME_VALIDATION_RUNBOOK.md`
  - replaced stale release example `v0.1.0` with normalized 2.8.03 tag `v2.8.3`;
  - aligned platform commands/artifact expectations with current workflows;
  - retained conservative evidence semantics.

### Architecture/security/privacy

- `docs/ARCHITECTURE.md`
  - replaced development-era future-module descriptions;
  - documented the actual domain/application/platform project inventory;
  - documented native versus Browser persistence composition;
  - documented current graph/localization/accessibility/platform architecture.

- `docs/PRIVACY.md`
  - removed obsolete future statements for history, Browser persistence, and currency;
  - documented current local history/settings/export flows;
  - documented optional network-enhanced currency behavior;
  - documented no-account/local-first/clipboard/privacy boundaries.

- `docs/SECURITY.md`
  - replaced future-network wording with the implemented provider/cache/offline currency model;
  - documented Browser, Android, and iOS security boundaries;
  - documented signing-secret and release-security expectations.

### Calculator/numerical features

- `docs/CALCULATION_ENGINE.md`
  - moved exact rational and engineering notation from future wording into current completed capability;
  - documented current graph/numerical workload boundaries;
  - kept optional post-2.8.03 numerical ideas separate.

- `docs/CALCULATOR_EDITING.md`
  - retained the selection/caret implementation contract;
  - removed a permanent environment-specific `NOT RUN` statement;
  - moved compiled/runtime state into per-run evidence semantics.

- `docs/INPUT_SAFETY.md`
  - retained sanitizer/clipboard safety behavior;
  - removed environment-specific SDK status from permanent feature documentation;
  - clarified external-text trust boundary.

- `docs/KEYBOARD_SHORTCUTS.md`
  - corrected the outdated claim that shifted top-row operators were only planned;
  - documented implemented Shift-only `+`, `*`, `(`, `)`, `^`, `%` mappings;
  - aligned calculator selection, graph keyboard, onboarding, and clipboard behavior with current source contracts.

- `docs/NUMERICAL_ANALYSIS.md`
  - removed obsolete remaining-work items for trace, multi-series, and export workflows;
  - documented current derivative/root/integration/sampling/trace/export integration;
  - retained approximation and workload-safety boundaries.

- `docs/BIVARIATE_STATISTICS.md`
  - retained current bounded statistical behavior;
  - normalized compiled/runtime evidence wording so status is recorded per observed run.

- `docs/EXACT_RATIONALS.md`
  - retained exact arithmetic/workload contracts;
  - normalized evidence wording instead of globally freezing a prior environment's `NOT RUN` status.

- `docs/ENGINEERING_NOTATION.md`
  - retained formatting/parsing/extreme-value/input-bound contracts;
  - normalized evidence wording to per-run evidence.

### Converter/programmer/accessibility/design

- `docs/CONVERTER_MODE.md`
  - removed obsolete “remaining product work” for implemented search, copy, clear-recents, defaults, persistence, and adaptive behavior;
  - documented completed 1–17 digit precision, recents/favorites, defaults, local persistence, and currency separation.

- `docs/PROGRAMMER_MODE.md`
  - removed obsolete remaining work for large-word grouping and copy actions;
  - documented current byte-grouped large-word presentation and explicit copy workflows;
  - preserved target screen-reader/runtime checks as evidence rather than unfinished source.

- `docs/ACCESSIBILITY.md`
  - removed future wording for implemented graph keyboard controls/programmer grouping/copy/onboarding focus behavior;
  - documented current shared accessibility baseline;
  - preserved strict target-runtime evidence requirements.

- `docs/DESIGN_SYSTEM.md`
  - replaced planned/future component/theme/branding wording with the implemented adaptive/accessibility/theme/brand baseline;
  - documented repository-owned geometric brand asset generation;
  - documented graph/programmer/converter/onboarding design contracts.

### Localization/onboarding/UI automation

- `docs/LOCALIZATION.md`
  - replaced obsolete statement that shared UI remained predominantly English;
  - documented completed English/Hindi semantic catalogs and reviewed live surfaces;
  - classified additional languages/detail migration as optional post-2.8.03 work.

- `docs/LIVE_LOCALIZATION.md`
  - converted “remaining localization work” from a release-completion gap into optional post-2.8.03 expansion;
  - retained the precise distinction between semantic UI text and invariant mathematical/technical data.

- `docs/ONBOARDING.md`
  - corrected obsolete statement that onboarding remained English-only;
  - documented current English/Hindi live-localized onboarding;
  - retained target accessibility/runtime checks as evidence requirements.

- `docs/UI_AUTOMATION.md`
  - replaced the early small headless-test inventory with the current broader representative coverage;
  - documented graph, onboarding, localization, exact rational, engineering notation, Unicode, bivariate statistics, adaptive/accessibility, and release-identity coverage categories;
  - removed one-environment `NOT RUN` wording from the permanent guide.

## Maintained documents audited and left unchanged

These files were reviewed against current source/status and did not require a consistency rewrite during this pass:

- `README.md`;
- `PROJECT_STATE.md`;
- `CHANGELOG.md`;
- `docs/ACCESSIBILITY_TEST_MATRIX.md`;
- `docs/ADAPTIVE_LAYOUT.md`;
- `docs/CALCULATOR_KEYBOARD_INPUT.md`;
- `docs/CONVERTER_DEFAULTS_AND_PRIVACY.md`;
- `docs/EXPORT_PREVIEWS.md`;
- `docs/FEATURES.md`;
- `docs/FOCUS_VISIBILITY.md`;
- `docs/GRAPH_INTERACTION.md`;
- `docs/GRAPH_NUMERICAL_SAFETY.md`;
- `docs/GRAPH_SERIES_PRESENTATION.md`;
- `docs/GRAPH_VIEWPORT_CONTROLS.md`;
- `docs/IOS_RELEASE_VALIDATION.md`;
- `docs/NUMERICAL_SAFETY.md`;
- `docs/PLATFORM_SUPPORT.md`;
- `docs/RELEASE.md`;
- `docs/RELEASE_READINESS_CHECKLIST.md`;
- `docs/ROADMAP.md`;
- `docs/SETTINGS_MIGRATION.md`;
- `docs/SETTINGS_STORAGE_CONTRACT.md`;
- `docs/SOURCE_HARDENING_SUITE.md`;
- `docs/SOURCE_PREFLIGHT.md`;
- `docs/TESTING.md`;
- `docs/UNICODE_METADATA.md`;
- `docs/VALIDATION_EVIDENCE.md`;
- `docs/VERSIONING.md`;
- `docs/XAML_VALIDATION.md`;
- `docs/release-evidence.schema.json`.

“Left unchanged” means no material contradiction was identified in this consistency pass; it does not exempt a file from future maintenance when its source contract changes.

## Historical records intentionally preserved

Dated checkpoint/audit files are historical evidence, not live product contracts. They were intentionally not rewritten merely to make old statements look current:

- `docs/CONTINUATION_2026-08-19_PASS2.md`;
- `docs/CONTINUATION_CHECKPOINT_2026-08-19.md`;
- `docs/FINAL_SOURCE_AUDIT_2026-08-19.md`;
- `docs/RELEASE_SOURCE_CHECKPOINT_2026-08-19.md`;
- `docs/history/`.

When these differ from current maintained documentation, current authoritative files win.

## Evidence-policy result

The audit preserves the rule:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

A completed source feature must not be called unfinished merely because a particular environment has not executed a runtime check.

Conversely, source/test/workflow presence must not be converted into a fabricated runtime PASS.

Permanent feature documentation now describes the feature contract; dated/machine-readable validation records describe what was actually executed.

## Documentation maintenance rule

For future maintenance:

- platform project/workflow changes must update `BUILDING.md`, `PLATFORM_SUPPORT.md`, and relevant release docs in the same change;
- data/network/storage changes must review `PRIVACY.md` and `SECURITY.md`;
- settings-schema changes must update settings migration/storage docs;
- keyboard/input changes must update calculator editing/input/shortcut docs;
- graph numerical/presentation changes must update the relevant graph/numerical guides;
- localization surface/catalog changes must update both localization guides;
- accessibility/adaptive behavior changes must update accessibility/design/runtime-matrix documentation;
- new validators/workflows must be represented in source-preflight/testing documentation;
- version changes must originate from the version source of truth and update public release documentation consistently.

## Audit conclusion

The maintained CalcNova documentation baseline is now reconciled with the completed 2.8.03 source state for the inconsistencies identified in this pass.

The major development-era contradictions around platform-head availability, implemented feature status, localization/onboarding status, keyboard mappings, and environment-specific `NOT RUN` prose have been removed from current-facing guides.

Runtime/device/signing/store evidence remains intentionally conservative and separate from product/source completion.
