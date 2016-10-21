namespace Stormpath.SDK.Oauth
{
    public abstract class AbstractOauthGrantRequest
    {
        protected AbstractOauthGrantRequest(string grantType)
        {
            grant_type = grantType;
        }

        internal string grant_type { get; }

        public string AccountStore { get; set; }

        public string NameKey { get; set; }
    }
}
