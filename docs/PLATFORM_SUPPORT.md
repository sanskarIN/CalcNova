# CalcNova Platform Support

This document records source/validation status separately. A target is not considered supported merely because Avalonia or .NET can theoretically target it.

## Current repository status

| Target | Source status | Validation status |
|---|---|---|
| Shared domain libraries | Implemented baseline | NOT RUN in current execution environment |
| Windows desktop | Shared desktop host source present | NOT RUN in current execution environment |
| Linux desktop | Shared desktop host source present | NOT RUN in current execution environment |
| macOS desktop | Shared desktop host source present | NOT RUN; macOS validation environment not used here |
| Android | Platform head not yet implemented | NOT RUN |
| iOS | Platform head not yet implemented | NOT RUN |
| Browser/WebAssembly | Platform head not yet implemented | NOT RUN |

`PROJECT_STATE.md` is the latest continuation source if this table becomes stale during active development.

## Shared desktop target

`src/CalcNova.Desktop` is the current Avalonia desktop entry point. The intended desktop behavior includes:

- resizable window;
- high-DPI support through the framework/platform;
- keyboard-first calculation;
- minimum usable window size;
- adaptive content as the design system evolves.

Windows/Linux/macOS packaging should remain separate from shared calculation logic.

## Windows

Planned release concerns:

- x64/arm64 target decisions;
- icon/version metadata;
- optional MSIX packaging;
- high-DPI behavior;
- keyboard/numpad coverage;
- installer/update strategy if adopted.

## Linux

Planned release concerns:

- supported desktop runtime dependencies;
- x64/arm64 decisions;
- desktop entry/icon metadata;
- maintainable package format selection.

Do not claim universal distro support without testing.

## macOS

Planned release concerns:

- Intel/Apple Silicon target policy;
- app bundle metadata;
- signing;
- notarization;
- native menu/keyboard conventions where appropriate.

Signing/notarization require Apple tooling/credentials that must remain outside Git.

## Android

The Android head must eventually provide:

- package/application ID;
- minimum/target Android SDK rationale;
- portrait/landscape adaptive layouts;
- tablet/foldable behavior;
- adaptive icon;
- platform splash screen;
- haptics setting;
- clipboard/share integration;
- debug APK;
- release APK/AAB configuration;
- secure signing placeholders/documentation.

The app should request no unrelated permissions.

## iOS

The iOS head must eventually provide:

- bundle identifier;
- simulator/device configuration;
- safe-area handling;
- app icon/launch screen;
- clipboard/share integration;
- appropriate haptics behavior;
- accessibility/text scaling validation;
- signing/archive documentation.

Real device/archive validation requires a supported macOS/Xcode environment.

## Browser/WebAssembly

The Browser target must:

- reuse shared calculation/domain logic;
- support keyboard input responsibly;
- use browser-compatible local persistence;
- avoid native SQLite dependencies;
- keep ordinary calculations client-side;
- include responsive layout;
- support installable/PWA behavior where implemented;
- provide offline core app shell where practical;
- handle browser base paths/hosting correctly.

## Feature consistency

Core mathematical semantics should stay consistent across targets. Platform-specific UI behavior may differ where native conventions require it, but a calculation should not produce a different result solely because it ran on another platform unless the difference is a documented numeric/platform limitation.

## Validation policy

For each release, record the exact targets actually built/tested. Use `NOT RUN` for unavailable environments instead of inferring success from another target.
