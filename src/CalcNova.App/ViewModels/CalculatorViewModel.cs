using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Evaluation;

namespace CalcNova.App.ViewModels;

public sealed class CalculatorViewModel : ViewModelBase
{
    private readonly ExpressionEvaluator _evaluator;
    private readonly Func<string, string, Task>? _recordCalculationAsync;
    private readonly Func<bool> _historyEnabledProvider;
    private string _expression = string.Empty;
    private string _result = "0";
    private string _statusMessage = string.Empty;
    private AngleUnit _angleUnit = AngleUnit.Degrees;

    public CalculatorViewModel(
        ExpressionEvaluator? evaluator = null,
        Func<string, string, Task>? recordCalculationAsync = null,
        Func<bool>? historyEnabledProvider = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
        _recordCalculationAsync = recordCalculationAsync;
        _historyEnabledProvider = historyEnabledProvider ?? (() => true);
        AppendCommand = new RelayCommand(Append);
        EvaluateCommand = new AsyncRelayCommand(_ => EvaluateAsync());
        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        SetAngleUnitCommand = new RelayCommand(SetAngleUnit);
        UseResultCommand = new RelayCommand(_ => UseResult());
    }

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

    public async Task EvaluateAsync()
    {
        var expression = Expression;
        var result = _evaluator.Evaluate(expression, new EvaluationOptions { AngleUnit = AngleUnit });
        if (!result.Success)
        {
            Result = "Error";
            StatusMessage = result.ErrorMessage ?? "Calculation failed.";
            return;
        }

        Result = result.Value.ToDisplayString();
        StatusMessage = string.Empty;

        if (_recordCalculationAsync is not null && _historyEnabledProvider() && !string.IsNullOrWhiteSpace(expression))
        {
            await _recordCalculationAsync(expression, Result);
        }
    }

    public void ApplyAngleUnit(AngleUnit angleUnit) => AngleUnit = angleUnit;

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
            ApplyAngleUnit(unit);
            return;
        }

        if (parameter is string text && Enum.TryParse<AngleUnit>(text, true, out var parsed))
        {
            ApplyAngleUnit(parsed);
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
}
