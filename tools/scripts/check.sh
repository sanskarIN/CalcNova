#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$repo_root"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "The dotnet CLI was not found. Install the SDK selected by global.json and reopen the terminal." >&2
  exit 1
fi

echo '==> dotnet --info'
dotnet --info

echo '==> Restoring CalcNova.slnx'
dotnet restore CalcNova.slnx

echo '==> Verifying formatting'
dotnet format CalcNova.slnx --verify-no-changes --no-restore

echo '==> Building Release configuration'
dotnet build CalcNova.slnx --configuration Release --no-restore

echo '==> Running tests'
dotnet test CalcNova.slnx --configuration Release --no-build

echo 'CalcNova repository verification completed successfully.'
