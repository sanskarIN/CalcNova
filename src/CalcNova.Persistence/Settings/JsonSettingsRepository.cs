using System.Text.Json;
using CalcNova.Platform.Settings;

namespace CalcNova.Persistence.Settings;

public sealed class JsonSettingsRepository : ISettingsRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly string _filePath;

    public JsonSettingsRepository(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    public async Task<AppSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return new AppSettings();
        }

        await using var stream = new FileStream(
            _filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        var settings = await JsonSerializer.DeserializeAsync<AppSettings>(stream, SerializerOptions, cancellationToken);
        return Validate(settings ?? new AppSettings());
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings = Validate(settings);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = _filePath + ".tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                FileOptions.Asynchronous | FileOptions.WriteThrough))
            {
                await JsonSerializer.SerializeAsync(stream, settings, SerializerOptions, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }

            File.Move(temporaryPath, _filePath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private static AppSettings Validate(AppSettings settings)
    {
        if (!Enum.IsDefined(settings.Theme))
        {
            throw new InvalidDataException("The stored theme preference is invalid.");
        }

        if (!Enum.IsDefined(settings.AngleUnit))
        {
            throw new InvalidDataException("The stored angle unit is invalid.");
        }

        if (settings.DecimalPrecision is < 1 or > 29)
        {
            throw new InvalidDataException("Decimal precision must be between 1 and 29.");
        }

        if (settings.HistoryLimit is < 1 or > 5000)
        {
            throw new InvalidDataException("History limit must be between 1 and 5000.");
        }

        ValidateConverterState(settings);
        return settings;
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
