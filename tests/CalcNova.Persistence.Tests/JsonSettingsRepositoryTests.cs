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

        var settings = await repository.LoadAsync();

        Assert.Equal(ThemePreference.System, settings.Theme);
        Assert.Equal(AngleUnit.Degrees, settings.AngleUnit);
        Assert.Equal("en", settings.CultureName);
        Assert.True(settings.HistoryEnabled);
        Assert.Equal(15, settings.ConverterSignificantDigits);
        Assert.Empty(settings.ConverterRecentPairs);
        Assert.Empty(settings.ConverterFavoritePairs);
        Assert.Equal(0, settings.CompletedOnboardingVersion);
    }

    [Fact]
    public async Task SaveAndLoad_RoundTripsSettings()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var expected = new AppSettings
        {
            Theme = ThemePreference.Dark,
            AngleUnit = AngleUnit.Radians,
            CultureName = "en-IN",
            DecimalPrecision = 20,
            UseGroupingSeparators = false,
            HapticsEnabled = false,
            HistoryEnabled = true,
            HistoryLimit = 250,
            ReducedMotion = true,
            HighContrast = true,
            ConverterSignificantDigits = 12,
            ConverterRecentPairs = ["v1:km>m", "v1:kg>g"],
            ConverterFavoritePairs = ["v1:c>f"],
            CompletedOnboardingVersion = 1
        };

        await repository.SaveAsync(expected);
        var actual = await repository.LoadAsync();

        Assert.Equal(expected.Theme, actual.Theme);
        Assert.Equal(expected.AngleUnit, actual.AngleUnit);
        Assert.Equal(expected.CultureName, actual.CultureName);
        Assert.Equal(expected.DecimalPrecision, actual.DecimalPrecision);
        Assert.Equal(expected.UseGroupingSeparators, actual.UseGroupingSeparators);
        Assert.Equal(expected.HapticsEnabled, actual.HapticsEnabled);
        Assert.Equal(expected.HistoryEnabled, actual.HistoryEnabled);
        Assert.Equal(expected.HistoryLimit, actual.HistoryLimit);
        Assert.Equal(expected.ReducedMotion, actual.ReducedMotion);
        Assert.Equal(expected.HighContrast, actual.HighContrast);
        Assert.Equal(expected.ConverterSignificantDigits, actual.ConverterSignificantDigits);
        Assert.Equal(expected.ConverterRecentPairs, actual.ConverterRecentPairs);
        Assert.Equal(expected.ConverterFavoritePairs, actual.ConverterFavoritePairs);
        Assert.Equal(expected.CompletedOnboardingVersion, actual.CompletedOnboardingVersion);
    }

    [Fact]
    public async Task Save_InvalidSettings_IsRejected()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings { HistoryLimit = 0 };

        await Assert.ThrowsAsync<InvalidDataException>(() => repository.SaveAsync(invalid));
    }

    [Fact]
    public async Task Save_InvalidConverterPrecision_IsRejected()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings { ConverterSignificantDigits = 18 };

        await Assert.ThrowsAsync<InvalidDataException>(() => repository.SaveAsync(invalid));
    }

    [Fact]
    public async Task Save_OversizedConverterRecentList_IsRejected()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings
        {
            ConverterRecentPairs = Enumerable.Range(0, 13).Select(index => $"v1:m>cm{index}").ToArray()
        };

        await Assert.ThrowsAsync<InvalidDataException>(() => repository.SaveAsync(invalid));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-real-culture")]
    public async Task Save_InvalidCultureName_IsRejected(string cultureName)
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings { CultureName = cultureName };

        await Assert.ThrowsAsync<InvalidDataException>(() => repository.SaveAsync(invalid));
    }

    [Fact]
    public async Task Save_NegativeOnboardingVersion_IsRejected()
    {
        var repository = new JsonSettingsRepository(_filePath);
        var invalid = new AppSettings { CompletedOnboardingVersion = -1 };

        await Assert.ThrowsAsync<InvalidDataException>(() => repository.SaveAsync(invalid));
    }
}
