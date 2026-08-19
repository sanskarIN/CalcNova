using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CodePointViewModelTests
{
    [Fact]
    public void DecodeCodePointCommand_ProducesCanonicalScalarAndText()
    {
        var viewModel = new CodePointViewModel
        {
            CodePointInput = "U+1F600"
        };

        viewModel.DecodeCodePointCommand.Execute(null);

        Assert.Equal("U+1F600 → 😀", viewModel.CodePointResult);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void InspectTextCommand_EnumeratesUnicodeScalars()
    {
        var viewModel = new CodePointViewModel
        {
            TextInput = "A😀"
        };

        viewModel.InspectTextCommand.Execute(null);

        Assert.Equal("U+0041  U+1F600", viewModel.TextResult);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void DecodeCodePointCommand_ReportsInvalidScalar()
    {
        var viewModel = new CodePointViewModel
        {
            CodePointInput = "U+D800"
        };

        viewModel.DecodeCodePointCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.CodePointResult);
        Assert.NotEqual(string.Empty, viewModel.ErrorMessage);
    }
}
