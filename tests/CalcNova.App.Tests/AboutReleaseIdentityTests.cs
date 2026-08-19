using CalcNova.App.ViewModels;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class AboutReleaseIdentityTests
{
    [Fact]
    public void AboutViewModel_ExposesCompletedReleaseIdentity()
    {
        var viewModel = new AboutViewModel(null);

        Assert.Equal("2.8.03", viewModel.Version);
        Assert.Equal("Complete", viewModel.CompletionStatus);
        Assert.Equal("Version 2.8.03 • Complete", viewModel.ReleaseLabel);
    }
}
