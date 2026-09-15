using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace CalcNova.Platform.Settings;

/// <summary>
/// The single place settings JSON is read and written, for every head.
/// </summary>
/// <remarks>
/// <para>
/// This deliberately uses the reflection-based <see cref="JsonSerializer"/> overloads.
/// <see cref="AppSettings"/> declares init-only properties with initializer defaults, and
/// System.Text.Json's source generator surfaces init-only properties as constructor
/// parameters: it emits a creator that passes <c>default(T)</c> for every property the
/// document does not contain. A settings file written before a preference existed would
/// therefore load that preference as 0 or false instead of its declared default, silently
/// resetting it on upgrade. The reflection path constructs the object first - running the
/// initializers - and overwrites only what the document actually contains.
/// </para>
/// <para>
/// That correctness is what the trimmer cannot see, so it is stated explicitly here instead:
/// <see cref="DynamicDependencyAttribute"/> roots the members the serializer reaches, which
/// is exactly what the warning asks for, and the suppression records that it has been done.
/// Keeping every settings call in this one type means that reasoning lives in one place
/// rather than being repeated at each head's storage layer.
/// </para>
/// </remarks>
public static class AppSettingsJson
{
    private const string TrimJustification =
        "AppSettings is rooted by DynamicDependency, so the members the serializer reaches survive trimming. " +
        "The source-generated contract cannot be used here because it drops initializer defaults for init-only properties.";

    public static AppSettings Deserialize(JsonElement root, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var settings = DeserializeElement(root, options) ?? new AppSettings();
        var hasSchemaVersion = root
            .EnumerateObject()
            .Any(property => string.Equals(property.Name, "schemaVersion", StringComparison.OrdinalIgnoreCase));

        return hasSchemaVersion
            ? settings
            : settings with { SchemaVersion = 0 };
    }

    /// <summary>Reads settings from a JSON string, returning null for a null literal.</summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSettings))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = TrimJustification)]
    public static AppSettings? Deserialize(string json, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return JsonSerializer.Deserialize<AppSettings>(json, options);
    }

    /// <summary>Reads settings from a stream, returning null for a null literal.</summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSettings))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = TrimJustification)]
    public static ValueTask<AppSettings?> DeserializeAsync(
        Stream stream,
        JsonSerializerOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        return JsonSerializer.DeserializeAsync<AppSettings>(stream, options, cancellationToken);
    }

    /// <summary>Writes settings to a JSON string.</summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSettings))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = TrimJustification)]
    public static string Serialize(AppSettings settings, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(options);
        return JsonSerializer.Serialize(settings, options);
    }

    /// <summary>Writes settings to a stream.</summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSettings))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = TrimJustification)]
    public static Task SerializeAsync(
        Stream stream,
        AppSettings settings,
        JsonSerializerOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(options);
        return JsonSerializer.SerializeAsync(stream, settings, options, cancellationToken);
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(AppSettings))]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = TrimJustification)]
    private static AppSettings? DeserializeElement(JsonElement root, JsonSerializerOptions options) =>
        root.Deserialize<AppSettings>(options);
}
