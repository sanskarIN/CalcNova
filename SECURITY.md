# Security Policy

CalcNova welcomes responsible reports that help keep users and contributors safe.

## Supported versions

**CalcNova 2.8.03 is the current completed and supported product baseline.**

Security fixes are applied to the actively maintained `main` branch and, when needed, to supported release or maintenance branches/tags.

| Version | Supported |
| --- | --- |
| `2.8.03` | Yes |
| Earlier pre-2.8.03 snapshots | No stable-support guarantee |

The normalized SemVer/package equivalent of 2.8.03 is `2.8.3`, and the normalized release tag is `v2.8.3`.

## Reporting a vulnerability

Please **do not** publish sensitive vulnerability details in a public GitHub issue, pull request, discussion, screenshot, or social post.

Send a private report to:

**supportramsandesh@gmail.com**

A useful report includes, where safe and applicable:

- affected CalcNova version or commit;
- affected platform;
- clear reproduction steps;
- security impact;
- required preconditions;
- minimal proof-of-concept information needed to reproduce the issue;
- suggested mitigation if you have one.

Do not send real credentials, unrelated private user data, private keys, or destructive payloads.

## What happens after a report

Maintainers will review the report, attempt to reproduce the issue, assess severity and affected versions, and prepare an appropriate fix or mitigation when confirmed.

Response and remediation time varies with complexity, platform requirements, and maintainer availability. This policy intentionally does not promise an impossible fixed resolution deadline.

## Coordinated disclosure

Please allow maintainers a reasonable opportunity to investigate and prepare a fix before publishing exploit details for a confirmed vulnerability.

After remediation is available, maintainers may publish an advisory, maintenance release note, or security changelog entry describing impact and affected versions without exposing unnecessary user data.

## Security design principles

CalcNova's baseline security requirements include:

- no arbitrary expression code execution;
- no hardcoded secrets;
- no signing credentials in Git;
- bounded input/workload handling;
- safe local persistence;
- dependency monitoring;
- sanitized imports;
- safe external-link handling;
- optional network features isolated behind interfaces;
- TLS for configured network providers;
- no remote upload of calculation history by default.

Implementation details are documented in `docs/SECURITY.md`.

## Completion and security maintenance

The completed status of CalcNova 2.8.03 does not end security maintenance. Confirmed vulnerabilities, compatibility problems, and dependency/security issues can be fixed through maintenance updates without reclassifying the 2.8.03 product baseline as unfinished.
