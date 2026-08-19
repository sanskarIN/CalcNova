using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphAnalysisViewModelTests
{
    [Fact]
    public void DerivativeCommand_ReportsSlopeAtRequestedPoint()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2",
            AnalysisX = "3"
        };

        viewModel.DerivativeCommand.Execute(null);

        Assert.Contains("6", viewModel.AnalysisResult, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void FindRootCommand_UsesVisibleGraphInterval()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2 - 2",
            MinimumX = "1",
            MaximumX = "2"
        };

        viewModel.FindRootCommand.Execute(null);

        Assert.Contains("1.414", viewModel.AnalysisResult, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void IntegrateCommand_UsesVisibleGraphInterval()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2",
            MinimumX = "0",
            MaximumX = "3"
        };

        viewModel.IntegrateCommand.Execute(null);

        Assert.Contains("9", viewModel.AnalysisResult, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void FindRootCommand_ReportsUnbracketedRootError()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2 + 1",
            MinimumX = "-1",
            MaximumX = "1"
        };

        viewModel.FindRootCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.AnalysisResult);
        Assert.Contains("sign change", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
