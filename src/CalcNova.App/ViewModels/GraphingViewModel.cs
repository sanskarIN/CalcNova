using System.Globalization;
using System.Text;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Graphing;

namespace CalcNova.App.ViewModels;

public sealed class GraphingViewModel : ViewModelBase
{
    private readonly GraphSampler _sampler = new();
    private string _expression = "sin(x)";
    private string _minimumX = "-6.283185307179586";
    private string _maximumX = "6.283185307179586";
    private int _sampleCount = 256;
    private IReadOnlyList<GraphSegment> _segments = Array.Empty<GraphSegment>();
    private string _summary = string.Empty;
    private string _preview = string.Empty;
    private string _errorMessage = string.Empty;

    public GraphingViewModel()
    {
        PlotCommand = new RelayCommand(_ => Plot());
        Plot();
    }

    public string Expression
    {
        get => _expression;
        set => SetField(ref _expression, value ?? string.Empty);
    }

    public string MinimumX
    {
        get => _minimumX;
        set => SetField(ref _minimumX, value ?? string.Empty);
    }

    public string MaximumX
    {
        get => _maximumX;
        set => SetField(ref _maximumX, value ?? string.Empty);
    }

    public int SampleCount
    {
        get => _sampleCount;
        set => SetField(ref _sampleCount, value);
    }

    public IReadOnlyList<GraphSegment> Segments
    {
        get => _segments;
        private set => SetField(ref _segments, value);
    }

    public string Summary
    {
        get => _summary;
        private set => SetField(ref _summary, value);
    }

    public string Preview
    {
        get => _preview;
        private set => SetField(ref _preview, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand PlotCommand { get; }

    private void Plot()
    {
        try
        {
            var minimum = ParseFinite(MinimumX, "Minimum X");
            var maximum = ParseFinite(MaximumX, "Maximum X");
            var result = _sampler.Sample(Expression, new GraphSamplingOptions
            {
                MinimumX = minimum,
                MaximumX = maximum,
                SampleCount = SampleCount
            });

            if (!result.Success)
            {
                Segments = Array.Empty<GraphSegment>();
                Summary = string.Empty;
                Preview = string.Empty;
                ErrorMessage = result.ErrorMessage ?? "Graph sampling failed.";
                return;
            }

            Segments = result.Segments;
            var pointCount = result.Segments.Sum(segment => segment.Points.Count);
            Summary = $"{result.Segments.Count} segment(s) • {pointCount} valid point(s) • {result.InvalidSampleCount} invalid sample(s)";
            Preview = BuildPreview(result.Segments);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            Segments = Array.Empty<GraphSegment>();
            Summary = string.Empty;
            Preview = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private static double ParseFinite(string text, string label)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
        {
            throw new FormatException($"{label} must be a finite number.");
        }

        return value;
    }

    private static string BuildPreview(IEnumerable<GraphSegment> segments)
    {
        var builder = new StringBuilder();
        var shown = 0;
        foreach (var segment in segments)
        {
            foreach (var point in segment.Points)
            {
                builder.Append(point.X.ToString("G8", CultureInfo.InvariantCulture));
                builder.Append(" → ");
                builder.AppendLine(point.Y.ToString("G8", CultureInfo.InvariantCulture));
                shown++;
                if (shown >= 12)
                {
                    return builder.ToString().TrimEnd();
                }
            }
        }

        return builder.ToString().TrimEnd();
    }
}
