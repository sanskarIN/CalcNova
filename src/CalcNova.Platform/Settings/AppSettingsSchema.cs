namespace CalcNova.Platform.Settings;

public static class AppSettingsSchema
{
    public const int CurrentVersion = 1;

    public static AppSettings Normalize(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return settings.SchemaVersion switch
        {
            0 => settings with { SchemaVersion = CurrentVersion },
            CurrentVersion => settings,
            < 0 => throw new InvalidDataException("The stored settings schema version cannot be negative."),
            _ => throw new InvalidDataException(
                $"Settings schema version {settings.SchemaVersion} is newer than supported version {CurrentVersion}.")
        };
    }
}
