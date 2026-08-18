# CalcNova Troubleshooting

Use the least-destructive diagnostic step first. Avoid deleting unrelated files or system configuration when a project-specific repair is sufficient.

## `dotnet` command not found

Check:

```bash
dotnet --info
```

If the command is unavailable, install a .NET SDK compatible with `global.json`, then reopen the terminal/IDE so PATH changes are applied.

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

The repository uses SDK roll-forward rules, but a compatible stable .NET 10 SDK still needs to be installed.

## Restore fails

Try:

```bash
dotnet restore CalcNova.slnx --force-evaluate
```

Also check:

- network/proxy access to NuGet;
- package-source configuration;
- whether a requested package version exists for the selected target;
- whether stale credentials for a private feed are interfering.

Do not add random package sources or disable TLS validation to work around restore failures.

## Build fails after package/project changes

Run:

```bash
dotnet clean CalcNova.slnx
dotnet restore CalcNova.slnx
dotnet build CalcNova.slnx --configuration Debug
```

If generated `bin`/`obj` output appears stale, remove only those project-generated directories after confirming their paths. Do not use broad recursive delete commands against parent/user directories.

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

Read the exact diagnostic. Prefer fixing the cause. Do not add a blanket analyzer suppression.

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

## Desktop app starts with blank/failed window

Check:

- application `Initialize()` loads `App.axaml`;
- desktop lifetime creates `MainWindow`;
- `DataContext` is assigned;
- `MainWindow.axaml` compiles;
- runtime native graphics dependencies are available.

Run a Debug build for richer local diagnostics.

## SQLite history failure

Check:

- the chosen database directory is writable;
- a file/directory permission error is not being swallowed by the caller;
- schema initialization was called;
- the file is not locked by an external process;
- application code is not using the native SQLite implementation on Browser/WebAssembly.

Do not delete user history automatically as the first repair step. Provide explicit backup/reset behavior when the app exposes history maintenance.

## Android workload / SDK issues

Android support is not yet implemented in the current source tree. Once added, troubleshooting should verify:

- required .NET Android workload;
- Android SDK/platform/build-tools versions;
- JDK compatibility;
- device/emulator connectivity;
- signing configuration for release builds.

Do not commit keystores/passwords while troubleshooting.

## Apple workload / Xcode/signing issues

iOS validation requires supported macOS/Xcode tooling. When the iOS head exists, verify:

- installed .NET Apple workloads;
- Xcode command-line selection;
- simulator/device availability;
- bundle ID;
- certificate/provisioning configuration.

Keep signing credentials outside source control.

## Linux native dependency issues

Avalonia desktop runtime requirements can vary by environment. Capture the actual missing library/error and document the affected distribution/version instead of installing unrelated desktop packages broadly.

## Browser rendering issues

Browser support is not yet implemented in the current source tree. When it is added, common areas to verify include:

- WebAssembly workload/toolchain;
- browser console/network errors;
- hosting base path;
- static asset paths;
- service-worker cache version;
- browser storage support;
- unsupported native dependencies.

## Tests fail only on one OS

Do not immediately weaken the assertion. Determine whether the failure is caused by:

- path separators/case sensitivity;
- locale/time zone assumptions;
- newline conventions;
- filesystem locking;
- platform numeric/runtime differences;
- missing native dependency.

Domain math tests should be culture-independent unless a test explicitly targets formatting.

## CI fails but local build passes

Compare:

- SDK versions;
- OS;
- environment variables;
- clean checkout versus local generated state;
- package cache behavior;
- warnings-as-errors;
- file-name case.

Reproduce with the same Release commands used in `.github/workflows`.

## Reporting unresolved build failures

Create a bug report with:

- commit SHA;
- platform/OS;
- `dotnet --info` output with sensitive paths reviewed;
- exact command;
- first meaningful error;
- minimal relevant log section.

Do not paste secrets or full private environment dumps.
