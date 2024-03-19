using System.Collections.Generic;

namespace SPAvalonia.NavigationPage;

public class NavigationStackChanges
{
	public NavigationChain? Previous { get; set; }
	public NavigationChain? Front { get; set; }
	public IList<NavigationChain>? Removed { get; set; }
}
