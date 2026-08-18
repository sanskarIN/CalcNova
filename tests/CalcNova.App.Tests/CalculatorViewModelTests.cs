using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CalculatorViewModelTests
{
    [Fact]
    public async Task Percentage_AdditionContext_ProducesExpectedResult()
    {
        var viewModel = new CalculatorViewModel
        {
            Expression = "50 + 10"
        };

        viewModel.PercentageCommand.Execute(null);
        await viewModel.EvaluateAsync();

        Assert.Equal("55", viewModel.Result);
    }

    [Fact]
    public async Task RepeatedEquals_ReusesRightOperand()
    {
        var recorded = new List<(string Expression, string Result)>();
        var viewModel = new CalculatorViewModel(
            recordCalculationAsync: (expression, result) =>
            {
                recorded.Add((expression, result));
                return Task.CompletedTask;
            })
        {
            Expression = "2 + 3"
        };

        await viewModel.EvaluateAsync();
        await viewModel.EvaluateAsync();

        Assert.Equal("8", viewModel.Result);
        Assert.Equal(("2 + 3", "5"), recorded[0]);
        Assert.Equal(("repeat(2 + 3)", "8"), recorded[1]);
    }

    [Fact]
    public async Task MemoryStoreAddRecall_UpdatesExpression()
    {
        var viewModel = new CalculatorViewModel
        {
            Expression = "10"
        };
        viewModel.MemoryStoreCommand.Execute(null);
        viewModel.Expression = "5";
        viewModel.MemoryAddCommand.Execute(null);
        viewModel.Clear();

        viewModel.MemoryRecallCommand.Execute(null);
        await viewModel.EvaluateAsync();

        Assert.Equal("15", viewModel.Result);
        Assert.Contains("15", viewModel.MemoryIndicator, StringComparison.Ordinal);
    }

    [Fact]
    public async Task EditingExpression_ResetsRepeatedEqualsSession()
    {
        var viewModel = new CalculatorViewModel { Expression = "2 + 3" };
        await viewModel.EvaluateAsync();
        viewModel.Expression = "10 - 1";

        await viewModel.EvaluateAsync();

        Assert.Equal("9", viewModel.Result);
    }
}
