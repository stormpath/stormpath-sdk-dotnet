using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;

namespace Stormpath.SDK.Impl.Oauth
{
    internal sealed class DefaultGrantAuthenticator
    {
        private const string OauthTokenPath = "/oauth/token";

        private readonly IApplication _application;
        private readonly IInternalDataStore _internalDataStore;

        public DefaultGrantAuthenticator(IApplication application, IInternalDataStore internalDataStore)
        {
            _application = application;
            _internalDataStore = internalDataStore;
        }

        public async Task<IOauthGrantAuthenticationResult> AuthenticateAsync(AbstractOauthGrantRequest request, CancellationToken cancellationToken)
        {
            return await (_internalDataStore as IInternalAsyncDataStore)
                .CreateAsync<AbstractOauthGrantRequest, IGrantAuthenticationToken>(
                    $"{_application.Href}{OauthTokenPath}",
                    request,
                    GetHeaderWithMediaType(),
                    cancellationToken).ConfigureAwait(false);
        }

        public IOauthGrantAuthenticationResult Authenticate(AbstractOauthGrantRequest request)
        {
            return (_internalDataStore as IInternalSyncDataStore)
                .Create<AbstractOauthGrantRequest, IGrantAuthenticationToken>(
                    $"{_application.Href}{OauthTokenPath}",
                    request,
                    GetHeaderWithMediaType());
        }

        private static HttpHeaders GetHeaderWithMediaType()
        {
            return new HttpHeaders
            {
                ContentType = HttpHeaders.MediaTypeApplicationFormUrlEncoded
            };
        }
    }
}
