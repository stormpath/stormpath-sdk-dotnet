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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Http;
using Stormpath.SDK.Shared;
using Stormpath.SDK.Tests.Fakes;
using Stormpath.SDK.Tests.Helpers;
using Xunit;

namespace Stormpath.SDK.Tests.Impl
{
    public class DefaultDataStore_tests
    {
        [Fact]
        public async Task Default_headers_are_applied_to_all_requests()
        {
            var stubRequestExecutor = Substitute.For<IRequestExecutor>();
            IInternalDataStore dataStore = new DefaultDataStore(stubRequestExecutor, "http://api.foo.bar", new SDK.Impl.NullLogger());

            var href = "http://foobar/account";
            stubRequestExecutor.ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new DefaultHttpResponse(200, "OK", new HttpHeaders(), body: FakeJson.Account, bodyContentType: "application/json") as IHttpResponse));

            var account = await dataStore.GetResourceAsync<IAccount>(href, CancellationToken.None);

            // Verify the default headers
            stubRequestExecutor.Received().ExecuteAsync(
                Arg.Is<IHttpRequest>(request =>
                    request.Headers.Accept == "application/json"),
                Arg.Any<CancellationToken>()).IgnoreAwait();
        }

        [Fact]
        public async Task Trace_log_is_sent_to_logger()
        {
            var stubRequestExecutor = new StubRequestExecutor<IAccount>(FakeJson.Account);

            var fakeLog = new List<LogEntry>();
            var stubLogger = Substitute.For<ILogger>();
            stubLogger.When(x => x.Log(Arg.Any<LogEntry>())).Do(call =>
            {
                fakeLog.Add(call.Arg<LogEntry>());
            });

            IInternalDataStore ds = new DefaultDataStore(stubRequestExecutor.Object, "http://api.foo.bar", stubLogger);

            var account = await ds.GetResourceAsync<IAccount>("account", CancellationToken.None);
            await account.DeleteAsync();

            fakeLog.Count.ShouldBeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task Saving_resource_posts_changed_values_only()
        {
            string savedHref = null;
            string savedJson = null;

            var stubRequestExecutor = new StubRequestExecutor<IAccount>(FakeJson.Account);
            stubRequestExecutor.Object
                .When(x => x.ExecuteAsync(Arg.Any<IHttpRequest>(), Arg.Any<CancellationToken>()))
                .Do(call =>
                {
                    savedHref = call.Arg<IHttpRequest>().CanonicalUri.ToString();
                    savedJson = call.Arg<IHttpRequest>().Body;
                });
            IInternalDataStore ds = new DefaultDataStore(stubRequestExecutor.Object, "http://api.foo.bar", new SDK.Impl.NullLogger());

            var account = await ds.GetResourceAsync<IAccount>("/account", CancellationToken.None);
            account.SetMiddleName("Test");
            account.SetUsername("newusername");
            await account.SaveAsync();

            savedHref.ShouldBe("https://api.stormpath.com/v1/accounts/foobarAccount");

            var savedMap = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(savedJson);
            savedMap.Count.ShouldBe(2);
            savedMap["middleName"].ShouldBe("Test");
            savedMap["username"].ShouldBe("newusername");
        }
    }
}
