using System;
using System.Collections.Generic;

namespace SPAvalonia.NavigationPage;

public class NavigationStack {
    public NavigationChain? Current { get; set; }

    public NavigationStackChanges Push(NavigationNode node, NavigateType type, string uri, Func<NavigationNode, Page> instance) {
        var chain = type switch {
            NavigateType.ReplaceRoot => PushReplaceRoot(node, type, instance),
            NavigateType.Normal or NavigateType.Modal => PushNormal(node, type, instance),
            NavigateType.Replace => PushReplace(node, type, instance),
            NavigateType.Top => PushTop(node, type, instance),
            NavigateType.Clear => PushClear(node, type, instance),
            NavigateType.Pop => Pop(node, type, uri, instance),
        };

        if (chain.Front != null) chain.Front.Uri = uri;
        return chain;
    }

    private NavigationStackChanges Pop(NavigationNode node, NavigateType type, string uri, Func<NavigationNode, Page> getInstance) {
        if (Current == null) return PushTop(node, type, getInstance);

        var previous = Current;
        var list = new List<NavigationChain>();

        foreach (var chain in Current.GetAscendingNodes()) {
            if (chain.Node == node) {
                Current = chain;
                return new NavigationStackChanges {
                    Front = chain,
                    Removed = list,
                    Previous = previous
                };
            }

            list.Add(chain);
        }

        return PushTop(node, type, getInstance);
    }

    private NavigationStackChanges PushReplaceRoot(NavigationNode node, NavigateType type, Func<NavigationNode, Page> getInstance) {
        var popList = new List<NavigationChain>();
        var chain = Current;
        var previous = Current;

        while (chain != null) {
            popList.Add(chain);
            chain = chain.Back;
        }

        foreach (var pop in popList)
            pop.Back = null;

        Current = new NavigationChain { Node = node, View = getInstance(node), Type = type };
        return new NavigationStackChanges() {
            Front = Current,
            Previous = previous,
            Removed = popList
        };
    }

    private NavigationStackChanges PushNormal(NavigationNode node, NavigateType type, Func<NavigationNode, Page> getInstance) {
        Current = new NavigationChain { Node = node, View = getInstance(node), Type = type, Back = Current };
        return new NavigationStackChanges() {
            Front = Current,
            Previous = Current.Back
        };
    }


    private NavigationStackChanges PushReplace(NavigationNode node, NavigateType type, Func<NavigationNode, Page> getInstance) {
        var pop = Current;

        Current = new NavigationChain { Node = node, View = getInstance(node), Type = type, Back = pop?.Back };
        return new NavigationStackChanges() {
            Previous = pop,
            Front = Current,
            Removed = pop != null ? new List<NavigationChain> { pop } : null
        };
    }

    private NavigationStackChanges PushTop(NavigationNode node, NavigateType type, Func<NavigationNode, Page> getInstance) {
        var previousChain = Current;
        var current = Current;
        NavigationChain? previous = null;
        while (current != null) {
            if (current.Node == node) {
                if (previous != null) {
                    previous.Back = current.Back;
                    current.Back = Current;
                    Current = current;
                }

                current.Type = type;
                return new NavigationStackChanges {
                    Previous = previousChain,
                    Front = Current
                };
            }

            previous = current;
            current = current.Back;
        }

        Current = new NavigationChain { Node = node, View = getInstance(node), Type = type, Back = Current };
        return new NavigationStackChanges() {
            Previous = previousChain,
            Front = Current
        };
    }

    private NavigationStackChanges PushClear(NavigationNode node, NavigateType type, Func<NavigationNode, Page> getInstance) {
        var removedNodes = new List<NavigationChain>();
        var previousChain = Current;
        var current = Current;
        NavigationChain? previous = null;
        while (current != null) {
            if (current.Node == node) {
                removedNodes.Add(current);
                if (previous != null) {
                    previous.Back = current.Back;
                }
                else if (Current != null) {
                    Current.Back = current.Back;
                }
            }
            else {
                previous = current;
            }
            current = current.Back;
        }

        Current = new NavigationChain { Node = node, View = getInstance(node), Type = type, Back = Current };
        return new NavigationStackChanges {
            Previous = previousChain,
            Front = Current,
            Removed = removedNodes
        };
    }
}