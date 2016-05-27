using System;
using Stormpath.Configuration.Abstractions;
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;

namespace examples.ProductGuide
{
    public class Configuration
    {
        public void DisableCaching()
        {
            #region code/csharp/configuration/disable_caching.cs
            var client = Clients.Builder()
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
            #endregion
        }

        public void CustomCacheConfig()
        {
            #region code/csharp/configuration/custom_cache_config.cs
            var cacheProvider = CacheProviders.Create().InMemoryCache()
                .WithDefaultTimeToLive(TimeSpan.FromSeconds(120))
                .WithDefaultTimeToIdle(TimeSpan.FromSeconds(600))
                .WithCache(Caches.ForResource<IAccount>()
                    .WithTimeToLive(TimeSpan.FromSeconds(900))
                    .WithTimeToIdle(TimeSpan.FromSeconds(900)))
                .Build();

            var client = Clients.Builder()
                .SetCacheProvider(cacheProvider)
                .Build();
            #endregion
        }

        public void RedisCache()
        {
            #region code/csharp/configuration/redis_cache.cs
            var client = Clients.Builder()
                .SetCacheProvider(CacheProviders.Create().RedisCache()
                    .WithRedisConnection("localhost:6379")
                    .Build())
                .Build();
            #endregion
        }

        public void ApiCredentials()
        {
            #region code/csharp/configuration/api_credentials.cs
            var client = Clients.Builder()
                .SetApiKeyId("your_id_here")
                .SetApiKeySecret("your_secret_here")
                .Build();
            #endregion
        }

        public void ConfigureBaseUrl()
        {
            #region code/csharp/configuration/use_enterprise_url.cs
            var client = Clients.Builder()
                .SetBaseUrl("https://enterprise.stormpath.io/v1")
                .Build();
            #endregion
        }

        public void ConnectionTimeout()
        {
            #region code/csharp/configuration/connection_timeout.cs
            var client = Clients.Builder()
                .SetConnectionTimeout(60 * 1000) // in milliseconds
                .Build();
            #endregion
        }

        public void UseBasicAuth()
        {
            #region code/csharp/configuration/use_basic_auth.cs
            var client = Clients.Builder()
                .SetAuthenticationScheme(ClientAuthenticationScheme.Basic)
                .Build();
            #endregion
        }

        public void UseProxy()
        {
            #region code/csharp/configuration/use_proxy.cs
            var client = Clients.Builder()
                .SetProxy(new ClientProxyConfiguration()
                {
                    Host = "myproxy.example.com",
                    Port = 8088,
                    Username = "proxyuser",
                    Password = "proxypassword"
                })
                .Build();
            #endregion
        }
    }
}
