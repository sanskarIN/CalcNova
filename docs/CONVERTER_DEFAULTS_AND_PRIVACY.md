# CalcNova Converter Defaults and Preference Privacy

CalcNova's fixed-unit converter remains an offline calculation feature. This document describes the deterministic default-pair behavior and the local preference state used by the shared converter UI.

## Per-category defaults

Each converter category opens with a useful non-identity pair instead of simply selecting the first two catalog units.

| Category | Default pair |
|---|---|
| Length | metre → kilometre |
| Area | square metre → square foot |
| Volume | litre → US gallon |
| Mass | kilogram → pound |
| Speed | kilometre/hour → mile/hour |
| Temperature | Celsius → Fahrenheit |
| Time | hour → minute |
| Data/storage | gigabyte → gibibyte |
| Frequency | hertz → kilohertz |
| Pressure | kilopascal → psi |
| Energy | joule → kilojoule |
| Power | watt → kilowatt |
| Force | newton → pound-force |
| Angle | degree → radian |

`ConversionDefaults.ForCategory` owns this mapping. The source validator requires exactly one pair for every current `UnitCategory`, validates both unit IDs against `UnitCatalog`, and rejects accidental identity defaults.

Changing category applies that category's default pair. Restoring an explicit recent/favorite pair still wins over the category default, so persisted user choices remain authoritative when deliberately restored.

## What CalcNova persists

Converter preference persistence is deliberately small and local:

- selected significant-digit precision;
- bounded recent conversion-pair tokens;
- bounded favorite conversion-pair tokens.

Those values use the shared app-settings repository. Native heads use the native settings path; Browser uses its browser-compatible local storage path. The converter does not require an account or cloud profile for this state.

The shared UI now displays a visible notice explaining that saved converter preferences stay in local app settings and that fixed unit conversion itself remains offline.

## What is not persisted as converter preference state

The converter preference contract does not require storing:

- calculation input history beyond the bounded pair list;
- location;
- contacts;
- advertising identifiers;
- analytics identifiers;
- account credentials;
- remote profile data.

Currency conversion is a separate optional network-enhanced module and should not be confused with fixed physical/data unit conversion.

## Validation

Relevant checks include:

```bash
python tools/validate_converter_defaults.py .
python -m unittest tools.tests.test_validate_converter_defaults
python tools/validate_converter_preference_notice.py .
python -m unittest tools.tests.test_validate_converter_preference_notice
```

The App and Converter test projects also contain domain/view-model/headless tests for default-pair behavior and the local preference notice.

Source validation does not replace real persistence checks on Desktop, Browser, Android, or iOS. Target storage behavior still requires observed runtime evidence before release readiness is claimed.
