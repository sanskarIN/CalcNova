using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphTableExporterTests
{
    [Fact]
    public void CreateRows_FlattensSampledSegmentsWithSegmentNumbers()
    {
        var sampler = new GraphSampler();
        var sample = sampler.Sample("x", new GraphSamplingOptions
        {
            MinimumX = 0,
            MaximumX = 1,
            SampleCount = 3
        });
        Assert.True(sample.Success);

        var rows = GraphTableExporter.CreateRows(sample.Segments);

        Assert.Equal(3, rows.Count);
        Assert.All(rows, row => Assert.Equal(1, row.Segment));
        Assert.Equal(0, rows[0].X, 12);
        Assert.Equal(0, rows[0].Y, 12);
        Assert.Equal(1, rows[^1].X, 12);
        Assert.Equal(1, rows[^1].Y, 12);
    }

    [Fact]
    public void CreateRows_RespectsMaximumRowBound()
    {
        var sampler = new GraphSampler();
        var sample = sampler.Sample("x ^ 2", new GraphSamplingOptions
        {
            MinimumX = -10,
            MaximumX = 10,
            SampleCount = 101
        });
        Assert.True(sample.Success);

        var rows = GraphTableExporter.CreateRows(sample.Segments, 10);

        Assert.Equal(10, rows.Count);
    }

    [Fact]
    public void ToCsv_UsesInvariantRoundTripFriendlyNumbers()
    {
        var csv = GraphTableExporter.ToCsv([
            new GraphTableRow(1, 1.25, -2.5),
            new GraphTableRow(2, 3, 4)
        ]);

        Assert.Equal("segment,x,y\n1,1.25,-2.5\n2,3,4", csv);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10001)]
    public void CreateRows_RejectsInvalidBounds(int maximumRows)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            GraphTableExporter.CreateRows(Array.Empty<GraphSegment>(), maximumRows));
    }
}
