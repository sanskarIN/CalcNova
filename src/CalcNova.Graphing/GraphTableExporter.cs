using System.Globalization;
using System.Text;

namespace CalcNova.Graphing;

public static class GraphTableExporter
{
    public static IReadOnlyList<GraphTableRow> CreateRows(
        IEnumerable<GraphSegment> segments,
        int maximumRows = 1000)
    {
        ArgumentNullException.ThrowIfNull(segments);
        if (maximumRows is < 1 or > 10000)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRows), maximumRows, "Maximum graph table rows must be between 1 and 10000.");
        }

        var rows = new List<GraphTableRow>(Math.Min(maximumRows, 256));
        var segmentIndex = 0;
        foreach (var segment in segments)
        {
            segmentIndex++;
            foreach (var point in segment.Points)
            {
                rows.Add(new GraphTableRow(segmentIndex, point.X, point.Y));
                if (rows.Count >= maximumRows)
                {
                    return rows;
                }
            }
        }

        return rows;
    }

    public static string ToCsv(IEnumerable<GraphTableRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var builder = new StringBuilder("segment,x,y\n");
        foreach (var row in rows)
        {
            builder.Append(row.Segment.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(row.X.ToString("G17", CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(row.Y.ToString("G17", CultureInfo.InvariantCulture));
            builder.Append('\n');
        }

        return builder.ToString().TrimEnd();
    }
}
