# Changelog

All notable CalcNova changes are recorded here.

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
- Added the `VIBRATE` permission and declared the vibrator feature as not required, so CalcNova still installs on devices without one.
- Added night-mode resources so a dark-themed device no longer flashes a light splash screen before the app paints.
- Declared a locale configuration for English and Hindi, which surfaces CalcNova in the Android 13+ per-app language picker.
- Added backup and data-extraction rules for both the pre-12 and 12+ APIs: history and settings restore onto a new device, and the reconstructible currency-rate cache does not.
- Opted in to the Android 13+ predictive back gesture.
- Moved the launcher label to a string resource instead of repeating a literal.
- Release builds now produce the Android app bundle Google Play distributes, and CI packages, verifies and uploads it rather than only compiling.

### iOS

- Implemented haptic feedback over UIKit's impact and notification generators, so the setting's promise of haptics on mobile targets holds on both mobile platforms.
