using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphTableViewModelTests
{
    [Fact]
    public void PlotCommand_PopulatesBoundedTableRowsAndCsv()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x",
            MinimumX = "0",
            MaximumX = "1",
            SampleCount = 5
        };

        viewModel.PlotCommand.Execute(null);

        Assert.Equal(5, viewModel.TableRows.Count);
        Assert.StartsWith("segment,x,y", viewModel.TableCsv, StringComparison.Ordinal);
        Assert.Contains("1,0,0", viewModel.TableCsv, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CopyTableCommand_CopiesCsvTable()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "x ^ 2",
            MinimumX = "0",
            MaximumX = "2",
            SampleCount = 3
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.CopyTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.TableCsv, clipboard.WrittenText);
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
