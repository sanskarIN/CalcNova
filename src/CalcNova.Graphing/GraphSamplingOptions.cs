using CalcNova.Core.Evaluation;

namespace CalcNova.Graphing;

public sealed record GraphSamplingOptions
{
    public double MinimumX { get; init; } = -10d;

    public double MaximumX { get; init; } = 10d;

    public int SampleCount { get; init; } = 512;

    public AngleUnit AngleUnit { get; init; } = AngleUnit.Radians;

    public double MaximumAbsoluteY { get; init; } = 1e12d;

    public double DiscontinuityJumpThreshold { get; init; } = 1e6d;
}
