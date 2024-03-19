using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage.Presenters;

public class RemovePresenter : PresenterBase
{
	public override Task PresentAsync(NavigationPage shellView,
        NavigationChain chain,
        NavigateType navigateType,
        CancellationToken cancellationToken) =>
		shellView?.RemoveViewAsync(chain.View, navigateType, cancellationToken) ?? Task.CompletedTask;
}
