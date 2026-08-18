# CalcNova Full Baseline Validation

This document defines the repository validation gate used before a release milestone is marked complete.

## Core solution

Run on Linux, Windows, and macOS where supported:

```bash
dotnet restore CalcNova.slnx
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

The solution includes the core calculator engine, scientific, programmer, fixed-unit converter, statistics, equations, matrices, graphing, date/time utilities, currency abstraction/cache tests, persistence, shared Avalonia application, desktop host, and application-level tests.

## Platform heads

Validate independently so one unavailable workload does not hide unrelated results:

```bash
dotnet build src/CalcNova.Desktop/CalcNova.Desktop.csproj --configuration Release
dotnet build src/CalcNova.Android/CalcNova.Android.csproj --configuration Release
dotnet publish src/CalcNova.Browser/CalcNova.Browser.csproj --configuration Release
```

For iOS, use a macOS runner with the iOS workload and an appropriate simulator runtime identifier. Device signing and App Store archive validation are separate release checks and must never be marked PASS without the required Apple environment and credentials.

## Required interpretation

- `PASS` means the command actually completed successfully.
- `FAIL` means the command ran and returned a failure.
- `NOT RUN` means the required SDK, workload, signing environment, or platform was unavailable.
- Warnings and analyzer findings must be reviewed rather than hidden through blanket suppression.
- No signing secret, API token, keystore, private key, or service credential belongs in this repository.

## Current execution environment

The ChatGPT execution container used during the August 18, 2026 development segment does not provide the .NET SDK. Local `dotnet` validation therefore remains **NOT RUN** there; GitHub Actions is the intended independent validation environment for this baseline.
