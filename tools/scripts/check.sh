#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$repo_root"

if command -v python3 >/dev/null 2>&1; then
  python_cmd=python3
elif command -v python >/dev/null 2>&1; then
  python_cmd=python
else
  python_cmd=""
fi

if [ -n "$python_cmd" ]; then
  echo '==> Running SDK-independent release preflight'
  "$python_cmd" tools/release_preflight.py

  echo '==> Running repository contract tests'
  "$python_cmd" -m unittest discover -s tools/tests -t tools/tests -p 'test_*.py'
else
  echo "Python was not found, so the repository contract checks were skipped. CI still enforces them." >&2
fi

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
