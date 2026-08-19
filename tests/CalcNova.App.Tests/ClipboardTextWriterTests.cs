using CalcNova.App.Services;
using CalcNova.Platform.Clipboard;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ClipboardTextWriterTests
{
    [Fact]
    public async Task CopyAsync_WritesTextAndReturnsSuccessStatus()
    {
        var clipboard = new FakeClipboardService();

        var status = await ClipboardTextWriter.CopyAsync(clipboard, "FF", "Hexadecimal value");

        Assert.Equal("FF", clipboard.WrittenText);
        Assert.Equal("Hexadecimal value copied.", status);
    }

    [Fact]
    public async Task CopyAsync_ReportsUnavailableClipboard()
    {
        var status = await ClipboardTextWriter.CopyAsync(null, "42", "Result");

        Assert.Contains("unavailable", status, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CopyAsync_RejectsEmptyText()
    {
        var clipboard = new FakeClipboardService();

        var status = await ClipboardTextWriter.CopyAsync(clipboard, " ", "Result");

        Assert.Null(clipboard.WrittenText);
        Assert.Contains("no result", status, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeClipboardService : IClipboardService
    {
        public bool IsAvailable => true;

        public string? WrittenText { get; private set; }

        public Task<string?> GetTextAsync(CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);

        public Task SetTextAsync(string text, CancellationToken cancellationToken = default)
        {
            WrittenText = text;
            return Task.CompletedTask;
        }
    }
}
