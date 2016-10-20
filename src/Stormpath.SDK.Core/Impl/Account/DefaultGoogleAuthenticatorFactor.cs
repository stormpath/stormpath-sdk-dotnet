using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Account
{
    internal sealed class DefaultGoogleAuthenticatorFactor : DefaultFactor, IGoogleAuthenticatorFactor
    {
        private const string AccountNamePropertyName = "accountName";
        private const string IssuerPropertyName = "issuer";
        private const string SecretPropertyName = "secret";
        private const string KeyUriPropertyName = "keyUri";
        private const string Base64QrImagePropertyName = "base64QRImage";

        public DefaultGoogleAuthenticatorFactor(ResourceData data)
            : base(data)
        {
        }

        public string AccountName
        {
            get { return GetStringProperty(AccountNamePropertyName); }
            set { SetProperty(AccountNamePropertyName, value); }
        }

        public string Issuer
        {
            get { return GetStringProperty(IssuerPropertyName); }
            set { SetProperty(IssuerPropertyName, value); }
        }

        public string Secret => GetStringProperty(SecretPropertyName);

        public string KeyUri => GetStringProperty(KeyUriPropertyName);

        public string Base64QrImage => GetStringProperty(Base64QrImagePropertyName);
    }
}
