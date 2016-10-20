using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="IChallenge">Challenge</see> resource.
    /// </summary>
    public static class SyncChallengeExtensions
    {
        /// <summary>
        /// Synchronously gets the <see cref="IAccount">Account</see> associated with this challenge.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <returns>The account.</returns>
        public static IAccount GetAccount(this IChallenge challenge)
            => (challenge as IChallengeSync).GetAccount();

        /// <summary>
        /// Synchronously gets the <see cref="IFactor">Factor</see> associated with this factor.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        /// <returns>The factor.</returns>
        public static IFactor GetFactor(this IChallenge challenge)
            => (challenge as IChallengeSync).GetFactor();

        /// <summary>
        /// Submits a code and returns the updated <see cref="IChallenge">Challenge</see> resource.
        /// </summary>
        /// <remarks>The <see cref="IChallenge.Status"/> property will be updated with the result of the code submission.</remarks>
        /// <param name="challenge">The challenge.</param>
        /// <param name="code">The code.</param>
        /// <returns>The updated resource.</returns>
        public static IChallenge Submit(this IChallenge challenge, string code)
            => (challenge as IChallengeSync).Submit(code);

        /// <summary>
        /// Submits a code and returns the result as a boolean.
        /// </summary>
        /// <remarks>Identical to the <see cref="Submit"/> method, other than the return semantics.</remarks>
        /// <param name="challenge">The challenge.</param>
        /// <param name="code">The code.</param>
        /// <returns><see langword="true"/> if the challenge was successful; <see langword="false"/> otherwise.</returns>
        public static bool Validate(this IChallenge challenge, string code)
            => (challenge as IChallengeSync).Validate(code);
    }
}
