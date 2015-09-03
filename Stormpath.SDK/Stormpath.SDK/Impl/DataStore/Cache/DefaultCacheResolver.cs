// <copyright file="DefaultCacheResolver.cs" company="Stormpath, Inc.">
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.ThreadSafeMap;

namespace Stormpath.SDK.Impl.DataStore.Cache
{
    internal class DefaultCacheResolver : ICacheResolver
    {
        private readonly ICacheManager cacheManager;
        private readonly ISynchronousCacheManager syncCacheManager;
        private readonly IAsynchronousCacheManager asyncCacheManager;
        private readonly ICacheRegionNameResolver cacheRegionNameResolver;

        public DefaultCacheResolver(ICacheManager cacheManager, ICacheRegionNameResolver cacheRegionNameResolver)
        {
            if (cacheManager == null)
                throw new ArgumentNullException(nameof(cacheManager));
            if (cacheRegionNameResolver == null)
                throw new ArgumentNullException(nameof(cacheRegionNameResolver));

            this.cacheManager = cacheManager;
            this.syncCacheManager = cacheManager as ISynchronousCacheManager;
            this.asyncCacheManager = cacheManager as IAsynchronousCacheManager;
            this.cacheRegionNameResolver = cacheRegionNameResolver;
        }

        ISynchronousCache<string, IThreadSafeMap<string, object>> ICacheResolver.GetCache<T>()
        {
            if (!this.cacheManager.IsSynchronousSupported || this.syncCacheManager == null)
                throw new ApplicationException($"A synchronous caching path is not supported in {this.cacheManager.GetType().Name}");

            var cacheRegionName = this.cacheRegionNameResolver.GetCacheRegionName<T>();

            return this.syncCacheManager.GetCache<string, IThreadSafeMap<string, object>>(cacheRegionName);
        }

        Task<IAsynchronousCache<string, IThreadSafeMap<string, object>>> ICacheResolver.GetCacheAsync<T>(CancellationToken cancellationToken)
        {
            if (!this.cacheManager.IsAsynchronousSupported || this.asyncCacheManager == null)
                throw new ApplicationException($"An asynchronous caching path is not supported in {this.cacheManager.GetType().Name}");

            var cacheRegionName = this.cacheRegionNameResolver.GetCacheRegionName<T>();

            return this.asyncCacheManager.GetCacheAsync<string, IThreadSafeMap<string, object>>(cacheRegionName, cancellationToken);
        }
    }
}
