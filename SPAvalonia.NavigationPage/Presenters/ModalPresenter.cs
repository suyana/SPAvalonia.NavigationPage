using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage.Presenters;

public class ModalPresenter : PresenterBase
{

	public override Task PresentAsync(NavigationPage shellView,
        NavigationChain chain,
        NavigateType navigateType,
        CancellationToken cancellationToken)
	{
		var hostControl = GetHostControl(chain);

		return shellView.ModalAsync(
			hostControl ?? chain.View,
            navigateType,
            cancellationToken) ?? Task.CompletedTask;
	}
}
