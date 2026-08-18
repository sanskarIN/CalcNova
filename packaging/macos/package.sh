#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
VERSION="${1:-0.1.0-dev}"
BUILD_NUMBER="${CALCNOVA_BUILD_NUMBER:-1}"
ARCH="$(uname -m)"
if [[ -n "${CALCNOVA_MACOS_RID:-}" ]]; then
  RID="$CALCNOVA_MACOS_RID"
elif [[ "$ARCH" == "arm64" ]]; then
  RID="osx-arm64"
else
  RID="osx-x64"
fi

OUT="$ROOT/artifacts/macos/$RID"
PUBLISH="$OUT/publish"
APP="$OUT/CalcNova.app"
CONTENTS="$APP/Contents"

rm -rf "$OUT"
mkdir -p "$PUBLISH" "$CONTENTS/MacOS" "$CONTENTS/Resources"

python3 "$ROOT/tools/scripts/generate_brand_assets.py"

dotnet publish "$ROOT/src/CalcNova.Desktop/CalcNova.Desktop.csproj" \
  --configuration Release \
  --runtime "$RID" \
  --self-contained true \
  --output "$PUBLISH"

cp -R "$PUBLISH/." "$CONTENTS/MacOS/"
cp "$ROOT/assets/generated/macos/CalcNova.icns" "$CONTENTS/Resources/CalcNova.icns"

python3 - "$ROOT/packaging/macos/Info.plist.template" "$CONTENTS/Info.plist" "$VERSION" "$BUILD_NUMBER" <<'PY'
from pathlib import Path
import sys
source, destination, version, build = sys.argv[1:]
text = Path(source).read_text(encoding="utf-8")
text = text.replace("__VERSION__", version).replace("__BUILD_NUMBER__", build)
Path(destination).write_text(text, encoding="utf-8")
PY

chmod +x "$CONTENTS/MacOS/CalcNova.Desktop"

if [[ -n "${CALCNOVA_CODESIGN_IDENTITY:-}" ]]; then
  codesign --force --deep --options runtime --sign "$CALCNOVA_CODESIGN_IDENTITY" "$APP"
fi

(
  cd "$OUT"
  ditto -c -k --sequesterRsrc --keepParent "CalcNova.app" "CalcNova-${VERSION}-${RID}.zip"
)

printf 'Created %s\n' "$OUT/CalcNova-${VERSION}-${RID}.zip"
