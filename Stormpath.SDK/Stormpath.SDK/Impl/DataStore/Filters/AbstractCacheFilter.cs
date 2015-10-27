// <copyright file="AbstractCacheFilter.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal abstract class AbstractCacheFilter : IAsynchronousFilter, ISynchronousFilter
    {
        protected readonly ICacheResolver cacheResolver;

        public AbstractCacheFilter(ICacheResolver cacheResolver)
        {
            if (cacheResolver == null)
                throw new ArgumentNullException(nameof(cacheResolver));

            this.cacheResolver = cacheResolver;
        }

        public abstract IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger);

        public abstract Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken);

        protected async Task<IDictionary<string, object>> GetCachedValueAsync(string cacheKey, Type resourceType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (resourceType == null)
                throw new ArgumentNullException(nameof(resourceType));

            var cache = await this.GetCacheAsync(resourceType, cancellationToken)
                .ConfigureAwait(false);
            return await cache.GetAsync(cacheKey, cancellationToken)
                .ConfigureAwait(false);
        }

        protected IDictionary<string, object> GetCachedValue(string cacheKey, Type resourceType)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (resourceType == null)
                throw new ArgumentNullException(nameof(resourceType));

            var cache = this.GetCache(resourceType);
            return cache.Get(cacheKey);
        }

        protected string GetCacheKey(IResourceDataRequest request)
        {
            return this.GetCacheKey(request.Uri.ResourcePath.ToString());
        }

        protected string GetCacheKey(string href)
        {
            return href;
        }

        protected Task<IAsynchronousCache<string, IDictionary<string, object>>> GetCacheAsync(Type resourceType, CancellationToken cancellationToken)
            => this.cacheResolver.GetCacheAsync(resourceType, cancellationToken);

        protected ISynchronousCache<string, IDictionary<string, object>> GetCache(Type resourceType)
            => this.cacheResolver.GetCache(resourceType);
    }
}
