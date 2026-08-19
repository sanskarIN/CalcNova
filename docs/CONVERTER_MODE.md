# Converter Mode

CalcNova's fixed-unit converter is designed to work fully offline. Unit definitions live in the project and conversions do not require an account, network request, external service, or embedded credential.

## Fixed conversion model

`UnitCatalog` defines units by stable ID, name, symbol, category, base factor, and optional base offset. `UnitConverter` converts through the category's base representation and rejects incompatible categories.

Current fixed categories include length, area, volume, mass, speed, temperature, time, data/storage, frequency, pressure, energy, power, force, and angle.

## Conversion pairs

`ConversionPair` represents a validated from/to unit pair. Construction resolves both unit IDs through `UnitCatalog` and rejects cross-category pairs.

Pairs expose:

- canonical from/to unit IDs;
- category;
- compact display name;
- a safe `Swap()` operation.

## Recent pairs

`ConversionPairHistory` keeps a bounded most-recent-first list. Recording a pair already in the list moves it to the front rather than creating a duplicate.

The default recent-pair capacity is 12. The capacity is configurable for tests or future product decisions.

## Favorite pairs

The same state object tracks favorite pairs independently of recency. Favorites are de-duplicated and exposed in deterministic category/display-name order.

The current app view model exposes recent/favorite pair collections and commands to apply a pair or toggle the current pair's favorite state.

Current limitation: this pair state is in memory. Persistence semantics across launches still need a deliberate settings/storage design before being treated as complete product behavior.

## Result precision

`ConverterViewModel.SignificantDigits` supports 1 through 17 significant digits. Suggested UI presets are 6, 9, 12, 15, and 17.

Changing precision re-runs the current conversion so the displayed result immediately reflects the selected precision. The conversion calculation remains based on the underlying `double` unit model; precision controls display formatting, not arbitrary-precision physical arithmetic.

## Currency is separate

Fixed unit conversion and currency conversion intentionally remain separate concepts:

- physical/data unit conversion is fully offline and deterministic;
- currency uses an optional replaceable rate provider/cache architecture;
- no provider credential belongs in source control;
- cached/last-known-rate behavior must be clearly labeled with its timestamp/source semantics.

## Remaining shared-UI work

- visible precision selector;
- recent-pair picker;
- favorite-pair picker and favorite toggle;
- searchable category/unit selection;
- accessible copy-result action;
- persistence decision for recent/favorite pair state;
- compact responsive layout for narrow screens.

## Validation

Converter pair/history/precision tests are present in the converter and app test projects. They are **NOT RUN locally in the current continuation environment** because the required .NET SDK is unavailable there.
