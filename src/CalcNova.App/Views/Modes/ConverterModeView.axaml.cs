using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views.Modes;

public partial class ConverterModeView : UserControl
{
    public ConverterModeView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void CopyResult(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not ConverterViewModel viewModel || string.IsNullOrWhiteSpace(viewModel.Result))
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
        {
            await clipboard.SetTextAsync(viewModel.Result);
        }
    }
}
