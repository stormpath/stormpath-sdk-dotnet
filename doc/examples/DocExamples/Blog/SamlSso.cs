using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Jwt;
using Stormpath.SDK.Saml;

namespace Stormpath.SDK.DocExamples.Blog
{
    public class SamlSso
    {
        public async Task CreateSamlRedirect()
        {
            IClient stormpathClient = null;

            #region code
            var app = await stormpathClient.GetApplicationAsync("myApplicationHref"); // Your Stormpath Application href

            var samlUrlBuilder = await app.NewSamlIdpUrlBuilderAsync();
            var redirectUrl = samlUrlBuilder
                .SetCallbackUri("myCallbackUrl") // The URL to your callback controller, see below
                .Build();
            #endregion
        }

        public async Task SamlHandler()
        {
            IClient stormpathClient = null;

            var Request = new
            {
                HttpMethod = "GET",
                Url = "http://foo.bar/callback?"
            };

            #region code
            var app = await stormpathClient.GetApplicationAsync("myApplicationHref"); // Your Stormpath Application href

            var incomingRequest = HttpRequests.NewRequestDescriptor()
                .WithMethod(Request.HttpMethod)
                .WithUri(Request.Url)
                .Build();

            var samlHandler = app.NewSamlAsyncCallbackHandler(incomingRequest);

            try
            {
                var accountResult = await samlHandler.GetAccountResultAsync();
                var account = await accountResult.GetAccountAsync();

                // Success! Do something with the account details
            }
            catch (InvalidJwtException ije)
            {
                // JWT validation failed (see Message property for details)
            }
            catch (SamlException se)
            {
                // SAML exchange failed (see Message property for details)
            }
            #endregion
        }
    }
}
