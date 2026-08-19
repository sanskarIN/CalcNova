using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Controls;

public sealed class EngineeringNotationPanel : Border
{
    public EngineeringNotationPanel()
    {
        DataContext = new EngineeringNotationViewModel();
        Padding = new Thickness(10);
        CornerRadius = new CornerRadius(10);

        var input = new TextBox
        {
            Watermark = "Finite value or engineering notation",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        input.Bind(TextBox.TextProperty, new Binding(nameof(EngineeringNotationViewModel.InputText))
        {
            Mode = BindingMode.TwoWay
        });

        var precision = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 15,
            Increment = 1,
            FormatString = "0",
            Width = 100,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
        precision.Bind(NumericUpDown.ValueProperty, new Binding(nameof(EngineeringNotationViewModel.SignificantDigits))
        {
            Mode = BindingMode.TwoWay
        });

        Child = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = "Engineering notation",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                },
                new TextBlock
                {
                    Text = "Format finite values with exponents in multiples of three, or parse canonical engineering notation.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Opacity = 0.68
                },
                input,
                new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Spacing = 8,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Significant digits",
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        },
                        precision
                    }
                },
                CreateCommandButton("Format", nameof(EngineeringNotationViewModel.FormatCommand)),
                CreateCommandButton("Parse", nameof(EngineeringNotationViewModel.ParseCommand)),
                CreateBoundTextBlock("Engineering: ", nameof(EngineeringNotationViewModel.FormattedText)),
                CreateBoundTextBlock("Value: ", nameof(EngineeringNotationViewModel.ParsedValue)),
                CreateBoundTextBlock(string.Empty, nameof(EngineeringNotationViewModel.ErrorMessage))
            }
        };
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

    private static TextBlock CreateBoundTextBlock(string prefix, string propertyName)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.82
        };
        textBlock.Bind(TextBlock.TextProperty, new Binding(propertyName)
        {
            StringFormat = string.IsNullOrEmpty(prefix) ? null : prefix + "{0}"
        });
        return textBlock;
    }
}
