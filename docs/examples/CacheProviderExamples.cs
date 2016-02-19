// <copyright file="CacheProviderExamples.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Client;

namespace Stormpath.SDK.DocExamples
{
    public class CacheProviderExamples
    {
        public void DisableCaching()
        {
            IClientBuilder clientBuilder = null;

            #region DisableCaching
            // To disable caching, pass null:
            clientBuilder.SetCacheProvider(null);

            // Or, use DisabledCache():
            clientBuilder.SetCacheProvider(CacheProviders.Create().DisabledCache());
            #endregion
        }

        public void InMemoryCacheWithOptions()
        {
            IClientBuilder clientBuilder = null;

            #region InMemoryCacheWithOptions
            clientBuilder.SetCacheProvider(
                CacheProviders.Create().InMemoryCache()
                    // Default TTI is 60 minutes
                    .WithDefaultTimeToIdle(TimeSpan.FromMinutes(60))

                    // Default TTL is 60 minutes
                    .WithDefaultTimeToLive(TimeSpan.FromMinutes(60))

                    // TTL and TTI can be overridden on a per-resource basis
                    .WithCache(Caches.ForResource<IAccount>()
                        .WithTimeToIdle(TimeSpan.FromMinutes(30))
                        .WithTimeToLive(TimeSpan.FromMinutes(30)))
                    .Build());
            #endregion
        }
    }
}
