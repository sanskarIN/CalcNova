# CalcNova Converter Mode

CalcNova's fixed-unit converter is a completed offline feature in the 2.8.03 baseline. Unit definitions live in the project, and fixed physical/data conversion does not require an account, network request, external service, or embedded credential.

## Fixed conversion model

`UnitCatalog` defines units by stable ID, name, symbol, category, base factor, and optional base offset. `UnitConverter` converts through the category's base representation and rejects incompatible categories.

Current fixed categories include:

- length;
- area;
- volume;
- mass;
- speed;
- temperature;
- time;
- data/storage;
- frequency;
- pressure;
- energy;
- power;
- force;
- angle.

## Conversion pairs

`ConversionPair` represents a validated From/To unit pair. Construction resolves both unit IDs through `UnitCatalog` and rejects cross-category pairs.

Pairs expose canonical From/To unit IDs, category, a compact display name, and a safe `Swap()` operation.

## Per-category defaults

Each category has a deterministic useful default pair instead of relying on the first two units in catalog order.

Examples include:

- length: metre → kilometre;
- temperature: Celsius → Fahrenheit;
- speed: kilometre/hour → mile/hour;
- data/storage: gigabyte → gibibyte;
- angle: degree → radian.

The complete mapping and validation rules are documented in [CONVERTER_DEFAULTS_AND_PRIVACY.md](CONVERTER_DEFAULTS_AND_PRIVACY.md).

Restoring an explicit recent/favorite pair takes precedence over the category default.

## Search and unit assignment

The shared converter UI supports category-scoped unit search.

Search results can be assigned to either side of the active pair:

- From;
- To.

Selection remains subject to category compatibility and normal conversion-pair validation.

Search is local over the project-owned unit catalog and does not require a network service.

## Recent and favorite pairs

`ConversionPairHistory` maintains:

- a bounded most-recent-first list;
- a de-duplicated favorite set.

Recording an already-most-recent pair is treated as no state change, avoiding unnecessary persistence writes.

The current shared behavior includes:

- up to 12 recent pairs;
- up to 100 persisted favorite pairs;
- recent-pair selection/restoration;
- favorite-pair selection/restoration;
- favorite toggle for the active pair;
- explicit clear-recents behavior;
- safe pair swapping.

Only deliberate conversion actions should affect the persisted recent-pair list. Editing category/From/To selections can refresh the preview without creating noisy intermediate history entries.

## Persistence

Converter preferences use the same application-facing `ISettingsRepository` abstraction as the rest of CalcNova.

Persisted converter state includes:

- significant-digit precision;
- bounded recent conversion-pair tokens;
- bounded favorite conversion-pair tokens.

`ConversionPairToken` uses a versioned `v1:from>to` format based on stable unit IDs. Restore logic validates tokens against `UnitCatalog`; malformed, unknown, or cross-category pairs are ignored by the converter restore layer. Native and Browser settings implementations also enforce token count/length bounds.

`MainViewModel` restores converter state when settings load and persists deliberate converter-state changes. Settings reset synchronizes the in-memory converter state as well.

Native targets and Browser/WebAssembly use target-appropriate local settings storage. Converter preferences do not require an online account or cloud profile.

## Result precision

`ConverterViewModel.SignificantDigits` supports 1 through 17 significant digits.

Shared UI presets include:

- 6;
- 9;
- 12;
- 15;
- 17.

Changing precision re-runs the current conversion and persists the selected preference.

Precision controls display formatting; it does not turn the fixed-unit `double` conversion model into arbitrary-precision physical arithmetic.

## Result copy

The shared converter workflow includes an explicit copy-result action.

Clipboard access remains user-triggered and goes through the shared platform abstraction. A platform/browser clipboard failure should be handled as an interaction failure rather than changing the mathematical result.

## Fixed conversion versus currency

Fixed unit conversion and currency conversion intentionally remain separate:

### Fixed physical/data units

- deterministic;
- project-owned unit definitions;
- local/offline calculation;
- no provider credential;
- no exchange-rate timestamp.

### Currency

- replaceable rate-provider/cache architecture;
- optional network-enhanced refresh;
- local cache and offline fallback semantics;
- provider credentials, when a provider requires them, must remain outside public source;
- rate freshness/source semantics should remain distinguishable from fixed-unit conversion.

See [PRIVACY.md](PRIVACY.md) for network/data-handling expectations.

## Accessibility and adaptive behavior

The converter participates in the shared CalcNova accessibility/adaptive baseline, including:

- minimum interaction-target contracts;
- visible keyboard focus on keyboard-capable targets;
- compact/medium/expanded shared layouts;
- reachable search/saved-pair controls;
- user-triggered clipboard behavior;
- localized reviewed shared surfaces where applicable.

Platform-specific screen-reader, text-scaling, orientation, and storage behavior remains runtime evidence and must only be marked PASS after it is actually observed.

See [ACCESSIBILITY.md](ACCESSIBILITY.md), [ADAPTIVE_LAYOUT.md](ADAPTIVE_LAYOUT.md), and [RUNTIME_VALIDATION_RUNBOOK.md](RUNTIME_VALIDATION_RUNBOOK.md).

## Validation

Converter behavior is protected by domain/application/settings regression coverage and SDK-independent source contracts.

Focused validation includes:

```bash
python tools/validate_converter_defaults.py .
python -m unittest tools.tests.test_validate_converter_defaults
python tools/validate_converter_preference_notice.py .
python -m unittest tools.tests.test_validate_converter_preference_notice
```

The integrated source gate is:

```bash
python tools/release_preflight.py
```

Compiled tests run through the normal .NET test gate described in [TESTING.md](TESTING.md).

Source/test presence is not runtime platform evidence. Desktop, Browser, Android, and iOS persistence/accessibility behavior should be recorded with `PASS / FAIL / BLOCKED / NOT RUN` only after the corresponding operation is observed.

## 2.8.03 classification

For CalcNova 2.8.03:

- fixed-unit categories: **COMPLETE**;
- unit search and From/To assignment: **COMPLETE**;
- swap: **COMPLETE**;
- 1–17 significant-digit precision: **COMPLETE**;
- recent pairs: **COMPLETE**;
- favorites: **COMPLETE**;
- saved-pair restoration: **COMPLETE**;
- clear recents: **COMPLETE**;
- result copy: **COMPLETE**;
- per-category defaults: **COMPLETE**;
- local preference persistence: **COMPLETE**.

Future converter changes are maintenance or optional enhancements rather than missing 2.8.03 requirements.
