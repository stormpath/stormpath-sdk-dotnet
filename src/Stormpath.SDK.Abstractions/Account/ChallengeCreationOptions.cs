namespace Stormpath.SDK.Account
{
    /// <summary>
    /// Defines the options available when creating a <see cref="IChallenge">Challenge</see>.
    /// </summary>
    public sealed class ChallengeCreationOptions
    {
        /// <summary>
        /// Gets or sets the optional message used for the challenge.
        /// </summary>
        /// <value>The message used for the challenge, or <see langword="null"/>.</value>
        public string Message { get; set; }
    }
}
