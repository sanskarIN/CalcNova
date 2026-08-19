using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class MultiGraphTableExporterTests
{
    [Fact]
    public void CreateRows_RoundRobinsSeriesWithoutLosingIdentity()
    {
        var sample = new MultiGraphSampler().Sample(
            [
                new GraphExpressionDefinition("f1", "Linear", "x"),
                new GraphExpressionDefinition("f2", "Square", "x ^ 2")
            ],
            new GraphSamplingOptions
            {
                MinimumX = 0,
                MaximumX = 2,
                SampleCount = 3
            });
        Assert.True(sample.Success);

        var rows = MultiGraphTableExporter.CreateRows(sample.Series, 4);

        Assert.Equal(4, rows.Count);
        Assert.Equal(["f1", "f2", "f1", "f2"], rows.Select(row => row.ExpressionId).ToArray());
        Assert.Equal(["Linear", "Square", "Linear", "Square"], rows.Select(row => row.Label).ToArray());
    }

    [Fact]
    public void ToCsv_IncludesExpressionIdentityAndEscapesLabels()
    {
        var csv = MultiGraphTableExporter.ToCsv([
            new MultiGraphTableRow("f1", "Linear, primary", 1, 1.25, -2.5),
            new MultiGraphTableRow("f2", "Square", 2, 3, 4)
        ]);

        Assert.Equal(
            "expression_id,label,segment,x,y\nf1,\"Linear, primary\",1,1.25,-2.5\nf2,Square,2,3,4",
            csv);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10001)]
    public void CreateRows_RejectsInvalidBounds(int maximumRows)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MultiGraphTableExporter.CreateRows(Array.Empty<GraphExpressionSample>(), maximumRows));
    }
}
