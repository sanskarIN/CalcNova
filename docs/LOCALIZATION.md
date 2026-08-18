# CalcNova Localization

CalcNova should be ready for reviewed translations without allowing locale formatting to change mathematical meaning.

## Source language

English is the initial source language.

Additional languages should be added only when translations can be reviewed for correctness and UI fit.

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

As localization infrastructure is implemented:

- keep resource keys stable and semantic;
- avoid concatenating translated fragments to build sentences;
- use formatting placeholders for values;
- add translator context for ambiguous terms;
- test missing-resource fallback;
- keep source strings out of calculation/domain code where they represent UI messages.

## Tests

Localization-related tests should eventually cover:

- resource completeness/fallback;
- long-string layout samples;
- decimal/grouping normalization;
- locale changes without corruption of stored values;
- history timestamp formatting;
- RTL layout smoke tests when RTL locales are supported.
