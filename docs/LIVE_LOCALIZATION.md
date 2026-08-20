# CalcNova 2.8.03 Live Localization

CalcNova has a semantic localization catalog and a live Avalonia refresh path for the reviewed English/Hindi 2.8.03 surfaces.

The completed baseline does not mean every technical token, user value, formula, unit ID, ISO code, URL, or remaining incidental detail string should be translated. It means the current reviewed semantic key set and protected major surfaces are implemented and validated as the 2.8.03 localization scope.

## Reviewed languages

Current reviewed semantic catalogs:

- English (`en`, including supported regional forms such as `en-IN`);
- Hindi (`hi`, including supported regional forms such as `hi-IN`).

The culture preference is persisted through shared settings. Malformed/unsupported selections are rejected or safely normalized according to the localization/settings contracts.

## Live-localized surfaces

The current shared shell refreshes reviewed semantic strings for major surfaces including:

- app name/tagline/local-first header;
- all primary mode tab headers;
- Calculator title/subtitle/expression prompt and reviewed common actions;
- primary headings for Programmer, Unicode, Converter, Statistics, Equations, Matrices, Graphing, and Date/Duration;
- onboarding welcome, capability/privacy explanations, Skip, and Start calculating;
- Currency heading/privacy text, amount/ISO prompts, and Refresh rates;
- History heading, search, management actions, export explanation/actions;
- Settings language/precision/history labels, Save/Reset, and accessibility preference controls;
- About/support headings/actions and local-first footer;
- Converter local preference/privacy notice and reviewed controls;
- visible Graph viewport Pan/Zoom/Fit/Reset controls.

Dynamic user data, expressions, formulas, numeric results, stable unit IDs, ISO currency codes, URLs, email addresses, and technical identifiers remain invariant or follow specialist formatting rather than being translated as prose.

## Architecture

`AppStringKey` defines stable semantic identities.

English and Hindi catalogs must each contain every required current semantic key exactly once.

`ShellLocalization` maps reviewed shared UI literals/surfaces into semantic localization behavior while CalcNova preserves canonical mathematical and persisted identifiers.

`MainView` refreshes applicable visible/realized shared controls when culture changes.

Settings accessibility preference controls and dynamically inserted reviewed product surfaces use the same localization state rather than maintaining unrelated per-feature translation stores.

## Why localization remains semantic

CalcNova intentionally does not mass-translate all strings mechanically.

The following must remain semantically stable:

- parser syntax;
- persisted mathematical tokens;
- stable conversion unit IDs;
- ISO currency codes;
- technical release/version identifiers;
- user-entered data.

Visible labels can be localized while the underlying identifiers remain invariant.

This avoids a language change altering calculation meaning or corrupting persisted state.

## Culture switching

Supported culture changes update reviewed shared surfaces during the running application.

The same localizer state is shared with settings so selected culture and displayed reviewed strings remain synchronized.

A culture change must not:

- reinterpret a stored mathematical expression;
- rewrite stable unit IDs;
- rewrite ISO codes;
- change release identifiers;
- translate user-entered data as though it were UI text.

## Validation

Focused catalog/source validation includes:

```bash
python tools/validate_localization_catalog.py .
python -m unittest tools.tests.test_validate_localization_catalog
```

Avalonia application/headless coverage protects reviewed Hindi/English shared surfaces, including shell headers, Calculator prompts, onboarding, Currency, History, Settings, About/footer, converter privacy notice, protected mode headings, and Graph viewport controls.

The integrated source gate is:

```bash
python tools/release_preflight.py
```

Compiled headless execution remains separate observed evidence from SDK-independent source validation.

## Runtime evidence

Real target validation should still check:

- English/Hindi layout at compact/medium/expanded widths;
- large text;
- Devanagari font/glyph rendering;
- screen-reader pronunciation/context;
- live culture refresh on supported targets;
- persistence across restart;
- no mathematical/persisted-identifier corruption after culture changes.

Use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

for observed runtime evidence.

## Optional post-2.8.03 localization improvements

Further localization work can still be valuable, for example:

- migrating additional technical/detail labels to semantic keys;
- adding more reviewed languages;
- extending localized status/error coverage;
- richer locale-aware numeric input where parser ambiguity can be handled safely;
- RTL language support after dedicated design/testing.

These are optional extensions. They should not be represented as unresolved requirements for the completed 2.8.03 English/Hindi localization baseline.

## Related documentation

- [Localization architecture](LOCALIZATION.md)
- [Accessibility](ACCESSIBILITY.md)
- [Adaptive layout](ADAPTIVE_LAYOUT.md)
- [Settings storage contract](SETTINGS_STORAGE_CONTRACT.md)
- [UI automation](UI_AUTOMATION.md)

## 2.8.03 classification

- reviewed English catalog: **COMPLETE**;
- reviewed Hindi catalog: **COMPLETE**;
- culture preference persistence: **COMPLETE**;
- reviewed live shared-surface refresh: **COMPLETE**;
- source/catalog/headless contract coverage: **COMPLETE**;
- additional languages/detail migration: **OPTIONAL POST-2.8.03**.
