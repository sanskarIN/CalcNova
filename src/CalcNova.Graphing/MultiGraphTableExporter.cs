using System.Globalization;
using System.Text;

namespace CalcNova.Graphing;

public static class MultiGraphTableExporter
{
    public static IReadOnlyList<MultiGraphTableRow> CreateRows(
        IEnumerable<GraphExpressionSample> series,
        int maximumRows = 400)
    {
        ArgumentNullException.ThrowIfNull(series);
        if (maximumRows is < 1 or > 10000)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumRows), maximumRows, "Maximum graph table rows must be between 1 and 10000.");
        }

        var sources = series
            .Select(item => new SeriesCursor(item, Flatten(item.Segments).GetEnumerator()))
            .ToList();
        var rows = new List<MultiGraphTableRow>(Math.Min(maximumRows, 400));

        try
        {
            while (sources.Count > 0 && rows.Count < maximumRows)
            {
                for (var index = sources.Count - 1; index >= 0 && rows.Count < maximumRows; index--)
                {
                    var source = sources[index];
                    if (!source.Points.MoveNext())
                    {
                        source.Points.Dispose();
                        sources.RemoveAt(index);
                        continue;
                    }

                    var point = source.Points.Current;
                    rows.Add(new MultiGraphTableRow(
                        source.Series.Definition.Id,
                        source.Series.Definition.Label,
                        point.Segment,
                        point.X,
                        point.Y));
                }
            }
        }
        finally
        {
            foreach (var source in sources)
            {
                source.Points.Dispose();
            }
        }

        return rows;
    }

    public static string ToCsv(IEnumerable<MultiGraphTableRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var builder = new StringBuilder("expression_id,label,segment,x,y\n");
        foreach (var row in rows)
        {
            builder.Append(Escape(row.ExpressionId));
            builder.Append(',');
            builder.Append(Escape(row.Label));
            builder.Append(',');
            builder.Append(row.Segment.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(row.X.ToString("G17", CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(row.Y.ToString("G17", CultureInfo.InvariantCulture));
            builder.Append('\n');
        }

        return builder.ToString().TrimEnd();
    }

    private static IEnumerable<SegmentPoint> Flatten(IEnumerable<GraphSegment> segments)
    {
        var segmentIndex = 0;
        foreach (var segment in segments)
        {
            segmentIndex++;
            foreach (var point in segment.Points)
            {
                yield return new SegmentPoint(segmentIndex, point.X, point.Y);
            }
        }
    }

    private static string Escape(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    private sealed record SeriesCursor(GraphExpressionSample Series, IEnumerator<SegmentPoint> Points);

    private sealed record SegmentPoint(int Segment, double X, double Y);
}
