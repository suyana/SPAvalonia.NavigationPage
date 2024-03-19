using System;
using System.Threading;
using System.Threading.Tasks;

namespace SPAvalonia.NavigationPage;

public interface INavigateStrategy
{
	Task<string> NavigateAsync(NavigationChain chain, string currentUri, string path, CancellationToken cancellationToken);
	Task<string?> BackAsync(NavigationChain chain, string currentUri, CancellationToken cancellationToken);
}
