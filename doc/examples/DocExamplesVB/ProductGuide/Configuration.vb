Imports Stormpath.Configuration.Abstractions
Imports Stormpath.SDK.Account
Imports Stormpath.SDK.Cache
Imports Stormpath.SDK.Client

Public Class Configuration

    Public Sub DisableCaching()
#Region "code/vbnet/configuration/disable_caching.vb"
        Dim client = Clients.Builder() _
            .SetCacheProvider(CacheProviders.Create().DisabledCache()) _
            .Build()
#End Region
    End Sub

    Public Sub CustomCacheConfig()
#Region "code/vbnet/configuration/custom_cache_config.vb"
        Dim cacheProvider = CacheProviders.Create().InMemoryCache().
            WithDefaultTimeToLive(TimeSpan.FromSeconds(120)).
            WithDefaultTimeToIdle(TimeSpan.FromSeconds(600)).
            WithCache(Caches.ForResource(Of IAccount)() _
                .WithTimeToLive(TimeSpan.FromSeconds(900)) _
                .WithTimeToIdle(TimeSpan.FromSeconds(900))) _
            .Build()

        Dim client = Clients.Builder().SetCacheProvider(cacheProvider).Build()
#End Region
    End Sub

    Public Sub RedisCache()
#Region "code/vbnet/configuration/redis_cache.vb"
        Dim client = Clients.Builder() _
            .SetCacheProvider(CacheProviders.Create().RedisCache() _
                .WithRedisConnection("localhost:6379") _
                .Build()) _
            .Build()
#End Region
    End Sub

    Public Sub ApiCredentials()
#Region "code/vbnet/configuration/api_credentials.vb"
        Dim client = Clients.Builder() _
            .SetApiKeyId("your_id_here") _
            .SetApiKeySecret("your_secret_here") _
            .Build()
#End Region
    End Sub

    Public Sub ConfigureBaseUrl()
#Region "code/vbnet/configuration/use_enterprise_url.vb"
        Dim client = Clients.Builder() _
            .SetBaseUrl("https://enterprise.stormpath.io/v1") _
            .Build()
#End Region
    End Sub

    Public Sub ConnectionTimeout()
#Region "code/vbnet/configuration/connection_timeout.vb"
        ' Value is in milliseconds
        Dim client = Clients.Builder() _
            .SetConnectionTimeout(60 * 1000) _
            .Build()
#End Region
    End Sub

    Public Sub UseBasicAuth()
#Region "code/vbnet/configuration/use_basic_auth.vb"
        Dim client = Clients.Builder() _
            .SetAuthenticationScheme(ClientAuthenticationScheme.Basic) _
            .Build()
#End Region
    End Sub

    Public Sub UseProxy()
#Region "code/vbnet/configuration/use_proxy.vb"
        Dim client = Clients.Builder() _
            .SetProxy(New ClientProxyConfiguration() With
            {
                .Host = "myproxy.example.com",
                .Port = 8088,
                .Username = "proxyuser",
                .Password = "proxypassword"
            }).Build()
#End Region
    End Sub

End Class
