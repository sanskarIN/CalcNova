using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ExportPreviewFormatterTests
{
    [Fact]
    public void Create_ReturnsShortContentUnchanged()
    {
        const string content = "first line\nsecond line";

        var preview = ExportPreviewFormatter.Create(content);

        Assert.Equal(content, preview);
    }

    [Fact]
    public void Create_TruncatesContentAboveCharacterBudget()
    {
        var content = new string('x', 600);

        var preview = ExportPreviewFormatter.Create(content, maximumCharacters: 160, maximumLines: 20);

        Assert.True(preview.Length <= 160);
        Assert.Contains("preview truncated", preview, StringComparison.Ordinal);
        Assert.DoesNotContain(content, preview, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_TruncatesContentAboveLineBudget()
    {
        var content = string.Join('\n', Enumerable.Range(1, 20).Select(index => $"line-{index}"));

        var preview = ExportPreviewFormatter.Create(content, maximumCharacters: 1_000, maximumLines: 3);

        Assert.Contains("line-1", preview, StringComparison.Ordinal);
        Assert.Contains("line-3", preview, StringComparison.Ordinal);
        Assert.DoesNotContain("line-4", preview, StringComparison.Ordinal);
        Assert.Contains("preview truncated", preview, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_DoesNotSplitUtf16SurrogatePairAtCharacterBoundary()
    {
        var content = new string('a', 95) + "😀" + new string('b', 100);

        var preview = ExportPreviewFormatter.Create(content, maximumCharacters: 160, maximumLines: 20);
        var prefix = preview.Split(Environment.NewLine, StringSplitOptions.None)[0];

        Assert.False(prefix.Length > 0 && char.IsHighSurrogate(prefix[^1]));
        Assert.DoesNotContain('\uFFFD', preview);
    }

    [Fact]
    public void Create_RejectsNonPositiveLineBudget()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ExportPreviewFormatter.Create("content", maximumLines: 0));
    }

    [Fact]
    public void Create_RejectsCharacterBudgetTooSmallForTruncationNotice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ExportPreviewFormatter.Create("content", maximumCharacters: 10));
    }
}
