using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphViewportToolbarAccessibilityHeadlessTests
{
    [AvaloniaFact]
    public async Task GraphViewportButtons_InheritTouchTargetAndKeyboardFocusBaseline()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };

        window.Show();
        try
        {
            viewModel.SelectMode(7);
            Dispatcher.UIThread.RunJobs();

            var buttons = view.GetVisualDescendants()
                .OfType<WrapPanel>()
                .Single(panel => panel.Classes.Contains("graph-viewport-toolbar"))
                .Children.OfType<Button>()
                .ToArray();

            Assert.Equal(8, buttons.Length);
            Assert.All(buttons, button =>
            {
                Assert.True(button.Focusable);
                Assert.True(button.MinHeight >= 44d, $"{button.Content} MinHeight was {button.MinHeight}.");
            });
        }
        finally
        {
            window.Close();
        }
    }
}
