namespace CalcNova.Equations;

public sealed record RootFindingResult(
    bool Success,
    double? Root,
    int Iterations,
    string? ErrorMessage)
{
    public static RootFindingResult Found(double root, int iterations) => new(true, root, iterations, null);

    public static RootFindingResult Failed(string message, int iterations = 0) => new(false, null, iterations, message);
}
