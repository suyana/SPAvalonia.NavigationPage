using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public interface INavigationUpdateStrategy {
    Task UpdateChangesAsync(NavigationPage shellView, NavigationStackChanges changes, List<Page> newInstances,
        NavigateType navigateType, object? argument, bool hasArgument, CancellationToken cancellationToken);
}