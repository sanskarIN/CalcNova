#!/usr/bin/env python3
"""Generate CalcNova raster/package icon assets using only Python's standard library.

The generated artwork is derived from the repository-owned geometric CalcNova mark.
No external fonts, images, design binaries, or network resources are required.
"""

from __future__ import annotations

import argparse
import binascii
import struct
import zlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]

BG = (24, 32, 51, 255)
FG = (248, 250, 252, 255)
ACCENT = (255, 206, 75, 255)


def _chunk(kind: bytes, payload: bytes) -> bytes:
    return struct.pack(">I", len(payload)) + kind + payload + struct.pack(">I", binascii.crc32(kind + payload) & 0xFFFFFFFF)


def _png(width: int, height: int, pixels: bytearray) -> bytes:
    stride = width * 4
    raw = bytearray()
    for row in range(height):
        raw.append(0)
        start = row * stride
        raw.extend(pixels[start : start + stride])

    header = struct.pack(">IIBBBBB", width, height, 8, 6, 0, 0, 0)
    return b"\x89PNG\r\n\x1a\n" + _chunk(b"IHDR", header) + _chunk(b"IDAT", zlib.compress(bytes(raw), 9)) + _chunk(b"IEND", b"")


def _canvas(size: int, color: tuple[int, int, int, int]) -> bytearray:
    return bytearray(color * (size * size))


def _paint(pixels: bytearray, size: int, x: int, y: int, color: tuple[int, int, int, int]) -> None:
    if 0 <= x < size and 0 <= y < size:
        offset = (y * size + x) * 4
        pixels[offset : offset + 4] = bytes(color)


def _rect(
    pixels: bytearray,
    size: int,
    left: int,
    top: int,
    right: int,
    bottom: int,
    color: tuple[int, int, int, int],
) -> None:
    left = max(0, left)
    top = max(0, top)
    right = min(size, right)
    bottom = min(size, bottom)
    if left >= right or top >= bottom:
        return

    row = bytes(color) * (right - left)
    for y in range(top, bottom):
        start = (y * size + left) * 4
        pixels[start : start + len(row)] = row


def _rounded_rect(
    pixels: bytearray,
    size: int,
    left: int,
    top: int,
    right: int,
    bottom: int,
    radius: int,
    color: tuple[int, int, int, int],
) -> None:
    radius = max(0, min(radius, (right - left) // 2, (bottom - top) // 2))
    if radius == 0:
        _rect(pixels, size, left, top, right, bottom, color)
        return

    inner_left = left + radius
    inner_right = right - radius - 1
    inner_top = top + radius
    inner_bottom = bottom - radius - 1
    radius_sq = radius * radius

    for y in range(max(0, top), min(size, bottom)):
        for x in range(max(0, left), min(size, right)):
            nearest_x = min(max(x, inner_left), inner_right)
            nearest_y = min(max(y, inner_top), inner_bottom)
            dx = x - nearest_x
            dy = y - nearest_y
            if dx * dx + dy * dy <= radius_sq:
                _paint(pixels, size, x, y, color)


def _diamond(
    pixels: bytearray,
    size: int,
    center_x: int,
    center_y: int,
    radius: int,
    color: tuple[int, int, int, int],
) -> None:
    for dy in range(-radius, radius + 1):
        span = radius - abs(dy)
        _rect(pixels, size, center_x - span, center_y + dy, center_x + span + 1, center_y + dy + 1, color)


def render_icon(size: int, *, maskable: bool = False) -> bytes:
    if size < 16:
        raise ValueError("Icon size must be at least 16 pixels.")

    pixels = _canvas(size, BG)
    scale = size / 512.0

    if maskable:
        body = (132, 112, 380, 408, 54)
        display = (164, 150, 348, 220, 18)
        keys = [
            (164, 252, 214, 302, 14),
            (231, 252, 281, 302, 14),
            (298, 252, 348, 302, 14),
            (164, 319, 214, 369, 14),
            (231, 319, 281, 369, 14),
        ]
        spark = (378, 121, 46)
        plus_center = (316, 348)
    else:
        body = (112, 88, 396, 428, 62)
        display = (148, 130, 360, 212, 22)
        keys = [
            (148, 250, 206, 308, 17),
            (228, 250, 286, 308, 17),
            (308, 250, 366, 308, 17),
            (148, 330, 206, 388, 17),
            (228, 330, 286, 388, 17),
        ]
        spark = (393, 98, 53)
        plus_center = (335, 366)

    def s(value: int) -> int:
        return max(1, round(value * scale))

    _rounded_rect(pixels, size, s(body[0]), s(body[1]), s(body[2]), s(body[3]), s(body[4]), FG)
    _rounded_rect(pixels, size, s(display[0]), s(display[1]), s(display[2]), s(display[3]), s(display[4]), BG)
    for key in keys:
        _rounded_rect(pixels, size, s(key[0]), s(key[1]), s(key[2]), s(key[3]), s(key[4]), BG)

    _diamond(pixels, size, s(spark[0]), s(spark[1]), s(spark[2]), ACCENT)

    cx, cy = s(plus_center[0]), s(plus_center[1])
    half = max(2, s(9))
    arm = max(2, s(24))
    _rect(pixels, size, cx - half, cy - arm, cx + half + 1, cy + arm + 1, ACCENT)
    _rect(pixels, size, cx - arm, cy - half, cx + arm + 1, cy + half + 1, ACCENT)

    return _png(size, size, pixels)


def _write(path: Path, payload: bytes) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(payload)
    print(path.relative_to(ROOT))


def _write_ico(path: Path, png_payload: bytes, size: int = 256) -> None:
    width = 0 if size >= 256 else size
    height = 0 if size >= 256 else size
    header = struct.pack("<HHH", 0, 1, 1)
    directory = struct.pack("<BBBBHHII", width, height, 0, 0, 1, 32, len(png_payload), 6 + 16)
    _write(path, header + directory + png_payload)


def _write_icns(path: Path, payloads: dict[str, bytes]) -> None:
    chunks = bytearray()
    for kind, payload in payloads.items():
        encoded_kind = kind.encode("ascii")
        chunks.extend(encoded_kind)
        chunks.extend(struct.pack(">I", len(payload) + 8))
        chunks.extend(payload)

    _write(path, b"icns" + struct.pack(">I", len(chunks) + 8) + bytes(chunks))


def generate() -> None:
    cache: dict[tuple[int, bool], bytes] = {}

    def icon(size: int, maskable: bool = False) -> bytes:
        key = (size, maskable)
        if key not in cache:
            cache[key] = render_icon(size, maskable=maskable)
        return cache[key]

    # Browser/PWA raster fallbacks.
    _write(ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-192.png", icon(192))
    _write(ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-512.png", icon(512))
    _write(ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-maskable-512.png", icon(512, True))

    # Linux / generic desktop raster sources.
    _write(ROOT / "assets/generated/linux/calcnova-256.png", icon(256))
    _write(ROOT / "assets/generated/linux/calcnova-512.png", icon(512))

    # Windows modern ICO (PNG-compressed 256x256 image inside ICO container).
    windows_png = icon(256)
    _write(ROOT / "assets/generated/windows/calcnova-256.png", windows_png)
    _write_ico(ROOT / "assets/generated/windows/CalcNova.ico", windows_png)

    # macOS iconset and modern PNG-compressed ICNS chunks.
    mac_sizes = {
        "icon_16x16.png": 16,
        "icon_16x16@2x.png": 32,
        "icon_32x32.png": 32,
        "icon_32x32@2x.png": 64,
        "icon_128x128.png": 128,
        "icon_128x128@2x.png": 256,
        "icon_256x256.png": 256,
        "icon_256x256@2x.png": 512,
        "icon_512x512.png": 512,
        "icon_512x512@2x.png": 1024,
    }
    mac_dir = ROOT / "assets/generated/macos/CalcNova.iconset"
    for filename, size in mac_sizes.items():
        _write(mac_dir / filename, icon(size))

    _write_icns(
        ROOT / "assets/generated/macos/CalcNova.icns",
        {
            "icp4": icon(16),
            "icp5": icon(32),
            "icp6": icon(64),
            "ic07": icon(128),
            "ic08": icon(256),
            "ic09": icon(512),
            "ic10": icon(1024),
        },
    )

    # iOS universal AppIcon asset catalog PNGs.
    ios_icons = {
        "AppIcon-20@2x.png": 40,
        "AppIcon-20@3x.png": 60,
        "AppIcon-29@2x.png": 58,
        "AppIcon-29@3x.png": 87,
        "AppIcon-40@2x.png": 80,
        "AppIcon-40@3x.png": 120,
        "AppIcon-60@2x.png": 120,
        "AppIcon-60@3x.png": 180,
        "AppIcon-20-ipad.png": 20,
        "AppIcon-20@2x-ipad.png": 40,
        "AppIcon-29-ipad.png": 29,
        "AppIcon-29@2x-ipad.png": 58,
        "AppIcon-40-ipad.png": 40,
        "AppIcon-40@2x-ipad.png": 80,
        "AppIcon-76.png": 76,
        "AppIcon-76@2x.png": 152,
        "AppIcon-83.5@2x.png": 167,
        "AppIcon-1024.png": 1024,
    }
    ios_dir = ROOT / "src/CalcNova.iOS/Assets.xcassets/AppIcon.appiconset"
    for filename, size in ios_icons.items():
        _write(ios_dir / filename, icon(size))


def verify() -> None:
    expected = [
        ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-192.png",
        ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-512.png",
        ROOT / "src/CalcNova.Browser/wwwroot/icons/calcnova-maskable-512.png",
        ROOT / "assets/generated/windows/CalcNova.ico",
        ROOT / "assets/generated/macos/CalcNova.icns",
        ROOT / "src/CalcNova.iOS/Assets.xcassets/AppIcon.appiconset/AppIcon-1024.png",
    ]
    missing = [path for path in expected if not path.is_file() or path.stat().st_size == 0]
    if missing:
        formatted = "\n".join(str(path.relative_to(ROOT)) for path in missing)
        raise SystemExit(f"Missing generated brand assets:\n{formatted}\nRun generate_brand_assets.py first.")
    print("Generated CalcNova brand assets are present.")


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--check", action="store_true", help="Verify key generated outputs exist instead of generating them.")
    args = parser.parse_args()
    if args.check:
        verify()
    else:
        generate()


if __name__ == "__main__":
    main()
