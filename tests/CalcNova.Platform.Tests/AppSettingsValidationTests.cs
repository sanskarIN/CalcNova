using System.Text.Json;
using CalcNova.Platform.Settings;
using Xunit;

namespace CalcNova.Platform.Tests;

public sealed class AppSettingsValidationTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Deserialize_UnversionedDocument_MarksLegacySchemaZero()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "cultureName": "hi-IN",
              "historyLimit": 321
            }
            """);

        var settings = AppSettingsJson.Deserialize(document.RootElement, SerializerOptions);

        Assert.Equal(0, settings.SchemaVersion);
        Assert.Equal("hi-IN", settings.CultureName);
        Assert.Equal(321, settings.HistoryLimit);
    }

    [Fact]
    public void Deserialize_SchemaPropertyIsDetectedCaseInsensitively()
    {
        using var document = JsonDocument.Parse(
            """
            {
              "SchemaVersion": 1,
              "cultureName": "en"
            }
            """);

        var settings = AppSettingsJson.Deserialize(document.RootElement, SerializerOptions);

        Assert.Equal(AppSettingsSchema.CurrentVersion, settings.SchemaVersion);
    }

    [Fact]
    public void NormalizeAndValidate_LegacySettings_MigratesAndPreservesPreferences()
    {
        var legacy = new AppSettings
        {
            SchemaVersion = 0,
            CultureName = "hi-IN",
            HistoryLimit = 42,
            ConverterSignificantDigits = 12,
            ConverterRecentPairs = ["v1:km>m"],
            ConverterFavoritePairs = ["v1:kg>g"]
        };

        var normalized = AppSettingsValidator.NormalizeAndValidate(legacy);

        Assert.Equal(AppSettingsSchema.CurrentVersion, normalized.SchemaVersion);
        Assert.Equal("hi-IN", normalized.CultureName);
        Assert.Equal(42, normalized.HistoryLimit);
        Assert.Equal(12, normalized.ConverterSignificantDigits);
        Assert.Equal(["v1:km>m"], normalized.ConverterRecentPairs);
        Assert.Equal(["v1:kg>g"], normalized.ConverterFavoritePairs);
    }

    [Fact]
    public void NormalizeAndValidate_FutureSchema_IsRejected()
    {
        var settings = new AppSettings { SchemaVersion = AppSettingsSchema.CurrentVersion + 1 };

        Assert.Throws<InvalidDataException>(() => AppSettingsValidator.NormalizeAndValidate(settings));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(30)]
    public void NormalizeAndValidate_InvalidDecimalPrecision_IsRejected(int precision)
    {
        var settings = new AppSettings { DecimalPrecision = precision };

        Assert.Throws<InvalidDataException>(() => AppSettingsValidator.NormalizeAndValidate(settings));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5001)]
    public void NormalizeAndValidate_InvalidHistoryLimit_IsRejected(int limit)
    {
        var settings = new AppSettings { HistoryLimit = limit };

        Assert.Throws<InvalidDataException>(() => AppSettingsValidator.NormalizeAndValidate(settings));
    }

    [Fact]
    public void NormalizeAndValidate_InvalidCulture_IsRejected()
    {
        var settings = new AppSettings { CultureName = "not-a-real-culture" };

        Assert.Throws<InvalidDataException>(() => AppSettingsValidator.NormalizeAndValidate(settings));
    }

    [Fact]
    public void NormalizeAndValidate_InvalidConverterToken_IsRejected()
    {
        var settings = new AppSettings { ConverterFavoritePairs = [""] };

        Assert.Throws<InvalidDataException>(() => AppSettingsValidator.NormalizeAndValidate(settings));
    }
}
