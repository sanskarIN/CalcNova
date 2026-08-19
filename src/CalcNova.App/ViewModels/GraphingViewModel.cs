using System.Globalization;
using System.Text;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.Graphing;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.ViewModels;

public sealed class GraphingViewModel : ViewModelBase
{
    private readonly GraphSampler _sampler = new();
    private readonly MultiGraphSampler _multiSampler = new();
    private readonly GraphNumericalAnalyzer _analyzer = new();
    private readonly SvgGraphExporter _svgExporter = new();
    private readonly IClipboardService? _clipboardService;
    private string _expression = "sin(x)";
    private string _multiExpressionsText = "sin(x)\ncos(x)";
    private string _minimumX = "-6.283185307179586";
    private string _maximumX = "6.283185307179586";
    private string _analysisX = "0";
    private string _traceX = "0";
    private int _sampleCount = 256;
    private IReadOnlyList<GraphSegment> _segments = Array.Empty<GraphSegment>();
    private IReadOnlyList<GraphTableRow> _tableRows = Array.Empty<GraphTableRow>();
    private IReadOnlyList<GraphExpressionSample> _multiSeries = Array.Empty<GraphExpressionSample>();
    private IReadOnlyList<MultiGraphTableRow> _multiTableRows = Array.Empty<MultiGraphTableRow>();
    private string _summary = string.Empty;
    private string _preview = string.Empty;
    private string _analysisResult = string.Empty;
    private string _traceResult = string.Empty;
    private string _tableCsv = string.Empty;
    private string _multiSummary = string.Empty;
    private string _multiTableCsv = string.Empty;
    private string _svgExport = string.Empty;
    private string _copyStatus = string.Empty;
    private string _errorMessage = string.Empty;

    public GraphingViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        PlotCommand = new RelayCommand(_ => Plot());
        PlotMultipleCommand = new RelayCommand(_ => PlotMultiple());
        DerivativeCommand = new RelayCommand(_ => CalculateDerivative());
        FindRootCommand = new RelayCommand(_ => FindRoot());
        IntegrateCommand = new RelayCommand(_ => Integrate());
        TraceCommand = new RelayCommand(_ => Trace());
        GenerateSvgCommand = new RelayCommand(_ => GenerateSvg());
        CopyPreviewCommand = new AsyncRelayCommand(_ => CopyPreviewAsync());
        CopyAnalysisResultCommand = new AsyncRelayCommand(_ => CopyAnalysisResultAsync());
        CopyTraceResultCommand = new AsyncRelayCommand(_ => CopyTraceResultAsync());
        CopyTableCommand = new AsyncRelayCommand(_ => CopyTableAsync());
        CopyMultiTableCommand = new AsyncRelayCommand(_ => CopyMultiTableAsync());
        CopySvgCommand = new AsyncRelayCommand(_ => CopySvgAsync());
        Plot();
    }

    public string Expression
    {
        get => _expression;
        set => SetField(ref _expression, value ?? string.Empty);
    }

    public string MultiExpressionsText
    {
        get => _multiExpressionsText;
        set => SetField(ref _multiExpressionsText, value ?? string.Empty);
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

    public string AnalysisX
    {
        get => _analysisX;
        set => SetField(ref _analysisX, value ?? string.Empty);
    }

    public string TraceX
    {
        get => _traceX;
        set => SetField(ref _traceX, value ?? string.Empty);
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

    public IReadOnlyList<GraphTableRow> TableRows
    {
        get => _tableRows;
        private set => SetField(ref _tableRows, value);
    }

    public IReadOnlyList<GraphExpressionSample> MultiSeries
    {
        get => _multiSeries;
        private set => SetField(ref _multiSeries, value);
    }

    public IReadOnlyList<MultiGraphTableRow> MultiTableRows
    {
        get => _multiTableRows;
        private set => SetField(ref _multiTableRows, value);
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

    public string AnalysisResult
    {
        get => _analysisResult;
        private set => SetField(ref _analysisResult, value);
    }

    public string TraceResult
    {
        get => _traceResult;
        private set => SetField(ref _traceResult, value);
    }

    public string TableCsv
    {
        get => _tableCsv;
        private set => SetField(ref _tableCsv, value);
    }

    public string MultiSummary
    {
        get => _multiSummary;
        private set => SetField(ref _multiSummary, value);
    }

    public string MultiTableCsv
    {
        get => _multiTableCsv;
        private set => SetField(ref _multiTableCsv, value);
    }

    public string SvgExport
    {
        get => _svgExport;
        private set => SetField(ref _svgExport, value);
    }

    public string CopyStatus
    {
        get => _copyStatus;
        private set => SetField(ref _copyStatus, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand PlotCommand { get; }

    public ICommand PlotMultipleCommand { get; }

    public ICommand DerivativeCommand { get; }

    public ICommand FindRootCommand { get; }

    public ICommand IntegrateCommand { get; }

    public ICommand TraceCommand { get; }

    public ICommand GenerateSvgCommand { get; }

    public ICommand CopyPreviewCommand { get; }

    public ICommand CopyAnalysisResultCommand { get; }

    public ICommand CopyTraceResultCommand { get; }

    public ICommand CopyTableCommand { get; }

    public ICommand CopyMultiTableCommand { get; }

    public ICommand CopySvgCommand { get; }

    private void Plot()
    {
        try
        {
            var options = CreateSamplingOptions();
            var result = _sampler.Sample(Expression, options);

            TraceResult = string.Empty;
            SvgExport = string.Empty;
            CopyStatus = string.Empty;

            if (!result.Success)
            {
                ClearPlotOutputs();
                ErrorMessage = result.ErrorMessage ?? "Graph sampling failed.";
                return;
            }

            Segments = result.Segments;
            TableRows = GraphTableExporter.CreateRows(result.Segments);
            TableCsv = GraphTableExporter.ToCsv(TableRows);
            var pointCount = result.Segments.Sum(segment => segment.Points.Count);
            Summary = $"{result.Segments.Count} segment(s) • {pointCount} valid point(s) • {result.InvalidSampleCount} invalid sample(s)";
            Preview = BuildPreview(result.Segments);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            TraceResult = string.Empty;
            SvgExport = string.Empty;
            CopyStatus = string.Empty;
            ClearPlotOutputs();
            ErrorMessage = exception.Message;
        }
    }

    private void PlotMultiple()
    {
        try
        {
            var definitions = GraphExpressionListParser.Parse(MultiExpressionsText);
            var result = _multiSampler.Sample(definitions, CreateSamplingOptions());
            CopyStatus = string.Empty;

            if (!result.Success)
            {
                ClearMultiPlotOutputs();
                ErrorMessage = result.ErrorMessage ?? "Multi-expression graph sampling failed.";
                return;
            }

            MultiSeries = result.Series;
            MultiTableRows = MultiGraphTableExporter.CreateRows(result.Series);
            MultiTableCsv = MultiGraphTableExporter.ToCsv(MultiTableRows);
            var pointCount = result.Series.Sum(series => series.ValidPointCount);
            var invalidCount = result.Series.Sum(series => series.InvalidSampleCount);
            MultiSummary = $"{result.Series.Count} expression(s) • {pointCount} valid point(s) • {invalidCount} invalid sample(s)";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            ClearMultiPlotOutputs();
            ErrorMessage = exception.Message;
        }
    }

    private void CalculateDerivative()
    {
        RunAnalysis(() =>
        {
            var x = ParseFinite(AnalysisX, "Analysis X");
            var value = _analyzer.Derivative(Expression, x);
            return $"f′({Format(x)}) ≈ {Format(value)}";
        });
    }

    private void FindRoot()
    {
        RunAnalysis(() =>
        {
            var minimum = ParseFinite(MinimumX, "Minimum X");
            var maximum = ParseFinite(MaximumX, "Maximum X");
            var root = _analyzer.FindRoot(Expression, minimum, maximum);
            return $"root ≈ {Format(root)}";
        });
    }

    private void Integrate()
    {
        RunAnalysis(() =>
        {
            var minimum = ParseFinite(MinimumX, "Minimum X");
            var maximum = ParseFinite(MaximumX, "Maximum X");
            var integral = _analyzer.Integrate(Expression, minimum, maximum);
            return $"∫[{Format(minimum)}, {Format(maximum)}] f(x) dx ≈ {Format(integral)}";
        });
    }

    private void Trace()
    {
        try
        {
            var requestedX = ParseFinite(TraceX, "Trace X");
            var trace = GraphTraceLocator.FindNearest(Segments, requestedX);
            TraceResult = $"requested x={Format(requestedX)} • sampled x≈{Format(trace.SampledX)} • y≈{Format(trace.Y)} • segment {trace.Segment}";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or OverflowException)
        {
            TraceResult = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void GenerateSvg()
    {
        try
        {
            if (Segments.Count == 0)
            {
                throw new InvalidOperationException("Plot a graph before generating SVG export.");
            }

            SvgExport = _svgExporter.Export(Segments);
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            SvgExport = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private async Task CopyPreviewAsync()
    {
        var text = string.IsNullOrWhiteSpace(Preview)
            ? string.Empty
            : $"{Summary}{Environment.NewLine}{Preview}";
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, text, "graph preview");
    }

    private async Task CopyAnalysisResultAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, AnalysisResult, "analysis result");
    }

    private async Task CopyTraceResultAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, TraceResult, "trace result");
    }

    private async Task CopyTableAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, TableCsv, "graph table");
    }

    private async Task CopyMultiTableAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, MultiTableCsv, "multi-expression graph table");
    }

    private async Task CopySvgAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, SvgExport, "SVG graph export");
    }

    private void RunAnalysis(Func<string> operation)
    {
        try
        {
            AnalysisResult = operation();
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or OverflowException)
        {
            AnalysisResult = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private GraphSamplingOptions CreateSamplingOptions()
    {
        return new GraphSamplingOptions
        {
            MinimumX = ParseFinite(MinimumX, "Minimum X"),
            MaximumX = ParseFinite(MaximumX, "Maximum X"),
            SampleCount = SampleCount
        };
    }

    private void ClearPlotOutputs()
    {
        Segments = Array.Empty<GraphSegment>();
        TableRows = Array.Empty<GraphTableRow>();
        TableCsv = string.Empty;
        Summary = string.Empty;
        Preview = string.Empty;
        SvgExport = string.Empty;
    }

    private void ClearMultiPlotOutputs()
    {
        MultiSeries = Array.Empty<GraphExpressionSample>();
        MultiTableRows = Array.Empty<MultiGraphTableRow>();
        MultiTableCsv = string.Empty;
        MultiSummary = string.Empty;
    }

    private static double ParseFinite(string text, string label)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
        {
            throw new FormatException($"{label} must be a finite number.");
        }

        return value;
    }

    private static string Format(double value) => value.ToString("G12", CultureInfo.InvariantCulture);

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
