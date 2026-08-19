using Xunit;

namespace CalcNova.Graphing.Tests;

public sealed class GraphExpressionListParserTests
{
    [Fact]
    public void Parse_AssignsStableIdsAndLabelsInInputOrder()
    {
        var expressions = GraphExpressionListParser.Parse("sin(x)\ncos(x)\nx ^ 2");

        Assert.Equal(3, expressions.Count);
        Assert.Equal("series-1", expressions[0].Id);
        Assert.Equal("f1", expressions[0].Label);
        Assert.Equal("sin(x)", expressions[0].Expression);
        Assert.Equal("series-3", expressions[2].Id);
        Assert.Equal("x ^ 2", expressions[2].Expression);
    }

    [Fact]
    public void Parse_IgnoresBlankLines()
    {
        var expressions = GraphExpressionListParser.Parse("\n sin(x) \n\n cos(x) \r\n");

        Assert.Equal(2, expressions.Count);
        Assert.Equal("sin(x)", expressions[0].Expression);
        Assert.Equal("cos(x)", expressions[1].Expression);
    }

    [Fact]
    public void Parse_RejectsMoreThanSupportedExpressionCount()
    {
        var text = string.Join('\n', Enumerable.Range(1, MultiGraphSampler.MaximumExpressions + 1).Select(index => $"x + {index}"));

        Assert.Throws<ArgumentOutOfRangeException>(() => GraphExpressionListParser.Parse(text));
    }
}
