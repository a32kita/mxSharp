using System.Threading;
using System.Threading.Tasks;

namespace MxSharp;

public interface IMxSharpAccessTokenProvider
{
    ValueTask<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}