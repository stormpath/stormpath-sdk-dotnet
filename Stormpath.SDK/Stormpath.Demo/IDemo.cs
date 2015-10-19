using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.Demo
{
    public interface IDemo
    {
        Task RunAsync(CancellationToken cancellationToken);

        Task CleanupAsync();
    }
}
