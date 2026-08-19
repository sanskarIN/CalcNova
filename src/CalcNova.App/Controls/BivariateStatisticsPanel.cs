using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
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
                CreateBoundTextBox("X values", nameof(StatisticsViewModel.PairedXText)),
                CreateBoundTextBox("Y values", nameof(StatisticsViewModel.PairedYText)),
                CreateCommandButton("Analyze pairs", nameof(StatisticsViewModel.AnalyzePairsCommand)),
                CreateBoundTextBlock(nameof(StatisticsViewModel.BivariateSummary)),
                CreateCommandButton("Copy paired summary", nameof(StatisticsViewModel.CopyBivariateSummaryCommand)),
                new TextBlock
                {
                    Text = "Regression prediction",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold,
                    Margin = new Thickness(0, 4, 0, 0)
                },
                CreateBoundTextBox("Prediction X", nameof(StatisticsViewModel.PredictionX)),
                CreateCommandButton("Predict Y", nameof(StatisticsViewModel.PredictCommand)),
                CreateBoundTextBlock(nameof(StatisticsViewModel.PredictionResult))
            }
        };
    }

    private static TextBox CreateBoundTextBox(string watermark, string propertyName)
    {
        var textBox = new TextBox
        {
            Watermark = watermark,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        textBox.Bind(TextBox.TextProperty, new Binding(propertyName) { Mode = BindingMode.TwoWay });
        return textBox;
    }

    private static TextBlock CreateBoundTextBlock(string propertyName)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.82
        };
        textBlock.Bind(TextBlock.TextProperty, new Binding(propertyName));
        return textBlock;
    }

    private static Button CreateCommandButton(string label, string commandPropertyName)
    {
        var button = new Button
        {
            Content = label,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
        button.Bind(Button.CommandProperty, new Binding(commandPropertyName));
        return button;
    }
}
