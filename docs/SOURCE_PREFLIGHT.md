# CalcNova SDK-Independent Source Preflight

CalcNova includes a deterministic source-level validation command for environments where the .NET SDK or platform workloads are not available.

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

The integrated preflight currently executes these source-contract groups:

1. repository structure, structured-file, contact-link, and secret-file guards;
2. Avalonia XAML XML parsing;
3. shared XAML/view-model UI contracts;
4. mode-navigation contracts;
5. calculator and shared-shell keyboard-input contracts;
6. graph keyboard pan/zoom/reset/fit contracts;
7. Avalonia headless UI-test package/bootstrap/scenario/workflow contracts;
8. source-level accessibility markup contracts;
9. shared and high-contrast focus-visibility contracts;
10. runtime-accessibility evidence/status-discipline contracts;
11. compact/medium/expanded adaptive-layout contracts;
12. shared touch-target contracts;
13. English/Hindi localization catalog and preference contracts;
14. versioned native/Browser settings-schema migration contracts;
15. onboarding persistence/visual/focus contracts;
16. cross-platform packaging metadata contracts;
17. Desktop/Browser/Android/iOS build-workflow and shared SDK-policy contracts;
18. tag-first release-workflow contracts;
19. release-documentation evidence contracts;
20. release-tag validator tests;
21. regression tests for release workflow/documentation validators;
22. regression tests for the headless UI source validator;
23. regression tests for focus/accessibility-evidence validators;
24. regression tests for keyboard/graph-keyboard validators;
25. regression tests for localization/settings-schema validators;
26. regression tests for adaptive-layout/touch-target validators;
27. regression tests for packaging/platform-workflow validators;
28. regression tests for the integrated preflight inventory itself;
29. optional validation of the requested release tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible preflight entry point.

## CI

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and also supports manual dispatch.

Its path filters cover the App/UI sources, App/platform/persistence tests relevant to the contracts, central package/SDK policy, package metadata, validator tooling, validation documentation, platform build workflows, the headless UI workflow, and the release workflow.

Specialized workflows remain in place because they provide narrower failure signals and path filtering. Current focused gates include:

- keyboard navigation;
- graph keyboard interaction;
- Avalonia headless UI execution;
- focus visibility;
- adaptive layout;
- touch targets;
- accessibility evidence;
- localization;
- settings schema;
- onboarding;
- packaging metadata;
- platform workflow contracts;
- release workflow contracts.

The integrated workflow is an additional cross-contract gate, not a replacement for focused checks.

## Headless UI distinction

The SDK-independent preflight validates that headless UI testing is correctly configured and that the expected scenarios/workflow commands remain present.

It does **not** execute `Avalonia.Headless.XUnit` tests because that requires the .NET SDK. Real headless test execution occurs in `.github/workflows/headless-ui-validate.yml` and through normal solution-level `dotnet test` runs. See [UI_AUTOMATION.md](UI_AUTOMATION.md).

## What this does not prove

A successful source preflight does **not** mean CalcNova compiled or ran successfully. It does not install or invoke:

- the .NET SDK;
- Avalonia compiled XAML/headless execution;
- Android/iOS workloads;
- WebAssembly tooling;
- Windows/macOS/Linux packaging tools;
- signing/notarization tools;
- screen readers or accessibility inspection tools.

Full release evidence still requires the build/test/platform checks documented in [RELEASE.md](RELEASE.md), [TESTING.md](TESTING.md), [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md), [FOCUS_VISIBILITY.md](FOCUS_VISIBILITY.md), and [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md).

Settings migration behavior is documented in [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md). Graph keyboard/pointer behavior is documented in [GRAPH_INTERACTION.md](GRAPH_INTERACTION.md).

If an environment cannot run a required check, record it as `NOT RUN` instead of treating source presence as a pass.

## Failure behavior

The preflight runs every configured source check so one invocation can surface multiple independent problems. It exits non-zero if any check fails.

Fix the first concrete failures, rerun the command, and then continue to the .NET/platform validation layer.
