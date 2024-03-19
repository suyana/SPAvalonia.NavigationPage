namespace SPAvalonia.NavigationPage {
    public interface IPageViewModel {
        string Route { get; }
        Page? View { get;}
        Page CreateView();
        INavigator NavigationService { get; set; }
    }
}
