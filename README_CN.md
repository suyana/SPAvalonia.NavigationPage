<h1>SPAvalonia.NavigationPage</h1>

[中文](README_CN.md) /  [英文](README.md)

## 项目说明
本项目基于：AvaloniaInside.Shell([https://github.com/AvaloniaInside/Shell](https://github.com/AvaloniaInside/Shell))项目修改而来，因修改内容较多，就没有提交到原仓库。  
本项目提供类似Xamarin/Maui的NavigationPage页。Shell页后续可能会提供吧（目前没用到且刚入门Avalonia，先做个App测试下）

## 与AvaloniaInside.Shell的不同点
- 提供Mvvm支持，使用ViewModel进行导航
- 使用代码注册路由，而不是像原项目那种直接配置在axaml上，且使用ViewModel的实例导航时，允许不注册路由（程序在导航前会自动注册）
- 由ViewModel负责创建View，并自动设置DataContext(只提供接口，自己需实现个BasePageViewModel来赋值)
- ViewModel支持生命周期管理(ViewModel需继承相关的生命周期接口)。当View和ViewModel同时实现生命周期接口时，优先调用ViewModel（此时View的不会被调用）
- 修改初始化方法，原项目的初始化方法在多平台上，每个平台都需要引用该库，实际并不需要，改在共享项目初始化
- 保留了路由地址用于支持无ViewModel的导航方式
- 不支持原项目的Shell那种布局

## Installation

目前尚未发布到NuGet，请手工下载代码并添加引用:


在`App.axmal.cs`处调用Init进行初始化（同时注册路由）.
```csharp
AppBuilderExtensions.Init(r => {
    //根据ViewModel注册路由
    r.RegistrarViewModel(new WelcomeViewModel(), NavigateType.Normal);

    //注册无ViewModel路由
    //r.RegisterRoute("/second", _ => new SecondView(), NavigateType.Normal);

    //根据ViewModel类型注册路由
    //r.RegistrarViewModel("/second", ()=>new SecondViewModel(), NavigateType.Modal);
});

AvaloniaXamlLoader.Load(this);
```

添加样式到`App.axmal`
```xml
<StyleInclude Source="avares://SPAvalonia.NavigationPage/Default.axaml"></StyleInclude>
```

## 使用 NavigationPage


```xml
  <!--如果不设置DefaultRoute，则默认导航到第一个路由。DefaultRoute="/welcome"-->
  <NavigationPage Name="ShellViewMain"
	              HorizontalAlignment="Stretch"
	              VerticalAlignment="Stretch"
                  CloseOnClickAway="True" IsEscBack="True"
	   			  DefaultPageTransition="{Binding CurrentTransition}">
```

#### NavigationPage属性
DefaultRoute：string 初始路由，如果未设置则默认导航到第一个注册路由上  
CloseOnClickAway：bool 设置当Modal打开时，是否允许点击空白处关闭弹窗  
IsEscBack：bool 设置当按下键盘Esc时，是否执行Back操作返回上一页

### NavigationBar （与原项目没有区别）
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
ViewModel必须继承IPageViewModel  

IPageViewModel返回一个路由地址（唯一）  
提供创建Page视图方法  
提供导航服务对象  

## 调用导航
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
在上面代码中，无需注册SecondViewModel路由，程序在导航时会自动注册路由

## 平台支持
在Windows、Android、Web测试通过，其他平台未测试。