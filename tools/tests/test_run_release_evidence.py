#!/usr/bin/env python3

from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

from tools.release_evidence import EvidenceStatus
from tools.run_release_evidence import CommandSpec, blocked, platform_plan, run_command, run_sequential_plan


class ReleaseEvidenceRunnerTests(unittest.TestCase):
    def test_run_command_records_pass_and_log(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            logs = root / "logs"
            spec = CommandSpec("ok", "Successful check", (sys.executable, "-c", "print('hello')"))

            check = run_command(root, logs, spec)

            self.assertEqual(EvidenceStatus.PASS, check.status)
            self.assertEqual(0, check.exit_code)
            self.assertEqual("hello\n", (logs / "ok.log").read_text(encoding="utf-8"))

    def test_run_command_records_failure_and_nonzero_exit(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            spec = CommandSpec("bad", "Failing check", (sys.executable, "-c", "raise SystemExit(7)"))

            check = run_command(root, root / "logs", spec)

            self.assertEqual(EvidenceStatus.FAIL, check.status)
            self.assertEqual(7, check.exit_code)

    def test_sequential_plan_blocks_downstream_checks_after_failure(self) -> None:
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            sentinel = root / "should-not-exist"
            plan = (
                CommandSpec("first", "First", (sys.executable, "-c", "raise SystemExit(2)")),
                CommandSpec(
                    "second",
                    "Second",
                    (sys.executable, "-c", f"from pathlib import Path; Path({str(sentinel)!r}).write_text('ran')"),
                ),
            )

            checks = run_sequential_plan(root, root / "logs", plan)

            self.assertEqual(EvidenceStatus.FAIL, checks[0].status)
            self.assertEqual(EvidenceStatus.BLOCKED, checks[1].status)
            self.assertFalse(sentinel.exists())

    def test_blocked_check_preserves_command_and_reason(self) -> None:
        spec = CommandSpec("build", "Build", ("dotnet", "build"))

        check = blocked(spec, "Prerequisite failed.")

        self.assertEqual(EvidenceStatus.BLOCKED, check.status)
        self.assertEqual("Prerequisite failed.", check.reason)
        self.assertIn("dotnet build", check.command or "")

    def test_platform_plans_use_release_projects(self) -> None:
        self.assertIn("CalcNova.Desktop.csproj", platform_plan("desktop").display_command)
        self.assertIn("CalcNova.Browser.csproj", platform_plan("browser").display_command)
        self.assertIn("CalcNova.Android.csproj", platform_plan("android").display_command)
        self.assertIn("CalcNova.iOS.csproj", platform_plan("ios").display_command)

    def test_unknown_platform_is_rejected(self) -> None:
        with self.assertRaises(ValueError):
            platform_plan("unknown")


if __name__ == "__main__":
    unittest.main()
