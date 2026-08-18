# Building CalcNova

This guide describes the repository's current build commands and the intended target-specific release paths. A command is only considered validated when it has actually run successfully in the relevant environment.

## Toolchain

CalcNova uses:

- C#;
- .NET 10;
- Avalonia UI;
- MSBuild / `dotnet` CLI;
- NuGet central package management.

The SDK feature band is selected by the repository `global.json`.

Check your environment:

```bash
dotnet --info
dotnet --list-sdks
```

## Restore

From the repository root:

```bash
dotnet restore CalcNova.slnx
```

## Formatting

```bash
dotnet format CalcNova.slnx --verify-no-changes --no-restore
```

To apply formatter changes locally:

```bash
dotnet format CalcNova.slnx --no-restore
```

Review formatter changes before committing them.

## Build

Debug:

```bash
dotnet build CalcNova.slnx --configuration Debug --no-restore
```

Release:

```bash
dotnet build CalcNova.slnx --configuration Release --no-restore
```

## Tests

```bash
dotnet test CalcNova.slnx --configuration Release --no-build
```

See `docs/TESTING.md` for test responsibilities.

## Run the current desktop application

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

The current source tree contains the shared Avalonia app and desktop host. Other platform heads are added only when their source and packaging configuration are ready to be maintained.

## Publish desktop output

### Windows

When the Windows release target has passed CI and packaging review, a framework-dependent publish can be produced with a Windows RID such as:

```powershell
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained false
```

A self-contained package can be evaluated later after size and update tradeoffs are documented.

MSIX packaging is a planned packaging option rather than a currently validated artifact.

### Linux

Example framework-dependent publish:

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false
```

Linux distribution packages may require system libraries used by Avalonia and the selected graphics backend. Packaging formats such as Flatpak/AppImage should only be added when the project can maintain and test them.

### macOS

Example framework-dependent publish on a macOS build environment:

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained false
```

macOS signing/notarization requires Apple credentials and a macOS environment. Signing secrets must never be stored in this repository.

## Android

The master project requires Android APK and AAB support, but the Android platform head is not yet present in the current project state.

Once `src/CalcNova.Android` is implemented and validated, this section will contain exact project-specific commands for:

- debug APK;
- release APK;
- AAB;
- package ID;
- Android SDK/API requirements;
- Java/JDK requirements;
- signing through local/CI secret configuration;
- adaptive icons and splash assets.

Until then, do not treat generic Android commands as proof that CalcNova Android builds succeed.

## iOS

The iOS platform head is not yet present in the current project state.

iOS device/archive validation requires a supported macOS/Xcode environment and appropriate Apple signing configuration. Real certificates, provisioning profiles, and passwords must remain outside Git.

When the head exists, this guide will include exact simulator/device/archive commands tied to the actual project file.

## Browser / WebAssembly

The Browser/WebAssembly platform head is not yet present in the current project state.

The intended browser target must:

- reuse the shared Avalonia app;
- avoid native SQLite dependencies;
- use browser-compatible local persistence;
- work without a server for ordinary calculations;
- include PWA/offline-shell configuration where supported.

Exact `dotnet run/publish` commands will be added with the real project.

## PWA deployment

PWA deployment is planned with the Browser target. The final deployment guide must describe base-path handling, caching/update behavior, manifest/icons, and hosting requirements based on the implemented output rather than a generic template.

## Signing and secrets

Never commit:

- keystores;
- signing passwords;
- Apple certificates/provisioning profiles;
- private keys;
- store credentials;
- API tokens;
- service-account secrets.

Use local secure storage or CI secret stores and provide placeholder/example configuration only.

## Cleaning local build output

Safe project-output cleanup:

```bash
dotnet clean CalcNova.slnx
```

If a stale build requires removal of `bin`/`obj`, confirm that only generated project output is being removed. Do not provide scripts that delete unrelated user files.

## CI

Current baseline workflows live under `.github/workflows/` and include formatting, build/test, and documentation checks.

CI is the source of truth for checks it actually executes. A missing, cancelled, or unavailable target must not be described as PASS.

## Common failures

See `docs/TROUBLESHOOTING.md` for SDK, workload, JDK, Xcode, Linux dependency, cache, and package-resolution troubleshooting.
