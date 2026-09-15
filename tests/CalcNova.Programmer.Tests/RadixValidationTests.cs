using Xunit;

namespace CalcNova.Programmer.Tests;

public sealed class RadixValidationTests
{
    [Theory]
    [InlineData("_")]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("___")]
    public void Parse_InputWithoutDigits_ThrowsFormatException(string input)
    {
        Assert.Throws<FormatException>(() => RadixConverter.Parse(input, 16));
    }

    [Theory]
    [InlineData("2", 2)]
    [InlineData("G", 16)]
    public void Parse_InvalidDigitForBase_ThrowsFormatException(string input, int radix)
    {
        Assert.Throws<FormatException>(() => RadixConverter.Parse(input, radix));
    }
}
