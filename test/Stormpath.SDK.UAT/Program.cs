using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.Configuration.Abstractions;
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Http;
using Stormpath.SDK.Serialization;

namespace Stormpath.SDK.UAT
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TestDefaultCacheCreation();

            TestCacheCreationWithBuilderConfig();

            TestCacheCreationWithConfig();

            TestRedisCacheCreation();

            Console.WriteLine("All UATs done");
        }

        private static void TestDefaultCacheCreation()
        {
            var client = Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetApiKeyId("fake")
                .SetApiKeySecret("reallyfake")
                .Build();
        }

        private static void TestCacheCreationWithBuilderConfig()
        {
            var client = Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetCacheProvider(CacheProviders.Create()
                    .InMemoryCache()
                    .WithDefaultTimeToIdle(TimeSpan.FromSeconds(600))
                    .WithDefaultTimeToLive(TimeSpan.FromSeconds(900))
                    .WithCache(Caches
                        .ForResource<IAccount>()
                        .WithTimeToIdle(TimeSpan.FromSeconds(5000))
                        .WithTimeToLive(TimeSpan.FromSeconds(6000)))
                    .Build())
                .Build();
        }

        private static void TestCacheCreationWithConfig()
        {
            var client = Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetConfiguration(new StormpathConfiguration
                {
                    Client = new ClientConfiguration
                    {
                        CacheManager = new ClientCacheManagerConfiguration
                        {
                            DefaultTti = 600,
                            DefaultTtl = 900,
                            Caches = new Dictionary<string, ClientCacheConfiguration>()
                            {
                                ["account"] = new ClientCacheConfiguration { Tti = 5000, Ttl = 6000 }
                            }
                        }
                    }
                })
                .Build();
        }

        private static void TestRedisCacheCreation()
        {
            var client = Clients.Builder()
                .SetHttpClient(HttpClients.Create().SystemNetHttpClient())
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetCacheProviderBuilder(CacheProviders.Create().RedisCache())
                .SetConfiguration(new StormpathConfiguration
                {
                    Client = new ClientConfiguration
                    {
                        CacheManager = new ClientCacheManagerConfiguration
                        {
                            DefaultTti = 600,
                            DefaultTtl = 900,
                            Caches = new Dictionary<string, ClientCacheConfiguration>()
                            {
                                ["account"] = new ClientCacheConfiguration { Tti = 5000, Ttl = 10000 }
                            }
                        }
                    }
                })
                .Build();
        }
    }
}
