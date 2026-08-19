# CalcNova Release Source Checkpoint — 2026-08-19

This document is not a release approval. It records the newest source-level capabilities and the evidence still required before a release tag may be considered validated.

## New release-relevant source behavior

### Calculator input

- selection-aware keypad editing remains in place;
- ordinary physical/numpad mappings are extended with safe punctuation;
- exact-Shift common operators are supported outside text editors;
- Control/Alt shortcut combinations are not captured by the Shift mapper;
- calculator Unicode glyphs are normalized only through the non-editor text-symbol path.

### Converter

- every fixed-unit category has a deterministic useful default pair;
- restored recent/favorite pairs override defaults when explicitly selected;
- shared UI explains local storage of converter precision/recents/favorites;
- fixed unit conversion remains offline.

### Graphing

- shared Graph mode contains a real interactive plot surface;
- single and multi-series mode are explicitly synchronized;
- multi-series identity uses deterministic non-color line patterns plus text legend;
- visible Pan/Zoom/Fit/Reset actions mirror keyboard viewport behavior;
- extreme-bound numerical analysis has additional finite/overflow protections.

### Localization

- English/Hindi semantic catalogs cover the current key inventory;
- live UI migration now includes shell headers, onboarding, Calculator, major mode headings, Currency, History, Settings accessibility options, About/footer, converter privacy notice, and Graph viewport controls.

### Source/repository gates

Focused source validators now cover, among other existing checks:

- converter defaults;
- converter preference notice;
- graph surface;
- graph multi-series presentation;
- numerical-analysis safety;
- expanded keyboard/glyph input;
- localization live surfaces;
- incomplete implementation markers.

An extended source-preflight workflow runs the integrated source preflight plus the incomplete-code audit for broad repository changes.

## Required release evidence not implied by this document

Before a release candidate is approved, record observed results for the exact candidate commit/tag:

```text
Integrated source preflight: PASS / FAIL / NOT RUN
Restore/format/build/test: PASS / FAIL / NOT RUN
Avalonia headless tests: PASS / FAIL / NOT RUN
Windows: PASS / FAIL / NOT RUN
Linux: PASS / FAIL / NOT RUN
macOS: PASS / FAIL / NOT RUN
Browser: PASS / FAIL / NOT RUN
Android: PASS / FAIL / NOT RUN
iOS: PASS / FAIL / NOT RUN
Accessibility runtime audit: PASS / FAIL / BLOCKED / NOT RUN
Responsive-layout runtime audit: PASS / FAIL / BLOCKED / NOT RUN
```

Do not infer a PASS from source presence, workflow presence, or a passing different commit.

## Runtime priorities

The highest-value remaining release work is execution-bound:

1. observe build/test/headless results for the newest source batch;
2. run compact/medium/expanded and mobile orientation smoke tests;
3. verify screen-reader and focus behavior on actual supported targets;
4. verify clipboard and persistence paths per target;
5. validate Android signing/store flow with external secure material;
6. validate iOS signing/provisioning/archive/distribution on supported Apple tooling;
7. validate native Windows/macOS packaging/runtime behavior;
8. update release screenshots/assets only from validated builds.
