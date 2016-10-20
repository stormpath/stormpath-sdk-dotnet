namespace Stormpath.SDK.Account
{
    /// <summary>
    /// A Google Authenticator (TOTP-based) <see cref="IFactor">Factor</see>.
    /// </summary>
    public interface IGoogleAuthenticatorFactor : IFactor
    {
        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        /// <value>The account name.</value>
        string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the issuer name.
        /// </summary>
        /// <value>The issuer name.</value>
        string Issuer { get; set; }

        /// <summary>
        /// Gets the TOTP secret.
        /// </summary>
        /// <value>The TOTP secret.</value>
        string Secret { get; }

        /// <summary>
        /// Gets the TOTP key URI.
        /// </summary>
        /// <value>The TOTP key URI.</value>
        string KeyUri { get; }

        /// <summary>
        /// Gets the TOTP QR image as a base64-encoded string.
        /// </summary>
        /// <value>The QR image as a base64-encoded string.</value>
        string Base64QrImage { get; }
    }
}
