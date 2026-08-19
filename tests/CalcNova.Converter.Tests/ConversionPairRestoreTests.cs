using CalcNova.Converter;
using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class ConversionPairRestoreTests
{
    [Fact]
    public void Restore_PreservesMostRecentFirstOrderingAndFavorites()
    {
        var recent = new[]
        {
            new ConversionPair("km", "m"),
            new ConversionPair("m", "cm")
        };
        var favorites = new[]
        {
            new ConversionPair("kg", "g")
        };
        var history = new ConversionPairHistory();

        history.Restore(recent, favorites);

        Assert.Equal(recent, history.Recent);
        Assert.Equal(favorites, history.Favorites);
    }

    [Fact]
    public void Restore_DeduplicatesAndHonorsRecentCapacity()
    {
        var first = new ConversionPair("m", "km");
        var second = new ConversionPair("m", "cm");
        var third = new ConversionPair("m", "mm");
        var history = new ConversionPairHistory(2);

        history.Restore(new[] { first, second, third, first }, null);

        Assert.Equal(2, history.Recent.Count);
        Assert.Equal(first, history.Recent[0]);
        Assert.Equal(third, history.Recent[1]);
    }

    [Fact]
    public void Record_ReturnsFalseWhenPairIsAlreadyMostRecent()
    {
        var pair = new ConversionPair("m", "km");
        var history = new ConversionPairHistory();

        Assert.True(history.Record(pair));
        Assert.False(history.Record(pair));
    }
}
