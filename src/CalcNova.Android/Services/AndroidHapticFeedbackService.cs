using Android.Content;
using Android.OS;
using CalcNova.Platform.Haptics;

namespace CalcNova.Android.Services;

/// <summary>
/// Haptic feedback backed by the device vibrator.
/// </summary>
/// <remarks>
/// Android has moved the vibrator API twice. API 31 introduced <c>VibratorManager</c> and
/// deprecated <c>Context.VibratorService</c>; API 26 introduced <c>VibrationEffect</c> and
/// deprecated the millisecond <c>Vibrate</c> overload. Both older paths are still the only ones
/// available on the API levels CalcNova supports down to, so each is used on the versions where
/// it is correct rather than calling one everywhere and accepting a deprecation.
///
/// Nothing here throws: a device with no vibrator, or one whose capability is withdrawn at run
/// time, simply produces no feedback. The action that asked for it has already happened.
/// </remarks>
public sealed class AndroidHapticFeedbackService : IHapticFeedbackService
{
    private readonly Vibrator? _vibrator;

    public AndroidHapticFeedbackService(Context context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _vibrator = ResolveVibrator(context);
    }

    /// <inheritdoc />
    public bool IsAvailable => _vibrator?.HasVibrator ?? false;

    /// <inheritdoc />
    public void Perform(HapticFeedbackKind kind)
    {
        var vibrator = _vibrator;
        if (vibrator is null || !vibrator.HasVibrator)
        {
            return;
        }

        var milliseconds = DurationFor(kind);

        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                using var effect = VibrationEffect.CreateOneShot(milliseconds, VibrationEffect.DefaultAmplitude);
                vibrator.Vibrate(effect);
            }
            else
            {
#pragma warning disable CA1422 // The millisecond overload is the only one that exists below API 26.
                vibrator.Vibrate(milliseconds);
#pragma warning restore CA1422
            }
        }
        catch (Java.Lang.Exception)
        {
            // The vibrator can be taken away between the capability check and the call - the
            // device enters a mode that forbids it, or the service dies. Feedback is a courtesy,
            // so losing it is never worth surfacing to the user.
        }
    }

    /// <summary>
    /// Durations chosen to read as distinct without being intrusive: a keypress is the shortest
    /// tick the hardware reliably renders, a completed calculation is a touch longer, and a
    /// rejected one is longer still so it is recognisable without looking.
    /// </summary>
    private static long DurationFor(HapticFeedbackKind kind) => kind switch
    {
        HapticFeedbackKind.Selection => 12L,
        HapticFeedbackKind.Success => 24L,
        HapticFeedbackKind.Warning => 48L,
        _ => 12L
    };

    private static Vibrator? ResolveVibrator(Context context)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            var manager = context.GetSystemService(Context.VibratorManagerService) as VibratorManager;
            return manager?.DefaultVibrator;
        }

#pragma warning disable CA1422 // VibratorService is the only lookup available below API 31.
        return context.GetSystemService(Context.VibratorService) as Vibrator;
#pragma warning restore CA1422
    }
}
