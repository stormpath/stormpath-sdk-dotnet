using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on the <see cref="IFactor">Factor</see> resource.
    /// </summary>
    internal interface IFactorSync :
        ISaveableSync<IFactor>,
        IDeletableSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IFactor.GetAccountAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The account.</returns>
        IAccount GetAccount();

        /// <summary>
        /// Synchronous counterpart to <see cref="IFactor.GetMostRecentChallengeAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The most recent challenge, or <see langword="null"/>.</returns>
        IChallenge GetMostRecentChallenge();
    }
}
