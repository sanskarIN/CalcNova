using System.Windows.Input;
using CalcNova.App.Infrastructure;

namespace CalcNova.App.ViewModels;

public sealed class BitCellViewModel : ViewModelBase
{
    private readonly Action<int> _toggle;
    private bool _isSet;

    public BitCellViewModel(int index, bool isSet, Action<int> toggle)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        Index = index;
        _isSet = isSet;
        _toggle = toggle ?? throw new ArgumentNullException(nameof(toggle));
        ToggleCommand = new RelayCommand(_ => _toggle(Index));
    }

    public int Index { get; }

    public bool IsSet
    {
        get => _isSet;
        private set
        {
            if (SetField(ref _isSet, value))
            {
                OnPropertyChanged(nameof(Label));
                OnPropertyChanged(nameof(AccessibleLabel));
            }
        }
    }

    public string Label => $"b{Index}: {(IsSet ? 1 : 0)}";

    public string AccessibleLabel => $"Bit {Index}, {(IsSet ? "set" : "clear")}";

    public ICommand ToggleCommand { get; }

    internal void Update(bool isSet) => IsSet = isSet;
}
