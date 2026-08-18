#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
VERSION="${1:-0.1.0-dev}"
RID="${CALCNOVA_LINUX_RID:-linux-x64}"
OUT="$ROOT/artifacts/linux/$RID"
APPDIR="$OUT/CalcNova"

rm -rf "$OUT"
mkdir -p "$APPDIR/bin" "$APPDIR/share/applications" "$APPDIR/share/icons/hicolor/256x256/apps" "$APPDIR/share/metainfo"

python3 "$ROOT/tools/scripts/generate_brand_assets.py"

dotnet publish "$ROOT/src/CalcNova.Desktop/CalcNova.Desktop.csproj" \
  --configuration Release \
  --runtime "$RID" \
  --self-contained true \
  --output "$APPDIR/bin"

cp "$ROOT/packaging/linux/in.sanskar.calcnova.desktop" "$APPDIR/share/applications/"
cp "$ROOT/packaging/linux/in.sanskar.calcnova.metainfo.xml" "$APPDIR/share/metainfo/"
cp "$ROOT/assets/generated/linux/calcnova-256.png" "$APPDIR/share/icons/hicolor/256x256/apps/in.sanskar.calcnova.png"

cat > "$APPDIR/run-calcnova.sh" <<'EOF'
#!/usr/bin/env bash
set -euo pipefail
HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
exec "$HERE/bin/CalcNova.Desktop" "$@"
EOF
chmod +x "$APPDIR/run-calcnova.sh"

tar -C "$OUT" -czf "$OUT/CalcNova-${VERSION}-${RID}.tar.gz" CalcNova
printf 'Created %s\n' "$OUT/CalcNova-${VERSION}-${RID}.tar.gz"
