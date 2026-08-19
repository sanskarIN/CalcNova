using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphMultiExpressionViewModelTests
{
    [Fact]
    public void PlotMultipleCommand_SamplesAndExportsEachExpression()
    {
        var viewModel = new GraphingViewModel
        {
            MultiExpressionsText = "x\nx ^ 2",
            MinimumX = "0",
            MaximumX = "2",
            SampleCount = 3
        };

        viewModel.PlotMultipleCommand.Execute(null);

        Assert.Equal(2, viewModel.MultiSeries.Count);
        Assert.Contains("2 expression", viewModel.MultiSummary, StringComparison.Ordinal);
        Assert.StartsWith("expression_id,label,segment,x,y", viewModel.MultiTableCsv, StringComparison.Ordinal);
        Assert.Contains("series-1,f1", viewModel.MultiTableCsv, StringComparison.Ordinal);
        Assert.Contains("series-2,f2", viewModel.MultiTableCsv, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void PlotMultipleCommand_RejectsEmptyExpressionList()
    {
        var viewModel = new GraphingViewModel { MultiExpressionsText = "   " };

        viewModel.PlotMultipleCommand.Execute(null);

        Assert.Empty(viewModel.MultiSeries);
        Assert.Contains("At least one", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyMultiTableCommand_CopiesIdentifiedCsv()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            MultiExpressionsText = "sin(x)\ncos(x)",
            MinimumX = "0",
            MaximumX = "1",
            SampleCount = 3
        };
        viewModel.PlotMultipleCommand.Execute(null);

        viewModel.CopyMultiTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.MultiTableCsv, clipboard.WrittenText);
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
