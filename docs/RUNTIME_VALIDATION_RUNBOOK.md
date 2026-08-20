# CalcNova 2.8.03 Runtime Validation Runbook

Use this runbook only from a trusted checkout of the exact commit/tag being evaluated. Record the environment and the observed result for every command. Do not convert an unavailable or unobserved check into PASS.

For platform prerequisites and exact project commands, use [BUILDING.md](BUILDING.md). For the release process, use [RELEASE.md](RELEASE.md).

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
Product version: 2.8.03
Normalized package version: 2.8.3
Normalized release tag: v2.8.3
Commit SHA:
Branch/tag:
Repository URL:
Validation date:
Validator/operator:
```

For release validation, use the exact normalized release tag `v2.8.3` and verify that it resolves to the expected commit before testing artifacts.

## 2. SDK-independent source preflight

From the repository root:

```bash
python tools/release_preflight.py
```

For the CalcNova 2.8.03 release tag:

```bash
python tools/release_preflight.py --tag v2.8.3
```

Record the Python version and full command result.

A source-preflight PASS is a prerequisite for the compiled gate; it does not replace compilation.

## 3. .NET solution gate

Record the installed SDK:

```bash
dotnet --info
dotnet workload list
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

Current headless scenarios cover shared shell loading, Calculator command binding/selection editing, compact layout state, keyboard mode navigation, high contrast, onboarding, graph keyboard viewport behavior, and other shared-control regression contracts documented in [UI_AUTOMATION.md](UI_AUTOMATION.md).

Record the exact failing test name and exception if any scenario fails.

## 5. Desktop validation

The maintained Desktop head is `src/CalcNova.Desktop`.

The current automated build matrix runs on Ubuntu, Windows, and macOS. The release workflow publishes self-contained artifacts for `win-x64`, `linux-x64`, and `osx-x64`.

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
- `win-x64` release publish;
- Appx/MSIX packaging/install behavior if that packaging path is part of the candidate.

### Linux

Verify the same shared workflows plus:

- `linux-x64` release publish;
- representative target distribution/runtime dependencies;
- clipboard integration;
- `.desktop`/AppStream metadata where packaged;
- chosen distributable package behavior.

### macOS

Verify the same shared workflows plus:

- `osx-x64` release publish for the current automated release artifact;
- launch on the intended test architecture;
- clipboard/persistence;
- keyboard conventions;
- bundle metadata;
- signing/notarization only when the appropriate external credentials/tooling are available.

Record Windows, Linux, and macOS independently. Success on one Desktop OS is not evidence for the others.

## 6. Browser/WebAssembly validation

The maintained Browser head is `src/CalcNova.Browser` and requires the `wasm-tools` workload.

Use the build path documented in [BUILDING.md](BUILDING.md), then verify in every browser claimed by the release:

- application loads from the intended base path;
- calculations work after startup;
- local history/settings survive refresh/restart as designed;
- unsupported/corrupt future settings schemas do not get silently overwritten;
- sanitized clipboard paste and explicit copy behavior;
- Ctrl+PageUp/PageDown/Home/End conflicts;
- graph keyboard shortcut conflicts;
- browser zoom and large-text behavior;
- optional currency network/offline behavior;
- offline/cached behavior only if the deployment claims it.

Do not infer Browser storage behavior from native JSON repository tests.

## 7. Android validation

The maintained Android head is `src/CalcNova.Android`, targets `net10.0-android`, uses minimum API 23, and CI uses JDK 17.

On a supported Android emulator/device, verify:

- Android workload/toolchain restore/build succeeds;
- application id is `in.sanskar.calcnova`;
- display version is `2.8.03`;
- numeric build code is `20803`;
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

A normal build and a production-signed AAB are separate evidence rows.

Never place signing passwords/private keys in repository files or logs.

## 8. iOS validation

The maintained iOS head is `src/CalcNova.iOS`, targets `net10.0-ios`, and declares minimum iOS platform version 15.0.

The normal simulator build workflow chooses:

- `iossimulator-arm64` on Apple Silicon runners;
- `iossimulator-x64` on Intel runners.

The exact-tag simulator workflow can validate unsigned simulator compilation for the selected release tag. For runtime/release evidence also verify, as applicable:

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

Simulator compilation alone is not App Store readiness. See [IOS_RELEASE_VALIDATION.md](IOS_RELEASE_VALIDATION.md).

## 9. Accessibility matrix

Use [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md) as the result ledger.

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

- prove it came from `v2.8.3` or the exact maintenance tag being evaluated;
- record build runner/toolchain;
- verify expected architecture/runtime identifier;
- verify checksum;
- smoke-test install/launch where applicable;
- verify package/application identifiers and version values;
- verify no debug-only configuration or signing secret is embedded;
- verify required notices/licenses are present.

For the 2.8.03 automated release workflow, expected artifact families are:

- Windows `win-x64` desktop ZIP;
- Linux `linux-x64` desktop ZIP;
- macOS `osx-x64` desktop ZIP;
- Browser/WebAssembly bundle;
- signed Android AAB when signing secrets are configured;
- generated SHA-256 checksum material.

iOS exact-tag simulator validation is a separate validation path and is not represented as a signed App Store artifact.

## 12. Security/privacy/documentation review

Before release promotion, verify:

- no secret/signing file is tracked;
- privacy documentation matches current dependencies/network behavior;
- platform/build documentation matches current project/workflow metadata;
- security/support contacts are current;
- release/version identity is consistent across project metadata and docs;
- user-facing known limitations are documented without overstating evidence.

## 13. Final release evidence block

Use a record similar to:

```text
Product: CalcNova 2.8.03 — COMPLETE
Normalized tag: v2.8.3
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

A stable release should only be promoted after every release-required evidence row has an acceptable observed result and any known limitation is documented. This evidence discipline is separate from the completed 2.8.03 source/product classification.
