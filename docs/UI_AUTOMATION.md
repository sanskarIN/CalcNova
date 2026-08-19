# CalcNova UI Automation

CalcNova now includes a focused Avalonia headless integration-test foundation for shared-shell behavior that is difficult to validate with pure view-model tests alone.

## Test stack

The App test project uses the repository-matched `Avalonia.Headless.XUnit` package and xUnit v3. The central package version is kept equal to the repository Avalonia version so the headless platform and application controls use the same Avalonia release family.

`tests/CalcNova.App.Tests/TestAppBuilder.cs` configures the CalcNova `App` through Avalonia's headless platform and registers it as the test application.

## Current headless scenarios

`MainViewHeadlessTests` currently covers:

1. the shared shell loads the expected number of primary mode tabs;
2. the Calculator `AC` control exposes and executes its real bound command;
3. a compact-width window applies the shared `compact` adaptive class;
4. `Ctrl+PageDown` advances shared mode selection through the real shell keyboard route;
5. enabling CalcNova high contrast applies the shared `high-contrast` shell class;
6. first-run onboarding is visible for a new/default settings state and becomes hidden after Skip.

`GraphPlotControlHeadlessTests` currently covers:

1. arrow-key panning changes the exposed graph viewport deterministically;
2. numpad Add/Subtract keyboard zoom changes viewport span;
3. Home resets the viewport to the documented default;
4. `F` fits the viewport around finite sampled data.

`GraphPlotControl.Viewport` is exposed as a read-only snapshot specifically so interaction behavior can be asserted without reading private rendering state.

These tests supplement existing view-model/domain tests. They intentionally do not attempt to emulate screen readers, real touch input, platform clipboard permission prompts, GPU rendering, mobile layout engines, or native package lifecycles.

## CI

`.github/workflows/headless-ui-validate.yml` provides a dedicated signal:

- runs the SDK-independent headless-test source validator;
- runs the Python validator regression tests;
- installs .NET 10;
- restores `CalcNova.App.Tests`;
- runs the App test project in Release configuration, including the Avalonia headless scenarios.

The source validator also protects these workflow commands so the real `.NET` execution path cannot be removed silently.

The normal solution-level build/test and release gates include `CalcNova.App.Tests`, so validated solution test runs are expected to execute the same headless scenarios as part of the App test project.

## Source contract validation

`tools/validate_headless_ui_tests.py` protects:

- Avalonia/headless package-version alignment;
- required App test project references;
- headless application bootstrap markers;
- presence of the current shared-shell and graph scenarios;
- use of `AvaloniaFact` for headless tests;
- representative real-control and keyboard interaction markers;
- graph read-only viewport assertions;
- inclusion of `CalcNova.App.Tests` in `CalcNova.slnx`;
- the dedicated .NET 10 restore/test workflow path.

`tools/tests/test_validate_headless_ui_tests.py` regression-tests this validator. The SDK-independent release preflight includes both checks, but it does **not** execute the .NET headless tests itself.

## Validation boundary

The headless source/tests are implemented, but this continuation environment does not provide the .NET SDK. Therefore the new headless suite is **NOT RUN locally** here and must not be described as passing until a real CI or suitable local execution result is observed.

Headless UI tests also do not replace the runtime evidence in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md). Desktop, Browser, Android, and iOS focus, screen-reader, clipboard, text-scaling, adaptive-layout, and packaging behavior still require their actual target environments.

## Expansion priorities

After the initial suite is observed passing, expand it in small stable increments around:

- Ctrl+PageUp/Home/End navigation in addition to the current PageDown scenario;
- focus restoration after onboarding dismissal;
- reduced-motion style-class application;
- converter search/favorite/clear-recents workflows;
- programmer representative bit-cell interaction;
- history search/export preview behavior;
- localized-string binding refresh once visible XAML localization migration begins.

Keep high-cost or platform-dependent behavior in target-specific tests rather than forcing it into a synthetic headless environment.
