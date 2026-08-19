# CalcNova Machine-Readable Validation Evidence

CalcNova can record validation results as structured JSON instead of relying only on prose checklists.

## Generate evidence

From the repository root:

```bash
python tools/run_release_evidence.py --scope core
```

The default output is:

```text
artifacts/validation/release-evidence.json
```

Command output is preserved under the adjacent `logs/` directory.

## Source-only evidence

When a host has Python but not the required .NET/platform toolchain:

```bash
python tools/run_release_evidence.py --scope source
```

This runs the source-hardening suite and records .NET/platform commands separately rather than pretending they ran.

## Optional platform commands

A platform may be requested explicitly:

```bash
python tools/run_release_evidence.py --scope core --platform desktop
python tools/run_release_evidence.py --scope source --platform browser
python tools/run_release_evidence.py --scope source --platform android
python tools/run_release_evidence.py --scope source --platform ios
```

Platform requests do not install workloads or signing credentials automatically. Missing prerequisites therefore produce conservative evidence instead of silently changing the machine.

The iOS simulator command is marked `BLOCKED` on non-macOS hosts.

## Status vocabulary

Every check uses one of four values:

- `PASS` — the recorded command actually exited successfully;
- `FAIL` — the recorded command actually ran and exited non-zero;
- `NOT RUN` — the check was not requested or is a manual audit outside this collector;
- `BLOCKED` — the check was requested but a prerequisite/toolchain/host requirement prevented execution.

`FAIL` carries a non-zero exit code. `NOT RUN` and `BLOCKED` must carry a reason.

## Require explicit PASS evidence

Generating a JSON file is not the same as approving it. CI/release automation can require specific gates:

```bash
python tools/verify_release_evidence.py artifacts/validation/release-evidence.json --scope core
```

Require a platform too:

```bash
python tools/verify_release_evidence.py artifacts/validation/release-evidence.json \
  --scope core \
  --require-platform desktop
```

The verifier fails if a required entry is missing, `FAIL`, `NOT RUN`, or `BLOCKED`.

## Schema

The machine-readable schema is documented in:

```text
docs/release-evidence.schema.json
```

Current schema version: `1`.

The record includes:

- repository identity;
- exact Git commit when available;
- UTC generation time;
- host operating system and architecture;
- check id/label/status;
- command and exit code when applicable;
- elapsed time;
- log-file location;
- reason for unexecuted/blocked checks.

## Manual/runtime evidence remains separate

The collector always keeps these outside automatic PASS claims unless another evidence process explicitly supplies real target results:

- screen-reader/focus/contrast/text-scaling audit;
- responsive mobile/orientation audit;
- signed distribution/store validation.

This is deliberate. A successful build or publish command does not prove accessibility, device UX, signing, notarization, provisioning, or store acceptance.
