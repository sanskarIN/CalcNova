# CalcNova 2.9.5 SDK-Independent Source Preflight

CalcNova includes a deterministic source-level validation command for environments where the .NET SDK or target-platform workloads are unavailable.

**CalcNova 2.9.5 is the completed product baseline.**

## Run it

From the repository root:

```bash
python tools/release_preflight.py
```

For the current release identity, include the exact release tag when validating a tagged source tree:

```bash
python tools/release_preflight.py --tag v2.9.5
```

The public/product version, strict SemVer package version, and normalized tag are `2.9.5`, `2.9.5`, and `v2.9.5`. Android/iOS use numeric build code `20905`.

The requested 2.9.0 checkpoint was prepared first with tag `v2.9.0` and build code `20900`; see [`releases/2.9.0.md`](releases/2.9.0.md).

## What it runs

The integrated preflight is intentionally broader than any one focused workflow. Its source-contract inventory covers:

### Repository and shared UI

- repository structure/security and structured-file checks;
- Avalonia XAML XML parsing;
- shared UI command/property contracts;
- mode-navigation contracts;
- calculator/shared-shell keyboard contracts, including printable operators;
- selection-aware calculator editing and function wrapping;
- graph keyboard interaction;
- graph surface integration;
- deterministic multi-series graph presentation.

### Numerical and data correctness

- graph numerical-analysis safety;
- graph numerical workload budgets;
- Unicode scalar metadata contracts;
- exact rational normalization, default-value safety, parsing, arithmetic, and workload budgets;
- engineering-notation formatting/parsing, 4,096-character text budget, finite exponent bounds, non-zero-underflow rejection, and shared UI input bounds;
- bounded export previews and full-content copy contracts;
- bivariate covariance/correlation/regression contracts.

### UI quality and accessibility

- Avalonia headless UI-test source/execution-path contracts;
- accessibility markup;
- focus visibility;
- accessibility runtime-evidence discipline;
- dynamically inserted shared-control accessibility and touch-target coverage;
- adaptive layout;
- shared touch-target baselines;
- English/Hindi localization catalog/preferences and reviewed live localization surfaces.

### Version, completion, settings, platform, security, and release infrastructure

- current release identity loaded from `Directory.Build.props`;
- current release completion-status contracts;
- in-app About release label `Version 2.9.5 • Complete`;
- converter default-pair and preference-notice contracts;
- versioned settings schema/shared codec/shared validation;
- onboarding persistence/visual/focus behavior;
- cross-platform packaging metadata;
- Desktop/Browser/Android/iOS build-workflow contracts;
- cross-platform source composition, Browser/PWA resources, and mobile architecture contracts;
- CodeQL/dependency-review/focused-security workflow contracts;
- repository-level NuGet dependency-security policy contracts;
- the Source Preflight workflow's own always-run PR/push, least-privilege, concurrency, and execution contract;
- exact-tag unsigned iOS simulator release-workflow contracts;
- tag-first release workflow contracts;
- six-target x64/ARM64 desktop release publication contracts;
- release filename/checksum/provenance/least-privilege contracts;
- release documentation/evidence contracts;
- artifact-manifest and SHA-256 integrity infrastructure;
- machine-readable release-evidence model, runner, verifier, and infrastructure.

### Regression inventory

The preflight also runs the Python regression suites for focused validators and release tooling, including:

- centralized release-identity parsing/build-code calculation;
- release workflow/documentation/iOS workflow validators;
- Source Preflight workflow validation;
- security workflow validation;
- NuGet dependency-security policy validation;
- current completion-status validation;
- platform-workflow and cross-platform-source validation;
- headless UI, keyboard, selection, graph, numerical, Unicode, rational, engineering, export, statistics, localization, settings, adaptive, accessibility, packaging, and platform validators;
- deterministic CycloneDX SBOM generation;
- artifact manifest generation/verification/integrity tooling;
- structured release-evidence model/runner/verifier/infrastructure;
- the integrated preflight inventory itself.

The optional `--tag` argument additionally invokes the release-tag validator against the requested SemVer tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible SDK-independent entry point.

## Release identity contract

The current release identity is protected by:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_packaging_metadata.py .
python tools/validate_completion_status.py .
```

`tools/release_identity.py` parses `Directory.Build.props` and requires:

- display version `2.9.5` to normalize to package version `2.9.5`;
- `VersionPrefix` and `PackageVersion` to match `Version`;
- assembly/file versions to match `2.9.5.0`;
- informational version to match the display version;
- release tag to derive as `v2.9.5`;
- mobile build code to derive as `20905` from `MAJOR * 10000 + MINOR * 100 + PATCH`.

This removes the earlier maintenance risk where validators could remain pinned to an old release after source workflows or version metadata advanced.

## Current completion-status contract

The completed current release status is protected by:

```bash
python tools/validate_completion_status.py .
python -m unittest tools.tests.test_validate_completion_status
```

The validator derives its version/tag/build-code expectations from the central release identity and requires authoritative current-facing documents plus the in-app About surface/tests to identify the current completed baseline consistently.

It rejects obsolete provisional-status wording such as:

- `under active development`;
- an `Unreleased` top-level release posture;
- planned first-milestone wording;
- `remaining product/runtime work`;
- `remaining high-priority work`;
- `remaining work is evidence-dependent`.

Historical source-audit/release-checkpoint documents are preserved as history and are not treated as the authoritative current status.

## Cross-platform source contract

The maintained cross-platform source matrix is protected by:

```bash
python tools/validate_platform_support.py .
python -m unittest tools.tests.test_validate_platform_support
python tools/validate_platform_workflows.py .
python -m unittest tools.tests.test_validate_platform_workflows
```

The platform-source validator protects Desktop, Browser/PWA, Android, iOS, shared platform abstractions, platform service composition, Browser storage/PWA resources, and explicit mobile runtime identifiers.

The workflow validator separately protects .NET/platform workload setup, `actions/checkout@v7`, Java 17 for Android, the Desktop OS runner matrix, and read-only validation permissions.

## Security automation source contract

The maintained security workflows are protected by:

```bash
python tools/validate_security_workflows.py .
python -m unittest tools.tests.test_validate_security_workflows
```

The source validator requires the intended CodeQL, Dependency Review, and focused security-validation contracts, including:

- CodeQL Action v4;
- C# analysis with source-analysis build mode;
- push/PR/schedule/manual CodeQL triggers;
- CodeQL's required `security-events: write` with no unnecessary repository write/OIDC grants;
- Dependency Review Action v5;
- `moderate` vulnerability severity enforcement;
- read-only dependency-review permission;
- focused workflow read-only permission;
- `Directory.Build.props` watch coverage on push and pull request;
- execution of both security validators and their regression suites;
- rejection of `pull_request_target` for these workflows.

See [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md).

## NuGet dependency-security source contract

Repository-level NuGet vulnerability policy is protected independently by:

```bash
python tools/validate_dependency_security.py .
python -m unittest tools.tests.test_validate_dependency_security
```

The validator requires `Directory.Build.props` to keep:

```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
<NuGetAudit>true</NuGetAudit>
<NuGetAuditMode>all</NuGetAuditMode>
<NuGetAuditLevel>moderate</NuGetAuditLevel>
```

This ensures CalcNova explicitly audits direct and transitive packages and keeps the moderate-or-higher threshold aligned with the dependency-review gate.

The SDK-independent validator only verifies policy source. Actual online vulnerability lookup occurs when a .NET restore executes with available audit sources.

## Source Preflight workflow trigger contract

`.github/workflows/source-preflight.yml` runs on every push to `main` and every pull request targeting `main`, plus manual dispatch.

It intentionally has no `paths` or `paths-ignore` filters. This makes `Source Preflight / source-preflight` suitable for use as a required branch-protection check: a documentation-only, metadata-only, or unusual pull request cannot omit the check merely because its files fall outside a path list.

The workflow remains least-privilege:

```yaml
permissions:
  contents: read
```

It also cancels superseded runs through a workflow/ref concurrency group.

This always-run workflow contract is protected by:

```bash
python tools/validate_source_preflight_workflow.py .
python -m unittest tools.tests.test_validate_source_preflight_workflow
```

## Branch-protection readiness

The always-run Source Preflight is a source prerequisite for reliable branch protection, but branch protection itself is a GitHub repository setting rather than a source file.

See [BRANCH_PROTECTION.md](BRANCH_PROTECTION.md) for the observed repository state, recommended required checks, and the external setting that still needs to be enabled if it remains disabled.

## Focused CI workflows

Specialized workflows remain because they provide narrower failure signals and useful path filtering. Focused gates cover keyboard/calculator editing, graph interaction/presentation/numerical budgets, Unicode metadata, exact rationals, engineering notation, bivariate statistics, bounded exports, headless UI setup/execution, focus/accessibility/adaptive/touch contracts, localization, settings/converter preferences, packaging/platform workflows, cross-platform source composition, dynamic controls accessibility, security automation/dependency policy, iOS release-tag validation, artifact integrity, structured release evidence, CI hygiene, and release workflow/documentation contracts.

The integrated Source Preflight is intentionally always present on pull requests so it can act as a stable required policy check.

## Headless UI distinction

The SDK-independent preflight validates that headless UI testing is correctly configured and that expected scenarios/workflow commands remain present.

It does not execute `Avalonia.Headless.XUnit` tests because that requires the .NET SDK. Actual headless execution occurs in the headless UI workflow and through normal solution-level `dotnet test` runs.

See [UI_AUTOMATION.md](UI_AUTOMATION.md).

## Artifact integrity, provenance, SBOMs, and structured evidence

Artifact integrity, release provenance, SBOM generation, and release evidence are separate but complementary contracts:

- deterministic CycloneDX 1.7 SBOM tooling records restored target dependency graphs;
- artifact tooling generates/verifies manifests with SHA-256 checks and repository/commit identity safeguards;
- the stable release workflow validates flat release filenames and generates a download-friendly basename checksum manifest;
- the stable release workflow generates GitHub provenance attestations for the prepared `release-assets/**/*` tree;
- source validation verifies filename guards, flat checksum behavior, job-scoped publication permissions, attestation action/subject/order, SBOM generation, and publication ordering;
- structured release evidence records whether commands actually passed, failed, were blocked, or were not run.

See [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md) and [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md).

## Evidence boundary

A successful source preflight validates deterministic repository contracts. It does not itself execute or replace:

- the .NET SDK build/test layer;
- online NuGet advisory queries performed by restore;
- Avalonia compiled XAML/headless execution;
- Android/iOS workloads;
- WebAssembly tooling;
- Windows/macOS/Linux package execution;
- physical-device or representative-browser testing;
- CodeQL's GitHub-hosted analysis service;
- GitHub dependency-review service evaluation;
- GitHub branch-protection/ruleset enforcement;
- GitHub artifact-attestation execution;
- signing/notarization/provisioning tools;
- TestFlight/App Store/Play Console processing;
- screen readers or accessibility inspection tools.

Those checks are external execution/settings evidence. They are recorded only when actually run, observed, or enabled.

An environment-specific `NOT RUN` or `BLOCKED` result does not change the completed implementation status of CalcNova 2.9.5; it only records whether that external verification operation executed in that environment.

See [RELEASE.md](RELEASE.md), [TESTING.md](TESTING.md), [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md), [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md), [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md), [BRANCH_PROTECTION.md](BRANCH_PROTECTION.md), [FOCUS_VISIBILITY.md](FOCUS_VISIBILITY.md), and [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).
