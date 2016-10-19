using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="IFactor">Factor</see> resource.
    /// </summary>
    public static class SyncFactorExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IAccount">account</see> associated with this factor.
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <returns>The account.</returns>
        public static IAccount GetAccount(this IFactor factor)
            => (factor as IFactorSync).GetAccount();

        /// <summary>
        /// Synchronously gets the most recent <see cref="IChallenge">challenge</see> against this factor, if any.
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <returns>The most recent challenge, or <see langword="null"/>.</returns>
        public static IChallenge GetMostRecentChallenge(this IFactor factor)
            => (factor as IFactorSync).GetMostRecentChallenge();
    }
}
