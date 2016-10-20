namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Defines the options available when creating a <see cref="IPhone">Phone</see>.
    /// </summary>
    public sealed class PhoneCreationOptions
    {
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the phone verification status.
        /// </summary>
        /// <value>The phone verification status.</value>
        public PhoneVerificationStatus VerificationStatus { get; set; }

        /// <summary>
        /// Gets or sets the phone status.
        /// </summary>
        /// <value>The phone status.</value>
        public PhoneStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the optional phone name.
        /// </summary>
        /// <value>The phone name, or <see langword="null"/>.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional phone description.
        /// </summary>
        /// <value>The phone description, or <see langword="null"/>.</value>
        public string Description { get; set; }
    }
}
