using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class ConverterClearRecentViewModelTests
{
    [Fact]
    public void ClearRecentCommand_ClearsRecordedPairsAndRaisesPersistenceChange()
    {
        var viewModel = new ConverterViewModel();
        viewModel.ConvertCommand.Execute(null);
        var persistenceChanges = 0;
        viewModel.PersistenceStateChanged += () => persistenceChanges++;

        viewModel.ClearRecentCommand.Execute(null);

        Assert.Empty(viewModel.RecentPairs);
        Assert.Equal(1, persistenceChanges);
        Assert.Contains("cleared", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ClearRecentCommand_DoesNotPersistWhenAlreadyEmpty()
    {
        var viewModel = new ConverterViewModel();
        var persistenceChanges = 0;
        viewModel.PersistenceStateChanged += () => persistenceChanges++;

        viewModel.ClearRecentCommand.Execute(null);

        Assert.Equal(0, persistenceChanges);
        Assert.Contains("already empty", viewModel.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
