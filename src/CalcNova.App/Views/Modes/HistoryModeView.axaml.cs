using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;

namespace CalcNova.App.Views.Modes;

public partial class HistoryModeView : UserControl
{
    public HistoryModeView()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async void ExportCsv(object? sender, RoutedEventArgs eventArgs)
    {
        if (DataContext is not HistoryViewModel viewModel)
        {
            return;
        }

        if (viewModel.Entries.Count == 0)
        {
            viewModel.ReportStatus("There are no visible history entries to export.");
            return;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is not { CanSave: true } storageProvider)
        {
            viewModel.ReportStatus("This platform does not currently provide a save-file picker.");
            return;
        }

        try
        {
            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export CalcNova history",
                SuggestedFileName = $"CalcNova_History_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv",
                DefaultExtension = "csv",
                ShowOverwritePrompt = true
            });

            if (file is null)
            {
                viewModel.ReportStatus("History export canceled.");
                return;
            }

            var csv = HistoryExportFormatter.ToCsv(viewModel.Entries);
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            await writer.WriteAsync(csv);
            await writer.FlushAsync();
            viewModel.ReportStatus($"Exported {viewModel.Entries.Count} history entr{(viewModel.Entries.Count == 1 ? "y" : "ies")}.");
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            viewModel.ReportStatus($"History could not be exported: {exception.Message}");
        }
    }
}
