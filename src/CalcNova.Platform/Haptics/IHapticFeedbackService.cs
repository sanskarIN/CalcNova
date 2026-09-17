namespace CalcNova.Platform.Haptics;

/// <summary>
/// Short tactile confirmations for touch input.
/// </summary>
/// <remarks>
/// Implementations must never throw and must never block: haptics are a courtesy on top of an
/// action that has already happened, so a device that cannot vibrate, or has had the capability
/// revoked, has to degrade to doing nothing rather than failing the action that asked for it.
/// </remarks>
public interface IHapticFeedbackService
{
    /// <summary>Whether this device can actually produce haptic feedback.</summary>
    bool IsAvailable { get; }

    /// <summary>Requests one short confirmation. Does nothing when unavailable.</summary>
    void Perform(HapticFeedbackKind kind);
}
