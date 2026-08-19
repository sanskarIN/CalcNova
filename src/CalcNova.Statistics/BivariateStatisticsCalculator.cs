namespace CalcNova.Statistics;

public sealed class BivariateStatisticsCalculator
{
    public const int MaximumPairCount = 100_000;

    public BivariateStatisticsSummary Analyze(IEnumerable<double> xValues, IEnumerable<double> yValues)
    {
        ArgumentNullException.ThrowIfNull(xValues);
        ArgumentNullException.ThrowIfNull(yValues);

        using var xEnumerator = xValues.GetEnumerator();
        using var yEnumerator = yValues.GetEnumerator();

        var count = 0;
        var meanX = 0d;
        var meanY = 0d;
        var sumSquaredX = 0d;
        var sumSquaredY = 0d;
        var coMoment = 0d;

        while (true)
        {
            var hasX = xEnumerator.MoveNext();
            var hasY = yEnumerator.MoveNext();
            if (hasX != hasY)
            {
                throw new ArgumentException("X and Y datasets must contain the same number of values.");
            }

            if (!hasX)
            {
                break;
            }

            count++;
            if (count > MaximumPairCount)
            {
                throw new ArgumentException($"Bivariate datasets may contain at most {MaximumPairCount} value pairs.");
            }

            var x = xEnumerator.Current;
            var y = yEnumerator.Current;
            if (!double.IsFinite(x) || !double.IsFinite(y))
            {
                throw new ArgumentException("Bivariate statistics datasets may only contain finite values.");
            }

            var deltaX = x - meanX;
            var deltaY = y - meanY;
            meanX += deltaX / count;
            meanY += deltaY / count;

            var adjustedX = x - meanX;
            var adjustedY = y - meanY;
            sumSquaredX += deltaX * adjustedX;
            sumSquaredY += deltaY * adjustedY;
            coMoment += deltaX * adjustedY;

            EnsureFiniteState(meanX, meanY, sumSquaredX, sumSquaredY, coMoment);
        }

        if (count == 0)
        {
            throw new ArgumentException("Bivariate statistics datasets must contain at least one value pair.");
        }

        var populationCovariance = coMoment / count;
        var sampleCovariance = count > 1 ? coMoment / (count - 1) : null;

        double? correlation = null;
        if (sumSquaredX > 0d && sumSquaredY > 0d)
        {
            correlation = coMoment / Math.Sqrt(sumSquaredX) / Math.Sqrt(sumSquaredY);
            correlation = Math.Clamp(correlation.Value, -1d, 1d);
        }

        double? slope = null;
        double? intercept = null;
        if (sumSquaredX > 0d)
        {
            slope = coMoment / sumSquaredX;
            intercept = meanY - (slope.Value * meanX);
            if (!double.IsFinite(slope.Value) || !double.IsFinite(intercept.Value))
            {
                throw new OverflowException("Linear regression coefficients exceed the supported numeric range.");
            }
        }

        var rSquared = correlation is null ? null : correlation.Value * correlation.Value;
        return new BivariateStatisticsSummary(
            count,
            meanX,
            meanY,
            populationCovariance,
            sampleCovariance,
            correlation,
            slope,
            intercept,
            rSquared);
    }

    private static void EnsureFiniteState(
        double meanX,
        double meanY,
        double sumSquaredX,
        double sumSquaredY,
        double coMoment)
    {
        if (!double.IsFinite(meanX) ||
            !double.IsFinite(meanY) ||
            !double.IsFinite(sumSquaredX) ||
            !double.IsFinite(sumSquaredY) ||
            !double.IsFinite(coMoment))
        {
            throw new OverflowException("Bivariate statistics calculation exceeds the supported numeric range.");
        }
    }
}
