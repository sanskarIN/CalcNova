# CalcNova Runtime Validation Runbook

Use this runbook only from a trusted checkout of the exact commit/tag being evaluated. Record the environment and the observed result for every command. Do not convert an unavailable or unobserved check into PASS.

## Evidence vocabulary

Use only:

- `PASS` — the check actually ran and the expected behavior was observed;
- `FAIL` — the check actually ran and did not satisfy the requirement;
- `BLOCKED` — execution started or was attempted but a concrete external/tooling blocker prevented completion;
- `NOT RUN` — the check was not executed.

A workflow definition, source validator, test file, or package template is not runtime evidence by itself.

## 1. Exact source identity

Record:

```text
Commit SHA:
Branch/tag:
Repository URL:
Validation date:
Validator/operator:
```

For release validation, use the exact release tag and verify it resolves to the expected commit before testing artifacts.

## 2. SDK-independent source preflight

From the repository root:

```bash
python tools/release_preflight.py
```

For a release tag:

```bash
python tools/release_preflight.py --tag v0.1.0
```

Record the Python version and full command result.

A source-preflight PASS is a prerequisite for the compiled gate; it does not replace compilation.

## 3. .NET solution gate

Record the installed SDK:

```bash
dotnet --info
```

Then run:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Do not skip the formatter/analyzer/build step merely because unit tests run.

Capture:

```text
Restore: PASS / FAIL / BLOCKED / NOT RUN
Format: PASS / FAIL / BLOCKED / NOT RUN
Build: PASS / FAIL / BLOCKED / NOT RUN
Tests: PASS / FAIL / BLOCKED / NOT RUN
```

Any compiler warning promoted to error, analyzer failure, XAML compile failure, or test failure is release-blocking until triaged.

## 4. Focused Avalonia headless UI gate

Run the App test project directly when diagnosing shared UI/headless behavior:

```bash
dotnet restore tests/CalcNova.App.Tests/CalcNova.App.Tests.csproj
dotnet test tests/CalcNova.App.Tests/CalcNova.App.Tests.csproj --configuration Release --no-restore
```

Current headless scenarios cover shared shell loading, Calculator command binding/selection editing, compact layout state, keyboard mode navigation, high contrast, onboarding, and graph keyboard viewport behavior.

Record the exact failing test name and exception if any scenario fails.

## 5. Desktop validation

### Windows

Run/build the Desktop head and verify:

- launch;
- standard/scientific calculation;
- calculator keyboard/numpad behavior;
- clipboard paste/copy;
- history/settings persistence across restart;
- window resize across compact/medium/expanded widths;
- high-DPI/text scaling;
- keyboard focus visibility;
- long graph/history/programmer surfaces;
- release publish/package installation if Windows packaging is part of the candidate.

### Linux

Verify the same shared workflows plus:

- representative supported distribution/runtime dependencies;
- clipboard integration;
- `.desktop`/AppStream metadata where packaged;
- chosen distributable package behavior.

### macOS

Verify the same shared workflows plus:

- launch on intended architecture;
- clipboard/persistence;
- keyboard conventions;
- bundle metadata;
- signing/notarization only when the appropriate external credentials/tooling are available.

Record Windows, Linux, and macOS independently. Success on one Desktop OS is not evidence for the others.

## 6. Browser/WebAssembly validation

Run the Browser publish path with the required WebAssembly workload and verify in every browser claimed by the release:

- application loads from the intended base path;
- calculations work after startup;
- local history/settings survive refresh/restart as designed;
- unsupported/corrupt future settings schemas do not get silently overwritten;
- sanitized clipboard paste and explicit copy behavior;
- Ctrl+PageUp/PageDown/Home/End conflicts;
- graph keyboard shortcut conflicts;
- browser zoom and large-text behavior;
- offline/cached behavior if the release claims it.

Do not infer Browser storage behavior from native JSON repository tests.

## 7. Android validation

On a supported Android emulator/device, verify:

- Release build succeeds with the required workload/toolchain;
- application launches;
- portrait and landscape layouts;
- representative phone/tablet widths;
- 64/128-bit programmer grid usability;
- calculator typing/keypad behavior;
- clipboard behavior;
- history/settings persistence after process restart;
- large text/font scaling;
- TalkBack traversal and labels;
- touch target usability;
- high contrast/reduced motion behavior;
- signed AAB only when external signing secrets are configured;
- store pre-launch/report checks when preparing publication.

Never place signing passwords/private keys in repository files or logs.

## 8. iOS validation

The tag-time simulator workflow can validate unsigned simulator compilation for the exact release tag. For runtime/release evidence also verify, as applicable:

- simulator launch;
- physical-device launch;
- portrait/landscape/safe-area behavior;
- Dynamic Type;
- VoiceOver traversal;
- external keyboard behavior;
- clipboard/persistence;
- icons/launch presentation;
- signing/provisioning;
- archive/export;
- TestFlight/App Store processing.

Simulator compilation alone is not App Store readiness.

## 9. Accessibility matrix

Use `docs/ACCESSIBILITY_TEST_MATRIX.md` as the result ledger.

At minimum test representative workflows with:

- keyboard only on keyboard targets;
- available screen readers on supported platforms;
- large text/text scaling;
- light/dark themes;
- CalcNova high contrast and relevant system accessibility settings;
- reduced motion;
- narrow and landscape layouts;
- onboarding first run/Skip/Start/focus restoration;
- Calculator selection-aware editing;
- Programmer 64/128-bit grids;
- graph pointer and keyboard interaction;
- converter saved/search controls;
- clipboard workflows;
- history export preview.

Measure contrast where required; do not call visual inspection a measured contrast PASS.

## 10. Settings migration evidence

Test representative historical/current storage states:

- historical JSON with no `schemaVersion` property;
- explicit schema `0`;
- current schema;
- corrupt negative schema;
- unsupported future schema.

Verify preference preservation for representative culture, history, converter, onboarding, theme, and angle-unit values.

Repeat the relevant checks for native and Browser storage rather than assuming one storage implementation proves the other.

## 11. Release artifacts

For every artifact intended for publication:

- prove it came from the release tag;
- record build runner/toolchain;
- verify expected architecture/runtime identifier;
- verify checksum;
- smoke-test install/launch where applicable;
- verify package/application identifiers and version values;
- verify no debug-only configuration or signing secret is embedded;
- verify required notices/licenses are present.

## 12. Final release evidence block

Use a record similar to:

```text
Source preflight: PASS / FAIL / BLOCKED / NOT RUN
Restore: PASS / FAIL / BLOCKED / NOT RUN
Format/analyzers: PASS / FAIL / BLOCKED / NOT RUN
Build: PASS / FAIL / BLOCKED / NOT RUN
Tests: PASS / FAIL / BLOCKED / NOT RUN
Headless UI: PASS / FAIL / BLOCKED / NOT RUN
Windows: PASS / FAIL / BLOCKED / NOT RUN
Linux: PASS / FAIL / BLOCKED / NOT RUN
macOS: PASS / FAIL / BLOCKED / NOT RUN
Browser: PASS / FAIL / BLOCKED / NOT RUN
Android: PASS / FAIL / BLOCKED / NOT RUN
iOS simulator: PASS / FAIL / BLOCKED / NOT RUN
iOS device/archive: PASS / FAIL / BLOCKED / NOT RUN
Accessibility matrix: PASS / FAIL / BLOCKED / NOT RUN
Packaging/signing: PASS / FAIL / BLOCKED / NOT RUN
Security/privacy/docs review: PASS / FAIL / BLOCKED / NOT RUN
```

A stable tag/release should only be promoted after every release-required row has acceptable observed evidence and any known limitation is documented.
