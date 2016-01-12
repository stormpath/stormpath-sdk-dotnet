// <copyright file="Expansion_save_options_tests.cs" company="Stormpath, Inc.">
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

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Stormpath.SDK.Account;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Tests.Common.Fakes;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class Expansion_save_options_tests
    {
        private static async Task GeneratedArgumentsWere(IInternalDataStore dataStore, string expectedQueryString)
        {
            await dataStore.RequestExecutor.Received(1).ExecuteAsync(
                Arg.Is<IHttpRequest>(x => x.CanonicalUri.QueryString.ToString() == expectedQueryString),
                Arg.Any<CancellationToken>());
        }

        private IInternalDataStore BuildDataStore(string resourceResponse)
        {
            return TestDataStore.Create(new StubRequestExecutor(resourceResponse).Object);
        }

        [Fact]
        public async Task Inline_syntax()
        {
            var dataStore = this.BuildDataStore(FakeJson.Account);
            var account = await dataStore.GetResourceAsync<IAccount>("/foobarAccount");
            account.SetEmail("test");

            await account.SaveAsync(response => response.Expand(x => x.GetCustomData()));

            await GeneratedArgumentsWere(dataStore, "expand=customData");
        }

        [Fact]
        public async Task Inline_syntax_with_no_save_payload()
        {
            var dataStore = this.BuildDataStore(FakeJson.Account);
            var account = await dataStore.GetResourceAsync<IAccount>("/foobarAccount");

            await account.SaveAsync(response => response.Expand(x => x.GetCustomData()));

            await GeneratedArgumentsWere(dataStore, "expand=customData");
        }

        [Fact]
        public async Task Expanded_syntax()
        {
            var dataStore = this.BuildDataStore(FakeJson.Account);
            var account = await dataStore.GetResourceAsync<IAccount>("/foobarAccount");
            account.SetEmail("test");

            await account.SaveAsync(response =>
            {
                response.Expand(x => x.GetDirectory());
                response.Expand(x => x.GetGroups(10, 10));
            });

            await GeneratedArgumentsWere(dataStore, "expand=directory,groups(offset:10,limit:10)");
        }

        [Fact]
        public async Task Expanded_syntax_with_no_save_payload()
        {
            var dataStore = this.BuildDataStore(FakeJson.Account);
            var account = await dataStore.GetResourceAsync<IAccount>("/foobarAccount");

            await account.SaveAsync(response =>
            {
                response.Expand(x => x.GetDirectory());
                response.Expand(x => x.GetGroups(10, 10));
            });

            await GeneratedArgumentsWere(dataStore, "expand=directory,groups(offset:10,limit:10)");
        }
    }
}
