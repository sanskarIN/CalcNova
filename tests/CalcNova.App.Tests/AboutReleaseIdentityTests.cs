using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AboutReleaseIdentityTests
{
    [Fact]
    public void AboutViewModel_ExposesCompletedReleaseIdentity()
    {
        var viewModel = new AboutViewModel(null);

        Assert.Equal("2.9.6", viewModel.Version);
        Assert.Equal("Complete", viewModel.CompletionStatus);
        Assert.Equal("Version 2.9.6 • Complete", viewModel.ReleaseLabel);
    }
}
