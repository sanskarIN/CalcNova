namespace CalcNova.Statistics;

public sealed class StatisticsCalculator
{
    public StatisticsSummary Analyze(IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var data = values.ToArray();
        if (data.Length == 0)
        {
            throw new ArgumentException("A statistics dataset must contain at least one value.", nameof(values));
        }

        if (data.Any(value => !double.IsFinite(value)))
        {
            throw new ArgumentException("Statistics datasets may only contain finite values.", nameof(values));
        }

        Array.Sort(data);

        var sum = CompensatedSum(data);
        var mean = sum / data.Length;
        var median = PercentileSorted(data, 0.5d);
        var minimum = data[0];
        var maximum = data[^1];
        var populationVariance = Variance(data, mean, sample: false);
        var sampleVariance = data.Length > 1 ? Variance(data, mean, sample: true) : null;
        var modes = CalculateModes(data);

        return new StatisticsSummary(
            data.Length,
            sum,
            mean,
            median,
            modes,
            minimum,
            maximum,
            maximum - minimum,
            populationVariance,
            Math.Sqrt(populationVariance),
            sampleVariance,
            sampleVariance is null ? null : Math.Sqrt(sampleVariance.Value),
            PercentileSorted(data, 0.25d),
            PercentileSorted(data, 0.75d));
    }

    public double Percentile(IEnumerable<double> values, double percentile)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (percentile is < 0d or > 1d || !double.IsFinite(percentile))
        {
            throw new ArgumentOutOfRangeException(nameof(percentile), percentile, "Percentile must be in the inclusive range 0 through 1.");
        }

        var data = values.ToArray();
        if (data.Length == 0)
        {
            throw new ArgumentException("A statistics dataset must contain at least one value.", nameof(values));
        }

        if (data.Any(value => !double.IsFinite(value)))
        {
            throw new ArgumentException("Statistics datasets may only contain finite values.", nameof(values));
        }

        Array.Sort(data);
        return PercentileSorted(data, percentile);
    }

    private static double PercentileSorted(IReadOnlyList<double> sorted, double percentile)
    {
        if (sorted.Count == 1)
        {
            return sorted[0];
        }

        var position = (sorted.Count - 1) * percentile;
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);
        if (lower == upper)
        {
            return sorted[lower];
        }

        var fraction = position - lower;
        return sorted[lower] + ((sorted[upper] - sorted[lower]) * fraction);
    }

    private static IReadOnlyList<double> CalculateModes(IReadOnlyList<double> sorted)
    {
        var modes = new List<double>();
        var bestCount = 1;
        var currentCount = 1;

        for (var index = 1; index <= sorted.Count; index++)
        {
            if (index < sorted.Count && sorted[index].Equals(sorted[index - 1]))
            {
                currentCount++;
                continue;
            }

            if (currentCount > bestCount)
            {
                bestCount = currentCount;
                modes.Clear();
                modes.Add(sorted[index - 1]);
            }
            else if (currentCount == bestCount && bestCount > 1)
            {
                modes.Add(sorted[index - 1]);
            }

            currentCount = 1;
        }

        return modes;
    }

    private static double Variance(IReadOnlyList<double> values, double mean, bool sample)
    {
        var sum = 0d;
        var compensation = 0d;
        foreach (var value in values)
        {
            var difference = value - mean;
            var squared = difference * difference;
            var adjusted = squared - compensation;
            var next = sum + adjusted;
            compensation = (next - sum) - adjusted;
            sum = next;
        }

        var denominator = sample ? values.Count - 1 : values.Count;
        return sum / denominator;
    }

    private static double CompensatedSum(IEnumerable<double> values)
    {
        var sum = 0d;
        var compensation = 0d;
        foreach (var value in values)
        {
            var adjusted = value - compensation;
            var next = sum + adjusted;
            compensation = (next - sum) - adjusted;
            sum = next;
        }

        return sum;
    }
}
