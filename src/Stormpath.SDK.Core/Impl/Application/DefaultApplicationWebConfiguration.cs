using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Impl.Application
{
    internal class DefaultApplicationWebConfiguration : AbstractInstanceResource, IApplicationWebConfiguration
    {
        private const string ApplicationPropertyName = "application";
        private const string ChangePasswordPropertyName = "changePassword";
        private const string DnsLabelPropertyName = "dnsLabel";
        private const string DomainNamePropertyName = "domainName";
        private const string ForgotPasswordPropertyName = "forgotPassword";
        private const string LoginPropertyName = "login";
        private const string MePropertyName = "me";
        private const string Oauth2PropertyName = "oauth2";
        private const string RegisterPropertyName = "register";
        private const string SigningApiKeyPropertyName = "signingApiKey";
        private const string StatusPropertyName = "status";
        private const string VerifyEmailPropertyName = "verifyEmail";

        public DefaultApplicationWebConfiguration(ResourceData data)
            : base(data)
        {
        }

        internal IEmbeddedProperty Application => GetLinkProperty(ApplicationPropertyName);

        internal IEmbeddedProperty SigningApiKey => GetLinkProperty(SigningApiKeyPropertyName);

        public IApplicationWebChangePasswordConfiguration ChangePassword
        {
            get { return GetPocoProperty<DefaultApplicationWebChangePasswordConfiguration>(ChangePasswordPropertyName); }
            set { SetProperty(ChangePasswordPropertyName, value); }
        }

        public string DnsLabel
        {
            get { return GetStringProperty(DnsLabelPropertyName); }
            set { SetProperty(DnsLabelPropertyName, value); }
        }

        public string DomainName
        {
            get { return GetStringProperty(DomainNamePropertyName); }
            set { SetProperty(DomainNamePropertyName, value); }
        }

        public IApplicationWebForgotPasswordConfiguration ForgotPassword
        {
            get { return GetPocoProperty<DefaultApplicationWebForgotPasswordConfiguration>(ForgotPasswordPropertyName); }
            set { SetProperty(ForgotPasswordPropertyName, value); }
        }

        public IApplicationWebLoginConfiguration Login
        {
            get { return GetPocoProperty<DefaultApplicationWebLoginConfiguration>(LoginPropertyName); }
            set { SetProperty(LoginPropertyName, value); }
        }

        public IApplicationWebMeConfiguration Me
        {
            get { return GetPocoProperty<DefaultApplicationWebMeConfiguration>(MePropertyName); }
            set { SetProperty(MePropertyName, value); }
        }

        public IApplicationWebOauth2Configuration Oauth2
        {
            get { return GetPocoProperty<DefaultApplicationWebOauth2Configuration>(Oauth2PropertyName); }
            set { SetProperty(Oauth2PropertyName, value); }
        }

        public IApplicationWebRegisterConfiguration Register
        {
            get { return GetPocoProperty<DefaultApplicationWebRegisterConfiguration>(RegisterPropertyName); }
            set { SetProperty(RegisterPropertyName, value); }
        }

        public ClientApiStatus Status
        {
            get { return GetEnumProperty<ClientApiStatus>(StatusPropertyName); }
            set { SetProperty(StatusPropertyName, value);}
        }

        public IApplicationWebVerifyEmailConfiguration VerifyEmail
        {
            get { return GetPocoProperty<DefaultApplicationWebVerifyEmailConfiguration>(VerifyEmailPropertyName); }
            set { SetProperty(VerifyEmailPropertyName, value); }
        }

        public Task<IApiKey> GetSigningApiKeyAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IApiKey>(SigningApiKey.Href, cancellationToken);

        public Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken)
            => GetInternalAsyncDataStore().GetResourceAsync<IApplication>(Application.Href, cancellationToken);
    }
}
