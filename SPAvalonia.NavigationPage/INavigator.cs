using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;

namespace SPAvalonia.NavigationPage;

public interface INavigator {
    string CurrentRoute { get; }

    INavigationRegistrar Registrar { get; }

    void RegisterShell(NavigationPage shellView);

    bool HasItemInStack();

    Task NavigateAsync(IPageViewModel vm, NavigateType? navigateType = null, object? argument = null,
        object? sender = null,
        bool withAnimation = true, IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(IPageViewModel vm, object? argument = null, object? sender = null,
        NavigateType? navigateType = null, bool withAnimation = true,
        IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);

    Task NavigateAsync(string path, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, NavigateType? navigateType, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, NavigateType? navigateType, object? argument, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, NavigateType? navigateType, object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, NavigateType? navigateType, object? argument, object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);

    Task BackAsync(CancellationToken cancellationToken = default);
    Task BackAsync(object? argument, CancellationToken cancellationToken = default);
    Task BackAsync(object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);
    Task BackAsync(object? argument, object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);

    

    Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(string path, NavigateType navigateType, CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, NavigateType navigateType, CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(string path, object? sender, NavigateType navigateType, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);
    Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, object? sender, NavigateType navigateType, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default);
}