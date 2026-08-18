namespace CalcNova.App.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private int _selectedModeIndex;

    public CalculatorViewModel Calculator { get; } = new();

    public ProgrammerViewModel Programmer { get; } = new();

    public ConverterViewModel Converter { get; } = new();

    public StatisticsViewModel Statistics { get; } = new();

    public EquationsViewModel Equations { get; } = new();

    public MatricesViewModel Matrices { get; } = new();

    public GraphingViewModel Graphing { get; } = new();

    public int SelectedModeIndex
    {
        get => _selectedModeIndex;
        set => SetField(ref _selectedModeIndex, value);
    }
}
