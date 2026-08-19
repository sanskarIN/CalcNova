using System.Globalization;
using System.Text.Json;
using CalcNova.Platform.Settings;

namespace CalcNova.Browser.Services;

public sealed class BrowserSettingsRepository : ISettingsRepository
{
    private const string StorageKey = "calcnova.settings.v1";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            var json = BrowserInterop.GetItem(StorageKey);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new AppSettings();
            }

            try
            {
                return Validate(JsonSerializer.Deserialize<AppSettings>(json, SerializerOptions) ?? new AppSettings());
            }
            catch (JsonException)
            {
                return new AppSettings();
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings = Validate(settings);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            await BrowserInterop.EnsureInitializedAsync(cancellationToken);
            BrowserInterop.SetItem(StorageKey, JsonSerializer.Serialize(settings, SerializerOptions));
        }
        finally
        {
            _gate.Release();
        }
    }

    private static AppSettings Validate(AppSettings settings)
    {
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

        ValidateConverterState(settings);
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

    private static void ValidateConverterState(AppSettings settings)
    {
        if (settings.ConverterSignificantDigits is < 1 or > 17)
        {
            throw new InvalidDataException("Converter precision must be between 1 and 17 significant digits.");
        }

        ValidatePairTokens(settings.ConverterRecentPairs, 12, "recent");
        ValidatePairTokens(settings.ConverterFavoritePairs, 100, "favorite");
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
