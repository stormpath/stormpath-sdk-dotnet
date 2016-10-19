using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    /// <summary>
    /// Provides synchronous access to the methods available on the <see cref="IChallengeCollection">Challenge collection</see>.
    /// </summary>
    public static class SyncChallengeCollectionExtensions
    {
        /// <summary>
        /// Synchronously adds a new challenge with the default options.
        /// </summary>
        /// <param name="challengeCollection">The <see cref="IChallengeCollection"/>.</param>
        /// <returns>The newly-created challenge.</returns>
        public static IChallenge Add(this IChallengeCollection challengeCollection)
            => (challengeCollection as IChallengeCollectionSync).Add();

        /// <summary>
        /// Synchronously adds a new challenge.
        /// </summary>
        /// <param name="options">The challenge options.</param>
        /// <param name="challengeCollection">The <see cref="IChallengeCollection"/>.</param>
        /// <returns>The newly-created challenge.</returns>
        public static IChallenge Add(this IChallengeCollection challengeCollection, ChallengeCreationOptions options)
            => (challengeCollection as IChallengeCollectionSync).Add(options);
    }
}
