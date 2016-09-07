using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.AccountStore;
using Stormpath.SDK.Application;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Directory;
using Stormpath.SDK.Http;
using Stormpath.SDK.IdSite;

namespace Stormpath.SDK.DocExamples.Blogs
{
    /// <summary>
    /// Code listings for https://stormpath.com/blog/new-dotnet-sdk-release-single-sign-on-and-faster-performance
    /// </summary>
    public class SsoAndFasterPerf
    {
        private void IdSiteBuilderExample()
        {
            IApplication application = null;

            #region code
            var redirectUrl = application.NewIdSiteUrlBuilder()
                .SetCallbackUri("https://my-site-url/HandleRedirect")
                .Build();
            // Redirect the user to redirectUrl
            #endregion
        }

        private async Task IdSiteHandlerExample()
        {
            IApplication application = null;

            var Request = new
            {
                HttpMethod = "GET",
                Url = "http://foo.bar"
            };

            #region code
            // Construct an IHttpRequest wrapper from the current Request object
            var requestDescriptor = HttpRequests
                .NewRequestDescriptor()
                .WithMethod(Request.HttpMethod)
                .WithUri(Request.Url)
                .Build();

            // Construct an ID Site callback handler
            var handler = application
                .NewIdSiteAsyncCallbackHandler(requestDescriptor);

            // Get the result of the ID Site request
            var accountResult = await handler.GetAccountResultAsync();
            // accountResult.Status is one of:
            // IdSiteResultStatus.Authenticated
            // IdSiteResultStatus.Logout
            // IdSiteResultStatus.Registered

            if (accountResult.Status != IdSiteResultStatus.Logout)
            {
                var account = await accountResult.GetAccountAsync();
                // Do cool things with account
            }
            #endregion
        }

        private void BuildClientWithDisabledCache()
        {
            #region code
            var client = Clients.Builder()
                // (other configuration)
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();
            #endregion
        }

        private void CustomizeDefaultCacheTtlTti()
        {
            #region code
            var myCacheProvider = CacheProviders.Create().InMemoryCache()
                .WithDefaultTimeToLive(TimeSpan.FromHours(2))
                .WithDefaultTimeToIdle(TimeSpan.FromMinutes(30))
                .Build();

            var client = Clients.Builder()
                // (other configuration)
                .SetCacheProvider(myCacheProvider)
                .Build();
            #endregion
        }

        private void CustomizePerCacheTtlTti()
        {
            #region code
            var myCacheProvider = CacheProviders.Create().InMemoryCache()
                .WithDefaultTimeToLive(TimeSpan.FromHours(2))
                .WithDefaultTimeToIdle(TimeSpan.FromMinutes(30))
                .WithCache(Caches
                .ForResource<IAccount>()
                    .WithTimeToLive(TimeSpan.FromMinutes(15))
                    .WithTimeToIdle(TimeSpan.FromMinutes(5)))
                .Build();
            #endregion
        }

        private void UseRedisAdapter()
        {
            #region code
            var cacheProvider = CacheProviders.Create().RedisCache()
                .WithRedisConnection("server:6379")
                .WithDefaultTimeToLive(TimeSpan.FromMinutes(60))
                .WithDefaultTimeToIdle(TimeSpan.FromMinutes(30))
                .Build();

            var client = Clients.Builder()
                // (other configuration)
                .SetCacheProvider(cacheProvider)
                .Build();
            #endregion
        }

        private async Task LinkExpansionExample()
        {
            IClient client = null;
            string accountHref = null;

            #region code
            var account = await client.GetResourceAsync<IAccount>(
                accountHref,
                opt => opt.Expand(x => x.GetCustomData()));

            // If caching is enabled, no HTTP request is made here:
            var accountCustomData = await account.GetCustomDataAsync();
            #endregion
        }

        private async Task MultipleExpansions()
        {
            IClient client = null;

            #region code
            var accounts = await client
                .GetAccounts()
                .Where(x => x.Surname.StartsWith("Sky"))
                .Expand(x => x.GetCustomData())
                .Expand(x => x.GetDirectory())
                .ToListAsync();
            #endregion
        }

        private async Task AddAccountStoreMappings()
        {
            IApplication app = null;
            string hrefOfGroupOrDirectory = null;
            IAccountStore myGroupOrDirectory = null;

            #region code
            // Add by name:
            await app.AddAccountStoreAsync("My Group Or Directory Name");

            // Or, add by href:
            await app.AddAccountStoreAsync(hrefOfGroupOrDirectory);

            // Or, from an existing IDirectory or IGroup object:
            await app.AddAccountStoreAsync(myGroupOrDirectory);

            // If you want to get really fancy, you can add by the
            // result of a query (provided the query produces one item only):
            await app.AddAccountStoreAsync<IDirectory>(
                dirs => dirs.Where(d => d.Name.StartsWith("My Dir")));
            #endregion
        }

        private async Task SetListIndex()
        {
            IClient client = null;
            IAccountStore existingGroupOrDirectory = null;
            IApplication app = null;
            
            #region code
            var mapping = client.Instantiate<IApplicationAccountStoreMapping>();
            mapping.SetAccountStore(existingGroupOrDirectory);
            mapping.SetListIndex(0);
            await app.CreateAccountStoreMappingAsync(mapping);
            #endregion
        }

        private async Task SetDefaultAccountStores()
        {
            IClient client = null;
            IAccountStore existingGroupOrDirectory = null;
            IApplication app = null;
            
            #region code
            var mapping = client.Instantiate<IApplicationAccountStoreMapping>();
            mapping.SetAccountStore(existingGroupOrDirectory);
            mapping.SetDefaultAccountStore(true);
            mapping.SetDefaultGroupStore(true);
            await app.CreateAccountStoreMappingAsync(mapping);
            #endregion
        }

        private async Task SetApplicationDefaultAccountStore()
        {
            IAccountStore myGroupOrDirectory = null;
            IApplication app = null;

            #region code
            await app.SetDefaultAccountStoreAsync(myGroupOrDirectory);
            #endregion
        }

        private async Task SpecifyAccountStoreDuringRequest()
        {
            IApplication app = null;
            IAccountStore accountStore = null;

            #region code
            // Specify an Account Store to search during a login attempt
            var loginResult = await app.AuthenticateAccountAsync(request => request
                .SetUsernameOrEmail("sonofthesuns")
                .SetPassword("whataPieceofjunk$1138")
                .SetAccountStore(accountStore));

            // Send a password reset email to the account
            // in the specified Account Store
            await app.SendPasswordResetEmailAsync(
                "vader@galacticempire.co", accountStore);
            #endregion
        }
    }
}
