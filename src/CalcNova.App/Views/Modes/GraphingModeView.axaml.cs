using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CalcNova.App.Views.Modes;

public partial class GraphingModeView : UserControl
{
    public GraphingModeView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void FitData(object? sender, RoutedEventArgs eventArgs) => PlotControl?.FitToData();

    private void ResetViewport(object? sender, RoutedEventArgs eventArgs) => PlotControl?.ResetViewport();
}
