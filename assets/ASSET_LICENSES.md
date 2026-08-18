# CalcNova Asset Licensing

This file records the licensing and provenance policy for visual assets stored under `assets/`.

## CalcNova-owned artwork

The following source artwork is original CalcNova project artwork and is distributed with CalcNova under the repository's Apache License 2.0 unless a file-specific notice states otherwise:

- `branding/calcnova-logo.svg`
- `branding/social-preview.svg`
- `branding/buy-me-a-coffee-support.svg`
- `icons/calcnova-icon.svg`

The Buy Me a Coffee support badge is project-owned artwork using a neutral coffee-cup treatment. It is not an official Buy Me a Coffee logo and must not be represented as official brand artwork.

## Generated assets

Files under `assets/generated/` are deterministic derivatives of CalcNova-owned source artwork produced by `tools/scripts/generate_brand_assets.py`. They are distributed under the same Apache License 2.0 terms as the source artwork.

Generated outputs may include browser/PWA PNG files, Windows ICO/PNG files, Linux PNG files, macOS iconset/ICNS files, and iOS AppIcon PNG files.

## Third-party assets

No third-party visual asset should be committed to `assets/` unless its redistribution terms have been verified and its provenance, copyright owner, license, and required attribution are recorded in this file before merge.

Third-party trademarks remain the property of their respective owners. CalcNova's repository license does not grant rights to third-party marks.

## Verification

Brand assets are generated and checked with:

```bash
python tools/scripts/generate_brand_assets.py
python tools/scripts/generate_brand_assets.py --check
```

See also [`README.md`](README.md) for the source-artwork and generation overview.
