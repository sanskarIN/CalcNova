using CalcNova.Graphing;
using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class MultiGraphSamplerTests
{
    [Fact]
    public void Sample_PreservesStableExpressionIdentityAndOrder()
    {
        var sampler = new MultiGraphSampler();
        var result = sampler.Sample(
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

        Assert.True(result.Success);
        Assert.Equal(2, result.Series.Count);
        Assert.Equal("f1", result.Series[0].Definition.Id);
        Assert.Equal("Linear", result.Series[0].Definition.Label);
        Assert.Equal("f2", result.Series[1].Definition.Id);
        Assert.Equal(6, result.TotalValidPointCount);
    }

    [Fact]
    public void Sample_RejectsDuplicateStableIds()
    {
        var sampler = new MultiGraphSampler();

        var result = sampler.Sample(
            [
                new GraphExpressionDefinition("same", "First", "x"),
                new GraphExpressionDefinition("same", "Second", "x + 1")
            ],
            new GraphSamplingOptions());

        Assert.False(result.Success);
        Assert.Contains("duplicated", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Sample_RejectsMoreThanMaximumExpressionCount()
    {
        var expressions = Enumerable.Range(1, MultiGraphSampler.MaximumExpressions + 1)
            .Select(index => new GraphExpressionDefinition($"f{index}", $"Expression {index}", "x"))
            .ToArray();

        var result = new MultiGraphSampler().Sample(expressions, new GraphSamplingOptions());

        Assert.False(result.Success);
        Assert.Contains(MultiGraphSampler.MaximumExpressions.ToString(), result.ErrorMessage, StringComparison.Ordinal);
    }

    [Fact]
    public void Sample_ReportsWhichExpressionFailed()
    {
        var sampler = new MultiGraphSampler();

        var result = sampler.Sample(
            [
                new GraphExpressionDefinition("f1", "Good", "x"),
                new GraphExpressionDefinition("f2", "Broken", "unknownFunction(x)")
            ],
            new GraphSamplingOptions());

        Assert.False(result.Success);
        Assert.Contains("Broken", result.ErrorMessage, StringComparison.Ordinal);
        Assert.Empty(result.Series);
    }

    [Fact]
    public void ExpressionDefinition_NormalizesIdentityFields()
    {
        var definition = new GraphExpressionDefinition(" f1 ", " Linear ", " x + 1 ");

        Assert.Equal("f1", definition.Id);
        Assert.Equal("Linear", definition.Label);
        Assert.Equal("x + 1", definition.Expression);
    }

    [Theory]
    [InlineData("", "Label", "x")]
    [InlineData("f1", "", "x")]
    [InlineData("f1", "Label", "")]
    public void ExpressionDefinition_RejectsBlankRequiredFields(string id, string label, string expression)
    {
        Assert.Throws<ArgumentException>(() => new GraphExpressionDefinition(id, label, expression));
    }
}
