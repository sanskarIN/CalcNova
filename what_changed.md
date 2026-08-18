# What Changed

## 2026-08-18

### Added

- Apache-2.0 license.
- Repository formatting, line-ending, ignore, analyzer, nullable, deterministic-build, and central-package configuration.
- `.NET 10` SDK baseline through `global.json`.
- Modular `CalcNova.slnx` solution.
- Core calculation engine with typed errors and workload limits.
- Numeric value layer using arbitrary-precision integers, decimal arithmetic where possible, and bounded floating-point fallback for transcendental functions.
- Safe expression tokenizer and recursive-descent parser.
- Right-associative exponentiation and unary operator handling.
- Scientific functions including roots, logs, trigonometry, inverse trigonometry, hyperbolic functions, rounding, factorial, GCD, LCM, combinations, and permutations.
- Degree, radian, and gradian angle modes.
- Programmer radix conversion for bases 2 through 36.
- Programmer bounded word-size bitwise helpers and two's-complement interpretation.
- Offline fixed-unit conversion engine and searchable unit catalog.
- Initial Avalonia shared app with standard/scientific calculator workspace.
- Desktop Avalonia application entry point and basic Enter/Escape/Backspace keyboard actions.
- Native SQLite calculation-history repository behind `ICalculationHistoryRepository`.
- Core, programmer, converter, and SQLite persistence test source projects.
- `PROJECT_STATE.md` for deterministic multi-chat continuation.

### Changed

- Central package versions were expanded to cover native persistence and planned Android/iOS/Browser Avalonia platform packages.
- Solution was expanded to include persistence and persistence tests.

### Fixed

- Fixed number parsing code that used an invalid string API overload for detecting exponent markers.
- Fixed programmer radix parsing so separator-only or sign-only input is rejected without indexing an empty span.

### Tests Added

- Arithmetic and precedence cases:
  - `1 + 1`
  - `2 + 3 * 4`
  - `(2 + 3) * 4`
  - `2 ^ 3 ^ 2`
  - unary-minus precedence
  - negative exponents
- Decimal and large-integer cases:
  - `0.1 + 0.2`
  - `999999999999999999 + 1`
  - scientific notation
- Domain and typed-error cases:
  - divide by zero
  - square root of a negative real value
  - input length limit
- Scientific cases:
  - degree-mode trigonometry
  - factorial
  - GCD / LCM
  - combinations / permutations
- Programmer cases:
  - bases 2, 8, 10, 16, 36
  - large integer round trips
  - signed/two's-complement boundaries
  - fixed-width NOT and shifts
  - invalid radix input regressions
- Converter cases:
  - exact/known length, mass, data, and energy identities
  - Celsius/Fahrenheit/Kelvin affine conversions
  - category mismatch rejection
  - unit search
- SQLite history cases:
  - initialization
  - add/get
  - favorite update
  - search
  - delete
  - clear

### Documentation

- Added project continuation state and exact next-task list.
- Recorded unvalidated platform/build status explicitly instead of marking unavailable checks as PASS.

### Git

Atomic commits created during this work include:

- `3c9a1773` — chore: add .NET and IDE ignore rules
- `f7b00ca0` — chore: define repository line ending policy
- `6bca621e` — chore: configure C# formatting and analyzer defaults
- `45b27069` — build: pin .NET 10 SDK feature band
- `8fc21a1e` — build: enable nullable analysis and deterministic builds
- `a9915cae` — build: centralize stable package versions
- `e99411cf` — chore: add modular CalcNova solution
- `eed81e36` — chore(core): create calculation engine project
- `777a1364` — chore(scientific): create scientific feature project
- `97180487` — chore(programmer): create programmer feature project
- `6e64dad5` — chore(converter): create conversion feature project
- `a30bfdd8` — chore(app): create shared Avalonia application project
- `4da5a85d` — chore(desktop): create desktop host project
- `0c5007d2` — test(core): create core test project
- `3d06fd73` — test(programmer): create programmer test project
- `7b2f9596` — test(converter): create converter test project
- `f8290dbc` — feat(core): define calculation error codes
- `e42a8199` — feat(core): add typed calculation exception
- `8cadfb96` — feat(core): add angle unit model
- `829b9c7c` — feat(core): add evaluator options and safety limits
- `542cdec2` — feat(core): model successful and failed evaluations
- `8e5b704c` — feat(core): add mixed exact and floating numeric value type
- `594b48b8` — feat(parser): define expression token kinds
- `8f4fe375` — feat(parser): add immutable expression token model
- `ad78e78b` — feat(parser): implement safe expression tokenizer
- `70bc400d` — feat(parser): add expression syntax tree
- `b37ee948` — feat(parser): implement precedence-aware recursive descent parser
- `e7a66976` — feat(calc): implement deterministic expression evaluator
- `db83eb5b` — fix(core): use valid exponent marker checks
- `c988431a` — test(core): cover arithmetic precedence precision and domains
- `9297cf1e` — feat(scientific): add scientific calculator facade
- `540a9258` — feat(scientific): publish supported scientific function catalog
- `af1a8f47` — feat(programmer): add arbitrary precision radix conversion
- `efc3aca6` — feat(programmer): add bounded word bitwise operations
- `4b57bcec` — test(programmer): cover radix and bitwise boundaries
- `9598a806` — feat(converter): define fixed unit categories
- `c06d36fa` — feat(converter): model affine fixed-unit definitions
- `b3ac5e0a` — feat(converter): add fixed physical unit catalog
- `c62644c9` — feat(converter): implement offline fixed-unit conversion engine
- `7d07c934` — test(converter): cover exact identities and affine temperature conversion
- `b7822a48` — feat(ui): add shared Avalonia application theme
- `4bd15af1` — feat(ui): add lightweight MVVM relay command
- `e1450913` — feat(ui): connect calculator view model to core evaluator
- `07d26e6e` — feat(ui): initialize shared application lifetime and view model
- `7941e01e` — feat(ui): build adaptive standard and scientific calculator workspace
- `eebea219` — feat(ui): add desktop keyboard actions
- `7829f706` — feat(desktop): add Avalonia desktop entry point
- `fea67947` — fix(programmer): reject separator-only radix input safely
- `b1f720bb` — test(programmer): add regression coverage for invalid radix input
- `906c466a` — build: add persistence and platform package baselines
- `a119d51d` — chore(persistence): create native persistence project
- `7f36f152` — feat(history): add immutable calculation history model
- `8b630020` — feat(history): define replaceable history repository contract
- `1f2c8dce` — feat(history): implement SQLite calculation history repository
- `9a7455be` — chore: include persistence projects in solution
- `89d45df2` — test(history): create persistence test project
- `dc575df2` — test(history): cover SQLite history lifecycle
- `8183858d` — docs: add resumable project state

### Validation

- GitHub repository write access — PASS.
- Requested Git author email `sanskarin@outlook.in` — VERIFIED on the initial repository commit.
- `dotnet restore` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet format --verify-no-changes` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet build --configuration Release --no-restore` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- `dotnet test --configuration Release --no-build` — **NOT RUN — .NET SDK unavailable in active execution environment**.
- Android build — **NOT RUN — platform head not yet implemented**.
- iOS build — **NOT RUN — platform head not yet implemented / Apple build environment unavailable**.
- Browser build — **NOT RUN — platform head not yet implemented**.
- Desktop build — **NOT RUN — .NET SDK unavailable in active execution environment**.

### Remaining

See `PROJECT_STATE.md` and `docs/ROADMAP.md`. Highest-priority next work is CI validation of the current C# solution, fixing any compile/analyzer/test failures, then history/settings composition, multi-mode UI, platform heads, graphing, advanced math tools, branding, and release engineering.
