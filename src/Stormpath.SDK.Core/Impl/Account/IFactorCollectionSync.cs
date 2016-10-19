using Stormpath.SDK.Account;

namespace Stormpath.SDK.Impl.Account
{
    /// <summary>
    /// Represents the synchronous actions that correspond to the default asynchronous actions
    /// available on <see cref="IFactorCollection"/>.
    /// </summary>
    internal interface IFactorCollectionSync
    {
        /// <summary>
        /// Adds a new SMS Factor.
        /// </summary>
        /// <param name="options">The options for the new factor.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        ISmsFactor Add(SmsFactorCreationOptions options);

        /// <summary>
        /// Adds a new Google Authenticator (TOTP-based) Factor.
        /// </summary>
        /// <param name="options">The options for the new factor.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        IGoogleAuthenticatorFactor Add(GoogleAuthenticatorFactorCreationOptions options);
    }
}
