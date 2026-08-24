using System.Windows.Input;
using CalcNova.App.Infrastructure;
using CalcNova.Platform.External;

namespace CalcNova.App.ViewModels;

public sealed class AboutViewModel : ViewModelBase
{
    private static readonly Uri RepositoryUri = new("https://github.com/sanskarIN/CalcNova");
    private static readonly Uri GitHubProfileUri = new("https://www.github.com/sanskarIN");
    private static readonly Uri BuyMeACoffeeUri = new("https://buymeacoffee.com/sanskarIN");
    private static readonly Uri BusinessEmailUri = new("mailto:sanskarin@outlook.in");
    private static readonly Uri SecondaryBusinessEmailUri = new("mailto:sanskarin.business@gmail.com");
    private static readonly Uri SupportEmailUri = new("mailto:supportramsandesh@gmail.com");

    private readonly IExternalLinkService? _externalLinkService;
    private string _statusMessage = string.Empty;

    public AboutViewModel(IExternalLinkService? externalLinkService)
    {
        _externalLinkService = externalLinkService;
        OpenRepositoryCommand = CreateCommand(RepositoryUri);
        OpenGitHubProfileCommand = CreateCommand(GitHubProfileUri);
        OpenBuyMeACoffeeCommand = CreateCommand(BuyMeACoffeeUri);
        EmailBusinessCommand = CreateCommand(BusinessEmailUri);
        EmailSecondaryBusinessCommand = CreateCommand(SecondaryBusinessEmailUri);
        EmailSupportCommand = CreateCommand(SupportEmailUri);
    }

    public string ProjectName => "CalcNova";

    public string Version => "2.9.0";

    public string CompletionStatus => "Complete";

    public string ReleaseLabel => $"Version {Version} • {CompletionStatus}";

    public string Tagline => "Fast. Precise. Private. Everywhere.";

    public string License => "Apache-2.0";

    public string Repository => RepositoryUri.AbsoluteUri;

    public string GitHubProfile => GitHubProfileUri.AbsoluteUri;

    public string BuyMeACoffee => BuyMeACoffeeUri.AbsoluteUri;

    public string BusinessEmail => "sanskarin@outlook.in";

    public string SecondaryBusinessEmail => "sanskarin.business@gmail.com";

    public string SupportEmail => "supportramsandesh@gmail.com";

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public ICommand OpenRepositoryCommand { get; }

    public ICommand OpenGitHubProfileCommand { get; }

    public ICommand OpenBuyMeACoffeeCommand { get; }

    public ICommand EmailBusinessCommand { get; }

    public ICommand EmailSecondaryBusinessCommand { get; }

    public ICommand EmailSupportCommand { get; }

    private ICommand CreateCommand(Uri uri) => new AsyncRelayCommand(_ => OpenAsync(uri));

    private async Task OpenAsync(Uri uri)
    {
        if (_externalLinkService is null)
        {
            StatusMessage = "External links are not configured for this platform yet.";
            return;
        }

        try
        {
            await _externalLinkService.OpenAsync(uri);
            StatusMessage = string.Empty;
        }
        catch (Exception exception) when (exception is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            StatusMessage = $"The link could not be opened: {exception.Message}";
        }
    }
}
