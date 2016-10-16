using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    public interface IChallengeCollection : IAsyncQueryable<IChallenge>
    {
        Task<IChallenge> AddAsync(ChallengeCreationOptions options);
    }
}
