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
        private const string ChangePasswordPropertyName = "changePassword";
        private const string DnsLabelPropertyName = "dnsLabel";
        private const string DomainNamePropertyName = "domainName";
        private const string ForgotPasswordPropertyName = "forgotPassword";
        private const string LoginPropertyName = "login";
        private const string MePropertyName = "me";
        private const string Oauth2PropertyName = "oauth2";
        private const string RegisterPropertyName = "register";
        private const string StatusPropertyName = "status";
        private const string VerifyEmailPropertyName = "verifyEmail";

        public DefaultApplicationWebConfiguration(ResourceData data)
            : base(data)
        {
        }

        public ClientApiWebChangePasswordConfiguration ChangePassword
        {
            get { return GetPocoProperty<ClientApiWebChangePasswordConfiguration>(ChangePasswordPropertyName); }
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

        public ClientApiWebForgotPasswordConfiguration ForgotPassword
        {
            get { return GetPocoProperty<ClientApiWebForgotPasswordConfiguration>(ForgotPasswordPropertyName); }
            set { SetProperty(ForgotPasswordPropertyName, value); }
        }

        public ClientApiWebLoginConfiguration Login
        {
            get { return GetPocoProperty<ClientApiWebLoginConfiguration>(LoginPropertyName); }
            set { SetProperty(LoginPropertyName, value); }
        }

        public ClientApiWebMeConfiguration Me
        {
            get { return GetPocoProperty<ClientApiWebMeConfiguration>(MePropertyName); }
            set { SetProperty(MePropertyName, value); }
        }

        public ClientApiWebOauth2Configuration Oauth2
        {
            get { return GetPocoProperty<ClientApiWebOauth2Configuration>(Oauth2PropertyName); }
            set { SetProperty(Oauth2PropertyName, value); }
        }

        public ClientApiWebRegisterConfiguration Register
        {
            get { return GetPocoProperty<ClientApiWebRegisterConfiguration>(RegisterPropertyName); }
            set { SetProperty(RegisterPropertyName, value); }
        }

        public ClientApiStatus Status
        {
            get { return GetEnumProperty<ClientApiStatus>(StatusPropertyName); }
            set { SetProperty(StatusPropertyName, value);}
        }

        public IApiKey SigningApiKey
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public ClientApiWebVerifyEmailConfiguration VerifyEmail
        {
            get { return GetPocoProperty<ClientApiWebVerifyEmailConfiguration>(VerifyEmailPropertyName); }
            set { SetProperty(VerifyEmailPropertyName, value); }
        }

        public Task<IApiKey> GetSigningApiKeyAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }
    }
}
