[CmdletBinding()]
param(
    [switch]$SkipFormat,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
Push-Location $repoRoot

try {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        throw 'The dotnet CLI was not found. Install the SDK selected by global.json and reopen the terminal.'
    }

    Write-Host '==> dotnet --info'
    dotnet --info

    Write-Host '==> Restoring CalcNova.slnx'
    dotnet restore CalcNova.slnx

    if (-not $SkipFormat) {
        Write-Host '==> Verifying formatting'
        dotnet format CalcNova.slnx --verify-no-changes --no-restore
    }

    Write-Host '==> Building Release configuration'
    dotnet build CalcNova.slnx --configuration Release --no-restore

    if (-not $SkipTests) {
        Write-Host '==> Running tests'
        dotnet test CalcNova.slnx --configuration Release --no-build
    }

    Write-Host 'CalcNova repository verification completed successfully.'
}
finally {
    Pop-Location
}
