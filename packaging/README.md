# CalcNova Packaging

This directory contains maintainable packaging metadata/templates for desktop distribution.

## Principles

- Build CalcNova from `src/CalcNova.Desktop`.
- Generate project-owned platform icons with `tools/scripts/generate_brand_assets.py`.
- Keep signing identities, certificates, notarization credentials, keystores, passwords, and private keys outside the repository.
- Treat generated package files as release artifacts rather than source files.
- Use `in.sanskar.calcnova` as the common application identifier where the target platform permits it.

## Directories

- `linux/` — freedesktop desktop entry and AppStream metadata.
- `macos/` — application bundle metadata template.
- `windows/` — MSIX/Appx manifest template for an optional packaged release.

The ordinary Avalonia desktop executable does not require these packaging formats for development builds. They are release-layer metadata intended for platform-native distribution workflows.
