# CalcNova 2.9.5 Documentation

**Project status: Complete for version 2.9.5.**

This directory contains the authoritative product, engineering, quality, platform, packaging, validation, security-maintenance, and release documentation for the completed CalcNova 2.9.5 baseline.

Normalized release tag: `v2.9.5`  
Android/iOS numeric build code: `20905`

The requested 2.9.0 intermediate preparation is preserved in [releases/2.9.0.md](releases/2.9.0.md). Current-facing guides use 2.9.5; dated older records remain historical.

Use this page as the documentation map. Current guides are grouped by responsibility; dated continuation/audit/release-checkpoint records are grouped separately so history is not confused with current product status.

## Start here

- [Project state](../PROJECT_STATE.md) — authoritative completion/status record.
- [Main README](../README.md) — product overview and major capabilities.
- [Versioning](VERSIONING.md) — current `2.9.5` / `v2.9.5` / `20905` mapping and centralized release-identity contract.
- [Completed feature inventory](FEATURES.md) — implemented 2.9.5 product scope.
- [Completed roadmap](ROADMAP.md) — completed milestone record and optional post-release ideas.
- [2.9.0 release checkpoint](releases/2.9.0.md) — preserved requested intermediate release preparation.
- [Platform support](PLATFORM_SUPPORT.md) — Windows/Linux/macOS, Browser/PWA, Android, and iOS composition status.
- [Building](BUILDING.md) — current build/run/publish commands and platform prerequisites.
- [Testing](TESTING.md) — test layers and responsibilities.
- [Troubleshooting](TROUBLESHOOTING.md) — common toolchain and platform problems.

## Architecture and engineering

- [Architecture](ARCHITECTURE.md)
- [Design system](DESIGN_SYSTEM.md)
- [Adaptive layout](ADAPTIVE_LAYOUT.md)
- [Input safety and expression import](INPUT_SAFETY.md)
- [Settings storage contract](SETTINGS_STORAGE_CONTRACT.md)
- [Settings migration](SETTINGS_MIGRATION.md)
- [Privacy](PRIVACY.md)
- [Security engineering](SECURITY.md)
- [Security automation](SECURITY_AUTOMATION.md)
- [Artifact provenance](ARTIFACT_PROVENANCE.md)

## Calculator and mathematical features

- [Calculation engine](CALCULATION_ENGINE.md)
- [Calculator expression editing](CALCULATOR_EDITING.md)
- [Calculator keyboard input](CALCULATOR_KEYBOARD_INPUT.md)
- [Keyboard shortcuts](KEYBOARD_SHORTCUTS.md)
- [Exact rational arithmetic](EXACT_RATIONALS.md)
- [Engineering notation](ENGINEERING_NOTATION.md)
- [Programmer mode](PROGRAMMER_MODE.md)
- [Unicode scalar metadata](UNICODE_METADATA.md)
- [Converter mode](CONVERTER_MODE.md)
- [Converter defaults and privacy](CONVERTER_DEFAULTS_AND_PRIVACY.md)
- [Bivariate statistics](BIVARIATE_STATISTICS.md)
- [Numerical analysis](NUMERICAL_ANALYSIS.md)
- [Numerical safety](NUMERICAL_SAFETY.md)

## Graphing

- [Graph interaction](GRAPH_INTERACTION.md)
- [Graph viewport controls](GRAPH_VIEWPORT_CONTROLS.md)
- [Graph series presentation](GRAPH_SERIES_PRESENTATION.md)
- [Graph numerical safety and workload bounds](GRAPH_NUMERICAL_SAFETY.md)

## History, export, onboarding, and localization

- [Bounded export previews](EXPORT_PREVIEWS.md)
- [Onboarding](ONBOARDING.md)
- [Localization architecture and catalogs](LOCALIZATION.md)
- [Live localization behavior](LIVE_LOCALIZATION.md)

## Accessibility and UI quality

- [Accessibility](ACCESSIBILITY.md)
- [Accessibility runtime test matrix](ACCESSIBILITY_TEST_MATRIX.md)
- [Focus visibility contract](FOCUS_VISIBILITY.md)
- [Adaptive layout](ADAPTIVE_LAYOUT.md)
- [UI automation](UI_AUTOMATION.md)
- [XAML validation](XAML_VALIDATION.md)

## Platform and runtime validation

- [Platform support](PLATFORM_SUPPORT.md)
- [Building](BUILDING.md)
- [Runtime validation runbook](RUNTIME_VALIDATION_RUNBOOK.md)
- [iOS release validation](IOS_RELEASE_VALIDATION.md)
- [Machine-readable validation evidence](VALIDATION_EVIDENCE.md)

Maintained platform build workflow source contracts live in:

- `.github/workflows/build-desktop.yml`;
- `.github/workflows/build-browser.yml`;
- `.github/workflows/build-android.yml`;
- `.github/workflows/build-ios.yml`;
- `.github/workflows/platform-support-validate.yml`.

Release publication is defined by `.github/workflows/release.yml`.

The maintained source matrix covers:

- Windows `win-x64` and `win-arm64`;
- Linux `linux-x64` and `linux-arm64`;
- macOS `osx-x64` and `osx-arm64`;
- Browser/WebAssembly/PWA;
- Android `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- iOS `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64`.

## Release identity and version consistency

`Directory.Build.props` remains the source of truth. `tools/release_identity.py` validates central release fields and derives the expected release tag and mobile build code.

Current identity:

```text
Product/display: 2.9.5
Package: 2.9.5
Tag: v2.9.5
Assembly/file: 2.9.5.0
Mobile build code: 20905
```

Focused checks:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
```

The 2.9.0 checkpoint used `v2.9.0` and build code `20900` before the repository intentionally advanced to 2.9.5.

## Security maintenance and supply-chain checks

- [Public security policy](../SECURITY.md)
- [Secure engineering](SECURITY.md)
- [Security automation](SECURITY_AUTOMATION.md)
- [Release artifact provenance](ARTIFACT_PROVENANCE.md)

Maintained security controls include:

- `Directory.Build.props` — direct/transitive NuGet Audit with `NuGetAuditMode=all`, `NuGetAuditLevel=moderate`, and warnings-as-errors enforcement;
- `.github/workflows/codeql.yml` — C# code scanning;
- `.github/workflows/dependency-review.yml` — pull-request dependency vulnerability review;
- `.github/dependabot.yml` — scheduled dependency update proposals;
- `.github/workflows/security-automation-validate.yml` — focused read-only workflow/policy contract validation;
- `.github/workflows/release.yml` — least-privilege release publication with flat checksums, deterministic CycloneDX SBOMs, and artifact provenance attestations.

Repository-owned source validation is provided by:

```bash
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python -m unittest tools.tests.test_validate_security_workflows
python -m unittest tools.tests.test_validate_dependency_security
python tools/validate_release_workflow.py .
python -m unittest tools.tests.test_validate_release_workflow
```

These checks are also integrated into the main source preflight. The online NuGet advisory query is performed by `dotnet restore`; source validation only protects the audit policy itself.

## Testing and source validation

- [Testing](TESTING.md)
- [SDK-independent source preflight](SOURCE_PREFLIGHT.md)
- [Source hardening suite](SOURCE_HARDENING_SUITE.md)
- [UI automation](UI_AUTOMATION.md)
- [XAML validation](XAML_VALIDATION.md)
- [Runtime validation runbook](RUNTIME_VALIDATION_RUNBOOK.md)
- [Validation evidence model](VALIDATION_EVIDENCE.md)
- [Release evidence JSON schema](release-evidence.schema.json)

The integrated SDK-independent current-release gate is:

```bash
python tools/release_preflight.py --tag v2.9.5
```

The compiled .NET gate is documented in [BUILDING.md](BUILDING.md) and [TESTING.md](TESTING.md).

## Release and packaging

- [Release process](RELEASE.md)
- [Release artifact provenance](ARTIFACT_PROVENANCE.md)
- [Release readiness checklist](RELEASE_READINESS_CHECKLIST.md)
- [Versioning](VERSIONING.md)
- [Platform support](PLATFORM_SUPPORT.md)
- [iOS release validation](IOS_RELEASE_VALIDATION.md)
- [Packaging overview](../packaging/README.md)
- [Changelog](../CHANGELOG.md)

Current automated release artifact families include Desktop (`win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64`), Browser/WebAssembly, and a signed Android AAB when signing secrets are configured. iOS simulator validation is maintained separately from signed App Store distribution.

Desktop/Browser and signed Android release assets receive deterministic CycloneDX 1.7 SBOMs where configured. The publication job validates unique/reserved release filenames, generates `SHA256SUMS.txt` with the flat basenames users actually download, then creates provenance attestations for the prepared `release-assets/**/*` tree before uploading intended GitHub Release assets.

After downloading release assets into one directory, GNU/coreutils-compatible systems can verify the checksum manifest with:

```bash
sha256sum -c SHA256SUMS.txt
```

See [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md) for checksum, SBOM, and attestation verification details.

## Completion and audit records

These records describe completed source passes/checkpoints and should not override newer authoritative files such as `PROJECT_STATE.md`:

- [2.9.0 release checkpoint — 2026-08-24](releases/2.9.0.md)
- [Documentation consistency audit — 2026-08-20](DOCUMENTATION_AUDIT_2026-08-20.md)
- [Final source audit — 2026-08-19](FINAL_SOURCE_AUDIT_2026-08-19.md)
- [Release source checkpoint — 2026-08-19](RELEASE_SOURCE_CHECKPOINT_2026-08-19.md)
- [Continuation checkpoint — 2026-08-19](CONTINUATION_CHECKPOINT_2026-08-19.md)
- [Continuation pass 2 — 2026-08-19](CONTINUATION_2026-08-19_PASS2.md)
- [What changed](../what_changed.md)
- [`docs/history/`](history/) — archived historical records.

When a historical statement differs from a current authoritative guide, use the current guide.

## Community and maintenance

- [Contributing](../CONTRIBUTING.md)
- [Support](../SUPPORT.md)
- [Security reporting](../SECURITY.md)
- [Code of conduct](../CODE_OF_CONDUCT.md)

## Version summary

- Product/display version: `2.9.5`
- Package version: `2.9.5`
- Normalized release tag: `v2.9.5`
- Assembly/file version: `2.9.5.0`
- Android/iOS display version: `2.9.5`
- Android/iOS numeric build code: `20905`
- Application id: `in.sanskar.calcnova`

## Documentation source-of-truth rules

Use these files when resolving conflicting wording:

1. `PROJECT_STATE.md` for completion status;
2. `Directory.Build.props`, `tools/release_identity.py`, and [VERSIONING.md](VERSIONING.md) for release version identity and repository-level NuGet audit policy;
3. actual `src/CalcNova.*` project files for target frameworks/platform metadata;
4. `.github/workflows/build-*.yml` and `.github/workflows/release.yml` for automated build/release commands;
5. `.github/workflows/codeql.yml`, `.github/workflows/dependency-review.yml`, [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md), and [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md) for automated security/supply-chain behavior;
6. [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md) for platform source status;
7. [BUILDING.md](BUILDING.md) for developer build instructions and restore-level dependency auditing;
8. [RELEASE.md](RELEASE.md) for release publication behavior;
9. [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md) for evidence-state semantics.

When code or a workflow changes, update the corresponding documentation in the same maintenance change.

## Evidence note

A runtime/platform/network/service check is recorded as PASS only when it actually runs and its result is observed. `NOT RUN` or `BLOCKED` describes verification evidence in a particular environment; it does not mean the completed 2.9.5 implementation is unfinished.

That distinction applies to CodeQL, Dependency Review, online NuGet vulnerability queries, provenance generation, signing, packaging, device/browser execution, and store processing.

Use the vocabulary:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

## External project links

- Repository: https://github.com/sanskarIN/CalcNova
- GitHub profile: https://www.github.com/sanskarIN
- Buy Me a Coffee: https://buymeacoffee.com/sanskarIN
