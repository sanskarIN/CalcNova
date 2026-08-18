# CalcNova Roadmap

This roadmap tracks remaining work without promising fixed release dates. Completed foundation/features are documented in `docs/FEATURES.md`; this file focuses on what still needs engineering, validation, or release polish.

## Now

### Full baseline validation

- Run the current `CalcNova.slnx` through formatting, analyzers, build, and tests in GitHub Actions.
- Validate Desktop, Android, Browser/WebAssembly, and iOS-simulator heads independently.
- Fix every concrete build/XAML/analyzer/test failure before declaring a release candidate.
- Keep unavailable signing/device checks recorded as `NOT RUN`, never PASS.
- Extend regression/property coverage when validation discovers an edge case.

### Standard/scientific polish

- Finish cursor-aware calculator editing beyond ordinary `TextBox` editing where it improves usability without creating ambiguity.
- Add an in-app keyboard-shortcut reference dialog.
- Apply global precision/formatting preferences consistently to advanced modes where appropriate.
- Review localization-safe number display/parser interactions before adding localized decimal entry.
- Add optional auto-copy only if it remains non-intrusive.

### History and persistence polish

- Add date grouping in the History UI.
- Add multi-select delete.
- Add an explicit auto-cleanup policy/setting.
- Keep CSV export user-initiated and validate save-picker behavior on every supported target.
- Add storage-schema migration tests before the first schema-changing release.

### Accessibility and responsive review

- Test large text, screen-reader labels, keyboard-only use, focus order, high contrast, and reduced-motion behavior.
- Review compact phone, landscape phone, tablet/foldable, desktop, and Browser breakpoints.
- Replace any glyph-only control whose accessible purpose is unclear.
- Wire stored high-contrast/reduced-motion preferences into concrete visual/motion behavior where Avalonia/platform capabilities allow it.

## Next

### Programmer experience

- Add an interactive bit-toggle grid tied to the selected word size.
- Expose custom base 2–36 selection in the shared UI rather than only common bases.
- Add an optional Unicode/code-point helper after accessibility review.

### Converter experience

- Add favorites and recently used conversion pairs.
- Add direct copy actions and precision controls.
- Review and add fuel-economy, transfer-speed, and typography categories only where definitions are unambiguous.
- Select a replaceable live currency-rate provider only if licensing/terms and credential handling are suitable for an open-source client; never embed a private API key.

### Graphing experience

- Add multiple simultaneously visible expressions with identifiers/styles.
- Surface point trace/crosshair coordinates as an intentional interaction mode.
- Add a table-of-values workspace.
- Add tested numerical roots/intercepts, derivative, and integral helpers.
- Add PNG/image share/export after cross-platform storage/rendering validation.
- Evaluate polar/parametric graphs only after the Cartesian path is stable.

### Equations, matrices, and vectors

- Add simultaneous-equation UI backed by the matrix solver.
- Replace text-only matrix entry with a richer cell editor while retaining paste support.
- Add a dedicated vector workspace.
- Add copy/export helpers for advanced results.
- Consider higher-degree polynomial utilities only with clearly documented approximate behavior.

### Platform release readiness

- Android: install/smoke-test APK and AAB, orientation/tablet layouts, accessibility, Play Store metadata/privacy requirements, signing via secrets.
- iOS: simulator validation, then real-device/archive validation on a proper Apple environment; App Store signing remains external to source control.
- Windows: validate portable package and optional maintained MSIX path if adopted.
- Linux: validate runtime dependencies and desktop integration on representative distributions.
- macOS: validate `.app` bundle, icon metadata, signing/notarization flow when credentials exist.
- Browser/PWA: validate install/offline/update behavior, hosted base paths, keyboard/accessibility, and major supported browsers.

## Later

### Product polish

- Optional skippable onboarding.
- Searchable command/function palette.
- Open-source license/acknowledgement browser in the app.
- Repository screenshots for all major modes/themes/form factors.
- Final GitHub social preview and release artwork generated from project-owned branding.
- Reviewed localization packs and locale-specific UI copy.
- Accessibility presets after real assistive-technology testing.

### Power-user features

- saved formulas;
- custom constants;
- engineering notation;
- exact fraction/rational presentation if numeric strategy is extended safely;
- recurring-decimal visualization;
- configurable shortcuts;
- custom keypad layouts;
- pinned converter pairs;
- desktop multi-window support;
- OS widgets/quick actions where maintainable.

## Research

These items are experiments, not release promises:

- exact rational arithmetic integrated with the existing numeric layer;
- richer complex-number workflows;
- constrained local natural-language calculation patterns;
- symbolic manipulation only where correctness can be demonstrated;
- reusable high-performance numeric backends if profiling shows a real bottleneck;
- advanced graph sampling algorithms after correctness/performance profiling.

## Release gates

A milestone is complete only when:

1. implementation is present and no release-critical placeholder remains;
2. relevant tests exist;
3. formatting/analyzers pass on project-owned code;
4. required CI jobs conclude successfully;
5. target builds are attempted in the appropriate environment;
6. accessibility/manual smoke checks are recorded where automation is insufficient;
7. documentation, `what_changed.md`, `PROJECT_STATE.md`, and `CHANGELOG.md` match the actual state;
8. no secrets/signing material are tracked;
9. remaining limitations are disclosed.
