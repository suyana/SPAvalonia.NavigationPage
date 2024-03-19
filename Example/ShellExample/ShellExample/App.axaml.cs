using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using ShellExample.ViewModels;
using ShellExample.Views;
using SPAvalonia.NavigationPage;

namespace ShellExample;

public partial class App : Application {
    public override void Initialize() {
        IconProvider.Current.Register<FontAwesomeIconProvider>();

        AppBuilderExtensions.Init(r => {
            r.RegistrarViewModel(new WelcomeViewModel(), NavigateType.Normal);//����ViewModelע��·��
            //r.RegisterRoute("/second", _ => new SecondView(), NavigateType.Normal);//ע����ViewModel·��
            //r.RegistrarViewModel("/second", () => new SecondViewModel(), NavigateType.Modal);//����ViewModel����ע��·��
        });

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            desktop.MainWindow = new MainWindow {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform) {
            singleViewPlatform.MainView = new MainView {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
