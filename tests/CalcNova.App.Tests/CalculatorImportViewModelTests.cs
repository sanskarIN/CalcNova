using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorImportViewModelTests
{
    [Fact]
    public async Task ImportExpression_NormalizesThenEvaluatesCalculatorGlyphs()
    {
        var viewModel = new CalculatorViewModel();

        viewModel.ImportExpression("= 2 × (3 + 4)");
        await viewModel.EvaluateAsync();

        Assert.Equal("2 * (3 + 4)", viewModel.Expression);
        Assert.Equal("14", viewModel.Result);
        Assert.Equal(string.Empty, viewModel.StatusMessage);
    }

    [Fact]
    public void ImportExpression_RejectsUnsupportedTextWithoutReplacingCurrentExpression()
    {
        var viewModel = new CalculatorViewModel
        {
            Expression = "1 + 1"
        };

        viewModel.ImportExpression("2 @ 3");

        Assert.Equal("1 + 1", viewModel.Expression);
        Assert.Contains("unsupported character", viewModel.StatusMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ImportExpressionCommand_UsesSameSanitizationPath()
    {
        var viewModel = new CalculatorViewModel();

        viewModel.ImportExpressionCommand.Execute("π × 2");

        Assert.Equal("pi * 2", viewModel.Expression);
        Assert.Equal(string.Empty, viewModel.StatusMessage);
    }
}
