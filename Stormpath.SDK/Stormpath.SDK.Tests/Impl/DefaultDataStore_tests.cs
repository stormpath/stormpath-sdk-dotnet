// <copyright file="DefaultDataStore_tests.cs" company="Stormpath, Inc.">
//      Copyright (c) 2015 Stormpath, Inc.
// </copyright>
// <remarks>
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </remarks>

using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultDataStore_tests
    {
        private readonly IRequestExecutor fakeRequestExecutor;
        private readonly IDataStore dataStore;

        public DefaultDataStore_tests()
        {
            fakeRequestExecutor = Substitute.For<IRequestExecutor>();
            dataStore = new DefaultDataStore(fakeRequestExecutor, "http://foobar");
        }

        [Fact]
        public async Task Default_headers_are_applied_to_all_requests()
        {
            var href = "http://foobar/account";
            fakeRequestExecutor.ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DefaultHttpResponse(200, new HttpHeaders(), body: FakeJson.Account) as IHttpResponse));

            var account = await dataStore.GetResourceAsync<IAccount>(href);

            // Verify the default headers
            fakeRequestExecutor.Received().ExecuteAsync(Arg.Is<IHttpRequest>(request =>
                 request.Headers.Accept == "application/json")).IgnoreAwait();
        }

        [Fact(Skip ="TODO Saving")]
        public async Task ContentType_header_is_added_to_requests_with_body()
        {
            fakeRequestExecutor.ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DefaultHttpResponse(200, new HttpHeaders(), body: FakeJson.Account) as IHttpResponse));

            var account = await dataStore.SaveAsync<IAccount>(new DefaultAccount(dataStore));

            // Verify the default headers
            fakeRequestExecutor.Received().ExecuteAsync(Arg.Is<IHttpRequest>(request =>
                 request.Headers.Accept == "application/json")).IgnoreAwait();
        }
    }
}
