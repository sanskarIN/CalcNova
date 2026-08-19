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
    [InlineData(Key.OemMinus, "-")]
    [InlineData(Key.OemQuestion, "/")]
    [InlineData(Key.OemPeriod, ".")]
    [InlineData(Key.OemComma, ",")]
    public void TryGetToken_KnownCalculatorKey_ReturnsCanonicalToken(Key key, string expected)
    {
        var mapped = CalculatorKeyboardInput.TryGetToken(key, out var token);

        Assert.True(mapped);
        Assert.Equal(expected, token);
    }

    [Theory]
    [InlineData(Key.OemPlus, "+")]
    [InlineData(Key.D8, "*")]
    [InlineData(Key.D9, "(")]
    [InlineData(Key.D0, ")")]
    [InlineData(Key.D6, "^")]
    [InlineData(Key.D5, "%")]
    public void TryGetModifiedToken_ShiftOperator_ReturnsCanonicalToken(Key key, string expected)
    {
        var mapped = CalculatorKeyboardInput.TryGetModifiedToken(key, KeyModifiers.Shift, out var token);

        Assert.True(mapped);
        Assert.Equal(expected, token);
    }

    [Theory]
    [InlineData(Key.OemPlus, KeyModifiers.Control)]
    [InlineData(Key.OemPlus, KeyModifiers.Alt)]
    [InlineData(Key.D8, KeyModifiers.Control | KeyModifiers.Shift)]
    [InlineData(Key.A, KeyModifiers.Shift)]
    public void TryGetModifiedToken_UnsafeOrUnknownModifierCombination_IsNotCaptured(Key key, KeyModifiers modifiers)
    {
        var mapped = CalculatorKeyboardInput.TryGetModifiedToken(key, modifiers, out var token);

        Assert.False(mapped);
        Assert.Equal(string.Empty, token);
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
