using Android.App;
using Android.Views;
using CalcNova.Platform.Haptics;

namespace CalcNova.Android.Services;

internal sealed class AndroidHapticFeedbackService : IHapticFeedbackService
{
    private readonly WeakReference<Activity> _activity;

    public AndroidHapticFeedbackService(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        _activity = new WeakReference<Activity>(activity);
    }

    public void PerformClick()
    {
        if (!_activity.TryGetTarget(out var activity) || activity.IsFinishing || activity.IsDestroyed)
        {
            return;
        }

        activity.Window?.DecorView?.PerformHapticFeedback(FeedbackConstants.KeyboardTap);
    }
}
