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

The integrated preflight currently executes:

1. repository structure, structured-file, contact-link, and secret-file guards;
2. Avalonia XAML XML parsing;
3. shared XAML/view-model UI contracts;
4. mode-navigation contracts;
5. calculator and shared-shell keyboard-input contracts;
6. graph keyboard pan/zoom/reset/fit contracts;
7. source-level accessibility markup contracts;
8. shared and high-contrast focus-visibility contracts;
9. runtime-accessibility evidence/status-discipline contracts;
10. compact/medium/expanded adaptive-layout contracts;
11. shared touch-target contracts;
12. English/Hindi localization catalog and preference contracts;
13. versioned native/Browser settings-schema migration contracts;
14. onboarding persistence/visual/focus contracts;
15. cross-platform packaging metadata contracts;
16. release-documentation contracts;
17. release-tag validator unit tests;
18. focus-validator regression tests;
19. accessibility-evidence validator regression tests;
20. keyboard-validator regression tests;
21. graph-keyboard validator regression tests;
22. localization-validator regression tests;
23. settings-schema validator regression tests;
24. adaptive-layout validator regression tests;
25. touch-target validator regression tests;
26. packaging-metadata validator regression tests;
27. optional validation of the requested release tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible preflight entry point.

## CI

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and also supports manual dispatch.

Specialized workflows remain in place because they provide narrower failure signals and path filtering. Current focused gates include keyboard, graph keyboard, focus visibility, adaptive layout, touch targets, accessibility evidence, localization, settings schema, onboarding, and packaging metadata.

The integrated workflow is an additional cross-contract gate, not a replacement for focused checks.

## What this does not prove

A successful source preflight does **not** mean CalcNova compiled or ran successfully. It does not install or invoke:

- the .NET SDK;
- Avalonia compilation;
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
