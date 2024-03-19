using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using ShellExample.Views;
using SPAvalonia.NavigationPage;

namespace ShellExample.ViewModels;

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

        //return NavigationService.NavigateAsync("/second", argument: argument, cancellationToken);
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
