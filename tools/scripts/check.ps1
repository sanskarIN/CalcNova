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
    $pythonCommand = $null
    foreach ($candidate in @('python3', 'python')) {
        if (Get-Command $candidate -ErrorAction SilentlyContinue) {
            $pythonCommand = $candidate
            break
        }
    }

    if ($pythonCommand) {
        Write-Host '==> Running SDK-independent release preflight'
        & $pythonCommand tools/release_preflight.py
        if ($LASTEXITCODE -ne 0) {
            throw "Release preflight failed with exit code $LASTEXITCODE."
        }

        Write-Host '==> Running repository contract tests'
        & $pythonCommand -m unittest discover -s tools/tests -t tools/tests -p 'test_*.py'
        if ($LASTEXITCODE -ne 0) {
            throw "Repository contract tests failed with exit code $LASTEXITCODE."
        }
    }
    else {
        Write-Warning 'Python was not found, so the repository contract checks were skipped. CI still enforces them.'
    }

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
