using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorTextSymbolInputTests
{
    [Theory]
    [InlineData("×", "*")]
    [InlineData("÷", "/")]
    [InlineData("−", "-")]
    [InlineData("–", "-")]
    [InlineData("—", "-")]
    [InlineData("·", "*")]
    [InlineData("∙", "*")]
    public void TryGetToken_KnownCalculatorGlyph_ReturnsCanonicalToken(string glyph, string expected)
    {
        Assert.True(CalculatorTextSymbolInput.TryGetToken(glyph, out var token));
        Assert.Equal(expected, token);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("+")]
    [InlineData("*")]
    [InlineData("1")]
    [InlineData("pi")]
    public void TryGetToken_AsciiOrUnknownText_IsNotCaptured(string? text)
    {
        Assert.False(CalculatorTextSymbolInput.TryGetToken(text, out var token));
        Assert.Equal(string.Empty, token);
    }
}
