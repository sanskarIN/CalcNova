[CmdletBinding()]
param(
    [string]$Version = "0.1.0-dev",
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "../..")).Path
$out = Join-Path $root "artifacts/windows/$RuntimeIdentifier"
$publish = Join-Path $out "publish"
$archive = Join-Path $out "CalcNova-$Version-$RuntimeIdentifier.zip"

if (Test-Path $out) {
    Remove-Item $out -Recurse -Force
}
New-Item -ItemType Directory -Path $publish -Force | Out-Null

$python = Get-Command python -ErrorAction SilentlyContinue
if (-not $python) {
    $python = Get-Command python3 -ErrorAction SilentlyContinue
}
if (-not $python) {
    throw "Python 3 is required to generate CalcNova release icons."
}

& $python.Source (Join-Path $root "tools/scripts/generate_brand_assets.py")
if ($LASTEXITCODE -ne 0) {
    throw "CalcNova brand asset generation failed."
}

dotnet publish (Join-Path $root "src/CalcNova.Desktop/CalcNova.Desktop.csproj") `
    --configuration Release `
    --runtime $RuntimeIdentifier `
    --self-contained true `
    --output $publish
if ($LASTEXITCODE -ne 0) {
    throw "CalcNova Windows publish failed."
}

Copy-Item (Join-Path $root "assets/generated/windows/CalcNova.ico") (Join-Path $publish "CalcNova.ico") -Force
Compress-Archive -Path (Join-Path $publish "*") -DestinationPath $archive -Force
Write-Host "Created $archive"
Write-Host "Optional MSIX metadata is available at packaging/windows/AppxManifest.xml.template. Signing certificates and publisher identity are intentionally not stored in this repository."
