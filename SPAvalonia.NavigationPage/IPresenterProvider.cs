namespace SPAvalonia.NavigationPage;

public interface IPresenterProvider
{
	IPresenter For(NavigateType type);
	IPresenter Remove();
}
