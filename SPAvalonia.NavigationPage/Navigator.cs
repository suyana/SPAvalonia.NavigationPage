using Avalonia.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public partial class Navigator : INavigator {
    private readonly INavigateStrategy _navigateStrategy;
    private readonly INavigationUpdateStrategy _updateStrategy;
    private readonly INavigationViewLocator _viewLocator;
    private readonly NavigationStack _stack = new();
    private readonly Dictionary<NavigationChain, TaskCompletionSource<NavigateResult>> _waitingList = new();

    private bool _navigating;
    private NavigationPage? _shellView;
    public NavigationPage ShellView => _shellView ?? throw new ArgumentNullException(nameof(ShellView));
    public string CurrentRoute => _stack.Current?.Uri;
    public INavigationRegistrar Registrar { get; }

    public Navigator(INavigationRegistrar navigationRegistrar, INavigateStrategy navigateStrategy, INavigationUpdateStrategy updateStrategy, INavigationViewLocator viewLocator) {
        Registrar = navigationRegistrar;
        _navigateStrategy = navigateStrategy;
        _updateStrategy = updateStrategy;
        _viewLocator = viewLocator;
    }

    public void RegisterShell(NavigationPage shellView) {
        if (_shellView != null) throw new ArgumentException("Register shell can call only once");
        _shellView = shellView;
    }

    public bool HasItemInStack() {
        var current = _stack.Current?.Back;
        if (current != null) return true;
        return false;
    }

    private async Task NotifyAsync(string origin, string newUri, object? argument, bool hasArgument, object? sender,
        NavigateType? navigateType, bool withAnimation, IPageTransition? overrideTransition, CancellationToken cancellationToken = default) {

        if (!Registrar.TryGetNode(newUri, out var node)) {
            Debug.WriteLine("Warning: Cannot find the path");
            return;
        }

        var instances = new List<Page>();
        var finalNavigateType = !origin.Equals(newUri) && Registrar.TryGetNode(origin, out var originalNode)
                ? navigateType ?? originalNode.Navigate
                : navigateType ?? node.Navigate;

        //生命周期来源（优先使用ViewModel的）
        var prevView = _stack.Current?.View;
        INavigatorLifecycle? navigationLifecycle = _stack.Current?.View.GetNavigatorLifecycle();
        if(navigationLifecycle!=null){
            var args = new NaviagatingEventArgs {
                Sender = sender,
                From = CurrentRoute,
                FromUri = origin,
                ToUri = newUri,
                Argument = argument,
                Navigate = finalNavigateType,
                WithAnimation = withAnimation,
                OverrideTransition = overrideTransition
            };

            await navigationLifecycle.OnNavigatingAsync(args, cancellationToken);
            if (args.Cancel) return;

            //Check for overrides 
            if (argument != args.Argument) {
                argument = args.Argument;
                hasArgument = true;
            }

            finalNavigateType = args.Navigate;
            withAnimation = args.WithAnimation;
            overrideTransition = args.OverrideTransition;
        }

        _navigating = true;

        var stackChanges = _stack.Push(node, finalNavigateType, newUri, instanceFor => {
            var instance = (Page)_viewLocator.GetView(instanceFor);
            SetShellToPage(instance);
            instances.Add(instance);
            return instance;
        });

        await _updateStrategy.UpdateChangesAsync(ShellView, stackChanges, instances, finalNavigateType, argument, hasArgument, cancellationToken);

        CheckWaitingList(stackChanges, argument, hasArgument);

        if (navigationLifecycle != null) {
            var args = new NaviagateEventArgs {
                Sender = sender,
                From = prevView,
                To = _stack.Current?.View,
                FromUri = origin,
                ToUri = newUri,
                Argument = argument,
                Navigate = finalNavigateType,
                WithAnimation = withAnimation,
                OverrideTransition = overrideTransition
            };

            await navigationLifecycle.OnNavigateAsync(args, cancellationToken);
        }

        _navigating = false;
    }

    private void SetShellToPage(object instance) {
        if (instance is Page page) {
            page.Shell = ShellView;
            if (page.DataContext is IPageViewModel pvm) {
                pvm.NavigationService = this;
            }
        }
    }

    public Task NavigateAsync(IPageViewModel vm, NavigateType? navigateType = null, object? argument = null,
        object? sender = null,
        bool withAnimation = true, IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default) {

        if (Registrar.TryGetNode(vm.Route, out var tmp) == false) {
            Registrar.RegistrarViewModel(vm,navigateType??NavigateType.Normal);
        }
        Registrar.SetViewModel(vm.Route, vm);
        return NavigateAsync(vm.Route, navigateType, argument, sender, withAnimation, overrideTransition, cancellationToken);
    }
    public Task<NavigateResult> NavigateAndWaitAsync(IPageViewModel vm, object? argument = null, object? sender = null,
        NavigateType? navigateType = null, bool withAnimation = true,
        IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) {

        if (Registrar.TryGetNode(vm.Route, out var tmp) == false) {
            Registrar.RegistrarViewModel(vm, navigateType ?? NavigateType.Normal);
        }
        Registrar.SetViewModel(vm.Route,vm);
        return NavigateAndWaitAsync(vm.Route,navigateType, argument, argument!=null, sender, withAnimation, overrideTransition,
            cancellationToken);
    }


    public Task NavigateAsync(string path, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, null, null, false, null, true, null, cancellationToken);

    public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, null, argument, true, null, true, null, cancellationToken);

    public Task NavigateAsync(string path, NavigateType? navigateType, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, navigateType, null, false, null, true, null, cancellationToken);

    public Task NavigateAsync(string path, NavigateType? navigateType, object? argument, CancellationToken cancellationToken = default) =>
        NavigateAsync(path, navigateType, argument, true, null, true, null, cancellationToken);

    public Task NavigateAsync(string path, NavigateType? navigateType, object? sender, bool withAnimation = true, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, overrideTransition, cancellationToken);

    public Task NavigateAsync(string path, NavigateType? navigateType, object? argument, object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, overrideTransition, cancellationToken);

    private async Task NavigateAsync(string path, NavigateType? navigateType, object? argument, bool hasArgument, object? sender, bool withAnimation, IPageTransition? overrideTransition, CancellationToken cancellationToken = default) {
        var originalUri = path;
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentRoute, path, cancellationToken);
        if (CurrentRoute != newUri) {
            await NotifyAsync(originalUri, newUri, argument, hasArgument, sender, navigateType, withAnimation, overrideTransition, cancellationToken);
        }
    }

    public Task BackAsync(CancellationToken cancellationToken = default) =>
        BackAsync(null, false, null, true, null, cancellationToken);

    public Task BackAsync(object? argument, CancellationToken cancellationToken = default) =>
        BackAsync(argument, true, null, true, null, cancellationToken);

    public Task BackAsync(object? sender, bool withAnimation = true, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) =>
        BackAsync(null, false, sender, withAnimation, overrideTransition, cancellationToken);

    public Task BackAsync(object? argument, object? sender, bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) => 
        BackAsync(argument, true, sender, withAnimation, overrideTransition, cancellationToken);

    private async Task BackAsync(object? argument, bool hasArgument, object? sender, bool withAnimation, IPageTransition? overrideTransition, CancellationToken cancellationToken = default) {
        var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentRoute, cancellationToken);
        if (newUri != null && CurrentRoute != newUri)
            await NotifyAsync(newUri, newUri, argument, hasArgument, sender, NavigateType.Pop, withAnimation, overrideTransition, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, null, null, false, null, true, null, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, null, argument, true, null, true, null, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(string path, NavigateType navigateType, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, null, true, null, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, NavigateType navigateType, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, null, true, null, cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(string path, object? sender, NavigateType navigateType, bool withAnimation = true,
        IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, overrideTransition,
            cancellationToken);

    public Task<NavigateResult> NavigateAndWaitAsync(string path, object? argument, object? sender, NavigateType navigateType,
        bool withAnimation, IPageTransition? overrideTransition = null, CancellationToken cancellationToken = default) =>
        NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, overrideTransition, cancellationToken);

    private async Task<NavigateResult> NavigateAndWaitAsync(string path, NavigateType? navigateType, object? argument, bool hasArgument,
        object? sender, bool withAnimation, IPageTransition? overrideTransition, CancellationToken cancellationToken = default) {
        var originalUri = path;
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentRoute, path, cancellationToken);
        if (CurrentRoute == newUri)
            return new NavigateResult(false, null); // Or maybe we should throw exception.

        await NotifyAsync(originalUri, newUri, argument, hasArgument, sender, navigateType, withAnimation, overrideTransition, cancellationToken);
        var chain = _stack.Current;

        if (!_waitingList.TryGetValue(chain, out var tcs))
            _waitingList[chain] = tcs = new TaskCompletionSource<NavigateResult>();

        try {
            return await tcs.Task;
        }
        finally {
            _waitingList.Remove(chain);
        }
    }

    private void CheckWaitingList(NavigationStackChanges navigationStackChanges, object? argument, bool hasArgument) {
        if (navigationStackChanges.Removed == null) return;
        foreach (var chain in navigationStackChanges.Removed) {
            if (_waitingList.TryGetValue(chain, out var tcs)) {
                tcs.TrySetResult(new NavigateResult(hasArgument, argument));
            }
        }
    }
}