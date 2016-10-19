using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;

namespace Stormpath.SDK.Sync
{
    public static class SyncFactorCollectionExtensions
    {
        /// <summary>
        /// Synchronously adds a new SMS Factor.
        /// </summary>
        /// <param name="factorCollection">The <see cref="IFactorCollection"/>.</param>
        /// <param name="options">The options for the new factor.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        public static ISmsFactor Add(
            this IFactorCollection factorCollection,
            SmsFactorCreationOptions options)
            => (factorCollection as IFactorCollectionSync).Add(options);

        /// <summary>
        /// Synchronously adds a new Google Authenticator (TOTP-based) Factor.
        /// </summary>
        /// <param name="factorCollection">The <see cref="IFactorCollection"/>.</param>
        /// <param name="options">The options for the new factor.</param>
        /// <returns>The new <see cref="IFactor">Factor</see>.</returns>
        public static IGoogleAuthenticatorFactor Add(
            this IFactorCollection factorCollection,
            GoogleAuthenticatorFactorCreationOptions options)
            => (factorCollection as IFactorCollectionSync).Add(options);
    }
}
