using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using SPAvalonia.NavigationPage;

namespace ShellExample.ViewModels;

public class HomePageViewModel : ViewModelBase
{
	private readonly INavigator _navigationService;

	public ICommand NavigateToSecondPage { get; set; }
	public ICommand ShowDialogCommand { get; set; }

	public HomePageViewModel(INavigator navigationService)
	{
		_navigationService = navigationService;
		NavigateToSecondPage = ReactiveCommand.CreateFromTask(Navigate);
		ShowDialogCommand = ReactiveCommand.CreateFromTask(ShowDialog);
	}

	private Task ShowDialog(CancellationToken cancellationToken)
	{
		return _navigationService.NavigateAsync("/main/home/confirmation", cancellationToken);
	}

	private Task Navigate()
	{
		return _navigationService.NavigateAsync("/second");
	}
}
