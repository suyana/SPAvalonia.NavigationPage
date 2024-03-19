using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ShellExample.Views;
using SPAvalonia.NavigationPage;

namespace ShellExample.ViewModels {
    public class SecondViewModel : BasePageViewModel {
        public ICommand BackCommand { get; set; }

        public SecondViewModel() {
            BackCommand = ReactiveCommand.CreateFromTask(BackAsync);
        }
        private Task BackAsync(CancellationToken cancellationToken) {
            return NavigationService.BackAsync(DateTime.Now.ToString(), cancellationToken);
            //return NavigationService.NavigateAsync("/second", argument: argument, cancellationToken);
        }

        public override string Route => "/second";
        public override Page CreateView() {
            return new SecondView();
        }

        private string? _argument;
        public string? Argument {
            get => _argument;
            set => this.RaiseAndSetIfChanged(ref _argument, value);
        }

        public override Task ArgumentAsync(object args, CancellationToken cancellationToken) {
            Argument = args?.ToString();

            
            return base.ArgumentAsync(args, cancellationToken);
        }
    }
}
