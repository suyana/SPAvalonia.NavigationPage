using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage.Presenters;

public class GenericPresenter : PresenterBase {
	public override async Task PresentAsync(NavigationPage shellView, NavigationChain chain, NavigateType navigateType, CancellationToken cancellationToken) {
		var hostControl = GetHostControl(chain);
		await (shellView.PushViewAsync(hostControl ?? chain.View, navigateType, cancellationToken) ?? Task.CompletedTask);
		await (shellView.NavigationBar?.UpdateAsync(chain.View, navigateType, cancellationToken) ?? Task.CompletedTask);
	}
}