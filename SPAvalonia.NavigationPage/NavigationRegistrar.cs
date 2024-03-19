using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using DynamicData;
using Splat;

namespace SPAvalonia.NavigationPage;

public interface INavigationRegistrar {
    void RegistrarViewModel(IPageViewModel viewModel, NavigateType navigate);
    void RegistrarViewModel(string route,Func<IPageViewModel> viewModelBuilder, NavigateType navigate);
    void RegisterRoute(string route, Func<string, Page> pageBuilder, NavigateType navigate);
    bool TryGetNode(string path, out NavigationNode node);
    string GetFirstNodeRoute();
    void SetViewModel(string route, IPageViewModel vm);
}

public class NavigationRegistrar : INavigationRegistrar {
    private Dictionary<string, NavigationNode> Navigations { get; } = new();
    private INavigationViewLocator viewLocator;

    void checkRouteExists(string route) {
        if (Navigations.ContainsKey(route)) {
            throw new ArgumentException("route already exists");
        }
    }
    public void RegistrarViewModel(IPageViewModel viewModel, NavigateType navigate) {
        var route = viewModel.Route;
        checkRouteExists(route);

        var node = new NavigationNode(viewModel, navigate);
        Navigations[route] = node;
    }

    public void RegistrarViewModel(string route, Func<IPageViewModel> viewModelBuilder, NavigateType navigate) {
        checkRouteExists(route);

        var node = new NavigationNode(route, viewModelBuilder, navigate);
        Navigations[route] = node;
    }

    public void RegisterRoute(string route, Func<string,Page> pageBuilder, NavigateType navigate) {
        checkRouteExists(route);

        if (viewLocator == null) {
            viewLocator = Locator.Current.GetService<INavigationViewLocator>()!;
        }
        viewLocator?.RegisterView(route,pageBuilder);

        var node = new NavigationNode(route, navigate);
        Navigations[route] = node;
    }

    public bool TryGetNode(string path, out NavigationNode node) => Navigations.TryGetValue(path.ToLower(), out node);
    public string? GetFirstNodeRoute() {
        return Navigations.Keys.FirstOrDefault();
    }

    public void SetViewModel(string route, IPageViewModel vm) {
        if (this.TryGetNode(route, out var node)) {
            node.ViewModel = vm;
        }
    }
}