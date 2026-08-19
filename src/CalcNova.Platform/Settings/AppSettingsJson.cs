using System.Text.Json;

namespace CalcNova.Platform.Settings;

public static class AppSettingsJson
{
    public static AppSettings Deserialize(JsonElement root, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var settings = root.Deserialize<AppSettings>(options) ?? new AppSettings();
        var hasSchemaVersion = root
            .EnumerateObject()
            .Any(property => string.Equals(property.Name, "schemaVersion", StringComparison.OrdinalIgnoreCase));

        return hasSchemaVersion
            ? settings
            : settings with { SchemaVersion = 0 };
    }
}
