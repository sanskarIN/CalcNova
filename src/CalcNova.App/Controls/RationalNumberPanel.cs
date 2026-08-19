using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Controls;

public sealed class RationalNumberPanel : Border
{
    public RationalNumberPanel()
    {
        DataContext = new RationalNumberViewModel();
        Padding = new Thickness(10);
        CornerRadius = new CornerRadius(10);

        var leftInput = CreateBoundTextBox("Left exact value", nameof(RationalNumberViewModel.LeftText));
        var rightInput = CreateBoundTextBox("Right exact value", nameof(RationalNumberViewModel.RightText));

        Child = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = "Exact rational arithmetic",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                },
                new TextBlock
                {
                    Text = "Use fractions, finite decimals, integers, or decimal scientific notation without binary floating-point rounding.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                    Opacity = 0.68
                },
                leftInput,
                rightInput,
                new WrapPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    Children =
                    {
                        CreateCommandButton("Normalize", nameof(RationalNumberViewModel.NormalizeCommand)),
                        CreateCommandButton("+", nameof(RationalNumberViewModel.AddCommand)),
                        CreateCommandButton("−", nameof(RationalNumberViewModel.SubtractCommand)),
                        CreateCommandButton("×", nameof(RationalNumberViewModel.MultiplyCommand)),
                        CreateCommandButton("÷", nameof(RationalNumberViewModel.DivideCommand))
                    }
                },
                CreateBoundTextBlock(nameof(RationalNumberViewModel.OperationSummary)),
                CreateBoundTextBlock(nameof(RationalNumberViewModel.ErrorMessage))
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

    private static Button CreateCommandButton(string label, string commandPropertyName)
    {
        var button = new Button
        {
            Content = label,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 8, 4)
        };
        button.Bind(Button.CommandProperty, new Binding(commandPropertyName));
        return button;
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
}
