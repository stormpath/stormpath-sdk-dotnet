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
        private readonly ResourceTypes resourceTypes;

        public WriteCacheFilter(ICacheResolver cacheResolver, IResourceFactory resourceFactory)
            : base(cacheResolver)
        {
            if (resourceFactory == null)
                throw new ArgumentNullException(nameof(resourceFactory));

            this.resourceFactory = resourceFactory;
            this.resourceTypes = new ResourceTypes();
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

            if (IsCacheable(request, result))
                await this.CacheAsync(result.Type, result.Body, cancellationToken).ConfigureAwait(false);

            //todo handle nested custom data

            return result;
        }

        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            // TODO
            return chain.Filter(request, logger);
        }

        private async Task CacheAsync(Type resourceType, IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            string href = data[AbstractResource.HrefPropertyName].ToString();

            var cacheData = new Dictionary<string, object>();

            // TODO CustomData edge case

            foreach (var item in data)
            {
                string key = item.Key;
                object value = item.Value;

                // TODO DefaultModelMap edge case
                // TODO ApiEncryptionMetadata edge case

                var asNestedResource = value as IDictionary<string, object>;
                var asNestedArray = value as IEnumerable<IDictionary<string, object>>;

                if (asNestedResource != null && IsResource(asNestedResource))
                {
                    var nestedType = this.resourceTypes.GetInterface(item.Key);
                    if (nestedType == null)
                        throw new ApplicationException($"Cannot cache nested item. Item type for '{item.Key}' unknown.");

                    await this.CacheAsync(nestedType, asNestedResource, cancellationToken).ConfigureAwait(false);
                    value = ToCanonicalReference(asNestedResource);
                }
                else if (asNestedArray != null)
                {
                    // This is a CollectionResponsePage<T>.Items property
                    // Find the type of objects to expect
                    var nestedType = this.resourceTypes.GetInnerCollectionInterface(resourceType);
                    if (nestedType == null)
                        throw new ApplicationException($"Can not cache array '{key}'. Item type for '{resourceType.Name}' unknown.");

                    // Recursively cache nested resources and create a new collection that only has references
                    var canonicalList = new List<object>();
                    foreach (var element in asNestedArray)
                    {
                        object canonicalElement = element;
                        var resourceElement = canonicalElement as IDictionary<string, object>;
                        if (resourceElement != null)
                        {
                            if (IsResource(resourceElement))
                            {
                                await this.CacheAsync(nestedType, resourceElement, cancellationToken).ConfigureAwait(false);
                                canonicalElement = ToCanonicalReference(resourceElement);
                            }
                        }

                        canonicalList.Add(canonicalElement);
                    }
                }

                bool isSensitive = DefaultAccount.PasswordPropertyName.Equals(key);
                if (!isSensitive)
                    cacheData.Add(key, value);
            }

            if (!ResourceTypes.IsCollectionResponse(resourceType))
            {
                var cache = await this.GetCacheAsync(resourceType, cancellationToken).ConfigureAwait(false);
                var cacheKey = this.GetCacheKey(href);
                await cache.PutAsync(cacheKey, cacheData, cancellationToken).ConfigureAwait(false);
            }
        }

        private static bool IsCacheable(IResourceDataRequest request, IResourceDataResult result)
        {
            bool hasData = result?.Body?.Any() ?? false;

            return

                // Must be a resource
                IsResource(result?.Body) &&

                // Don't cache PasswordResetTokens
                result.Type != typeof(IPasswordResetToken) &&

                // ProviderAccountResults look like Accounts but should not be cached either
                result.Type != typeof(IProviderAccountResult);
        }

        private static bool IsResource(IDictionary<string, object> data)
        {
            bool hasItems = data.Count > 1;
            bool hasHref = data.ContainsKey(AbstractResource.HrefPropertyName);

            return hasHref && hasItems;
        }

        private static object ToCanonicalReference(IDictionary<string, object> resourceData)
        {
            if (IsResource(resourceData))
                return new LinkProperty(resourceData[AbstractResource.HrefPropertyName].ToString());

            // TODO collections or other stuff?
            throw new NotImplementedException();
        }
    }
}
