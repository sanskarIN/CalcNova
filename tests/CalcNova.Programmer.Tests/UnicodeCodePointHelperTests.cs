using CalcNova.Programmer;
using Xunit;

namespace CalcNova.Programmer.Tests;

public sealed class UnicodeCodePointHelperTests
{
    [Theory]
    [InlineData("U+0041", 0x41)]
    [InlineData("0x03C0", 0x03C0)]
    [InlineData("1F600", 0x1F600)]
    public void Parse_AcceptsCommonHexadecimalForms(string text, int expected)
    {
        Assert.Equal(expected, UnicodeCodePointHelper.Parse(text));
    }

    [Fact]
    public void FormatAndToText_RoundTripSupplementaryScalar()
    {
        const int codePoint = 0x1F600;

        Assert.Equal("U+1F600", UnicodeCodePointHelper.Format(codePoint));
        Assert.Equal("😀", UnicodeCodePointHelper.ToText(codePoint));
    }

    [Fact]
    public void Parse_RejectsSurrogateCodePoint()
    {
        Assert.Throws<FormatException>(() => UnicodeCodePointHelper.Parse("U+D800"));
    }

    [Fact]
    public void GetCodePoints_EnumeratesUnicodeScalarsNotUtf16Units()
    {
        var codePoints = UnicodeCodePointHelper.GetCodePoints("A😀");

        Assert.Equal(["U+0041", "U+1F600"], codePoints);
    }

    [Fact]
    public void GetCodePoints_EnforcesInspectionLimit()
    {
        Assert.Throws<ArgumentException>(() => UnicodeCodePointHelper.GetCodePoints("abc", 2));
    }
}
