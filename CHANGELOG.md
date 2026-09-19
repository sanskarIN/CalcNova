# Changelog

All notable CalcNova changes are recorded here.

## Maintenance since 1.0.0

### Fixed

- **Android: the app crashed on every launch.** `CalcNovaTheme` inherited from the framework
  theme `android:Theme.Material`, but Avalonia hosts the shell on `AvaloniaMainActivity`, which
  derives from `androidx.appcompat.app.AppCompatActivity`. AppCompat rejects a non-AppCompat
  theme while inflating its decor, so `onCreate` threw `IllegalStateException: You need to use a
  Theme.AppCompat theme (or descendant) with this activity` before any CalcNova code ran. All
  four theme variants now inherit from `Theme.AppCompat`, and `validate_platform_support.py`
  checks each parent so the regression cannot return unnoticed.
- **First-run onboarding could not be dismissed on a short phone.** On a 360x800 device the
  overlay card was taller than the screen, and `Skip` and `Start calculating` were the last
  children of the card's scrolled content, so the only way out of onboarding was cut off and the
  app could not be reached at all. The card set `VerticalAlignment="Center"` inside the very
  ScrollViewer meant to scroll it, and content aligned inside a scroll viewport is measured
  against the viewport rather than the extent, so it clipped instead of scrolling. The card now
  stretches, so it can never exceed the space it has, and both buttons sit in their own row
  outside the scroll region where they keep their place however little room the prose has. The
  section headings also gained `TextWrapping`, since "Keyboard and touch friendly" is wider than
  a narrow phone card and was being cut off mid-heading.
- **Compact layouts panned sideways instead of fitting the window.** The adaptive layout gave
  every ScrollViewer in the shell — not only the mode strip — an unconstrained horizontal
  measure at compact width. Nothing inside a mode could then size itself to the window:
  WrapPanels such as the programmer bit grid laid out in one unwrapped line, and star-sized
  keypad columns stretched to the widest label in the pane, so keys sat off-screen and had to be
  panned to. Mode content panes are now horizontally constrained in every profile, as
  `docs/ADAPTIVE_LAYOUT.md` already specified, and the profile's horizontal-scrolling allowance
  applies to the mode strip alone. This was only reachable below 600 DIPs, which is every phone.

### Changed

- **The shell has a calculator's visual language.** It had no colour design of its own — every
  surface took the framework's defaults, so a calculator with thirteen modes read as a generic
  settings form. There is now one palette defined per theme variant, and every surface draws from
  it. The keypad is readable at a glance: digits stay quiet, operators carry a tint, equals is the
  one filled key and clear the one warning-coloured key. Keys fill their grid cell instead of
  being sized to their label and left-aligned in a column several times wider. The display is a
  panel reading expression, then answer in the largest type on screen, then the engine's message.
  The selected mode is a filled pill rather than an underline, which survives the strip wrapping
  on a phone. The onboarding overlay follows the palette instead of forcing a light card, so a
  device in dark mode no longer gets a white sheet across the screen.
- **Onboarding actions no longer depend on the theme.** They carry their own colours, because a
  hardcoded light card with theme-coloured buttons on it rendered them white-on-white in dark
  mode at a contrast ratio of 1.05:1 — present and tappable, but invisible.

- Removed the `android.hardware.vibrate` `uses-feature` declaration from the Android manifest.
  Android has no vibrator feature constant — the name is absent from the platform's own
  features list — so the entry declared a feature that does not exist and nothing reads, and
  `aapt2 dump badging` only echoed it back. `VIBRATE` is not one of the permissions Google Play
  maps to an implied feature requirement either, so the declaration never kept CalcNova
  installable on a device without a vibrator; `AndroidHapticFeedbackService`'s `HasVibrator`
  check is what actually does that. Because it was declared `android:required="false"`,
  removing it changes no install or store-filtering behaviour. `validate_platform_support.py`
  now fails if the line returns.

### Documentation

- `docs/BUILDING.md` now states that the Android workload accepts a JDK from 17 to 21 and
  refuses anything newer, and shows how to point `JAVA_HOME` at a supported JDK. A machine whose
  default `java` is 22 or later previously failed the Android build with no explanation of why.

## [1.0.0] - 2026-09-17

**Status: Complete**

Public/product version: `1.0.0`  
Package version: `1.0.0`  
Normalized release tag: `v1.0.0`  
Assembly/file version: `1.0.0.0`  
Mobile build code: `10000`

### Calculator

- Standard, scientific and exact-rational arithmetic, with engineering notation and a programmer mode covering bases 2–36, fixed-width bitwise operations, shifts and bit inspection.
- Unicode code-point inspection, unit conversion, statistics including bivariate regression, equation solving, matrices and graphing.
- Currency conversion with an offline-capable cache, calculation history with search, favourites and export, and persisted settings with schema migration.

### Platforms

- Desktop application heads for Windows, Linux and macOS.
- Browser/WebAssembly head published as an installable PWA.
- Android and iOS heads sharing the same view-model layer as the desktop head.

### Accessibility and localization

- English and Hindi throughout, with live language switching.
- Keyboard-first operation, adaptive layout from compact to expanded, high contrast and reduced motion.

### Android

- Implemented haptic feedback. The Haptics setting had been persisted, validated, localized and shown as a checkbox since before this release without any head implementing it; calculator input now asks the device for a short confirmation, with distinct patterns for an accepted key, a completed calculation and a rejected one.
- Added the `VIBRATE` permission, alongside an `android.hardware.vibrate` `uses-feature` declaration that was later found to be inert and removed (see Maintenance above). Devices without a vibrator are handled by the service's `HasVibrator` check.
- Added night-mode resources so a dark-themed device no longer flashes a light splash screen before the app paints.
- Declared a locale configuration for English and Hindi, which surfaces CalcNova in the Android 13+ per-app language picker.
- Added backup and data-extraction rules for both the pre-12 and 12+ APIs: history and settings restore onto a new device, and the reconstructible currency-rate cache does not.
- Opted in to the Android 13+ predictive back gesture.
- Moved the launcher label to a string resource instead of repeating a literal.
- Release builds produce both distribution shapes from one packaging pass: the app bundle Google Play distributes, and the universal APK bundletool derives from it, which is what a downloaded release actually installs. CI packages, verifies and uploads both rather than only compiling.
- The release workflow always builds and publishes Android. Every step used to be gated on the release signing secrets, so with none configured the release published no Android asset and still reported success. The secrets now decide only which key signs the packages: the project release key, or the throwaway key the Android SDK generates for that run, in which case the assets are named `-debug-signed` and the run raises a warning.

### iOS

- Implemented haptic feedback over UIKit's impact and notification generators, so the setting's promise of haptics on mobile targets holds on both mobile platforms.
