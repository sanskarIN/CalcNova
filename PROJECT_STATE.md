# CalcNova Project State

## Current Version

`0.1.0-dev`

## Current Branch

`main`

## Current Phase

Foundation + core calculation engines + first shared Avalonia desktop workspace. Persistence foundation is in progress.

## Master Technical Direction

- C# / .NET 10
- Avalonia UI 12.1.1
- Feature-first modular solution
- Pure C# calculation/domain projects independent of Avalonia UI
- Native calculation history via SQLite behind a repository abstraction
- Browser-compatible persistence remains a separate planned implementation
- Apache-2.0

The uploaded master prompt contains one contradictory sentence in section 4A saying not to make C#/.NET/Avalonia primary, while the surrounding final decisions and final technical baseline repeatedly require C# + .NET + Avalonia. The repository follows the repeated final baseline: C# + .NET + Avalonia UI.

## Stable Source Foundations Implemented

- Repository quality configuration:
  - `.editorconfig`
  - `.gitattributes`
  - `.gitignore`
  - `Directory.Build.props`
  - `Directory.Packages.props`
  - `global.json`
  - `CalcNova.slnx`
- Apache-2.0 license
- Core typed calculation errors
- Mixed numeric representation using `BigInteger`, `decimal`, and bounded `double` fallback
- Safe tokenizer and recursive-descent expression parser
- Right-associative exponentiation
- Unary operators and parentheses
- Standard arithmetic evaluator
- Scientific functions and constants
- Degrees/radians/gradians
- Programmer radix conversion for bases 2–36
- Programmer bounded word-size bitwise helpers
- Fixed offline unit conversion engine
- Initial SQLite history repository
- Initial standard/scientific Avalonia calculator workspace
- Desktop Avalonia entry point
- Core/programmer/converter/persistence test source projects

## In Progress

- Full calculation history integration into the app UI
- Settings/preferences architecture
- Memory operations
- Favorites/pinned history UI
- Additional desktop keyboard input routing
- Responsive/adaptive shell beyond the initial window
- Design-system controls
- Full platform heads
- CI/CD workflows
- Complete documentation set

## Not Yet Implemented

- Android platform head and packaging
- iOS platform head and packaging
- Browser/WebAssembly platform head and PWA shell
- Full macOS-specific packaging/notarization documentation validation
- Graphing module
- Statistics module
- Equation solver module
- Matrices/vectors module
- Currency provider abstraction/UI
- Full history export/import
- Onboarding
- Full Settings/About/Support UI
- Accessibility audit and UI automation suite
- Branding/icon/splash final assets
- Release artifacts

## Known Issues / Risks

1. **Build validation has not run in this execution environment.** The available environment does not contain the .NET SDK.
2. Avalonia XAML and platform package integration must be compiled in CI or a machine with the pinned SDK before being marked PASS.
3. `NumberValue` equality/hash semantics need an audit so numerically equal values of different internal kinds always satisfy the .NET hash contract.
4. Standard-calculator `%` UX currently maps to modulo in expression syntax; user-friendly contextual percentage behavior is still a separate task.
5. The current app shell is desktop-first and is not yet the final adaptive multi-mode navigation system.
6. SQLite is a native persistence implementation; browser persistence must not depend on it.

## Test Status

- Core test source: IMPLEMENTED — NOT RUN locally
- Programmer test source: IMPLEMENTED — NOT RUN locally
- Converter test source: IMPLEMENTED — NOT RUN locally
- Persistence test source: IMPLEMENTED — NOT RUN locally
- UI tests: NOT IMPLEMENTED
- Integration tests: PARTIAL SOURCE ONLY

Reason tests were not run: `.NET SDK unavailable in the active execution environment`.

## Build Status

- Core/solution: **NOT RUN — .NET SDK unavailable in active environment**
- Windows/Desktop: **NOT RUN — .NET SDK unavailable in active environment**
- Linux/Desktop: **NOT RUN — .NET SDK unavailable in active environment**
- macOS/Desktop: **NOT RUN — required platform environment unavailable**
- Android: **NOT RUN — platform head not yet implemented**
- iOS: **NOT RUN — platform head not yet implemented and Apple environment unavailable**
- Browser/WebAssembly: **NOT RUN — platform head not yet implemented**

## Last Verified Actions

Repository-side verification performed:

- Confirmed repository exists and `main` is writable.
- Confirmed initial commit author email is `sanskarin@outlook.in`.
- Reviewed newly created source files during implementation.
- Fixed an invalid exponent-marker API usage in `NumberValue.Parse` before continuing.
- Fixed programmer radix separator-only input before continuing and added regression tests.

Commands that require .NET were **not** falsely reported as passing.

## Next Exact Tasks

1. Audit and fix `NumberValue` equality/hash semantics.
2. Add build/test/format GitHub Actions workflows and let CI validate the current solution.
3. Fix any compile/analyzer/test failures reported by CI before adding more platform surface.
4. Add history/settings abstractions that do not force SQLite into browser builds.
5. Wire native history to the desktop app through composition-root services.
6. Add standard calculator memory model and tests.
7. Implement programmer and converter view models/views.
8. Build the adaptive navigation/design-system foundation.
9. Add Browser head and browser persistence.
10. Add Android and iOS heads with platform-specific validation.
11. Continue graphing/statistics/equations/matrices only after the baseline CI is green.

## Important Paths

- `src/CalcNova.Core/`
- `src/CalcNova.Scientific/`
- `src/CalcNova.Programmer/`
- `src/CalcNova.Converter/`
- `src/CalcNova.Persistence/`
- `src/CalcNova.App/`
- `src/CalcNova.Desktop/`
- `tests/`
- `what_changed.md`
- `docs/ROADMAP.md`

## Recent Important Commits

See `what_changed.md` and `git log` for the full atomic history. Important milestones include:

- `e7a66976` — feat(calc): implement deterministic expression evaluator
- `c988431a` — test(core): cover arithmetic precedence precision and domains
- `efc3aca6` — feat(programmer): add bounded word bitwise operations
- `fea67947` — fix(programmer): reject separator-only radix input safely
- `c62644c9` — feat(converter): implement offline fixed-unit conversion engine
- `7941e01e` — feat(ui): build adaptive standard and scientific calculator workspace
- `1f2c8dce` — feat(history): implement SQLite calculation history repository
- `dc575df2` — test(history): cover SQLite history lifecycle

## Continuation Rule

Before new development, read this file and `what_changed.md`, inspect current `main`, and continue the first incomplete task. Do not recreate completed files or reset the repository.
