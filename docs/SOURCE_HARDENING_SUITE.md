# CalcNova Source Hardening Suite

`tools/source_hardening_suite.py` is an additive release-source gate layered on top of CalcNova's existing integrated source preflight.

It exists so newly added repository hardening checks can be enforced immediately without making concurrent edits to the established preflight inventory a destructive bottleneck.

## Current checks

The suite runs:

1. the normal integrated `tools/release_preflight.py`;
2. the incomplete implementation marker audit;
3. regression tests for the incomplete-code validator;
4. dynamic-control accessibility validation;
5. regression tests for the dynamic-control accessibility validator.

Run it from the repository root:

```bash
python tools/source_hardening_suite.py
```

Validate the suite inventory itself with:

```bash
python -m unittest tools.tests.test_source_hardening_suite
```

## CI

`.github/workflows/source-hardening-suite.yml` runs the suite for broad source, test, tool, documentation, workflow, and state/changelog changes.

Focused workflows remain useful because they produce narrower failure signals. This suite is the cross-contract safety net for the newest additive gates.

## What it proves

A successful run proves only the deterministic source-level contracts executed by the suite.

It does not prove:

- C# compilation if the .NET toolchain was not run;
- Avalonia runtime behavior;
- Android/iOS workload behavior;
- native packaging/signing;
- browser permission behavior;
- screen-reader behavior;
- real device touch/layout behavior.

Those remain separate release evidence and must be recorded from actual execution.
