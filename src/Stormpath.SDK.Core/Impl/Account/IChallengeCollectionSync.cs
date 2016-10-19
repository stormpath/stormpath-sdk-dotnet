using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on the <see cref="IChallengeCollection">Challenge collection</see>.
    /// </summary>
    internal interface IChallengeCollectionSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IChallengeCollection.AddAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The newly-created challenge.</returns>
        IChallenge Add();

        /// <summary>
        /// Synchronous counterpart to <see cref="IChallengeCollection.AddAsync(ChallengeCreationOptions, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <param name="options">The challenge options.</param>
        /// <returns>The newly-created challenge.</returns>
        IChallenge Add(ChallengeCreationOptions options);
    }
}
