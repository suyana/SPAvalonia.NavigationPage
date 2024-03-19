using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace SPAvalonia.NavigationPage;

public class DefaultNavigationUpdateStrategy : INavigationUpdateStrategy {
    private readonly IPresenterProvider _presenterProvider;

    public DefaultNavigationUpdateStrategy(IPresenterProvider presenterProvider) {
        _presenterProvider = presenterProvider;
    }

    public event EventHandler<HostItemChangeEventArgs>? HostItemChanged;

    public async Task UpdateChangesAsync(NavigationPage shellView, NavigationStackChanges changes, List<Page> newInstances, NavigateType navigateType, object? argument, bool hasArgument, CancellationToken cancellationToken) {
        var isSame = changes.Previous == changes.Front;

        foreach (var instance in newInstances) {
            var navigationLifecycle = instance.GetNavigationLifecycle();
            await navigationLifecycle.InitialiseAsync(cancellationToken);

            SubscribeForUpdateIfNeeded(instance);
        }

        if (changes.Previous?.View !=null && !isSame) {
            var oldInstanceLifecycle = changes.Previous.View.GetNavigationLifecycle();
            await oldInstanceLifecycle?.DisappearAsync(cancellationToken)!;
        }

        if (changes.Removed != null) {
            await InvokeRemoveAsync(shellView, changes.Removed, changes.Previous, navigateType, cancellationToken);
        }

        if (changes.Front?.View!=null) {
            var newInstanceLifecycle = changes.Front.View.GetNavigationLifecycle();
            if (!isSame) await newInstanceLifecycle.AppearAsync(cancellationToken);
            if (hasArgument) await newInstanceLifecycle.ArgumentAsync(argument, cancellationToken);
        }

        if (!isSame && changes.Front != null) {
            await _presenterProvider.For(navigateType).PresentAsync(shellView, changes.Front, navigateType, cancellationToken);
        }
    }

    private async Task InvokeRemoveAsync(NavigationPage shellView, IList<NavigationChain> removed, NavigationChain? previous, NavigateType navigateType, CancellationToken cancellationToken) {
        var presenter = _presenterProvider.Remove();
        foreach (var chain in removed) {
            cancellationToken.ThrowIfCancellationRequested();
            if (previous == chain) {
                await _presenterProvider.Remove().PresentAsync(shellView, previous, navigateType, cancellationToken);
            }
            else {
                await presenter.PresentAsync(shellView, chain, navigateType, cancellationToken);
            }

            if (chain.View!=null) {
                var lifecycle=chain.View.GetNavigationLifecycle();
                await lifecycle.TerminateAsync(cancellationToken);
            }

            UnSubscribeForUpdateIfNeeded(chain.View);
        }
    }

    private void SubscribeForUpdateIfNeeded(object? instance) {
        if (instance is not SelectingItemsControl selectingItemsControl) return;
        selectingItemsControl.SelectionChanged += SelectingItemsControlOnSelectionChanged;
    }

    private void UnSubscribeForUpdateIfNeeded(object instance) {
        if (instance is not SelectingItemsControl selectingItemsControl) return;
        selectingItemsControl.SelectionChanged -= SelectingItemsControlOnSelectionChanged;
    }

    private void SelectingItemsControlOnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (e.AddedItems?.Count > 0 && e.AddedItems[0] is NavigationChain chain) {
            HostItemChanged?.Invoke(this, new HostItemChangeEventArgs(
                e.RemovedItems?.Count > 0 ? e.RemovedItems[0] as NavigationChain : null,
                chain));
        }
    }
}