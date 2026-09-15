using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class SvgGraphExporterTests
{
    private readonly SvgGraphExporter _exporter = new();

    [Fact]
    public void Export_ProducesStandaloneSvgWithSeparatePolylines()
    {
        var segments = new[]
        {
            new GraphSegment([new GraphPoint(-1d, -1d), new GraphPoint(0d, 0d)]),
            new GraphSegment([new GraphPoint(1d, 1d), new GraphPoint(2d, 4d)])
        };

        var svg = _exporter.Export(segments, 640, 360);

        Assert.StartsWith("<svg", svg, StringComparison.Ordinal);
        Assert.EndsWith("</svg>", svg, StringComparison.Ordinal);
        Assert.Equal(2, CountOccurrences(svg, "<polyline"));
        Assert.DoesNotContain("NaN", svg, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Infinity", svg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Export_UsesExplicitViewport()
    {
        var segments = new[]
        {
            new GraphSegment([new GraphPoint(0d, 0d), new GraphPoint(1d, 1d)])
        };
        var viewport = new GraphViewport(-1d, 1d, -1d, 1d);

        var svg = _exporter.Export(segments, 100, 100, viewport);

        Assert.Contains("50,50", svg, StringComparison.Ordinal);
        Assert.Contains("100,0", svg, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(63, 720)]
    [InlineData(1280, 63)]
    [InlineData(8193, 720)]
    [InlineData(1280, 8193)]
    public void Export_RejectsUnsafeDimensions(int width, int height)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _exporter.Export(Array.Empty<GraphSegment>(), width, height));
    }

    [Fact]
    public void GraphViewport_FromSegments_AddsPaddingAndHandlesFlatData()
    {
        var viewport = GraphViewport.FromSegments([
            new GraphSegment([new GraphPoint(5d, 2d), new GraphPoint(5d, 2d)])
        ]);

        viewport.Validate();
        Assert.True(viewport.Width > 0d);
        Assert.True(viewport.Height > 0d);
        Assert.True(viewport.MinimumX < 5d);
        Assert.True(viewport.MaximumX > 5d);
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
