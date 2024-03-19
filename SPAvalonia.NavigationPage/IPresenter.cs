using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public interface IPresenter {
    Task PresentAsync(NavigationPage shellView, NavigationChain chain, NavigateType navigateType,
        CancellationToken cancellationToken);
}