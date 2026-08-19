# CalcNova Localization

CalcNova should be ready for reviewed translations without allowing locale formatting to change mathematical meaning.

## Current implementation state

The application layer now contains a reviewed two-catalog localization foundation:

- `AppStringKey` defines stable semantic keys for core shell modes, common actions, common labels, status text, and baseline errors;
- `EnglishAppStrings` is the complete source-language catalog;
- `HindiAppStrings` is the complete reviewed Hindi semantic catalog for the currently defined key set;
- `IAppLocalizer` defines culture selection and lookup behavior;
- `AppLocalizer` validates every supported catalog at startup, exposes English and Hindi, accepts regional cultures such as `en-IN` and `hi-IN`, and rejects unsupported/unreviewed cultures without changing the active culture;
- `AppSettings.CultureName` persists the preferred application culture through native JSON and browser settings storage;
- `SettingsViewModel` exposes the reviewed supported-culture list, restores a saved culture, normalizes unsupported saved preferences back to English, and rejects unsupported selections during save;
- `MainViewModel` shares one localizer instance with the settings workflow so culture selection and application string lookup use the same active state;
- native and browser settings validation reject malformed culture names before they reach the application layer;
- source/test coverage checks catalog completeness, fallback behavior, English/Hindi regional selection, preference persistence, unsupported cultures, invalid culture names, and culture-change behavior;
- the SDK-independent localization validator now verifies both catalogs and has its own regression tests in CI.

This foundation is **not yet a claim that the shared XAML has been fully migrated to localized bindings**. Existing visible English strings still need incremental migration after the binding approach is validated with Avalonia compilation and UI tests. The supported semantic catalogs are English and Hindi, while the current shared XAML remains predominantly English.

## Supported semantic catalogs

### English

English remains the source language and fallback language.

Regional English preferences such as `en-IN` can be preserved as the active culture while using the reviewed English catalog. This allows locale-aware presentation work without inventing a separate translation.

### Hindi

Hindi (`hi`) and Hindi regional cultures such as `hi-IN` use the reviewed `HindiAppStrings` catalog.

The catalog currently covers the same semantic key set as English. New `AppStringKey` entries must be translated in both catalogs before the localization source validator can pass.

Hindi support should not be described as complete UI localization until hard-coded visible XAML strings, accessibility descriptions, units, date formatting, and all runtime text are migrated and visually reviewed.

## Persisted preference contract

The preferred culture is stored in `AppSettings.CultureName` and follows the same local settings repository abstraction as other CalcNova preferences.

Current rules:

- default culture is `en`;
- syntactically malformed stored culture names are rejected by persistence validation;
- well-formed but unsupported cultures are normalized to `en` by the application localization layer;
- supported English regional cultures may be persisted, for example `en-IN`;
- supported Hindi regional cultures may be persisted, for example `hi-IN`;
- selecting an unsupported culture through the settings view model does not overwrite a valid persisted preference;
- parser syntax and persisted mathematical meaning do not change when the UI culture changes.

## What should be localizable

Application resources should eventually cover:

- mode names;
- menu/navigation labels;
- settings;
- error messages;
- onboarding/help text;
- button semantic labels;
- accessibility descriptions;
- unit/category display names;
- date/time labels;
- empty states;
- About/Support text.

Mathematical symbols and function identifiers should be reviewed separately because translating them can make expression syntax ambiguous.

## Internal expression syntax

The parser's canonical mathematical syntax is culture-independent.

Internal numeric meaning should use invariant representations. Locale-specific presentation belongs at input/display boundaries.

This avoids cases where the same persisted expression means different things after a device locale change.

## Decimal separator

User-facing input may eventually accept the locale's decimal separator when it can be normalized unambiguously to the internal parser format.

Do not globally replace commas with decimal points because commas also separate function arguments.

A robust input layer must distinguish localized numeric entry from expression punctuation.

## Grouping separators

Thousands/grouping separators are presentation only. They should not silently become part of internal persisted expression meaning.

Copy actions may offer display-formatted and invariant/raw values if the distinction becomes useful.

## Unit names

Unit IDs and conversion definitions remain stable/invariant. Display names can be localized.

For example, a unit record may retain internal ID `km` even when its localized display name changes.

## Date/time formatting

History grouping can use locale-aware date/time presentation while storing timestamps in an unambiguous representation.

## Layout impact

Translations can be longer or structurally different from English. UI must avoid fixed-width assumptions for navigation labels, settings rows, word-based buttons, error messages, and dialog actions.

Hindi also requires verification at large text sizes because Devanagari glyph metrics and line heights can differ from Latin text. Scientific symbol buttons can remain compact, but their accessible names need localized strings.

The current compact/medium/expanded shell is a useful baseline for long-string testing, but target-language layout must still be validated rather than inferred from English behavior.

## Right-to-left languages

Future RTL support needs intentional testing for navigation direction, text alignment, expression editing, keypad ordering, mixed mathematical/RTL text, and directional icons.

Do not simply mirror mathematical expressions without confirming correct behavior.

## Translation quality

Machine-generated translations may be useful as draft material but should not be shipped as reviewed translations without human-quality checking, especially for mathematical, accessibility, privacy, and security terminology.

Every supported catalog must remain complete. The source validator rejects missing, unknown, and duplicate semantic keys.

## Resource organization

As localization infrastructure is expanded:

- keep `AppStringKey` entries stable and semantic;
- keep one reviewed catalog per supported language;
- validate that every supported catalog contains every required key;
- avoid concatenating translated fragments to build sentences;
- use formatting placeholders for values;
- add translator context for ambiguous terms;
- test missing-resource fallback;
- keep source strings out of calculation/domain code where they represent UI messages;
- never localize parser keywords or persisted unit IDs without an explicit canonicalization layer.

## Tests

Current automated source/test coverage includes:

- English and Hindi catalog completeness;
- English fallback;
- English regional culture selection;
- Hindi regional culture selection and semantic string lookup;
- unsupported/invalid culture rejection;
- culture-change event behavior;
- settings culture restore and fallback;
- persisted regional culture round-trip on native JSON settings storage;
- malformed persisted culture rejection;
- duplicate localization-key detection;
- localization preference source-contract validation in CI;
- localization-validator regression tests in CI.

Future localization tests should cover localized XAML/view-model binding refresh, long-string layout samples, decimal/grouping normalization, locale changes without corruption of stored values, history timestamp formatting, and RTL smoke tests when RTL locales are supported.
