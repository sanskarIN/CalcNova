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
    private readonly IClipboardService? _clipboardService;
    private string _datasetText = "1, 2, 2, 3, 4";
    private string _summary = string.Empty;
    private string _copyStatus = string.Empty;
    private string _errorMessage = string.Empty;

    public StatisticsViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        AnalyzeCommand = new RelayCommand(_ => Analyze());
        CopySummaryCommand = new AsyncRelayCommand(_ => CopySummaryAsync());
        Analyze();
    }

    public string DatasetText
    {
        get => _datasetText;
        set => SetField(ref _datasetText, value ?? string.Empty);
    }

    public string Summary
    {
        get => _summary;
        private set => SetField(ref _summary, value);
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

    public ICommand CopySummaryCommand { get; }

    private void Analyze()
    {
        try
        {
            var values = ParseDataset(DatasetText);
            var result = _calculator.Analyze(values);
            var modes = result.Modes.Count == 0
                ? "No repeated mode"
                : string.Join(", ", result.Modes.Select(Format));

            var builder = new StringBuilder();
            builder.AppendLine($"Count: {result.Count}");
            builder.AppendLine($"Sum: {Format(result.Sum)}");
            builder.AppendLine($"Mean: {Format(result.Mean)}");
            builder.AppendLine($"Median: {Format(result.Median)}");
            builder.AppendLine($"Mode: {modes}");
            builder.AppendLine($"Minimum: {Format(result.Minimum)}");
            builder.AppendLine($"Maximum: {Format(result.Maximum)}");
            builder.AppendLine($"Range: {Format(result.Range)}");
            builder.AppendLine($"Population variance: {Format(result.PopulationVariance)}");
            builder.AppendLine($"Population σ: {Format(result.PopulationStandardDeviation)}");
            builder.AppendLine($"Sample variance: {(result.SampleVariance is null ? "N/A" : Format(result.SampleVariance.Value))}");
            builder.AppendLine($"Sample s: {(result.SampleStandardDeviation is null ? "N/A" : Format(result.SampleStandardDeviation.Value))}");
            builder.AppendLine($"Q1: {Format(result.FirstQuartile)}");
            builder.Append($"Q3: {Format(result.ThirdQuartile)}");

            Summary = builder.ToString();
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            Summary = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private async Task CopySummaryAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, Summary, "statistics summary");
    }

    private static double[] ParseDataset(string text)
    {
        var tokens = text.Split([',', ';', ' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            throw new ArgumentException("Enter at least one dataset value.", nameof(text));
        }

        var values = new double[tokens.Length];
        for (var index = 0; index < tokens.Length; index++)
        {
            if (!double.TryParse(tokens[index], NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
            {
                throw new FormatException($"'{tokens[index]}' is not a finite number.");
            }

            values[index] = value;
        }

        return values;
    }

    private static string Format(double value) => value.ToString("G15", CultureInfo.InvariantCulture);
}
