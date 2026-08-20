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
- direct and transitive NuGet vulnerability auditing at a moderate-or-higher enforcement threshold;
- dependency monitoring;
- automated C# code scanning;
- pull-request dependency vulnerability review;
- sanitized imports;
- safe external-link handling;
- optional network features isolated behind interfaces;
- TLS for configured network providers;
- no remote upload of calculation history by default.

Implementation details are documented in `docs/SECURITY.md`.

## Automated security maintenance

The maintained `main` branch contains repository-owned security controls:

- `Directory.Build.props` — explicit `NuGetAudit=true`, `NuGetAuditMode=all`, and `NuGetAuditLevel=moderate`; warnings-as-errors causes reported moderate/high/critical audit warnings to fail restore/build gates;
- `.github/workflows/codeql.yml` — CodeQL C# scanning on pushes and pull requests to `main`, weekly scheduled analysis, and manual runs;
- `.github/workflows/dependency-review.yml` — pull-request dependency review that fails for newly introduced known vulnerabilities at moderate severity or higher;
- `.github/dependabot.yml` — scheduled NuGet and GitHub Actions dependency update proposals;
- `.github/workflows/security-automation-validate.yml` — focused read-only source-contract validation for the security workflows and NuGet audit policy.

The source contracts are protected by:

```bash
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python -m unittest tools.tests.test_validate_security_workflows
python -m unittest tools.tests.test_validate_dependency_security
```

Those checks are also integrated into `python tools/release_preflight.py`.

The focused security workflow watches `Directory.Build.props`, so a change that disables transitive audit, weakens the severity threshold, suppresses protected NU190x warnings through the guarded properties, or removes warnings-as-errors becomes a source-contract failure.

The existence of workflow/MSBuild policy source is not execution evidence. CodeQL/dependency-review results and online NuGet audit results are recorded as PASS only after the corresponding operation actually executes successfully.

See `docs/SECURITY_AUTOMATION.md` for triggers, permissions, action versions, NuGet audit enforcement, and maintenance rules. See `docs/ARTIFACT_PROVENANCE.md` for the stable release provenance contract.

## Completion and security maintenance

The completed status of CalcNova 2.8.03 does not end security maintenance. Confirmed vulnerabilities, compatibility problems, dependency advisories, and security-tooling changes can be fixed through maintenance updates without reclassifying the 2.8.03 product baseline as unfinished.
