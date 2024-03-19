using System.Threading;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views;

public partial class HomePage : Page
{
	public HomePage()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

    public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
		DataContext = new ViewModels.HomePageViewModel(Navigator);
		return Task.CompletedTask;
	}

    public string Icon => "fa-solid fa-house";
}
