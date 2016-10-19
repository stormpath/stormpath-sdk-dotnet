using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Linq;

namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Represents a collection of <see cref="IFactor">Factors</see>.
    /// </summary>
    public interface IFactorCollection : IAsyncQueryable<IFactor>
    {
        /// <summary>
        /// Adds a new SMS Factor.
        /// </summary>
        /// <param name="options">The options for the new factor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        Task<ISmsFactor> AddAsync(
            SmsFactorCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds a new Google Authenticator (TOTP-based) Factor.
        /// </summary>
        /// <param name="options">The options for the new factor.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        Task<IGoogleAuthenticatorFactor> AddAsync(
            GoogleAuthenticatorFactorCreationOptions options,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
