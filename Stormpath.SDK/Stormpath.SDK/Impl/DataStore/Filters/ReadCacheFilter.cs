// <copyright file="ReadCacheFilter.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Impl.Auth;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class ReadCacheFilter : IAsynchronousFilter, ISynchronousFilter
    {
        private readonly string baseUrl;
        private readonly ICacheResolver cacheResolver;

        public ReadCacheFilter(string baseUrl, ICacheResolver cacheResolver)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));
            if (cacheResolver == null)
                throw new ArgumentNullException(nameof(cacheResolver));

            this.baseUrl = baseUrl;
            this.cacheResolver = cacheResolver;

            throw new NotImplementedException();
        }

        IResourceDataResult ISynchronousFilter.Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            if (this.cacheResolver.IsSynchronousSupported && this.IsCacheRetrievalEnabled(request))
            {
                var result = this.GetCachedResourceData(request);

                if (result != null)
                    return result; // short-circuit the remainder of the filter chain
            }

            return chain.Filter(request, logger); // cache miss
        }

        Task<IResourceDataResult> IAsynchronousFilter.FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private IResourceDataResult GetCachedResourceData(IResourceDataRequest request)
        {
            IDictionary<string, object> data = null;

            var cacheKey = this.GetCacheKey(request);

            if (data == null)
                return null;

            return new DefaultResourceDataResult(request.Action, request.ResourceType, request.Uri, 200, data);
        }

        private string GetCacheKey(IResourceDataRequest request)
        {
            throw new NotImplementedException();
        }

        private bool IsCacheRetrievalEnabled(IResourceDataRequest request)
        {
            bool isRead = request.Action == ResourceAction.Read;
            bool isLoginAttempt = request.ResourceType is ILoginAttempt;

            return
                isRead &&
                !isLoginAttempt;
        }
    }
}
