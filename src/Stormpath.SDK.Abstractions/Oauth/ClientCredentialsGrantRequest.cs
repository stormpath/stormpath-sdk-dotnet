namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a client_id and client_secret for an OAuth 2.0 access token.
    /// </summary>
    public sealed class ClientCredentialsGrantRequest : AbstractOauthGrantRequest
    {
        /// <summary>
        /// Creates a new <see cref="ClientCredentialsGrantRequest"/> instance.
        /// </summary>
        public ClientCredentialsGrantRequest()
            : base("client_credentials")
        {
        }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        /// <value>Client ID.</value>
        public string ClientID { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>Client Secret</value>
        public string ClientSecret { get; set; }
    }
}
