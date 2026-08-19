using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphExportPreviewViewModelTests
{
    [Fact]
    public async Task SingleTablePreview_IsBoundedWhileCopyKeepsFullCsv()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "sin(x) + x",
            MinimumX = "-25",
            MaximumX = "25",
            SampleCount = 600
        };

        viewModel.PlotCommand.Execute(null);

        Assert.True(viewModel.TableCsv.Length > viewModel.TablePreview.Length);
        Assert.True(viewModel.TablePreview.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.TablePreview, StringComparison.Ordinal);

        viewModel.CopyTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.TableCsv, clipboard.WrittenText);
        Assert.NotEqual(viewModel.TablePreview, clipboard.WrittenText);
    }

    [Fact]
    public async Task MultiTablePreview_IsBoundedWhileCopyKeepsFullCsv()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            MultiExpressionsText = "sin(x)\ncos(x)\nx ^ 2",
            MinimumX = "-10",
            MaximumX = "10",
            SampleCount = 400
        };

        viewModel.PlotMultipleCommand.Execute(null);

        Assert.True(viewModel.MultiTableCsv.Length > viewModel.MultiTablePreview.Length);
        Assert.True(viewModel.MultiTablePreview.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.MultiTablePreview, StringComparison.Ordinal);

        viewModel.CopyMultiTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.MultiTableCsv, clipboard.WrittenText);
        Assert.NotEqual(viewModel.MultiTablePreview, clipboard.WrittenText);
    }

    [Fact]
    public async Task SvgPreview_IsBoundedWhileCopyKeepsFullSvg()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new GraphingViewModel(clipboard)
        {
            Expression = "sin(x)",
            MinimumX = "-20",
            MaximumX = "20",
            SampleCount = 600
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.GenerateSvgCommand.Execute(null);

        Assert.True(viewModel.SvgExport.Length > viewModel.SvgPreview.Length);
        Assert.True(viewModel.SvgPreview.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.SvgPreview, StringComparison.Ordinal);

        viewModel.CopySvgCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(viewModel.SvgExport, clipboard.WrittenText);
        Assert.NotEqual(viewModel.SvgPreview, clipboard.WrittenText);
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
