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
    }
}
