using CalcNova.App.ViewModels;
using CalcNova.Graphing;
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
        var expectedFullExport = GraphTableExporter.ToCsv(viewModel.TableRows);

        Assert.True(expectedFullExport.Length > viewModel.TableCsv.Length);
        Assert.Equal(viewModel.TablePreview, viewModel.TableCsv);
        Assert.True(viewModel.TableCsv.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.TableCsv, StringComparison.Ordinal);

        viewModel.CopyTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(expectedFullExport, clipboard.WrittenText);
        Assert.NotEqual(viewModel.TableCsv, clipboard.WrittenText);
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
        var expectedFullExport = MultiGraphTableExporter.ToCsv(viewModel.MultiTableRows);

        Assert.True(expectedFullExport.Length > viewModel.MultiTableCsv.Length);
        Assert.Equal(viewModel.MultiTablePreview, viewModel.MultiTableCsv);
        Assert.True(viewModel.MultiTableCsv.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.MultiTableCsv, StringComparison.Ordinal);

        viewModel.CopyMultiTableCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(expectedFullExport, clipboard.WrittenText);
        Assert.NotEqual(viewModel.MultiTableCsv, clipboard.WrittenText);
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
            SampleCount = 2_000
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.GenerateSvgCommand.Execute(null);
        var expectedFullExport = new SvgGraphExporter().Export(viewModel.Segments);

        Assert.True(expectedFullExport.Length > viewModel.SvgExport.Length);
        Assert.Equal(viewModel.SvgPreview, viewModel.SvgExport);
        Assert.True(viewModel.SvgExport.Length <= 4_096);
        Assert.Contains("preview truncated", viewModel.SvgExport, StringComparison.Ordinal);

        viewModel.CopySvgCommand.Execute(null);
        await clipboard.WaitForWriteAsync();

        Assert.Equal(expectedFullExport, clipboard.WrittenText);
        Assert.NotEqual(viewModel.SvgExport, clipboard.WrittenText);
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
