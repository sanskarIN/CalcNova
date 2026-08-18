using System.Globalization;
using System.Text;

namespace CalcNova.Graphing;

public sealed class SvgGraphExporter
{
    public string Export(
        IReadOnlyList<GraphSegment> segments,
        int width = 1280,
        int height = 720,
        GraphViewport? viewport = null)
    {
        ArgumentNullException.ThrowIfNull(segments);
        if (width is < 64 or > 8192)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "SVG width must be between 64 and 8192 pixels.");
        }

        if (height is < 64 or > 8192)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "SVG height must be between 64 and 8192 pixels.");
        }

        var resolvedViewport = viewport ?? GraphViewport.FromSegments(segments);
        resolvedViewport.Validate();

        var builder = new StringBuilder(capacity: 4096);
        builder.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 ")
            .Append(width.ToString(CultureInfo.InvariantCulture))
            .Append(' ')
            .Append(height.ToString(CultureInfo.InvariantCulture))
            .Append("\" role=\"img\" aria-label=\"CalcNova graph export\">");
        builder.Append("<rect width=\"100%\" height=\"100%\" fill=\"#ffffff\"/>");

        AppendGrid(builder, width, height, resolvedViewport);
        AppendAxes(builder, width, height, resolvedViewport);

        foreach (var segment in segments)
        {
            var finitePoints = segment.Points
                .Where(point => double.IsFinite(point.X) && double.IsFinite(point.Y))
                .ToArray();
            if (finitePoints.Length == 0)
            {
                continue;
            }

            builder.Append("<polyline fill=\"none\" stroke=\"#182033\" stroke-width=\"2.5\" stroke-linecap=\"round\" stroke-linejoin=\"round\" points=\"");
            foreach (var point in finitePoints)
            {
                var mapped = Map(point, width, height, resolvedViewport);
                builder.Append(Format(mapped.X))
                    .Append(',')
                    .Append(Format(mapped.Y))
                    .Append(' ');
            }

            builder.Append("\"/>");
        }

        builder.Append("</svg>");
        return builder.ToString();
    }

    public async Task ExportAsync(
        Stream destination,
        IReadOnlyList<GraphSegment> segments,
        int width = 1280,
        int height = 720,
        GraphViewport? viewport = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite)
        {
            throw new ArgumentException("Destination stream must be writable.", nameof(destination));
        }

        var svg = Export(segments, width, height, viewport);
        await using var writer = new StreamWriter(destination, new UTF8Encoding(false), bufferSize: 4096, leaveOpen: true);
        await writer.WriteAsync(svg.AsMemory(), cancellationToken);
        await writer.FlushAsync(cancellationToken);
    }

    private static void AppendGrid(StringBuilder builder, int width, int height, GraphViewport viewport)
    {
        const int divisions = 10;
        builder.Append("<g stroke=\"#d8dce5\" stroke-width=\"1\">");
        for (var index = 1; index < divisions; index++)
        {
            var x = width * index / (double)divisions;
            var y = height * index / (double)divisions;
            builder.Append("<line x1=\"").Append(Format(x)).Append("\" y1=\"0\" x2=\"")
                .Append(Format(x)).Append("\" y2=\"").Append(height.ToString(CultureInfo.InvariantCulture)).Append("\"/>");
            builder.Append("<line x1=\"0\" y1=\"").Append(Format(y)).Append("\" x2=\"")
                .Append(width.ToString(CultureInfo.InvariantCulture)).Append("\" y2=\"").Append(Format(y)).Append("\"/>");
        }

        builder.Append("</g>");
    }

    private static void AppendAxes(StringBuilder builder, int width, int height, GraphViewport viewport)
    {
        builder.Append("<g stroke=\"#7b8496\" stroke-width=\"1.5\">");
        if (viewport.MinimumX <= 0d && viewport.MaximumX >= 0d)
        {
            var x = Map(new GraphPoint(0d, viewport.MinimumY), width, height, viewport).X;
            builder.Append("<line x1=\"").Append(Format(x)).Append("\" y1=\"0\" x2=\"")
                .Append(Format(x)).Append("\" y2=\"").Append(height.ToString(CultureInfo.InvariantCulture)).Append("\"/>");
        }

        if (viewport.MinimumY <= 0d && viewport.MaximumY >= 0d)
        {
            var y = Map(new GraphPoint(viewport.MinimumX, 0d), width, height, viewport).Y;
            builder.Append("<line x1=\"0\" y1=\"").Append(Format(y)).Append("\" x2=\"")
                .Append(width.ToString(CultureInfo.InvariantCulture)).Append("\" y2=\"").Append(Format(y)).Append("\"/>");
        }

        builder.Append("</g>");
    }

    private static GraphPoint Map(GraphPoint point, int width, int height, GraphViewport viewport)
    {
        var x = (point.X - viewport.MinimumX) / viewport.Width * width;
        var y = height - ((point.Y - viewport.MinimumY) / viewport.Height * height);
        return new GraphPoint(x, y);
    }

    private static string Format(double value) => value.ToString("0.########", CultureInfo.InvariantCulture);
}
