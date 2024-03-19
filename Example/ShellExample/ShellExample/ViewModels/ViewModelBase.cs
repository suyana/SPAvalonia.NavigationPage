using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using SPAvalonia.NavigationPage;

namespace ShellExample.ViewModels;

public abstract class BasePageViewModel : ReactiveObject, IPageViewModel,INavigationLifecycle,INavigatorLifecycle {
    public abstract string Route { get; }

    private Page? _view = null;
    public Page? View {
        get{
            if (_view == null) View=CreateView();
            return _view;
        }
        set {
            if (_view != null) {
                _view.DataContext = null;
            }

            _view = value;

            if (_view != null) {
                _view.DataContext = this;
            }
        }
    }

    public abstract Page CreateView();

    public INavigator NavigationService { get; set; }

    #region INavigationLifecycle
    public virtual Task AppearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task ArgumentAsync(object args, CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task DisappearAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task InitialiseAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public virtual Task TerminateAsync(CancellationToken cancellationToken) {
        View = null;
        return Task.CompletedTask;
    }
    #endregion

    #region INavigatorLifecycle
    public virtual Task OnNavigateAsync(NaviagateEventArgs args, CancellationToken cancellationToken) => Task.CompletedTask;
    public virtual Task OnNavigatingAsync(NaviagatingEventArgs args, CancellationToken cancellationToken) => Task.CompletedTask;
    #endregion
}
public class ViewModelBase : ReactiveObject
{
}
