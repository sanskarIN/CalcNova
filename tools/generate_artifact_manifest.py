#!/usr/bin/env python3
"""Generate a deterministic CalcNova artifact integrity manifest."""

from __future__ import annotations

import argparse
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from tools.artifact_manifest import collect_records, write_manifest


def git_commit(root: Path) -> str:
    completed = subprocess.run(
        ("git", "rev-parse", "HEAD"),
        cwd=root,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.DEVNULL,
        check=False,
    )
    return completed.stdout.strip() if completed.returncode == 0 and completed.stdout.strip() else "unknown"


def main() -> int:
    parser = argparse.ArgumentParser(description="Generate a CalcNova artifact SHA-256 manifest.")
    parser.add_argument("paths", nargs="+", help="Artifact files/directories relative to --root")
    parser.add_argument("--root", default=".", help="Repository or artifact root")
    parser.add_argument(
        "--output",
        default="artifacts/manifest.json",
        help="Manifest output path; relative paths are resolved under --root",
    )
    parser.add_argument("--repository", default="sanskarIN/CalcNova", help="Repository identity")
    parser.add_argument("--commit", help="Commit identity; defaults to git HEAD when available")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    output = Path(args.output)
    if not output.is_absolute():
        output = root / output

    try:
        records = collect_records(root, [Path(path) for path in args.paths])
        output_relative = output.resolve().relative_to(root) if output.resolve().is_relative_to(root) else None
        if output_relative is not None and any(record.path == output_relative.as_posix() for record in records):
            raise ValueError("Manifest output cannot also be included as an artifact input.")
        write_manifest(output, args.repository, args.commit or git_commit(root), records)
    except (OSError, ValueError) as exception:
        print(f"Artifact manifest generation failed: {exception}", file=sys.stderr)
        return 1

    print(f"Wrote {len(records)} artifact record(s) to {output}.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
