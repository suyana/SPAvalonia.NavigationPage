using System;
using Avalonia;
using SPAvalonia.NavigationPage.Presenters;
using Splat;

namespace SPAvalonia.NavigationPage;

public static class AppBuilderExtensions {
    public static void Init(Action<INavigationRegistrar>? registrar=null) {
        if (Locator.CurrentMutable is null) return;

        var _registrar = new NavigationRegistrar();
        Locator.CurrentMutable.Register<INavigationRegistrar>(()=> _registrar);

        Locator.CurrentMutable.Register<IPresenterProvider, PresenterProvider>();

        var _viewLocator = new DefaultNavigationViewLocator();
        Locator.CurrentMutable.Register<INavigationViewLocator>(()=>_viewLocator);

        Locator.CurrentMutable.Register<INavigationUpdateStrategy>(() =>
            new DefaultNavigationUpdateStrategy(Locator.Current.GetService<IPresenterProvider>()!));

        Locator.CurrentMutable.Register<INavigator>(() => {
            return new Navigator(_registrar,
                new RelativeNavigateStrategy(_registrar),
                Locator.Current.GetService<INavigationUpdateStrategy>()!,
                _viewLocator);
        });

        registrar?.Invoke(_registrar);
    }

    public static AppBuilder UseNavigationPage(this AppBuilder builder) => builder.AfterPlatformServicesSetup(_ => { Init(); });
}