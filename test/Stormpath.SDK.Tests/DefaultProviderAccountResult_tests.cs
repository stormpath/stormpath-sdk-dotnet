// <copyright file="DefaultProviderAccountResult_tests.cs" company="Stormpath, Inc.">
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
using Shouldly;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Tests.Common;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class DefaultProviderAccountResult_tests
    {
        [Fact]
        public async Task When_getting_new_provider_based_account()
        {
            // 201 Created indicates the account is new
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.Account).Object) as IInternalAsyncDataStore;
            dataStore.RequestExecutor
                .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IHttpResponse>(new FakeHttpResponse(201, "Created", null, FakeJson.Account, "application/json", transportError: false)));

            var providerAccountResult = await dataStore.GetResourceAsync<IProviderAccountResult>("/providerAccount", CancellationToken.None);

            providerAccountResult.IsNewAccount.ShouldBeTrue();
            var account = providerAccountResult.Account;

            // Verify against data from FakeJson.Account
            account.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account.FullName.ShouldBe("Han Solo");
        }

        [Fact]
        public async Task When_getting_existing_provider_based_account()
        {
            // 200 OK indicates the account is not new
            var dataStore = TestDataStore.Create(new StubRequestExecutor(FakeJson.Account).Object) as IInternalAsyncDataStore;
            dataStore.RequestExecutor
                .ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IHttpResponse>(new FakeHttpResponse(200, "OK", null, FakeJson.Account, "application/json", transportError: false)));

            var providerAccountResult = await dataStore.GetResourceAsync<IProviderAccountResult>("/providerAccount", CancellationToken.None);

            providerAccountResult.IsNewAccount.ShouldBeFalse();
            var account = providerAccountResult.Account;

            // Verify against data from FakeJson.Account
            account.Email.ShouldBe("han.solo@testmail.stormpath.com");
            account.FullName.ShouldBe("Han Solo");
        }
    }
}
