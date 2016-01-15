// <copyright file="User_agent_tests.cs" company="Stormpath, Inc.">
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

using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Stormpath.SDK.Account;
using Stormpath.SDK.Client;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class User_agent_tests
    {
        [Fact]
        public async Task Csharp_code_creates_csharp_user_agent()
        {
            var fakeHttpClient = new ResourceReturningHttpClient("http://foo.bar", FakeJson.Account);
            var client = Clients.Builder()
                .SetApiKey(FakeApiKey.Create(valid: true))
                .SetBaseUrl("http://foo.bar/")
                .SetHttpClient(fakeHttpClient)
                .SetCacheProvider(Cache.Caches.NewDisabledCacheProvider())
                .Build();

            var tenant = await client.GetResourceAsync<IAccount>("http://foo.bar/fooAccount");

            var userAgent = fakeHttpClient.Calls.Single().Headers.UserAgent;
            userAgent.Split(' ').Count(x => x.StartsWith("lang")).ShouldBe(1);
            userAgent.ShouldContain("lang/csharp");
        }
    }
}
