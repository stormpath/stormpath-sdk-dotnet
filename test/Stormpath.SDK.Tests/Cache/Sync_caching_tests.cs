// <copyright file="Sync_caching_tests.cs" company="Stormpath, Inc.">
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
using System.Linq;
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
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
using Stormpath.SDK.Sync;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Cache
{
    public class Sync_caching_tests : IDisposable
    {
        private IInternalDataStore dataStore;

        private void BuildDataStore(string resourceResponse, ICacheProvider cacheProviderUnderTest)
        {
            var fakeRequestExecutor = new StubRequestExecutor(resourceResponse);

            this.dataStore = TestDataStore.Create(fakeRequestExecutor.Object, cacheProviderUnderTest);
        }

        [Fact]
        public void Resource_access_not_cached_with_null_cache()
        {
            var cacheProvider = CacheProviders.Create().DisabledCache();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            var account2 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account1.FullName.ShouldBe("Han Solo");

            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Resource_access_is_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            var account2 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account1.FullName.ShouldBe("Han Solo");

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Does_not_hit_cache_if_no_hronous_path_exists()
        {
            var fakeCacheProvider = Substitute.For<ISynchronousCacheProvider>();
            fakeCacheProvider.IsAsynchronousSupported.Returns(true);
            fakeCacheProvider.IsSynchronousSupported.Returns(false);
            fakeCacheProvider
                .When(x => x.GetSyncCache(Arg.Any<string>()))
                .Do(_ => { throw new NotImplementedException(); });

            this.BuildDataStore(FakeJson.Account, fakeCacheProvider);

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            account.Email.ShouldBe("han.solo@testmail.stormpath.com");
        }

        [Fact]
        public void Collection_access_is_not_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountList, cacheProvider);

            IQueryable<IAccount> accounts = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore).Synchronously();
            accounts.First();

            IQueryable<IAccount> accounts2 = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            accounts2.First();

            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Collection_items_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountList, cacheProvider);

            IQueryable<IAccount> accounts = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            accounts.First();

            var han = this.dataStore.GetResource<IAccount>("/accounts/account1");
            han.Email.ShouldBe("han.solo@testmail.stormpath.com");
            han.FullName.ShouldBe("Han Solo");

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Custom_data_is_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.CustomData, cacheProvider);

            var customData1 = this.dataStore.GetResource<ICustomData>("/accounts/foobarAccount/customData");
            var customData2 = this.dataStore.GetResource<ICustomData>("/accounts/foobarAccount/customData");

            customData2["membershipType"].ShouldBe("lifetime");

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Expanded_nested_resources_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountWithExpandedCustomData, cacheProvider);

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            account.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account.FullName.ShouldBe("Han Solo");

            var customDataFromHref = this.dataStore.GetResource<ICustomData>("/accounts/foobarAccount/customData");
            var customDataFromLink = account.GetCustomData();

            customDataFromHref["isAdmin"].ShouldBe(false);

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Expanded_collection_items_are_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.AccountWithExpandedGroups, cacheProvider);

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount?expand=groups(offset:0,limit:25)");
            var group1 = this.dataStore.GetResource<IGroup>("/groups/group1");
            var group2 = this.dataStore.GetResource<IGroup>("/groups/group2");

            group1.Name.ShouldBe("Imperials");
            group2.Name.ShouldBe("Rebels");

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Deleting_resource_removes_cached_item()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");

            account1.Delete();

            var account2 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");

            account1.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account1.FullName.ShouldBe("Han Solo");

            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get));
        }

        [Fact]
        public void Updating_resource_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns original
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false));

            // Save returns update data
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account.Replace("han.solo@testmail.stormpath.com", "han@solo.me"), "application/json", transportError: false));

            var account1 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            account1.Email.ShouldBe("han.solo@testmail.stormpath.com");

            account1.SetEmail("han@solo.me");
            account1.Save();
            account1.Email.ShouldBe("han@solo.me");

            var account2 = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            account2.Email.ShouldBe("han@solo.me");

            // Only one GET; second is intercepted by the cache (but the updated data is returned!) #magic
            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get));
        }

        [Fact]
        public void Updating_custom_data_with_proxy_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns expanded request
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.AccountWithExpandedCustomData, "application/json", transportError: false));

            // Save is not an expanded request
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false));

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount?expand=customData");

            account.CustomData.Put("isAdmin", true);
            account.CustomData.Put("writeAccess", "yes");
            account.Save();

            var customData = account.GetCustomData();
            customData["isAdmin"].ShouldBe(true);
            customData["writeAccess"].ShouldBe("yes");

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get));
        }

        [Fact]
        public void Deleting_custom_data_with_proxy_updates_cache()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var requestExecutor = Substitute.For<IRequestExecutor>();
            this.dataStore = TestDataStore.Create(requestExecutor, cacheProvider);

            // GET returns expanded request
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.AccountWithExpandedCustomData, "application/json", transportError: false));

            // Save is not an expanded request
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(201, "Created", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false));

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount?expand=customData");
            account.CustomData.Remove("isAdmin");
            account.Save();

            var customData = account.GetCustomData();
            customData["isAdmin"].ShouldBe(null);

            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get));
        }

        [Fact]
        public void Custom_data_is_always_cached_on_parent_resource_save()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account = this.dataStore.Instantiate<IAccount>();
            account.CustomData.Put("foo", "bar");
            account.CustomData.Put("baz", 1234);
            (this.dataStore as IInternalSyncDataStore).Create("/accounts", account);

            var customData = account.GetCustomData();
            customData["foo"].ShouldBe("bar");
            customData["baz"].ShouldBe(1234);

            // CustomData was cached; did not make a request
            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Custom_data_updates_are_not_cached_if_authoritative_custom_data_is_not_cached()
        {
            // This test differs from Updating_custom_data_with_proxy_updates_cache
            // because we aren't GETting the custom data first (whether explicitly or with an expanded query).
            // In this case, we don't want to cache updates because we have no authoritative version.
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account = this.dataStore.GetResource<IAccount>("/account");
            account.CustomData.Put("foo", "bar!");
            account.CustomData.Put("isWorking", true);
            account.Save();

            var customData = account.GetCustomData();

            // CustomData was *not* cached
            this.dataStore.RequestExecutor.Received(3).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Email_verification_result_removes_associated_account_from_cache()
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
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), emailVerificationTokenResponse, "application/json", transportError: false));

            // GET returns account as unverified first,
            // then second GET returns account as verified
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get))
                .Returns(
                    new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account.Replace(@"""status"": ""ENABLED""", @"""status"": ""UNVERIFIED"""), "application/json", transportError: false),
                    new FakeHttpResponse(200, "OK", new HttpHeaders(), FakeJson.Account, "application/json", transportError: false));

            var account = this.dataStore.GetResource<IAccount>("/accounts/foobarAccount");
            account.Status.ShouldBe(AccountStatus.Unverified);

            var href = $"/accounts/emailVerificationTokens/fooToken";
            var tokenResponse = (this.dataStore as IInternalSyncDataStore).Create<IResource, IEmailVerificationToken>(href, null);
            this.dataStore.GetResource<IAccount>(tokenResponse.Href);

            account.Status.ShouldBe(AccountStatus.Enabled);

            // Second GET should *not* hit cache
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Is<IHttpRequest>(x => x.Method == HttpMethod.Get));
        }

        [Fact]
        public void Does_not_cache_provider_account_access()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();
            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var customData1 = this.dataStore.GetResource<IProviderAccountResult>("/accounts/foobarAccount");
            var customData2 = this.dataStore.GetResource<IProviderAccountResult>("/accounts/foobarAccount");

            // Not cached
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Does_not_cache_email_verification_tokens()
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
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), emailVerificationTokenResponse, "application/json", transportError: false));

            var href = $"/accounts/emailVerificationTokens/fooToken";
            (this.dataStore as IInternalSyncDataStore).Create<IResource, IEmailVerificationToken>(href, null);
            (this.dataStore as IInternalSyncDataStore).Create<IResource, IEmailVerificationToken>(href, null);

            // Not cached
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Does_not_cache_password_reset_tokens()
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
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), passwordResetTokenResponse, "application/json", transportError: false));

            // GET also returns token response
            requestExecutor
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Get))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), passwordResetTokenResponse, "application/json", transportError: false));

            this.dataStore.GetResource<IPasswordResetToken>("https://api.stormpath.com/v1/applications/foo/passwordResetTokens/bar");
            this.dataStore.GetResource<IPasswordResetToken>("https://api.stormpath.com/v1/applications/foo/passwordResetTokens/bar");

            // Not cached
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Does_not_cache_login_attempts()
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
                .Execute(Arg.Is<IHttpRequest>(req => req.Method == HttpMethod.Post))
                .Returns(new FakeHttpResponse(200, "OK", new HttpHeaders(), authResponse, "application/json", transportError: false));

            var request = new UsernamePasswordRequest("foo", "bar", null, null) as IAuthenticationRequest;
            var authenticator = new BasicAuthenticator(this.dataStore);

            var result1 = authenticator.Authenticate("/loginAttempts", request, null);
            var result2 = authenticator.Authenticate("/loginAttempts", request, null);

            // Not cached
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        [Obsolete("Add encryption functionality and remove this test.")]
        public void Does_not_cache_api_keys()
        {
            var cacheProvider = Caches.NewInMemoryCacheProvider().Build();
            this.dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.ApiKey).Object, cacheProvider);

            var request1 = this.dataStore.GetResource<IApiKey>("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");
            var request2 = this.dataStore.GetResource<IApiKey>("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");

            // Verify data from FakeJson.ApiKey
            request1.Href.ShouldBe("https://api.stormpath.com/v1/apiKeys/83JFN57290NFKDHENXEXAMPLE");
            request1.Id.ShouldBe("83JFN57290NFKDHENXEXAMPLE");
            request1.Secret.ShouldBe("asdfqwerty1234567890/ASDFQWERTY09876example");
            request1.Status.ShouldBe(ApiKeyStatus.Enabled);

            // Not cached
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        /// <summary>
        /// Regression test for stormpath/stormpath-sdk-dotnet#96.
        /// Unknown/new items in a JSON response should not cause the caching layer to explode.
        /// </summary>
        [Fact]
        public void Resource_with_unknown_property_is_not_cached()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeData = FakeJson.Application.Replace("authorizedCallbackUris", "foobarProperty");
            this.BuildDataStore(fakeData, cacheProvider);

            var app1 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");
            var app2 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");

            app1.ShouldNotBeNull();
            app1.Name.ShouldBe("Lightsabers Galore");

            // Fail silently by falling back to no caching
            this.dataStore.RequestExecutor.Received(2).Execute(
                Arg.Any<IHttpRequest>());
        }

        [Fact]
        public void Caches_scalar_arrays()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeResponse = FakeJson.Application;
            this.BuildDataStore(fakeResponse, cacheProvider);

            var app1 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");
            var app2 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");

            // The authorizedCallbackUris property is a List<string>, which the caching layer couldn't
            // handle in the first version.
            app1.AuthorizedCallbackUris.Count.ShouldBe(2);
            app2.AuthorizedCallbackUris.Count.ShouldBe(2);

            // Second request should hit cache
            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        /// <summary>
        /// Regression test for caching empty scalar arrays.
        /// </summary>
        [Fact]
        public void Caches_empty_scalar_arrays()
        {
            var cacheProvider = CacheProviders.Create().InMemoryCache().Build();

            var fakeResponse = FakeJson.Application.Replace(
                    @"""authorizedCallbackUris"": [""https://foo.bar/1"", ""https://foo.bar/2""],",
                    @"""authorizedCallbackUris"": [],");

            this.BuildDataStore(fakeResponse, cacheProvider);

            var app1 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");
            var app2 = this.dataStore.GetResource<IApplication>("/applications/foobarApplication");

            app1.AuthorizedCallbackUris.Count.ShouldBe(0);
            app2.AuthorizedCallbackUris.Count.ShouldBe(0);

            // Second request should hit cache
            this.dataStore.RequestExecutor.Received(1).Execute(
                Arg.Any<IHttpRequest>());
        }

        public void Dispose()
        {
            this.dataStore?.Dispose();
        }
    }
}
