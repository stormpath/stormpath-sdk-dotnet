namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Defines the options available when creating a Google Authenticator (TOTP-based) <see cref="IFactor">Factor</see>.
    /// </summary>
    public sealed class GoogleAuthenticatorFactorCreationOptions
    {
        /// <summary>
        /// Gets or sets whether to challenge this factor immediately.
        /// </summary>
        /// <value>Whether to challenge this factor immediately.</value>
        public bool Challenge { get; set; }

        /// <summary>
        /// Gets or sets the optional account name.
        /// </summary>
        /// <value>The account name, or <see langword="null"/>.</value>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the optional issuer.
        /// </summary>
        /// <value>The issuer, or <see langword="null"/>.</value>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the factor status.
        /// </summary>
        /// <value>The factor status.</value>
        public FactorStatus Status { get; set; }
    }
}
