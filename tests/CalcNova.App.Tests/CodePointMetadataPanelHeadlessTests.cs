using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class CodePointMetadataPanelHeadlessTests
{
    [AvaloniaFact]
    public async Task CodePointMode_SurfacesLocalScalarMetadataPanel()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(2);
            Dispatcher.UIThread.RunJobs();

            var panel = view.GetVisualDescendants().OfType<CodePointMetadataPanel>().Single();
            Assert.Same(viewModel.CodePoint, panel.DataContext);

            viewModel.CodePoint.CodePointInput = "U+1F600";
            viewModel.CodePoint.DecodeCodePointCommand.Execute(null);
            Dispatcher.UIThread.RunJobs();

            Assert.Contains(
                panel.GetVisualDescendants().OfType<TextBlock>(),
                block => block.Text?.Contains("U+1F600", StringComparison.Ordinal) == true &&
                         block.Text.Contains("UTF-8 4 byte", StringComparison.Ordinal));
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task CodePointMetadataPanel_BindsBothCopyCommands()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(2);
            Dispatcher.UIThread.RunJobs();

            var panel = view.GetVisualDescendants().OfType<CodePointMetadataPanel>().Single();
            var buttons = panel.GetVisualDescendants().OfType<Button>().ToArray();
            var scalarButton = Assert.Single(buttons, button =>
                string.Equals(button.Content?.ToString(), "Copy scalar metadata", StringComparison.Ordinal));
            var textButton = Assert.Single(buttons, button =>
                string.Equals(button.Content?.ToString(), "Copy inspected metadata", StringComparison.Ordinal));

            Assert.Same(viewModel.CodePoint.CopyCodePointMetadataCommand, scalarButton.Command);
            Assert.Same(viewModel.CodePoint.CopyTextMetadataCommand, textButton.Command);
        }
        finally
        {
            window.Close();
        }
    }
}
