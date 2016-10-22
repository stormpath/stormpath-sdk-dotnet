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
    public class StormpathTokenGrantRequestShould
    {
        [Fact]
        public async Task MatchExpectedSyntaxAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new StormpathTokenGrantRequest
            {
                Token = "eyJra..."
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

            application.ExecuteOauthRequest(new StormpathTokenGrantRequest
            {
                Token = "eyJra..."
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        private static void ValidateParameters(IHttpRequest request)
        {
            request.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            request.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBody = request.Body.Split('&');
            requestBody.Should().Contain("grant_type=stormpath_token");
            requestBody.Should().Contain("token=eyJra...");
        }
    }
}
