using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IChallengeCollection : IAsyncQueryable<IChallenge>
    {
        Task<IChallenge> AddAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IChallenge> AddAsync(
            ChallengeCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
