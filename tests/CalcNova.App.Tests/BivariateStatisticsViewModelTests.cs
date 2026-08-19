using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class BivariateStatisticsViewModelTests
{
    [Fact]
    public void AnalyzePairsCommand_FormatsCovarianceCorrelationAndRegression()
    {
        var viewModel = new StatisticsViewModel
        {
            PairedXText = "1, 2, 3, 4",
            PairedYText = "3, 5, 7, 9"
        };

        viewModel.AnalyzePairsCommand.Execute(null);

        Assert.Contains("Pairs: 4", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Contains("Population covariance: 2.5", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Contains("Pearson r: 1", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Contains("Regression slope: 2", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Contains("Regression intercept: 1", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Contains("R²: 1", viewModel.BivariateSummary, StringComparison.Ordinal);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void PredictCommand_UsesMostRecentPairedAnalysis()
    {
        var viewModel = new StatisticsViewModel
        {
            PairedXText = "1 2 3 4",
            PairedYText = "3 5 7 9",
            PredictionX = "5"
        };
        viewModel.AnalyzePairsCommand.Execute(null);

        viewModel.PredictCommand.Execute(null);

        Assert.Equal("ŷ(5) = 11", viewModel.PredictionResult);
        Assert.Empty(viewModel.ErrorMessage);
    }

    [Fact]
    public void AnalyzePairsCommand_RejectsMismatchedDatasetsAndClearsStalePrediction()
    {
        var viewModel = new StatisticsViewModel
        {
            PairedXText = "1,2,3",
            PairedYText = "2,4,6",
            PredictionX = "4"
        };
        viewModel.AnalyzePairsCommand.Execute(null);
        viewModel.PredictCommand.Execute(null);
        Assert.NotEmpty(viewModel.PredictionResult);

        viewModel.PairedYText = "2,4";
        viewModel.AnalyzePairsCommand.Execute(null);

        Assert.Empty(viewModel.BivariateSummary);
        Assert.Empty(viewModel.PredictionResult);
        Assert.Contains("same number", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PredictCommand_ReportsUndefinedRegressionForConstantX()
    {
        var viewModel = new StatisticsViewModel
        {
            PairedXText = "2,2,2",
            PairedYText = "1,2,3",
            PredictionX = "4"
        };
        viewModel.AnalyzePairsCommand.Execute(null);

        viewModel.PredictCommand.Execute(null);

        Assert.Empty(viewModel.PredictionResult);
        Assert.Contains("zero variance", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyBivariateSummaryCommand_CopiesFormattedSummary()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new StatisticsViewModel(clipboard)
        {
            PairedXText = "1,2,3",
            PairedYText = "2,4,6"
        };
        viewModel.AnalyzePairsCommand.Execute(null);

        viewModel.CopyBivariateSummaryCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.BivariateSummary, clipboard.WrittenText);
    }

    [Fact]
    public void AnalyzeCommand_UsesBoundedSharedDatasetParser()
    {
        var viewModel = new StatisticsViewModel
        {
            DatasetText = "1 2 3 4 5"
        };

        viewModel.AnalyzeCommand.Execute(null);

        Assert.Contains("Count: 5", viewModel.Summary, StringComparison.Ordinal);
        Assert.Empty(viewModel.ErrorMessage);
    }

    private sealed class FakeClipboardService : IClipboardService
    {
        private readonly TaskCompletionSource _writeSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public bool IsAvailable => true;

        public string? WrittenText { get; private set; }

        public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        {
            WrittenText = text;
            _writeSignal.TrySetResult();
            return Task.CompletedTask;
        }

        public Task WaitForWriteAsync() => _writeSignal.Task.WaitAsync(TimeSpan.FromSeconds(2));
    }
}
