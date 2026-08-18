namespace CalcNova.App.Services;

public static class AppComposition
{
    private static AppDependencies _dependencies = AppDependencies.Empty;

    public static AppDependencies Dependencies => Volatile.Read(ref _dependencies);

    public static void Configure(AppDependencies dependencies)
    {
        ArgumentNullException.ThrowIfNull(dependencies);
        Volatile.Write(ref _dependencies, dependencies);
    }

    public static void Reset() => Volatile.Write(ref _dependencies, AppDependencies.Empty);
}
