# CalcNova 2.8.03 Release Evidence Checklist

## Purpose

CalcNova 2.8.03 is the completed product baseline. This checklist records **execution evidence** for a release/tag/environment; unchecked items do not change the completed implementation status.

Use only observed statuses:

```text
PASS / FAIL / BLOCKED / NOT RUN
```

## Release identity

- [ ] Product/display version is `2.8.03`.
- [ ] Normalized package version is `2.8.3`.
- [ ] Normalized release tag is `v2.8.3`.
- [ ] Assembly/file version is `2.8.3.0`.
- [ ] Android/iOS display version is `2.8.03`.
- [ ] Android/iOS numeric build code is `20803`.
- [ ] `Directory.Build.props` is the shared version source of truth.
- [ ] Release tag equals `v` plus the normalized source `<Version>`.
- [ ] In-app About shows `Version 2.8.03 • Complete`.

## Source preflight

- [ ] `python tools/release_preflight.py --tag v2.8.3` passes from the exact release-tag checkout.
- [ ] Repository required-file/security guards pass.
- [ ] Avalonia `.axaml` XML validation passes.
- [ ] Shared UI/navigation/keyboard contracts pass.
- [ ] Calculator editing/wrapping contracts pass.
- [ ] Graph interaction/presentation/numerical contracts pass.
- [ ] Unicode metadata contracts pass.
- [ ] Exact-rational contracts pass.
- [ ] Engineering-notation contracts pass.
- [ ] Bounded export-preview contracts pass.
- [ ] Bivariate-statistics contracts pass.
- [ ] Accessibility/focus/adaptive/touch/dynamic-control contracts pass.
- [ ] English/Hindi localization contracts pass.
- [ ] Settings/onboarding/converter contracts pass.
- [ ] Packaging metadata contracts pass.
- [ ] 2.8.03 completion-status contract passes.
- [ ] Platform workflow contracts pass.
- [ ] Source Preflight workflow self-validation passes.
- [ ] Release workflow/tag/documentation contracts pass.
- [ ] Artifact integrity contracts pass.
- [ ] Structured release-evidence contracts pass.

## .NET quality evidence

- [ ] `dotnet restore CalcNova.slnx` passes.
- [ ] `dotnet format CalcNova.slnx --verify-no-changes --no-restore` passes.
- [ ] `dotnet build CalcNova.slnx --configuration Release --no-restore` passes.
- [ ] `dotnet test CalcNova.slnx --configuration Release --no-build` passes.
- [ ] Compiler/analyzer diagnostics introduced by the release commit are reviewed.

## Core calculation evidence

- [ ] Arithmetic precedence/parentheses checks pass.
- [ ] Power associativity checks pass.
- [ ] Decimal/large-number boundaries are checked.
- [ ] Scientific-function domain boundaries are checked.
- [ ] Degree/radian/gradian behavior is checked.
- [ ] Percentage behavior is checked.
- [ ] Memory operations are checked.
- [ ] Repeated-equals/result reuse is checked.
- [ ] Sanitized paste/import behavior is checked.
- [ ] Caret/selection-aware editing is checked.
- [ ] Printable/numpad mappings are checked.

## Exact rational evidence

- [ ] Canonical normalization is checked.
- [ ] Default-value canonical zero is checked.
- [ ] Integer/fraction/decimal/scientific parsing is checked.
- [ ] Exact add/subtract/multiply/divide is checked.
- [ ] Comparison/equality/hashing is checked.
- [ ] Pre-trim input limit is checked.
- [ ] Decimal exponent/scale bound is checked.
- [ ] Reduced bit-length bound is checked.
- [ ] Shared panel operations are checked.

## Engineering notation evidence

- [ ] Multiples-of-three exponent formatting is checked.
- [ ] 1–15 significant digits are checked.
- [ ] Canonical parsing is checked.
- [ ] `-324..306` exponent bounds are checked.
- [ ] Non-zero-underflow rejection is checked.
- [ ] Extreme finite-value round trips are checked.
- [ ] 4,096-character core/App/UI bound is checked.
- [ ] Shared panel Format/Parse behavior is checked.

## Programmer and Unicode evidence

- [ ] Base 2/8/10/16 conversions are checked.
- [ ] At least one nonstandard radix is checked.
- [ ] 8/16/32/64/128-bit boundaries are checked.
- [ ] Signed two's-complement interpretation is checked.
- [ ] AND/OR/XOR/NOT are checked.
- [ ] Left/logical-right/arithmetic-right shifts are checked.
- [ ] Interactive bit toggling is checked.
- [ ] Representation copy actions are checked.
- [ ] Unicode scalar conversion/inspection is checked.
- [ ] Supplementary-plane handling is checked.
- [ ] Local Unicode metadata/copy behavior is checked.

## Converter, currency, and date/time evidence

- [ ] Representative offline unit conversions are checked.
- [ ] Search/swap are checked.
- [ ] Significant-digit formatting is checked.
- [ ] Recents/favorites/restoration/clear are checked.
- [ ] Converter preferences persist.
- [ ] Currency refresh behavior is checked where network access is available.
- [ ] Cached/offline currency fallback is checked.
- [ ] Date difference/calendar/business-day/duration workflows are checked.

## Statistics, equations, matrices, and graph evidence

- [ ] Descriptive statistics are checked.
- [ ] Covariance/correlation/regression/`R²` are checked where defined.
- [ ] Regression prediction is checked.
- [ ] Constant-X/constant-Y/single-pair/mismatched/oversized/non-finite cases are checked.
- [ ] Equation normal/degenerate/error cases are checked.
- [ ] Matrix determinant/inverse/rank/solve are checked.
- [ ] Singular/malformed matrix cases are checked.
- [ ] Graph sample bounds/discontinuities are checked.
- [ ] Pointer pan/wheel zoom/fit are checked.
- [ ] Keyboard pan/zoom/reset/fit are checked.
- [ ] Multi-series patterns/text legend are checked.
- [ ] Trace behavior is checked.
- [ ] Derivative/root/integration behavior is checked as approximate numerical analysis.
- [ ] Numerical workload limits are checked.
- [ ] CSV/SVG output is checked.

## History, settings, and persistence evidence

- [ ] History create/search/favorite/delete/clear flows are checked.
- [ ] History limit is checked.
- [ ] TXT/CSV/JSON export is checked.
- [ ] Bounded preview/full private copy behavior is checked.
- [ ] Theme/angle/culture/accessibility preferences persist.
- [ ] Converter state persists.
- [ ] Current settings schema persists.
- [ ] Legacy/unversioned settings migrate safely.
- [ ] Unsupported future schema fails closed without overwriting stored state.
- [ ] Corrupt settings fail safely.

## Onboarding and localization evidence

- [ ] First-launch onboarding behavior is checked.
- [ ] Complete/Skip persistence is checked.
- [ ] Show-introduction-again behavior is checked.
- [ ] Background shortcuts remain suppressed while onboarding is visible.
- [ ] Focus enters/leaves onboarding predictably.
- [ ] English catalog is complete for the current key set.
- [ ] Hindi catalog is complete for the current key set.
- [ ] Regional English/Hindi selection is checked.
- [ ] Reviewed localized surfaces are checked at representative sizes.

Additional language packs are optional post-2.8.03 improvements.

## Accessibility and responsive-layout evidence

- [ ] Keyboard-only calculator workflow is usable.
- [ ] Ctrl+PageUp/PageDown/Home/End navigation is usable.
- [ ] Focus remains visible on keyboard-capable targets.
- [ ] CalcNova high-contrast focus styling is distinguishable.
- [ ] Screen-reader smoke tests are recorded where tooling is available.
- [ ] Symbol-heavy keys announce understandable names.
- [ ] Programmer bit states announce state.
- [ ] Graph viewport is keyboard operable and textual alternatives are reachable.
- [ ] Large text/display scaling is checked.
- [ ] Light/dark/high-contrast states are checked.
- [ ] No essential information depends only on color or motion.
- [ ] Compact width is checked.
- [ ] Medium width is checked.
- [ ] Expanded width is checked.
- [ ] Mobile portrait/landscape is checked where applicable.
- [ ] Wide programmer grids remain reachable.

## Desktop evidence

### Windows

- [ ] Release publish completes.
- [ ] App launches.
- [ ] Core-mode smoke test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist across restart.
- [ ] Chosen packaging/install path is checked.

### Linux

- [ ] Release publish completes.
- [ ] App launches on a representative target distribution.
- [ ] Core-mode smoke test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist.
- [ ] `.desktop`/AppStream metadata is checked.

### macOS

- [ ] Release publish completes on the required macOS toolchain.
- [ ] App launches.
- [ ] Core-mode smoke test passes.
- [ ] Clipboard works.
- [ ] Local settings/history persist.
- [ ] Bundle metadata is generated from the release template.
- [ ] Signing/notarization is recorded if required for the distribution path.

## Browser evidence

- [ ] Required WebAssembly workload/build succeeds.
- [ ] Browser publish completes.
- [ ] App loads in each claimed browser target.
- [ ] Browser settings/history persist.
- [ ] Legacy Browser settings migrate as expected.
- [ ] Clipboard permission/failure flows are usable.
- [ ] Shell/graph keyboard shortcuts are checked for browser conflicts.
- [ ] Currency network/offline behavior matches documentation.

## Android evidence

- [ ] Android workload restore/publish succeeds.
- [ ] Application id is `in.sanskar.calcnova`.
- [ ] Display version is `2.8.03`.
- [ ] Numeric build code is `20803`.
- [ ] Signed AAB is produced only with configured external secrets.
- [ ] Temporary signing material is removed.
- [ ] App installs/launches on a representative device/emulator.
- [ ] Portrait/landscape behavior is checked.
- [ ] Local settings/history persist.
- [ ] Clipboard behavior is checked.
- [ ] Accessibility smoke test is recorded where tooling is available.
- [ ] Store package checks are recorded if publication is attempted.

## iOS evidence

- [ ] iOS workload/build succeeds on macOS/Xcode.
- [ ] Application id is `in.sanskar.calcnova`.
- [ ] Display version is `2.8.03`.
- [ ] Numeric build code is `20803`.
- [ ] Launch metadata is correct.
- [ ] Unsigned exact-tag simulator workflow is checked where applicable.
- [ ] Signing/provisioning uses external secure material for distribution builds.
- [ ] App installs/launches on representative simulator/device as appropriate.
- [ ] Portrait/landscape behavior is checked.
- [ ] Local settings/history persist.
- [ ] Clipboard behavior is checked.
- [ ] VoiceOver/accessibility evidence is recorded where tooling is available.
- [ ] Archive/distribution evidence is recorded if publication is attempted.

## Security and privacy evidence

- [ ] No secret/signing file is tracked.
- [ ] Dependency/security alerts are reviewed.
- [ ] Currency networking matches privacy/security documentation.
- [ ] Core calculation requires no account.
- [ ] Local-first data behavior matches documentation.
- [ ] Error output does not expose credentials/secrets.
- [ ] Support/donation links remain optional and separate from core functionality.

## Release artifacts

- [ ] Every artifact comes from the normalized release tag `v2.8.3`.
- [ ] Stable release artifacts are not debug builds.
- [ ] SHA-256/checksum material is generated.
- [ ] Artifact manifest/provenance tooling is used where required.
- [ ] Reruns preserve release notes/history and replace only intended assets.
- [ ] Android artifact is omitted rather than falsely published as signed when signing secrets are absent.

## Evidence record

```text
Product: CalcNova 2.8.03 — COMPLETE
Normalized tag: v2.8.3
Source preflight: PASS / FAIL / BLOCKED / NOT RUN
.NET restore/format/build/test: PASS / FAIL / BLOCKED / NOT RUN
Windows: PASS / FAIL / BLOCKED / NOT RUN
Linux: PASS / FAIL / BLOCKED / NOT RUN
macOS: PASS / FAIL / BLOCKED / NOT RUN
Browser: PASS / FAIL / BLOCKED / NOT RUN
Android: PASS / FAIL / BLOCKED / NOT RUN
iOS: PASS / FAIL / BLOCKED / NOT RUN
Accessibility audit: PASS / FAIL / BLOCKED / NOT RUN
Responsive-layout audit: PASS / FAIL / BLOCKED / NOT RUN
Signing/store evidence: PASS / FAIL / BLOCKED / NOT RUN
```

Never convert `NOT RUN` or `BLOCKED` into PASS because source files/workflows merely exist. That evidence discipline coexists with the completed 2.8.03 product status.
