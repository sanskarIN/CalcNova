using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Errors;
using CalcNova.Core.Evaluation;
using CalcNova.Core.Memory;
using CalcNova.Core.Numerics;
using CalcNova.Core.Parsing;
using CalcNova.Platform.Clipboard;

namespace CalcNova.App.ViewModels;

public sealed class CalculatorViewModel : ViewModelBase
{
    private readonly ExpressionEvaluator _evaluator;
    private readonly CalculationSession _session;
    private readonly CalculatorPercentageTransformer _percentageTransformer;
    private readonly CalculatorMemory _memory = new();
    private readonly Func<string, string, Task>? _recordCalculationAsync;
    private readonly Func<bool> _historyEnabledProvider;
    private readonly IClipboardService? _clipboardService;
    private string? _lastEvaluatedExpression;
    private string _expression = string.Empty;
    private string _result = "0";
    private string _statusMessage = string.Empty;
    private AngleUnit _angleUnit = AngleUnit.Degrees;
    private int _selectionStart;
    private int _selectionEnd;

    public CalculatorViewModel(
        ExpressionEvaluator? evaluator = null,
        Func<string, string, Task>? recordCalculationAsync = null,
        Func<bool>? historyEnabledProvider = null,
        IClipboardService? clipboardService = null)
    {
        _evaluator = evaluator ?? new ExpressionEvaluator();
        _session = new CalculationSession(_evaluator);
        _percentageTransformer = new CalculatorPercentageTransformer(_evaluator);
        _recordCalculationAsync = recordCalculationAsync;
        _historyEnabledProvider = historyEnabledProvider ?? (() => true);
        _clipboardService = clipboardService;

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
        ImportExpressionCommand = new RelayCommand(ImportExpression);
        PasteCommand = new AsyncRelayCommand(_ => PasteAsync());
        CopyResultCommand = new AsyncRelayCommand(_ => CopyResultAsync());
    }

    public event Action<int, int>? SelectionRequested;

    public string Expression
    {
        get => _expression;
        set
        {
            var normalized = value ?? string.Empty;
            if (SetField(ref _expression, normalized))
            {
                _selectionStart = normalized.Length;
                _selectionEnd = normalized.Length;

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

    public ICommand ImportExpressionCommand { get; }

    public ICommand PasteCommand { get; }

    public ICommand CopyResultCommand { get; }

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

    public void UpdateSelection(int selectionStart, int selectionEnd)
    {
        _selectionStart = ClampSelectionIndex(selectionStart);
        _selectionEnd = ClampSelectionIndex(selectionEnd);
    }

    public void ImportExpression(string? text)
    {
        try
        {
            Expression = ExpressionTextSanitizer.Sanitize(text);
            StatusMessage = string.Empty;
            RequestSelection(Expression.Length);
        }
        catch (ArgumentException exception)
        {
            StatusMessage = exception.Message;
        }
    }

    public async Task PasteAsync(CancellationToken cancellationToken = default)
    {
        if (_clipboardService?.IsAvailable != true)
        {
            StatusMessage = "Clipboard access is unavailable on this platform.";
            return;
        }

        try
        {
            var text = await _clipboardService.GetTextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(text))
            {
                StatusMessage = "The clipboard does not contain expression text.";
                return;
            }

            ImportExpression(text);
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Clipboard paste was cancelled.";
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            StatusMessage = exception.Message;
        }
    }

    public async Task CopyResultAsync(CancellationToken cancellationToken = default)
    {
        if (_clipboardService?.IsAvailable != true)
        {
            StatusMessage = "Clipboard access is unavailable on this platform.";
            return;
        }

        if (Result is "Error" || string.IsNullOrWhiteSpace(Result))
        {
            StatusMessage = "There is no valid result to copy.";
            return;
        }

        try
        {
            await _clipboardService.SetTextAsync(Result, cancellationToken);
            StatusMessage = "Result copied.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Clipboard copy was cancelled.";
        }
        catch (Exception exception) when (exception is InvalidOperationException or NotSupportedException)
        {
            StatusMessage = exception.Message;
        }
    }

    public void Clear()
    {
        Expression = string.Empty;
        Result = "0";
        StatusMessage = string.Empty;
        ResetRepeatState();
        RequestSelection(0);
    }

    public void Backspace()
    {
        if (Expression.Length == 0)
        {
            return;
        }

        var (start, end) = NormalizedSelection();
        if (start != end)
        {
            Expression = Expression.Remove(start, end - start);
            StatusMessage = string.Empty;
            RequestSelection(start);
            return;
        }

        if (start == 0)
        {
            return;
        }

        Expression = Expression.Remove(start - 1, 1);
        StatusMessage = string.Empty;
        RequestSelection(start - 1);
    }

    private void Append(object? parameter)
    {
        if (parameter is not string token || token.Length == 0)
        {
            return;
        }

        var (start, end) = NormalizedSelection();
        var replacedLength = end - start;
        var nextLength = Expression.Length - replacedLength + token.Length;
        if (nextLength > EvaluationOptions.Default.MaximumExpressionLength)
        {
            StatusMessage = "Expression limit reached.";
            return;
        }

        Expression = string.Concat(Expression.AsSpan(0, start), token, Expression.AsSpan(end));
        StatusMessage = string.Empty;
        RequestSelection(start + token.Length);
    }

    private void ImportExpression(object? parameter)
    {
        ImportExpression(parameter as string);
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
        RequestSelection(Expression.Length);
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
            RequestSelection(Expression.Length);
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or CalculationException)
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
        RequestSelection(Expression.Length);
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
            catch (Exception exception) when (exception is ArgumentException or OverflowException or CalculationException)
            {
            }
        }

        value = NumberValue.Zero;
        return false;
    }

    private (int Start, int End) NormalizedSelection()
    {
        var start = ClampSelectionIndex(_selectionStart);
        var end = ClampSelectionIndex(_selectionEnd);
        return start <= end ? (start, end) : (end, start);
    }

    private int ClampSelectionIndex(int index) => Math.Clamp(index, 0, Expression.Length);

    private void RequestSelection(int caretIndex)
    {
        var clamped = ClampSelectionIndex(caretIndex);
        _selectionStart = clamped;
        _selectionEnd = clamped;
        SelectionRequested?.Invoke(clamped, clamped);
    }

    private EvaluationOptions CreateOptions() => new() { AngleUnit = AngleUnit };

    private void ResetRepeatState()
    {
        _session.Reset();
        _lastEvaluatedExpression = null;
    }
}
