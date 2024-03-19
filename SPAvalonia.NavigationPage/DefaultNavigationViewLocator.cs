using System;
using System.Collections.Generic;

namespace SPAvalonia.NavigationPage;

public interface INavigationViewLocator {
    Page GetView(NavigationNode navigationItem);
    void RegisterView(string route,Func<string,Page>  viewFunc);
}

public class DefaultNavigationViewLocator : INavigationViewLocator {
    private Dictionary<string, Func<string, Page>> PageBuilders { get; } = new();

    public Page GetView(NavigationNode navigationItem) {
        if (navigationItem.ViewModel!=null || navigationItem.ViewModelBuilder != null) {
            var view= navigationItem.ViewModel?.View;
            if (view != null) return view;
            throw new TypeLoadException("Cannot create instance of ViewModel type:" + navigationItem.Route);
        }
        if (PageBuilders.TryGetValue(navigationItem.Route, out var func)) {
            return func.Invoke(navigationItem.Route);
        }
        throw new TypeLoadException("Cannot create instance of page type:"+navigationItem.Route);
    }

    public void RegisterView(string route, Func<string, Page> viewFunc) {
        PageBuilders[route] = viewFunc;
    }
}