using System;

namespace SPAvalonia.NavigationPage;

public class NavigationNode {
    public NavigationNode(string route,NavigateType navigate) {
        Route = route;
        Navigate = navigate;
    }

    public NavigationNode(IPageViewModel viewModel, NavigateType navigate) {
        Route=viewModel.Route;
        Navigate = navigate;
        _viewModel = viewModel;
    }

    public NavigationNode(string route, Func<IPageViewModel> viewModelBuilder,NavigateType navigate) {
        Route = route;
        ViewModelBuilder = viewModelBuilder;
        Navigate = navigate;
    }

    public string Route { get; }
    public NavigateType Navigate { get; }

    public Func<IPageViewModel>? ViewModelBuilder { get; }
    private IPageViewModel? _viewModel = null;

    public IPageViewModel? ViewModel {
        get {
            if (_viewModel == null) {
                _viewModel = ViewModelBuilder?.Invoke();
            }

            return _viewModel;
        }
        internal set {
            _viewModel = value;
        }
    }
}