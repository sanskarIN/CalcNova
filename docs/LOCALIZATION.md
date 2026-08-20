# CalcNova 2.8.03 Localization

CalcNova's localization architecture keeps mathematical meaning culture-independent while providing reviewed live localization for the completed 2.8.03 semantic baseline.

The current reviewed languages are English and Hindi. Additional languages and further migration of non-semantic/technical UI text are optional post-2.8.03 improvements rather than incomplete release requirements.

## Completed localization baseline

The application layer includes:

- `AppStringKey` stable semantic keys;
- complete English semantic catalog for the current key set;
- complete Hindi semantic catalog for the current key set;
- `IAppLocalizer` culture-selection/lookup abstraction;
- `AppLocalizer` catalog validation and fallback behavior;
- regional English/Hindi culture selection such as `en-IN` and `hi-IN`;
- persisted `AppSettings.CultureName` preference;
- settings validation/fallback for malformed or unsupported cultures;
- shared localizer state used by application/settings workflows;
- live refresh for reviewed shared surfaces;
- source validators for catalog completeness, duplicate/unknown keys, preferences, and live-localization contracts;
- application/headless regression coverage for reviewed localized surfaces.

## Reviewed languages

### English

English is the source/fallback catalog.

Regional English preferences such as `en-IN` can remain the active culture while using the reviewed English semantic catalog.

### Hindi

Hindi (`hi`) and Hindi regional cultures such as `hi-IN` use the reviewed Hindi semantic catalog.

The Hindi catalog contains the same current semantic key set as English. Adding a new required semantic key requires catalog updates so completeness validation continues to pass.

## Reviewed live-localized surfaces

The 2.8.03 baseline includes reviewed live localization across major shared product surfaces, including:

- application shell/header and primary mode names;
- Calculator reviewed title/prompt/action surfaces;
- onboarding welcome/feature/privacy/action text;
- Settings reviewed labels/actions/accessibility preferences;
- History reviewed headings/search/management/export surfaces;
- Currency reviewed heading/privacy/input/refresh surfaces;
- About/support/footer surfaces;
- Converter local preference/privacy notice and related reviewed controls;
- major mode headings;
- Graph viewport controls and related reviewed strings.

Culture changes refresh the reviewed shared strings without changing mathematical parser semantics.

See [LIVE_LOCALIZATION.md](LIVE_LOCALIZATION.md) for the live-refresh architecture and exact scope.

## Semantic catalogs versus technical data

Not every visible token should be translated as prose.

The following generally remain invariant or are handled by specialist formatting rules:

- parser keywords/function identifiers;
- mathematical symbols;
- persisted mathematical syntax;
- stable unit IDs;
- ISO currency codes;
- URLs/email addresses;
- technical identifiers;
- user-entered expressions/data.

A localized display label must not silently change the canonical machine/persisted meaning.

## Persisted preference contract

The preferred culture is stored in `AppSettings.CultureName` through the shared local settings abstraction.

Current rules include:

- default culture: `en`;
- malformed stored culture names are rejected by persistence validation;
- well-formed but unsupported preferences fall back safely to English through application localization behavior;
- supported regional English/Hindi cultures can be persisted;
- unsupported selections do not replace a valid supported persisted preference;
- parser syntax/persisted mathematical meaning remains invariant across UI culture changes.

Native and Browser storage use the same settings schema/validation contract while retaining target-appropriate persistence implementations.

## Internal expression syntax

The expression parser remains culture-independent.

This avoids ambiguity such as a persisted expression changing meaning when the operating-system language/region changes.

Localized input presentation must normalize safely before it reaches canonical parser syntax.

## Decimal and grouping separators

Locale-aware display/input formatting must not blindly replace punctuation.

In particular, commas can represent function-argument separators, so a locale-aware decimal-input feature must distinguish localized numeric entry from expression punctuation.

Grouping separators are presentation concerns and must not silently alter persisted mathematical meaning.

## Units

Unit IDs and conversion definitions remain stable/invariant.

Localized display names can change without changing the persisted conversion-pair identity.

This separation allows converter state to survive a language change safely.

## Date/time presentation

Date/time labels and history timestamps may use locale-aware presentation while underlying stored timestamps/data remain unambiguous.

Formatting changes must not change the represented instant/duration/date calculation semantics.

## Layout impact

Localized strings may be longer or have different glyph/line metrics.

The adaptive UI must therefore avoid assuming English-width labels. Review should include:

- compact/medium/expanded layouts;
- large text;
- Hindi Devanagari line height/glyph metrics;
- navigation/tab headers;
- settings rows;
- errors/status text;
- onboarding;
- graph/converter/history actions.

Source/headless coverage reduces regression risk, but real target-language rendering remains runtime evidence.

## Accessibility localization

Accessible names/descriptions that are part of the reviewed semantic catalog should use localized semantic strings rather than relying only on visual English text.

Mathematical symbol keys may retain their visual symbol while exposing an understandable localized semantic name where the accessibility contract requires one.

Screen-reader pronunciation/quality must be validated on target assistive technologies; catalog presence alone is not runtime evidence.

## Right-to-left languages

RTL languages are not part of the reviewed 2.8.03 language baseline.

A future RTL language pack requires intentional design/testing for:

- navigation direction;
- alignment;
- expression editing;
- keypad ordering;
- mixed mathematical/RTL text;
- directional icons;
- accessibility reading order.

Do not simply mirror mathematical expressions without proving correct semantics.

## Translation quality

Machine translation may assist drafting, but a language must not be advertised as reviewed merely because automated text exists.

Mathematical, privacy, security, onboarding, and accessibility terminology requires quality review.

Every supported semantic catalog must remain complete. The source validator rejects missing, duplicate, and unknown keys.

## Resource organization

Localization maintenance should preserve these rules:

- keep semantic keys stable;
- keep one reviewed catalog per supported language;
- require every supported catalog to contain every required current semantic key;
- avoid sentence construction by concatenating translated fragments;
- use formatting placeholders for values;
- provide translator context for ambiguous terms;
- test fallback behavior;
- keep canonical parser/unit identifiers invariant;
- update live-localization/headless tests when a reviewed surface gains semantic localization.

## Validation

Focused checks include localization catalog/preferences/live-surface validators and their Python regression tests.

The integrated SDK-independent gate is:

```bash
python tools/release_preflight.py
```

Compiled/headless application localization coverage runs through the normal .NET test workflows described in [TESTING.md](TESTING.md) and [UI_AUTOMATION.md](UI_AUTOMATION.md).

Runtime language/layout/screen-reader evidence should be recorded using:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

## Optional post-2.8.03 localization work

Possible optional improvements include:

- additional reviewed language packs;
- further semantic migration of remaining technical/detail labels;
- richer locale-aware numeric entry where ambiguity can be resolved safely;
- additional localized date/time presentation;
- RTL support after dedicated design/testing.

These are optional enhancements, not missing requirements for the completed English/Hindi 2.8.03 semantic baseline.

## 2.8.03 classification

- semantic localization architecture: **COMPLETE**;
- English current semantic catalog: **COMPLETE**;
- Hindi current semantic catalog: **COMPLETE**;
- regional English/Hindi selection: **COMPLETE**;
- culture preference persistence: **COMPLETE**;
- reviewed live localized major surfaces: **COMPLETE**;
- catalog/source validation: **COMPLETE**;
- additional languages/further text migration: **OPTIONAL POST-2.8.03**.
