using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views.Modes;

public partial class ProgrammerModeView : UserControl
{
    public ProgrammerModeView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ToggleBit(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is ProgrammerViewModel viewModel && sender is Button { DataContext: ProgrammerBitCell cell })
        {
            viewModel.ToggleBitCommand.Execute(cell);
        }
    }
}
