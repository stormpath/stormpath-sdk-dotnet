using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Oauth;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Oauth
{
    public class FactorChallengeRequestShould
    {
        private static IInternalDataStore BuildDataStore(string resourceResponse)
        {
            return TestDataStore.Create(new StubRequestExecutor(resourceResponse).Object);
        }

        [Fact]
        public async Task UseFactorHrefFromString()
        {
            var dataStore = BuildDataStore(FakeJson.Application);

            var factorChallengeRequest = OauthRequests.NewFactorChallengeRequest()
                .SetChallenge("http://mychallengehref")
                .SetCode("12345")
                .Build();

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            var authenticateResult = await application.NewFactorChallengeAuthenticator()
                .AuthenticateAsync(factorChallengeRequest);

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;

            httpRequest.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            httpRequest.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBody = httpRequest.Body.Split('&');
            requestBody.Should().Contain("grant_type=stormpath_factor_challenge");
            requestBody.Should().Contain("code=12345");
            requestBody.Should().Contain("challenge=http%3A%2F%2Fmychallengehref");
        }

        [Fact]
        public async Task UseFactorHrefFromChallengeInstance()
        {
            var dataStore = BuildDataStore(FakeJson.Application);

            var challenge = Substitute.For<IChallenge>();
            challenge.Href.Returns("http://mychallengehref");

            var factorChallengeRequest = OauthRequests.NewFactorChallengeRequest()
                .SetChallenge(challenge)
                .SetCode("12345")
                .Build();

            var application = await dataStore.GetResourceAsync<IApplication>("http://myapp");

            var authenticateResult = await application.NewFactorChallengeAuthenticator()
                .AuthenticateAsync(factorChallengeRequest);

            var call = dataStore.RequestExecutor.ReceivedCalls().Last();
            var httpRequest = call.GetArguments()[0] as IHttpRequest;

            httpRequest.CanonicalUri.ToString().Should().EndWith("/oauth/token");
            httpRequest.BodyContentType.Should().Be("application/x-www-form-urlencoded");

            var requestBody = httpRequest.Body.Split('&');
            requestBody.Should().Contain("grant_type=stormpath_factor_challenge");
            requestBody.Should().Contain("code=12345");
            requestBody.Should().Contain("challenge=http%3A%2F%2Fmychallengehref");
        }
    }
}
