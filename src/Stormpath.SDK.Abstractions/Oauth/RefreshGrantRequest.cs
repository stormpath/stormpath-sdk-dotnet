namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a refresh token for an OAuth 2.0 access token.
    /// </summary>
    public sealed class RefreshGrantRequest : AbstractOauthGrantRequest
    {
        /// <summary>
        /// Creates a new <see cref="RefreshGrantRequest"/> instance.
        /// </summary>
        public RefreshGrantRequest()
            : base("refresh_token")
        {
        }

        /// <summary>
        /// Gets or sets the refresh token string.
        /// </summary>
        /// <value>The refresh token string.</value>
        [SerializedProperty(Name="refresh_token")]
        public string RefreshToken { get; set; }
    }
}
