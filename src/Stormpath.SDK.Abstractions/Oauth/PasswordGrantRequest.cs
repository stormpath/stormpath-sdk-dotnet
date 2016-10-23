namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a username and password for an OAuth 2.0 access token.
    /// </summary>
    public sealed class PasswordGrantRequest : AbstractOauthGrantRequest
    {
        /// <summary>
        /// Creates a new <see cref="PasswordGrantRequest"/> instance.
        /// </summary>
        public PasswordGrantRequest()
            : base("password")
        {
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>The user's password.</value>
        public string Password { get; set; }
    }
}
