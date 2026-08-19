using CalcNova.Programmer;
using Xunit;

namespace CalcNova.Programmer.Tests;

public sealed class UnicodeScalarMetadataTests
{
    [Fact]
    public void Describe_BasicLatinLetter_ReportsCategoryAndEncodingWidths()
    {
        var metadata = UnicodeCodePointHelper.Describe(0x41);

        Assert.Equal(0x41, metadata.Value);
        Assert.Equal("U+0041", metadata.CodePoint);
        Assert.Equal("A", metadata.Text);
        Assert.Equal(0, metadata.Plane);
        Assert.Equal("UppercaseLetter", metadata.GeneralCategory);
        Assert.Equal(1, metadata.Utf8ByteCount);
        Assert.Equal(1, metadata.Utf16CodeUnitCount);
    }

    [Fact]
    public void Describe_SupplementaryScalar_ReportsPlaneAndEncodingWidths()
    {
        var metadata = UnicodeCodePointHelper.Describe(0x1F600);

        Assert.Equal("U+1F600", metadata.CodePoint);
        Assert.Equal("😀", metadata.Text);
        Assert.Equal(1, metadata.Plane);
        Assert.Equal("OtherSymbol", metadata.GeneralCategory);
        Assert.Equal(4, metadata.Utf8ByteCount);
        Assert.Equal(2, metadata.Utf16CodeUnitCount);
    }

    [Fact]
    public void DescribeText_EnumeratesScalarsWithoutSplittingSurrogatePairs()
    {
        var metadata = UnicodeCodePointHelper.DescribeText("A😀");

        Assert.Equal(2, metadata.Count);
        Assert.Equal("U+0041", metadata[0].CodePoint);
        Assert.Equal("U+1F600", metadata[1].CodePoint);
    }

    [Fact]
    public void DescribeText_EnforcesExistingInspectionLimit()
    {
        Assert.Throws<ArgumentException>(() => UnicodeCodePointHelper.DescribeText("abc", 2));
    }

    [Fact]
    public void Describe_RejectsSurrogateCodePoint()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => UnicodeCodePointHelper.Describe(0xD800));
    }
}
