// <copyright file="Caching_tests.cs" company="Stormpath, Inc.">
// Copyright (c) 2015 Stormpath, Inc.
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
using Stormpath.SDK.Cache;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Linq;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Linq;
using Stormpath.SDK.Tests.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests.Impl.Cache
{
    public class Caching_tests : IDisposable
    {
        private static readonly string BaseUrl = "https://api.stormpath.com/v1";
        private IInternalDataStore dataStore;

        private void BuildDataStore(string resourceResponse, ICacheProvider cacheProviderUnderTest)
        {
            var fakeRequestExecutor = new StubRequestExecutor(resourceResponse);

            this.dataStore = new DefaultDataStore(
                fakeRequestExecutor.Object,
                baseUrl: BaseUrl,
                serializer: new JsonNetSerializer(),
                logger: new NullLogger(),
                cacheProvider: cacheProviderUnderTest,
                identityMapExpiration: TimeSpan.FromMinutes(10));
        }

        [Fact]
        public async Task Resource_access_not_cached_with_null_cache()
        {
            var cacheProvider = Caches.NewDisabledCacheProvider();

            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            await this.dataStore.RequestExecutor.Received(2).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Resource_access_is_cached()
        {
            var cacheProvider = Caches.NewInMemoryCacheProvider().Build();

            this.BuildDataStore(FakeJson.Account, cacheProvider);

            var account1 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");
            var account2 = await this.dataStore.GetResourceAsync<IAccount>("/accounts/foobarAccount");

            (account1 as AbstractResource).IsLinkedTo(account2 as AbstractResource).ShouldBeTrue();

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Collection_access_is_not_cached()
        {
            var cacheProvider = Caches.NewInMemoryCacheProvider().Build();
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
            var cacheProvider = Caches.NewInMemoryCacheProvider().Build();
            this.BuildDataStore(FakeJson.AccountList, cacheProvider);

            IAsyncQueryable<IAccount> accounts = new CollectionResourceQueryable<IAccount>("/accounts", this.dataStore);
            await accounts.MoveNextAsync();

            var han = this.dataStore.GetResourceAsync<IAccount>("/accounts/account1");

            await this.dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Any<IHttpRequest>(),
                Arg.Any<CancellationToken>());
        }

        //test IProviderAccountAccess
        //test CollectionResponsePage

        public void Dispose()
        {
            this.dataStore?.Dispose();
            //todo test disposing a live cache
        }
    }
}
