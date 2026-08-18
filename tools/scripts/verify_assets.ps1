[CmdletBinding()]
param(
    [switch]$Generate
)

$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$generator = Join-Path $repoRoot 'tools/scripts/generate_brand_assets.py'

$python = Get-Command python -ErrorAction SilentlyContinue
if (-not $python) {
    $python = Get-Command python3 -ErrorAction SilentlyContinue
}
if (-not $python) {
    throw 'Python 3 is required to verify CalcNova generated brand assets.'
}

$requiredSources = @(
    'assets/branding/calcnova-logo.svg',
    'assets/branding/social-preview.svg',
    'assets/branding/buy-me-a-coffee-support.svg',
    'assets/icons/calcnova-icon.svg',
    'src/CalcNova.Android/Resources/drawable/ic_launcher_foreground.xml',
    'src/CalcNova.Browser/wwwroot/icons/calcnova.svg',
    'src/CalcNova.Browser/wwwroot/icons/calcnova-maskable.svg',
    'src/CalcNova.iOS/Assets.xcassets/AppIcon.appiconset/Contents.json'
)

foreach ($relativePath in $requiredSources) {
    $path = Join-Path $repoRoot $relativePath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) {
        throw "Required branding source is missing: $relativePath"
    }
}

if ($Generate) {
    & $python.Source $generator
    if ($LASTEXITCODE -ne 0) {
        throw 'CalcNova brand asset generation failed.'
    }
}

& $python.Source $generator --check
if ($LASTEXITCODE -ne 0) {
    throw 'CalcNova generated brand asset verification failed.'
}

Write-Host 'CalcNova branding sources and generated outputs are valid.'
