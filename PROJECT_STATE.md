# CalcNova Project State

## Current Version

`0.1.0-dev` — unreleased development baseline.

No stable semantic-version release has been declared yet. `CHANGELOG.md` keeps `0.1.0` as a planned validated milestone rather than pretending the current source is already a release.

## Current Branch

`ci/full-baseline-validation`

Validation pull request: **#6 — `ci: validate full cross-platform CalcNova baseline`**.

Base branch: `main`.

## Current Phase

**Full baseline validation / pre-0.1.0 integration.**

Major domain engines, shared modular Avalonia UI, native/browser persistence, platform heads, branding, packaging helpers, documentation, automated test source, and CI workflows are implemented. The current blocking work is to let the full GitHub Actions validation set conclude and fix any concrete formatter/compiler/XAML/analyzer/test/platform failures before declaring a release milestone.

## Master Technical Direction

- Primary language: C#
- Runtime: .NET 10
- UI: Avalonia UI
- UI markup: Avalonia XAML
- Architecture: feature-first modular solution + MVVM
- Build: `dotnet` CLI / MSBuild
- Packages: NuGet with central package management
- Native history: SQLite behind platform-neutral repository contracts
- Native settings/currency cache: local JSON implementations
- Browser persistence: `localStorage` implementations behind the same application contracts
- License: Apache-2.0
- Privacy baseline: local-first; no ads SDK, behavioral analytics SDK, or embedded currency API key

The uploaded master prompt contains one contradictory sentence in section 4A saying not to make C#/.NET/Avalonia primary, while its surrounding final technical decisions repeatedly require C# + .NET + Avalonia. The repository follows the repeated final baseline and project mission: **C# + .NET + Avalonia UI**.

## Implemented Source — Core Calculator

- Safe project-owned tokenizer, recursive-descent parser, expression syntax tree, and evaluator.
- No arbitrary source-code `eval` path.
- Numbers, decimal numbers, scientific notation, parentheses, unary plus/minus, standard binary operators, right-associative exponentiation, functions, and constants.
- Typed calculation errors for syntax, domain, divide-by-zero, overflow, invalid arguments, unsupported functions, input limits, and workload limits.
- Numeric representation using `BigInteger`, `decimal`, and finite `double` fallback where transcendental BCL operations require it.
- Cross-kind numeric equality/hash contract fixes.
- Compiled expressions with scoped variables for graphing.
- Configurable angle modes: degrees, radians, gradians.
- Standard arithmetic and scientific functions.
- Contextual calculator-style percentage while explicit `%` expression syntax remains modulo/remainder.
- Repeated-equals calculation session behavior.
- Positive/negative toggle based on evaluated value.
- Classic memory model: MC, MR, MS, M+, M−.
- Backspace, clear, result reuse, keyboard/numpad routing, F9 sign toggle, and clipboard copy/paste.
- Canonical parser-safe result state separated from locale-aware formatted display result.
- Display precision/grouping preferences, including locale group sizes such as Indian digit grouping.

## Implemented Source — Scientific

- square/cube/arbitrary power;
- square/cube/nth roots;
- reciprocal and absolute value;
- natural/base-10/base-2/arbitrary-base logs;
- exponential function;
- trig/inverse trig;
- hyperbolic/inverse hyperbolic;
- floor/ceiling/truncate/round/sign;
- min/max;
- factorial;
- GCD/LCM;
- combinations/permutations;
- π, e, τ;
- workload guards for expensive integer operations.

## Implemented Source — Programmer

- bases 2–36 parsing/formatting;
- arbitrary-precision integer conversion;
- binary/octal/decimal/hex outputs;
- configurable word sizes;
- signed/unsigned two's-complement interpretation;
- AND/OR/XOR/NOT;
- left shift;
- logical/arithmetic right shift;
- fixed-width bit-pattern display;
- invalid-digit and separator/sign-only input protection.

Remaining programmer UI polish includes the interactive bit-toggle grid and richer custom-base selection.

## Implemented Source — Unit Converter

Offline fixed-unit definitions and category-safe conversion cover:

- length;
- area;
- volume;
- mass;
- speed;
- temperature;
- time;
- data/storage;
- frequency;
- pressure;
- energy;
- power;
- force;
- angle.

The shared UI supports category/unit selection, source/target swapping, conversion, and validation without network access.

## Implemented Source — Statistics

- count;
- compensated sum;
- mean;
- median;
- mode where meaningful;
- minimum/maximum/range;
- population/sample variance;
- population/sample standard deviation;
- quartiles;
- percentiles;
- sorted data model;
- editable pasted dataset parsing;
- shared Statistics mode.

## Implemented Source — Equations

- linear equations with unique/no/infinite solution states;
- quadratic equations;
- repeated roots;
- complex roots;
- degenerate quadratic-to-linear behavior;
- bounded numerical bisection root finding;
- shared equation UI.

## Implemented Source — Matrices and Vectors

- matrix model and dimension validation;
- addition/subtraction;
- matrix multiplication;
- scalar multiplication;
- transpose;
- determinant with partial pivoting;
- inverse when defined;
- rank;
- linear-system solving;
- vector magnitude;
- dot product;
- supported 3D cross product;
- shared matrix UI.

Remaining UI polish includes a dedicated vector workspace and richer matrix cell editor.

## Implemented Source — Graphing

- safe `y = f(x)` sampling through the shared evaluator;
- one compiled expression per sampled function;
- bounded sample counts;
- invalid-domain/discontinuity segmentation;
- jump splitting safeguards;
- automatic viewport calculations;
- deterministic SVG graph exporter;
- Avalonia `GraphPlotControl`;
- axes/grid rendering;
- pointer drag panning;
- wheel zoom;
- coordinate text;
- double-tap / explicit fit-to-data;
- reset viewport;
- modular shared Graph UI.

Remaining advanced graph work includes multiple simultaneous expressions, point-trace workflow, roots/intercepts/extrema helpers, numerical derivative/integral, and PNG/share export.

## Implemented Source — Date and Duration

- `DateOnly` signed difference;
- absolute day count;
- whole weeks + remaining days;
- Monday–Friday business-day calculation;
- add/subtract years, months, weeks, days in documented order;
- leap-year/month-end behavior via .NET calendar semantics;
- fixed-duration conversion without silently assuming month/year lengths;
- strict `yyyy-MM-dd` shared UI parsing;
- modular shared Date/Duration UI.

No timezone is silently introduced into date-only calculations.

## Implemented Source — Currency Architecture

- optional `ICurrencyRateProvider` abstraction;
- `ICurrencyRateCache` abstraction;
- normalized three-letter currency codes;
- timestamped validated rate snapshots;
- freshness/staleness detection;
- provider refresh;
- cached fallback after provider failure;
- native JSON currency-rate cache;
- Browser `localStorage` currency-rate cache;
- mocked provider/cache tests;
- modular Currency UI exposing amount, ISO codes, conversion, refresh, source, rate timestamp, freshness/staleness state.

**Intentional limitation:** the open-source client does not hard-code a live exchange-rate provider/API key. A future provider may be injected only when its terms, maintenance, and credential model are suitable. This is not treated as a hidden failure.

## Implemented Source — History

- chronological local calculation history;
- local-only persistence by default;
- search;
- favorite/un-favorite;
- selected-entry delete;
- clear-all with explicit confirm/cancel flow;
- configurable history limit;
- optional history enable/disable;
- native SQLite repository;
- Browser `localStorage` repository;
- user-initiated CSV export through Avalonia's cross-platform save picker;
- deterministic CSV quoting/UTC timestamp formatter;
- modular shared History UI.

Remaining history polish includes date grouping, multi-select delete, and configurable auto-cleanup.

## Implemented Source — Settings

- Light / Dark / System theme;
- angle unit;
- decimal precision validation;
- grouping-separator preference;
- history enable/limit;
- haptics preference flag;
- reduced-motion preference;
- high-contrast preference;
- reset defaults;
- atomic native JSON persistence;
- Browser `localStorage` persistence;
- saved angle/formatting settings applied into calculator state;
- locale-aware `DisplayResult` formatting separated from canonical `Result`;
- modular shared Settings UI.

The haptics/high-contrast/reduced-motion flags are persisted but still need additional target-specific behavioral/visual wiring before they are considered fully implemented platform features.

## Implemented Source — About / Support

- project/tagline/open-source statement;
- repository link;
- GitHub profile link;
- Buy Me a Coffee link;
- business contacts;
- support contact;
- safe external-link abstraction;
- Desktop/Android/iOS/Browser implementations;
- modular shared About UI.

Support is non-blocking and never required to use calculator functionality.

## Shared UI Architecture

The previous monolithic shared XAML has been refactored into focused mode views under:

`src/CalcNova.App/Views/Modes/`

Current modular views:

- `CalculatorModeView`
- `ProgrammerModeView`
- `ConverterModeView`
- `StatisticsModeView`
- `EquationsModeView`
- `MatricesModeView`
- `GraphingModeView`
- `DateTimeModeView`
- `CurrencyModeView`
- `HistoryModeView`
- `SettingsModeView`
- `AboutModeView`

`MainView` is now a small tab/navigation composition shell. Desktop `MainWindow` hosts this same shared `MainView`. Android, iOS, and Browser use the shared single-view application lifetime.

## Implemented Platform Heads

### Desktop

`src/CalcNova.Desktop/`

- shared Avalonia Desktop startup;
- Windows/Linux/macOS source path;
- native local history/settings/currency cache composition;
- safe external-link service;
- keyboard/numpad/clipboard support through shared UI;
- deterministic project-owned icon generation;
- Windows/Linux/macOS packaging metadata/helper scripts.

### Android

`src/CalcNova.Android/`

- Avalonia Android startup;
- application identity `in.sanskar.calcnova`;
- native local storage composition;
- safe external-link service;
- adaptive launcher icon/splash resources;
- permission-minimal manifest baseline;
- Android workload CI;
- release signing path through secrets instead of committed keystore/passwords.

### iOS

`src/CalcNova.iOS/`

- Avalonia iOS startup;
- native local storage composition;
- safe external-link service;
- launch screen metadata;
- generated AppIcon asset catalog from repository-owned artwork;
- iOS simulator CI workflow.

Real device/archive/App Store signing validation remains environment-dependent and has not been falsely marked PASS.

### Browser/WebAssembly + PWA

`src/CalcNova.Browser/`

- Avalonia Browser startup;
- Browser `localStorage` history/settings/currency cache;
- safe external-link JavaScript bridge;
- PWA manifest;
- service worker/offline app-shell baseline;
- Browser icons/social metadata;
- Browser publish CI.

## Branding / Packaging Implemented

- master CalcNova SVG logo;
- original CalcNova icon source;
- optional support badge;
- social-preview artwork;
- Android adaptive icon/splash resources;
- Browser/PWA artwork;
- deterministic cross-platform icon/raster generator;
- asset ownership/license documentation;
- Windows packaging template/helper;
- Linux `.desktop`/AppStream/package helper;
- macOS bundle metadata/package helper;
- release workflow foundation with checksums and secret-based signing inputs.

## Test Source Status

Test projects/source are implemented for:

- Core
- Programmer
- Converter
- Statistics
- Equations
- Matrices
- Graphing
- Date/Duration
- Currency
- Persistence
- Application/MVVM

Important regression coverage includes:

- parser precedence/associativity;
- decimal and large-integer arithmetic;
- numeric equality/hash semantics;
- scientific domains/angle modes;
- programmer radix/two's-complement edge cases;
- converter identities;
- statistics formulas;
- linear/quadratic/bisection behavior;
- matrix singularity/rank/system solving;
- graph discontinuities/workload/SVG export;
- Date/Duration leap-year/reversed-range/month-end behavior;
- currency refresh/cache/stale fallback;
- SQLite/settings/currency-cache persistence;
- calculator repeated equals, percentage, memory, sign toggle;
- Date/Duration and Currency view models;
- history clear confirmation and CSV formatting;
- settings propagation;
- result formatting/grouping, including Indian grouping.

## Validation Status

### Active execution environment

- `dotnet restore` — **NOT RUN — .NET SDK unavailable in active execution environment**
- `dotnet format --verify-no-changes` — **NOT RUN — .NET SDK unavailable in active execution environment**
- `dotnet build` — **NOT RUN — .NET SDK unavailable in active execution environment**
- `dotnet test` — **NOT RUN — .NET SDK unavailable in active execution environment**

No local PASS has been fabricated.

### GitHub Actions validation

A dedicated validation branch/PR exists to exercise the full baseline. At the latest confirmed workflow query during this development segment, the following PR-triggered workflows were **QUEUED** rather than PASS/FAIL:

- Repository Validation
- Code Coverage
- Security Audit
- Formatting
- Build and Test
- Documentation Check

Desktop, Android, Browser, and iOS-simulator workflows were also triggered through shared-source changes during validation work. Their final successful/failed conclusion has not yet been observed through the connector, so they must not be described as PASS.

If a later GitHub Actions conclusion differs, update this file before a release/tag.

## Build Status by Target

- Core solution: **QUEUED in GitHub Actions; NOT RUN locally**
- Windows Desktop: **source implemented; target validation pending/queued**
- Linux Desktop: **source implemented; target validation pending/queued**
- macOS Desktop: **source implemented; target validation pending/queued; signing/notarization NOT RUN**
- Android: **source implemented; target validation pending/queued; signed store artifact NOT RUN**
- iOS Simulator: **source implemented; target validation pending/queued**
- iOS device/archive/App Store: **NOT RUN — Apple signing/device environment required**
- Browser/WebAssembly/PWA: **source implemented; publish validation pending/queued; manual install/offline browser matrix pending**

## Known Issues / Remaining Release Risks

1. Full formatter/compiler/analyzer/test conclusions have not yet been observed for the current validation branch.
2. Manual accessibility review with real assistive technologies remains pending.
3. Compact phone, landscape, tablet/foldable, desktop-resize, high-DPI, and large-text smoke tests remain pending.
4. iOS real-device/archive signing and macOS notarization remain NOT RUN because they require Apple credentials/platform tooling outside this environment.
5. Android signed store artifact installation/Play Store readiness checks remain pending.
6. Browser/PWA supported-browser install/offline/cache-update behavior still needs manual hosted testing.
7. No live currency provider is configured by design; only provider/cache architecture and local cache behavior are implemented.
8. High-contrast, reduced-motion, and haptics settings need concrete target behavior beyond persistence.
9. Programmer bit-toggle grid and several advanced graph/power-user features remain roadmap items rather than release-critical hidden placeholders.
10. UI/headless visual/accessibility automation coverage should be expanded before a stable public release.

Repository index searches performed during this segment returned no `TODO`, `FIXME`, `HACK`, or `placeholder` markers in indexed source. This does not replace build/test/manual validation.

## Current CI / Repository Workflows

Intended validation workflows include:

- formatting;
- build/test matrix;
- code coverage;
- repository validation;
- documentation checks;
- dependency/security audit;
- Desktop build;
- Android build;
- Browser publish;
- iOS simulator build;
- release packaging.

Legacy GitHub template workflows targeting .NET 8 and placeholder WPF/WAP packaging were removed from the validation branch.

## Last Verified Repository Actions

Repository-side source/API review has verified that:

- the repository is writable;
- the master branch remains `main`;
- a full validation branch/PR is open;
- Date/Duration and Currency modules are included in solution graphs;
- Desktop/Android/iOS/Browser composition uses the current `AppDependencies` contract;
- Browser/native currency caches implement the shared cache contract;
- Desktop reuses the shared `MainView`;
- shared UI is modularized into focused mode views;
- calculator touch/keyboard bindings correspond to implemented commands;
- History clear-all has explicit confirmation;
- History CSV export is user-initiated;
- result precision/grouping settings affect presentation while canonical results remain invariant;
- target CI path filters watch shared `src/**` changes;
- README/docs/changelog have been refreshed against actual source.

## Next Exact Tasks

1. Let the current PR-triggered GitHub Actions runs conclude.
2. Inspect and fix every concrete formatter/compiler/XAML/analyzer/test/platform failure reported by CI.
3. Re-run validation until required automated jobs are green.
4. Add/expand Avalonia headless/UI tests for modular view creation, mode switching, history confirmation, graph custom-control rendering, theme switching, focus, and accessibility semantics.
5. Perform manual accessibility and responsive-form-factor smoke tests on representative platforms.
6. Validate Desktop packages on Windows/Linux/macOS; keep macOS signing/notarization status separate.
7. Validate Android APK/AAB installation and store-signing flow using external secrets.
8. Validate iOS simulator, then real-device/archive signing on an appropriate Apple environment.
9. Validate Browser PWA install/offline/update behavior across supported browsers/hosting paths.
10. Wire persisted high-contrast/reduced-motion/haptics preferences into actual target behavior.
11. Complete remaining non-release-blocking power-user roadmap items only after baseline stability.
12. Update `what_changed.md`, this file, and `CHANGELOG.md` with actual CI/manual validation results before tagging `v0.1.0`.

## Important Paths

- `src/CalcNova.Core/`
- `src/CalcNova.Programmer/`
- `src/CalcNova.Converter/`
- `src/CalcNova.Statistics/`
- `src/CalcNova.Equations/`
- `src/CalcNova.Matrices/`
- `src/CalcNova.Graphing/`
- `src/CalcNova.DateTime/`
- `src/CalcNova.Currency/`
- `src/CalcNova.Platform/`
- `src/CalcNova.Persistence/`
- `src/CalcNova.App/`
- `src/CalcNova.App/Views/Modes/`
- `src/CalcNova.Desktop/`
- `src/CalcNova.Android/`
- `src/CalcNova.iOS/`
- `src/CalcNova.Browser/`
- `tests/`
- `.github/workflows/`
- `assets/`
- `packaging/`
- `tools/scripts/`
- `docs/`
- `what_changed.md`
- `CHANGELOG.md`

## Recent Important Commits

The repository contains a large number of small atomic commits. Recent important branch milestones include:

- `974baf91` — test(settings): verify calculator formatting preferences propagate
- `fb876dab` — docs(changelog): record current unreleased implementation baseline
- `711c7cd1` — docs(build): document implemented target build and packaging paths

Use `git log` / GitHub history for the complete atomic commit sequence; do not manufacture missing commit hashes.

## Continuation Rule

For the next work segment:

1. read `PROJECT_STATE.md` and `what_changed.md` first;
2. inspect the current branch/PR and latest workflow conclusions;
3. do not recreate completed domain modules/platform heads/UI refactors;
4. fix validation failures before adding unrelated feature count;
5. keep status factual (`PASS`, `FAIL`, `QUEUED/IN PROGRESS`, or `NOT RUN`);
6. update both state logs before ending the segment.
