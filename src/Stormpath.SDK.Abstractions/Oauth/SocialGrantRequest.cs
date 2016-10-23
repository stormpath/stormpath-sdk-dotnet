namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a social provider Access Token or Authorization Code for an OAuth 2.0 access token from Stormpath.
    /// </summary>
    /// <remarks>
    /// Only one property (<see cref="AccessToken"/> or <see cref="Code"/> should be set,
    /// depending on what the social provider returned to your application.
    /// </remarks>
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
        /// <value>The provider ID, such as <c>facebook</c>.</value>
        public string ProviderId { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; set; }


        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        /// <value>The authorization code.</value>
        public string Code { get; set; }
    }
}
