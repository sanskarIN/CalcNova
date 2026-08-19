using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Controls;

public sealed class CodePointMetadataPanel : Border
{
    public CodePointMetadataPanel()
    {
        Padding = new Thickness(10);
        CornerRadius = new CornerRadius(10);

        var codePointMetadata = CreateMetadataTextBlock(nameof(CodePointViewModel.CodePointMetadata));
        var textMetadata = CreateMetadataTextBlock(nameof(CodePointViewModel.TextMetadata));

        Child = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new TextBlock
                {
                    Text = "Local Unicode metadata",
                    FontWeight = Avalonia.Media.FontWeight.SemiBold
                },
                new TextBlock
                {
                    Text = "Scalar category, Unicode plane, UTF-8 byte count, and UTF-16 code-unit count are derived locally without a network lookup.",
                    Opacity = 0.62,
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap
                },
                codePointMetadata,
                textMetadata
            }
        };
    }

    private static TextBlock CreateMetadataTextBlock(string propertyName)
    {
        var block = new TextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Opacity = 0.8
        };
        block.Bind(TextBlock.TextProperty, new Binding(propertyName));
        return block;
    }
}
