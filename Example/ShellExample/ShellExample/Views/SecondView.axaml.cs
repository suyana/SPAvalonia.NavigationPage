using Avalonia.Markup.Xaml;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views;

public partial class SecondView : Page
{
	public SecondView()
	{
		InitializeComponent();
	}

	private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}
}

