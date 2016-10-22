// <copyright file="Async_caching_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Api;
using Stormpath.SDK.Application;
using Stormpath.SDK.Auth;
using Stormpath.SDK.Cache;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Group;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Cache
{
    public class Async_caching_tests : IDisposable
    {
        private IInternalDataStore dataStore;

        private void BuildDataStore(string resourceResponse, ICacheProvider cacheProviderUnderTest)
        {
            var fakeRequestExecutor = new StubRequestExecutor(resourceResponse);

            this.dataStore = TestDataStore.Create(fakeRequestExecutor.Object, cacheProviderUnderTest);
        }

        [Fact]
        public async Task Resource_access_not_cached_with_null_cache()
        {
            var cacheProvider = CacheProviders.Create().DisabledCache();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@corellia.core");
            account1.FullName.ShouldBe("Han Solo");

            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Resource_access_is_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@corellia.core");
            account1.FullName.ShouldBe("Han Solo");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Does_not_hit_cache_if_no_asynchronous_path_exists()
        {
            var fakeCacheProvider = Substitute.For<IAsynchronousCacheProvider>();
            fakeCacheProvider.IsAsynchronousSupported.Returns(false);
            fakeCacheProvider.IsSynchronousSupported.Returns(true);
            fakeCacheProvider
                .When(x => x.GetAsyncCache(Arg.Any<string>()))
                .Do(_ => { throw new NotImplementedException(); });

            this.BuildDataStore(FakeJson.Account, fakeCacheProvider);

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            account.Email.ShouldBe("han.solo@corellia.core");
        }

        [Fact]
        public async Task Collection_access_is_not_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountList, cacheProvider);

            IAsyncQueryable<IAccount> accounts = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            await accounts.MoveNextAsync();

            IAsyncQueryable<IAccount> accounts2 = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            await accounts2.MoveNextAsync();

            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Collection_items_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountList, cacheProvider);

            IAsyncQueryable<IAccount> accounts = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            await accounts.MoveNextAsync();

            var han = await this.dataStore.GetResourceAsync<IAccount>("/accounts/account1");
            han.Email.ShouldBe("han.solo@corellia.core");
            han.FullName.ShouldBe("Han Solo");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Custom_data_is_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.CustomData, cacheProvider);

            var customData1 = await this.dataStore.GetResourceAsync<ICustomData>("/accounts/foobarAccount/customData");
            var customData2 = await this.dataStore.GetResourceAsync<ICustomData>("/accounts/foobarAccount/customData");

            customData2["membershipType"].ShouldBe("lifetime");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Expanded_nested_resources_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountWithExpandedCustomData, cacheProvider);

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            account.Email.ShouldBe("han.solo@corellia.core");
            account.FullName.ShouldBe("Han Solo");

            var customDataFromHref = await this.dataStore.GetResourceAsync<ICustomData>("/accounts/foobarAccount/customData");
            var customDataFromLink = await account.GetCustomDataAsync();

            customDataFromHref["isAdmin"].ShouldBe(false);

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Expanded_collection_items_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountWithExpandedGroups, cacheProvider);

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount?expand=groups(offset:0,limit:25)");
            var group1 = await this.dataStore.GetResourceAsync<IGroup>("/groups/group1");
            var group2 = await this.dataStore.GetResourceAsync<IGroup>("/groups/group2");

            group1.Name.ShouldBe("Imperials");
            group2.Name.ShouldBe("Rebels");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deleting_resource_removes_cached_item()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            await account1.DeleteAsync();

            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@corellia.core");
            account1.FullName.ShouldBe("Han Solo");

            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Updating_resource_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns original
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false) as IHttpResponse));

            // Save returns update data
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account.Replace("han.solo@corellia.core", "han@solo.me"), "application/json", transportError: false) as IHttpResponse));

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            account1.Email.ShouldBe("han.solo@corellia.core");

            account1.SetEmail("han@solo.me");
            await account1.SaveAsync();
            account1.Email.ShouldBe("han@solo.me");

            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            account2.Email.ShouldBe("han@solo.me");

            // Only one GET; second is intercepted by the cache (but the updated data is returned!) #magic
            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Updating_custom_data_with_proxy_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns expanded request
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.AccountWithExpandedCustomData, "application/json", transportError: false) as IHttpResponse));

            // Save is not an expanded request
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false) as IHttpResponse));

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount?expand=customData");

            account.CustomData.Put("isAdmin", true);
            account.CustomData.Put("writeAccess", "yes");
            await account.SaveAsync();

            var customData = await account.GetCustomDataAsync();
            customData["isAdmin"].ShouldBe(true);
            customData["writeAccess"].ShouldBe("yes");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Custom_data_is_always_cached_on_parent_resource_save()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account = this.dataStore.Instantiate<IAccount>();
            account.CustomData.Put("foo", "bar");
            account.CustomData.Put("baz", 1234);
            await (this.dataStore as IInternalAsyncDataStore).CreateAsync("/accounts", account, CancellationToken.None);

            var customData = await account.GetCustomDataAsync();
            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(1234);

            // CustomData was cached; did not make a request
            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Custom_data_updates_are_not_cached_if_authoritative_custom_data_is_not_cached()
        {
            // This test differs from Updating_custom_data_with_proxy_updates_cache
            // because we aren't GETting the custom data first (whether explicitly or with an expanded query).
            // In this case, we don't want to cache updates because we have no authoritative version.
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account = await this.dataStore.GetResourceAsync<IAccount>("/account");
            account.CustomData.Put("foo", "bar!");
            account.CustomData.Put("isWorking", true);
            await account.SaveAsync();

            var customData = await account.GetCustomDataAsync();

            // CustomData was *not* cached
            await this.dataStore.RequestExecutor.Received(3).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Deleting_custom_data_with_proxy_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns expanded request
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.AccountWithExpandedCustomData, "application/json", transportError: false) as IHttpResponse));

            // Save is not an expanded request
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false) as IHttpResponse));

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount?expand=customData");
            account.CustomData.Remove("isAdmin");
            await account.SaveAsync();

            var customData = await account.GetCustomDataAsync();
            customData["isAdmin"].ShouldBe(null);

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Email_verification_result_removes_associated_account_from_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            var emailVerificationTokenResponse = @"
{
    ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount""
}
";

            // POST returns email verification token response
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), emailVerificationTokenResponse, "application/json", transportError: false) as IHttpResponse));

            // GET returns account as unverified first,
            // then second GET returns account as verified
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account.Replace(@"""status"": ""ENABLED""", @"""status"": ""UNVERIFIED"""), "application/json", transportError: false) as IHttpResponse),
                    Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false) as IHttpResponse));

            var account = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            account.Status.ShouldBe(AccountStatus.Unverified);

            var href = $"/accounts/emailVerificationTokens/fooToken";
            var tokenResponse = await (this.dataStore as IInternalAsyncDataStore).CreateAsync<IResource, IEmailVerificationToken>(href, null, CancellationToken.None);
            await this.dataStore.GetResourceAsync<IAccount>(tokenResponse.Href);

            account.Status.ShouldBe(AccountStatus.Enabled);

            // Second GET should *not* hit cache
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Does_not_cache_provider_account_access()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var customData1 = await this.dataStore.GetResourceAsync<IProviderAccountResult>("/accounts/foobarAccount");
            var customData2 = await this.dataStore.GetResourceAsync<IProviderAccountResult>("/accounts/foobarAccount");

            // Not cached
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Does_not_cache_email_verification_tokens()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            var emailVerificationTokenResponse = @"
{
    ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount""
}
";

            // POST returns email verification token response
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), emailVerificationTokenResponse, "application/json", transportError: false) as IHttpResponse));

            var href = $"/accounts/emailVerificationTokens/fooToken";
            await (this.dataStore as IInternalAsyncDataStore).CreateAsync<IResource, IEmailVerificationToken>(href, null, CancellationToken.None);
            await (this.dataStore as IInternalAsyncDataStore).CreateAsync<IResource, IEmailVerificationToken>(href, null, CancellationToken.None);

            // Not cached
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Does_not_cache_password_reset_tokens()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            var passwordResetTokenResponse = @"
{
    ""href"": ""https://api.stormpath.com/v1/applications/foo/passwordResetTokens/bar"",
    ""email"": ""john.smith@stormpath.com"",
    ""account"": {
        ""href"": ""https://api.stormpath.com/v1/accounts/foobarAccount""
    }
}
";

            // POST returns token response
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), passwordResetTokenResponse, "application/json", transportError: false) as IHttpResponse));

            // GET also returns token response
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), passwordResetTokenResponse, "application/json", transportError: false) as IHttpResponse));

            await this.dataStore.GetResourceAsync<IPasswordResetToken>("https://api.stormpath.com/v1/applications/foo/passwordResetTokens/bar");
            await this.dataStore.GetResourceAsync<IPasswordResetToken>("https://api.stormpath.com/v1/applications/foo/passwordResetTokens/bar");

            // Not cached
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Does_not_cache_login_attempts()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            var authResponse = @"
{
  ""account"": {
    ""href"" : ""https://api.stormpath.com/v1/accounts/5BedLIvyfLjdKKEEXAMPLE""
  }
}";

            // POST returns auth response
            requestExecutor
                .ExecuteAsync(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new FakeHttpResponse(200, "OK", new HttpHeaders(), authResponse, "application/json", transportError: false) as IHttpResponse));

            var request = new UsernamePasswordRequest("foo", "bar", null, null) as IAuthenticationRequest;
            var authenticator = new BasicAuthenticator(this.dataStore);

            var result1 = await authenticator.AuthenticateAsync("/loginAttempts", request, null, CancellationToken.None);
            var result2 = await authenticator.AuthenticateAsync("/loginAttempts", request, null, CancellationToken.None);

            // Not cached
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        [Obsolete("Add encryption functionality and remove this test.")]
        public async Task Does_not_cache_api_keys()
        {
            var cacheProvider = Caches.NewInMemoryCacheProvider().Build();
            this.dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.ApiKey).Object, cacheProvider);

            var request1 = await this.dataStore.GetResourceAsync<IApiKey>("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");
            var request2 = await this.dataStore.GetResourceAsync<IApiKey>("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");

            // Verify data from FakeJson.ApiKey
            request1.Href.ShouldBe("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");
            request1.Id.ShouldBe("83JFN57290NFKDHENXEXAMPLE");
            request1.Secret.ShouldBe("asdfqwerty1234567890/ASDFQWERTY09876example");
            request1.Status.ShouldBe(ApiKeyStatus.Enabled);

            // Not cached
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Regression test for stormpath/stormpath-sdk-dotnet#96.
        /// Unknown/new items in a JSON response should not cause the caching layer to explode.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous test.</returns>
        [Fact]
        public async Task Resource_with_unknown_property_is_not_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeResponse = FakeJson.Application.Replace("authorizedCallbackUris", "foobarProperty");
            this.BuildDataStore(fakeResponse, cacheProvider);

            var app1 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");
            var app2 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");

            app1.ShouldNotBeNull();
            app1.Name.ShouldBe("Lightsabers Galore");

            // Fail silently by falling back to no caching
            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Caches_scalar_arrays()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeResponse = FakeJson.Application;
            this.BuildDataStore(fakeResponse, cacheProvider);

            var app1 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");
            var app2 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");

            // The authorizedCallbackUris property is a List<string>, which the caching layer couldn't
            // handle in the first version.
            app1.AuthorizedCallbackUris.Count.ShouldBe(2);
            app2.AuthorizedCallbackUris.Count.ShouldBe(2);

            // Second request should hit cache
            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Regression test for caching empty scalar arrays.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous test.</returns>
        [Fact]
        public async Task Caches_empty_scalar_arrays()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeResponse = FakeJson.Application.Replace(
                    @"""authorizedCallbackUris"": [""https://foo.bar/1"", ""https://foo.bar/2""],",
                    @"""authorizedCallbackUris"": [],");

            this.BuildDataStore(fakeResponse, cacheProvider);

            var app1 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");
            var app2 = await this.dataStore.GetResourceAsync<IApplication>("/applications/foobarApplication");

            app1.AuthorizedCallbackUris.Count.ShouldBe(0);
            app2.AuthorizedCallbackUris.Count.ShouldBe(0);

            // Second request should hit cache
            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        public void Dispose()
        {
            this.dataStore?.Dispose();
        }
    }
}
