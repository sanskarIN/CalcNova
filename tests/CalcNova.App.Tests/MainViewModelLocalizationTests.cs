using CalcNova.App.Localization;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class MainViewModelLocalizationTests
{
    [Fact]
    public void Constructor_UsesInjectedLocalizer()
    {
        var localizer = new AppLocalizer("en-IN");
        var dependencies = new AppDependencies(null, null)
        {
            Localizer = localizer
        };

        var viewModel = new MainViewModel(dependencies);

        Assert.Same(localizer, viewModel.Localizer);
        Assert.Equal("en-IN", viewModel.Localizer.Culture.Name);
    }

    [Fact]
    public void Constructor_ProvidesEnglishLocalizerWhenDependencyIsAbsent()
    {
        var viewModel = new MainViewModel();

        Assert.Equal("en", viewModel.Localizer.Culture.Name);
        Assert.Equal("CalcNova", viewModel.Localizer[AppStringKey.AppName]);
    }
}
