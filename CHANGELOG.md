# Changelog

All notable user-visible changes to CalcNova are documented here.

The format is inspired by Keep a Changelog, and the project intends to use semantic versioning for validated releases.

## [Unreleased]

### Added

- Initial modular C#/.NET/Avalonia solution.
- Safe expression tokenizer, parser, and evaluator.
- Arbitrary-precision integer support and decimal-first arithmetic path.
- Standard arithmetic operators and parentheses.
- Scientific functions and angle modes.
- Programmer radix conversion and bitwise helpers.
- Offline fixed-unit conversion engine.
- Initial standard/scientific Avalonia calculator UI.
- Initial desktop host.
- SQLite-backed native calculation history repository.
- Automated source test projects for core, programmer, converter, and persistence behavior.
- GitHub Actions build/test, formatting, and documentation workflow foundations.
- Contributor, support, and security policies.

### Changed

- Package management is centralized through `Directory.Packages.props`.
- Nullable reference types, analyzers, warnings-as-errors, and deterministic build settings are enabled centrally.

### Fixed

- Scientific-notation marker detection in numeric parsing now uses valid APIs.
- Programmer radix parsing safely rejects separator-only and sign-only input.
- Numeric equality and hash-code behavior now share a compatible cross-kind representation.

### Security

- Expression evaluation uses project-owned parsing/evaluation rather than arbitrary code execution.
- Input and expensive integer operations include configurable workload limits.
- Repository ignore rules exclude common signing credentials and local secret files.

## [0.1.0] - Planned

The first validated milestone will be created only after the baseline build, analyzer, formatter, and test suite pass in supported CI environments. It has not been released yet.
