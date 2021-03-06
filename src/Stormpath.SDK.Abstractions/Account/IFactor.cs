﻿using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

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
        /// Gets or sets the factor's status.
        /// </summary>
        /// <value>The factor's status.</value>
        FactorStatus Status { get; set; }

        /// <summary>
        /// Gets the <see cref="IAccount">Account</see> associated with this factor.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the most recent <see cref="IChallenge">Challenge</see> against this factor, if any.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The most recent challenge, or <see langword="null"/>.</returns>
        Task<IChallenge> GetMostRecentChallengeAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the collection of <see cref="IChallenge">Challenges</see> against this factor.
        /// </summary>
        /// <returns>A collection of the <see cref="IChallenge">challenges</see> against this factor.</returns>
        IChallengeCollection Challenges { get; }
    }
}
