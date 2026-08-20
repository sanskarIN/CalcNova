# CalcNova 2.8.03 Privacy

CalcNova is designed as a local-first calculator. This document describes the completed open-source 2.8.03 baseline and must be updated whenever implementation changes data handling, storage, permissions, networking, telemetry, or third-party services.

## Privacy summary

For ordinary calculator use:

- no account is required;
- mathematical expressions are evaluated locally;
- fixed-unit conversion is local/offline;
- Unicode metadata is derived locally;
- history and settings are stored locally through target-appropriate storage;
- clipboard access is user-triggered;
- history export is user-triggered;
- network-enhanced currency refresh is optional and separated from ordinary calculation;
- the open-source baseline does not intentionally include advertising or behavioral-tracking SDKs;
- no provider credential is embedded in public source.

## Core calculations

Standard, scientific, programmer, exact-rational, engineering-notation, statistics, equation, matrix, graph/numerical, date/time, and fixed-unit conversion features are designed to operate locally on the user's device.

Ordinary calculation does not require an account or remote calculation service.

CalcNova uses a project-owned parser/evaluator rather than sending expressions to a remote code-execution service.

## Calculation history

History is local by default.

Native composition uses a local SQLite-backed history implementation behind an application-facing repository abstraction.

Browser/WebAssembly uses Browser-safe local storage composition rather than native SQLite.

Current history workflows include:

- recent history;
- search;
- favorites;
- delete;
- clear;
- bounded history behavior;
- user-initiated TXT/CSV/JSON export;
- bounded on-screen export previews while preserving the complete private copy/export payload where designed.

History is not intended to be uploaded as ordinary telemetry.

Users and packagers should treat history/export files as private user data because they can contain entered expressions and results.

## Settings and preferences

Settings are local-first and stored through target-appropriate repositories.

Persisted state includes supported preferences such as:

- theme/display preferences;
- angle mode;
- culture/language preference;
- accessibility-related preferences;
- converter precision/state;
- converter recents/favorites;
- onboarding state;
- other supported application settings.

Settings use a versioned schema with migration behavior for supported older/unversioned forms and fail-closed handling for unsupported future schemas.

An older/unsupported schema consumer should not silently overwrite a newer unsupported settings schema.

See [SETTINGS_STORAGE_CONTRACT.md](SETTINGS_STORAGE_CONTRACT.md) and [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

## Browser storage

Browser/WebAssembly composition does not use the native SQLite history path.

Browser persistence is intentionally composed through Browser-compatible local storage behavior and shared validation/decoding rules where appropriate.

Browser storage remains subject to browser/device constraints such as:

- site-data deletion;
- private/incognito-session behavior;
- storage quotas;
- browser permission/policy changes;
- origin/base-path differences.

Do not describe Browser local persistence as cloud synchronization.

## Currency conversion and network access

Currency infrastructure is the primary optional network-enhanced feature in the 2.8.03 baseline.

It is designed around:

- a replaceable rate-provider interface;
- local rate caching;
- offline fallback semantics;
- no provider credentials embedded in public source.

Ordinary calculator features do not require a currency-network request.

A configured online rate provider may receive normal request metadata inherent to network communication, such as the requesting IP address and HTTP metadata, according to that provider's own service/privacy terms. A distributor that configures or replaces a provider must update release privacy disclosures to match the actual provider and transmitted fields.

Currency networking must not upload calculation history as part of normal rate refresh behavior.

## Offline behavior

Most CalcNova functionality is local and remains usable without internet access.

Offline/local features include core calculation, scientific functions, exact rationals, engineering notation, programmer tools, Unicode metadata, fixed-unit conversion, date/time utilities, statistics, equations, matrices, graph/numerical analysis, local history, settings, and local export generation.

Currency conversion can use cached/offline behavior when fresh network rates are unavailable, subject to the configured provider/cache state.

## Analytics and advertising

The open-source 2.8.03 baseline does not intentionally include:

- advertising SDKs;
- behavioral tracking;
- remote analytics enabled by default.

If telemetry is introduced in a future maintenance/optional enhancement release, it must be separately documented, minimized, and subject to an explicit privacy review. Calculation expressions/history must not be collected as ordinary analytics telemetry.

Store/distributor builds must be reviewed independently if they add dependencies not present in the repository baseline.

## Crash reporting

The open-source baseline does not intentionally define a silently enabled remote crash-reporting pipeline.

If a future distributor or maintenance release adds remote crash reporting, the privacy documentation must identify:

- the provider;
- data fields transmitted;
- whether expressions/results can appear in diagnostic payloads;
- retention behavior where known;
- user controls/consent model;
- how to disable or opt out where applicable.

Sensitive calculation/history content should not be intentionally included in remote crash payloads.

## Clipboard

Copy/paste features interact with the device/browser clipboard only after an explicit user action or user-triggered workflow.

CalcNova should not continuously poll, read, or remotely upload clipboard contents.

Browser/mobile operating systems may apply their own clipboard permission prompts or restrictions. A permission denial/failure should be handled as a local interaction failure rather than a reason to transmit clipboard data elsewhere.

## Expression import

Externally supplied expression text is treated as untrusted input and sanitized/validated before evaluation.

The calculation engine parses mathematical expressions; it does not execute imported text as arbitrary source code.

## Export

History export is user-initiated.

Current export generation supports bounded TXT/CSV/JSON workflows. Exported content may contain private calculations and should be stored/shared only where the user intends.

CalcNova does not treat export generation as permission to upload the generated file to a remote service.

Any future platform share/cloud integration must remain an explicit user action and must be documented if it introduces a new data recipient.

## External links

About/support/help/donation links can open an external browser through platform-facing link abstractions.

External navigation is separate from calculation processing. Untrusted calculation text should not be treated as an arbitrary external URL.

Once an external site is opened, that site's own privacy practices apply.

## Permissions

CalcNova should request only permissions required by an enabled feature/platform workflow.

A calculator should not request unrelated access such as contacts, location, microphone, or camera unless a future explicit feature genuinely requires it and the privacy/security documentation is updated first.

Platform packaging/release review should verify the actual permission manifest generated for the release artifact.

## Cloud synchronization

Cloud synchronization is not part of the default 2.8.03 architecture.

If cloud sync is introduced in a later optional release, it must be opt-in and documented separately, including:

- authentication;
- data categories synchronized;
- encryption/transport;
- provider;
- retention/deletion;
- account deletion behavior;
- conflict handling;
- user controls.

Local history/settings must not silently become cloud-synchronized merely because a platform supports cloud storage.

## Signing, build, and repository secrets

Keystores, signing passwords, Apple certificates/provisioning profiles, private keys, store credentials, API tokens, service-account secrets, and provider credentials must remain outside source control.

The Android release workflow uses external CI secrets and removes temporary keystore material after publication attempts.

Secrets are build/release data, not user calculation data, but leaking them can compromise distributed application integrity and therefore is also a privacy/security concern.

## Store privacy disclosures

Any app-store or distributor privacy disclosure must match the actual release artifact, not only this repository document.

Before publication, review:

- configured currency provider/network behavior;
- any distributor-added crash/analytics SDK;
- permissions;
- Browser/native storage behavior;
- third-party dependencies;
- external links;
- platform/store requirements.

Do not claim that a release transmits no data if its actual configured provider or added dependency transmits data.

## Data deletion/reset expectations

Current local history workflows include delete/clear operations. Settings/history files also remain subject to normal platform application-data removal behavior.

A destructive reset should be explicit. Troubleshooting should not silently delete history/settings as a first repair step.

## Privacy review rule

Any change that adds or changes one of the following requires this document to be reviewed in the same maintenance change:

- network endpoint/provider;
- telemetry/crash reporting;
- storage location/schema;
- cloud synchronization;
- permissions;
- clipboard behavior;
- file/share integration;
- account/authentication;
- third-party SDK with data collection;
- advertising/analytics.

## Related documentation

- [Security engineering](SECURITY.md)
- [Input safety](INPUT_SAFETY.md)
- [Settings storage contract](SETTINGS_STORAGE_CONTRACT.md)
- [Settings migration](SETTINGS_MIGRATION.md)
- [Converter defaults and privacy](CONVERTER_DEFAULTS_AND_PRIVACY.md)
- [Platform support](PLATFORM_SUPPORT.md)
- [Building](BUILDING.md)

## Contact

Questions about CalcNova privacy can be sent to:

**supportramsandesh@gmail.com**
