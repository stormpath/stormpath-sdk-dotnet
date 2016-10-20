namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Defines the options available when creating an SMS <see cref="IFactor">Factor</see>.
    /// </summary>
    public sealed class SmsFactorCreationOptions
    {
        /// <summary>
        /// Gets or sets whether to challenge this factor immediately.
        /// </summary>
        /// <remarks>If <see langword="true"/>, the factor will be challenged immediately.</remarks>
        /// <value>Whether to challenge this factor immediately.</value>
        public bool Challenge { get; set; } = false;

        /// <summary>
        /// Gets or sets the phone number to use for the factor.
        /// </summary>
        /// <value>The phone number to use for the factor.</value>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the factor status.
        /// </summary>
        /// <value>The factor status.</value>
        public FactorStatus Status { get; set; }
    }
}
