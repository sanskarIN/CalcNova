# CalcNova Troubleshooting

Use the least-destructive diagnostic step first. Avoid deleting unrelated files, user data, credentials, or system configuration when a project-specific repair is sufficient.

For authoritative build commands, see [BUILDING.md](BUILDING.md). For platform status, see [PLATFORM_SUPPORT.md](PLATFORM_SUPPORT.md).

## `dotnet` command not found

Check:

```bash
dotnet --info
```

If the command is unavailable, install a .NET SDK compatible with `global.json`, then reopen the terminal or IDE so PATH changes are applied.

## SDK version mismatch

Check:

```bash
dotnet --list-sdks
cat global.json
```

On PowerShell:

```powershell
Get-Content global.json
```

CalcNova uses .NET 10. A compatible stable .NET 10 SDK must be installed even when SDK roll-forward rules permit a nearby feature band.

## Workload inventory

Before troubleshooting a platform head, check installed workloads:

```bash
dotnet workload list
```

Required platform workloads include:

- Browser/WebAssembly: `wasm-tools`;
- Android: `android`;
- iOS: `ios` on a supported macOS/Xcode host.

Install only the workload required for the target you are building.

## Restore fails

For the core solution:

```bash
dotnet restore CalcNova.slnx --force-evaluate
```

For a platform head, restore the target project directly, for example:

```bash
dotnet restore src/CalcNova.Android/CalcNova.Android.csproj
dotnet restore src/CalcNova.Browser/CalcNova.Browser.csproj
```

Also check:

- network/proxy access to NuGet;
- package-source configuration;
- whether the requested package version exists for the selected target;
- whether the required platform workload is installed;
- whether stale credentials for a private feed are interfering.

Do not add random package sources or disable TLS validation to work around restore failures.

## Build fails after package/project changes

Run:

```bash
dotnet clean CalcNova.slnx
dotnet restore CalcNova.slnx
dotnet build CalcNova.slnx --configuration Debug
```

If generated `bin`/`obj` output appears stale, remove only those project-generated directories after confirming their paths. Do not use broad recursive delete commands against parent, home, or user-data directories.

## Formatter failure

Run locally:

```bash
dotnet format CalcNova.slnx --no-restore
```

Review the diff, then rerun:

```bash
dotnet format CalcNova.slnx --verify-no-changes --no-restore
```

## Analyzer/warnings-as-errors failure

Read the exact diagnostic and fix the cause. Do not add a blanket analyzer suppression.

If a suppression is genuinely necessary, scope it narrowly and document why.

## Avalonia XAML compile failure

Common causes include:

- invalid property/event name;
- missing `x:Class` code-behind type;
- incorrect XML namespace;
- unsupported control property for the selected Avalonia version;
- binding/property typo;
- malformed markup.

Use the first XAML compiler error as the starting point because later diagnostics can be cascading failures.

The repository also contains SDK-independent XAML validation. See [XAML_VALIDATION.md](XAML_VALIDATION.md).

## Desktop app starts with a blank or failed window

Check:

- application `Initialize()` loads `App.axaml`;
- desktop lifetime creates `MainWindow`;
- `DataContext` is assigned;
- `MainWindow.axaml` compiles;
- runtime native graphics dependencies are available;
- the correct desktop project is being launched.

Run:

```bash
dotnet run --project src/CalcNova.Desktop/CalcNova.Desktop.csproj
```

Use a Debug build for richer local diagnostics.

## Windows desktop issues

If compilation succeeds but Windows runtime/package behavior fails, separate these layers:

1. shared desktop build;
2. `win-x64` publish output;
3. normal executable launch;
4. Appx/MSIX packaging metadata;
5. package signing/install behavior.

A packaging failure does not automatically imply the shared Avalonia desktop project failed.

Check `packaging/windows/` for the repository-owned manifest template and [BUILDING.md](BUILDING.md) for the current release RID.

## Linux native dependency issues

Avalonia desktop runtime requirements can vary by distribution and graphics stack.

Capture the actual missing library/error and document the affected distribution/version instead of installing unrelated desktop packages broadly.

Also distinguish:

- compile/publish success;
- executable launch;
- graphics backend issues;
- clipboard/storage behavior;
- freedesktop/AppStream integration.

Repository packaging metadata is under `packaging/linux/`.

## macOS desktop issues

Separate normal desktop compilation from bundle, signing, and notarization concerns.

For signing/notarization failures, verify:

- the build is running on a suitable macOS environment;
- required Apple command-line tools are selected;
- certificate/keychain configuration is correct;
- identifiers in generated package metadata match the release configuration;
- credentials are supplied externally rather than committed.

The current desktop release workflow publishes `osx-x64`. Additional release architectures require their own recorded publish/runtime evidence.

## SQLite history failure

Check:

- the selected database directory is writable;
- a file/directory permission error is not being swallowed by the caller;
- schema initialization was called;
- the file is not locked by an external process;
- native composition is not being used where Browser-safe persistence is required.

Do not delete user history automatically as the first repair step. Provide explicit backup/reset behavior when the app exposes history maintenance.

## Browser/WebAssembly workload or publish issues

The maintained Browser head is `src/CalcNova.Browser` and targets `net10.0-browser`.

Check the workload:

```bash
dotnet workload list
```

Install if needed:

```bash
dotnet workload install wasm-tools
```

Then use the CI-equivalent path:

```bash
dotnet restore src/CalcNova.Browser/CalcNova.Browser.csproj
dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj \
  --configuration Release \
  --no-restore \
  --output artifacts/browser
```

If publish succeeds but the browser app fails at runtime, inspect:

- browser console errors;
- static asset/base-path configuration;
- hosting MIME types;
- cache/service-worker behavior if a host adds those layers;
- browser storage availability/quota;
- clipboard permission behavior;
- unsupported native-only dependency usage;
- optional currency network requests.

Browser persistence intentionally uses Browser-safe composition instead of native SQLite.

## Android workload / SDK issues

The maintained Android head is `src/CalcNova.Android` and targets `net10.0-android` with minimum platform/API 23.

CI uses Temurin JDK 17.

Check:

```bash
dotnet --info
dotnet workload list
java -version
```

Install the workload if needed:

```bash
dotnet workload install android
```

Then run the CI-equivalent build:

```bash
dotnet restore src/CalcNova.Android/CalcNova.Android.csproj
dotnet build src/CalcNova.Android/CalcNova.Android.csproj \
  --configuration Release \
  --no-restore
```

If it fails, verify:

- Android workload installation;
- Android SDK/platform/build-tools availability;
- JDK 17 selection;
- Android SDK environment configuration;
- emulator/device connectivity for runtime checks;
- package/application id `in.sanskar.calcnova`;
- minimum API compatibility;
- release signing configuration when producing a signed AAB.

A normal compile/build does not require production signing. A signed release AAB does.

Never commit keystores, aliases with secrets, or passwords while troubleshooting. See [BUILDING.md](BUILDING.md) for the release secret contract.

## Android signed AAB issues

If an unsigned/normal Android build passes but signed publication fails, focus on signing rather than changing calculator code.

Check that all required CI secrets are configured:

- `CALCNOVA_ANDROID_KEYSTORE_BASE64`;
- `CALCNOVA_ANDROID_KEY_ALIAS`;
- `CALCNOVA_ANDROID_KEY_PASSWORD`;
- `CALCNOVA_ANDROID_STORE_PASSWORD`.

Verify the keystore/alias/password combination locally using secure tooling without printing secrets into logs.

The release workflow intentionally skips the signed Android artifact when signing secrets are absent.

## iOS workload / Xcode issues

The maintained iOS head is `src/CalcNova.iOS`, targets `net10.0-ios`, and declares minimum iOS platform version 15.0.

iOS builds require a supported macOS/Xcode environment.

Check:

```bash
dotnet --info
dotnet workload list
xcode-select -p
xcodebuild -version
```

Install the .NET workload if needed:

```bash
dotnet workload install ios
```

For an Apple Silicon simulator, use:

```bash
dotnet restore src/CalcNova.iOS/CalcNova.iOS.csproj \
  -p:RuntimeIdentifier=iossimulator-arm64
dotnet build src/CalcNova.iOS/CalcNova.iOS.csproj \
  --configuration Release \
  --no-restore \
  -p:RuntimeIdentifier=iossimulator-arm64
```

For an Intel simulator, use `iossimulator-x64` instead.

If the simulator build fails, verify:

- installed iOS workload;
- Xcode command-line selection;
- compatible Xcode/iOS SDK availability;
- simulator runtime availability;
- application id/bundle configuration.

If device/archive/App Store publication fails after simulator compilation succeeds, investigate signing/provisioning separately:

- certificate/keychain access;
- provisioning profile;
- team/bundle identifier;
- entitlements;
- device registration where required;
- archive/export settings.

Keep Apple signing credentials outside source control. See [IOS_RELEASE_VALIDATION.md](IOS_RELEASE_VALIDATION.md).

## Tests fail only on one OS

Do not immediately weaken the assertion. Determine whether the failure is caused by:

- path separators or case sensitivity;
- locale/time-zone assumptions;
- newline conventions;
- filesystem locking;
- platform numeric/runtime differences;
- missing native dependency;
- a target-specific workload or runtime behavior.

Domain math tests should be culture-independent unless a test explicitly targets formatting/localization.

## CI fails but local build passes

Compare:

- SDK versions;
- installed workloads;
- JDK version for Android;
- Xcode/iOS environment for Apple builds;
- operating system/architecture;
- environment variables;
- clean checkout versus local generated state;
- package cache behavior;
- warnings-as-errors;
- file-name case.

Reproduce with the same project-specific commands used by the corresponding `.github/workflows/build-*.yml` workflow.

## Source preflight failure

Run:

```bash
python tools/release_preflight.py
```

Read the first failing validator and run it directly when the error output recommends a narrower command.

Do not bypass a source contract just to make the aggregate preflight green. Fix the source/documentation/workflow inconsistency that the validator detected.

## Evidence-state confusion

Use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

`NOT RUN` means the operation was not executed in that evidence environment. It does not mean the source head does not exist. Likewise, the existence of a source head or workflow is not enough to label a runtime/device/store scenario PASS.

See [VALIDATION_EVIDENCE.md](VALIDATION_EVIDENCE.md).

## Reporting unresolved build failures

Create a bug report with:

- commit SHA;
- target platform/OS and architecture;
- `dotnet --info` output with sensitive paths reviewed;
- workload list for Browser/Android/iOS failures;
- JDK version for Android failures;
- Xcode version for iOS/macOS failures where relevant;
- exact command;
- first meaningful error;
- minimal relevant log section.

Do not paste secrets, signing material, tokens, or full private environment dumps.
