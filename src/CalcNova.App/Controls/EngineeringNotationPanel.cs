using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CalcNova.App.ViewModels;
using CalcNova.Core.Numerics;

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
            PlaceholderText = "Finite value or engineering notation",
            MaxLength = EngineeringNotationFormatter.MaximumInputCharacters,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch
        };
        input.Bind(
            TextBox.TextProperty,
            TrimSafeBinding.TwoWay<EngineeringNotationViewModel, string>(
                nameof(EngineeringNotationViewModel.InputText),
                static viewModel => viewModel.InputText,
                static (viewModel, value) => viewModel.InputText = value));

        var precision = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 15,
            Increment = 1,
            FormatString = "0",
            Width = 100,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
        precision.Bind(
            NumericUpDown.ValueProperty,
            TrimSafeBinding.TwoWay<EngineeringNotationViewModel, int>(
                nameof(EngineeringNotationViewModel.SignificantDigits),
                static viewModel => viewModel.SignificantDigits,
                static (viewModel, value) => viewModel.SignificantDigits = value));

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
                CreateCommandButton("Format", nameof(EngineeringNotationViewModel.FormatCommand), static viewModel => viewModel.FormatCommand),
                CreateCommandButton("Parse", nameof(EngineeringNotationViewModel.ParseCommand), static viewModel => viewModel.ParseCommand),
                CreateBoundTextBlock("Engineering: ", nameof(EngineeringNotationViewModel.FormattedText), static viewModel => viewModel.FormattedText),
                CreateBoundTextBlock("Value: ", nameof(EngineeringNotationViewModel.ParsedValue), static viewModel => viewModel.ParsedValue),
                CreateBoundTextBlock(string.Empty, nameof(EngineeringNotationViewModel.ErrorMessage), static viewModel => viewModel.ErrorMessage)
            }
        };
    }

    private static Button CreateCommandButton(
        string label,
        string commandPropertyName,
        Func<EngineeringNotationViewModel, ICommand> getter)
    {
        var button = new Button
        {
            Content = label,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left
        };
        button.Bind(Button.CommandProperty, TrimSafeBinding.OneWay(commandPropertyName, getter));
        return button;
    }

    private static TextBlock CreateBoundTextBlock(
        string prefix,
        string propertyName,
        Func<EngineeringNotationViewModel, string> getter)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.82
        };
        textBlock.Bind(
            TextBlock.TextProperty,
            TrimSafeBinding.OneWay(propertyName, getter, string.IsNullOrEmpty(prefix) ? null : prefix + "{0}"));
        return textBlock;
    }
}
