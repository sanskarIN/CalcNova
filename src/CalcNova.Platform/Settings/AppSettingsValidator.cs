using System.Globalization;

namespace CalcNova.Platform.Settings;

public static class AppSettingsValidator
{
    public static AppSettings NormalizeAndValidate(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings = AppSettingsSchema.Normalize(settings);

        if (!Enum.IsDefined(settings.Theme))
        {
            throw new InvalidDataException("The stored theme preference is invalid.");
        }

        if (!Enum.IsDefined(settings.AngleUnit))
        {
            throw new InvalidDataException("The stored angle unit is invalid.");
        }

        ValidateCultureName(settings.CultureName);

        if (settings.DecimalPrecision is < 1 or > 29)
        {
            throw new InvalidDataException("Decimal precision must be between 1 and 29.");
        }

        if (settings.HistoryLimit is < 1 or > 5000)
        {
            throw new InvalidDataException("History limit must be between 1 and 5000.");
        }

        if (settings.CompletedOnboardingVersion < 0)
        {
            throw new InvalidDataException("The stored onboarding version cannot be negative.");
        }

        if (settings.ConverterSignificantDigits is < 1 or > 17)
        {
            throw new InvalidDataException("Converter precision must be between 1 and 17 significant digits.");
        }

        ValidatePairTokens(settings.ConverterRecentPairs, 12, "recent");
        ValidatePairTokens(settings.ConverterFavoritePairs, 100, "favorite");
        return settings;
    }

    private static void ValidateCultureName(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName) || cultureName.Length > 64)
        {
            throw new InvalidDataException("The stored application culture is invalid.");
        }

        try
        {
            _ = CultureInfo.GetCultureInfo(cultureName);
        }
        catch (CultureNotFoundException exception)
        {
            throw new InvalidDataException("The stored application culture is invalid.", exception);
        }
    }

    private static void ValidatePairTokens(string[]? tokens, int maximumCount, string label)
    {
        if (tokens is null || tokens.Length > maximumCount)
        {
            throw new InvalidDataException($"The stored converter {label} pair list is invalid.");
        }

        if (tokens.Any(token => string.IsNullOrWhiteSpace(token) || token.Length > 128))
        {
            throw new InvalidDataException($"A stored converter {label} pair token is invalid.");
        }
    }
}
