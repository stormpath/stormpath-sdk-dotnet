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
    public class SocialGrantShould
    {
        [Fact]
        public async Task MatchExpectedSyntaxAccessTokenAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new SocialGrantRequest
            {
                ProviderID = "facebook",
                AccessToken = "0987aBCdefg.654hiJkL"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        [Fact]
        public void MatchExpectedSyntaxAccessToken()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = dataStore.GetResource<IApplication>("http://myapp");

            application.ExecuteOauthRequest(new SocialGrantRequest
            {
                ProviderID = "facebook",
                AccessToken = "0987aBCdefg.654hiJkL"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

 [Fact]
        public async Task MatchExpectedSyntaxAuthCodeAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new SocialGrantRequest
            {
                ProviderID = "google",
                AuthorizationCode = "BFt6789oiHGFRjbgf67UGkjhg"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        [Fact]
        public void MatchExpectedSyntaxAuthCode()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = dataStore.GetResource<IApplication>("http://myapp");

            application.ExecuteOauthRequest(new SocialGrantRequest
            {
                ProviderID = "google",
                AuthorizationCode = "BFt6789oiHGFRjbgf67UGkjhg"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);
        }

        private static void ValidateParameters(IHttpRequest request)
        {
            request.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            request.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBodyAccessToken = request.Body.Split('&');
            requestBodyAccessToken.Should().Contain("grant_type=stormpath_social");
            requestBodyAccessToken.Should().Contain("providerID=facebook");
            requestBodyAccessToken.Should().Contain("accessToken=0987aBCdefg.654hiJkL");

            var requestBodyAuthCode = request.Body.Split('&');
            requestBodyAuthCode.Should().Contain("grant_type=stormpath_social");
            requestBodyAuthCode.Should().Contain("providerID=google");
            requestBodyAuthCode.Should().Contain("code=BFt6789oiHGFRjbgf67UGkjhg");
        }
    }
}
