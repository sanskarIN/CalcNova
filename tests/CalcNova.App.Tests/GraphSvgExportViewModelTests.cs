using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphSvgExportViewModelTests
{
    [Fact]
    public void GenerateSvgCommand_ProducesAccessibleSvgMarkup()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2",
            MinimumX = "-2",
            MaximumX = "2",
            SampleCount = 9
        };
        viewModel.PlotCommand.Execute(null);

        viewModel.GenerateSvgCommand.Execute(null);

        Assert.StartsWith("<svg", viewModel.SvgExport, StringComparison.Ordinal);
        Assert.Contains("role=\"img\"", viewModel.SvgExport, StringComparison.Ordinal);
        Assert.Contains("CalcNova graph export", viewModel.SvgExport, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void PlotCommand_ClearsStaleSvgExport()
    {
        var viewModel = new GraphingViewModel();
        viewModel.GenerateSvgCommand.Execute(null);
        Assert.NotEmpty(viewModel.SvgExport);

        viewModel.Expression = "cos(x)";
        viewModel.PlotCommand.Execute(null);

        Assert.Equal(string.Empty, viewModel.SvgExport);
    }
}
