using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents a collection of <see cref="IFactor">Factors</see>.
    /// </summary>
    public interface IChallengeCollection : IAsyncQueryable<IChallenge>
    {
        /// <summary>
        /// Adds a new challenge with the default options.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created challenge.</returns>
        Task<IChallenge> AddAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new challenge.
        /// </summary>
        /// <param name="options">The challenge options.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The newly-created challenge.</returns>
        Task<IChallenge> AddAsync(
            ChallengeCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
