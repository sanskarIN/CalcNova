# CalcNova Privacy

CalcNova is designed as a local-first calculator. This document describes the intended open-source baseline and must be updated whenever implementation changes data handling.

## Core calculations

Standard, scientific, programmer, fixed-unit conversion, and other offline mathematical features are intended to run locally on the user's device.

Ordinary calculation does not require an account.

## Calculation history

History is local by default.

The current native persistence implementation stores calculation history in a local SQLite database. Application/platform composition must choose a device-appropriate local storage location.

Browser/WebAssembly will require browser-compatible storage rather than native SQLite.

Future history controls should include:

- enable/disable history;
- delete one or multiple entries;
- clear all;
- optional maximum history size;
- optional auto-cleanup;
- user-initiated export.

## Settings

Settings are intended to be stored locally. Theme, precision, angle mode, accessibility preferences, and similar configuration should not require cloud storage.

## Analytics and advertising

The open-source baseline does not intentionally include:

- advertising SDKs;
- behavioral tracking;
- remote analytics enabled by default.

If telemetry is ever proposed, it must be separate, documented, minimized, and explicitly opt-in. Calculation expressions/history must not be collected as ordinary telemetry.

## Network features

Most CalcNova functionality should work without internet access.

A future currency converter is an example of a feature that may access the network for current exchange-rate data. If implemented, it must:

- be optional;
- identify the provider;
- use TLS;
- avoid embedding secret API keys in public source;
- cache recent rates locally;
- display rate timestamp/staleness;
- allow network features to be disabled;
- avoid uploading calculation history.

## Crash reporting

No remote crash-reporting system should be silently enabled. If one is added later, this document must list the actual fields sent and the user control available.

## Clipboard

Copy/paste features interact with the device clipboard only when the user invokes them. CalcNova should not continuously read or remotely upload clipboard contents.

## File export/import

Exports are user-initiated. Imports must be treated as untrusted input and validated before use.

Potential export formats include history text/CSV and safe settings JSON. Platform file pickers/storage permissions should be used only when required by an explicit user action.

## Permissions

CalcNova should request the minimum permissions necessary for a feature. A calculator should not request unrelated contacts, location, microphone, camera, or similar permissions.

## External links

About/support/help links may open an external browser. Link destinations should be allowlisted or otherwise constrained so untrusted calculation text cannot become an arbitrary external URL.

## Cloud synchronization

Cloud sync is not enabled by default and is not part of the current baseline. If introduced later, it must be opt-in and documented separately, including authentication, encryption, retention, deletion, and provider data handling.

## Store privacy disclosures

Any store privacy disclosure must match the actual release build. Do not claim that a release collects nothing if a network provider, crash reporter, analytics service, or another dependency transmits data.

## Contact

Questions about CalcNova privacy can be sent to:

**supportramsandesh@gmail.com**
