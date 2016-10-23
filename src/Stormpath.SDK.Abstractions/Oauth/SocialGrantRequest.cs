namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a social providerss Access Token or Authorization Code for an OAuth 2.0 access token from Stormpath.
    /// Either an access token or an authorization code MUST be specified
    /// </summary>
    public sealed class SocialGrantRequest : AbstractOauthGrantRequest
    {
        /// <summary>
        /// Creates a new <see cref="SocialGrantRequest"/> instance.
        /// </summary>
        public SocialGrantRequest()
            : base("stormpath_social")
        {
        }

        /// <summary>
        /// Gets or sets the provider ID.
        /// </summary>
        /// <value>Client ID.</value>
        public string ProviderID { get; set; }

        /// <summary>
        /// Gets or sets the Access Token
        /// </summary>
        /// <value>Access Token</value>
        public string AccessToken { get; set; }


        /// <summary>
        /// Gets or sets the Authorization Code. This is referred to only as "code" in the REST call. 
        /// </summary>
        /// <value>Authorization Code</value>
        public string Code { get; set; }
    }
}
