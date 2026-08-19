# CalcNova 2.8.03 Platform Support

## Status

**Cross-platform source composition is complete for CalcNova 2.8.03.**

CalcNova contains completed application composition heads for Desktop, Browser/WebAssembly, Android, and iOS. External runtime/build/signing/store results are tracked as evidence and are not used to redefine the completed source status.

## Platform inventory

| Target | 2.8.03 source status | External evidence status |
|---|---|---|
| Shared domain libraries | COMPLETE | Record observed build/test evidence separately |
| Windows desktop | COMPLETE source composition | Record observed Windows build/launch/package evidence separately |
| Linux desktop | COMPLETE source composition | Record observed Linux build/launch/package evidence separately |
| macOS desktop | COMPLETE source composition | Record observed macOS build/launch/signing evidence separately |
| Browser/WebAssembly | COMPLETE source composition | Record observed publish/browser behavior separately |
| Android | COMPLETE source composition | Record observed workload/device/signing/store evidence separately |
| iOS | COMPLETE source composition | Record observed workload/simulator/device/signing/store evidence separately |

`PROJECT_STATE.md` is the authoritative 2.8.03 completion record.

## Shared platform architecture

Completed platform infrastructure includes:

- shared application composition root;
- shared Avalonia UI;
- shared clipboard abstraction and Avalonia adapter;
- shared external-link abstraction;
- native persistence composition;
- Browser-safe persistence composition;
- platform-specific startup/packaging heads;
- source validators for build workflow contracts;
- release workflow contracts;
- package metadata validation.

## Desktop — Windows, Linux, macOS

`src/CalcNova.Desktop` is the shared Avalonia desktop entry point.

Completed source behavior includes:

- resizable shared window;
- compact/medium/expanded adaptive profiles;
- keyboard calculator and mode-navigation support;
- graph keyboard interaction;
- shared clipboard composition;
- native local persistence composition;
- settings/onboarding/About surfaces;
- 2.8.03 release identity inherited from central build properties.

### Windows

Completed source/release infrastructure:

- Desktop host;
- Windows runner validation workflow;
- Windows release publish path;
- Appx/MSIX manifest template;
- package identity validation;
- release artifact path.

External evidence may record Windows build/launch, DPI/text scaling, clipboard/persistence, accessibility/high-contrast behavior, and the selected installer/package path.

### Linux

Completed source/release infrastructure:

- Desktop host;
- Linux runner validation workflow;
- Linux x64 release publish path;
- `.desktop` metadata;
- AppStream metadata;
- package identity validation.

External evidence may record representative distribution launch behavior, clipboard/persistence, desktop integration, accessibility behavior, and the chosen packaging format.

### macOS

Completed source/release infrastructure:

- Desktop host;
- macOS runner validation workflow;
- macOS release publish path;
- plist template;
- package identity validation.

External evidence may record macOS launch behavior, clipboard/persistence, VoiceOver/keyboard/scaling behavior, bundle generation, signing, and notarization where a distribution path requires them.

Signing/notarization credentials remain external to source control.

## Browser/WebAssembly

Completed source composition includes:

- Browser head;
- WebAssembly workload workflow contract;
- Browser publish path;
- Browser-safe history/settings storage;
- shared application features;
- keyboard and graph interaction source contracts;
- local-first ordinary calculation behavior.

External evidence may record actual publish output, supported-browser load behavior, storage persistence, clipboard permissions/failure handling, keyboard conflicts, accessibility, and optional currency networking behavior.

## Android

Completed Android source/release identity:

- project: `src/CalcNova.Android`;
- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.8.03`;
- numeric build code: `20803`;
- Android workload/Java validation workflow contract;
- release AAB workflow path;
- signing only from external secrets;
- temporary keystore cleanup;
- shared clipboard/settings/history composition.

Release publication does not override the source-owned display/build versions.

External evidence may record workload build output, emulator/device launch, portrait/landscape behavior, persistence, clipboard, TalkBack/large text behavior, signed AAB production, and store checks where credentials/services are available.

## iOS

Completed iOS source/release identity:

- project: `src/CalcNova.iOS`;
- application id: `in.sanskar.calcnova`;
- application title: `CalcNova`;
- display version: `2.8.03`;
- numeric build code: `20803`;
- Info.plist/launch metadata;
- iOS workload workflow contract;
- exact-tag unsigned simulator validation workflow contract;
- shared clipboard/settings/history composition.

External evidence may record macOS/Xcode workload build output, simulator/device behavior, persistence, clipboard, Dynamic Type/VoiceOver behavior, signing/provisioning, archive/TestFlight/App Store processing where required.

The unsigned simulator workflow intentionally does not represent a signed App Store artifact.

## Version mapping

The public CalcNova release version is `2.8.03`.

Strict SemVer package/tag tooling uses:

- normalized package version: `2.8.3`;
- normalized release tag: `v2.8.3`.

See [VERSIONING.md](VERSIONING.md).

## Source validation

Platform and package contracts are protected by:

```bash
python tools/validate_packaging_metadata.py .
python tools/validate_platform_workflows.py .
python tools/validate_completion_status.py .
python tools/release_preflight.py
```

The Source Preflight workflow watches `src/**`, `tests/**`, `tools/**`, `docs/**`, packaging, workflows, and release/build root metadata.

## Evidence vocabulary

Record external target evidence using:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

Do not convert an unexecuted target operation into PASS because source or a workflow exists. Conversely, `NOT RUN` in one environment does **not** mean the 2.8.03 platform source composition is unfinished.

## Final platform classification

- Desktop source composition: **COMPLETE**
- Browser/WebAssembly source composition: **COMPLETE**
- Android source composition: **COMPLETE**
- iOS source composition: **COMPLETE**
- Platform workflow source contracts: **COMPLETE**
- Packaging metadata contracts: **COMPLETE**
- 2.8.03 platform version identity: **COMPLETE**

Future platform changes are maintenance, compatibility/security updates, packaging refinements, or optional enhancements.
