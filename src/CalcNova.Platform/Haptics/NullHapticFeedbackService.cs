namespace CalcNova.Platform.Haptics;

/// <summary>
/// The haptic service used where a platform has no haptics, such as desktop and the browser.
/// </summary>
/// <remarks>
/// Supplied as the default rather than leaving the dependency null, so callers request feedback
/// unconditionally and no call site has to decide whether haptics exist.
/// </remarks>
public sealed class NullHapticFeedbackService : IHapticFeedbackService
{
    /// <summary>The shared instance; the type holds no state.</summary>
    public static NullHapticFeedbackService Instance { get; } = new();

    /// <inheritdoc />
    public bool IsAvailable => false;

    /// <inheritdoc />
    public void Perform(HapticFeedbackKind kind)
    {
        // Deliberately empty: there is nothing to vibrate.
    }
}
