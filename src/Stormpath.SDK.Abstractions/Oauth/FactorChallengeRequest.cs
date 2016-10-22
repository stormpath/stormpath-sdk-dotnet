namespace Stormpath.SDK.Oauth
{
    /// <summary>
    /// A request to exchange a Factor Challenge code for an OAuth 2.0 access token.
    /// </summary>
    public sealed class FactorChallengeRequest : AbstractOauthGrantRequest
    {
        /// <summary>
        /// Creates a new <see cref="FactorChallengeRequest"/> instance.
        /// </summary>
        public FactorChallengeRequest()
            : base("stormpath_factor_challenge")
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Account.IChallenge">Challenge</see> <c>href</c>.
        /// </summary>
        /// <value>The <see cref="Account.IChallenge">Challenge</see> <c>href</c>.</value>
        [SerializedProperty(Name="challenge")]
        public string ChallengeHref { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }
    }
}
