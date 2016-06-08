using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Client;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Organization;

// ReSharper disable PossibleNullReferenceException
#pragma warning disable 219

namespace Stormpath.SDK.DocExamples.ProductGuide
{
    public class IdSite
    {
        public async Task BuildIdSiteUrl()
        {
            IClient client = null;

            #region code/csharp/idsite/build_idsite_url.cs

            var app = await client.GetApplicationAsync("application_url");

            var idSiteUrl = app.NewIdSiteUrlBuilder()
                .SetCallbackUri("http://mysite.foo/idsiteCallback")
                .Build();

            #endregion
        }

        public async Task ConsumeIdSiteAssertion()
        {
            IApplication app = null;

            #region code/csharp/idsite/build_idsite_url.cs

            var requestDescriptor = HttpRequests.NewRequestDescriptor()
                .WithMethod("GET")
                .WithUri("incoming_uri")
                .Build();

            var idSiteListener = app.NewIdSiteAsyncCallbackHandler(requestDescriptor);

            var accountResult = await idSiteListener.GetAccountResultAsync();

            #endregion
        }

        public async Task ExchangeIdSiteAssertionForToken()
        {
            IApplication app = null;
            string incoming_uri = null;

            #region code/csharp/idsite/jwt_for_oauth_req.cs

            var jwt = incoming_uri.Split('=')[1];

            var idSiteTokenExchangeRequest = OauthRequests
                .NewIdSiteTokenAuthenticationRequest()
                .SetJwt(jwt)
                .Build();

            var grantResponse = await app.NewIdSiteTokenAuthenticator()
                .AuthenticateAsync(idSiteTokenExchangeRequest);

            // Token is stored in grantResponse.AccessTokenString
#endregion
        }

        public void BuildLogoutUrl()
        {
            IApplication app = null;

            #region code/csharp/idsite/logout_from_idsite_req.cs

            var logoutUrl = app.NewIdSiteUrlBuilder()
                .SetCallbackUri("http://mysite.foo/idsiteCallback")
                .ForLogout()
                .Build();

            #endregion
        }

        public void ConsumeLogout()
        {
            IAccountResult accountResult = null;

            #region code/csharp/idsite/logout_from_idsite_resp.cs
            
            // In your callback handler, after GetAccountResultAsync()

            if (accountResult.Status == IdSiteResultStatus.Logout)
            {
                // This was a logout! Proceed accordingly...
            }

            #endregion
        }

        public void BuildPasswordResetUrl()
        {
            IApplication app = null;
            string sptoken_from_url = null;

            #region code/csharp/idsite/idsite_reset_pwd.cs

            var logoutUrl = app.NewIdSiteUrlBuilder()
                .SetCallbackUri("http://mysite.foo/idsiteCallback")
                .SetPath("/#/reset")
                .SetSpToken(sptoken_from_url)
                .Build();

            #endregion
        }
    }
}
