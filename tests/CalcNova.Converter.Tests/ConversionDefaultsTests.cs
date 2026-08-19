using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class ConversionDefaultsTests
{
    [Fact]
    public void EveryUnitCategory_HasAValidDefaultPair()
    {
        foreach (var category in Enum.GetValues<UnitCategory>())
        {
            var pair = ConversionDefaults.ForCategory(category);

            Assert.Equal(category, pair.Category);
            Assert.NotEqual(pair.FromUnitId, pair.ToUnitId);
            Assert.Equal(category, UnitCatalog.Get(pair.FromUnitId).Category);
            Assert.Equal(category, UnitCatalog.Get(pair.ToUnitId).Category);
        }
    }

    [Theory]
    [InlineData(UnitCategory.Length, "m", "km")]
    [InlineData(UnitCategory.Temperature, "c", "f")]
    [InlineData(UnitCategory.Speed, "kmh", "mph")]
    [InlineData(UnitCategory.Angle, "deg", "rad")]
    public void RepresentativeCategories_UseExpectedDefaults(UnitCategory category, string fromId, string toId)
    {
        var pair = ConversionDefaults.ForCategory(category);

        Assert.Equal(fromId, pair.FromUnitId);
        Assert.Equal(toId, pair.ToUnitId);
    }

    [Fact]
    public void UnknownCategory_IsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ConversionDefaults.ForCategory((UnitCategory)999));
        Assert.False(ConversionDefaults.TryGet((UnitCategory)999, out _));
    }
}
