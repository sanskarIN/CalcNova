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
- engineering-notation formatting/parsing and finite exponent bounds;
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
- exact-tag unsigned iOS simulator release-workflow contracts;
- tag-first release workflow contracts;
- release documentation/evidence contracts;
- artifact-manifest and SHA-256 integrity infrastructure;
- machine-readable release-evidence model, runner, verifier, and infrastructure.

### Regression inventory

The preflight also runs the Python regression suites for the focused validators and release tooling, including:

- release workflow/documentation/iOS workflow validators;
- headless UI, keyboard, selection, graph, numerical, Unicode, rational, engineering, export, statistics, localization, settings, adaptive, accessibility, packaging, and platform validators;
- artifact manifest generation/verification/integrity tooling;
- structured release-evidence model/runner/verifier/infrastructure;
- the integrated preflight inventory itself.

The optional `--tag` argument additionally invokes the release-tag validator against the requested tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible SDK-independent entry point.

## CI

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and supports manual dispatch.

Specialized workflows remain in place because they provide narrower failure signals and path filtering. Current focused gates include keyboard/calculator editing, graph interaction/presentation/numerical budgets, Unicode metadata, exact rationals, engineering notation, bivariate statistics, bounded exports, headless UI setup/execution, focus/accessibility/adaptive/touch contracts, localization, settings/converter preferences, packaging/platform workflows, dynamic controls accessibility, iOS release-tag validation, artifact integrity, structured release evidence, and release workflow/documentation contracts.

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
