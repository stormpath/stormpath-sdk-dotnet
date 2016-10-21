namespace Stormpath.SDK.Oauth
{
    public abstract class AbstractOauthGrantRequest
    {
        protected AbstractOauthGrantRequest(string grantType)
        {
            grant_type = grantType;
        }

        internal string grant_type { get; }

        public string AccountStoreHref { get; set; }

        public string OrganizationNameKey { get; set; }
    }
}
