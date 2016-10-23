namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a Stormpath assertion token for an OAuth 2.0 access token.
    /// </summary>
    public sealed class StormpathTokenGrantRequest : AbstractOauthGrantRequest
    {
        public StormpathTokenGrantRequest()
            : base("stormpath_token")
        {
        }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; set; }
    }
}
