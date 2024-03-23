using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public class RelativeNavigateStrategy : NaturalNavigateStrategy {
	public RelativeNavigateStrategy(INavigationRegistrar navigationRegistrar) : base(navigationRegistrar) {
	}

	public override Task<string?> BackAsync(NavigationChain chain, string currentUri, CancellationToken cancellationToken) {
		var current = chain.Back;
		//while (current is HostNavigationChain {} host)
		//	current = host.Back;

		return Task.FromResult(current?.Uri);
	}
}