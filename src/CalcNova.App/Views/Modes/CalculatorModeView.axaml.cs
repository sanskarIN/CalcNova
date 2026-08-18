using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CalcNova.App.ViewModels;
using CalcNova.Core.Evaluation;

namespace CalcNova.App.Views.Modes;

public partial class CalculatorModeView : UserControl
{
    public CalculatorModeView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void CopyResult(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not CalculatorViewModel viewModel)
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        var text = viewModel.Result == "Error" ? viewModel.Expression : viewModel.Result;
        await clipboard.SetTextAsync(text);
    }

    private async void CopyExpression(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not CalculatorViewModel viewModel)
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
        {
            await clipboard.SetTextAsync(viewModel.Expression);
        }
    }

    private async void PasteExpression(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not CalculatorViewModel viewModel)
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            return;
        }

        var text = await clipboard.TryGetTextAsync();
        if (!string.IsNullOrWhiteSpace(text) && text.Length <= EvaluationOptions.Default.MaximumExpressionLength)
        {
            viewModel.Expression = text.Trim();
        }
    }
}
