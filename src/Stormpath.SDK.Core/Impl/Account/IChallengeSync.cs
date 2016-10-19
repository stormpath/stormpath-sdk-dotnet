using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on the <see cref="IChallenge">Challenge</see> resource.
    /// </summary>
    internal interface IChallengeSync
    {
        /// <summary>
        /// Synchronous counterpart to <see cref="IChallenge.GetAccountAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The account.</returns>
        IAccount GetAccount();

        /// <summary>
        /// Synchronous counterpart to <see cref="IChallenge.GetFactorAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <returns>The factor.</returns>
        IFactor GetFactor();

        /// <summary>
        /// Synchronous counterpart to <see cref="IChallenge.SubmitAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <remarks>The <see cref="IChallenge.Status"/> property will be updated with the result of the code submission.</remarks>
        /// <param name="code">The code.</param>
        /// <returns>The updated resource.</returns>
        IChallenge Submit(string code);

        /// <summary>
        /// Synchronous counterpart to <see cref="IChallenge.ValidateAsync(string, System.Threading.CancellationToken)"/>.
        /// </summary>
        /// <remarks>Identical to the <see cref="Submit"/> method, other than the return semantics.</remarks>
        /// <param name="code">The code.</param>
        /// <returns><see langword="true"/> if the challenge was successful; <see langword="false"/> otherwise.</returns>
        bool Validate(string code);
    }
}
