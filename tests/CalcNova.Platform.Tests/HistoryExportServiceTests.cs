using System.Text;
using CalcNova.Platform.History;
using Xunit;

namespace CalcNova.Platform.Tests;

public sealed class HistoryExportServiceTests
{
    private readonly HistoryExportService _service = new();

    [Fact]
    public void PlainText_ContainsUtcTimestampExpressionResultAndFavoriteMarker()
    {
        var entry = new HistoryEntry(
            7,
            "2 + 3",
            "5",
            new DateTimeOffset(2026, 8, 18, 15, 0, 0, TimeSpan.FromHours(5.5)),
            true);

        var text = _service.Export([entry], HistoryExportFormat.PlainText);

        Assert.Contains("2026-08-18T09:30:00", text, StringComparison.Ordinal);
        Assert.Contains("2 + 3 = 5", text, StringComparison.Ordinal);
        Assert.Contains("★", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Csv_EscapesCommasQuotesAndNewlines()
    {
        var entry = new HistoryEntry(
            1,
            "max(1, 2) + \"note\"\nnext",
            "2,000",
            DateTimeOffset.UnixEpoch,
            false);

        var csv = _service.Export([entry], HistoryExportFormat.Csv);

        Assert.StartsWith("id,created_at,expression,result,is_favorite\r\n", csv, StringComparison.Ordinal);
        Assert.Contains("\"max(1, 2) + \"\"note\"\"\nnext\"", csv, StringComparison.Ordinal);
        Assert.Contains("\"2,000\"", csv, StringComparison.Ordinal);
    }

    [Fact]
    public void Json_RoundTripsHistoryEntryData()
    {
        var entry = new HistoryEntry(3, "sqrt(81)", "9", DateTimeOffset.UnixEpoch, true);

        var json = _service.Export([entry], HistoryExportFormat.Json);

        Assert.Contains("\"expression\": \"sqrt(81)\"", json, StringComparison.Ordinal);
        Assert.Contains("\"result\": \"9\"", json, StringComparison.Ordinal);
        Assert.Contains("\"isFavorite\": true", json, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExportAsync_WritesUtf8WithoutClosingDestination()
    {
        await using var stream = new MemoryStream();
        var entry = new HistoryEntry(1, "1 + 1", "2", DateTimeOffset.UnixEpoch, false);

        await _service.ExportAsync(stream, [entry], HistoryExportFormat.PlainText);
        stream.WriteByte(0x0A);

        var text = Encoding.UTF8.GetString(stream.ToArray());
        Assert.Contains("1 + 1 = 2", text, StringComparison.Ordinal);
    }

    [Fact]
    public void Export_RejectsMoreThanConfiguredMaximumEntries()
    {
        var entries = Enumerable.Range(1, 5001)
            .Select(index => new HistoryEntry(index, index.ToString(), index.ToString(), DateTimeOffset.UnixEpoch, false))
            .ToArray();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.Export(entries, HistoryExportFormat.Json));
    }
}
