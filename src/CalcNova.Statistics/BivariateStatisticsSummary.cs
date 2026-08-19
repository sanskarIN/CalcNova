namespace CalcNova.Statistics;

public sealed record BivariateStatisticsSummary(
    int Count,
    double MeanX,
    double MeanY,
    double PopulationCovariance,
    double? SampleCovariance,
    double? PearsonCorrelation,
    double? RegressionSlope,
    double? RegressionIntercept,
    double? RSquared)
{
    public bool HasLinearRegression => RegressionSlope is not null && RegressionIntercept is not null;

    public double Predict(double x)
    {
        if (!double.IsFinite(x))
        {
            throw new ArgumentOutOfRangeException(nameof(x), x, "Prediction X must be finite.");
        }

        if (RegressionSlope is not { } slope || RegressionIntercept is not { } intercept)
        {
            throw new InvalidOperationException("Linear regression is undefined when the X dataset has zero variance.");
        }

        var prediction = (slope * x) + intercept;
        if (!double.IsFinite(prediction))
        {
            throw new OverflowException("The regression prediction exceeds the supported numeric range.");
        }

        return prediction;
    }
}
