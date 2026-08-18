using CalcNova.Platform.Haptics;
using UIKit;

namespace CalcNova.iOS.Services;

internal sealed class IosHapticFeedbackService : IHapticFeedbackService
{
    public void PerformClick()
    {
        using var generator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
        generator.Prepare();
        generator.ImpactOccurred();
    }
}
