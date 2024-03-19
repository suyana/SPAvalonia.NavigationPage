using Avalonia.Markup.Xaml;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views;

public partial class ProfileView : Page
{
	public ProfileView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-user";
}

