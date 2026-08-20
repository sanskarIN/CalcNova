# CalcNova Security Automation

This guide documents the repository-owned security automation used to maintain the completed CalcNova 2.8.03 baseline.

Security automation is a maintenance control. It does not change the public product version and it does not replace manual security review, runtime validation, platform signing review, or responsible vulnerability reporting.

## Security automation inventory

CalcNova maintains three complementary dependency/code-security layers:

1. **Dependabot** — scheduled dependency update proposals for NuGet packages and GitHub Actions.
2. **Dependency Review** — pull-request gate for newly introduced vulnerable dependency versions.
3. **CodeQL** — GitHub code-scanning analysis for C# source.

Repository-owned source validation protects the workflow contracts so they cannot silently lose their intended triggers, action major versions, language selection, severity threshold, or least-privilege permissions.

## CodeQL workflow

Workflow:

```text
.github/workflows/codeql.yml
```

Triggers:

- pushes to `main`;
- pull requests targeting `main`;
- a weekly scheduled scan;
- manual `workflow_dispatch`.

The workflow uses:

```text
github/codeql-action/init@v4
github/codeql-action/analyze@v4
```

Language:

```text
csharp
```

Build mode:

```text
none
```

C# is analyzed directly from source/dependency metadata in this workflow. This avoids coupling the security scan to Desktop/Browser/Android/iOS workload availability while still allowing the normal platform build workflows to validate target compilation separately.

Permissions are intentionally limited to:

```yaml
contents: read
security-events: write
```

`security-events: write` is required to publish code-scanning results. The workflow must not gain repository-content write, package write, Actions write, or OIDC token permissions without an explicit reviewed requirement.

## Dependency Review workflow

Workflow:

```text
.github/workflows/dependency-review.yml
```

Trigger:

- pull requests targeting `main`.

The workflow uses:

```text
actions/dependency-review-action@v5
```

The current enforcement threshold is:

```yaml
fail-on-severity: moderate
```

A pull request that introduces a dependency with a known vulnerability at moderate, high, or critical severity is therefore expected to fail this gate when GitHub's dependency-review service identifies it.

The workflow uses read-only repository contents permission:

```yaml
permissions:
  contents: read
```

It intentionally does not use `pull_request_target`. Dependency review is a PR inspection gate and does not require write access to repository contents or secrets.

## Dependabot

Configuration:

```text
.github/dependabot.yml
```

CalcNova checks:

- NuGet dependencies weekly;
- GitHub Actions dependencies weekly.

Avalonia packages are grouped for minor/patch updates, and test-tooling packages are grouped separately. Major upgrades remain reviewable changes rather than being silently merged.

Dependabot update proposals do not prove compatibility. Apply the relevant source, .NET, platform, and runtime checks before accepting dependency updates.

## Security workflow source validator

Validator:

```bash
python tools/validate_security_workflows.py .
```

Regression suite:

```bash
python -m unittest tools.tests.test_validate_security_workflows
```

The validator protects:

- CodeQL push/PR/schedule/manual triggers;
- CodeQL Action major `v4`;
- C# language selection;
- source-analysis build mode;
- CodeQL result publication permission;
- Dependency Review Action major `v5`;
- `moderate` vulnerability enforcement threshold;
- read-only dependency-review permissions;
- rejection of `pull_request_target` for these workflows;
- rejection of unnecessary write/OIDC permissions.

## Focused validation workflow

Workflow:

```text
.github/workflows/security-automation-validate.yml
```

It runs when the security workflows, their validator/tests, or their integrated-preflight wiring changes.

The focused job executes:

```bash
python tools/validate_security_workflows.py .
python -m unittest tools.tests.test_validate_security_workflows
```

It has read-only contents permission.

## Integrated release preflight

The security workflow validator and its regression suite are part of:

```bash
python tools/release_preflight.py
```

That means a release-source preflight fails when the required CodeQL/dependency-review source contracts disappear or drift from the protected configuration.

The source preflight validates workflow source. It does **not** claim that a GitHub-hosted CodeQL or dependency-review run has passed unless that workflow actually executes successfully.

## Evidence policy

Use the standard CalcNova evidence vocabulary:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Examples:

- CodeQL workflow source satisfies `validate_security_workflows.py`: source-contract PASS when actually executed and observed.
- CodeQL Actions run was never started: runtime/service evidence `NOT RUN`.
- Dependency Review cannot access required GitHub dependency data because repository/service configuration is unavailable: `BLOCKED`.
- Dependency Review detects a prohibited vulnerable dependency: `FAIL` until the dependency change is remediated or an explicitly reviewed exception policy is adopted.

Never infer a service-level PASS solely from workflow YAML existing in the repository.

## Maintenance rules

When changing security automation:

1. use currently supported action major versions;
2. keep permissions at the minimum required level;
3. avoid `pull_request_target` unless a separately reviewed design genuinely requires it;
4. do not expose repository or signing secrets to untrusted pull-request code;
5. update `tools/validate_security_workflows.py` and its regression tests with intentional contract changes;
6. keep `tools/release_preflight.py` integration intact;
7. update this guide and the security policy when behavior or enforcement changes;
8. record actual workflow results separately from source completeness.

## Related documentation

- [`../SECURITY.md`](../SECURITY.md) — public security policy and reporting.
- [`SECURITY.md`](SECURITY.md) — implementation-level secure-engineering rules.
- [`SOURCE_PREFLIGHT.md`](SOURCE_PREFLIGHT.md) — integrated SDK-independent source gate.
- [`TESTING.md`](TESTING.md) — compiled/source test layers.
- [`RELEASE.md`](RELEASE.md) — release security and publication process.
- [`VALIDATION_EVIDENCE.md`](VALIDATION_EVIDENCE.md) — evidence-state semantics.
