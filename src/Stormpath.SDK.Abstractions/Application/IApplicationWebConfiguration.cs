using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Application
{
    public interface IApplicationWebConfiguration : IResource, IHasTenant, IAuditable
    {
        ClientApiWebChangePasswordConfiguration ChangePassword { get; set; }

        string DnsLabel { get; set; }

        string DomainName { get; }

        ClientApiWebForgotPasswordConfiguration ForgotPassword { get; set; }

        ClientApiWebLoginConfiguration Login { get; set; }

        ClientApiWebMeConfiguration Me { get; set; }

        ClientApiWebOauth2Configuration Oauth2 { get; set; }

        ClientApiWebRegisterConfiguration Register { get; set; }

        IApiKey SigningApiKey { set; }

        ClientApiStatus Status { get; set; }

        ClientApiWebVerifyEmailConfiguration VerifyEmail { get; set; }

        Task<IApiKey> GetSigningApiKeyAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
