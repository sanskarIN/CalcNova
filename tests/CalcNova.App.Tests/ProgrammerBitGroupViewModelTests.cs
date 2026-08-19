using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ProgrammerBitGroupViewModelTests
{
    [Theory]
    [InlineData(8, 1)]
    [InlineData(16, 2)]
    [InlineData(32, 4)]
    [InlineData(64, 8)]
    [InlineData(128, 16)]
    public void BitGroups_MatchSelectedWordSize(int wordSize, int expectedGroups)
    {
        var viewModel = new ProgrammerViewModel
        {
            WordSize = wordSize,
            Input = "0",
            InputBase = 10
        };
        viewModel.ConvertCommand.Execute(null);

        Assert.Equal(expectedGroups, viewModel.BitGroups.Count);
        Assert.All(viewModel.BitGroups, group => Assert.Equal(8, group.Bits.Count));
        Assert.Equal(expectedGroups - 1, viewModel.BitGroups[0].ByteIndex);
        Assert.Equal(0, viewModel.BitGroups[^1].ByteIndex);
    }

    [Fact]
    public void BitGroup_KeepsMostSignificantBitFirstWithinByte()
    {
        var viewModel = new ProgrammerViewModel { WordSize = 16 };

        var mostSignificantByte = viewModel.BitGroups[0];

        Assert.Equal("Byte 1", mostSignificantByte.Label);
        Assert.Equal(15, mostSignificantByte.Bits[0].Index);
        Assert.Equal(8, mostSignificantByte.Bits[^1].Index);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(24)]
    [InlineData(256)]
    public void WordSize_RejectsUnsupportedUiPreset(int wordSize)
    {
        var viewModel = new ProgrammerViewModel();

        Assert.Throws<ArgumentOutOfRangeException>(() => viewModel.WordSize = wordSize);
    }
}
