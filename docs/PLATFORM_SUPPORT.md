# CalcNova Platform Support

This document separates **source availability** from **validated release support**. A target is never marked PASS merely because Avalonia/.NET can theoretically target it.

## Current source matrix

| Target | Source status | Persistence/composition | Validation policy |
|---|---|---|---|
| Shared domain/application libraries | Implemented modular baseline | Platform-neutral contracts | Build/test on Linux, Windows, and macOS CI |
| Windows desktop | `CalcNova.Desktop` host present | SQLite history + JSON settings/currency cache | Desktop workflow + manual packaging smoke test |
| Linux desktop | `CalcNova.Desktop` host present | SQLite history + JSON settings/currency cache | Desktop workflow + representative distro smoke test |
| macOS desktop | `CalcNova.Desktop` host present | SQLite history + JSON settings/currency cache | Desktop workflow + bundle/sign/notarization checks when credentials/environment exist |
| Android | `CalcNova.Android` head present | Native local storage + safe link service | Android workload build + APK/AAB checks |
| iOS | `CalcNova.iOS` head present | Native local storage + safe link service | macOS/iOS simulator build; device/archive signing separately |
| Browser/WebAssembly | `CalcNova.Browser` head present | `localStorage` history/settings/currency cache | Browser publish + PWA/offline smoke test |

The shared modular Avalonia view is reused across Desktop, Android, iOS, and Browser so mathematical/user-mode behavior does not fork by platform unnecessarily.

## Validation status rules

Use only these interpretations in release/state files:

- **PASS** — the stated command/check actually completed successfully;
- **FAIL** — the stated command/check ran and failed;
- **QUEUED / IN PROGRESS** — CI has been triggered but has not concluded;
- **NOT RUN** — the required SDK/workload/platform/signing environment was unavailable.

The active ChatGPT execution container used during the August 18, 2026 development segment does not include the .NET SDK, so local `dotnet` build/test commands remain **NOT RUN** there. GitHub Actions PR validation is used as an independent build environment.

## Shared UI and application behavior

The shared application currently includes modular views for:

- Standard/Scientific calculator;
- Programmer calculator;
- Unit converter;
- Statistics;
- Equations;
- Matrices;
- interactive Graphing;
- Date/Duration utilities;
- optional Currency converter architecture/UI;
- History;
- Settings;
- About/Support.

Desktop `MainWindow` hosts the same `MainView` used by the single-view Android/iOS/Browser application lifetime.

## Windows

Source/build support includes:

- shared Avalonia desktop executable;
- resizable desktop window;
- keyboard/numpad routing;
- clipboard shortcuts/actions;
- deterministic project-owned icon generation;
- Windows portable ZIP packaging helper;
- Windows packaging metadata/templates;
- optional signing hooks that do not store credentials in Git.

MSIX can remain optional unless a maintained packaging path is adopted and validated.

## Linux

Source/build support includes:

- shared Avalonia desktop executable;
- generated Linux PNG icon assets;
- `.desktop`/packaging metadata;
- Linux bundle helper script;
- no claim of universal distro compatibility.

Representative runtime dependency and desktop integration testing is still required before a stable release claim.

## macOS

Source/build support includes:

- shared Avalonia desktop executable;
- generated ICNS/iconset assets;
- `.app` bundle helper script;
- bundle metadata;
- optional external signing identity hook.

Notarization and trusted release signing require Apple tools/credentials outside the repository and must be reported separately from ordinary macOS compilation.

## Android

The Android head includes:

- application/package identifier based on `in.sanskar.calcnova`;
- Avalonia Android startup using the shared `SingleViewApp`;
- local SQLite history;
- local JSON settings and currency-rate cache;
- Android-safe external-link service;
- permission-minimal manifest baseline;
- adaptive launcher icon resources using original CalcNova artwork;
- platform splash/theme resources;
- CI workflow installing the Android workload;
- release workflow support for signing through repository secrets rather than committed keystores/passwords.

Remaining release validation includes actual APK/AAB installation/smoke tests, orientation/tablet accessibility review, and store metadata/privacy-policy checks.

## iOS

The iOS head includes:

- Avalonia iOS startup using the shared `SingleViewApp`;
- local SQLite history;
- local JSON settings and currency cache;
- iOS-safe external-link service;
- launch-screen metadata;
- generated AppIcon asset-catalog inputs from project-owned artwork;
- simulator CI workflow.

Real device/archive/App Store validation still requires a supported macOS/Xcode environment, Apple signing identity, provisioning, and store credentials that are intentionally not stored in the repository.

## Browser/WebAssembly + PWA

The Browser head includes:

- Avalonia Browser/WebAssembly startup;
- shared calculation/application logic;
- Browser-specific `localStorage` history repository;
- Browser settings repository;
- Browser currency cache;
- safe external-link bridge;
- PWA manifest;
- service worker/offline app-shell baseline;
- favicon/icon/social assets;
- client-side ordinary calculations with no server requirement;
- Browser publish CI workflow.

Remaining validation includes supported-browser smoke testing, install/offline behavior, base-path hosting behavior, keyboard/accessibility review, and cache-update behavior across releases.

## Feature consistency requirement

Core mathematical semantics must remain platform-independent. Platform heads may differ in lifecycle, packaging, storage implementation, external-link handling, clipboard/haptics behavior, and native conventions, but the same expression should not produce a different result simply because it ran on another supported target unless a documented numeric/platform limitation explains the difference.

## Release acceptance

Before a platform is listed as stable for a release:

1. build it using the documented toolchain;
2. record the exact runtime/OS/workload used;
3. run relevant automated tests;
4. perform representative manual launch/navigation/calculation smoke tests;
5. verify local persistence behavior;
6. verify icon/splash/metadata;
7. verify accessibility basics;
8. verify no secrets or private signing material are tracked;
9. record any target-specific limitations in `PROJECT_STATE.md` and release notes.
