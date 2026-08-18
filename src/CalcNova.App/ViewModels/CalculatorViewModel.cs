using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Evaluation;
using CalcNova.Core.Memory;
using CalcNova.Core.Numerics;

namespace CalcNova.App.ViewModels;

public sealed class CalculatorViewModel : ViewModelBase
{
    private readonly ExpressionEvaluator _evaluator;
    private readonly CalculationSession _session;
    private readonly CalculatorPercentageTransformer _percentageTransformer;
    private readonly CalculatorMemory _memory = new();
    private readonly Func<string, string, Task>? _recordCalculationAsync;
    private readonly Func<bool> _historyEnabledProvider;
    private string? _lastEvaluatedExpression;
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
        _session = new CalculationSession(_evaluator);
        _percentageTransformer = new CalculatorPercentageTransformer(_evaluator);
        _recordCalculationAsync = recordCalculationAsync;
        _historyEnabledProvider = historyEnabledProvider ?? (() => true);

        AppendCommand = new RelayCommand(Append);
        EvaluateCommand = new AsyncRelayCommand(_ => EvaluateAsync());
        ClearCommand = new RelayCommand(_ => Clear());
        BackspaceCommand = new RelayCommand(_ => Backspace());
        SetAngleUnitCommand = new RelayCommand(SetAngleUnit);
        UseResultCommand = new RelayCommand(_ => UseResult());
        PercentageCommand = new RelayCommand(_ => ApplyPercentage());
        MemoryClearCommand = new RelayCommand(_ => MemoryClear());
        MemoryRecallCommand = new RelayCommand(_ => MemoryRecall());
        MemoryStoreCommand = new RelayCommand(_ => MemoryStore());
        MemoryAddCommand = new RelayCommand(_ => MemoryAdd());
        MemorySubtractCommand = new RelayCommand(_ => MemorySubtract());
    }

    public string Expression
    {
        get => _expression;
        set
        {
            var normalized = value ?? string.Empty;
            if (SetField(ref _expression, normalized))
            {
                if (!string.Equals(_lastEvaluatedExpression, normalized, StringComparison.Ordinal))
                {
                    ResetRepeatState();
                }
            }
        }
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
                ResetRepeatState();
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

    public string MemoryIndicator => _memory.HasValue
        ? $"M = {_memory.Recall().ToDisplayString()}"
        : string.Empty;

    public ICommand AppendCommand { get; }

    public ICommand EvaluateCommand { get; }

    public ICommand ClearCommand { get; }

    public ICommand BackspaceCommand { get; }

    public ICommand SetAngleUnitCommand { get; }

    public ICommand UseResultCommand { get; }

    public ICommand PercentageCommand { get; }

    public ICommand MemoryClearCommand { get; }

    public ICommand MemoryRecallCommand { get; }

    public ICommand MemoryStoreCommand { get; }

    public ICommand MemoryAddCommand { get; }

    public ICommand MemorySubtractCommand { get; }

    public async Task EvaluateAsync()
    {
        var options = CreateOptions();
        var expression = Expression;
        var isRepeat = _session.CanRepeat &&
                       !string.IsNullOrWhiteSpace(_lastEvaluatedExpression) &&
                       string.Equals(expression, _lastEvaluatedExpression, StringComparison.Ordinal) &&
                       Result is not "0" and not "Error";

        EvaluationResult evaluation;
        if (isRepeat && TryParseResult(out var currentValue))
        {
            evaluation = _session.Repeat(currentValue, options);
        }
        else
        {
            evaluation = _session.Evaluate(expression, options);
        }

        if (!evaluation.Success)
        {
            Result = "Error";
            StatusMessage = evaluation.ErrorMessage ?? "Calculation failed.";
            ResetRepeatState();
            return;
        }

        Result = evaluation.Value.ToDisplayString();
        StatusMessage = string.Empty;
        _lastEvaluatedExpression = expression;

        if (_recordCalculationAsync is not null && _historyEnabledProvider() && !string.IsNullOrWhiteSpace(expression))
        {
            var historyExpression = isRepeat ? $"repeat({expression})" : expression;
            await _recordCalculationAsync(historyExpression, Result);
        }
    }

    public void ApplyAngleUnit(AngleUnit angleUnit) => AngleUnit = angleUnit;

    public void Clear()
    {
        _expression = string.Empty;
        OnPropertyChanged(nameof(Expression));
        Result = "0";
        StatusMessage = string.Empty;
        ResetRepeatState();
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

    private void ApplyPercentage()
    {
        if (string.IsNullOrWhiteSpace(Expression))
        {
            StatusMessage = "Enter a value or expression before using percentage.";
            return;
        }

        try
        {
            var transformation = _percentageTransformer.Transform(Expression, CreateOptions());
            Expression = transformation.TransformedExpression;
            StatusMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            StatusMessage = exception.Message;
        }
    }

    private void MemoryClear()
    {
        _memory.Clear();
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(MemoryIndicator));
    }

    private void MemoryRecall()
    {
        if (!_memory.HasValue)
        {
            StatusMessage = "Memory is empty.";
            return;
        }

        Expression = _memory.Recall().ToDisplayString();
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(MemoryIndicator));
    }

    private void MemoryStore()
    {
        if (!TryGetCurrentValue(out var value))
        {
            return;
        }

        _memory.Store(value);
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(MemoryIndicator));
    }

    private void MemoryAdd()
    {
        if (!TryGetCurrentValue(out var value))
        {
            return;
        }

        _memory.Add(value);
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(MemoryIndicator));
    }

    private void MemorySubtract()
    {
        if (!TryGetCurrentValue(out var value))
        {
            return;
        }

        _memory.Subtract(value);
        StatusMessage = string.Empty;
        OnPropertyChanged(nameof(MemoryIndicator));
    }

    private bool TryGetCurrentValue(out NumberValue value)
    {
        if (!string.IsNullOrWhiteSpace(Expression))
        {
            var evaluation = _evaluator.Evaluate(Expression, CreateOptions());
            if (evaluation.Success)
            {
                value = evaluation.Value;
                return true;
            }

            StatusMessage = evaluation.ErrorMessage ?? "The current expression cannot be stored in memory.";
            value = NumberValue.Zero;
            return false;
        }

        if (TryParseResult(out value))
        {
            return true;
        }

        StatusMessage = "There is no valid value to store in memory.";
        return false;
    }

    private bool TryParseResult(out NumberValue value)
    {
        if (Result is not "Error" && !string.IsNullOrWhiteSpace(Result))
        {
            try
            {
                value = NumberValue.Parse(Result);
                return true;
            }
            catch (Exception exception) when (exception is ArgumentException or OverflowException)
            {
            }
        }

        value = NumberValue.Zero;
        return false;
    }

    private EvaluationOptions CreateOptions() => new() { AngleUnit = AngleUnit };

    private void ResetRepeatState()
    {
        _session.Reset();
        _lastEvaluatedExpression = null;
    }
}
