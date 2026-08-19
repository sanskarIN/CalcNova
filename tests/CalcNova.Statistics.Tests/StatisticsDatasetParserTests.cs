using CalcNova.Statistics;
using Xunit;

namespace CalcNova.Statistics.Tests;

public sealed class StatisticsDatasetParserTests
{
    [Fact]
    public void Parse_AcceptsSupportedSeparatorsAndInvariantNumbers()
    {
        var values = StatisticsDatasetParser.Parse("1, 2;3\n4\t5.5");

        Assert.Equal([1d, 2d, 3d, 4d, 5.5d], values);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyDataset()
    {
        Assert.Empty(StatisticsDatasetParser.Parse("   "));
    }

    [Theory]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    [InlineData("-Infinity")]
    [InlineData("one")]
    public void Parse_RejectsNonFiniteOrInvalidTokens(string token)
    {
        Assert.Throws<FormatException>(() => StatisticsDatasetParser.Parse($"1,{token},2"));
    }

    [Fact]
    public void Parse_RejectsValuesAboveRequestedBudget()
    {
        Assert.Throws<ArgumentException>(() => StatisticsDatasetParser.Parse("1,2,3", maximumValues: 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100001)]
    public void Parse_RejectsInvalidMaximumValueBudget(int maximumValues)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            StatisticsDatasetParser.Parse("1", maximumValues));
    }

    [Fact]
    public void Parse_RejectsInputAboveCharacterBudgetBeforeSplitting()
    {
        var input = new string('1', StatisticsDatasetParser.MaximumInputCharacters + 1);

        Assert.Throws<ArgumentException>(() => StatisticsDatasetParser.Parse(input));
    }
}
