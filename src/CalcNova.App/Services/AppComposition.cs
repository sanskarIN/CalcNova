namespace CalcNova.App.Services;

public static class AppComposition
{
    private static AppDependencies _dependencies = AppDependencies.Empty;

    public static AppDependencies Dependencies => _dependencies;

    public static void Configure(AppDependencies dependencies)
    {
        _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
    }

    public static void Reset() => _dependencies = AppDependencies.Empty;
}
