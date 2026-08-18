using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Evaluation;

namespace CalcNova.App.ViewModels;

public sealed class CalculatorViewModel : INotifyPropertyChanged
{
    private readonly ExpressionEvaluator _evaluator;
    private string _expression = string.Empty;
    private string _result = "0";
    private string _statusMessage = string.Empty;
    private AngleUnit _angleUnit = AngleUnit.Degrees;

    public CalculatorViewModel(ExpressionEvaluator? evaluator = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
        AppendCommand = new RelayCommand(Append);
        EvaluateCommand = new RelayCommand(_ => Evaluate());
        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        SetAngleUnitCommand = new RelayCommand(SetAngleUnit);
        UseResultCommand = new RelayCommand(_ => UseResult());
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Expression
    {
        get => _expression;
        set => SetField(ref _expression, value ?? string.Empty);
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public AngleUnit AngleUnit
    {
        get => _angleUnit;
        private set
        {
            if (SetField(ref _angleUnit, value))
            {
                OnPropertyChanged(nameof(AngleModeLabel));
            }
        }
    }

    public string AngleModeLabel => AngleUnit switch
    {
        AngleUnit.Degrees => "DEG",
        AngleUnit.Radians => "RAD",
        AngleUnit.Gradians => "GRAD",
        _ => "ANGLE"
    };

    public ICommand AppendCommand { get; }

    public ICommand EvaluateCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand BackspaceCommand { get; }

    public ICommand SetAngleUnitCommand { get; }

    public ICommand UseResultCommand { get; }

    public void Evaluate()
    {
        var result = _evaluator.Evaluate(Expression, new EvaluationOptions { AngleUnit = AngleUnit });
        if (result.Success)
        {
            Result = result.Value.ToDisplayString();
            StatusMessage = string.Empty;
            return;
        }

        Result = "Error";
        StatusMessage = result.ErrorMessage ?? "Calculation failed.";
    }

    public void Clear()
    {
        Expression = string.Empty;
        Result = "0";
        StatusMessage = string.Empty;
    }

    public void Backspace()
    {
        if (Expression.Length == 0)
        {
            return;
        }

        Expression = Expression[..^1];
        StatusMessage = string.Empty;
    }

    private void Append(object? parameter)
    {
        if (parameter is not string token || token.Length == 0)
        {
            return;
        }

        if (Expression.Length + token.Length > EvaluationOptions.Default.MaximumExpressionLength)
        {
            StatusMessage = "Expression limit reached.";
            return;
        }

        Expression += token;
        StatusMessage = string.Empty;
    }

    private void SetAngleUnit(object? parameter)
    {
        if (parameter is AngleUnit unit)
        {
            AngleUnit = unit;
            return;
        }

        if (parameter is string text && Enum.TryParse<AngleUnit>(text, true, out var parsed))
        {
            AngleUnit = parsed;
        }
    }

    private void UseResult()
    {
        if (Result is "0" or "Error")
        {
            return;
        }

        Expression = Result;
        StatusMessage = string.Empty;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
