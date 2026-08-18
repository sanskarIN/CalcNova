using CalcNova.App.Services;
using CalcNova.Platform.History;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class HistoryExportFormatterTests
{
    [Fact]
    public void ToCsv_WritesStableHeaderAndFields()
    {
        var entries = new[]
        {
            new HistoryEntry(
                7,
                "2 + 3",
                "5",
                new DateTimeOffset(2026, 8, 18, 12, 30, 0, TimeSpan.Zero),
                true)
        };

        var csv = HistoryExportFormatter.ToCsv(entries);

        Assert.StartsWith("id,created_at_utc,is_favorite,expression,result", csv, StringComparison.Ordinal);
        Assert.Contains("7,2026-08-18T12:30:00.0000000+00:00,true,2 + 3,5", csv, StringComparison.Ordinal);
    }

    [Fact]
    public void ToCsv_EscapesCommaQuoteAndNewline()
    {
        var entries = new[]
        {
            new HistoryEntry(1, "max(1, 2)", "\"quoted\"\nvalue", DateTimeOffset.UnixEpoch, false)
        };

        var csv = HistoryExportFormatter.ToCsv(entries);

        Assert.Contains("\"max(1, 2)\"", csv, StringComparison.Ordinal);
        Assert.Contains("\"\"\"quoted\"\"\nvalue\"", csv, StringComparison.Ordinal);
    }
}
