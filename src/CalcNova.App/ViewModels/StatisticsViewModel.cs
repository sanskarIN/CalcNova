using System.Globalization;
using System.Text;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.Platform.Clipboard;
using CalcNova.Statistics;

namespace CalcNova.App.ViewModels;

public sealed class StatisticsViewModel : ViewModelBase
{
    private readonly StatisticsCalculator _calculator = new();
    private readonly BivariateStatisticsCalculator _bivariateCalculator = new();
    private readonly IClipboardService? _clipboardService;
    private BivariateStatisticsSummary? _lastBivariateSummary;
    private string _datasetText = "1, 2, 2, 3, 4";
    private string _pairedXText = "1, 2, 3, 4";
    private string _pairedYText = "3, 5, 7, 9";
    private string _predictionX = "5";
    private string _summary = string.Empty;
    private string _bivariateSummary = string.Empty;
    private string _predictionResult = string.Empty;
    private string _copyStatus = string.Empty;
    private string _errorMessage = string.Empty;

    public StatisticsViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        AnalyzeCommand = new RelayCommand(_ => Analyze());
        AnalyzePairsCommand = new RelayCommand(_ => AnalyzePairs());
        PredictCommand = new RelayCommand(_ => Predict());
        CopySummaryCommand = new AsyncRelayCommand(_ => CopySummaryAsync());
        CopyBivariateSummaryCommand = new AsyncRelayCommand(_ => CopyBivariateSummaryAsync());
        Analyze();
        AnalyzePairs();
    }

    public string DatasetText
    {
        get => _datasetText;
        set => SetField(ref _datasetText, value ?? string.Empty);
    }

    public string PairedXText
    {
        get => _pairedXText;
        set => SetField(ref _pairedXText, value ?? string.Empty);
    }

    public string PairedYText
    {
        get => _pairedYText;
        set => SetField(ref _pairedYText, value ?? string.Empty);
    }

    public string PredictionX
    {
        get => _predictionX;
        set => SetField(ref _predictionX, value ?? string.Empty);
    }

    public string Summary
    {
        get => _summary;
        private set => SetField(ref _summary, value);
    }

    public string BivariateSummary
    {
        get => _bivariateSummary;
        private set => SetField(ref _bivariateSummary, value);
    }

    public string PredictionResult
    {
        get => _predictionResult;
        private set => SetField(ref _predictionResult, value);
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

    public ICommand AnalyzeCommand { get; }

    public ICommand AnalyzePairsCommand { get; }

    public ICommand PredictCommand { get; }

    public ICommand CopySummaryCommand { get; }

    public ICommand CopyBivariateSummaryCommand { get; }

    private void Analyze()
    {
        try
        {
            var values = StatisticsDatasetParser.Parse(DatasetText);
            var result = _calculator.Analyze(values);
            var modes = result.Modes.Count == 0
                ? "No repeated mode"
                : string.Join(", ", result.Modes.Select(Format));

            var builder = new StringBuilder();
            builder.AppendLine(CultureInfo.InvariantCulture, $"Count: {result.Count}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Sum: {Format(result.Sum)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Mean: {Format(result.Mean)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Median: {Format(result.Median)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Mode: {modes}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Minimum: {Format(result.Minimum)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Maximum: {Format(result.Maximum)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Range: {Format(result.Range)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Population variance: {Format(result.PopulationVariance)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Population σ: {Format(result.PopulationStandardDeviation)}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Sample variance: {(result.SampleVariance is null ? "N/A" : Format(result.SampleVariance.Value))}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Sample s: {(result.SampleStandardDeviation is null ? "N/A" : Format(result.SampleStandardDeviation.Value))}");
            builder.AppendLine(CultureInfo.InvariantCulture, $"Q1: {Format(result.FirstQuartile)}");
            builder.Append(CultureInfo.InvariantCulture, $"Q3: {Format(result.ThirdQuartile)}");

            Summary = builder.ToString();
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            Summary = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void AnalyzePairs()
    {
        try
        {
            var xValues = StatisticsDatasetParser.Parse(PairedXText, BivariateStatisticsCalculator.MaximumPairCount);
            var yValues = StatisticsDatasetParser.Parse(PairedYText, BivariateStatisticsCalculator.MaximumPairCount);
            var result = _bivariateCalculator.Analyze(xValues, yValues);

            _lastBivariateSummary = result;
            BivariateSummary = BuildBivariateSummary(result);
            PredictionResult = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            _lastBivariateSummary = null;
            BivariateSummary = string.Empty;
            PredictionResult = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Predict()
    {
        try
        {
            if (_lastBivariateSummary is null)
            {
                throw new InvalidOperationException("Analyze paired X/Y data before making a prediction.");
            }

            if (!double.TryParse(PredictionX, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) || !double.IsFinite(x))
            {
                throw new FormatException("Prediction X must be a finite number.");
            }

            var y = _lastBivariateSummary.Predict(x);
            PredictionResult = $"ŷ({Format(x)}) = {Format(y)}";
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or OverflowException)
        {
            PredictionResult = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private async Task CopySummaryAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, Summary, "statistics summary");
    }

    private async Task CopyBivariateSummaryAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, BivariateSummary, "bivariate statistics summary");
    }

    private static string BuildBivariateSummary(BivariateStatisticsSummary result)
    {
        var builder = new StringBuilder();
        builder.AppendLine(CultureInfo.InvariantCulture, $"Pairs: {result.Count}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Mean X: {Format(result.MeanX)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Mean Y: {Format(result.MeanY)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Population covariance: {Format(result.PopulationCovariance)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Sample covariance: {FormatNullable(result.SampleCovariance)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Pearson r: {FormatNullable(result.PearsonCorrelation)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Regression slope: {FormatNullable(result.RegressionSlope)}");
        builder.AppendLine(CultureInfo.InvariantCulture, $"Regression intercept: {FormatNullable(result.RegressionIntercept)}");
        builder.Append(CultureInfo.InvariantCulture, $"R²: {FormatNullable(result.RSquared)}");
        return builder.ToString();
    }

    private static string FormatNullable(double? value) => value is null ? "N/A" : Format(value.Value);

    private static string Format(double value) => value.ToString("G15", CultureInfo.InvariantCulture);
}
