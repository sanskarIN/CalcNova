# CalcNova Roadmap

This roadmap tracks planned work without promising fixed release dates. Implementation order may change when correctness, accessibility, platform constraints, or CI findings require it.

## Now

### Baseline correctness and CI

- Keep the calculation engine independent of UI.
- Run the current solution through GitHub Actions on Linux, Windows, and macOS.
- Fix compile, analyzer, formatter, and test failures before expanding platform complexity.
- Extend parser regression coverage and numeric boundary testing.
- Add fuzz/property-style invariant tests where practical.

### Standard calculator

- Complete expression editing behavior.
- Add repeated-equals semantics with explicit tests.
- Add calculator-style percentage semantics without changing the expression-language modulo operator.
- Add classic memory operations: MC, MR, M+, M−, MS.
- Add copy/paste services with sanitized expression import.
- Add local history integration and favorites.

### Application architecture

- Create adaptive multi-mode navigation.
- Add settings/preferences abstractions.
- Add a platform composition root.
- Keep native SQLite out of Browser/WebAssembly dependency paths.
- Add design-system controls and reusable view primitives.

## Next

### Scientific experience

- Complete scientific keypad groups.
- Add visible angle-mode state in every relevant layout.
- Add additional scientific boundary tests.
- Define and test factorial/combinatorics workload behavior.

### Converter experience

- Add searchable category/unit UI.
- Add favorites and recent conversion pairs.
- Add swap/copy/precision controls.
- Keep fixed physical conversions fully offline.
- Add optional replaceable currency-rate provider architecture without embedded secrets.

### Programmer experience

- Add binary/octal/decimal/hex synchronized views.
- Add base 2–36 custom radix workflow.
- Add word-size selector.
- Add signed/unsigned selector.
- Add bit toggle grid.
- Add code-point helper after accessibility review.

### Platform targets

- Browser/WebAssembly head and browser storage implementation.
- Android head, adaptive icon, splash behavior, and package configuration.
- iOS head and Apple-specific packaging documentation.
- Desktop packaging guidance for Windows, Linux, and macOS.

## Later

### Graphing

- Plot `y = f(x)` using the shared expression engine where safe.
- Multiple expressions.
- Pan/zoom/reset.
- Axis/grid labels.
- Discontinuity segmentation.
- Adaptive sampling and workload budgets.
- Trace/table-of-values.
- Tested numerical roots, derivatives, and integrals if included.

### Statistics

- Editable datasets.
- Count, sum, mean, median, mode, min, max, range.
- Population/sample variance and standard deviation.
- Quartiles and percentiles.
- Optional covariance, correlation, and simple regression.

### Equations

- Linear equations in one variable.
- Simultaneous linear equations.
- Quadratic equations.
- Numeric root finding with documented convergence rules.
- Clear exact/approximate result labeling.

### Matrices and vectors

- Matrix editor.
- Add/subtract/multiply/scalar operations.
- Transpose, determinant, rank, inverse where defined.
- Linear-system solving.
- Vector magnitude/dot/cross product for supported dimensions.

### Product polish

- Original CalcNova logo and complete icon set.
- Splash assets.
- Optional onboarding.
- About/Support/Open-source licenses UI.
- Repository screenshots and social preview.
- Localization packs after review.
- Accessibility presets.

## Research

These ideas are not release promises:

- exact rational representation;
- recurring-decimal visualization;
- complex-number mode;
- engineering notation;
- local deterministic natural-language calculation patterns;
- saved formulas and user constants;
- richer graph analysis;
- OS widgets and quick actions;
- reusable experimental high-performance numeric backends if profiling demonstrates a need.

## Release gates

A milestone is not complete until its implementation, tests, documentation, and supported validation checks are complete. Unavailable checks must be recorded as `NOT RUN`, not PASS.
