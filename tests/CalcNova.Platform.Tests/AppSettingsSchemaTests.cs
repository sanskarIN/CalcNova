using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.Platform.Tests;

public sealed class AppSettingsSchemaTests
{
    [Fact]
    public void NewSettings_UseCurrentSchemaVersion()
    {
        var settings = new AppSettings();

        Assert.Equal(AppSettingsSchema.CurrentVersion, settings.SchemaVersion);
    }

    [Fact]
    public void Normalize_LegacyUnversionedSettings_MigratesToCurrentVersion()
    {
        var legacy = new AppSettings { SchemaVersion = 0, CultureName = "hi-IN", HistoryLimit = 42 };

        var migrated = AppSettingsSchema.Normalize(legacy);

        Assert.Equal(AppSettingsSchema.CurrentVersion, migrated.SchemaVersion);
        Assert.Equal("hi-IN", migrated.CultureName);
        Assert.Equal(42, migrated.HistoryLimit);
    }

    [Fact]
    public void Normalize_CurrentSettings_PreservesInstance()
    {
        var settings = new AppSettings();

        Assert.Same(settings, AppSettingsSchema.Normalize(settings));
    }

    [Fact]
    public void Normalize_FutureSchema_RejectsUnsafeDowngrade()
    {
        var future = new AppSettings { SchemaVersion = AppSettingsSchema.CurrentVersion + 1 };

        Assert.Throws<InvalidDataException>(() => AppSettingsSchema.Normalize(future));
    }

    [Fact]
    public void Normalize_NegativeSchema_RejectsCorruptState()
    {
        var corrupt = new AppSettings { SchemaVersion = -1 };

        Assert.Throws<InvalidDataException>(() => AppSettingsSchema.Normalize(corrupt));
    }
}
