using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class UnitConverterTests
{
    private readonly UnitConverter _converter = new();

    [Theory]
    [InlineData(1d, "km", "m", 1000d)]
    [InlineData(1d, "in", "mm", 25.4d)]
    [InlineData(1d, "mi", "ft", 5280d)]
    [InlineData(1d, "lb", "kg", 0.45359237d)]
    [InlineData(1d, "kwh", "j", 3600000d)]
    [InlineData(1d, "byte", "bit", 8d)]
    public void Convert_KnownIdentity_ReturnsExpectedValue(double input, string from, string to, double expected)
    {
        var result = _converter.Convert(input, from, to);

        Assert.InRange(result, expected - 1e-9, expected + 1e-9);
    }

    [Theory]
    [InlineData(0d, "c", "f", 32d)]
    [InlineData(100d, "c", "f", 212d)]
    [InlineData(32d, "f", "c", 0d)]
    [InlineData(0d, "c", "k", 273.15d)]
    public void Convert_Temperature_HandlesOffsets(double input, string from, string to, double expected)
    {
        var result = _converter.Convert(input, from, to);

        Assert.InRange(result, expected - 1e-9, expected + 1e-9);
    }

    [Fact]
    public void Convert_DifferentCategories_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _converter.Convert(1d, "m", "kg"));
    }

    [Fact]
    public void Search_FindsSymbolAndName()
    {
        var matches = _converter.Search("mile", UnitCategory.Length);

        Assert.Contains(matches, unit => unit.Id == "mi");
        Assert.Contains(matches, unit => unit.Id == "nmi");
    }
}
