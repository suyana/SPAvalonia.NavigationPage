using System.Threading;
using System.Threading.Tasks;
using SPAvalonia.NavigationPage;

namespace ShellExample.Views {
    public partial class WelcomeView : Page
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        public override Task InitialiseAsync(CancellationToken cancellationToken)
        {
            //DataContext = new ViewModels.WelcomeViewModel(Navigator);
            return base.InitialiseAsync(cancellationToken);
        }
    }
}
