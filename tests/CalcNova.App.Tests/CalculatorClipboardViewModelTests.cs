using CalcNova.App.ViewModels;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorClipboardViewModelTests
{
    [Fact]
    public async Task PasteAsync_SanitizesClipboardExpressionBeforeImport()
    {
        var clipboard = new FakeClipboardService("= 2 × π");
        var viewModel = new CalculatorViewModel(clipboardService: clipboard);

        await viewModel.PasteAsync();

        Assert.Equal("2 * pi", viewModel.Expression);
        Assert.Equal(string.Empty, viewModel.StatusMessage);
    }

    [Fact]
    public async Task PasteAsync_RejectsUnsafeClipboardTextWithoutReplacingExpression()
    {
        var clipboard = new FakeClipboardService("2 @ 3");
        var viewModel = new CalculatorViewModel(clipboardService: clipboard)
        {
            Expression = "1 + 1"
        };

        await viewModel.PasteAsync();

        Assert.Equal("1 + 1", viewModel.Expression);
        Assert.Contains("unsupported character", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyResultAsync_WritesOnlyValidResult()
    {
        var clipboard = new FakeClipboardService();
        var viewModel = new CalculatorViewModel(clipboardService: clipboard)
        {
            Expression = "6 * 7"
        };
        await viewModel.EvaluateAsync();

        await viewModel.CopyResultAsync();

        Assert.Equal("42", clipboard.WrittenText);
        Assert.Equal("Result copied.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task ClipboardCommands_ReportUnavailableService()
    {
        var viewModel = new CalculatorViewModel();

        await viewModel.PasteAsync();
        Assert.Contains("unavailable", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);

        await viewModel.CopyResultAsync();
        Assert.Contains("unavailable", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeClipboardService(string? initialText = null) : IClipboardService
    {
        public bool IsAvailable { get; init; } = true;

        public string? ReadText { get; set; } = initialText;

        public string? WrittenText { get; private set; }

        public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(ReadText);

        public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        {
            WrittenText = text;
            return Task.CompletedTask;
        }
    }
}
