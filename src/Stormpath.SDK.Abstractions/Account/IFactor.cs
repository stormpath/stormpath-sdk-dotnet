using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A factor used for multi-factor authentication against an <see cref="IAccount">Account</see>.
    /// </summary>
    public interface IFactor :
        IResource,
        ISaveable<IFactor>,
        IDeletable,
        IAuditable
    {
        /// <summary>
        /// Gets the factor's type.
        /// </summary>
        /// <value>The factor's type.</value>
        FactorType Type { get; }

        /// <summary>
        /// Gets the factor's verification status.
        /// </summary>
        /// <value>The factor's verification status.</value>
        FactorVerificationStatus VerificationStatus { get; }

        /// <summary>
        /// Gets the factor's status.
        /// </summary>
        /// <value>The factor's status.</value>
        FactorStatus Status { get; set; }

        /// <summary>
        /// Gets the <see cref="IAccount">account</see> associated with this factor.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the most recent <see cref="IChallenge">challenge</see> against this factor, if any.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The most recent challenge, or <see langword="null"/>.</returns>
        Task<IChallenge> GetMostRecentChallengeAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Accesses the factor's <c>challenges</c> collection.
        /// </summary>
        /// <returns>A queryable list of the <see cref="IChallenge">challenge</see>s against this factor.</returns>
        IChallengeCollection Challenges { get; }
    }
}
