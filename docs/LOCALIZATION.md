# CalcNova Localization

CalcNova should be ready for reviewed translations without allowing locale formatting to change mathematical meaning.

## Current implementation state

The application layer now contains an initial localization foundation:

- `AppStringKey` defines stable semantic keys for core shell modes, common actions, common labels, status text, and baseline errors;
- `EnglishAppStrings` is the complete initial source-language catalog;
- `IAppLocalizer` defines culture selection and lookup behavior;
- `AppLocalizer` validates catalog completeness at startup, exposes supported cultures, accepts English regional cultures, and rejects unsupported/unreviewed cultures without changing the active culture;
- tests cover catalog completeness, fallback behavior, regional English selection, unsupported cultures, invalid culture names, and culture-change notification behavior.

This foundation is **not yet a claim that the shared XAML has been fully migrated to localized bindings**. Existing visible English strings should be migrated incrementally after the binding approach is validated with Avalonia compilation and UI tests. Until reviewed translations are added, English remains the only supported source language.

## Source language

English is the initial source language.

Additional languages should be added only when translations can be reviewed for correctness and UI fit. A new language must not be added to `SupportedCultures` merely because a machine-generated draft exists.

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

Copy actions may offer:

- display-formatted result;
- invariant/raw value;

if the distinction becomes useful.

## Unit names

Unit IDs and conversion definitions remain stable/invariant. Display names can be localized.

For example, a unit record may retain internal ID `km` even when its localized display name changes.

## Date/time formatting

History grouping can use locale-aware date/time presentation while storing timestamps in an unambiguous representation.

## Layout impact

Translations can be longer than English. UI must avoid fixed-width assumptions for:

- navigation labels;
- settings rows;
- buttons with words;
- error messages;
- dialog actions.

Scientific symbol buttons can remain compact, but their accessible names need localized strings.

The current compact/medium/expanded shell is a useful baseline for long-string testing, but target-language layout must still be validated rather than inferred from English behavior.

## Right-to-left languages

Future RTL support needs intentional testing for:

- navigation direction;
- text alignment;
- expression editing;
- keypad ordering;
- mixed mathematical/RTL text;
- icons with directional meaning.

Do not simply mirror mathematical expressions without confirming correct behavior.

## Translation quality

Machine-generated translations may be useful as draft material but should not be shipped as reviewed translations without human-quality checking, especially for mathematical, accessibility, privacy, and security terminology.

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

Current automated source tests cover:

- source-catalog completeness;
- English fallback;
- English regional culture selection;
- unsupported/invalid culture rejection;
- culture-change event behavior.

Future localization tests should cover:

- localized XAML/view-model binding refresh;
- long-string layout samples;
- decimal/grouping normalization;
- locale changes without corruption of stored values;
- history timestamp formatting;
- RTL layout smoke tests when RTL locales are supported.
