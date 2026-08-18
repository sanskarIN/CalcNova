using System.Globalization;
using System.Text;
using CalcNova.Platform.History;

namespace CalcNova.App.Services;

public static class HistoryExportFormatter
{
    public static string ToCsv(IEnumerable<HistoryEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        var builder = new StringBuilder();
        builder.AppendLine("id,created_at_utc,is_favorite,expression,result");
        foreach (var entry in entries)
        {
            builder.Append(entry.Id.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(Escape(entry.CreatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
            builder.Append(',');
            builder.Append(entry.IsFavorite ? "true" : "false");
            builder.Append(',');
            builder.Append(Escape(entry.Expression));
            builder.Append(',');
            builder.Append(Escape(entry.Result));
            builder.AppendLine();
        }

        return builder.ToString();
    }

    private static string Escape(string value)
    {
        value ??= string.Empty;
        if (!value.Contains(',', StringComparison.Ordinal) &&
            !value.Contains('"') &&
            !value.Contains('\n') &&
            !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
