using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Impl.Utility;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class GetProviderShould
    {
        /// <summary>
        /// Regression test to ensure ProviderType changes don't impact existing code.
        /// </summary>
        /// <returns>The asynchronous test.</returns>
        [Fact]
        public async Task ReturnFacebookProvider()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.FacebookProvider).Object);

            var provider = await (dataStore as IInternalAsyncDataStore).GetResourceAsync<IProvider>("/provider", ProviderTypeConverter.TypeLookup, CancellationToken.None);

            // Verify against data from FakeJson.FacebookProvider
            provider.Href.Should().Be("https://api.stormpath.com/v1/directories/directory1/provider");
            provider.CreatedAt.Should().Be(Iso8601.Parse("2015-10-14T00:29:21.485Z"));
            provider.ModifiedAt.Should().Be(Iso8601.Parse("2016-07-02T05:03:49.601Z"));
            provider.ProviderId.Should().Be("facebook");
            provider.ProviderType.Should().Be("facebook");

            (provider as IFacebookProvider).ClientId.Should().Be("12345");
            (provider as IFacebookProvider).ClientSecret.Should().Be("abc123");
        }

        [Fact]
        public async Task ReturnSamlProvider()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.SamlProvider).Object);

            var provider = await (dataStore as IInternalAsyncDataStore).GetResourceAsync<IProvider>("/provider", ProviderTypeConverter.TypeLookup, CancellationToken.None);

            // Verify against data from FakeJson.SamlProvider
            provider.Href.Should().Be("https://api.stormpath.com/v1/directories/directory1/provider");
            provider.CreatedAt.Should().Be(Iso8601.Parse("2016-05-16T18:59:59.183Z"));
            provider.ModifiedAt.Should().Be(Iso8601.Parse("2016-05-16T18:59:59.183Z"));
            provider.ProviderId.Should().Be("saml");

            (provider as ISamlProvider).SsoLoginUrl.Should().Be("https://test.foo.bar/trust/saml2/http-post/sso/12345");
            (provider as ISamlProvider).SsoLogoutUrl.Should().Be("https://test.foo.bar/trust/saml2/http-post/slo/12345");
            (provider as ISamlProvider).RequestSignatureAlgorithm.Should().Be("RSA-SHA1");
            (provider as ISamlProvider).EncodedX509SigningCertificate.Should()
                .Be("-----BEGIN CERTIFICATE-----fakefakefake\n-----END CERTIFICATE-----");
        }

        [Fact]
        public async Task ReturnAdProvider()
        {
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.AdProvider).Object);

            var provider = await (dataStore as IInternalAsyncDataStore).GetResourceAsync<IProvider>("/provider", ProviderTypeConverter.TypeLookup, CancellationToken.None);

            // Verify against data from FakeJson.AdProvider
            provider.Href.Should().Be("https://api.stormpath.com/v1/directories/foobarDirectory/provider");
            provider.ProviderId.Should().Be("ad");

            // TODO test getting Agent href
        }

        /// <summary>
        /// Regression test for https://github.com/stormpath/stormpath-sdk-dotnet/issues/210 etc.
        /// </summary>
        /// <returns>The asynchronous test.</returns>
        [Fact]
        public async Task ReturnBaseInterfaceForUnknownProvider()
        {
            // Get a provider with the providerId "foo"
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.AdProvider.Replace("ad", "foo")).Object);

            var provider = await (dataStore as IInternalAsyncDataStore).GetResourceAsync<IProvider>("/provider", ProviderTypeConverter.TypeLookup, CancellationToken.None);

            // Verify against data from FakeJson.AdProvider
            provider.Should().NotBeNull();
            provider.Href.Should().Be("https://api.stormpath.com/v1/directories/foobarDirectory/provider");
            provider.ProviderId.Should().Be("foo");
        }
    }
}
