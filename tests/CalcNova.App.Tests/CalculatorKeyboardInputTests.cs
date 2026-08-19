using Avalonia.Input;
using CalcNova.App.Infrastructure;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorKeyboardInputTests
{
    [Theory]
    [InlineData(Key.D0, "0")]
    [InlineData(Key.D5, "5")]
    [InlineData(Key.D9, "9")]
    [InlineData(Key.NumPad0, "0")]
    [InlineData(Key.NumPad5, "5")]
    [InlineData(Key.NumPad9, "9")]
    [InlineData(Key.Add, "+")]
    [InlineData(Key.Subtract, "-")]
    [InlineData(Key.Multiply, "*")]
    [InlineData(Key.Divide, "/")]
    [InlineData(Key.Decimal, ".")]
    public void TryGetToken_KnownCalculatorKey_ReturnsCanonicalToken(Key key, string expected)
    {
        var mapped = CalculatorKeyboardInput.TryGetToken(key, out var token);

        Assert.True(mapped);
        Assert.Equal(expected, token);
    }

    [Theory]
    [InlineData(Key.A)]
    [InlineData(Key.Tab)]
    [InlineData(Key.PageDown)]
    [InlineData(Key.Escape)]
    public void TryGetToken_UnmappedKey_ReturnsFalse(Key key)
    {
        var mapped = CalculatorKeyboardInput.TryGetToken(key, out var token);

        Assert.False(mapped);
        Assert.Equal(string.Empty, token);
    }
}
