# CalcNova CI Hygiene

CalcNova uses repository-owned, product-specific GitHub Actions workflows rather than generic starter templates. This document records the maintenance contract that keeps those workflows aligned with the completed .NET 10 / Avalonia codebase.

## Canonical continuous-integration workflows

The primary repository-wide workflows are:

- `.github/workflows/build-test.yml` — cross-platform restore, build, and test for `CalcNova.slnx` on Ubuntu, Windows, and macOS;
- `.github/workflows/format.yml` — restore plus `dotnet format` verification for `CalcNova.slnx`;
- `.github/workflows/docs-check.yml` — mandatory-document and unresolved-placeholder validation;
- `.github/workflows/source-preflight.yml` — SDK-independent integrated source-policy gate;
- `.github/workflows/ci-hygiene-validate.yml` — focused regression gate for this CI hygiene contract.

Feature/platform-specific workflows remain useful for targeted validation, but they do not replace these repository-wide gates.

## Retired generic templates

The following GitHub starter-template workflows are intentionally absent:

```text
.github/workflows/dotnet.yml
.github/workflows/dotnet-desktop.yml
```

They were removed because they did not describe CalcNova's actual build:

- the generic `.NET` template targeted .NET 8 rather than the repository's .NET 10 baseline;
- the desktop template was designed for WPF/Windows Forms + Windows Application Packaging/MSIX;
- it contained unresolved placeholder project paths such as `your-solution-name` and `your-wap-project-path`;
- it depended on signing secrets and WAP packaging that are unrelated to CalcNova's Avalonia desktop release pipeline;
- keeping those templates would create duplicate, misleading, or predictably failing CI signals.

The dedicated CalcNova workflows are the source of truth instead.

## Action-version maintenance baseline

The canonical Build and Test, Formatting, and Documentation Check workflows use:

```text
actions/checkout@v7
```

The .NET workflows use:

```text
actions/setup-dotnet@v6
dotnet-version: 10.0.x
```

`tools/validate_ci_hygiene.py` additionally rejects `actions/checkout` majors 1 through 5 and `actions/setup-dotnet` majors 1 through 5 anywhere under `.github/workflows`.

This lower-bound contract intentionally permits a workflow that still uses `actions/checkout@v6` when a separate source validator explicitly protects that workflow's contract. Future major updates can raise the minimum after compatibility is reviewed.

## Source validator

Run:

```bash
python tools/validate_ci_hygiene.py .
```

The validator checks that:

1. canonical workflows exist;
2. canonical Build and Test / Formatting jobs use the repository's .NET 10 solution commands;
3. canonical Build, Formatting, and Documentation workflows use `actions/checkout@v7`;
4. canonical .NET workflows use `actions/setup-dotnet@v6`;
5. repository-wide canonical workflows retain read-only contents permission;
6. the retired generic starter workflows do not return;
7. known starter-template placeholder paths do not appear in workflow source;
8. obsolete checkout/setup-dotnet major versions do not return.

Regression tests live in:

```text
tools/tests/test_validate_ci_hygiene.py
```

The validator and its regression suite are integrated into `tools/release_preflight.py`.

## Focused workflow

`.github/workflows/ci-hygiene-validate.yml` runs when workflow/preflight hygiene files change and can also be launched manually. It uses read-only repository permission and runs both the validator and its unit tests.

The focused workflow is diagnostic convenience. The integrated Source Preflight remains the broad SDK-independent policy gate.

## Dependabot

`.github/dependabot.yml` continues to check GitHub Actions dependencies weekly. Dependabot pull requests are inputs to maintenance review, not an instruction to preserve obsolete workflows merely so their dependency updates can merge.

If a dependency PR only modifies a workflow that is no longer appropriate for CalcNova, removing the obsolete workflow is preferable to upgrading an invalid template.

## Evidence semantics

Source validation and workflow source do not prove that a GitHub Actions run succeeded. Use the repository's standard evidence vocabulary:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

A CI-hygiene source contract can be complete while a newly pushed workflow run remains `NOT RUN` or unobserved in the available tool environment. Do not convert source inspection into invented hosted-run evidence.

## Version status

This is post-completion maintenance. It does not change:

- product version `2.8.03`;
- normalized package version `2.8.3`;
- normalized release tag `v2.8.3`;
- mobile build code `20803`.

## Related documentation

- [SOURCE_PREFLIGHT.md](SOURCE_PREFLIGHT.md)
- [BRANCH_PROTECTION.md](BRANCH_PROTECTION.md)
- [SECURITY_AUTOMATION.md](SECURITY_AUTOMATION.md)
- [TESTING.md](TESTING.md)
- [RELEASE.md](RELEASE.md)
