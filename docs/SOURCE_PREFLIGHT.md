# CalcNova SDK-Independent Source Preflight

CalcNova includes a deterministic source-level validation command for environments where the .NET SDK or target-platform workloads are unavailable.

## Run it

From the repository root:

```bash
python tools/release_preflight.py
```

To include release-tag syntax validation:

```bash
python tools/release_preflight.py --tag v0.1.0
```

## What it runs

The integrated preflight is intentionally broader than any one focused workflow. Its current source-contract inventory covers:

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

### Settings, platform, and release infrastructure

- converter default-pair and preference-notice contracts;
- versioned settings schema/shared codec/shared validation;
- onboarding persistence/visual/focus behavior;
- cross-platform packaging metadata;
- Desktop/Browser/Android/iOS build-workflow contracts;
- the Source Preflight workflow's own trigger/least-privilege/execution contract;
- exact-tag unsigned iOS simulator release-workflow contracts;
- tag-first release workflow contracts;
- release documentation/evidence contracts;
- artifact-manifest and SHA-256 integrity infrastructure;
- machine-readable release-evidence model, runner, verifier, and infrastructure.

### Regression inventory

The preflight also runs the Python regression suites for the focused validators and release tooling, including:

- release workflow/documentation/iOS workflow validators;
- Source Preflight workflow validation;
- headless UI, keyboard, selection, graph, numerical, Unicode, rational, engineering, export, statistics, localization, settings, adaptive, accessibility, packaging, and platform validators;
- artifact manifest generation/verification/integrity tooling;
- structured release-evidence model/runner/verifier/infrastructure;
- the integrated preflight inventory itself.

The optional `--tag` argument additionally invokes the release-tag validator against the requested tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible SDK-independent entry point.

## Source Preflight workflow trigger contract

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and supports manual dispatch.

The final source audit found that the earlier path filter watched only selected App/platform files even though the integrated preflight reads contracts across many domain libraries, tests, tools, docs, packaging files, and workflows. That could allow a future core-only change to bypass the unified gate.

The workflow now deliberately watches the broad repository surfaces the preflight can inspect:

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

Specialized workflows remain in place because they provide narrower failure signals and path filtering. Current focused gates include keyboard/calculator editing, graph interaction/presentation/numerical budgets, Unicode metadata, exact rationals, engineering notation, bivariate statistics, bounded exports, headless UI setup/execution, focus/accessibility/adaptive/touch contracts, localization, settings/converter preferences, packaging/platform workflows, dynamic controls accessibility, iOS release-tag validation, artifact integrity, structured release evidence, and release workflow/documentation contracts.

The engineering focused gate specifically watches its core formatter/tests, App view model/panel/tests, validator, and validator tests so its new input-budget contract cannot be changed through an unwatched App path.

The integrated workflow is an additional cross-contract gate, not a replacement for focused checks.

## Headless UI distinction

The SDK-independent preflight validates that headless UI testing is correctly configured and that the expected scenarios/workflow commands remain present.

It does **not** execute `Avalonia.Headless.XUnit` tests because that requires the .NET SDK. Real headless test execution occurs in `.github/workflows/headless-ui-validate.yml` and through normal solution-level `dotnet test` runs. See [UI_AUTOMATION.md](UI_AUTOMATION.md).

## Artifact integrity and structured evidence

Artifact integrity and release evidence are separate but complementary contracts:

- artifact tooling generates/verifies manifests with SHA-256 checks and repository/commit identity safeguards;
- structured release evidence records whether commands actually passed, failed, were blocked, or were not run;
- source validation verifies that those toolchains and their tests remain present and wired correctly.

See [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md) for the machine-readable evidence model.

## What this does not prove

A successful source preflight does **not** mean CalcNova compiled or ran successfully. It does not install or invoke:

- the .NET SDK;
- Avalonia compiled XAML/headless execution;
- Android/iOS workloads;
- WebAssembly tooling;
- Windows/macOS/Linux packaging tools;
- signing/notarization/provisioning tools;
- screen readers or accessibility inspection tools.

Full release evidence still requires the build/test/platform checks documented in [RELEASE.md](RELEASE.md), [TESTING.md](TESTING.md), [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md), [FOCUS_VISIBILITY.md](FOCUS_VISIBILITY.md), and [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).

Settings migration behavior is documented in [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md). Graph keyboard/pointer behavior is documented in [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md).

If an environment cannot run a required check, record it as `NOT RUN` or `BLOCKED` as appropriate instead of treating source presence as a pass.

## Failure behavior

The preflight runs every configured source check so one invocation can surface multiple independent problems. It exits non-zero if any check fails.

Fix concrete failures, rerun the command, and then continue to the .NET/platform validation layer. A source-level success is only one release-evidence layer.

## Final-audit execution note

The final current `main` tree was hardened through GitHub source/commit inspection, but the active assistant execution environment could not materialize the full repository locally and does not provide .NET 10.

Accordingly, this documentation does not claim that the final integrated preflight, compiled build, or compiled tests passed merely because the contracts are present. Observe CI or execute the commands in a suitable environment before recording PASS evidence for the release-candidate commit.
