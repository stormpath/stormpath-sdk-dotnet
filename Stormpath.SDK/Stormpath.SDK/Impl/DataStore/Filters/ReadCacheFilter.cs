// <copyright file="ReadCacheFilter.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Provider;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class ReadCacheFilter : AbstractCacheFilter
    {
        private readonly string baseUrl;

        public ReadCacheFilter(string baseUrl, ICacheResolver cacheResolver)
            : base(cacheResolver)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            this.baseUrl = baseUrl;
        }

        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            if (this.cacheResolver.IsSynchronousSupported && this.IsCacheRetrievalEnabled(request))
            {
                var result = this.GetCachedResourceData(request);

                if (result != null)
                    return result; // short-circuit the remainder of the filter chain
            }

            // cache miss, let the chain continue
            return chain.Filter(request, logger);
        }

        public override async Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            if (this.cacheResolver.IsAsynchronousSupported && this.IsCacheRetrievalEnabled(request))
            {
                var result = await this.GetCachedResourceDataAsync(request, cancellationToken).ConfigureAwait(false);

                if (result != null)
                    return result; // short-circuit the remainder of the filter chain
            }

            // cache miss, let the chain continue
            return await chain.ExecuteAsync(request, logger, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IResourceDataResult> GetCachedResourceDataAsync(IResourceDataRequest request, CancellationToken cancellationToken)
        {
            // TODO isApiKeyCollectionQuery

            var cacheKey = this.GetCacheKey(request);
            if (string.IsNullOrEmpty(cacheKey))
                return null;
            // todo log - this is weird

            var data = await this.GetCachedValueAsync(cacheKey, request.ResourceType, cancellationToken).ConfigureAwait(false);

            if (data == null)
                return null;

            return new DefaultResourceDataResult(request.Action, request.ResourceType, request.Uri, 200, data);
        }

        private IResourceDataResult GetCachedResourceData(IResourceDataRequest request)
        {
            // TODO isApiKeyCollectionQuery

            var cacheKey = this.GetCacheKey(request);
            if (string.IsNullOrEmpty(cacheKey))
                return null;
            // todo log - this is weird

            var data = this.GetCachedValue(cacheKey, request.ResourceType);

            if (data == null)
                return null;

            return new DefaultResourceDataResult(request.Action, request.ResourceType, request.Uri, 200, data);
        }

        private bool IsCacheRetrievalEnabled(IResourceDataRequest request)
        {
            bool isRead = request.Action == ResourceAction.Read;
            bool isLoginAttempt = request.ResourceType == typeof(ILoginAttempt);
            bool isProviderAccountAccess = request.ResourceType == typeof(IProviderAccountAccess);
            bool isCollectionResource = ResourceTypes.IsCollectionResponse(request.ResourceType);

            return
                isRead &&
                !isLoginAttempt &&
                !isProviderAccountAccess &&
                !isCollectionResource;
        }
    }
}
