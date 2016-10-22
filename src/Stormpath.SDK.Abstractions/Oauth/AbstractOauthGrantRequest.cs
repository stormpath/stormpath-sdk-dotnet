namespace Stormpath.SDK.Oauth
{
    public abstract class AbstractOauthGrantRequest
    {
        protected AbstractOauthGrantRequest(string grantType)
        {
            GrantType = grantType;
        }

        [SerializedProperty(Name="grant_type")]
        internal string GrantType { get; }

        [SerializedProperty(Name="accountStore")]
        public string AccountStoreHref { get; set; }

        [SerializedProperty(Name="nameKey")]
        public string OrganizationNameKey { get; set; }
    }
}
