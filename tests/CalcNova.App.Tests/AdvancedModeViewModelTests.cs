using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AdvancedModeViewModelTests
{
    [Fact]
    public void Programmer_ConvertsHexadecimalInput()
    {
        var viewModel = new ProgrammerViewModel
        {
            Input = "FF",
            InputBase = 16,
            WordSize = 8,
            Signed = true
        };

        viewModel.ConvertCommand.Execute(null);

        Assert.Equal("255", viewModel.Decimal);
        Assert.Equal("11111111", viewModel.Binary);
        Assert.Equal("-1", viewModel.InterpretedValue);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void Converter_ConvertsKilometersToMeters()
    {
        var viewModel = new ConverterViewModel
        {
            SelectedCategory = CalcNova.Converter.UnitCategory.Length,
            Input = "1"
        };

        viewModel.FromUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "km");
        viewModel.ToUnit = viewModel.AvailableUnits.Single(unit => unit.Id == "m");
        viewModel.ConvertCommand.Execute(null);

        Assert.Equal("1000 m", viewModel.Result);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void Statistics_AnalyzesDataset()
    {
        var viewModel = new StatisticsViewModel
        {
            DatasetText = "1, 2, 2, 3, 4"
        };

        viewModel.AnalyzeCommand.Execute(null);

        Assert.Contains("Mean: 2.4", viewModel.Summary, StringComparison.Ordinal);
        Assert.Contains("Mode: 2", viewModel.Summary, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void Equations_SolvesQuadratic()
    {
        var viewModel = new EquationsViewModel
        {
            A = "1",
            B = "-5",
            C = "6"
        };

        viewModel.SolveCommand.Execute(null);

        Assert.Contains("2", viewModel.Result, StringComparison.Ordinal);
        Assert.Contains("3", viewModel.Result, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void Matrices_ComputesDeterminant()
    {
        var viewModel = new MatricesViewModel
        {
            MatrixText = "4, 7\n2, 6"
        };

        viewModel.DeterminantCommand.Execute(null);

        Assert.Contains("10", viewModel.Result, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }

    [Fact]
    public void Graphing_SamplesFunction()
    {
        var viewModel = new GraphingViewModel
        {
            Expression = "x ^ 2",
            MinimumX = "-1",
            MaximumX = "1",
            SampleCount = 5
        };

        viewModel.PlotCommand.Execute(null);

        Assert.Single(viewModel.Segments);
        Assert.Contains("5 valid point", viewModel.Summary, StringComparison.Ordinal);
        Assert.Equal(string.Empty, viewModel.ErrorMessage);
    }
}
