# CalcNova 2.9.5 Platform Support

## Status

**Cross-platform source composition is complete for CalcNova 2.9.5.**

CalcNova contains completed application composition heads for Desktop, Browser/WebAssembly, Android, and iOS. External runtime/build/signing/store results are tracked as evidence and are not used to redefine the completed source status.

## Supported platform matrix

| Platform | Source target | Architecture/runtime contract | Source status | Runtime evidence |
|---|---|---|---|---|
| Windows | Desktop | `win-x64`, `win-arm64` release targets | COMPLETE | Record observed build/launch/package evidence separately |
| Linux | Desktop | `linux-x64`, `linux-arm64` release targets | COMPLETE | Record observed build/launch/package evidence separately |
| macOS | Desktop | `osx-x64`, `osx-arm64` release targets | COMPLETE | Record observed build/launch/signing evidence separately |
| Browser | WebAssembly/PWA | `net10.0-browser`, service worker + web manifest | COMPLETE | Record observed browser behavior separately |
| Android | Native mobile | `android-arm`, `android-arm64`, `android-x86`, `android-x64` | COMPLETE | Record observed emulator/device/signing/store evidence separately |
| iOS | Native mobile | `ios-arm64`, `iossimulator-arm64`, `iossimulator-x64` | COMPLETE | Record observed simulator/device/signing/store evidence separately |

The matrix describes the maintained CalcNova targets. It does not imply support for every operating system, CPU, browser engine, store, or legacy device ever produced.

`PROJECT_STATE.md` is the authoritative 2.9.5 completion record.

## Shared platform architecture

Completed platform infrastructure includes:

- shared application composition root;
- shared Avalonia UI;
- shared clipboard abstraction and Avalonia adapter;
- shared external-link abstraction;
- shared history/settings contracts;
- native persistence composition;
- Browser-safe persistence composition;
- platform-specific startup/packaging heads;
- browser PWA resources;
- source validators for platform composition and build workflows;
- release workflow contracts;
- package metadata validation;
- centralized release-identity validation.

The platform heads remain thin. Calculator/domain behavior stays in shared projects wherever practical, while startup, lifecycle, packaging, storage selection, links, and unavoidable native integration stay in the platform heads.

## Desktop — Windows, Linux, macOS

`src/CalcNova.Desktop` is the shared Avalonia desktop entry point and uses platform detection rather than separate Windows/Linux/macOS UI forks.

Completed source behavior includes:

- resizable shared window;
- compact/medium/expanded adaptive profiles;
- keyboard calculator and mode-navigation support;
- graph keyboard interaction;
- shared clipboard composition;
- native SQLite history and JSON settings/cache composition;
- external-link composition;
- settings/onboarding/About surfaces;
- 2.9.5 release identity inherited from central build properties.

The stable release workflow publishes self-contained desktop archives for both x64 and ARM64 on all three maintained desktop operating systems:

- Windows: `win-x64`, `win-arm64`;
- Linux: `linux-x64`, `linux-arm64`;
- macOS: `osx-x64`, `osx-arm64`.

Each architecture is packaged independently and protected by the release-workflow source validator. Runtime/package execution evidence for each architecture remains separate from source completeness.

### Windows

Completed source/release infrastructure:

- Desktop host;
- Windows runner validation workflow;
- Windows x64 and ARM64 self-contained release publish paths;
- Appx/MSIX manifest template;
- package identity validation;
- release artifact path.

External evidence may record Windows build/launch, DPI/text scaling, clipboard/persistence, accessibility/high-contrast behavior, native x64/ARM64 execution, and the selected installer/package path.

### Linux

Completed source/release infrastructure:

- Desktop host;
- Linux runner validation workflow;
- Linux x64 and ARM64 self-contained release publish paths;
- `.desktop` metadata;
- AppStream metadata with stable 2.8.03, 2.9.0, and 2.9.5 release entries;
- package identity validation.

External evidence may record representative distribution launch behavior, clipboard/persistence, desktop integration, accessibility behavior, native x64/ARM64 execution, and the chosen packaging format.

### macOS

Completed source/release infrastructure:

- Desktop host;
- macOS runner validation workflow;
- macOS Intel x64 and Apple Silicon ARM64 self-contained release publish paths;
- plist template;
- package identity validation.

External evidence may record macOS launch behavior, clipboard/persistence, VoiceOver/keyboard/scaling behavior, native Intel/Apple Silicon execution, bundle generation, signing, and notarization where a distribution path requires them.

Signing/notarization credentials remain external to source control.

## Browser/WebAssembly and PWA

Completed source composition includes:

- `net10.0-browser` Browser head;
- Avalonia Browser composition;
- WebAssembly workload workflow contract;
- Browser publish path;
- Browser-safe history/settings storage;
- Browser-safe currency cache;
- shared application features;
- keyboard and graph interaction source contracts;
- local-first ordinary calculation behavior;
- `index.html` application host;
- `manifest.webmanifest` install metadata;
- `service-worker.js` offline/cache infrastructure;
- maintained browser icon resources.

External evidence may record actual publish output, Chromium/Firefox/Safari load behavior where applicable, installability, storage persistence, offline behavior, clipboard permissions/failure handling, keyboard conflicts, accessibility, and optional currency networking behavior.

## Android

Completed Android source/release identity:

- project: `src/CalcNova.Android`;
- target framework: `net10.0-android`;
- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.9.5`;
- numeric build code: `20905`;
- explicit runtime identifiers: `android-arm`, `android-arm64`, `android-x86`, `android-x64`;
- Android workload/Java validation workflow contract;
- release AAB workflow path;
- signing only from external secrets;
- temporary keystore cleanup;
- native app-files storage selection;
- SQLite history, JSON settings/cache, shared clipboard, and Android external-link composition.

Release publication does not override the source-owned display/build versions.

External evidence may record workload build output, emulator/device launch, portrait/landscape behavior, persistence, clipboard, TalkBack/large text behavior, signed AAB production, and store checks where credentials/services are available.

## iOS

Completed iOS source/release identity:

- project: `src/CalcNova.iOS`;
- target framework: `net10.0-ios`;
- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.9.5`;
- numeric build code: `20905`;
- explicit device runtime identifier: `ios-arm64`;
- explicit simulator runtime identifiers: `iossimulator-arm64`, `iossimulator-x64`;
- Info.plist/launch metadata;
- iOS workload workflow contract;
- exact-tag unsigned simulator validation workflow contract;
- native local-data storage selection with a documents fallback;
- SQLite history, JSON settings/cache, shared clipboard, and iOS external-link composition.

External evidence may record macOS/Xcode workload build output, simulator/device behavior, persistence, clipboard, Dynamic Type/VoiceOver behavior, signing/provisioning, archive/TestFlight/App Store processing where required.

The unsigned simulator workflow intentionally does not represent a signed App Store artifact.

## Cross-platform source contract

`tools/validate_platform_support.py` fails if maintained platform composition drifts. It protects:

- Desktop `net10.0` + Avalonia Desktop composition;
- Desktop platform detection and native history/settings/cache/link/clipboard services;
- Browser `net10.0-browser` + Avalonia Browser composition;
- Browser-safe history/settings/cache/link/clipboard services;
- required Browser/PWA resources;
- Android target, application identity/version, all declared Android runtime identifiers, and native composition services;
- iOS target, application identity/version, device/simulator runtime identifiers, and native composition services;
- the shared `CalcNova.Platform` project and clipboard/link/history/settings contracts;
- presence of this platform-support document.

A focused `Platform Support Validate` GitHub Actions workflow runs the validator and its regression suite when relevant platform source, documentation, or validator files change. The same validator and regression suite are also part of `tools/release_preflight.py`.

## Build workflow contract

`tools/validate_platform_workflows.py` independently protects the build workflows and shared SDK policy. Its current contract requires `actions/checkout@v7`, .NET 10, the appropriate WebAssembly/Android/iOS workloads, Java 17 for Android, read-only validation permissions, and the Windows/Linux/macOS desktop runner matrix.

This separation is intentional:

- `validate_platform_support.py` protects **what platform source exists and how it is composed**;
- `validate_platform_workflows.py` protects **how platform CI validates that source**.

## Version mapping

The public CalcNova release version is `2.9.5`.

Current package/tag/build identity:

- package version: `2.9.5`;
- release tag: `v2.9.5`;
- mobile build code: `20905`.

The requested 2.9.0 checkpoint used tag `v2.9.0` and build code `20900`; see [releases/2.9.0.md](releases/2.9.0.md).

See [VERSIONING.md](VERSIONING.md).

## Source validation

Platform and package contracts are protected by:

```bash
python -m unittest tools.tests.test_release_identity
python tools/validate_platform_support.py .
python tools/validate_platform_workflows.py .
python tools/validate_packaging_metadata.py .
python tools/validate_release_workflow.py .
python tools/validate_completion_status.py .
python tools/release_preflight.py --tag v2.9.5
```

The release-workflow validator protects the six x64/ARM64 desktop release targets in addition to release-tag/version/publication safety contracts.

The Source Preflight workflow watches `src/**`, `tests/**`, `tools/**`, `docs/**`, packaging, workflows, and release/build root metadata.

## Evidence vocabulary

Record external target evidence using:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Do not convert an unexecuted target operation into PASS because source or a workflow exists. Conversely, `NOT RUN` in one environment does **not** mean the 2.9.5 platform source composition is unfinished.

## Remaining platform work

No additional platform head is required for the defined CalcNova 2.9.5 product scope. Remaining platform work is operational or optional:

1. observe hosted CI results for all maintained build workflows after changes;
2. run physical-device Android and iOS smoke/accessibility tests;
3. run browser compatibility/install/offline tests on maintained browser engines;
4. execute downloaded desktop artifacts on representative x64/ARM64 Windows, Linux, and macOS systems;
5. perform signing, notarization, store/TestFlight/Play Console validation when distribution credentials are available;
6. maintain platform SDK/workload/Avalonia compatibility as dependencies evolve;
7. optionally add further OS/CPU/store/package targets only when there is a real support requirement.

These items are evidence, distribution, maintenance, or optional expansion work. They are not unimplemented core calculator features.

## Final platform classification

- Windows desktop source composition: **COMPLETE**
- Linux desktop source composition: **COMPLETE**
- macOS desktop source composition: **COMPLETE**
- Browser/WebAssembly/PWA source composition: **COMPLETE**
- Android source composition + explicit ABI/RID contract: **COMPLETE**
- iOS source composition + explicit device/simulator RID contract: **COMPLETE**
- x64/ARM64 desktop release matrix: **COMPLETE SOURCE CONTRACT**
- Platform workflow source contracts: **COMPLETE**
- Cross-platform composition source contract: **COMPLETE**
- Packaging metadata contracts: **COMPLETE**
- 2.9.5 platform version identity: **COMPLETE**

Future platform changes are maintenance, compatibility/security updates, packaging refinements, evidence collection, or optional enhancements.
