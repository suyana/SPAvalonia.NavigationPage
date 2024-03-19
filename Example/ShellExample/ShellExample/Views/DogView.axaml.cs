using Avalonia.Markup.Xaml;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views;

public partial class DogView : Page
{
	public DogView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-dog";
}

