namespace CalcNova.App.ViewModels;

public sealed class GraphExpressionEntryViewModel : ViewModelBase
{
    private string _expression;
    private bool _isVisible = true;
    private string _errorMessage = string.Empty;

    public GraphExpressionEntryViewModel(string identifier, string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);
        Identifier = identifier;
        _expression = expression ?? string.Empty;
    }

    public string Identifier { get; }

    public string Expression
    {
        get => _expression;
        set => SetField(ref _expression, value ?? string.Empty);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetField(ref _isVisible, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value ?? string.Empty);
    }

    public override string ToString() => $"{Identifier}: {Expression}";
}
