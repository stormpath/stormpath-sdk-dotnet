using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Unit.Oauth
{
    public class ClientCredentialsGrantShould
    {
        [Fact]
        public async Task MatchExpectedSyntaxAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new ClientCredentialsGrantRequest
            {
                ClientID = "clientidtest",
                ClientSecret = "clientsecret+test"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        [Fact]
        public void MatchExpectedSyntax()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = dataStore.GetResource<IApplication>("http://myapp");

            application.ExecuteOauthRequest(new ClientCredentialsGrantRequest
            {
                ClientID = "clientidtest",
                ClientSecret = "clientsecret+test"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        [Fact]
        public async Task IncludeAccountStoreAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new ClientCredentialsGrantRequest
            {
                ClientID = "clientidtest",
                ClientSecret = "clientsecret+test",
                AccountStoreHref = "https://api.stormpath.com/v1/directories/1bcd23ec1d0a8wa6"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);

            // Ensure Account Store href is included
            var requestBody = httpRequest.Body.Split('&');
            requestBody.Should().Contain("accountStore=https%3A%2F%2Fapi.stormpath.com%2Fv1%2Fdirectories%2F1bcd23ec1d0a8wa6");
        }

        private static void ValidateParameters(IHttpRequest request)
        {
            request.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            request.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBody = request.Body.Split('&');
            requestBody.Should().Contain("grant_type=client_credentials");
            requestBody.Should().Contain("client_id=clientidtest");
            requestBody.Should().Contain("client_secret=clientsecret%2Btest");
        }
    }
}
