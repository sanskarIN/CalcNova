using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Controls;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;
using Xunit;

namespace CalcNova.App.Tests;

public sealed class GraphViewportToolbarHeadlessTests
{
    [AvaloniaFact]
    public async Task ViewportToolbar_ExposesPanZoomFitAndResetActions()
    {
        var (window, view, viewModel) = await CreateGraphWindowAsync();
        try
        {
            var plot = view.GetVisualDescendants().OfType<GraphPlotControl>().Single();
            var toolbar = view.GetVisualDescendants()
                .OfType<WrapPanel>()
                .Single(panel => panel.Classes.Contains("graph-viewport-toolbar"));
            var buttons = toolbar.Children.OfType<Button>().ToArray();

            Assert.Equal(8, buttons.Length);
            Assert.Contains(buttons, button => Equals(button.Content, "Pan left"));
            Assert.Contains(buttons, button => Equals(button.Content, "Zoom in"));
            Assert.Contains(buttons, button => Equals(button.Content, "Fit graph"));
            Assert.Contains(buttons, button => Equals(button.Content, "Reset"));

            var initial = plot.Viewport;
            Click(buttons, "Pan right");
            Assert.True(plot.Viewport.MinimumX > initial.MinimumX);

            var panned = plot.Viewport;
            Click(buttons, "Zoom in");
            Assert.True(plot.Viewport.Width < panned.Width);

            Click(buttons, "Reset");
            Assert.Equal(-10d, plot.Viewport.MinimumX);
            Assert.Equal(10d, plot.Viewport.MaximumX);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public async Task HindiCulture_LocalizesViewportToolbarLabels()
    {
        var (window, view, viewModel) = await CreateGraphWindowAsync();
        try
        {
            viewModel.Settings.CultureName = "hi-IN";
            await viewModel.Settings.SaveAsync();
            Dispatcher.UIThread.RunJobs();

            var buttons = view.GetVisualDescendants()
                .OfType<WrapPanel>()
                .Single(panel => panel.Classes.Contains("graph-viewport-toolbar"))
                .Children.OfType<Button>()
                .Select(button => button.Content?.ToString())
                .ToArray();

            Assert.Contains("बाएँ खिसकाएँ", buttons);
            Assert.Contains("ज़ूम इन", buttons);
            Assert.Contains("ग्राफ़ फ़िट करें", buttons);
            Assert.Contains("रीसेट करें", buttons);
        }
        finally
        {
            window.Close();
        }
    }

    private static void Click(IEnumerable<Button> buttons, string content)
    {
        var button = buttons.Single(item => Equals(item.Content, content));
        button.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
    }

    private static async Task<(Window Window, MainView View, MainViewModel ViewModel)> CreateGraphWindowAsync()
    {
        var viewModel = new MainViewModel();
        await viewModel.InitializeAsync();
        await viewModel.Settings.CompleteOnboardingAsync();
        var view = new MainView { DataContext = viewModel };
        var window = new Window { Width = 980, Height = 780, Content = view };
        window.Show();
        viewModel.SelectMode(7);
        Dispatcher.UIThread.RunJobs();
        return (window, view, viewModel);
    }
}
