using System.Globalization;
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
    }
}
