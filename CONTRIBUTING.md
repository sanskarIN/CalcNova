# Contributing to CalcNova

Thank you for considering a contribution to CalcNova. The project values correctness, accessibility, maintainability, privacy, and focused changes over feature count.

## Before you start

1. Read `PROJECT_STATE.md` to understand current implementation status.
2. Read `docs/ARCHITECTURE.md` before changing dependency boundaries.
3. Check existing issues before starting duplicate work.
4. For a substantial new feature, open a feature request before investing in a large implementation.
5. Never place secrets, private signing material, tokens, credentials, or personal data in an issue, commit, screenshot, or pull request.

## Development environment

Use the SDK selected by `global.json` and restore packages from the repository root:

```bash
dotnet restore CalcNova.slnx
```

Before opening a pull request, run as many of the following checks as your environment supports:

```bash
dotnet format CalcNova.slnx --verify-no-changes --no-restore
dotnet build CalcNova.slnx --configuration Release --no-restore
dotnet test CalcNova.slnx --configuration Release --no-build
```

Do not state that a check passed unless it actually completed successfully. Use `NOT RUN` and explain why when a platform or workload is unavailable.

## Branching

- Keep `main` stable.
- Use short-lived feature or fix branches.
- Keep each pull request focused on one logical change.
- Do not rewrite shared history unless repository maintainers explicitly request it.

Examples:

```text
feature/programmer-bit-grid
fix/parser-unary-power
feature/history-search
```

## Commit messages

Use Conventional Commits when practical:

```text
feat(scientific): add inverse trig functions
fix(parser): handle nested unary minus
test(converter): cover temperature boundaries
docs(build): document Linux prerequisites
ci: add formatting verification
```

Each commit should represent a meaningful, reviewable unit. Do not create empty commits or artificial commit spam.

## C# conventions

The repository's `.editorconfig` and `Directory.Build.props` are authoritative.

General expectations:

- nullable reference types stay enabled;
- warnings are treated seriously;
- public/reusable APIs should have clear names and predictable behavior;
- calculation/domain libraries must not depend on Avalonia UI;
- avoid global mutable state;
- avoid duplicate calculation logic;
- use comments to explain non-obvious reasoning rather than restating code;
- do not suppress analyzers without a specific documented reason.

## Mathematical changes

Calculation changes need stronger evidence than ordinary UI changes.

When changing parser, numeric, scientific, programmer, converter, statistics, equation, matrix, or graphing behavior:

1. define expected mathematical semantics;
2. handle invalid domains explicitly;
3. add boundary/edge-case tests;
4. add a regression test for each confirmed bug;
5. document approximation or numeric limitations where relevant;
6. avoid returning plausible-looking values for undefined operations.

## Tests

Add tests near the feature they protect.

Prefer tests for:

- normal cases;
- boundaries;
- invalid input;
- previous regressions;
- deterministic behavior;
- round trips and invariants where appropriate.

UI changes should include relevant headless/UI tests when the infrastructure supports them.

## Accessibility

User-interface changes should consider:

- keyboard access;
- focus order and visible focus;
- semantic labels;
- screen-reader behavior;
- high contrast;
- large text/text scaling;
- touch target size;
- reduced motion;
- avoiding color-only meaning.

Document any known limitation instead of hiding it.

## Documentation

Update documentation in the same change when behavior, commands, package versions, project paths, platform requirements, or user-facing features change.

For significant implementation sessions, update:

- `what_changed.md`;
- `PROJECT_STATE.md`;
- `CHANGELOG.md` when the change is user-visible/release-relevant;
- `docs/ROADMAP.md` when planned work changes.

## Dependencies

Before adding a package, consider:

- whether the framework already provides the needed functionality;
- maintenance activity;
- license compatibility;
- security history;
- supported targets;
- package size;
- long-term API stability.

Avoid adding packages for trivial helpers.

## Pull requests

A good pull request includes:

- concise problem/solution explanation;
- focused diff;
- tests for behavior changes;
- actual validation commands/results;
- screenshots for meaningful UI changes;
- accessibility notes for UI changes;
- documentation changes where needed;
- no unrelated formatting churn.

The pull-request template contains the final checklist.

## Bug reports

Use the repository bug-report form and provide reproducible steps. Calculation expressions can be included when they are safe to share. Remove private information from logs and screenshots.

## Security reports

Do not open a public issue for a suspected security vulnerability. Follow `SECURITY.md` and contact:

**supportramsandesh@gmail.com**

## License

By contributing to this repository, you agree that your contribution may be distributed under the repository's Apache-2.0 license.
