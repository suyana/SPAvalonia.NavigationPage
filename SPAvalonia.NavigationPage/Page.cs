using Avalonia.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public class Page : UserControl, INavigationLifecycle, INavigatorLifecycle {
    public NavigationPage? Shell { get; internal set; }
    public INavigator? Navigator => Shell?.Navigator;

    #region INavigationLifecycle

    public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task TerminateAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #endregion

    #region INavigatorLifecycle

    public virtual Task OnNavigateAsync(NaviagateEventArgs args, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public virtual Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    #endregion

    internal INavigationLifecycle GetNavigationLifecycle() {
        if (this.DataContext!=null && this.DataContext is INavigationLifecycle ic) return ic;
        return this;
    }
    internal INavigatorLifecycle GetNavigatorLifecycle() {
        if (this.DataContext != null && this.DataContext is INavigatorLifecycle ic) return ic;
        return this;
    }
}