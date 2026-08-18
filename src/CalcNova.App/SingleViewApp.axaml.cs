using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CalcNova.App.Services;
using CalcNova.App.ViewModels;
using CalcNova.App.Views;

namespace CalcNova.App;

public partial class SingleViewApp : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainView
            {
                DataContext = new MainViewModel(AppComposition.Dependencies)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
