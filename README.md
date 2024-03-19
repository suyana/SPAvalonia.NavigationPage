<h1>SPAvalonia.NavigationPage</h1>

[中文](README_CN.md) /  [英文](README.md)

## Description
This project is based on：AvaloniaInside.Shell([https://github.com/AvaloniaInside/Shell](https://github.com/AvaloniaInside/Shell)).  
The project was modified, but due to a large amount of modifications, it was not submitted to the original warehouse.  
This project provides a NavigationPage similar to Xamarin/Maui.


## The differences of AvaloniaInside.Shell
- Provide Mvvm support and use ViewModel for navigation
- Register routes using code instead of directly configuring them on axaml, and allow routes not to be registered when using the instance of ViewModel (the program will automatically register before navigation)
- ViewModel is responsible for creating the View and automatically setting the DataContext (only providing interfaces, you need to implement a BasePageViewModel to assign values)
- ViewModel supports lifecycle management (ViewModel needs to inherit  lifecycle interfaces). When View and ViewModel implement the lifecycle interface at the same time, prioritize calling ViewModel (in which case View will not be called)
- Modify the initialization method. The original project's initialization method is on multiple platforms, and each platform needs to reference the library, which is not actually needed. Change it to shared project initialization
- Reserved routing address for supporting navigation without ViewModel
- Does not support the Shell layout of the original project

## Installation

Currently not published to NuGet, please manually download the code and add references:


Call Init at 'App. xmal. cs' for initialization (while registering the route).
```csharp
AppBuilderExtensions.Init(r => {
    //Register routing based on ViewModel
    r.RegistrarViewModel(new WelcomeViewModel(), NavigateType.Normal);

    //Registering a route without ViewModel
    //r.RegisterRoute("/second", _ => new SecondView(), NavigateType.Normal);

    //Register routing based on ViewModel Type
    //r.RegistrarViewModel("/second", ()=>new SecondViewModel(), NavigateType.Modal);
});

AvaloniaXamlLoader.Load(this);
```

Use Style at `App.axmal`
```xml
<StyleInclude Source="avares://SPAvalonia.NavigationPage/Default.axaml"></StyleInclude>
```

## Use NavigationPage


```xml
  <NavigationPage Name="ShellViewMain"
	              HorizontalAlignment="Stretch"
	              VerticalAlignment="Stretch"
                  CloseOnClickAway="True" IsEscBack="True"
	   			  DefaultPageTransition="{Binding CurrentTransition}">
```

#### NavigationPage Property
DefaultRoute：string. Initial route, if not set, defaults to navigation to the first registered route 
CloseOnClickAway：bool. Set whether to allow clicking on blank spaces to close popup when Modal is opened  
IsEscBack：bool. Set whether to perform the Back operation to return to the previous page when pressing Esc on the keyboard

### NavigationBar （No difference from the original project）
![image](https://user-images.githubusercontent.com/956077/227613963-9b1a10b5-c2b0-4dcb-ba43-cd72f3a27333.png)

Each page that is currently on top of the navigation stack has access to the navigation bar's title and navigation item. In hierarchical hosts, the currently selected item in the host will be the one that has access to the navigation bar. For example, in the case of /home/pets/cat, the page associated with the cat would be able to modify the navigation bar. This can be done by setting the NavigationBar.Header and NavigationBar.Item properties, as shown in the code snippet below:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             x:Class="ShellExample.Views.ShopViews.ProductCatalogView"
             NavigationBar.Header="Products">
	<NavigationBar.Item>
		<Button Content="Filter" Command="{Binding FilterCommand}"></Button>
	</NavigationBar.Item>
 ...
</UserControl>
```

## IPageViewModel
```
public interface IPageViewModel {
    string Route { get; }
    Page View { get;}
    Page CreateView();
    INavigator NavigationService { get; set; }
}
```
ViewModel must inherit IPageViewModel  
IPageViewModel returns a unique routing address  
Provide methods for creating Page views  
Provide navigation service objects  

## Call Navigation
```
internal class WelcomeViewModel : BasePageViewModel {
    public ICommand OpenCommand { get; set; }
    public WelcomeViewModel() {
        OpenCommand = ReactiveCommand.CreateFromTask(OpenAsync);
    }

    SecondViewModel vm=new SecondViewModel();
    private async Task OpenAsync(CancellationToken cancellationToken) {
        var text = View.FindControl<TextBox>("txt");
        var argument = "null";
        if(text != null) { argument=text.Text; }

        var r = await NavigationService.NavigateAndWaitAsync(vm,navigateType:NavigateType.Normal, argument: argument, cancellationToken: cancellationToken);
        Debug.WriteLine(r.Argument);
    }

    public override string Route => "/welcome";
    public override Page CreateView() {
        return new WelcomeView();
    }

    public override Task AppearAsync(CancellationToken cancellationToken) {
        Debug.WriteLine("Welcome AppearAsync");
        return base.AppearAsync(cancellationToken);
    }
}
```
In the above code, there is no need to register a SecondViewModel route, and the program will automatically register a route during navigation

## Platform support
Passed tests on Windows, Android, and Web, but not on other platforms.
