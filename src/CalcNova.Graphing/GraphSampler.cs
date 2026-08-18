using CalcNova.Core.Evaluation;
using CalcNova.Core.Numerics;

namespace CalcNova.Graphing;

public sealed class GraphSampler
{
    public const int MaximumSamples = 10_000;

    private readonly ExpressionEvaluator _evaluator;

    public GraphSampler(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
    }

    public GraphSamplingResult Sample(string expression, GraphSamplingOptions? options = null)
    {
        options ??= new GraphSamplingOptions();
        ValidateOptions(options);

        var variables = new Dictionary<string, NumberValue>(StringComparer.OrdinalIgnoreCase)
        {
            ["x"] = NumberValue.Zero
        };
        var evaluationOptions = new EvaluationOptions
        {
            AngleUnit = options.AngleUnit,
            Variables = variables
        };

        CompiledExpression compiled;
        try
        {
            compiled = _evaluator.Compile(expression, evaluationOptions);
        }
        catch (Exception exception) when (exception is CalcNova.Core.Errors.CalculationException or OverflowException)
        {
            return GraphSamplingResult.Failed(exception.Message);
        }

        var segments = new List<GraphSegment>();
        var current = new List<GraphPoint>();
        var invalidSamples = 0;
        double? previousY = null;
        var step = (options.MaximumX - options.MinimumX) / (options.SampleCount - 1);

        for (var index = 0; index < options.SampleCount; index++)
        {
            var x = index == options.SampleCount - 1
                ? options.MaximumX
                : options.MinimumX + (step * index);
            variables["x"] = NumberValue.FromDouble(x);

            var evaluation = _evaluator.Evaluate(compiled, evaluationOptions);
            if (!evaluation.Success)
            {
                invalidSamples++;
                CloseCurrentSegment(segments, current);
                previousY = null;
                continue;
            }

            var y = evaluation.Value.ToDouble();
            if (!double.IsFinite(y) || Math.Abs(y) > options.MaximumAbsoluteY)
            {
                invalidSamples++;
                CloseCurrentSegment(segments, current);
                previousY = null;
                continue;
            }

            if (previousY is not null && Math.Abs(y - previousY.Value) > options.DiscontinuityJumpThreshold)
            {
                CloseCurrentSegment(segments, current);
            }

            current.Add(new GraphPoint(x, y == 0d ? 0d : y));
            previousY = y;
        }

        CloseCurrentSegment(segments, current);
        return GraphSamplingResult.Completed(segments, invalidSamples);
    }

    private static void CloseCurrentSegment(ICollection<GraphSegment> segments, List<GraphPoint> current)
    {
        if (current.Count > 0)
        {
            segments.Add(new GraphSegment(current.ToArray()));
            current.Clear();
        }
    }

    private static void ValidateOptions(GraphSamplingOptions options)
    {
        if (!double.IsFinite(options.MinimumX) || !double.IsFinite(options.MaximumX) || options.MinimumX >= options.MaximumX)
        {
            throw new ArgumentException("Graph X bounds must be finite and MinimumX must be less than MaximumX.", nameof(options));
        }

        if (options.SampleCount is < 2 or > MaximumSamples)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                options.SampleCount,
                $"Graph sample count must be between 2 and {MaximumSamples}.");
        }

        if (!double.IsFinite(options.MaximumAbsoluteY) || options.MaximumAbsoluteY <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "MaximumAbsoluteY must be finite and positive.");
        }

        if (!double.IsFinite(options.DiscontinuityJumpThreshold) || options.DiscontinuityJumpThreshold <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "DiscontinuityJumpThreshold must be finite and positive.");
        }
    }
}
