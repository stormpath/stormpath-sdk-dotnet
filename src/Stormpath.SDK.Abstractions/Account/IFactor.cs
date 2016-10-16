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
        IAuditable // TODO?
        //, TODO IExtendable?
    {
        FactorVerificationStatus VerificationStatus { get; }

        FactorStatus Status { get; set; }

        Task<IAccount> GetAccountAsync();

        Task<IChallenge> GetMostRecentChallengeAsync();

        IChallengeCollection Challenges { get; set; }
    }
}
