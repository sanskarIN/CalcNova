# Building CalcNova

This guide describes commands tied to the repository's actual project files. A command is only considered validated when it has really completed successfully in the appropriate environment.

## Toolchain

CalcNova uses:

- C#;
- .NET 10;
- Avalonia UI;
- MSBuild / `dotnet` CLI;
- NuGet central package management;
- workload-specific Android, iOS, and WebAssembly tooling.

The SDK feature band is selected by `global.json`.

Verify your environment:

```bash
dotnet --info
dotnet --list-sdks
```

## Core restore / format / build / test

From the repository root:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

To apply formatter changes locally:

```bash
dotnet format CalcNova.slnx --no-restore
```

Review the diff before committing formatter changes.

`CalcNova.slnx` contains the core/application/domain/desktop/test graph. `CalcNova.All.slnx` records all platform heads as well, but workload-specific builds are intentionally validated independently.

## Safe helper checks

PowerShell:

```powershell
./tools/scripts/check.ps1
```

Shell:

```bash
./tools/scripts/check.sh
```

Repository/docs/assets also have dedicated validation scripts under `tools/scripts/`.

## Run desktop

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

The Desktop host reuses the same shared modular `MainView` used by mobile/Browser heads.

# Desktop publish / packaging

## Windows

Framework-dependent example:

```powershell
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained false `
  --output artifacts/windows/win-x64
```

The repository contains:

```text
packaging/windows/AppxManifest.xml.template
packaging/windows/package.ps1
```

The packaging helper is intended for reproducible portable packaging and can use generated project-owned icons. MSIX is an optional packaging path and should only be called release-ready after its actual packaging/signing flow is validated.

Never commit a Windows signing private key/certificate password.

## Linux

Framework-dependent example:

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false \
  --output artifacts/linux/linux-x64
```

Linux integration files:

```text
packaging/linux/in.sanskar.calcnova.desktop
packaging/linux/in.sanskar.calcnova.metainfo.xml
packaging/linux/package.sh
```

Run the packaging helper on a Linux environment after validating its dependencies:

```bash
./packaging/linux/package.sh
```

Avalonia runtime/system-library requirements can vary by Linux distribution. Validate on representative distributions instead of claiming universal compatibility.

## macOS

Framework-dependent Apple Silicon example:

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained false \
  --output artifacts/macos/osx-arm64
```

macOS packaging files:

```text
packaging/macos/Info.plist.template
packaging/macos/package.sh
```

Run on macOS:

```bash
./packaging/macos/package.sh
```

The helper can build a `.app`/archive using project-owned generated icon assets. Signing/notarization requires Apple tools and credentials supplied outside Git; an unsigned bundle build is not equivalent to a notarized release.

# Android

## Prerequisites

Install a compatible JDK/Android SDK and the .NET Android workload:

```bash
dotnet workload install android
```

The Android project is:

```text
src/CalcNova.Android/CalcNova.Android.csproj
```

Current app identity:

```text
ApplicationId: in.sanskar.calcnova
ApplicationTitle: CalcNova
Minimum supported Android API baseline: 23
```

## Restore / build

```bash
dotnet restore src/CalcNova.Android/CalcNova.Android.csproj
dotnet build src/CalcNova.Android/CalcNova.Android.csproj --configuration Debug --no-restore
dotnet build src/CalcNova.Android/CalcNova.Android.csproj --configuration Release --no-restore
```

## APK only

```bash
dotnet publish src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  -p:AndroidPackageFormats=apk
```

## AAB only

```bash
dotnet publish src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  -p:AndroidPackageFormats=aab
```

## APK + AAB

Release builds can request both:

```bash
dotnet publish src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  -p:AndroidPackageFormats="aab;apk"
```

The project contains adaptive launcher icon/splash resources. Generated/signing artifacts belong outside source control.

## Android signing

Do not commit:

- keystore files;
- aliases/passwords;
- Play credentials.

The release workflow expects signing material through GitHub Actions Secrets/temporary files. Keep the same production signing identity across releases once distribution begins.

# iOS

## Prerequisites

Use a supported macOS/Xcode environment and install the iOS workload:

```bash
dotnet workload install ios
```

Project:

```text
src/CalcNova.iOS/CalcNova.iOS.csproj
```

## Simulator restore / build

Apple Silicon example:

```bash
dotnet restore src/CalcNova.iOS/CalcNova.iOS.csproj \
  -p:RuntimeIdentifier=iossimulator-arm64

dotnet build src/CalcNova.iOS/CalcNova.iOS.csproj \
  --configuration Release \
  --no-restore \
  -p:RuntimeIdentifier=iossimulator-arm64
```

Intel simulator environments use the appropriate simulator RID instead. The CI workflow chooses the simulator RID according to the runner architecture.

## Device/archive

Real device/App Store archive work requires:

- supported Xcode/iOS workload;
- bundle identifier/team configuration;
- signing identity;
- provisioning profile;
- App Store credentials for distribution.

These are environment/secret concerns and are not stored in Git. Do not describe a simulator build as proof that a signed device/archive build passed.

# Browser / WebAssembly

## Prerequisites

Install the WebAssembly workload:

```bash
dotnet workload install wasm-tools
```

Project:

```text
src/CalcNova.Browser/CalcNova.Browser.csproj
```

## Restore / publish

```bash
dotnet restore src/CalcNova.Browser/CalcNova.Browser.csproj

dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj \
  --configuration Release \
  --no-restore \
  --output artifacts/browser
```

The Browser head does **not** reference native SQLite. It uses Browser-compatible `localStorage` implementations for history/settings/currency cache.

# PWA shell

The Browser project contains:

- install manifest;
- service worker;
- Browser icons/social assets;
- offline app-shell baseline.

After publishing, validate:

- correct host/base path;
- first-load behavior;
- installability where the browser supports it;
- offline reload after successful initial load;
- cache update behavior after a release;
- keyboard/touch/accessibility flows.

Do not assume a local publish guarantees correct hosting under every static-site base path.

# Branding assets

Project-owned master artwork lives under `assets/`. Deterministic raster/icon generation is handled by:

```bash
python tools/scripts/generate_brand_assets.py
```

Generated raster outputs that can be reproduced from source artwork are intentionally ignored where appropriate. Asset licensing notes are in `assets/ASSET_LICENSES.md`.

# Release workflow

`.github/workflows/release.yml` prepares release artifacts for supported paths and uses repository secrets for signing material where required. It must never contain a real private key/password/token.

Semantic release tags use forms such as:

```text
v0.1.0
v0.2.0
v1.0.0
```

Only tag a milestone after its required validation gate is complete.

# CI workflows

Current workflows are separated by responsibility:

- formatting;
- build/test;
- coverage;
- repository/docs checks;
- security/dependency checks;
- Desktop build;
- Android build;
- Browser publish;
- iOS simulator build;
- release artifact generation.

Platform workflows watch shared `src/**` changes so modifications to domain/application code are validated by heads that consume them.

# Signing / secret rules

Never commit:

- Android keystores;
- signing passwords;
- Apple certificates/provisioning profiles;
- private keys;
- store credentials;
- API tokens;
- service-account secrets;
- live currency-provider private keys.

Use local secret stores or GitHub Actions Secrets and materialize temporary files only during the trusted build job.

# Cleaning generated build output

Safe baseline:

```bash
dotnet clean CalcNova.slnx
```

If manual `bin`/`obj` removal is required, inspect paths before deletion and remove only generated project output. Never use repository helper scripts to delete unrelated user data.

# Troubleshooting

See [`docs/TROUBLESHOOTING.md`](TROUBLESHOOTING.md) for SDK, Android workload/JDK, Xcode/iOS, Linux dependency, cache, signing, and package-resolution issues.

# Validation truthfulness

A source target or documented command is not a PASS by itself. Record:

- `PASS` only after successful execution;
- `FAIL` after an executed failure;
- `QUEUED/IN PROGRESS` while CI is running;
- `NOT RUN` when the required environment is unavailable.

`PROJECT_STATE.md` is the current source of truth for the repository's actual validation state.
