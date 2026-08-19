namespace CalcNova.Graphing;

public sealed record NumericalAnalysisOptions
{
    public double DerivativeStep { get; init; } = 1e-5;

    public double RootTolerance { get; init; } = 1e-10;

    public int MaximumRootIterations { get; init; } = 128;

    public int IntegrationIntervals { get; init; } = 1000;

    public int MaximumIntegrationIntervals { get; init; } = 100_000;

    public void Validate()
    {
        if (!double.IsFinite(DerivativeStep) || DerivativeStep <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(DerivativeStep), "Derivative step must be finite and positive.");
        }

        if (!double.IsFinite(RootTolerance) || RootTolerance <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(RootTolerance), "Root tolerance must be finite and positive.");
        }

        if (MaximumRootIterations is < 1 or > 10_000)
        {
            throw new ArgumentOutOfRangeException(nameof(MaximumRootIterations), "Root iteration limit must be between 1 and 10000.");
        }

        if (MaximumIntegrationIntervals is < 2 or > 1_000_000)
        {
            throw new ArgumentOutOfRangeException(nameof(MaximumIntegrationIntervals), "Maximum integration intervals must be between 2 and 1000000.");
        }

        if (IntegrationIntervals < 2 || IntegrationIntervals > MaximumIntegrationIntervals || (IntegrationIntervals & 1) != 0)
        {
            throw new ArgumentOutOfRangeException(nameof(IntegrationIntervals), "Simpson integration intervals must be even, at least 2, and within the configured maximum.");
        }
    }
}
