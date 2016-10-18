using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Account
{
    public interface IFactor :
        IResource,
        IHasTenant,
        ISaveable<IFactor>,
        IDeletable,
        IAuditable
    {
        string Type { get; }

        FactorVerificationStatus VerificationStatus { get; }

        FactorStatus Status { get; set; }

        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IChallenge> GetMostRecentChallengeAsync(CancellationToken cancellationToken = default(CancellationToken));

        IChallengeCollection Challenges { get; }
    }
}
