# Converter Mode

CalcNova's fixed-unit converter is designed to work fully offline. Unit definitions live in the project and conversions do not require an account, network request, external service, or embedded credential.

## Fixed conversion model

`UnitCatalog` defines units by stable ID, name, symbol, category, base factor, and optional base offset. `UnitConverter` converts through the category's base representation and rejects incompatible categories.

Current fixed categories include length, area, volume, mass, speed, temperature, time, data/storage, frequency, pressure, energy, power, force, and angle.

## Conversion pairs

`ConversionPair` represents a validated from/to unit pair. Construction resolves both unit IDs through `UnitCatalog` and rejects cross-category pairs.

Pairs expose canonical from/to unit IDs, category, a compact display name, and a safe `Swap()` operation.

## Recent and favorite pairs

`ConversionPairHistory` keeps a bounded most-recent-first list and a de-duplicated favorite set. Recording an already-most-recent pair is treated as no state change, preventing unnecessary persistence writes.

The default recent capacity is 12. Favorites are capped to 100 when persisted by the shared app.

The shared converter UI exposes recent-pair and favorite-pair selectors plus a favorite toggle for the current pair.

## Persistence

Converter preferences are stored through the same `ISettingsRepository` abstraction used by the rest of CalcNova.

Persisted converter state includes:

- significant-digit precision;
- up to 12 recent conversion-pair tokens;
- up to 100 favorite conversion-pair tokens.

`ConversionPairToken` uses a versioned `v1:from>to` format based on stable unit IDs. Restore logic validates tokens against `UnitCatalog`; malformed, unknown, or cross-category pairs are ignored by the converter restore layer. Native JSON and Browser settings repositories also enforce token count/length bounds.

`MainViewModel` restores converter state when settings are loaded and autosaves deliberate converter-state changes. Settings reset also synchronizes the in-memory converter state immediately.

## Result precision

`ConverterViewModel.SignificantDigits` supports 1 through 17 significant digits. Shared UI presets are 6, 9, 12, 15, and 17.

Changing precision re-runs the current conversion and persists the selected value. The conversion calculation remains based on the underlying `double` unit model; precision controls display formatting, not arbitrary-precision physical arithmetic.

## Recent-history behavior

Only deliberate conversion actions should affect the persisted recent-pair list. Editing category/from/to properties can refresh the preview result without creating noisy intermediate recent entries. Convert, swap, and restoring a saved pair record the final selected pair.

## Currency is separate

Fixed unit conversion and currency conversion intentionally remain separate concepts:

- physical/data unit conversion is fully offline and deterministic;
- currency uses an optional replaceable rate provider/cache architecture;
- no provider credential belongs in source control;
- cached/last-known-rate behavior must be clearly labeled with its timestamp/source semantics.

## Remaining product work

- searchable category/unit selection;
- accessible copy-result action;
- optional per-category default pairs;
- explicit clear-recents management;
- compact responsive layout refinement for narrow screens;
- platform validation of persistence and accessibility behavior.

## Validation

Converter pair/history/token/restore/precision tests, app persistence-integration tests, and settings repository tests are present. They are **NOT RUN locally in the current continuation environment** because the required .NET SDK is unavailable there.
