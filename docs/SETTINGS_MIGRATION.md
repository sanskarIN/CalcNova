# CalcNova Settings Migration

CalcNova settings are local-first and now carry an explicit schema version so future preference changes can be migrated deliberately instead of relying on incidental serializer behavior.

## Current schema

`AppSettingsSchema.CurrentVersion` is currently `1`.

Every newly created `AppSettings` instance defaults `SchemaVersion` to the current version. Both native JSON storage and Browser storage normalize the schema before applying the rest of their validation rules.

## Migration policy

Current behavior is intentionally conservative:

- schema `0` is treated as the legacy pre-versioned format and is normalized to schema `1` while preserving existing preference values;
- schema `1` is accepted unchanged;
- negative schema versions are rejected as corrupt state;
- schema versions newer than the running application supports are rejected rather than silently downgraded.

Rejecting future schemas prevents an older CalcNova build from overwriting newer preference data whose meaning it does not understand.

## Storage paths

The same schema normalization contract is applied by:

- `JsonSettingsRepository` for native file-backed preferences;
- `BrowserSettingsRepository` for Browser/WebAssembly local storage.

The Browser storage key remains `calcnova.settings.v1`; this key names the storage container and is separate from the serialized settings schema version. A future storage-key change should be made only when an in-place schema migration is insufficient.

## Validation

Automated coverage includes:

- default settings use the current schema;
- legacy schema `0` migrates to the current version;
- migration preserves representative culture/history values;
- current-schema normalization is stable;
- negative schema values are rejected;
- future schema values are rejected;
- native JSON loading exercises both legacy migration and future-schema rejection;
- source validation confirms both native and Browser repositories normalize before field validation.

`tools/validate_settings_schema.py`, its Python regression tests, and `.github/workflows/settings-schema-validate.yml` guard these contracts without requiring the .NET SDK. The integrated release preflight also includes the schema validator and its regression suite.

## Future schema changes

When adding schema version `N + 1`:

1. add an explicit migration from every supported older version;
2. keep migrations deterministic and side-effect free;
3. preserve unknown-independent user choices whenever possible;
4. add unit tests using representative old serialized data;
5. update Browser and native persistence tests together;
6. document removed or changed preferences;
7. never mark migration validation complete until real builds/tests have run in a suitable .NET environment.

Settings migration must not silently alter mathematical meaning, history privacy choices, localization preferences, accessibility preferences, or converter favorites.
