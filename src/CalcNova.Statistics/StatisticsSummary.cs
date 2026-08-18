namespace CalcNova.Statistics;

public sealed record StatisticsSummary(
    int Count,
    double Sum,
    double Mean,
    double Median,
    IReadOnlyList<double> Modes,
    double Minimum,
    double Maximum,
    double Range,
    double PopulationVariance,
    double PopulationStandardDeviation,
    double? SampleVariance,
    double? SampleStandardDeviation,
    double FirstQuartile,
    double ThirdQuartile);
