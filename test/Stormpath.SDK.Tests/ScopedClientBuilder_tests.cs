// <copyright file="ScopedClientBuilder_tests.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;
using Stormpath.SDK.Serialization;
using Stormpath.SDK.Tests.Common.Fakes;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class ScopedClientBuilder_tests
    {
        [Fact]
        public async Task Creates_new_client_with_options()
        {
            var fakeHttpClient = new ResourceReturningHttpClient("http://foo.bar", FakeJson.Account);
            var baseClient = Clients.Builder()
                .SetApiKeyId("foo")
                .SetApiKeySecret("bar")
                .SetBaseUrl("http://foo.bar/")
                .SetHttpClient(fakeHttpClient)
                .SetSerializer(Serializers.Create().JsonNetSerializer())
                .SetCacheProvider(CacheProviders.Create().DisabledCache())
                .Build();

            IScopedClientFactory scopedClientFactory = new ScopedClientFactory(baseClient);

            var scopedClient = scopedClientFactory.Create(new ScopedClientOptions()
            {
                Identifier = "foobar",
                UserAgent = "foo/123"
            });

            scopedClient.ShouldNotBeSameAs(baseClient);

            var tenant = await scopedClient.GetResourceAsync<IAccount>("http://foo.bar/fooAccount");
            var userAgent = fakeHttpClient.Calls.Single().Headers.UserAgent;

            userAgent.ShouldStartWith("foo/123");
        }
    }
}
