# CalcNova Main-Branch Protection

CalcNova's source and CI gates are designed to protect the completed 2.8.03 baseline, but those checks only become merge enforcement when GitHub branch protection or a repository ruleset requires them.

## Observed repository state — 2026-08-23

The GitHub branch metadata for `main` currently reports:

```text
protected: false
required status checks: enforcement off
```

That means `main` is currently **not protected at the GitHub repository-setting layer**.

This is an external repository configuration state, not a missing CalcNova source feature. It cannot be enabled by a Markdown file, workflow, or source validator alone.

## Source readiness completed

Before documenting this external gap, CalcNova was hardened so the main policy check is safe to require:

- `.github/workflows/source-preflight.yml` runs on every pull request targeting `main`;
- it also runs on every push to `main` and supports manual dispatch;
- it has no `paths` or `paths-ignore` filters;
- it uses read-only repository contents permission;
- it cancels superseded runs;
- `tools/validate_source_preflight_workflow.py` rejects path-filtered or privileged drift;
- the validator and regression suite are part of the integrated preflight itself;
- `tools/validate_ci_hygiene.py` now protects canonical .NET 10 CI workflows from starter-template and obsolete-action drift;
- obsolete generic .NET 8 and WPF/MSIX starter workflows have been removed.

The absence of path filters on Source Preflight is important. GitHub required checks should be checks that reliably appear on every pull request; a required workflow that is skipped by path filters can leave a pull request waiting for a check that never starts.

## Recommended GitHub ruleset

Create a branch ruleset targeting the repository's default branch / `main`.

Recommended baseline:

1. **Require a pull request before merging.**
2. **Require status checks to pass before merging.**
3. **Require conversation resolution before merging.**
4. **Block force pushes.**
5. **Block branch deletion.**
6. **Require the branch to be up to date before merging** when the resulting CI rerun cost is acceptable.
7. Keep bypass permissions as narrow as practical; do not use a broad permanent bypass merely for convenience.

### Pull-request approval count

For a repository with multiple active maintainers, require at least one approving review.

For a genuinely single-maintainer repository, requiring a pull request is still valuable even if the approval count must remain zero; the PR provides reviewable diffs, conversations, and required CI enforcement without making self-maintenance impossible.

## Recommended required checks

Select the actual check names shown by GitHub after recent successful pull-request runs. The source workflows/jobs intended to be required are:

### Source policy gate

Workflow:

```text
Source Preflight
```

Job:

```text
source-preflight
```

This is the SDK-independent policy/integrity gate and is intentionally always present on pull requests. It now includes the CI-hygiene validator and its regression suite through `tools/release_preflight.py`.

### Cross-platform core build/test gate

Workflow:

```text
Build and Test
```

Matrix jobs:

```text
ubuntu-latest
windows-latest
macos-latest
```

Require all three matrix checks if the GitHub ruleset UI exposes them individually.

The restore phase in these jobs also executes the repository-level NuGet vulnerability-audit policy.

### Dependency change gate

Workflow:

```text
Dependency Review
```

Job:

```text
dependency-review
```

This protects pull requests from newly introduced known dependency vulnerabilities at the configured moderate-or-higher threshold.

### Code scanning gate

Workflow:

```text
CodeQL
```

Job name:

```text
Analyze C#
```

Require this check when GitHub exposes it as an eligible branch/ruleset status check for the repository.

## Checks that should not be blindly required

Many CalcNova focused workflows use path filters intentionally. They are valuable diagnostic gates but should not automatically be made required status checks unless GitHub's configuration guarantees a successful neutral/skipped status for unchanged paths.

Examples include feature-specific accessibility, graph, localization, packaging, CI-hygiene, and other focused validators.

The always-run Source Preflight exists to provide one stable policy check that covers those source contracts without requiring every path-filtered workflow independently.

## Signed commits

Requiring signed commits is **not** part of this initial recommendation because the repository's current connected commit path produces commits that GitHub reports as unsigned.

If a verified signing path is established for all maintainers and automation, signed-commit enforcement can be considered as a later governance hardening step. Do not enable it first and then discover that normal maintenance can no longer be committed.

## Ruleset verification

After enabling the ruleset, verify the branch metadata no longer reports `protected: false` / enforcement off.

Then validate behavior with a test pull request:

- direct merge should be blocked until required checks finish;
- failing Source Preflight should block merge;
- failing Build and Test should block merge;
- failing Dependency Review should block merge;
- failing CodeQL should block merge if configured as required;
- unresolved conversations should block merge when that rule is enabled;
- force-push/deletion behavior should match the intended policy.

Do not mark branch-protection evidence PASS until the GitHub repository setting is actually enabled and observed.

## Evidence state

Use:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Current source readiness:

```text
Always-run Source Preflight source contract: COMPLETE
CI workflow hygiene source contract: COMPLETE
```

Current observed GitHub repository-setting state on 2026-08-23:

```text
main branch protection/ruleset enforcement: FAIL / NOT ENABLED
```

This repository-setting gap does not change CalcNova 2.8.03's completed product/source classification, but it is a meaningful governance/security maintenance item because direct pushes can bypass required-review/check enforcement until the setting is enabled.

## Related files

- `.github/workflows/source-preflight.yml`
- `.github/workflows/build-test.yml`
- `.github/workflows/ci-hygiene-validate.yml`
- `.github/workflows/codeql.yml`
- `.github/workflows/dependency-review.yml`
- `tools/validate_source_preflight_workflow.py`
- `tools/validate_ci_hygiene.py`
- [SOURCE_PREFLIGHT.md](SOURCE_PREFLIGHT.md)
- [CI_HYGIENE.md](CI_HYGIENE.md)
- [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md)
- [SECURITY.md](SECURITY.md)
