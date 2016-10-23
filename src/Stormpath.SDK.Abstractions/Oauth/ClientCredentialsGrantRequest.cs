namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a client ID and secret for an OAuth 2.0 access token.
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
        /// Gets or sets the client ID.
        /// </summary>
        /// <value>The client ID.</value>
        [SerializedProperty(Name="client_id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        /// <value>The client secret.</value>
        [SerializedProperty(Name="client_secret")]
        public string Secret { get; set; }
    }
}
