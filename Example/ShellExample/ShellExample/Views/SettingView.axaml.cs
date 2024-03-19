using System.Threading;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using ShellExample.ViewModels;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views;

public partial class SettingView : Page
{
	public SettingView()
	{
		InitializeComponent();
	}

	public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
        DataContext = new SettingViewModel()
        {
            MainViewModel = (MainViewModel)MainView.Current.DataContext
        };

        return base.InitialiseAsync(cancellationToken);
	}

    private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-user";
}

