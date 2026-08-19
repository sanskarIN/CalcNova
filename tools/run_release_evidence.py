#!/usr/bin/env python3
"""Run CalcNova validation commands and emit conservative JSON release evidence."""

from __future__ import annotations

import argparse
import platform
import shlex
import shutil
import subprocess
import sys
import time
from dataclasses import dataclass
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
if str(ROOT) not in sys.path:
    sys.path.insert(0, str(ROOT))

from tools.release_evidence import EvidenceCheck, EvidenceStatus, create_evidence


@dataclass(frozen=True)
class CommandSpec:
    id: str
    label: str
    command: tuple[str, ...]

    @property
    def display_command(self) -> str:
        return shlex.join(self.command)


SOURCE_PLAN: tuple[CommandSpec, ...] = (
    CommandSpec(
        "source-hardening",
        "Source hardening suite",
        (sys.executable, "tools/source_hardening_suite.py"),
    ),
)

CORE_PLAN: tuple[CommandSpec, ...] = (
    CommandSpec("restore", ".NET restore", ("dotnet", "restore", "CalcNova.slnx")),
    CommandSpec(
        "format",
        ".NET formatting verification",
        ("dotnet", "format", "CalcNova.slnx", "--verify-no-changes", "--no-restore"),
    ),
    CommandSpec(
        "build",
        "Release solution build",
        ("dotnet", "build", "CalcNova.slnx", "--configuration", "Release", "--no-restore"),
    ),
    CommandSpec(
        "test",
        "Release solution tests",
        ("dotnet", "test", "CalcNova.slnx", "--configuration", "Release", "--no-build"),
    ),
)


def run_command(root: Path, logs_dir: Path, spec: CommandSpec) -> EvidenceCheck:
    logs_dir.mkdir(parents=True, exist_ok=True)
    log_path = logs_dir / f"{spec.id}.log"
    started = time.perf_counter()
    try:
        completed = subprocess.run(
            spec.command,
            cwd=root,
            text=True,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            check=False,
        )
        output = completed.stdout or ""
        exit_code = completed.returncode
    except OSError as exception:
        output = f"{type(exception).__name__}: {exception}\n"
        exit_code = 127

    duration = time.perf_counter() - started
    log_path.write_text(output, encoding="utf-8")
    status = EvidenceStatus.PASS if exit_code == 0 else EvidenceStatus.FAIL
    return EvidenceCheck(
        id=spec.id,
        label=spec.label,
        status=status,
        command=spec.display_command,
        exit_code=exit_code,
        duration_seconds=duration,
        log_file=str(log_path.relative_to(root)) if log_path.is_relative_to(root) else str(log_path),
    )


def blocked(spec: CommandSpec, reason: str) -> EvidenceCheck:
    return EvidenceCheck(
        id=spec.id,
        label=spec.label,
        status=EvidenceStatus.BLOCKED,
        command=spec.display_command,
        reason=reason,
    )


def not_run(check_id: str, label: str, reason: str) -> EvidenceCheck:
    return EvidenceCheck(check_id, label, EvidenceStatus.NOT_RUN, reason=reason)


def git_commit(root: Path) -> str:
    completed = subprocess.run(
        ("git", "rev-parse", "HEAD"),
        cwd=root,
        text=True,
        stdout=subprocess.PIPE,
        stderr=subprocess.DEVNULL,
        check=False,
    )
    if completed.returncode != 0:
        return "unknown"
    return completed.stdout.strip() or "unknown"


def run_sequential_plan(
    root: Path,
    logs_dir: Path,
    plan: tuple[CommandSpec, ...],
    prerequisite_label: str | None = None,
) -> list[EvidenceCheck]:
    checks: list[EvidenceCheck] = []
    blocking_reason: str | None = None

    for spec in plan:
        if blocking_reason is not None:
            checks.append(blocked(spec, blocking_reason))
            continue

        check = run_command(root, logs_dir, spec)
        checks.append(check)
        if check.status is EvidenceStatus.FAIL:
            blocking_reason = f"Blocked because prerequisite check '{check.label}' failed."

    return checks


def platform_plan(target: str) -> CommandSpec:
    if target == "desktop":
        return CommandSpec(
            "desktop-publish",
            "Current-host Desktop publish",
            (
                "dotnet",
                "publish",
                "src/CalcNova.Desktop/CalcNova.Desktop.csproj",
                "--configuration",
                "Release",
                "--output",
                "artifacts/validation/desktop",
            ),
        )
    if target == "browser":
        return CommandSpec(
            "browser-publish",
            "Browser publish",
            (
                "dotnet",
                "publish",
                "src/CalcNova.Browser/CalcNova.Browser.csproj",
                "--configuration",
                "Release",
                "--output",
                "artifacts/validation/browser",
            ),
        )
    if target == "android":
        return CommandSpec(
            "android-build",
            "Android Release build",
            (
                "dotnet",
                "build",
                "src/CalcNova.Android/CalcNova.Android.csproj",
                "--configuration",
                "Release",
            ),
        )
    if target == "ios":
        architecture = platform.machine().lower()
        rid = "iossimulator-arm64" if architecture in {"arm64", "aarch64"} else "iossimulator-x64"
        return CommandSpec(
            "ios-simulator-build",
            "iOS simulator Release build",
            (
                "dotnet",
                "build",
                "src/CalcNova.iOS/CalcNova.iOS.csproj",
                "--configuration",
                "Release",
                f"-p:RuntimeIdentifier={rid}",
            ),
        )
    raise ValueError(f"Unsupported platform target: {target}")


def main() -> int:
    parser = argparse.ArgumentParser(description="Collect CalcNova release-validation evidence.")
    parser.add_argument("--root", default=".", help="Repository root")
    parser.add_argument(
        "--output",
        default="artifacts/validation/release-evidence.json",
        help="JSON evidence output path, relative to the repository root by default",
    )
    parser.add_argument(
        "--scope",
        choices=("source", "core"),
        default="core",
        help="source runs source hardening only; core additionally runs restore/format/build/test",
    )
    parser.add_argument(
        "--platform",
        action="append",
        choices=("desktop", "browser", "android", "ios"),
        default=[],
        help="Optional target validation to run after the requested scope; may be repeated",
    )
    parser.add_argument("--repository", default="sanskarIN/CalcNova", help="Repository identity for evidence")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    if not (root / "CalcNova.slnx").is_file():
        print(f"Not a CalcNova repository root: {root}", file=sys.stderr)
        return 2

    output_path = Path(args.output)
    if not output_path.is_absolute():
        output_path = root / output_path
    logs_dir = output_path.parent / "logs"

    checks = run_sequential_plan(root, logs_dir, SOURCE_PLAN)
    source_failed = any(check.status is EvidenceStatus.FAIL for check in checks)

    if args.scope == "core":
        if shutil.which("dotnet") is None:
            checks.extend(blocked(spec, ".NET SDK executable is unavailable on this host.") for spec in CORE_PLAN)
        elif source_failed:
            checks.extend(blocked(spec, "Blocked because source hardening failed.") for spec in CORE_PLAN)
        else:
            checks.extend(run_sequential_plan(root, logs_dir, CORE_PLAN))

    core_failed_or_blocked = any(
        check.id in {spec.id for spec in CORE_PLAN}
        and check.status in (EvidenceStatus.FAIL, EvidenceStatus.BLOCKED)
        for check in checks
    )

    requested_platforms = tuple(dict.fromkeys(args.platform))
    for target in ("desktop", "browser", "android", "ios"):
        spec = platform_plan(target)
        if target not in requested_platforms:
            checks.append(not_run(spec.id, spec.label, f"Platform target '{target}' was not requested."))
            continue
        if shutil.which("dotnet") is None:
            checks.append(blocked(spec, ".NET SDK executable is unavailable on this host."))
            continue
        if args.scope == "core" and core_failed_or_blocked:
            checks.append(blocked(spec, "Blocked because the core .NET validation gate did not pass."))
            continue
        if target == "ios" and platform.system() != "Darwin":
            checks.append(blocked(spec, "iOS simulator builds require a supported macOS/Xcode host."))
            continue
        checks.append(run_command(root, logs_dir, spec))

    checks.extend(
        [
            not_run(
                "runtime-accessibility",
                "Runtime accessibility audit",
                "Manual target screen-reader/focus/contrast/text-scaling evidence is not automated by this collector.",
            ),
            not_run(
                "responsive-layout-runtime",
                "Runtime responsive-layout audit",
                "Manual target compact/medium/expanded/orientation evidence is not automated by this collector.",
            ),
            not_run(
                "signed-distribution",
                "Signed distribution/store validation",
                "Signing, provisioning, notarization, and store checks require external secure credentials/tooling.",
            ),
        ]
    )

    evidence = create_evidence(args.repository, git_commit(root), checks)
    evidence.write_json(output_path)
    print(f"Wrote release evidence: {output_path}")

    failed = any(check.status is EvidenceStatus.FAIL for check in checks)
    return 1 if failed else 0


if __name__ == "__main__":
    raise SystemExit(main())
