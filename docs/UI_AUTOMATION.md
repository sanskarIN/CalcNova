# CalcNova 2.8.03 UI Automation

CalcNova includes focused Avalonia headless integration coverage for shared-application behavior that is difficult to validate with pure domain/view-model tests alone.

Headless automation is one test layer. It does not replace real Desktop, Browser, Android, or iOS runtime/accessibility/packaging evidence.

## Test stack

`tests/CalcNova.App.Tests` uses repository-matched `Avalonia.Headless.XUnit` with xUnit v3.

The central headless package version is kept aligned with the repository Avalonia version so the headless platform and application controls use the same release family.

`TestAppBuilder.cs` configures the CalcNova application through Avalonia's headless platform for tests that exercise real shared controls/views.

## Covered shared-shell scenarios

The current headless/source inventory protects shared-shell scenarios including:

- expected primary mode inventory;
- real Calculator command binding;
- Calculator selection-aware editing/function wrapping integration;
- compact adaptive-class application;
- keyboard mode navigation;
- high-contrast shell state;
- first-run onboarding visibility/dismissal;
- onboarding shortcut/focus-related contracts;
- reviewed live localization across protected shell surfaces;
- release/About identity presentation.

## Graph headless coverage

Graph control/application scenarios include:

- arrow-key panning;
- keyboard zoom through numpad Add/Subtract;
- Home viewport reset;
- `F` fit-to-data;
- exposed read-only viewport assertions;
- multi-series presentation/legend integration;
- dynamic graph-control focus/touch-target contracts.

Headless graph coverage complements domain numerical/sampling tests; it does not attempt to prove GPU rendering quality or target screen-reader behavior.

## Supplemental feature-panel coverage

Focused headless tests also protect integration/presentation for implemented supplemental shared panels such as:

- Unicode metadata;
- exact-rational utility;
- engineering-notation utility and UI input bounds;
- paired/bivariate statistics.

These tests verify that feature panels remain attached to the intended shared mode and expose expected application bindings/interactions without requiring a full native platform host.

## Other application regression coverage

`CalcNova.App.Tests` also contains non-headless view-model/application tests for areas including:

- calculator session/clipboard/editing workflows;
- converter search/defaults/persistence/productivity behavior;
- programmer/Unicode workflows;
- statistics/equation/matrix/graph/date/currency behavior;
- history/export preview behavior;
- settings/schema/preferences;
- onboarding;
- localization;
- accessibility/adaptive behavior;
- release identity.

The exact file/test inventory remains the source of truth as maintenance evolves.

## CI

`.github/workflows/headless-ui-validate.yml` provides a dedicated headless signal that:

- runs SDK-independent headless source-contract validation;
- runs validator regression tests;
- installs .NET 10;
- restores `CalcNova.App.Tests`;
- executes the App test project in Release configuration.

The normal solution-level test path also includes `CalcNova.App.Tests` through the maintained solution composition.

## Source-contract validation

`tools/validate_headless_ui_tests.py` protects deterministic infrastructure requirements such as:

- Avalonia/headless package alignment;
- App test project references/bootstrap;
- expected representative headless scenarios;
- correct headless test attributes;
- real-control/keyboard interaction markers;
- graph viewport assertions;
- solution inclusion;
- the dedicated .NET workflow execution path.

The validator has Python regression coverage and is included in:

```bash
python tools/release_preflight.py
```

The SDK-independent preflight validates the headless-test source/configuration contract; it does not itself execute the compiled Avalonia headless suite.

## Evidence boundary

A headless test file existing in the repository is not an observed PASS.

Likewise, an SDK-independent validator PASS proves source-contract structure, not compiled headless execution.

Record a compiled test result only after the corresponding `dotnet test`/CI run is observed:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

This evidence state is environment/run specific. Permanent feature documentation should not hard-code one assistant/local environment as the global status.

## What headless UI does not prove

Headless tests intentionally do not emulate or replace:

- Narrator/NVDA/VoiceOver/TalkBack/browser screen readers;
- real touch input and gesture timing;
- platform clipboard permission prompts;
- GPU/graphics-driver rendering;
- system font/text scaling behavior;
- native application lifecycle;
- Android/iOS package signing/provisioning;
- Windows/macOS/Linux installer/package behavior;
- App Store/Play Store processing;
- browser hosting/CSP/service-worker behavior.

Those belong in target-platform/runtime validation.

## Maintenance expansion rule

Future headless tests should be added in small deterministic increments when they provide stable value for shared-control behavior.

Good candidates are interactions that:

- use shared Avalonia controls;
- have deterministic state transitions;
- do not require unavailable native services;
- protect a previous bug or important binding/focus/navigation contract.

High-cost or target-specific behavior should remain in platform runtime/manual/target automation instead of being forced into a synthetic headless environment.

## Related documentation

- [Testing](TESTING.md)
- [Accessibility](ACCESSIBILITY.md)
- [Adaptive layout](ADAPTIVE_LAYOUT.md)
- [Runtime validation runbook](RUNTIME_VALIDATION_RUNBOOK.md)
- [Validation evidence](VALIDATION_EVIDENCE.md)
- [Source preflight](SOURCE_PREFLIGHT.md)

## 2.8.03 classification

- Avalonia headless test infrastructure: **COMPLETE**;
- shared-shell representative scenarios: **COMPLETE**;
- graph keyboard/viewport scenarios: **COMPLETE**;
- supplemental feature-panel scenarios: **COMPLETE**;
- CI execution path/source contract: **COMPLETE**;
- target-platform runtime/accessibility evidence: **SEPARATE OBSERVED EVIDENCE**.
