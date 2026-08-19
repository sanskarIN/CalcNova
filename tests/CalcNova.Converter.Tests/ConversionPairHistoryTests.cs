using CalcNova.Converter;
using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class ConversionPairHistoryTests
{
    [Fact]
    public void ConversionPair_RejectsCategoryMismatch()
    {
        Assert.Throws<InvalidOperationException>(() => new ConversionPair("m", "kg"));
    }

    [Fact]
    public void Record_DeduplicatesAndMovesPairToFront()
    {
        var history = new ConversionPairHistory();
        var metersToKilometers = new ConversionPair("m", "km");
        var metersToCentimeters = new ConversionPair("m", "cm");

        history.Record(metersToKilometers);
        history.Record(metersToCentimeters);
        history.Record(metersToKilometers);

        Assert.Equal([metersToKilometers, metersToCentimeters], history.Recent);
    }

    [Fact]
    public void Record_EnforcesRecentCapacity()
    {
        var history = new ConversionPairHistory(2);

        history.Record(new ConversionPair("m", "km"));
        history.Record(new ConversionPair("m", "cm"));
        history.Record(new ConversionPair("m", "mm"));

        Assert.Equal(2, history.Recent.Count);
        Assert.DoesNotContain(new ConversionPair("m", "km"), history.Recent);
    }

    [Fact]
    public void ToggleFavorite_AddsAndRemovesPair()
    {
        var history = new ConversionPairHistory();
        var pair = new ConversionPair("kg", "g");

        Assert.True(history.ToggleFavorite(pair));
        Assert.True(history.IsFavorite(pair));
        Assert.False(history.ToggleFavorite(pair));
        Assert.False(history.IsFavorite(pair));
    }

    [Fact]
    public void Swap_ReversesPair()
    {
        var pair = new ConversionPair("c", "f");

        Assert.Equal(new ConversionPair("f", "c"), pair.Swap());
    }
}
