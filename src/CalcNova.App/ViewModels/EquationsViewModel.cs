using System.Globalization;
using System.Numerics;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Equations;

namespace CalcNova.App.ViewModels;

public sealed class EquationsViewModel : ViewModelBase
{
    private readonly EquationSolver _solver = new();
    private string _a = "1";
    private string _b = "-5";
    private string _c = "6";
    private string _result = string.Empty;
    private string _errorMessage = string.Empty;

    public EquationsViewModel()
    {
        SolveCommand = new RelayCommand(_ => Solve());
        Solve();
    }

    public string A
    {
        get => _a;
        set => SetField(ref _a, value ?? string.Empty);
    }

    public string B
    {
        get => _b;
        set => SetField(ref _b, value ?? string.Empty);
    }

    public string C
    {
        get => _c;
        set => SetField(ref _c, value ?? string.Empty);
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand SolveCommand { get; }

    private void Solve()
    {
        try
        {
            var a = ParseFinite(A, nameof(A));
            var b = ParseFinite(B, nameof(B));
            var c = ParseFinite(C, nameof(C));
            var solution = _solver.SolveQuadratic(a, b, c);

            Result = solution.Kind switch
            {
                EquationSolutionKind.TwoReal => $"x₁ = {Format(solution.FirstRoot!.Value)}\nx₂ = {Format(solution.SecondRoot!.Value)}",
                EquationSolutionKind.RepeatedReal => $"Repeated root: x = {Format(solution.FirstRoot!.Value)}",
                EquationSolutionKind.ComplexPair => $"x₁ = {Format(solution.FirstRoot!.Value)}\nx₂ = {Format(solution.SecondRoot!.Value)}",
                EquationSolutionKind.UniqueReal => $"x = {Format(solution.FirstRoot!.Value)}",
                EquationSolutionKind.InfiniteSolutions => "Infinitely many solutions.",
                EquationSolutionKind.NoSolution => "No solution.",
                _ => "No supported result."
            };
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            Result = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private static double ParseFinite(string text, string name)
    {
        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
        {
            throw new FormatException($"Coefficient {name} must be a finite number.");
        }

        return value;
    }

    private static string Format(Complex value)
    {
        var real = value.Real == 0d ? 0d : value.Real;
        var imaginary = value.Imaginary == 0d ? 0d : value.Imaginary;
        if (imaginary == 0d)
        {
            return real.ToString("G15", CultureInfo.InvariantCulture);
        }

        var sign = imaginary < 0d ? "−" : "+";
        return $"{real.ToString("G15", CultureInfo.InvariantCulture)} {sign} {Math.Abs(imaginary).ToString("G15", CultureInfo.InvariantCulture)}i";
    }
}
