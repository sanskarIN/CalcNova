# CalcNova 2.8.03 Secure Engineering

This document describes implementation-level security expectations for the completed CalcNova 2.8.03 baseline. Public vulnerability-reporting instructions live in the root [`SECURITY.md`](../SECURITY.md).

## Threat model

CalcNova is primarily a local calculator, but it processes data from multiple trust boundaries:

- user-entered mathematical expressions;
- clipboard/imported expression text;
- local history/settings files;
- Browser local storage;
- optional currency-rate network responses;
- external links;
- generated/exported data;
- platform/runtime metadata.

Security goals include:

- never executing calculation input as arbitrary code;
- bounding expensive calculations and large inputs;
- treating clipboard/import/network/storage data as untrusted;
- preventing secrets/signing material from entering source control;
- keeping calculation history local by default;
- minimizing permissions and platform attack surface;
- separating source completeness from signing/store credentials;
- failing safely on malformed/unsupported persisted state.

## Expression evaluation

Expression text passes through the project tokenizer/parser/evaluator.

Do not replace this path with:

- dynamic C# compilation;
- JavaScript `eval`;
- shell/process execution;
- reflection-based arbitrary invocation;
- script-engine execution of user text;
- remote arbitrary-code evaluation.

Supported mathematical functions/operators must be explicitly mapped through project-owned calculation behavior.

Imported/pasted expressions must pass through the same safety limits as manually entered expressions.

## Input and workload limits

Security/resource limits are part of the product architecture rather than optional UI validation.

Protected areas include:

- expression/input length;
- factorial/exponent/workload limits;
- exact-rational raw input, scale/exponent, and reduced bit length;
- engineering-notation input/exponent bounds;
- graph sample and numerical-analysis budgets;
- statistics dataset bounds;
- matrix parsing/dimensions where applicable;
- Unicode inspection bounds;
- history/export preview limits;
- settings/storage decoding limits;
- currency response validation/timeouts at provider boundaries.

A new parser/import/network path must define its own reasonable size/time/work limits before accepting untrusted input.

## Numerical failure safety

Non-finite values, domain errors, overflow, invalid arguments, malformed expressions, and workload-limit failures should be represented through typed/controlled errors rather than uncontrolled process failure.

Numerical-analysis features are approximate and explicitly bounded. Extreme finite values must not be allowed to produce unbounded loops or accidental resource exhaustion.

## Persistence

Local data is still untrusted because application files can be corrupted, partially written, manually modified, restored from backup, or created by another/newer build.

Requirements include:

- parameterized SQL for native SQLite paths;
- versioned settings schema;
- explicit supported migration behavior;
- fail-closed unsupported future-schema handling;
- bounded queries/collections;
- graceful handling of corrupt records;
- no signing/API secret material in user history/settings;
- no remote upload of history by default.

The native SQLite repository uses parameterized values for user-controlled input.

Browser composition uses Browser-safe local storage rather than native SQLite. Shared decoding/validation rules should keep native and Browser settings semantics aligned where applicable.

See [SETTINGS_STORAGE_CONTRACT.md](SETTINGS_STORAGE_CONTRACT.md) and [SETTINGS_MIGRATION.md](SETTINGS_MIGRATION.md).

## Clipboard and imports

Clipboard/imported text must always be treated as data.

Requirements:

- access the clipboard only through explicit user-triggered workflows;
- sanitize/validate imported expressions;
- enforce normal parser/input limits;
- never interpret clipboard text as source code or shell commands;
- avoid logging private clipboard contents;
- handle platform/browser permission denial safely.

## Export

History exports are user-triggered and can contain private expressions/results.

Generated TXT/CSV/JSON content must be treated as user data, not as an executable format.

Any future sharing/cloud-upload integration must remain explicit and must not silently transmit exports.

## External links

About/support/help/donation links should use known destinations through the platform-facing external-link abstraction.

Do not construct arbitrary external URLs from calculation input, imported data, history records, or network responses.

A platform must handle unavailable/blocked link launching without attempting unsafe command execution fallbacks.

## Currency/network features

CalcNova 2.8.03 includes replaceable currency-rate infrastructure with local caching and offline fallback semantics.

Network-enhanced currency behavior must preserve these requirements:

- use HTTPS/TLS for remote providers;
- keep provider access behind a defined interface;
- enforce timeout/cancellation behavior;
- validate response shape and numeric/rate ranges;
- reject non-finite/invalid values;
- avoid embedded provider credentials in public source;
- cache only required rate metadata/data;
- represent offline/stale/failure states explicitly;
- never upload calculation history as part of normal rate refresh;
- avoid allowing provider content to become executable code or arbitrary external links.

A distributor that configures a different provider must review security/privacy implications for the actual endpoint and authentication mechanism.

## Browser/WebAssembly boundary

Browser execution has different platform constraints from native targets.

Security review should consider:

- origin/base-path deployment;
- Browser storage isolation/availability;
- clipboard permissions;
- static-host MIME/configuration correctness;
- service-worker/cache behavior if a deployment adds it;
- Content Security Policy and hosting headers where controlled by the deployer;
- accidental use of native-only dependencies;
- optional network-provider behavior.

Browser publish success alone is not proof that deployment headers/origin policies are secure.

## Android boundary

Android source uses application id `in.sanskar.calcnova` and external signing for production AAB publication.

Requirements:

- request only permissions actually needed;
- keep keystores/passwords outside Git;
- provide signing values through secure local/CI secret storage;
- remove temporary CI keystore material;
- do not log secret signing properties;
- verify the generated package/permission manifest before store publication.

The repository release workflow intentionally skips a signed Android artifact when signing secrets are not configured rather than fabricating a signed result.

## iOS boundary

iOS device/archive distribution requires external Apple signing/provisioning material.

Requirements:

- keep certificates/private keys/provisioning profiles outside Git;
- use supported macOS/Xcode tooling;
- keep simulator compilation distinct from signed-device/App Store evidence;
- avoid leaking signing identities/passwords in logs;
- review entitlements/permissions in the generated application before distribution.

The exact-tag simulator workflow is intentionally unsigned and must not be described as a signed App Store artifact.

## Secrets

Do not commit:

- API keys/tokens;
- Android keystores;
- Android signing passwords;
- Apple certificates/provisioning profiles;
- `.p12` or private-key files;
- notarization/App Store credentials;
- service-account files;
- production provider credentials;
- recovery codes or personal access tokens.

Repository ignore rules and source validators help reduce risk, but they are not substitutes for reviewing diffs and repository history.

If a real credential is accidentally committed, removing the file from the latest commit is not sufficient: revoke/rotate the credential and follow the relevant incident-response/history-cleanup process.

## Dependencies

Dependencies are monitored through repository automation such as Dependabot.

New or upgraded dependencies should be reviewed for:

- active maintenance;
- license compatibility;
- known security advisories;
- package integrity/source;
- API stability;
- package size;
- Desktop/Browser/Android/iOS support;
- permissions/network/data-collection behavior;
- transitive dependencies.

Do not blindly auto-merge major dependency updates without appropriate build/test/platform evidence.

## Logging and diagnostics

Development or production logs must not intentionally contain:

- private clipboard contents;
- entire calculation-history databases/exports;
- API/provider credentials;
- signing secrets;
- private keys/certificates;
- unrelated local file contents;
- raw authentication/store tokens.

Production logging should remain minimal and local unless a separately reviewed opt-in remote diagnostic system is introduced and documented.

## UI error handling

Normal users should receive clear errors without raw stack traces, credential values, sensitive local paths, or internal implementation details that are unnecessary for recovery.

Debug/developer builds may expose richer diagnostics, but those diagnostics still must not print secrets.

## Security and privacy relationship

Security and privacy documentation must describe the same implemented data flows.

See [PRIVACY.md](PRIVACY.md) for local storage, clipboard, export, currency network, analytics/advertising, and cloud-sync policy.

Any change to networking, telemetry, permissions, storage, account/authentication, file sharing, or third-party data collection requires both security and privacy review.

## Release security checks

For a stable release or maintenance update:

1. run `python tools/release_preflight.py` from the intended source checkout;
2. run the applicable .NET restore/format/build/test gate;
3. run target-specific build workflows/commands for claimed platforms;
4. review dependency/security alerts;
5. inspect tracked changes for likely secrets/signing files;
6. verify Android/iOS signing credentials remain external;
7. review package/application identifiers and generated permission/entitlement metadata;
8. review external-link destinations;
9. test malformed/imported expression handling;
10. test relevant workload/input boundaries;
11. review currency provider response/error behavior if networking changed;
12. review privacy documentation against actual release dependencies/configuration;
13. verify release artifacts originate from the expected tag/commit and checksum process;
14. document unresolved security limitations accurately.

See [RELEASE_READINESS_CHECKLIST.md](RELEASE_READINESS_CHECKLIST.md) and [RUNTIME_VALIDATION_RUNBOOK.md](RUNTIME_VALIDATION_RUNBOOK.md).

## Security change rule

A confirmed security defect should normally include:

1. private triage when disclosure risk requires it;
2. impact/scope assessment;
3. regression coverage where safe/practical;
4. root-cause fix;
5. adjacent-boundary review;
6. affected platform/release validation;
7. security/privacy documentation update when behavior changes;
8. appropriate coordinated disclosure/release notes.

Do not publish exploit-sensitive details prematurely when doing so would put users at avoidable risk.

## Reporting

Private security contact:

**supportramsandesh@gmail.com**
