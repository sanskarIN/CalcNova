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
5. calculator keyboard-input contracts;
6. source-level accessibility markup contracts;
7. localization catalog and preference contracts;
8. onboarding persistence/visual/focus contracts;
9. cross-platform packaging metadata contracts;
10. release-tag validator unit tests;
11. optional validation of the requested release tag.

Each underlying validator remains independently runnable. The integrated command exists to catch interactions between contracts and give maintainers one reproducible preflight entry point.

## CI

`.github/workflows/source-preflight.yml` runs the integrated command for relevant pushes and pull requests and also supports manual dispatch.

Specialized workflows remain in place because they provide narrower failure signals and path filtering. The integrated workflow is an additional cross-contract gate, not a replacement for focused checks.

## What this does not prove

A successful source preflight does **not** mean CalcNova compiled or ran successfully. It does not install or invoke:

- the .NET SDK;
- Avalonia compilation;
- Android/iOS workloads;
- WebAssembly tooling;
- Windows/macOS/Linux packaging tools;
- signing/notarization tools;
- screen readers or accessibility inspection tools.

Full release evidence still requires the build/test/platform checks documented in [RELEASE.md](RELEASE.md), [TESTING.md](TESTING.md), and [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md).

If an environment cannot run a required check, record it as `NOT RUN` instead of treating source presence as a pass.

## Failure behavior

The preflight runs every configured source check so one invocation can surface multiple independent problems. It exits non-zero if any check fails.

Fix the first concrete failures, rerun the command, and then continue to the .NET/platform validation layer.
