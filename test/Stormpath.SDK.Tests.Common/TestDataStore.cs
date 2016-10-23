using System;
using NSubstitute;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Logging;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common.Fakes;

namespace Stormpath.SDK.Tests.Common
{
    public static class TestDataStore
    {
        private const string BaseUrl = "https://api.stormpath.com/v1";

        internal static IInternalDataStore Create(string resourceJson)
            => Create(new StubRequestExecutor(resourceJson).Object);

        internal static IInternalDataStore Create(IRequestExecutor requestExecutor = null, ICacheProvider cacheProvider = null, ILogger logger = null, IClient client = null)
        {
            return new DefaultDataStore(
                client: client ?? Substitute.For<IClient>(),
                requestExecutor: requestExecutor ?? Substitute.For<IRequestExecutor>(),
                baseUrl: BaseUrl,
                serializer: Serializers.Create().JsonNetSerializer().Build(),
                logger: logger ?? new NullLogger(),
                userAgentBuilder: new FakeUserAgentBuilder(),
                instanceIdentifier: Guid.NewGuid().ToString(),
                cacheProvider: cacheProvider ?? CacheProviders.Create().DisabledCache(),
                identityMapExpiration: TimeSpan.FromMinutes(10));
        }
    }
}
