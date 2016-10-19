using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Resource;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents a challenge against a <see cref="IFactor">Factor</see>.
    /// </summary>
    public interface IChallenge : 
        IResource,
        IAuditable
    {
        /// <summary>
        /// Gets the challenge status.
        /// </summary>
        /// <value>The factor's status.</value>
        ChallengeStatus Status { get; }

        /// <summary>
        /// Gets the challenge message.
        /// </summary>
        /// <value>The challenge message, if any.</value>
        string Message { get; }

        /// <summary>
        /// Gets the <see cref="IAccount">Account</see> associated with this challenge.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The account.</returns>
        Task<IAccount> GetAccountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the <see cref="IFactor">Factor</see> associated with this factor.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The factor.</returns>
        Task<IFactor> GetFactorAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Submits a code and returns the updated <see cref="IChallenge">Challenge</see> resource.
        /// </summary>
        /// <remarks>The <see cref="Status"/> property will be updated with the result of the code submission.</remarks>
        /// <param name="code">The code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated resource.</returns>
        Task<IChallenge> SubmitAsync(string code, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Submits a code and returns the result as a boolean.
        /// </summary>
        /// <remarks>Identical to the <see cref="SubmitAsync"/> method, other than the return semantics.</remarks>
        /// <param name="code">The code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><see langword="true"/> if the challenge was successful; <see langword="false"/> otherwise.</returns>
        Task<bool> ValidateAsync(string code, CancellationToken cancellationToken = default(CancellationToken));
    }
}