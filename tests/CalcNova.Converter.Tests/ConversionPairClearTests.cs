using CalcNova.Converter;
using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class ConversionPairClearTests
{
    [Fact]
    public void ClearRecent_ReturnsTrueOnlyWhenStateChanges()
    {
        var history = new ConversionPairHistory();
        history.Record(new ConversionPair("m", "km"));

        Assert.True(history.ClearRecent());
        Assert.Empty(history.Recent);
        Assert.False(history.ClearRecent());
    }

    [Fact]
    public void ClearRecent_DoesNotRemoveFavorites()
    {
        var history = new ConversionPairHistory();
        var pair = new ConversionPair("kg", "g");
        history.Record(pair);
        history.ToggleFavorite(pair);

        history.ClearRecent();

        Assert.Empty(history.Recent);
        Assert.Contains(pair, history.Favorites);
    }
}
