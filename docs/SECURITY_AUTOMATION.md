# CalcNova Security Automation

This guide documents the repository-owned security automation used to maintain the completed CalcNova 2.8.03 baseline.

Security automation is a maintenance control. It does not change the public product version and it does not replace manual security review, runtime validation, platform signing review, or responsible vulnerability reporting.

## Security automation inventory

CalcNova maintains four complementary dependency/code-security layers:

1. **NuGet Audit** — repository-level restore policy that audits direct and transitive packages and turns moderate-or-higher advisories into restore failures through warnings-as-errors.
2. **Dependabot** — scheduled dependency update proposals for NuGet packages and GitHub Actions.
3. **Dependency Review** — pull-request gate for newly introduced vulnerable dependency versions.
4. **CodeQL** — GitHub code-scanning analysis for C# source.

Repository-owned source validation protects both the GitHub workflow contracts and the NuGet audit policy so they cannot silently lose their intended triggers, action major versions, language selection, severity threshold, transitive coverage, or least-privilege permissions.

## NuGet Audit policy

Repository policy source:

```text
Directory.Build.props
```

CalcNova explicitly sets:

```xml
<NuGetAudit>true</NuGetAudit>
<NuGetAuditMode>all</NuGetAuditMode>
<NuGetAuditLevel>moderate</NuGetAuditLevel>
```

The repository also keeps:

```xml
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

For .NET 10 projects, NuGet audit can inspect transitive packages during restore. CalcNova makes that behavior explicit rather than relying only on SDK defaults. The `moderate` threshold means low-severity advisories are below the enforced reporting threshold, while moderate, high, and critical advisories are expected to surface as audit warnings; with warnings-as-errors enabled, those warnings fail the restore/build gate.

This closes a gap that pull-request dependency review cannot cover by itself: a package may be considered clean when merged and receive a vulnerability advisory later. A later restore can then surface the newly known advisory even if no dependency file changed in that commit.

Repository policy validator:

```bash
python tools/validate_dependency_security.py .
```

Regression suite:

```bash
python -m unittest tools.tests.test_validate_dependency_security
```

The validator requires:

- `TreatWarningsAsErrors=true`;
- `NuGetAudit=true`;
- `NuGetAuditMode=all`;
- `NuGetAuditLevel=moderate`;
- exactly one definition of each protected property;
- no direct-mode/disabled audit drift;
- no repository-level suppression of NU1901/NU1902/NU1903/NU1904 through the protected markers.

A specific advisory should not be broadly suppressed merely to make CI green. Remediate the dependency where practical; if an exceptional advisory suppression is ever necessary, document and review the reason explicitly rather than weakening the repository-wide policy.

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
- focused security-validation workflow existence;
- focused workflow read-only permissions;
- `Directory.Build.props` push/PR watch coverage;
- execution of both security-workflow and NuGet-policy validators/tests;
- rejection of `pull_request_target` for these workflows;
- rejection of unnecessary write/OIDC/package permissions.

## Focused validation workflow

Workflow:

```text
.github/workflows/security-automation-validate.yml
```

It runs when the security workflows, `Directory.Build.props`, their validators/tests, or their integrated-preflight wiring changes.

The focused job executes:

```bash
python tools/validate_security_workflows.py .
python tools/validate_dependency_security.py .
python -m unittest tools.tests.test_validate_security_workflows
python -m unittest tools.tests.test_validate_dependency_security
```

It has read-only contents permission.

The security-workflow validator also checks this focused workflow, so removing the NuGet policy watch path or one of the focused validation commands becomes a source-contract failure.

## Integrated release preflight

Both security validators and their regression suites are part of:

```bash
python tools/release_preflight.py
```

That means a release-source preflight fails when required CodeQL/dependency-review source contracts disappear, the focused security workflow drifts, or the repository-level NuGet audit policy is weakened.

The release workflow also runs `dotnet restore CalcNova.slnx`. Because the NuGet audit policy lives in `Directory.Build.props`, a .NET 10 restore is an execution layer for the direct/transitive vulnerability audit in addition to the SDK-independent policy-source validation.

Source preflight validates policy/workflow source. It does **not** claim that a GitHub-hosted CodeQL/dependency-review run or an online NuGet vulnerability query has passed unless that operation actually executes successfully.

## Evidence policy

Use the standard CalcNova evidence vocabulary:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Examples:

- `validate_dependency_security.py` executes successfully against the repository: NuGet-policy source-contract `PASS`.
- `dotnet restore` actually completes with the configured audit sources and no moderate-or-higher audit error: NuGet audit execution evidence `PASS` for that restore.
- NuGet advisory sources are unreachable during restore: record the observed result accurately rather than claiming an audit PASS.
- CodeQL workflow source satisfies `validate_security_workflows.py`: source-contract PASS when actually executed and observed.
- CodeQL Actions run was never started: runtime/service evidence `NOT RUN`.
- Dependency Review cannot access required GitHub dependency data because repository/service configuration is unavailable: `BLOCKED`.
- Dependency Review or NuGet Audit detects a prohibited vulnerable dependency: `FAIL` until the dependency is remediated or an explicitly reviewed exception is adopted.

Never infer a service/network-level PASS solely from workflow or MSBuild policy source existing in the repository.

## Maintenance rules

When changing security automation or dependency policy:

1. use currently supported action major versions;
2. keep permissions at the minimum required level;
3. keep NuGet direct + transitive audit enabled at the approved severity threshold;
4. avoid broad vulnerability-warning suppression to bypass restore failures;
5. avoid `pull_request_target` unless a separately reviewed design genuinely requires it;
6. do not expose repository or signing secrets to untrusted pull-request code;
7. update the relevant validator and regression tests with intentional contract changes;
8. keep `tools/release_preflight.py` integration intact;
9. update this guide and the security policy when behavior or enforcement changes;
10. record actual workflow/restore results separately from source completeness.

## Related documentation

- [`../SECURITY.md`](../SECURITY.md) — public security policy and reporting.
- [`SECURITY.md`](SECURITY.md) — implementation-level secure-engineering rules.
- [`SOURCE_PREFLIGHT.md`](SOURCE_PREFLIGHT.md) — integrated SDK-independent source gate.
- [`BUILDING.md`](BUILDING.md) — restore/build commands that execute NuGet Audit.
- [`TESTING.md`](TESTING.md) — compiled/source test layers.
- [`RELEASE.md`](RELEASE.md) — release security and publication process.
- [`ARTIFACT_PROVENANCE.md`](ARTIFACT_PROVENANCE.md) — release artifact provenance.
- [`VALIDATION_EVIDENCE.md`](VALIDATION_EVIDENCE.md) — evidence-state semantics.
