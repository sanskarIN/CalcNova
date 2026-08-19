# CalcNova iOS Release Validation

CalcNova separates iOS **release-tag compilation validation** from Apple signing/distribution credentials.

## Tag-time simulator validation

`.github/workflows/release-ios-validate.yml` runs for pushed `v*` tags and can also be dispatched manually with an already-existing release tag.

The workflow:

1. fetches complete Git history;
2. verifies that the requested tag exists;
3. detaches the checkout at that exact tag;
4. installs .NET 10 and the iOS workload on `macos-latest`;
5. chooses `iossimulator-arm64` or `iossimulator-x64` from the runner architecture;
6. restores the tagged `CalcNova.iOS` project;
7. builds the tagged iOS simulator head in Release configuration.

This closes the source/release gap where iOS previously had a normal validation workflow but no release-tag-specific validation path.

## What this workflow proves

When an actual run passes, it can provide evidence that the selected release tag compiles for the chosen iOS simulator runtime on the workflow's macOS/.NET/Xcode environment.

It does **not** prove:

- physical-device installation;
- code signing;
- provisioning-profile correctness;
- archive/export success;
- App Store Connect upload/processing;
- TestFlight behavior;
- App Store review acceptance;
- VoiceOver/Dynamic Type/device-layout behavior.

Those checks require supported Apple tooling, credentials, and target/runtime evidence.

## Signing policy

Do not commit certificates, private keys, `.p12` files, provisioning profiles, signing passwords, App Store Connect private keys, or other secrets to the repository.

A future signed/archive workflow must obtain credentials from an approved secret store, use temporary runner files, avoid printing secret material, and remove temporary signing artifacts even when a job fails.

## Contract validation

`tools/validate_release_ios_workflow.py` protects the tag-first, simulator-only source contract. Its regression tests live in `tools/tests/test_validate_release_ios_workflow.py`, and `.github/workflows/release-ios-workflow-validate.yml` provides a focused CI signal when this workflow or its validator changes.

## Evidence status

The workflow/source contract exists, but no simulator-build PASS should be claimed until a real run for the relevant commit/tag is observed. If no suitable run has been observed, record iOS release validation as `NOT RUN` rather than inferring success from workflow presence.
