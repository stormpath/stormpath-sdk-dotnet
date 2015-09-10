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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    internal class DefaultCacheResolver : ICacheResolver
    {
        private readonly ICacheProvider cacheProvider;
        private readonly ISynchronousCacheProvider syncCacheProvider;
        private readonly IAsynchronousCacheProvider asyncCacheProvider;
        private readonly ICacheRegionNameResolver cacheRegionNameResolver;

        bool ICacheResolver.IsSynchronousSupported => this.cacheProvider.IsAsynchronousSupported;

        bool ICacheResolver.IsAsynchronousSupported => this.cacheProvider.IsSynchronousSupported;

        public DefaultCacheResolver(ICacheProvider cacheProvider, ICacheRegionNameResolver cacheRegionNameResolver)
        {
            if (cacheProvider == null)
                throw new ArgumentNullException(nameof(cacheProvider));
            if (cacheRegionNameResolver == null)
                throw new ArgumentNullException(nameof(cacheRegionNameResolver));

            this.cacheProvider = cacheProvider;
            this.syncCacheProvider = cacheProvider as ISynchronousCacheProvider;
            this.asyncCacheProvider = cacheProvider as IAsynchronousCacheProvider;
            this.cacheRegionNameResolver = cacheRegionNameResolver;
        }

        ISynchronousCache<string, IDictionary<string, object>> ICacheResolver.GetCache<T>()
        {
            if (!this.cacheProvider.IsSynchronousSupported || this.syncCacheProvider == null)
                throw new ApplicationException($"A synchronous caching path is not supported in {this.cacheProvider.GetType().Name}");

            var cacheRegionName = this.cacheRegionNameResolver.GetCacheRegionName<T>();

            return this.syncCacheProvider.GetCache<string, IDictionary<string, object>>(cacheRegionName);
        }

        Task<IAsynchronousCache<string, IDictionary<string, object>>> ICacheResolver.GetCacheAsync<T>(CancellationToken cancellationToken)
        {
            if (!this.cacheProvider.IsAsynchronousSupported || this.asyncCacheProvider == null)
                throw new ApplicationException($"An asynchronous caching path is not supported in {this.cacheProvider.GetType().Name}");

            var cacheRegionName = this.cacheRegionNameResolver.GetCacheRegionName<T>();

            return this.asyncCacheProvider.GetCacheAsync<string, IDictionary<string, object>>(cacheRegionName, cancellationToken);
        }
    }
}
