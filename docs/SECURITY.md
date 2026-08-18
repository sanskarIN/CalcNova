# CalcNova Secure Engineering

This document describes implementation-level security expectations. Public vulnerability reporting instructions live in the root `SECURITY.md`.

## Threat model

CalcNova is primarily a local calculator, but it still processes untrusted expression text, clipboard content, imported files, local database records, external links, and potentially future network responses.

Security goals include:

- never executing calculation input as arbitrary code;
- bounding expensive calculations;
- preventing secrets from entering source control;
- treating imports/network responses as untrusted;
- keeping private calculation history local by default;
- minimizing permissions and platform attack surface.

## Expression evaluation

Expression text passes through the project tokenizer/parser/evaluator. Do not replace this path with dynamic C# compilation, JavaScript `eval`, shell execution, reflection-based arbitrary invocation, or another code-execution mechanism.

Supported functions must be explicitly mapped.

## Input limits

The calculation engine has configurable limits such as expression length, factorial input, and integer exponent magnitude.

Additional modules need their own limits where necessary:

- graph sample counts/time budgets;
- dataset row counts;
- matrix dimensions;
- imported file size;
- history query/export limits;
- network response size/timeouts.

## Persistence

Local database input should still be treated defensively because files can be corrupted or modified externally.

Requirements:

- parameterized SQL;
- schema versioning/migrations before breaking changes;
- bounded queries;
- graceful handling of corrupt records;
- no secret material in the history database;
- no remote upload of history by default.

The current SQLite repository uses SQL parameters for user-controlled values.

## Clipboard and imports

Clipboard/file input must be parsed as data. It must never be treated as executable source.

Imported expressions should pass through the same tokenizer/parser limits as manually entered expressions.

## External links

About/support/help links should use known destinations. Avoid constructing arbitrary URLs from calculation input or imported content.

## Network features

Future currency/network features must:

- use HTTPS/TLS;
- define provider interface boundaries;
- enforce timeout/cancellation;
- validate response shape and numeric ranges;
- avoid public-source secrets;
- cache only needed data;
- handle offline/stale states explicitly.

## Secrets

Do not commit:

- API keys/tokens;
- Android keystores;
- Apple certificates/provisioning profiles;
- signing passwords;
- private keys;
- service-account files;
- production credentials.

Repository `.gitignore` excludes common secret/signing file patterns, but ignore rules are not a substitute for reviewing diffs.

## Dependencies

Dependencies are monitored through Dependabot. New dependencies should be reviewed for maintenance, license, security history, package size, and target compatibility.

Major dependency updates should not be auto-merged without tests.

## Logging

Development logging must not contain:

- private clipboard contents;
- full calculation history uploads;
- credentials;
- signing secrets;
- unrelated local file contents.

Production logging should be minimal and local unless an explicitly opt-in remote diagnostic system is later designed and documented.

## UI error handling

Normal users should receive clear errors without raw stack traces or file-system/internal implementation details.

Debug builds may provide richer diagnostics to developers.

## Release security checks

Before a stable release:

1. run formatter/analyzers/tests;
2. review dependency alerts;
3. search tracked files for likely secrets;
4. verify signing credentials are external;
5. review external-link destinations;
6. test malformed/imported expression handling;
7. test workload boundaries;
8. review privacy documentation against actual release dependencies;
9. document unresolved security limitations.

## Reporting

Private security contact:

**supportramsandesh@gmail.com**
