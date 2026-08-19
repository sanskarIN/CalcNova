# Bivariate Statistics

CalcNova Statistics mode supports bounded paired X/Y analysis for covariance, Pearson correlation, ordinary least-squares linear regression, and regression prediction.

## Input model

Enter one X dataset and one Y dataset. Both datasets must contain the same number of finite values.

The shared dataset parser accepts invariant-culture numbers separated by:

- commas;
- semicolons;
- spaces;
- tabs;
- LF, CRLF, or CR line boundaries.

The parser rejects non-finite values such as `NaN` and infinities.

## Workload bounds

Statistics input is intentionally bounded before numerical analysis:

- maximum values per dataset: **100,000**;
- maximum input text per dataset: **2,000,000 characters**;
- maximum bivariate value pairs: **100,000**.

The text-length guard runs before token splitting so obviously oversized inputs are rejected before a large token array is created.

## Numerical method

`BivariateStatisticsCalculator` processes X/Y enumerators in lockstep instead of copying both sequences into new arrays.

The running state uses a Welford-style update for:

- mean X;
- mean Y;
- X sum of squared deviations;
- Y sum of squared deviations;
- X/Y co-moment.

This supports covariance, correlation, and regression without a separate second pass over the datasets.

Intermediate running state is required to remain finite. If the requested data exceeds the supported floating-point range, CalcNova reports a deterministic overflow error instead of silently returning an infinite result.

## Reported values

For a valid paired dataset CalcNova reports:

- pair count;
- mean X;
- mean Y;
- population covariance;
- sample covariance when at least two pairs exist;
- Pearson correlation coefficient `r` when both datasets have non-zero variance;
- regression slope;
- regression intercept;
- coefficient of determination `R²` when correlation is defined.

The linear model is:

`ŷ = slope × x + intercept`

## Degenerate datasets

Some mathematically valid datasets do not define every statistic.

### Constant X

If every X value is identical, X has zero variance. Linear regression and Pearson correlation are undefined because a unique slope cannot be calculated.

CalcNova reports those values as `N/A`, and prediction is unavailable.

### Constant Y

If X varies but every Y value is identical:

- regression slope is `0`;
- regression intercept is the constant Y value;
- prediction returns that same Y value;
- Pearson correlation and `R²` are reported as `N/A` because Y variance is zero.

### Single pair

With one X/Y pair:

- population covariance is `0`;
- sample covariance is undefined;
- correlation and regression are undefined.

## Prediction

After a successful paired analysis, enter a finite prediction X and choose **Predict Y**.

The prediction uses the most recently successful regression coefficients. If a later paired analysis fails, CalcNova clears the stored regression state so a stale model cannot be used accidentally.

Prediction rejects non-finite X values and non-finite predicted results.

## Clipboard behavior

**Copy paired summary** copies the formatted bivariate summary through CalcNova's existing platform clipboard abstraction. Clipboard access remains explicit and user-triggered.

## Shared UI integration

`BivariateStatisticsPanel` provides:

- X dataset input;
- Y dataset input;
- paired-analysis command;
- formatted covariance/correlation/regression output;
- paired-summary copy action;
- prediction X input;
- prediction command and result.

The panel is attached to the existing Statistics mode by `MainView.BivariateStatistics.cs`. That integration is kept in a small partial-class file so paired-statistics work does not require replacing the large shared XAML file while other UI work is active.

The extension locates the Statistics panel by its actual `StatisticsViewModel` data context rather than by a hard-coded tab index.

## Source contracts

Domain:

- `src/CalcNova.Statistics/StatisticsDatasetParser.cs`;
- `src/CalcNova.Statistics/BivariateStatisticsSummary.cs`;
- `src/CalcNova.Statistics/BivariateStatisticsCalculator.cs`.

Application:

- `src/CalcNova.App/ViewModels/StatisticsViewModel.cs`;
- `src/CalcNova.App/Controls/BivariateStatisticsPanel.cs`;
- `src/CalcNova.App/Views/MainView.BivariateStatistics.cs`.

Tests:

- `tests/CalcNova.Statistics.Tests/StatisticsDatasetParserTests.cs`;
- `tests/CalcNova.Statistics.Tests/BivariateStatisticsCalculatorTests.cs`;
- `tests/CalcNova.App.Tests/BivariateStatisticsViewModelTests.cs`;
- `tests/CalcNova.App.Tests/BivariateStatisticsPanelHeadlessTests.cs`;
- `tests/CalcNova.App.Tests/BivariateStatisticsMainViewHeadlessTests.cs`.

## SDK-independent validation

```bash
python tools/validate_bivariate_statistics.py .
python -m unittest tools.tests.test_validate_bivariate_statistics
```

The focused workflow is `.github/workflows/bivariate-statistics-validate.yml`. The same validator and regression test are included in the integrated SDK-independent release preflight.

## Evidence policy

The implementation, regression source, workflow, and source-contract validation are present. Compiled .NET/Avalonia tests and real target-platform interaction remain **NOT RUN** until their execution is observed in a suitable environment.
