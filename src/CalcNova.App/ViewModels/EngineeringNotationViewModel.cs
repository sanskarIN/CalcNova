using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Core.Numerics;

namespace CalcNova.App.ViewModels;

public sealed class EngineeringNotationViewModel : ViewModelBase
{
    private string _inputText = "1234";
    private int _significantDigits = 12;
    private string _formattedText = "1.234e+3";
    private string _parsedValue = "1234";
    private string _errorMessage = string.Empty;

    public EngineeringNotationViewModel()
    {
        FormatCommand = new RelayCommand(_ => Format());
        ParseCommand = new RelayCommand(_ => Parse());
    }

    public string InputText
    {
        get => _inputText;
        set => SetField(ref _inputText, value ?? string.Empty);
    }

    public int SignificantDigits
    {
        get => _significantDigits;
        set => SetField(ref _significantDigits, value);
    }

    public string FormattedText
    {
        get => _formattedText;
        private set => SetField(ref _formattedText, value);
    }

    public string ParsedValue
    {
        get => _parsedValue;
        private set => SetField(ref _parsedValue, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand FormatCommand { get; }

    public ICommand ParseCommand { get; }

    private void Format()
    {
        try
        {
            if (!double.TryParse(InputText, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value))
            {
                throw new FormatException("Enter a finite invariant-culture number to format.");
            }

            FormattedText = EngineeringNotationFormatter.Format(value, SignificantDigits);
            ParsedValue = value.ToString("G17", CultureInfo.InvariantCulture);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            FormattedText = string.Empty;
            ParsedValue = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Parse()
    {
        try
        {
            var value = EngineeringNotationFormatter.Parse(InputText);
            ParsedValue = value.ToString("G17", CultureInfo.InvariantCulture);
            FormattedText = EngineeringNotationFormatter.Format(value, SignificantDigits);
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            FormattedText = string.Empty;
            ParsedValue = string.Empty;
            ErrorMessage = exception.Message;
        }
    }
}
