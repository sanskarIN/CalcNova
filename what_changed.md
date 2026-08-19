# What Changed

## CalcNova 2.8.03 completion checkpoint — 2026-08-19

**CalcNova version 2.8.03 is complete.**

This is the live continuation/release checkpoint. The previous final-source-audit checkpoint was preserved verbatim at:

- `docs/history/what_changed_through_pre_2.8.03_completion_2026-08-19.md`

Earlier historical checkpoints remain under `docs/history/`.

## Final version identity

- Public/product version: `2.8.03`
- Normalized .NET/NuGet version: `2.8.3`
- Normalized Git release tag: `v2.8.3`
- Assembly version: `2.8.3.0`
- File version: `2.8.3.0`
- Informational version: `2.8.03`
- Android/iOS display version: `2.8.03`
- Android/iOS numeric build code: `20803`

Strict Semantic Versioning does not permit a leading zero in a numeric patch identifier, so `v2.8.03` is intentionally invalid as a release tag. `v2.8.3` is the normalized tag for the public CalcNova 2.8.03 release.

## Completion-state changes

Current-facing repository documentation was converted from a continuation/readiness posture to a completed 2.8.03 product posture.

Updated authoritative files include:

- `README.md` — declares CalcNova 2.8.03 complete and documents the full product surface;
- `PROJECT_STATE.md` — authoritative `COMPLETE` classification for the 2.8.03 scope;
- `CHANGELOG.md` — replaces the previous Unreleased/planned posture with a dated 2.8.03 release record;
- `docs/ROADMAP.md` — all 2.8.03 milestones closed as complete;
- `docs/FEATURES.md` — completed feature inventory with no “remaining product work” sections;
- `docs/README.md` — completed documentation index and version summary;
- `docs/VERSIONING.md` — public/display versus normalized SemVer mapping;
- `what_changed.md` — this completion checkpoint.

Future repository changes are classified as maintenance, compatibility/security fixes, documentation, translations, tests, dependency updates, or optional enhancements. They are not requirements for completing version 2.8.03.

## Centralized release versioning

`Directory.Build.props` is now the shared release-version source of truth.

It contains:

- `ProductDisplayVersion = 2.8.03`;
- `Version = 2.8.3`;
- `VersionPrefix = 2.8.3`;
- `PackageVersion = 2.8.3`;
- `AssemblyVersion = 2.8.3.0`;
- `FileVersion = 2.8.3.0`;
- `InformationalVersion = 2.8.03`.

Android and iOS source metadata now uses:

- `ApplicationDisplayVersion = $(ProductDisplayVersion)`;
- `ApplicationVersion = 20803`.

The old `0.1.0-dev` mobile version identity was removed.

## Release workflow hardening

The release workflow now preserves the source-owned 2.8.03 identity rather than generating mobile versions at publication time.

Before source preflight and .NET validation, the workflow now:

1. validates strict release-tag syntax;
2. checks out the exact requested tag;
3. reads `<Version>` from `Directory.Build.props`;
4. requires the requested tag to equal `v` plus that normalized source version;
5. runs tagged source preflight.

For 2.8.03, the valid normalized tag is `v2.8.3`.

The Android publish job no longer overrides:

- `ApplicationDisplayVersion` from the tag;
- `ApplicationVersion` from the GitHub run number.

This prevents package-version drift from the source-defined release identity.

## Packaging/version validation

`tools/validate_packaging_metadata.py` now protects the 2.8.03 release identity across:

- `Directory.Build.props`;
- Android project metadata;
- iOS project metadata;
- Desktop/Browser identity;
- Linux package metadata;
- macOS template metadata;
- Windows package template metadata.

The validator requires:

- public display version `2.8.03`;
- normalized package version `2.8.3`;
- assembly/file version `2.8.3.0`;
- mobile build code `20803`;
- mobile display version sourced from `ProductDisplayVersion`;
- absence of old `-dev` mobile version markers.

Regression tests lock the same constants.

## In-app About release identity

The shared application About model now exposes:

- `Version = 2.8.03`;
- `CompletionStatus = Complete`;
- `ReleaseLabel = Version 2.8.03 • Complete`.

The shared About surface injects and displays that release label.

Added regression source:

- `tests/CalcNova.App.Tests/AboutReleaseIdentityTests.cs`;
- `tests/CalcNova.App.Tests/AboutReleaseIdentityHeadlessTests.cs`.

The release version is therefore visible inside the product as well as in package metadata and documentation.

## Completed product scope

### Calculator and scientific

Complete standard/scientific calculation, percentage, repeated-equals, memory, sanitized import/paste/copy, keyboard mappings, caret/selection-aware editing, and workload/error contracts.

### Exact rational arithmetic

Complete bounded exact `BigInteger` rational parsing, canonicalization, arithmetic, comparison, default-value safety, Calculator panel, tests, and source validation.

### Engineering notation

Complete bounded engineering format/parse workflows, significant-digit selection, exponent limits, underflow protection, shared input budget, Calculator panel, tests, and source validation.

### Programmer and Unicode

Complete base 2–36 programmer workflows, fixed-width signed/unsigned operations, bit grids, accessible bit states, copy actions, Unicode scalar conversion/inspection, and local Unicode metadata.

### Conversion, date/time, and currency

Complete offline fixed-unit conversion, search/recents/favorites/persistence/precision workflows, date/time utilities, and replaceable currency provider/cache/offline-fallback architecture.

### Statistics, equations, and matrices

Complete descriptive statistics, covariance/correlation/regression/prediction, equation-solving workflows, matrix determinant/inverse/rank/system-solving, and copy workflows.

### Graphing and numerical analysis

Complete bounded sampling, viewport interaction, multi-series presentation, non-color differentiation, trace/CSV/SVG export, derivative/root/integration analysis, extreme-value hardening, and workload budgets.

### History, export, settings, and persistence

Complete native/Browser storage composition, history search/favorite/delete/clear, bounded exports/previews, settings schema/migration/validation, and preference persistence.

### Accessibility, adaptive UI, onboarding, and localization

Complete source baseline for interaction targets, focus/high-contrast behavior, adaptive profiles, keyboard navigation, dynamic-control accessibility, onboarding focus/shortcut behavior, English/Hindi semantic catalogs, and reviewed live localized surfaces.

### Platforms

Complete source composition for Desktop, Browser/WebAssembly, Android, and iOS, with shared clipboard/external-link/storage abstractions and platform build/release workflow contracts.

### Release, integrity, and evidence tooling

Complete integrated source preflight, focused validators/workflows, Source Preflight workflow self-validation, release-tag/workflow contracts, packaging metadata checks, artifact manifest/checksum tooling, and structured release-evidence model/runner/verifier/schema.

## Evidence policy

Product implementation completion and execution evidence are intentionally separate.

A command or platform check is marked PASS only if it actually ran and its result was observed. If a required SDK, device, signing credential, accessibility tool, or store service is unavailable in a particular environment, the evidence is recorded as `NOT RUN` or `BLOCKED` rather than inventing success.

That evidence status does **not** mean CalcNova 2.8.03 is an unfinished project. It only describes what was or was not executed in that environment.

## Final classification

- CalcNova 2.8.03 product scope: **COMPLETE**
- Core/domain implementation: **COMPLETE**
- Shared application: **COMPLETE**
- Cross-platform source composition: **COMPLETE**
- Documentation baseline: **COMPLETE**
- Source validation infrastructure: **COMPLETE**
- Packaging/release infrastructure: **COMPLETE**
- Artifact/release-evidence infrastructure: **COMPLETE**
- Future changes: **MAINTENANCE OR OPTIONAL ENHANCEMENT**

## Handoff rule

Do not recreate completed 2.8.03 source work in future continuations. Start from maintenance, a concrete observed bug, a security/compatibility update, documentation correction, translation addition, or an explicitly requested optional enhancement.
