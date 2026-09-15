using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Controls;

public sealed class BivariateStatisticsPanel : Border
{
    public BivariateStatisticsPanel()
    {
        Padding = new Thickness(10);
        CornerRadius = new CornerRadius(10);

        Child = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = "Paired data analysis",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                },
                new TextBlock
                {
                    Text = "Enter matching X and Y datasets to calculate covariance, Pearson correlation, and a least-squares linear regression.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Opacity = 0.68
                },
                CreateBoundTextBox(
                    "X values",
                    nameof(StatisticsViewModel.PairedXText),
                    static viewModel => viewModel.PairedXText,
                    static (viewModel, value) => viewModel.PairedXText = value),
                CreateBoundTextBox(
                    "Y values",
                    nameof(StatisticsViewModel.PairedYText),
                    static viewModel => viewModel.PairedYText,
                    static (viewModel, value) => viewModel.PairedYText = value),
                CreateCommandButton("Analyze pairs", nameof(StatisticsViewModel.AnalyzePairsCommand), static viewModel => viewModel.AnalyzePairsCommand),
                CreateBoundTextBlock(nameof(StatisticsViewModel.BivariateSummary), static viewModel => viewModel.BivariateSummary),
                CreateCommandButton("Copy paired summary", nameof(StatisticsViewModel.CopyBivariateSummaryCommand), static viewModel => viewModel.CopyBivariateSummaryCommand),
                new TextBlock
                {
                    Text = "Regression prediction",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold,
                    Margin = new Thickness(0, 4, 0, 0)
                },
                CreateBoundTextBox(
                    "Prediction X",
                    nameof(StatisticsViewModel.PredictionX),
                    static viewModel => viewModel.PredictionX,
                    static (viewModel, value) => viewModel.PredictionX = value),
                CreateCommandButton("Predict Y", nameof(StatisticsViewModel.PredictCommand), static viewModel => viewModel.PredictCommand),
                CreateBoundTextBlock(nameof(StatisticsViewModel.PredictionResult), static viewModel => viewModel.PredictionResult)
            }
        };
    }

    private static TextBox CreateBoundTextBox(
        string watermark,
        string propertyName,
        Func<StatisticsViewModel, string> getter,
        Action<StatisticsViewModel, string> setter)
    {
        var textBox = new TextBox
        {
            PlaceholderText = watermark,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        textBox.Bind(TextBox.TextProperty, TrimSafeBinding.TwoWay(propertyName, getter, setter));
        return textBox;
    }

    private static TextBlock CreateBoundTextBlock(string propertyName, Func<StatisticsViewModel, string> getter)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.82
        };
        textBlock.Bind(TextBlock.TextProperty, TrimSafeBinding.OneWay(propertyName, getter));
        return textBlock;
    }

    private static Button CreateCommandButton(
        string label,
        string commandPropertyName,
        Func<StatisticsViewModel, ICommand> getter)
    {
        var button = new Button
        {
            Content = label,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
        button.Bind(Button.CommandProperty, TrimSafeBinding.OneWay(commandPropertyName, getter));
        return button;
    }
}
