using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.App.Services;
using CalcNova.Platform.Clipboard;
using CalcNova.Programmer;

namespace CalcNova.App.ViewModels;

public sealed class CodePointViewModel : ViewModelBase
{
    private readonly IClipboardService? _clipboardService;
    private string _codePointInput = "U+0041";
    private string _textInput = "A";
    private string _codePointResult = "U+0041 → A";
    private string _textResult = "U+0041";
    private string _codePointMetadata = UnicodeCodePointHelper.Describe(0x41).CompactSummary;
    private string _textMetadata = UnicodeCodePointHelper.Describe(0x41).CompactSummary;
    private string _errorMessage = string.Empty;

    public CodePointViewModel(IClipboardService? clipboardService = null)
    {
        _clipboardService = clipboardService;
        DecodeCodePointCommand = new RelayCommand(_ => DecodeCodePoint());
        InspectTextCommand = new RelayCommand(_ => InspectText());
        CopyCodePointResultCommand = new AsyncRelayCommand(_ => CopyCodePointResultAsync());
        CopyTextResultCommand = new AsyncRelayCommand(_ => CopyTextResultAsync());
        CopyCodePointMetadataCommand = new AsyncRelayCommand(_ => CopyCodePointMetadataAsync());
        CopyTextMetadataCommand = new AsyncRelayCommand(_ => CopyTextMetadataAsync());
    }

    public string CodePointInput
    {
        get => _codePointInput;
        set => SetField(ref _codePointInput, value ?? string.Empty);
    }

    public string TextInput
    {
        get => _textInput;
        set => SetField(ref _textInput, value ?? string.Empty);
    }

    public string CodePointResult
    {
        get => _codePointResult;
        private set => SetField(ref _codePointResult, value);
    }

    public string TextResult
    {
        get => _textResult;
        private set => SetField(ref _textResult, value);
    }

    public string CodePointMetadata
    {
        get => _codePointMetadata;
        private set => SetField(ref _codePointMetadata, value);
    }

    public string TextMetadata
    {
        get => _textMetadata;
        private set => SetField(ref _textMetadata, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetField(ref _errorMessage, value);
    }

    public ICommand DecodeCodePointCommand { get; }

    public ICommand InspectTextCommand { get; }

    public ICommand CopyCodePointResultCommand { get; }

    public ICommand CopyTextResultCommand { get; }

    public ICommand CopyCodePointMetadataCommand { get; }

    public ICommand CopyTextMetadataCommand { get; }

    private void DecodeCodePoint()
    {
        try
        {
            var value = UnicodeCodePointHelper.Parse(CodePointInput);
            CodePointResult = $"{UnicodeCodePointHelper.Format(value)} → {UnicodeCodePointHelper.ToText(value)}";
            CodePointMetadata = UnicodeCodePointHelper.Describe(value).CompactSummary;
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            CodePointResult = string.Empty;
            CodePointMetadata = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private void InspectText()
    {
        try
        {
            var metadata = UnicodeCodePointHelper.DescribeText(TextInput);
            TextResult = metadata.Count == 0
                ? "No code points."
                : string.Join("  ", metadata.Select(item => item.CodePoint));
            TextMetadata = metadata.Count == 0
                ? string.Empty
                : string.Join(Environment.NewLine, metadata.Select(item => item.CompactSummary));
            ErrorMessage = string.Empty;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            TextResult = string.Empty;
            TextMetadata = string.Empty;
            ErrorMessage = exception.Message;
        }
    }

    private async Task CopyCodePointResultAsync()
    {
        ErrorMessage = await ClipboardTextWriter.CopyAsync(_clipboardService, CodePointResult, "code point result");
    }

    private async Task CopyTextResultAsync()
    {
        ErrorMessage = await ClipboardTextWriter.CopyAsync(_clipboardService, TextResult, "text inspection result");
    }

    private async Task CopyCodePointMetadataAsync()
    {
        ErrorMessage = await ClipboardTextWriter.CopyAsync(_clipboardService, CodePointMetadata, "code point metadata");
    }

    private async Task CopyTextMetadataAsync()
    {
        ErrorMessage = await ClipboardTextWriter.CopyAsync(_clipboardService, TextMetadata, "text metadata");
    }
}
