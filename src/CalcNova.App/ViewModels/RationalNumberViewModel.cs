using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Numerics;

namespace CalcNova.App.ViewModels;

public sealed class RationalNumberViewModel : ViewModelBase
{
    private string _leftText = "1/3";
    private string _rightText = "1/6";
    private string _leftCanonical = "1/3";
    private string _rightCanonical = "1/6";
    private string _result = "1/2";
    private string _operationSummary = "1/3 + 1/6 = 1/2";
    private string _errorMessage = string.Empty;

    public RationalNumberViewModel()
    {
        NormalizeCommand = new RelayCommand(_ => Normalize());
        AddCommand = new RelayCommand(_ => Calculate((left, right) => left + right, "+"));
        SubtractCommand = new RelayCommand(_ => Calculate((left, right) => left - right, "−"));
        MultiplyCommand = new RelayCommand(_ => Calculate((left, right) => left * right, "×"));
        DivideCommand = new RelayCommand(_ => Calculate((left, right) => left / right, "÷"));
    }

    public string LeftText
    {
        get => _leftText;
        set => SetField(ref _leftText, value ?? string.Empty);
    }

    public string RightText
    {
        get => _rightText;
        set => SetField(ref _rightText, value ?? string.Empty);
    }

    public string LeftCanonical
    {
        get => _leftCanonical;
        private set => SetField(ref _leftCanonical, value);
    }

    public string RightCanonical
    {
        get => _rightCanonical;
        private set => SetField(ref _rightCanonical, value);
    }

    public string Result
    {
        get => _result;
        private set => SetField(ref _result, value);
    }

    public string OperationSummary
    {
        get => _operationSummary;
        private set => SetField(ref _operationSummary, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand NormalizeCommand { get; }

    public ICommand AddCommand { get; }

    public ICommand SubtractCommand { get; }

    public ICommand MultiplyCommand { get; }

    public ICommand DivideCommand { get; }

    private void Normalize()
    {
        try
        {
            var left = RationalNumber.Parse(LeftText);
            var right = RationalNumber.Parse(RightText);
            LeftCanonical = left.ToString();
            RightCanonical = right.ToString();
            Result = string.Empty;
            OperationSummary = $"Left = {LeftCanonical} • Right = {RightCanonical}";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException or DivideByZeroException)
        {
            ClearOutputs(exception.Message);
        }
    }

    private void Calculate(Func<RationalNumber, RationalNumber, RationalNumber> operation, string symbol)
    {
        try
        {
            var left = RationalNumber.Parse(LeftText);
            var right = RationalNumber.Parse(RightText);
            var result = operation(left, right);

            LeftCanonical = left.ToString();
            RightCanonical = right.ToString();
            Result = result.ToString();
            OperationSummary = $"{LeftCanonical} {symbol} {RightCanonical} = {Result}";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException or DivideByZeroException)
        {
            ClearOutputs(exception.Message);
        }
    }

    private void ClearOutputs(string error)
    {
        LeftCanonical = string.Empty;
        RightCanonical = string.Empty;
        Result = string.Empty;
        OperationSummary = string.Empty;
        ErrorMessage = error;
    }
}
