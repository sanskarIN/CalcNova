using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Controls;

public sealed class RationalNumberPanel : Border
{
    public RationalNumberPanel()
    {
        DataContext = new RationalNumberViewModel();
        Padding = new Thickness(10);
        CornerRadius = new CornerRadius(10);

        var leftInput = CreateBoundTextBox(
            "Left exact value",
            nameof(RationalNumberViewModel.LeftText),
            static viewModel => viewModel.LeftText,
            static (viewModel, value) => viewModel.LeftText = value);
        var rightInput = CreateBoundTextBox(
            "Right exact value",
            nameof(RationalNumberViewModel.RightText),
            static viewModel => viewModel.RightText,
            static (viewModel, value) => viewModel.RightText = value);

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
                        CreateCommandButton("Normalize", nameof(RationalNumberViewModel.NormalizeCommand), static viewModel => viewModel.NormalizeCommand),
                        CreateCommandButton("+", nameof(RationalNumberViewModel.AddCommand), static viewModel => viewModel.AddCommand),
                        CreateCommandButton("−", nameof(RationalNumberViewModel.SubtractCommand), static viewModel => viewModel.SubtractCommand),
                        CreateCommandButton("×", nameof(RationalNumberViewModel.MultiplyCommand), static viewModel => viewModel.MultiplyCommand),
                        CreateCommandButton("÷", nameof(RationalNumberViewModel.DivideCommand), static viewModel => viewModel.DivideCommand)
                    }
                },
                CreateBoundTextBlock(nameof(RationalNumberViewModel.OperationSummary), static viewModel => viewModel.OperationSummary),
                CreateBoundTextBlock(nameof(RationalNumberViewModel.ErrorMessage), static viewModel => viewModel.ErrorMessage)
            }
        };
    }

    private static TextBox CreateBoundTextBox(
        string watermark,
        string propertyName,
        Func<RationalNumberViewModel, string> getter,
        Action<RationalNumberViewModel, string> setter)
    {
        var textBox = new TextBox
        {
            PlaceholderText = watermark,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        textBox.Bind(TextBox.TextProperty, TrimSafeBinding.TwoWay(propertyName, getter, setter));
        return textBox;
    }

    private static Button CreateCommandButton(
        string label,
        string commandPropertyName,
        Func<RationalNumberViewModel, ICommand> getter)
    {
        var button = new Button
        {
            Content = label,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            Margin = new Thickness(0, 0, 8, 4)
        };
        button.Bind(Button.CommandProperty, TrimSafeBinding.OneWay(commandPropertyName, getter));
        return button;
    }

    private static TextBlock CreateBoundTextBlock(string propertyName, Func<RationalNumberViewModel, string> getter)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.82
        };
        textBlock.Bind(TextBlock.TextProperty, TrimSafeBinding.OneWay(propertyName, getter));
        return textBlock;
    }
}
