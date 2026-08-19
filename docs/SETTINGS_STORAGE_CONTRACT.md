# CalcNova Settings Storage Contract

CalcNova native and Browser settings persistence share the same schema-aware decoder and preference validator. Storage implementations are responsible for transport only; they must not duplicate business validation rules.

## Shared components

### `AppSettingsJson`

The shared JSON decoder:

- deserializes a JSON object into `AppSettings`;
- detects the `schemaVersion` property case-insensitively;
- treats historical JSON that has no schema-version property as schema `0`;
- leaves an explicitly serialized schema version intact so `AppSettingsSchema` can decide whether it is supported.

This missing-property detection is important because `AppSettings` defaults newly created in-memory settings to the current schema. Without inspecting the JSON document itself, an old pre-versioning settings file could otherwise look like a current-schema document merely because the C# property initializer supplied the current value.

### `AppSettingsValidator`

The shared validator:

1. normalizes the settings schema through `AppSettingsSchema`;
2. validates theme and angle-unit enum values;
3. validates the stored culture name;
4. bounds decimal precision and history limits;
5. rejects negative onboarding versions;
6. bounds converter significant-digit precision;
7. bounds recent/favorite converter-token counts and token lengths.

Both native and Browser storage must call this shared validator for load/save behavior.

## Native JSON storage

`JsonSettingsRepository`:

- parses the local JSON document;
- delegates schema-aware decoding to `AppSettingsJson`;
- delegates normalization/validation to `AppSettingsValidator`;
- writes through a temporary file and replaces the target file only after serialization/flush completes.

The repository must not reintroduce a private copy of the shared preference-validation logic.

## Browser storage

`BrowserSettingsRepository`:

- reads the local Browser storage value;
- parses the JSON document;
- delegates schema-aware decoding to `AppSettingsJson`;
- delegates normalization/validation to `AppSettingsValidator`;
- returns defaults for malformed JSON syntax;
- keeps unsupported future schema versions fail-closed rather than silently downgrading them.

The Browser storage key name and the serialized settings schema are different concepts. A storage key containing `v1` does not remove the need for an explicit schema inside the serialized settings document.

## Migration compatibility

The current contract recognizes:

- historical JSON with **no** `schemaVersion` property -> legacy schema `0` -> migrate to the current schema;
- explicit schema `0` -> migrate to the current schema;
- current schema -> validate normally;
- negative schema -> reject as corrupt;
- future unsupported schema -> reject rather than allowing an older build to overwrite newer preferences.

## Automated coverage

Platform/persistence tests cover:

- unversioned JSON detection;
- case-insensitive schema-property detection;
- legacy migration with representative preference preservation;
- explicit schema-zero migration;
- future-schema rejection;
- invalid culture/precision/history/token boundaries;
- current-schema round trips through native JSON storage.

`tools/validate_settings_schema.py` additionally protects the architectural rule that native and Browser repositories consume `AppSettingsJson` and `AppSettingsValidator` instead of duplicating validation.

## Runtime evidence boundary

The shared source contract and tests are implemented, but real native filesystem and Browser local-storage behavior must still be observed in suitable target executions. Source presence or validator success alone does not turn those runtime checks into PASS evidence.
