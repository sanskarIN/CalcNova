# CalcNova 2.8.03 SDK-Independent Source Preflight

CalcNova includes a deterministic source-level validation command for environments where the .NET SDK or target-platform workloads are unavailable.

## Run it

From the repository root:

```bash
python tools/release_preflight.py
```

For the completed 2.8.03 release identity, include the normalized SemVer tag when validating a tagged source tree:

```bash
python tools/release_preflight.py --tag v2.8.3
```

The public product version is `2.8.03`; strict SemVer tooling uses `2.8.3` / `v2.8.3` because leading zeroes are not allowed in numeric SemVer identifiers.

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

- CalcNova 2.8.03 authoritative completion-status contracts;
- public/display `2.8.03` and normalized package/tag `2.8.3` identity;
- in-app About release label `Version 2.8.03 • Complete`;
- converter default-pair and preference-notice contracts;
- versioned settings schema/shared codec/shared validation;
- onboarding persistence/visual/focus behavior;
- cross-platform packaging metadata;
- Desktop/Browser/Android/iOS build-workflow contracts;
- CodeQL/dependency-review security workflow contracts;
- the Source Preflight workflow's own trigger/least-privilege/execution contract;
- exact-tag unsigned iOS simulator release-workflow contracts;
- tag-first release workflow contracts;
- six-target x64/ARM64 desktop release publication contracts;
- release least-privilege and artifact-provenance contracts;
- release documentation/evidence contracts;
- artifact-manifest and SHA-256 integrity infrastructure;
- machine-readable release-evidence model, runner, verifier, and infrastructure.

### Regression inventory

The preflight also runs the Python regression suites for the focused validators and release tooling, including:

- release workflow/documentation/iOS workflow validators;
- Source Preflight workflow validation;
- security workflow validation;
- 2.8.03 completion-status validation;
- headless UI, keyboard, selection, graph, numerical, Unicode, rational, engineering, export, statistics, localization, settings, adaptive, accessibility, packaging, and platform validators;
- artifact manifest generation/verification/integrity tooling;
- structured release-evidence model/runner/verifier/infrastructure;
- the integrated preflight inventory itself.

The optional `--tag` argument additionally invokes the release-tag validator against the requested normalized SemVer tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible SDK-independent entry point.

## 2.8.03 completion-status contract

The completed product status is itself protected by:

```bash
python tools/validate_completion_status.py .
python -m unittest tools.tests.test_validate_completion_status
```

That validator requires the authoritative current-facing documents to identify CalcNova 2.8.03 as complete and rejects obsolete current-status markers such as:

- `under active development`;
- an `Unreleased` top-level release posture;
- planned first-milestone wording;
- `remaining product/runtime work`;
- `remaining high-priority work`;
- `remaining work is evidence-dependent`.

It also verifies that the About model and shared shell expose the completed `2.8.03` release label and that regression source protects it.

Historical source-audit documents under `docs/history/` are preserved as history and are not treated as the authoritative current status.

## Security automation source contract

The maintained security workflows are protected by:

```bash
python tools/validate_security_workflows.py .
python -m unittest tools.tests.test_validate_security_workflows
```

The source validator requires the intended CodeQL and Dependency Review contracts, including:

- CodeQL Action v4;
- C# analysis with source-analysis build mode;
- push/PR/schedule/manual CodeQL triggers;
- CodeQL's required `security-events: write` with no unnecessary repository write/OIDC grants;
- Dependency Review Action v5;
- `moderate` vulnerability severity enforcement;
- read-only dependency-review permission;
- rejection of `pull_request_target` for these workflows.

`.github/workflows/security-automation-validate.yml` runs the validator and its tests as a focused read-only workflow when the relevant security/preflight files change.

See [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md).

## Source Preflight workflow trigger contract

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and supports manual dispatch.

The workflow deliberately watches the broad repository surfaces the preflight can inspect:

- `src/**`;
- `tests/**`;
- `tools/**`;
- `docs/**`;
- `packaging/**`;
- `.github/workflows/**`;
- release/build root metadata such as the solution, SDK/package/build properties, README/changelog/project-state/checkpoint files, and `.gitignore`.

The workflow remains least-privilege with `contents: read` and runs the preflight on Ubuntu with the pinned Python setup used by the repository.

This workflow contract is itself protected by:

```bash
python tools/validate_source_preflight_workflow.py .
python -m unittest tools.tests.test_validate_source_preflight_workflow
```

Those checks are also part of `tools/release_preflight.py`, so narrowing the master gate or making it unnecessarily privileged becomes a source-preflight failure.

## Focused CI workflows

Specialized workflows remain in place because they provide narrower failure signals and path filtering. Focused gates cover keyboard/calculator editing, graph interaction/presentation/numerical budgets, Unicode metadata, exact rationals, engineering notation, bivariate statistics, bounded exports, headless UI setup/execution, focus/accessibility/adaptive/touch contracts, localization, settings/converter preferences, packaging/platform workflows, dynamic controls accessibility, security automation, iOS release-tag validation, artifact integrity, structured release evidence, and release workflow/documentation contracts.

The engineering focused gate watches its core formatter/tests, App view model/panel/tests, validator, and validator tests so its input-budget contract cannot be changed through an unwatched App path.

The integrated workflow is an additional cross-contract gate, not a replacement for focused checks.

## Headless UI distinction

The SDK-independent preflight validates that headless UI testing is correctly configured and that expected scenarios/workflow commands remain present.

It does **not** execute `Avalonia.Headless.XUnit` tests because that requires the .NET SDK. Actual headless execution occurs in `.github/workflows/headless-ui-validate.yml` and through normal solution-level `dotnet test` runs. See [UI_AUTOMATION.md](UI_AUTOMATION.md).

## Artifact integrity, provenance, and structured evidence

Artifact integrity, release provenance, and release evidence are separate but complementary contracts:

- artifact tooling generates/verifies manifests with SHA-256 checks and repository/commit identity safeguards;
- the stable release workflow generates GitHub provenance attestations for intended ZIP/AAB/checksum release files;
- `tools/validate_release_workflow.py` verifies the least-privilege permission and attestation source contract;
- structured release evidence records whether commands actually passed, failed, were blocked, or were not run;
- source validation verifies that those toolchains and their tests remain present and wired correctly.

See [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md) and [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md).

## Evidence boundary

A successful source preflight validates deterministic repository contracts. It does not itself execute or replace:

- the .NET SDK build/test layer;
- Avalonia compiled XAML/headless execution;
- Android/iOS workloads;
- WebAssembly tooling;
- Windows/macOS/Linux packaging tools;
- CodeQL's GitHub-hosted analysis service;
- GitHub dependency-review service evaluation;
- GitHub artifact-attestation execution;
- signing/notarization/provisioning tools;
- screen readers or accessibility inspection tools.

Those checks are external execution evidence. They are recorded only when actually run and observed.

An environment-specific `NOT RUN` or `BLOCKED` result does **not** change the completed implementation status of CalcNova 2.8.03; it only records whether that external verification operation executed in that environment.

See [RELEASE.md](RELEASE.md), [TESTING.md](TESTING.md), [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md), [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md), [ARTIFACT_PROVENANCE.md](ARTIFACT_PROVENANCE.md), [FOCUS_VISIBILITY.md](FOCUS_VISIBILITY.md), and [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).

## Failure behavior

The preflight runs every configured source check so one invocation can surface multiple independent problems. It exits non-zero if any check fails.

Fix concrete failures and rerun the command. Source-level success is one evidence layer; external compiled/platform/security-service evidence remains independently recorded.

## Current completion note

CalcNova 2.8.03 is the completed product baseline. The source preflight protects its source, documentation, security automation, release identity/provenance, and completion-status contracts against regression.
