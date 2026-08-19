using System.Globalization;
using System.Text;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.Matrices;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.ViewModels;

public sealed class MatricesViewModel : ViewModelBase
{
    private readonly IClipboardService? _clipboardService;
    private string _matrixText = "4, 7\n2, 6";
    private string _rightHandSideText = "1, 0";
    private string _result = string.Empty;
    private string _copyStatus = string.Empty;
    private string _errorMessage = string.Empty;

    public MatricesViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        DeterminantCommand = new RelayCommand(_ => CalculateDeterminant());
        InverseCommand = new RelayCommand(_ => CalculateInverse());
        RankCommand = new RelayCommand(_ => CalculateRank());
        SolveCommand = new RelayCommand(_ => SolveSystem());
        CopyResultCommand = new AsyncRelayCommand(_ => CopyResultAsync());
        CalculateDeterminant();
    }

    public string MatrixText
    {
        get => _matrixText;
        set => SetField(ref _matrixText, value ?? string.Empty);
    }

    public string RightHandSideText
    {
        get => _rightHandSideText;
        set => SetField(ref _rightHandSideText, value ?? string.Empty);
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
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

    public ICommand DeterminantCommand { get; }

    public ICommand InverseCommand { get; }

    public ICommand RankCommand { get; }

    public ICommand SolveCommand { get; }

    public ICommand CopyResultCommand { get; }

    private void CalculateDeterminant() => Execute(matrix => $"det(A) = {Format(matrix.Determinant())}");

    private void CalculateInverse() => Execute(matrix => $"A⁻¹ =\n{FormatMatrix(matrix.Inverse())}");

    private void CalculateRank() => Execute(matrix => $"rank(A) = {matrix.Rank().ToString(CultureInfo.InvariantCulture)}");

    private void SolveSystem()
    {
        try
        {
            var matrix = ParseMatrix(MatrixText);
            var rightHandSide = ParseVector(RightHandSideText);
            var solution = matrix.Solve(rightHandSide);
            Result = $"x = [{string.Join(", ", solution.Select(Format))}]";
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or OverflowException)
        {
            Result = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Execute(Func<Matrix, string> operation)
    {
        try
        {
            var matrix = ParseMatrix(MatrixText);
            Result = operation(matrix);
            CopyStatus = string.Empty;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or InvalidOperationException or OverflowException)
        {
            Result = string.Empty;
            CopyStatus = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private async Task CopyResultAsync()
    {
        CopyStatus = await ClipboardTextWriter.CopyAsync(_clipboardService, Result, "matrix result");
    }

    private static Matrix ParseMatrix(string text)
    {
        var rowTexts = text.Split(['\r', '\n', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (rowTexts.Length == 0)
        {
            throw new ArgumentException("Enter at least one matrix row.", nameof(text));
        }

        var rows = rowTexts.Select(ParseVector).ToArray();
        return Matrix.FromRows(rows);
    }

    private static double[] ParseVector(string text)
    {
        var tokens = text.Split([',', ' ', '\t'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0)
        {
            throw new ArgumentException("Enter at least one numeric value.", nameof(text));
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

    private static string FormatMatrix(Matrix matrix)
    {
        var builder = new StringBuilder();
        for (var row = 0; row < matrix.Rows; row++)
        {
            builder.Append('[');
            for (var column = 0; column < matrix.Columns; column++)
            {
                if (column > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(Format(matrix[row, column]));
            }

            builder.Append(']');
            if (row < matrix.Rows - 1)
            {
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }

    private static string Format(double value) => value.ToString("G12", CultureInfo.InvariantCulture);
}
