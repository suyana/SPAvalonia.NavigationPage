using System.Threading;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using ShellExample.ViewModels.ShopViewModels;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views.ShopViews;

public partial class ProductCatalogView : Page
{
	public ProductCatalogView()
	{
		InitializeComponent();
	}

	public override Task InitialiseAsync(CancellationToken cancellationToken)
	{
        DataContext = new ProductCatalogViewModel(Navigator);

        return base.InitialiseAsync(cancellationToken);
	}

    private void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public string Icon => "fa-solid fa-tag";
}

