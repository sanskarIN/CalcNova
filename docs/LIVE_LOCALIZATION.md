# CalcNova Live Localization

CalcNova now has a semantic localization catalog plus a progressively migrated live Avalonia UI path. Catalog support alone is not treated as proof that every visible string is localized.

## Reviewed languages

Current reviewed catalogs:

- English (`en`, including regional fallback such as `en-IN`);
- Hindi (`hi`, including regional fallback such as `hi-IN`).

The language preference is persisted through shared settings. Unsupported culture selections are rejected/fallback safely through `AppLocalizer` and settings validation.

## Live-localized surfaces

The current shared shell updates immediately when culture changes for reviewed semantic strings including:

- app name/tagline/local-first header;
- all primary mode tab headers;
- Calculator title/subtitle/expression watermark and selected common actions;
- primary headings for Programmer, Unicode, Converter, Statistics, Equations, Matrices, Graphing, and Date/Duration;
- onboarding welcome, feature/privacy explanations, Skip, and Start calculating;
- Currency heading/privacy text, amount/ISO prompts, and Refresh rates;
- History heading, search, management actions, and export explanation/actions;
- Settings language/precision/history labels, Save/Reset, and accessibility preference checkboxes;
- About/support headings/actions and persistent local-first footer;
- Converter local preference/privacy notice;
- visible Graph viewport Pan/Zoom/Fit/Reset controls.

Dynamic user data, formula text, numeric results, unit IDs, ISO currency codes, URLs, email addresses, and technical identifiers are not translated as prose.

## Architecture

`AppStringKey` defines stable semantic identities. English and Hindi catalogs must each contain every key exactly once.

`ShellLocalization` maps reviewed legacy English XAML literals to semantic keys while the large shared XAML surface is migrated incrementally. `MainView` caches applicable visible controls and refreshes them on culture changes and tab realization.

CheckBox content uses the same semantic keys through an isolated `MainView` partial lifecycle, allowing Settings accessibility preferences to localize without duplicating a second catalog.

New dynamically inserted product surfaces such as the converter preference notice and Graph viewport toolbar read directly from the same `IAppLocalizer`.

## Why migration is incremental

CalcNova keeps parser syntax, persisted mathematical tokens, unit IDs, and other machine-facing contracts culture-independent. Visible UI migration is therefore done in compile/testable increments rather than mass-replacing strings in mathematical/domain code.

A catalog entry is not sufficient to advertise a language as fully supported. Compact-width layout, large text, screen-reader output, and terminology quality must still be reviewed on real targets.

## Validation

Run:

```bash
python tools/validate_localization_catalog.py .
python -m unittest tools.tests.test_validate_localization_catalog
```

Avalonia headless tests additionally verify Hindi shell headers, Calculator prompts, onboarding, Currency, History, Settings, About/footer, converter privacy notice, mode headings, and Graph viewport controls.

## Remaining localization work

Continue migrating remaining hard-coded shared XAML strings in focused groups, especially:

- detailed Programmer operation labels;
- deeper Converter search/recents/favorites labels;
- Statistics/Equations/Matrices field descriptions;
- Graph analysis/export labels and technical help text;
- Date/Duration field descriptions;
- dynamic status/error messages that are currently generated in view models.

Do not localize persisted mathematical syntax in a way that changes calculation semantics.
