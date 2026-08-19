using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CodePointMetadataViewModelTests
{
    [Fact]
    public void DecodeCodePointCommand_ProjectsLocalScalarMetadata()
    {
        var viewModel = new CodePointViewModel
        {
            CodePointInput = "U+1F600"
        };

        viewModel.DecodeCodePointCommand.Execute(null);

        Assert.Contains("U+1F600", viewModel.CodePointMetadata, StringComparison.Ordinal);
        Assert.Contains("plane 1", viewModel.CodePointMetadata, StringComparison.Ordinal);
        Assert.Contains("OtherSymbol", viewModel.CodePointMetadata, StringComparison.Ordinal);
        Assert.Contains("UTF-8 4 byte", viewModel.CodePointMetadata, StringComparison.Ordinal);
        Assert.Contains("UTF-16 2 unit", viewModel.CodePointMetadata, StringComparison.Ordinal);
    }

    [Fact]
    public void InspectTextCommand_ProjectsOneMetadataLinePerScalar()
    {
        var viewModel = new CodePointViewModel
        {
            TextInput = "A😀"
        };

        viewModel.InspectTextCommand.Execute(null);

        var lines = viewModel.TextMetadata.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.Contains("U+0041", lines[0], StringComparison.Ordinal);
        Assert.Contains("U+1F600", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public void InvalidScalar_ClearsPreviouslyProjectedMetadata()
    {
        var viewModel = new CodePointViewModel
        {
            CodePointInput = "U+03C0"
        };
        viewModel.DecodeCodePointCommand.Execute(null);
        Assert.NotEmpty(viewModel.CodePointMetadata);

        viewModel.CodePointInput = "U+D800";
        viewModel.DecodeCodePointCommand.Execute(null);

        Assert.Empty(viewModel.CodePointMetadata);
        Assert.NotEmpty(viewModel.ErrorMessage);
    }
}
