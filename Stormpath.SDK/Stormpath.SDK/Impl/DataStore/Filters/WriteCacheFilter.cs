// <copyright file="WriteCacheFilter.cs" company="Stormpath, Inc.">
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Account;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Shared;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed class WriteCacheFilter : AbstractCacheFilter, IAsynchronousFilter, ISynchronousFilter
    {
        private readonly IResourceFactory resourceFactory;

        public WriteCacheFilter(ICacheResolver cacheResolver, IResourceFactory resourceFactory)
            : base(cacheResolver)
        {
            if (resourceFactory == null)
                throw new ArgumentNullException(nameof(resourceFactory));

            this.resourceFactory = resourceFactory;
        }

        public override async Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            if (request.Action == ResourceAction.Delete)
            {
                // TODO uncache
                throw new NotImplementedException();
            }

            var result = await chain.ExecuteAsync(request, logger, cancellationToken).ConfigureAwait(false);

            //todo edge cases:
            // - remove account from cache on email verification token
            // - cache collection *items*

            if (IsCacheable(request, result))
                await this.CacheAsync(result.Type, result.Body, result.Uri, cancellationToken).ConfigureAwait(false);

            //todo handle nested custom data

            return result;
        }

        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            // TODO
            return chain.Filter(request, logger);
        }

        private static bool IsCacheable(IResourceDataRequest request, IResourceDataResult result)
        {
            bool hasData = result?.Body?.Any() ?? false;

            bool isCollectionResponse =

            return

                // Must be a resource
                IsComplexResource(result?.Body) &&

                // Not currently caching collections
                !isCollectionResponse &&

                // Don't cache PasswordResetTokens
                result.Type != typeof(IPasswordResetToken) &&

                // ProviderAccountResults look like Accounts but should not be cached either
                result.Type != typeof(IProviderAccountResult);
        }

        private static bool IsComplexResource(IDictionary<string, object> data)
        {
            bool hasItems = data.Count > 1;
            bool hasHref = data.ContainsKey(AbstractResource.HrefPropertyName);

            return hasHref && hasItems;
        }

        private async Task CacheAsync(Type resourceType, IDictionary<string, object> data, CanonicalUri uri, CancellationToken cancellationToken)
        {
            string href = data[AbstractResource.HrefPropertyName].ToString();

            var cacheData = new Dictionary<string, object>();

            foreach (var item in data)
            {
                // TODO DefaultModelMap edge case

                // TODO ApiEncryptionMetadata edge case

                var asNestedResource = item.Value as IDictionary<string, object>;
                var asNestedArray = item.Value as IList<IDictionary<string, object>>;

                if (asNestedResource != null && IsComplexResource(asNestedResource))
                {
                    // TODO find the implied object type
                    // recursively cache
                    // convert this to an unmaterialized/canonical reference
                    throw new NotImplementedException();
                }
                else if (asNestedArray != null)
                {
                    throw new NotImplementedException();
                }

                bool isSensitive = DefaultAccount.PasswordPropertyName.Equals(item.Key);
                if (!isSensitive)
                    cacheData.Add(item.Key, item.Value);
            }

            var cache = await this.GetCacheAsync(resourceType, cancellationToken).ConfigureAwait(false);
            var cacheKey = this.GetCacheKey(href);
            await cache.PutAsync(cacheKey, cacheData, cancellationToken).ConfigureAwait(false);
        }
    }
}
