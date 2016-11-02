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
    public class PasswordGrantRequestShould
    {
        [Fact]
        public async Task MatchExpectedSyntaxAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new PasswordGrantRequest
            {
                Username = "tom@testmail.stormpath.com",
                Password = "Secret123!!"
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

            application.ExecuteOauthRequest(new PasswordGrantRequest
            {
                Username = "tom@testmail.stormpath.com",
                Password = "Secret123!!"
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

            await application.ExecuteOauthRequestAsync(new PasswordGrantRequest
            {
                Username = "tom@testmail.stormpath.com",
                Password = "Secret123!!",
                AccountStoreHref = "https://api.stormpath.com/v1/directories/1bcd23ec1d0a8wa6"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);

            // Ensure Account Store href is included
            var requestBody = httpRequest.Body.Split('&');
            requestBody.Should().Contain("accountStore=https%3A%2F%2Fapi.stormpath.com%2Fv1%2Fdirectories%2F1bcd23ec1d0a8wa6");
        }

        [Fact]
        public async Task IncludeNameKeyAsync()
        {
            var dataStore = TestDataStore.Create(FakeJson.Application);

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            await application.ExecuteOauthRequestAsync(new PasswordGrantRequest
            {
                Username = "tom@testmail.stormpath.com",
                Password = "Secret123!!",
                OrganizationNameKey = "anOrganization"
            });

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;
            ValidateParameters(httpRequest);

            // Ensure Account Store href is included
            var requestBody = httpRequest.Body.Split('&');
            requestBody.Should().Contain("nameKey=anOrganization");
        }

        private static void ValidateParameters(IHttpRequest request)
        {
            request.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            request.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBody = request.Body.Split('&');
            requestBody.Should().Contain("grant_type=password");
            requestBody.Should().Contain("username=tom%40testmail.stormpath.com");
            requestBody.Should().Contain("password=Secret123%21%21");
        }
    }
}
