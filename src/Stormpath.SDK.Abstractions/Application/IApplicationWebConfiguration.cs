using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Api;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tenant;

namespace Stormpath.SDK.Application
{
    public interface IApplicationWebConfiguration : IResource, IHasTenant, IAuditable
    {
        IApplicationWebChangePasswordConfiguration ChangePassword { get; }

        string DnsLabel { get; }

        string DomainName { get; }

        IApplicationWebForgotPasswordConfiguration ForgotPassword { get; }

        IApplicationWebLoginConfiguration Login { get; }

        IApplicationWebMeConfiguration Me { get; }

        IApplicationWebOauth2Configuration Oauth2 { get; }

        IApplicationWebRegisterConfiguration Register { get; }

        //IApiKey SigningApiKey { set; }

        ClientApiStatus Status { get; }

        IApplicationWebVerifyEmailConfiguration VerifyEmail { get; }

        Task<IApiKey> GetSigningApiKeyAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<IApplication> GetApplicationAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
