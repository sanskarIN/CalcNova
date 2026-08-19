# CalcNova UI Automation

CalcNova now includes a focused Avalonia headless integration-test foundation for shared-shell behavior that is difficult to validate with pure view-model tests alone.

## Test stack

The App test project uses the repository-matched `Avalonia.Headless.XUnit` package and xUnit v3. The central package version is kept equal to the repository Avalonia version so the headless platform and application controls use the same Avalonia release family.

`tests/CalcNova.App.Tests/TestAppBuilder.cs` configures the CalcNova `App` through Avalonia's headless platform and registers it as the test application.

## Current headless scenarios

`MainViewHeadlessTests` currently covers:

1. the shared shell loads the expected number of primary mode tabs;
2. the calculator evaluate control executes a real bound command through a rendered headless window;
3. a compact-width window applies the shared `compact` adaptive class;
4. first-run onboarding is visible for a new/default settings state and becomes hidden after Skip.

These tests supplement existing view-model/domain tests. They intentionally do not attempt to emulate screen readers, real touch input, platform clipboard permission prompts, GPU rendering, mobile layout engines, or native package lifecycles.

## CI

`.github/workflows/headless-ui-validate.yml` provides a dedicated signal:

- runs the SDK-independent headless-test source validator;
- runs the Python validator regression tests;
- installs .NET 10;
- restores `CalcNova.App.Tests`;
- runs the App test project in Release configuration, including the Avalonia headless scenarios.

The normal solution-level release test gate also includes `CalcNova.App.Tests`, so validated releases are expected to execute the headless tests as part of the ordinary test project once the workflow reaches the .NET test step.

## Source contract validation

`tools/validate_headless_ui_tests.py` protects:

- Avalonia/headless package-version alignment;
- required App test project references;
- headless application bootstrap markers;
- presence of the core shared-shell scenarios;
- use of `AvaloniaFact` for headless tests;
- representative real-control interaction markers;
- inclusion of `CalcNova.App.Tests` in `CalcNova.slnx`.

`tools/tests/test_validate_headless_ui_tests.py` regression-tests this validator. The SDK-independent release preflight includes both checks, but it does **not** execute the .NET headless tests itself.

## Validation boundary

The headless source/tests are implemented, but this continuation environment does not provide the .NET SDK. Therefore the new headless suite is **NOT RUN locally** here and must not be described as passing until a real CI or suitable local execution result is observed.

Headless UI tests also do not replace the runtime evidence in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md). Desktop, Browser, Android, and iOS focus, screen-reader, clipboard, text-scaling, adaptive-layout, and packaging behavior still require their actual target environments.

## Expansion priorities

After the initial suite is observed passing, expand it in small stable increments around:

- Ctrl+PageUp/PageDown/Home/End mode navigation;
- focus restoration after onboarding dismissal;
- high-contrast style-class application;
- converter search/favorite/clear-recents workflows;
- programmer representative bit-cell interaction;
- graph focus and keyboard pan/zoom/reset/fit behavior;
- history search/export preview behavior.

Keep high-cost or platform-dependent behavior in target-specific tests rather than forcing it into a synthetic headless environment.
