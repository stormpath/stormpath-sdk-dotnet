using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    public interface IChallenge : IResource, IAuditable
    {
        ChallengeStatus Status { get; }

        string Message { get; }

        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IFactor> GetFactorAsync(CancellationToken cancellationToken = default(CancellationToken));

        // TODO what does this return?
        Task SubmitAsync(string code, CancellationToken cancellationToken = default(CancellationToken));
    }
}