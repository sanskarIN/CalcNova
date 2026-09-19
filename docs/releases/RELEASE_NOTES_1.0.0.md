# CalcNova 1.0.0

The first release of CalcNova — a local-first calculator that keeps your work on your device.

Thirteen modes share one workspace: Calculator, Programmer, Unicode Code Points, Converter,
Statistics, Equations, Matrices, Graphing, Date & Duration, Currency, History, Settings and About.

## Highlights

**Calculation**
- Standard and scientific arithmetic with a safe tokenizer and recursive-descent parser — no
  `eval`, no expression injection.
- Exact rational arithmetic on `BigInteger` numerator/denominator pairs, decimal arithmetic, and a
  bounded floating-point fallback, so results stay exact whenever exactness is possible.
- Engineering notation with exponents in multiples of three and 1–15 significant digits.
- Degrees, radians and gradians; percentage handled separately from modulo; repeated equals and
  full calculator memory.

**Beyond the keypad**
- Programmer mode: bases 2–36, 8/16/32/64/128-bit word sizes, signed and unsigned two's
  complement, bitwise operations and an accessible bit grid.
- Unicode code-point inspection with category, plane and UTF-8/UTF-16 metadata, resolved locally.
- Offline unit conversion across fourteen quantity families, plus currency conversion with a local
  cache that keeps working without a network.
- Descriptive and bivariate statistics including covariance, Pearson correlation and OLS
  regression; equation solving; matrix determinant, inverse, rank and linear systems.
- Graphing with bounded sampling, discontinuity segmentation, pointer and keyboard pan/zoom,
  nearest-point trace, and CSV and accessible SVG export.

**Local-first by design**
- Calculation history and preferences live in local application storage. Physical unit conversion
  is entirely offline. Currency rates are the one optional network feature and fall back to cached
  data.
- No account, no telemetry, no analytics.

**Accessibility and localization**
- A 44-DIP minimum interaction target throughout, with a 54-DIP baseline for calculator keys.
- Visible keyboard focus, a high-contrast mode, reduced motion, and adaptive layouts from phone to
  desktop.
- Keyboard-first operation, including `Ctrl+PageUp`/`Ctrl+PageDown` to cycle modes.
- Complete English and Hindi catalogues with live language switching.

**Interface**
- One palette per theme variant drives every surface, so light and dark are consistent throughout.
- The keypad reads at a glance: digits quiet, operators tinted, equals filled, clear
  warning-coloured.
- The display reads expression, then answer in the largest type on screen, then the engine's
  message.

## Platforms

| Platform | Asset | What it is |
| --- | --- | --- |
| Android | `CalcNova-android.apk` | Universal APK — arm64-v8a, armeabi-v7a, x86, x86_64. Install directly or with `adb install`. |
| Android | `CalcNova-android.aab` | App bundle for Google Play. Cannot be installed by hand. |
| Windows | `CalcNova-win-x64.zip`, `CalcNova-win-arm64.zip` | Self-contained folder; run `CalcNova.Desktop.exe`. No .NET install needed. |
| Linux | `CalcNova-linux-x64.zip`, `CalcNova-linux-arm64.zip` | Self-contained folder; run `CalcNova.Desktop` after `chmod +x`. |
| macOS | `CalcNova-osx-x64.zip`, `CalcNova-osx-arm64.zip` | Self-contained folder; unsigned and un-notarized. |
| Browser | `CalcNova-browser.zip` | WebAssembly build, installable as a PWA when served over HTTPS. |

Minimum Android version is 6.0 (API 23); the build targets API 36.

## Verifying what you downloaded

`SHA256SUMS.txt` lists every asset by its published name:

```bash
sha256sum -c SHA256SUMS.txt --ignore-missing
```

Each release also carries a CycloneDX 1.7 SBOM per artifact and a build provenance attestation:

```bash
gh attestation verify CalcNova-android.apk --repo sanskarIN/CalcNova
```

## Known limitations

- The macOS builds are unsigned and un-notarized, so Gatekeeper will object on first launch.
- There is no installer for any desktop platform — each is a self-contained folder in a zip.
- Currency rates depend on the configured provider being reachable; without it, cached rates are
  used and their age is shown.

## Release identity

Product version `1.0.0` · package version `1.0.0` · assembly and file version `1.0.0.0` ·
Android build code `10000` · application id `in.sanskar.calcnova` · Apache-2.0.
