# CalcNova Assets

CalcNova branding in this repository is original project artwork.

## Source artwork

- `branding/calcnova-logo.svg` — master horizontal logo and tagline.
- `icons/calcnova-icon.svg` — master square application icon.
- `branding/social-preview.svg` — 1280×640 repository/social preview source.
- `branding/buy-me-a-coffee-support.svg` — original neutral coffee-cup support badge; it does not copy official Buy Me a Coffee artwork.

## Generated platform assets

Run:

```bash
python tools/scripts/generate_brand_assets.py
```

or on systems where Python 3 is exposed separately:

```bash
python3 tools/scripts/generate_brand_assets.py
```

The generator uses only Python's standard library and writes deterministic project-owned raster/package files for:

- Browser/PWA PNG fallbacks;
- Windows PNG/ICO;
- Linux PNG;
- macOS iconset/ICNS;
- iOS AppIcon PNGs.

Verify key outputs with:

```bash
python tools/scripts/generate_brand_assets.py --check
```

## Licensing

CalcNova-owned branding may be redistributed with the project under the repository license unless a future asset-specific notice states otherwise. Third-party marks must never be introduced into the CalcNova brand without verifying redistribution/brand terms first.
