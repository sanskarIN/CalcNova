# CalcNova Platform Support

This document records source/validation status separately. A target is not considered supported merely because source, an Avalonia head, or a workflow exists.

## Current repository status

| Target | Source status | Validation status |
|---|---|---|
| Shared domain libraries | Implemented | NOT RUN in the current continuation environment |
| Windows desktop | Desktop host + validation workflow present | NOT RUN in the current continuation environment |
| Linux desktop | Desktop host + validation workflow present | NOT RUN in the current continuation environment |
| macOS desktop | Desktop host + validation workflow present | NOT RUN in the current continuation environment |
| Android | Android head + application metadata + validation workflow present | NOT RUN in the current continuation environment |
| iOS | iOS head + Info.plist + simulator validation workflow present | NOT RUN in the current continuation environment |
| Browser/WebAssembly | Browser head + browser-safe storage + validation workflow present | NOT RUN in the current continuation environment |

`PROJECT_STATE.md` remains the authoritative short-form continuation status.

## Cross-platform validation infrastructure

Source-level workflow contracts are protected by `tools/validate_platform_workflows.py` and its regression tests. The validator checks:

- the shared .NET 10 SDK policy;
- Desktop validation on Windows, Linux, and macOS runners;
- Browser/WebAssembly `wasm-tools` setup and publish command;
- Android Java 17 + Android workload setup and build command;
- iOS macOS runner + iOS workload + simulator-RID build command;
- read-only repository permissions for validation workflows;
- absence of signing-password/certificate settings from ordinary validation builds.

This source gate is included in the integrated release preflight. It does not prove that any target build actually completed.

## Shared desktop target

`src/CalcNova.Desktop` is the Avalonia desktop entry point shared by Windows, Linux, and macOS composition.

Current source behavior includes:

- resizable shared window;
- shared adaptive compact/medium/expanded layout handling;
- keyboard calculator and mode-navigation support;
- shared clipboard composition;
- local native persistence composition;
- shared first-run/settings/about surfaces.

Windows/Linux/macOS packaging remains separate from shared calculation logic.

## Windows

Implemented source/release foundations include:

- Desktop host;
- Windows runner build validation workflow;
- Windows release publish path;
- Appx/MSIX manifest template with release placeholders;
- cross-platform package identity validation.

Still requires real Windows evidence for:

- build and launch;
- x64 release artifact/install path;
- high-DPI and text scaling;
- keyboard/numpad behavior;
- clipboard and persistence;
- accessibility/high-contrast behavior;
- MSIX/installer packaging if used for a release.

Arm64 packaging is not currently claimed.

## Linux

Implemented source/release foundations include:

- Desktop host;
- Linux runner build validation workflow;
- Linux x64 release publish path;
- `.desktop` entry metadata;
- AppStream metadata;
- package-identity validation.

Still requires real Linux evidence for:

- publish output and launch on representative supported distributions;
- runtime dependency behavior;
- clipboard and persistence;
- keyboard/accessibility behavior;
- chosen distributable package format.

Do not claim universal distribution compatibility without testing.

## macOS

Implemented source/release foundations include:

- Desktop host;
- macOS runner build validation workflow;
- macOS x64 release publish path;
- plist packaging template with release placeholders;
- package-identity validation.

Still requires real Apple-toolchain evidence for:

- launch/runtime behavior;
- Intel/Apple Silicon distribution policy;
- clipboard and persistence;
- keyboard/accessibility behavior;
- bundle generation;
- signing and notarization.

Signing/notarization credentials must remain outside Git.

## Android

`src/CalcNova.Android` is implemented as a thin Android composition head.

Current source/release foundations include:

- application ID `in.sanskar.calcnova`;
- application title/version metadata;
- shared app composition;
- Android workload build workflow using Java 17;
- release workflow support for a signed AAB only when all external signing secrets are configured;
- temporary-keystore cleanup in the release workflow;
- package metadata validation;
- shared clipboard and settings/history composition.

Still requires real Android evidence for:

- workload restore/build;
- emulator/device launch;
- portrait/landscape and tablet behavior;
- large text/TalkBack/external keyboard interaction;
- clipboard behavior;
- persistence across restart;
- adaptive icon/splash packaging;
- signed AAB verification and store pre-launch checks.

The normal validation workflow intentionally contains no signing secrets.

## iOS

`src/CalcNova.iOS` is implemented as a thin iOS composition head.

Current source/release foundations include:

- application/bundle identity metadata;
- shared app composition;
- `Info.plist` launch metadata;
- macOS iOS-workload validation workflow;
- simulator RID selection for Apple Silicon/x64 runners;
- package metadata validation;
- shared clipboard and settings/history composition.

Still requires real macOS/Xcode evidence for:

- workload restore/build;
- simulator/device launch;
- safe-area and orientation behavior;
- Dynamic Type/VoiceOver/external keyboard behavior;
- clipboard and persistence;
- icons/launch presentation;
- signing, provisioning, archive, and distribution.

The validation workflow intentionally avoids signing configuration. Real device/archive validation requires supported Apple tooling and credentials outside Git.

## Browser/WebAssembly

`src/CalcNova.Browser` is implemented and uses browser-compatible storage rather than native SQLite.

Current source/release foundations include:

- shared calculation/domain logic;
- Browser Avalonia head;
- browser-safe history/settings storage;
- settings-schema normalization;
- shared clipboard adapter;
- `wasm-tools` validation/publish workflow;
- Browser artifact generation in the release workflow;
- shared keyboard/adaptive/onboarding behavior.

Still requires real Browser evidence for:

- WebAssembly restore/publish;
- application load in supported browsers;
- base-path/hosting behavior;
- local settings/history persistence;
- clipboard permission/failure flows;
- shell and graph keyboard shortcut conflicts;
- browser zoom/large text/accessibility behavior;
- offline/cached behavior where applicable.

## Feature consistency

Core mathematical semantics should stay consistent across targets. Platform-specific UI behavior may differ where native conventions require it, but a calculation should not produce a different result solely because it ran on another platform unless the difference is a documented numeric/platform limitation.

Shared source tests, headless UI tests, and source-contract validators reduce regression risk, but they do not replace target runtime evidence.

## Validation policy

For each release, record the exact targets actually built/tested and the relevant toolchain/environment. Use the release evidence vocabulary `PASS`, `FAIL`, `BLOCKED`, or `NOT RUN`.

Use `NOT RUN` for unavailable environments instead of inferring success from another target, from source presence, or from a workflow definition.

Accessibility-specific runtime results belong in [ACCESSIBILITY_TEST_MATRIX.md](ACCESSIBILITY_TEST_MATRIX.md). Release packaging evidence belongs in [RELEASE_READINESS_CHECKLIST.md](RELEASE_READINESS_CHECKLIST.md).
