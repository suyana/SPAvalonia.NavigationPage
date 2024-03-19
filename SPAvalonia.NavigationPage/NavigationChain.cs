using System;
using System.Collections.Generic;

namespace SPAvalonia.NavigationPage;

public class NavigationChain {
    public NavigationNode Node { get; set; } = default!;
    public Page View { get; set; } = default!;
    public NavigateType Type { get; set; }
    public string Uri { get; set; } = default!;
    public NavigationChain? Back { get; set; }

    public IEnumerable<NavigationChain> GetAscendingNodes() {
        yield return this;
        if (Back == null) yield break;

        foreach (var node in Back.GetAscendingNodes()) {
            yield return node;
        }
    }
}