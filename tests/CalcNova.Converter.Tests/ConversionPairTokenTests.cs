using CalcNova.Converter;
using Xunit;

namespace CalcNova.Converter.Tests;

public sealed class ConversionPairTokenTests
{
    [Fact]
    public void EncodeAndDecode_RoundTripValidatedPair()
    {
        var pair = new ConversionPair("km", "m");
        var token = ConversionPairToken.Encode(pair);

        Assert.True(ConversionPairToken.TryDecode(token, out var decoded));
        Assert.Equal(pair, decoded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("km>m")]
    [InlineData("v2:km>m")]
    [InlineData("v1:km>")]
    [InlineData("v1:km>m>cm")]
    [InlineData("v1:km>kg")]
    [InlineData("v1:missing>m")]
    public void TryDecode_RejectsMalformedOrInvalidTokens(string? token)
    {
        Assert.False(ConversionPairToken.TryDecode(token, out var pair));
        Assert.Null(pair);
    }
}
