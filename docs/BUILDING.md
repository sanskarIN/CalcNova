# Building CalcNova

This guide documents the current CalcNova 2.8.03 build, run, publish, and platform-workload paths.

CalcNova contains maintained composition heads for:

- Desktop (`src/CalcNova.Desktop`) for Windows, Linux, and macOS;
- Browser/WebAssembly (`src/CalcNova.Browser`);
- Android (`src/CalcNova.Android`);
- iOS (`src/CalcNova.iOS`).

A source project or CI workflow proves that a supported build path exists; it does not by itself prove that a particular machine, device, signing identity, store submission, or runtime scenario has passed. Record actual execution results using the evidence rules in [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md) and [RUNTIME_VALIDATION_RUNBOOK.md](RUNTIME_VALIDATION_RUNBOOK.md).

## Toolchain

CalcNova uses:

- C#;
- .NET 10;
- Avalonia UI;
- MSBuild / `dotnet` CLI;
- NuGet central package management.

The repository SDK feature band is selected by `global.json`.

Check the local environment with:

```bash
dotnet --info
dotnet --list-sdks
dotnet workload list
```

## Platform prerequisites

| Target | Required build environment |
|---|---|
| Shared/core solution | .NET 10 SDK |
| Windows desktop | .NET 10 SDK on Windows, Linux, or macOS for normal compilation; Windows is required for Windows-specific runtime/package verification |
| Linux desktop | .NET 10 SDK; representative Linux runtime verification is recommended |
| macOS desktop | .NET 10 SDK; macOS is required for macOS runtime, signing, and notarization verification |
| Browser/WebAssembly | .NET 10 SDK + `wasm-tools` workload |
| Android | .NET 10 SDK + Android workload + JDK 17 + Android SDK/toolchain |
| iOS | supported macOS/Xcode environment + .NET 10 SDK + iOS workload |

The current project metadata sets:

- Android target framework: `net10.0-android`;
- Android minimum platform/API: 23;
- iOS target framework: `net10.0-ios`;
- iOS minimum platform version: 15.0;
- Browser target framework: `net10.0-browser`.

## Source preflight

The SDK-independent repository gate can be run first:

```bash
python tools/release_preflight.py
```

For a tagged 2.8.03 release checkout:

```bash
python tools/release_preflight.py --tag v2.8.3
```

## Restore, format, build, and test the core solution

From the repository root:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

To apply formatter changes locally rather than verify them:

```bash
dotnet format CalcNova.slnx --no-restore
```

Review formatter changes before committing them.

See [TESTING.md](TESTING.md) for test responsibilities and [SOURCE_PREFLIGHT.md](SOURCE_PREFLIGHT.md) for SDK-independent validation.

## Desktop — Windows, Linux, macOS

The shared desktop entry point is:

```text
src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

### Run the desktop application

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

### CI-equivalent desktop build

The `build-desktop.yml` workflow restores and builds the desktop project in Release configuration on Ubuntu, Windows, and macOS runners:

```bash
dotnet restore src/CalcNova.Desktop/CalcNova.Desktop.csproj
dotnet build src/CalcNova.Desktop/CalcNova.Desktop.csproj --configuration Release --no-restore
```

### Release publish targets

The current release workflow produces self-contained desktop publish output for:

- `win-x64`;
- `linux-x64`;
- `osx-x64`.

Equivalent commands are:

#### Windows x64

```powershell
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --output publish/win-x64
```

#### Linux x64

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output publish/linux-x64
```

#### macOS x64

```bash
dotnet publish src/CalcNova.Desktop/CalcNova.Desktop.csproj \
  --configuration Release \
  --runtime osx-x64 \
  --self-contained true \
  --output publish/osx-x64
```

The shared Avalonia desktop source is not limited to those release RIDs, but additional release architectures should only be advertised after their publish/runtime evidence is recorded.

### Desktop packaging metadata

Repository-owned release metadata is under `packaging/`:

- `packaging/windows/` — Windows Appx/MSIX manifest template;
- `packaging/linux/` — freedesktop desktop entry and AppStream metadata;
- `packaging/macos/` — macOS application-bundle metadata template.

These files are packaging-layer metadata. A normal development build does not require a native installer/package.

Windows package signing and macOS signing/notarization require external credentials and target-specific tooling.

## Browser / WebAssembly

The Browser head is:

```text
src/CalcNova.Browser/CalcNova.Browser.csproj
```

Install the WebAssembly workload:

```bash
dotnet workload install wasm-tools
```

Restore and publish using the same contract as `build-browser.yml`:

```bash
dotnet restore src/CalcNova.Browser/CalcNova.Browser.csproj
dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj \
  --configuration Release \
  --no-restore \
  --output artifacts/browser
```

The release workflow publishes the Browser head to its own release bundle.

Browser composition uses Browser-safe history/settings storage rather than native SQLite composition. Ordinary calculation remains local-first.

After publishing, browser runtime evidence should cover at least:

- application load/initialization;
- local storage persistence;
- keyboard behavior where applicable;
- clipboard permission/failure behavior;
- graph interaction;
- accessibility behavior;
- optional network-enhanced currency behavior.

Do not describe a publish-only result as proof that all browser/runtime scenarios passed.

## Android

The Android head is:

```text
src/CalcNova.Android/CalcNova.Android.csproj
```

Current identity and platform metadata:

- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.8.03`;
- numeric build code: `20803`;
- minimum Android API: 23;
- JDK used by CI: Temurin 17.

Install the workload:

```bash
dotnet workload install android
```

CI-equivalent restore/build:

```bash
dotnet restore src/CalcNova.Android/CalcNova.Android.csproj
dotnet build src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  --no-restore
```

A normal build can be used for compilation validation without production signing.

### Signed Android App Bundle

The release workflow publishes an AAB only when the required signing secrets are configured. Its publish contract is equivalent to:

```bash
dotnet publish src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  -p:AndroidPackageFormats=aab \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore="<secure-keystore-path>" \
  -p:AndroidSigningKeyAlias="<alias>" \
  -p:AndroidSigningKeyPass="<key-password>" \
  -p:AndroidSigningStorePass="<store-password>"
```

Never put real signing values in shell history, committed scripts, documentation examples, or repository files. Prefer secure local secret storage or CI secrets.

The GitHub release workflow uses these secret names:

- `CALCNOVA_ANDROID_KEYSTORE_BASE64`;
- `CALCNOVA_ANDROID_KEY_ALIAS`;
- `CALCNOVA_ANDROID_KEY_PASSWORD`;
- `CALCNOVA_ANDROID_STORE_PASSWORD`.

The workflow decodes a temporary keystore, publishes the AAB, uploads the artifact, and removes the temporary keystore.

Android runtime evidence should distinguish compilation from emulator/device launch, orientation behavior, persistence, clipboard, TalkBack/large-text checks, signed package generation, and Play Store processing.

## iOS

The iOS head is:

```text
src/CalcNova.iOS/CalcNova.iOS.csproj
```

Current identity and platform metadata:

- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.8.03`;
- numeric build code: `20803`;
- minimum iOS platform version: 15.0.

The iOS toolchain requires a supported macOS/Xcode environment.

Install the workload:

```bash
dotnet workload install ios
```

### Simulator build

The CI workflow chooses a simulator RID from the runner architecture:

- Apple Silicon: `iossimulator-arm64`;
- Intel: `iossimulator-x64`.

Example Apple Silicon simulator commands:

```bash
dotnet restore src/CalcNova.iOS/CalcNova.iOS.csproj \
  -p:RuntimeIdentifier=iossimulator-arm64

dotnet build src/CalcNova.iOS/CalcNova.iOS.csproj \
  --configuration Release \
  --no-restore \
  -p:RuntimeIdentifier=iossimulator-arm64
```

For an Intel simulator environment, replace the RID with `iossimulator-x64`.

Real-device/archive/App Store distribution requires appropriate Apple signing, provisioning, entitlements, Xcode configuration, and credentials outside Git.

The repository's unsigned simulator validation must not be described as a signed App Store artifact. See [IOS_RELEASE_VALIDATION.md](IOS_RELEASE_VALIDATION.md).

## Platform workflow source of truth

Current platform workflows are:

- `.github/workflows/build-desktop.yml`;
- `.github/workflows/build-browser.yml`;
- `.github/workflows/build-android.yml`;
- `.github/workflows/build-ios.yml`;
- `.github/workflows/release.yml`.

If a future maintenance change modifies a target project or workflow, update this build guide in the same change.

## Signing and secrets

Never commit:

- Android keystores;
- signing passwords;
- Apple certificates or provisioning profiles;
- private keys;
- notarization credentials;
- store credentials;
- API tokens;
- service-account secrets.

Use secure local storage or CI secret stores. Documentation and example configuration must use placeholders only.

## Cleaning generated output

Safe solution cleanup:

```bash
dotnet clean CalcNova.slnx
```

Target-specific cleanup can use the corresponding project path. If manually removing `bin` or `obj`, confirm that only generated project output is being removed.

## Validation status vocabulary

Use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

A platform source head may be complete while a device/signing/store check remains `NOT RUN` or `BLOCKED` in a particular environment. Never convert an unexecuted operation into PASS.

## Common failures

See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for SDK, workload, JDK, Android SDK, Xcode, Linux dependency, cache, and package-resolution troubleshooting.

See [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md) for the authoritative platform-composition status and [RELEASE.md](RELEASE.md) for release publication behavior.
