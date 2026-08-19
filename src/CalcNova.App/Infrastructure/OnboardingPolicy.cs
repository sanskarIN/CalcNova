namespace CalcNova.App.Infrastructure;

public static class OnboardingPolicy
{
    public const int CurrentVersion = 1;

    public static bool ShouldShow(int completedVersion)
    {
        return NormalizeCompletedVersion(completedVersion) < CurrentVersion;
    }

    public static int MarkCurrentVersionCompleted()
    {
        return CurrentVersion;
    }

    public static int NormalizeCompletedVersion(int completedVersion)
    {
        return Math.Max(0, completedVersion);
    }
}
