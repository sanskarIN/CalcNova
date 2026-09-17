using CalcNova.Platform.Haptics;
using UIKit;

namespace CalcNova.iOS.Services;

/// <summary>
/// Haptic feedback backed by UIKit's feedback generators.
/// </summary>
/// <remarks>
/// The settings checkbox offers haptics on mobile targets, plural, so iOS implements the same
/// contract as Android rather than leaving half of that promise unmet. UIKit's generators map
/// cleanly onto the kinds CalcNova asks for: a keypress is a selection change, an outcome is a
/// notification.
///
/// The selection generator is used rather than an impact generator for two reasons. It is what
/// Apple designed for a discrete choice, which is exactly what pressing a calculator key is; and
/// it is the one that is not deprecated - <c>UIImpactFeedbackGenerator</c>'s style constructor
/// was obsoleted in iOS 17.5 in favour of an overload that needs the <c>UIView</c> the feedback
/// belongs to, which a service composed before any window exists cannot supply.
///
/// Generators are created per call rather than cached. Apple's guidance is to prepare a
/// generator shortly before use; a calculator's presses are far enough apart that holding the
/// Taptic Engine ready between them would cost battery for no perceptible gain.
/// </remarks>
public sealed class IosHapticFeedbackService : IHapticFeedbackService
{
    /// <inheritdoc />
    public bool IsAvailable => true;

    /// <inheritdoc />
    public void Perform(HapticFeedbackKind kind)
    {
        switch (kind)
        {
            case HapticFeedbackKind.Success:
                using (var generator = new UINotificationFeedbackGenerator())
                {
                    generator.Prepare();
                    generator.NotificationOccurred(UINotificationFeedbackType.Success);
                }

                break;

            case HapticFeedbackKind.Warning:
                using (var generator = new UINotificationFeedbackGenerator())
                {
                    generator.Prepare();
                    generator.NotificationOccurred(UINotificationFeedbackType.Warning);
                }

                break;

            default:
                using (var generator = new UISelectionFeedbackGenerator())
                {
                    generator.Prepare();
                    generator.SelectionChanged();
                }

                break;
        }
    }
}
