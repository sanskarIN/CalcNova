using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CalcNova.App.Localization;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views;

public partial class MainView
{
    private readonly Dictionary<CheckBox, AppStringKey> _localizedCheckBoxes = new();
    private MainViewModel? _checkBoxLocalizationViewModel;
    private TabControl? _checkBoxLocalizationTabControl;
    private Border? _converterPreferenceNotice;
    private TextBlock? _converterPreferenceTitle;
    private TextBlock? _converterPreferenceBody;
    private WrapPanel? _graphViewportToolbar;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        DataContextChanged += HandleCheckBoxLocalizationDataContextChanged;
        AttachCheckBoxLocalization();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DataContextChanged -= HandleCheckBoxLocalizationDataContextChanged;
        DetachCheckBoxLocalization();
        base.OnDetachedFromVisualTree(e);
    }

    private void HandleCheckBoxLocalizationDataContextChanged(object? sender, EventArgs e) =>
        AttachCheckBoxLocalization();

    private void AttachCheckBoxLocalization()
    {
        DetachCheckBoxLocalization();

        if (DataContext is not MainViewModel viewModel)
        {
            return;
        }

        _checkBoxLocalizationViewModel = viewModel;
        viewModel.Localizer.CultureChanged += HandleCheckBoxCultureChanged;
        _checkBoxLocalizationTabControl = this.GetVisualDescendants().OfType<TabControl>().FirstOrDefault();
        if (_checkBoxLocalizationTabControl is not null)
        {
            _checkBoxLocalizationTabControl.SelectionChanged += HandleCheckBoxTabSelectionChanged;
        }

        RefreshLocalizedCheckBoxes();
    }

    private void DetachCheckBoxLocalization()
    {
        if (_checkBoxLocalizationViewModel is not null)
        {
            _checkBoxLocalizationViewModel.Localizer.CultureChanged -= HandleCheckBoxCultureChanged;
        }

        if (_checkBoxLocalizationTabControl is not null)
        {
            _checkBoxLocalizationTabControl.SelectionChanged -= HandleCheckBoxTabSelectionChanged;
        }

        RemoveGraphViewportToolbar();
        _checkBoxLocalizationViewModel = null;
        _checkBoxLocalizationTabControl = null;
        _localizedCheckBoxes.Clear();
    }

    private void HandleCheckBoxCultureChanged(CultureInfo culture) => RefreshLocalizedCheckBoxes();

    private void HandleCheckBoxTabSelectionChanged(object? sender, SelectionChangedEventArgs e) =>
        RefreshLocalizedCheckBoxes();

    private void RefreshLocalizedCheckBoxes()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            CaptureAndApplyLocalizedCheckBoxes();
            return;
        }

        Dispatcher.UIThread.Post(CaptureAndApplyLocalizedCheckBoxes);
    }

    private void CaptureAndApplyLocalizedCheckBoxes()
    {
        var localizer = _checkBoxLocalizationViewModel?.Localizer;
        if (localizer is null)
        {
            return;
        }

        EnsureConverterPreferenceNotice();
        EnsureGraphViewportToolbar();

        foreach (var checkBox in this.GetVisualDescendants().OfType<CheckBox>())
        {
            if (!_localizedCheckBoxes.ContainsKey(checkBox) &&
                checkBox.Content is string literal &&
                ShellLocalization.TryGetLiteralKey(literal, out var key))
            {
                _localizedCheckBoxes[checkBox] = key;
            }
        }

        foreach (var (checkBox, key) in _localizedCheckBoxes)
        {
            checkBox.Content = localizer[key];
        }

        if (_converterPreferenceTitle is not null)
        {
            _converterPreferenceTitle.Text = localizer[AppStringKey.ConverterPreferencesTitle];
        }

        if (_converterPreferenceBody is not null)
        {
            _converterPreferenceBody.Text = localizer[AppStringKey.ConverterPreferencesBody];
        }
    }

    private void EnsureConverterPreferenceNotice()
    {
        if (_converterPreferenceNotice is not null)
        {
            return;
        }

        var converter = _checkBoxLocalizationViewModel?.Converter;
        if (converter is null)
        {
            return;
        }

        var panel = this.GetVisualDescendants()
            .OfType<StackPanel>()
            .FirstOrDefault(candidate => ReferenceEquals(candidate.DataContext, converter));
        if (panel is null)
        {
            return;
        }

        _converterPreferenceTitle = new TextBlock
        {
            FontWeight = Avalonia.Media.FontWeight.SemiBold,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        _converterPreferenceBody = new TextBlock
        {
            Opacity = 0.72,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap
        };
        var content = new StackPanel { Spacing = 4 };
        content.Children.Add(_converterPreferenceTitle);
        content.Children.Add(_converterPreferenceBody);

        _converterPreferenceNotice = new Border
        {
            Padding = new Thickness(10),
            CornerRadius = new CornerRadius(10),
            Child = content
        };
        _converterPreferenceNotice.Classes.Add("converter-preference-notice");
        panel.Children.Insert(Math.Min(1, panel.Children.Count), _converterPreferenceNotice);
    }

    private void EnsureGraphViewportToolbar()
    {
        if (_graphViewportToolbar is not null)
        {
            return;
        }

        var graphing = _checkBoxLocalizationViewModel?.Graphing;
        if (graphing is null)
        {
            return;
        }

        EnsureGraphPlot(graphing);
        if (_graphPlotControl is null || _graphPlotControl.Parent is not StackPanel graphPanel)
        {
            return;
        }

        var plot = _graphPlotControl;
        _graphViewportToolbar = new WrapPanel();
        _graphViewportToolbar.Classes.Add("graph-viewport-toolbar");

        AddGraphViewportButton(AppStringKey.ActionGraphPanLeft, plot.PanLeft);
        AddGraphViewportButton(AppStringKey.ActionGraphPanRight, plot.PanRight);
        AddGraphViewportButton(AppStringKey.ActionGraphPanUp, plot.PanUp);
        AddGraphViewportButton(AppStringKey.ActionGraphPanDown, plot.PanDown);
        AddGraphViewportButton(AppStringKey.ActionGraphZoomIn, plot.ZoomIn);
        AddGraphViewportButton(AppStringKey.ActionGraphZoomOut, plot.ZoomOut);
        AddGraphViewportButton(AppStringKey.ActionReset, plot.ResetViewport);
        AddGraphViewportButton(AppStringKey.ActionGraphFit, plot.FitToData);

        var plotIndex = graphPanel.Children.IndexOf(plot);
        graphPanel.Children.Insert(Math.Max(0, plotIndex), _graphViewportToolbar);
    }

    private void AddGraphViewportButton(AppStringKey key, Action action)
    {
        if (_graphViewportToolbar is null)
        {
            return;
        }

        var button = new Button
        {
            Content = _checkBoxLocalizationViewModel?.Localizer[key] ?? key.ToString(),
            Focusable = true,
            MinHeight = 44,
            Margin = new Thickness(0, 0, 6, 6)
        };
        button.Click += (_, _) => action();
        _localizedButtons[button] = key;
        _graphViewportToolbar.Children.Add(button);
    }

    private void RemoveGraphViewportToolbar()
    {
        if (_graphViewportToolbar is null)
        {
            return;
        }

        foreach (var button in _graphViewportToolbar.Children.OfType<Button>())
        {
            _localizedButtons.Remove(button);
        }

        if (_graphViewportToolbar.Parent is Panel panel)
        {
            panel.Children.Remove(_graphViewportToolbar);
        }

        _graphViewportToolbar = null;
    }
}
