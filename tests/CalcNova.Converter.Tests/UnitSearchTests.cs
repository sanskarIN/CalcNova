using CalcNova.Converter;
using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class UnitSearchTests
{
    [Fact]
    public void Search_MatchesUnitNameSymbolAndIdWithinCategory()
    {
        var results = UnitSearch.Search(UnitCategory.Length, "meter");

        Assert.Contains(results, unit => unit.Id == "m");
        Assert.DoesNotContain(results, unit => unit.Category != UnitCategory.Length);
    }

    [Fact]
    public void Search_PrioritizesExactSymbolMatch()
    {
        var results = UnitSearch.Search(UnitCategory.Length, "m");

        Assert.NotEmpty(results);
        Assert.Equal("m", results[0].Id);
    }

    [Fact]
    public void Search_BlankQueryReturnsBoundedCategoryUnits()
    {
        var results = UnitSearch.Search(UnitCategory.Length, " ", 3);

        Assert.Equal(3, results.Count);
        Assert.All(results, unit => Assert.Equal(UnitCategory.Length, unit.Category));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public void Search_RejectsUnboundedResultLimits(int maximumResults)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            UnitSearch.Search(UnitCategory.Length, "m", maximumResults));
    }
}
