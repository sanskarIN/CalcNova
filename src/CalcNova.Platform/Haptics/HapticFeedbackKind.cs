namespace CalcNova.Platform.Haptics;

/// <summary>
/// The kinds of tactile confirmation CalcNova asks a device for.
/// </summary>
/// <remarks>
/// These name what happened, not how strongly to buzz. Each platform maps them onto its own
/// conventions - Android to vibration effects, iOS to its feedback generators - so a
/// selection feels the same as a selection elsewhere on the device.
/// </remarks>
public enum HapticFeedbackKind
{
    /// <summary>A key or control was pressed.</summary>
    Selection = 0,

    /// <summary>An action completed, such as a successful evaluation.</summary>
    Success,

    /// <summary>An action was rejected, such as an expression that would not parse.</summary>
    Warning
}
