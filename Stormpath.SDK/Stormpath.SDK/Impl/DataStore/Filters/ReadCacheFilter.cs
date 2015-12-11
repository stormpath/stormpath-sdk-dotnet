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
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class ReadCacheFilter : AbstractCacheFilter
    {
        private readonly string baseUrl;
        private readonly ResourceTypeLookup typeLookup;

        public ReadCacheFilter(string baseUrl, ICacheResolver cacheResolver)
            : base(cacheResolver)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

            this.baseUrl = baseUrl;
            this.typeLookup = new ResourceTypeLookup();
        }

        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            bool cacheEnabled =
                this.cacheResolver.IsAsynchronousSupported
                && this.IsCacheRetrievalEnabled(request)
                && !request.SkipCache;
            if (cacheEnabled)
            {
                logger.Trace($"Checking cache for resource {request.Uri}", "ReadCacheFilter.Filter");
                var result = this.GetCachedResourceData(request, logger);

                if (result != null)
                {
                    logger.Trace($"Cache hit for {request.Uri}; returning cached data", "ReadCacheFilter.Filter");
                    return result; // short-circuit the remainder of the filter chain
                }

                logger.Trace($"Cache miss for {request.Uri}", "ReadCacheFilter.Filter");
            }

            return chain.Filter(request, logger);
        }

        public override async Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            bool cacheEnabled =
                this.cacheResolver.IsAsynchronousSupported
                && this.IsCacheRetrievalEnabled(request)
                && !request.SkipCache;
            if (cacheEnabled)
            {
                logger.Trace($"Checking cache for resource {request.Uri}", "ReadCacheFilter.FilterAsync");
                var result = await this.GetCachedResourceDataAsync(request, logger, cancellationToken).ConfigureAwait(false);

                if (result != null)
                {
                    logger.Trace($"Cache hit for {request.Uri}; returning cached data", "ReadCacheFilter.FilterAsync");
                    return result; // short-circuit the remainder of the filter chain
                }

                logger.Trace($"Cache miss for {request.Uri}", "ReadCacheFilter.FilterAsync");
            }

            return await chain.FilterAsync(request, logger, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IResourceDataResult> GetCachedResourceDataAsync(IResourceDataRequest request, ILogger logger, CancellationToken cancellationToken)
        {
            // TODO isApiKeyCollectionQuery
            var cacheKey = this.GetCacheKey(request);
            if (string.IsNullOrEmpty(cacheKey))
            {
                logger.Warn($"Could not construct cacheKey for request {request.Uri}; aborting", "ReadCacheFilter.GetCachedResourceDataAsync");
                return null;
            }

            var data = await this.GetCachedValueAsync(request.Type, cacheKey, cancellationToken).ConfigureAwait(false);

            if (data == null)
            {
                return null;
            }

            return new DefaultResourceDataResult(request.Action, request.Type, request.Uri, 200, data);
        }

        private IResourceDataResult GetCachedResourceData(IResourceDataRequest request, ILogger logger)
        {
            // TODO isApiKeyCollectionQuery
            var cacheKey = this.GetCacheKey(request);
            if (string.IsNullOrEmpty(cacheKey))
            {
                logger.Warn($"Could not construct cacheKey for request {request.Uri}; aborting", "ReadCacheFilter.GetCachedResourceData");
                return null;
            }

            var data = this.GetCachedValue(request.Type, cacheKey);

            if (data == null)
            {
                return null;
            }

            return new DefaultResourceDataResult(request.Action, request.Type, request.Uri, 200, data);
        }

        private bool IsCacheRetrievalEnabled(IResourceDataRequest request)
        {
            bool isRead = request.Action == ResourceAction.Read;
            bool isLoginAttempt = request.Type == typeof(ILoginAttempt);
            bool isProviderAccountAccess = request.Type == typeof(IProviderAccountAccess);
            bool isCollectionResource = this.typeLookup.IsCollectionResponse(request.Type);
            bool isExpandedRequest = request.Uri.ToString().Contains("expand=");

            return

                // Only consider cache retrieval for GETs
                isRead &&

                // Login attempts are always sent to the server
                !isLoginAttempt &&

                // Provider account access looks like an IAccount response, but it should not be cached
                !isProviderAccountAccess &&

                // Not currently caching collections
                !isCollectionResource &&

                // Always send expanded requests to the server
                !isExpandedRequest;
        }
    }
}
