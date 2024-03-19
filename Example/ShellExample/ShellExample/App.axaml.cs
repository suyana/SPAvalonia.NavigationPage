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
            r.RegistrarViewModel(new WelcomeViewModel(), NavigateType.Normal);//根据ViewModel注册路由
            //r.RegisterRoute("/second", _ => new SecondView(), NavigateType.Normal);//注册无ViewModel路由
            //r.RegistrarViewModel("/second", () => new SecondViewModel(), NavigateType.Modal);//根据ViewModel类型注册路由
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
