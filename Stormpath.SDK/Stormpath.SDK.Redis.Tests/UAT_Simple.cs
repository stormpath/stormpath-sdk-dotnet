// <copyright file="UAT_simple.cs" company="Stormpath, Inc.">
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

using System.Threading.Tasks;
using Stormpath.SDK.Application;
using Stormpath.SDK.Client;
using Stormpath.SDK.Extensions.Serialization;
using Stormpath.SDK.Logging;
using Xunit;

namespace Stormpath.SDK.Extensions.Cache.Redis.Tests
{
    public class UAT_simple
    {
        [Fact]
        public async Task MyTestMethodAsync()
        {
            var redisCacheProvider = new RedisCacheProvider("localhost:6379", new JsonNetSerializer());

            var logger = new InMemoryLogger();

            var client = Clients.Builder()
                .SetCacheProvider(redisCacheProvider)
                .SetLogger(logger)
                .Build();
            var tenant = await client.GetCurrentTenantAsync();

            // Prime the cache
            var application = await tenant.GetApplications().FirstAsync();

            // Should be cache hit
            await client.GetResourceAsync<IApplication>(application.Href, req => req.Expand(x => x.GetCustomDataAsync));

            // Should be cache hit
            var customData = await application.GetCustomDataAsync();

            var log = logger.ToString();
        }
    }
}
