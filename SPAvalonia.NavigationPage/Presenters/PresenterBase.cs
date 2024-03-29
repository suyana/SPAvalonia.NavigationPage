using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace SPAvalonia.NavigationPage.Presenters;

public abstract class PresenterBase : IPresenter {
    protected object GetHostControl(NavigationChain chain) {
        //if (!chain.Hosted) 
            return chain.View;

        ////var current = chain;
        ////while (current != null) {
        ////    if (current.Back is HostNavigationChain { Instance: ItemsControl itemsControl } parent) {
        ////        if ((itemsControl.Items ?? itemsControl.ItemsSource) is not IList collection) {
        ////            itemsControl.ItemsSource = collection = new AvaloniaList<object>();
        ////        }

        ////        foreach (var hostedChildChain in parent.Nodes.Where(hostedChildChain =>
        ////                     !collection.Contains(hostedChildChain))) {
        ////            collection.Add(hostedChildChain);
        ////        }

        ////        if (itemsControl is SelectingItemsControl selectingItemsControl) {
        ////            selectingItemsControl.SelectedItem = current;
        ////        }
        ////    }
        ////    else {
        ////        break;
        ////    }

        ////    current = current.Back;
        ////}

        //return current?.Instance ?? chain.Instance;
    }

    public abstract Task PresentAsync(NavigationPage shellView, NavigationChain chain, NavigateType navigateType,
        CancellationToken cancellationToken);
}