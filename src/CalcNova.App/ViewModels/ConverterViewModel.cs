using System.Globalization;
using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Converter;

namespace CalcNova.App.ViewModels;

public sealed class ConverterViewModel : ViewModelBase
{
    private readonly UnitConverter _converter = new();
    private UnitCategory _selectedCategory = UnitCategory.Length;
    private IReadOnlyList<UnitDefinition> _availableUnits;
    private UnitDefinition _fromUnit;
    private UnitDefinition _toUnit;
    private string _input = "1";
    private string _result = string.Empty;
    private string _errorMessage = string.Empty;

    public ConverterViewModel()
    {
        _availableUnits = UnitCatalog.ForCategory(_selectedCategory);
        _fromUnit = _availableUnits[0];
        _toUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
        ConvertCommand = new RelayCommand(_ => Convert());
        SwapCommand = new RelayCommand(_ => Swap());
        Convert();
    }

    public IReadOnlyList<UnitCategory> Categories { get; } = Enum.GetValues<UnitCategory>();

    public UnitCategory SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (!SetField(ref _selectedCategory, value))
            {
                return;
            }

            _availableUnits = UnitCatalog.ForCategory(value);
            OnPropertyChanged(nameof(AvailableUnits));
            FromUnit = _availableUnits[0];
            ToUnit = _availableUnits.Count > 1 ? _availableUnits[1] : _availableUnits[0];
            Convert();
        }
    }

    public IReadOnlyList<UnitDefinition> AvailableUnits => _availableUnits;

    public UnitDefinition FromUnit
    {
        get => _fromUnit;
        set
        {
            if (value is not null && SetField(ref _fromUnit, value))
            {
                Convert();
            }
        }
    }

    public UnitDefinition ToUnit
    {
        get => _toUnit;
        set
        {
            if (value is not null && SetField(ref _toUnit, value))
            {
                Convert();
            }
        }
    }

    public string Input
    {
        get => _input;
        set => SetField(ref _input, value ?? string.Empty);
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

    public ICommand ConvertCommand { get; }

    public ICommand SwapCommand { get; }

    private void Convert()
    {
        if (!double.TryParse(Input, NumberStyles.Float, CultureInfo.InvariantCulture, out var input) || !double.IsFinite(input))
        {
            Result = string.Empty;
            ErrorMessage = "Enter a finite number using invariant decimal notation.";
            return;
        }

        try
        {
            var converted = _converter.Convert(input, FromUnit.Id, ToUnit.Id);
            Result = $"{converted.ToString("G15", CultureInfo.InvariantCulture)} {ToUnit.Symbol}";
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is InvalidOperationException or ArgumentException or OverflowException)
        {
            Result = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void Swap()
    {
        (FromUnit, ToUnit) = (ToUnit, FromUnit);
        Convert();
    }
}
