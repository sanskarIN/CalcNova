#!/usr/bin/env python3
"""Run CalcNova's integrated preflight plus newest additive source hardening gates."""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path

CHECKS: tuple[tuple[str, tuple[str, ...]], ...] = (
    ("Integrated source preflight", ("tools/release_preflight.py",)),
    ("Incomplete implementation audit", ("tools/validate_incomplete_code.py", ".")),
    ("Incomplete implementation validator tests", ("-m", "unittest", "tools.tests.test_validate_incomplete_code")),
    ("Dynamic-control accessibility", ("tools/validate_dynamic_controls_accessibility.py", ".")),
    ("Dynamic-control accessibility tests", ("-m", "unittest", "tools.tests.test_validate_dynamic_controls_accessibility")),
)


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    failed = False

    for label, arguments in CHECKS:
        print(f"\n==> {label}", flush=True)
        completed = subprocess.run([sys.executable, *arguments], cwd=root, check=False)
        if completed.returncode != 0:
            failed = True
            print(f"FAILED: {label} exited with {completed.returncode}.", file=sys.stderr)

    if failed:
        print("\nCalcNova source hardening suite FAILED.", file=sys.stderr)
        return 1

    print("\nCalcNova source hardening suite passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
