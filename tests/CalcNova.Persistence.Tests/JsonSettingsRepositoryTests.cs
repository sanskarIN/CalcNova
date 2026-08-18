using CalcNova.Core.Evaluation;
using CalcNova.Persistence.Settings;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.Persistence.Tests;

public sealed class JsonSettingsRepositoryTests : IAsyncLifetime
{
    private readonly string _filePath = Path.Combine(Path.GetTempPath(), $"calcnova-settings-{Guid.NewGuid():N}.json");

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public ValueTask DisposeAsync()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }

        if (File.Exists(_filePath + ".tmp"))
        {
            File.Delete(_filePath + ".tmp");
        }

        return ValueTask.CompletedTask;
    }

    [Fact]
    public async Task Load_WhenMissing_ReturnsDefaults()
    {
        var repository = new JsonSettingsRepository(_filePath);

        var settings = await repository.LoadAsync(TestContext.Current.CancellationToken);

        Assert.Equal(ThemePreference.System, settings.Theme);
        Assert.Equal(AngleUnit.Degrees, settings.AngleUnit);
        Assert.True(settings.HistoryEnabled);
    }

    [Fact]
    public async Task SaveAndLoad_RoundTripsSettings()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var expected = new AppSettings
        {
            Theme = ThemePreference.Dark,
            AngleUnit = AngleUnit.Radians,
            DecimalPrecision = 20,
            UseGroupingSeparators = false,
            HapticsEnabled = false,
            HistoryEnabled = true,
            HistoryLimit = 250,
            ReducedMotion = true,
            HighContrast = true
        };

        await repository.SaveAsync(expected, TestContext.Current.CancellationToken);
        var actual = await repository.LoadAsync(TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Save_InvalidSettings_IsRejected()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings { HistoryLimit = 0 };

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            repository.SaveAsync(invalid, TestContext.Current.CancellationToken));
    }
}
