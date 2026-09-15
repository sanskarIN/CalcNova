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
    public void Deserialize_OmittedProperties_KeepTheirDeclaredDefaults()
    {
        // A settings file written before a preference existed omits it entirely, so every
        // property the document does not mention has to come back as its declared default
        // rather than default(T). System.Text.Json's source generator surfaces init-only
        // properties as constructor parameters and would pass 0 or false for each missing
        // one, silently resetting preferences on upgrade; this pins the behaviour the
        // reflection path gives and fails if settings JSON is ever moved onto a generated
        // contract.
        using var document = JsonDocument.Parse(
            """
            {
              "schemaVersion": 1,
              "cultureName": "hi-IN"
            }
            """);

        var settings = AppSettingsJson.Deserialize(document.RootElement, SerializerOptions);
        var declaredDefaults = new AppSettings();

        Assert.Equal("hi-IN", settings.CultureName);
        Assert.Equal(declaredDefaults.DecimalPrecision, settings.DecimalPrecision);
        Assert.Equal(declaredDefaults.HistoryLimit, settings.HistoryLimit);
        Assert.Equal(declaredDefaults.ConverterSignificantDigits, settings.ConverterSignificantDigits);
        Assert.Equal(declaredDefaults.UseGroupingSeparators, settings.UseGroupingSeparators);
        Assert.Equal(declaredDefaults.HapticsEnabled, settings.HapticsEnabled);
        Assert.Equal(declaredDefaults.HistoryEnabled, settings.HistoryEnabled);
        Assert.Equal(declaredDefaults.Theme, settings.Theme);
        Assert.Equal(declaredDefaults.AngleUnit, settings.AngleUnit);
        Assert.NotNull(settings.ConverterRecentPairs);
        Assert.NotNull(settings.ConverterFavoritePairs);
    }

    [Fact]
    public void Serialize_RoundTripsEveryPreference()
    {
        var original = new AppSettings
        {
            CultureName = "hi-IN",
            DecimalPrecision = 9,
            HistoryLimit = 42,
            UseGroupingSeparators = false,
            HapticsEnabled = false,
            HistoryEnabled = false,
            ReducedMotion = true,
            HighContrast = true,
            ConverterSignificantDigits = 12,
            ConverterRecentPairs = ["v1:km>m"],
            ConverterFavoritePairs = ["v1:kg>g"],
            CompletedOnboardingVersion = 3
        };

        var restored = AppSettingsJson.Deserialize(AppSettingsJson.Serialize(original, SerializerOptions), SerializerOptions);

        Assert.NotNull(restored);

        // Compared property by property rather than with record equality: the two array
        // properties are compiler-compared by reference, so a round-tripped instance is
        // never equal to its original however faithful the values are.
        Assert.Equal(original.SchemaVersion, restored.SchemaVersion);
        Assert.Equal(original.Theme, restored.Theme);
        Assert.Equal(original.AngleUnit, restored.AngleUnit);
        Assert.Equal(original.CultureName, restored.CultureName);
        Assert.Equal(original.DecimalPrecision, restored.DecimalPrecision);
        Assert.Equal(original.UseGroupingSeparators, restored.UseGroupingSeparators);
        Assert.Equal(original.HapticsEnabled, restored.HapticsEnabled);
        Assert.Equal(original.HistoryEnabled, restored.HistoryEnabled);
        Assert.Equal(original.HistoryLimit, restored.HistoryLimit);
        Assert.Equal(original.ReducedMotion, restored.ReducedMotion);
        Assert.Equal(original.HighContrast, restored.HighContrast);
        Assert.Equal(original.ConverterSignificantDigits, restored.ConverterSignificantDigits);
        Assert.Equal(original.ConverterRecentPairs, restored.ConverterRecentPairs);
        Assert.Equal(original.ConverterFavoritePairs, restored.ConverterFavoritePairs);
        Assert.Equal(original.CompletedOnboardingVersion, restored.CompletedOnboardingVersion);
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
