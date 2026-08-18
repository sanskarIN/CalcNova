using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CalcNova.Platform.History;

public sealed class HistoryExportService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public string Export(IReadOnlyList<HistoryEntry> entries, HistoryExportFormat format)
    {
        ArgumentNullException.ThrowIfNull(entries);
        if (entries.Count > 5000)
        {
            throw new ArgumentOutOfRangeException(nameof(entries), "A single history export may contain at most 5000 entries.");
        }

        return format switch
        {
            HistoryExportFormat.PlainText => ExportPlainText(entries),
            HistoryExportFormat.Csv => ExportCsv(entries),
            HistoryExportFormat.Json => JsonSerializer.Serialize(entries, JsonOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported history export format.")
        };
    }

    public async Task ExportAsync(
        Stream destination,
        IReadOnlyList<HistoryEntry> entries,
        HistoryExportFormat format,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination stream must be writable.", nameof(destination));
        }

        var content = Export(entries, format);
        await using var writer = new StreamWriter(destination, new UTF8Encoding(false), 4096, leaveOpen: true);
        await writer.WriteAsync(content.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }

    private static string ExportPlainText(IReadOnlyList<HistoryEntry> entries)
    {
        var builder = new StringBuilder();
        foreach (var entry in entries)
        {
            builder.Append('[')
                .Append(entry.CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))
                .Append("] ")
                .Append(entry.Expression.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal))
                .Append(" = ")
                .Append(entry.Result.Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal));

            if (entry.IsFavorite)
            {
                builder.Append("  ★");
            }

            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string ExportCsv(IReadOnlyList<HistoryEntry> entries)
    {
        var builder = new StringBuilder("id,created_at,expression,result,is_favorite\r\n");
        foreach (var entry in entries)
        {
            builder.Append(entry.Id.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(EscapeCsv(entry.CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))).Append(',')
                .Append(EscapeCsv(entry.Expression)).Append(',')
                .Append(EscapeCsv(entry.Result)).Append(',')
                .Append(entry.IsFavorite ? "true" : "false")
                .Append("\r\n");
        }

        return builder.ToString();
    }

    private static string EscapeCsv(string value)
    {
        var normalized = value.Replace("\0", string.Empty, StringComparison.Ordinal);
        var mustQuote = normalized.Contains(',') ||
                        normalized.Contains('"') ||
                        normalized.Contains('\r') ||
                        normalized.Contains('\n');
        if (!mustQuote)
        {
            return normalized;
        }

        return $"\"{normalized.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
