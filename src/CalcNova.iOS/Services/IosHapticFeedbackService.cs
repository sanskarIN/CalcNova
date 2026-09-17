using CalcNova.Platform.Haptics;
using UIKit;

namespace CalcNova.iOS.Services;

/// <summary>
/// Haptic feedback backed by UIKit's feedback generators.
/// </summary>
/// <remarks>
/// The settings checkbox offers haptics on mobile targets, plural, so iOS implements the same
/// contract as Android rather than leaving half of that promise unmet. UIKit distinguishes
/// impact from notification feedback, which maps cleanly onto the two kinds CalcNova asks for:
/// a keypress is an impact, an outcome is a notification.
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
                using (var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light))
                {
                    generator.Prepare();
                    generator.ImpactOccurred();
                }

                break;
        }
    }
}
