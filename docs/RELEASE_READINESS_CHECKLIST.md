# CalcNova Release Readiness Checklist

Use this checklist for a real release candidate. Check an item only when there is concrete evidence from the release commit/tag and the stated environment.

## Source and repository preflight

- [ ] `python tools/release_preflight.py --tag <release-tag>` passes from the release-tag checkout.
- [ ] Repository required-file/secret guards pass.
- [ ] All Avalonia `.axaml` files parse as XML.
- [ ] Shared UI/navigation/keyboard contracts pass.
- [ ] Accessibility source contracts pass.
- [ ] Localization catalog/preference contracts pass.
- [ ] Onboarding persistence/visual/focus contracts pass.
- [ ] Packaging metadata contracts pass.
- [ ] Release-tag tests and requested tag validation pass.

## .NET quality gate

- [ ] `dotnet restore CalcNova.slnx` passes.
- [ ] `dotnet format CalcNova.slnx --verify-no-changes --no-restore` passes.
- [ ] `dotnet build CalcNova.slnx --configuration Release --no-restore` passes.
- [ ] `dotnet test CalcNova.slnx --configuration Release --no-build` passes.
- [ ] Compiler warnings introduced by the release are reviewed.
- [ ] Analyzer warnings introduced by the release are reviewed.

## Core calculation behavior

- [ ] Arithmetic precedence/parentheses regression set passes.
- [ ] Power associativity regression set passes.
- [ ] Decimal/large-number boundaries are checked.
- [ ] Scientific-function domain errors are checked.
- [ ] Degree/radian/gradian behavior is checked.
- [ ] Percentage behavior is checked.
- [ ] Memory operations are checked.
- [ ] Result reuse is checked.
- [ ] Clipboard paste sanitization/rejection behavior is checked.

## Programmer mode

- [ ] Base 2/8/10/16 common conversions are checked.
- [ ] At least one nonstandard radix is checked.
- [ ] 8/16/32/64/128-bit word-size boundaries are checked.
- [ ] Signed two's-complement interpretation is checked.
- [ ] AND/OR/XOR/NOT are checked.
- [ ] Left/logical-right/arithmetic-right shifts are checked.
- [ ] Interactive bit toggling is checked.
- [ ] Copy representations are checked.

## Converter and currency

- [ ] Offline unit identities and representative cross-unit conversions are checked.
- [ ] Unit search is checked.
- [ ] Swap is checked.
- [ ] Significant-digit formatting is checked.
- [ ] Recent/favorite pair persistence is checked.
- [ ] Currency provider refresh behavior is checked on a target with network access.
- [ ] Cached currency fallback is checked.
- [ ] Currency failure messages do not expose secrets/raw internal details.

## Statistics, equations, matrices, graphing

- [ ] Statistics representative datasets and malformed input are checked.
- [ ] Quadratic normal/degenerate/error cases are checked.
- [ ] Matrix determinant/inverse/rank/solve cases are checked.
- [ ] Singular/malformed matrix cases are checked.
- [ ] Graph sampling bounds are checked.
- [ ] Graph discontinuity/error behavior is checked.
- [ ] Trace behavior is checked.
- [ ] Numerical derivative/root/integration results are explicitly treated as approximate.
- [ ] CSV copy/export is checked.
- [ ] SVG output is checked for deterministic/text-accessible metadata.

## History and settings persistence

- [ ] History create/search/favorite/delete/clear flows are checked.
- [ ] History limit is enforced.
- [ ] History export formats are checked.
- [ ] Theme preference persists.
- [ ] Angle-unit preference persists.
- [ ] Culture preference persists.
- [ ] Accessibility preferences persist.
- [ ] Converter state persists.
- [ ] Corrupt/malformed settings fail safely.

## Onboarding

- [ ] Clean first launch shows onboarding only after settings load.
- [ ] Complete dismisses and persists.
- [ ] Skip dismisses and persists.
- [ ] Show introduction again reopens without resetting other settings.
- [ ] Ordinary settings reset does not unexpectedly re-trigger onboarding.
- [ ] Onboarding-state storage failure does not leave the main workspace disabled incorrectly.
- [ ] Background mode controls are not keyboard reachable while onboarding is visible.
- [ ] Background calculator/mode shortcuts do not fire through onboarding.
- [ ] Focus enters onboarding predictably.
- [ ] Focus returns to the calculator predictably after dismissal.

## Localization

- [ ] English source catalog is complete.
- [ ] Regional English preference such as `en-IN` behaves as documented.
- [ ] Unsupported cultures fall back/reject safely.
- [ ] No additional language is advertised unless its catalog has been reviewed.
- [ ] Parser/persisted mathematical syntax remains culture-independent.
- [ ] Any newly localized visible strings fit compact/medium/expanded layouts.

## Accessibility

- [ ] Keyboard-only Calculator workflow is usable.
- [ ] Keyboard-only Settings/onboarding workflow is usable.
- [ ] Focus remains visible on supported desktop/browser targets.
- [ ] Screen-reader smoke test is completed on each release-supported target where tooling is available.
- [ ] Symbol-heavy keys announce understandable names.
- [ ] Programmer bit states announce understandable state.
- [ ] Text scaling/large text is checked.
- [ ] Light and dark themes are checked.
- [ ] CalcNova high-contrast preference is checked.
- [ ] System high-contrast composition is checked where available.
- [ ] Reduced-motion behavior is checked if the release adds any motion/transition.
- [ ] No essential information depends only on color or animation.

## Responsive layout

- [ ] Compact width is checked.
- [ ] Medium width is checked.
- [ ] Expanded width is checked.
- [ ] Narrow portrait mobile layout is checked.
- [ ] Mobile landscape layout is checked.
- [ ] Tablet-sized layout is checked.
- [ ] Desktop resize behavior is checked.
- [ ] Wide scientific/date grids remain reachable at compact widths.
- [ ] 64/128-bit programmer grids remain usable/reachable on narrow targets.

## Desktop targets

### Windows

- [ ] Release publish completes.
- [ ] App launches.
- [ ] Core modes smoke-test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist across restart.
- [ ] Packaging/install path is checked if distributing an installer/package.

### Linux

- [ ] Release publish completes.
- [ ] App launches on a representative supported distribution.
- [ ] Core modes smoke-test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist across restart.
- [ ] `.desktop`/AppStream metadata is checked in the chosen packaging path.

### macOS

- [ ] Release publish completes on the required macOS toolchain.
- [ ] App launches.
- [ ] Core modes smoke-test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist across restart.
- [ ] Bundle metadata is generated from the release template.
- [ ] Signing/notarization is completed if required for distribution.

## Browser

- [ ] Required WebAssembly workload installs/builds successfully.
- [ ] Browser publish completes.
- [ ] App loads in each release-supported browser.
- [ ] Browser local settings persist.
- [ ] Browser history persists.
- [ ] Clipboard permission/failure flows are usable.
- [ ] Currency networking obeys documented optional behavior.
- [ ] Offline/cached behavior is checked where applicable.

## Android

- [ ] Android workload restore/publish succeeds.
- [ ] Release application ID/version metadata is correct.
- [ ] Signed AAB is produced only with configured external secrets.
- [ ] Temporary signing material is removed after the build.
- [ ] App installs/launches on representative device/emulator.
- [ ] Portrait/landscape layout is checked.
- [ ] Local settings/history persist.
- [ ] Clipboard behavior is checked.
- [ ] Accessibility service/screen-reader smoke test is completed.
- [ ] Store pre-launch/package checks are reviewed before publication.

## iOS

- [ ] iOS workload/build succeeds on macOS/Xcode.
- [ ] Bundle/application identity is correct.
- [ ] Launch metadata is correct.
- [ ] Signing/provisioning uses external secure material.
- [ ] App installs/launches on representative simulator/device as appropriate.
- [ ] Portrait/landscape layout is checked.
- [ ] Local settings/history persist.
- [ ] Clipboard behavior is checked.
- [ ] VoiceOver/accessibility smoke test is completed.
- [ ] Distribution/archive checks are completed before publication.

## Security and privacy

- [ ] No secret/signing file is tracked.
- [ ] Dependency/security alerts are reviewed.
- [ ] Currency networking behavior matches privacy/security documentation.
- [ ] Core calculation features do not require an account.
- [ ] Local-first data behavior matches documentation.
- [ ] Error output does not expose internal secrets/credentials.
- [ ] Support/donation links remain optional and separate from core functionality.

## Release artifacts

- [ ] Every artifact comes from the release tag.
- [ ] Stable releases do not attach debug builds.
- [ ] Checksums are generated for attached artifacts.
- [ ] Existing release reruns preserve release notes/history and replace only intended assets.
- [ ] Android artifact is omitted rather than falsely published as signed when signing secrets are absent.
- [ ] Release notes include important changes and known limitations.

## Evidence record

Record each target separately:

```text
Source preflight: PASS / FAIL / NOT RUN
.NET restore/format/build/test: PASS / FAIL / NOT RUN
Windows: PASS / FAIL / NOT RUN
Linux: PASS / FAIL / NOT RUN
macOS: PASS / FAIL / NOT RUN
Browser: PASS / FAIL / NOT RUN
Android: PASS / FAIL / NOT RUN
iOS: PASS / FAIL / NOT RUN
Accessibility audit: PASS / FAIL / NOT RUN
Responsive-layout audit: PASS / FAIL / NOT RUN
```

Never replace `NOT RUN` with `PASS` because source files or workflows merely exist.
