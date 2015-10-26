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
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Http;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Provider;
using Stormpath.SDK.Resource;
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
            bool isDelete = request.Action == ResourceAction.Delete;
            bool isCustomDataPropertyRequest = request.Uri.ResourcePath.ToString().Contains("/customData/");

            if (isCustomDataPropertyRequest && isDelete)
            {
                await this.UncacheCustomDataPropertyAsync(request.Uri.ResourcePath, cancellationToken).ConfigureAwait(false);
            }
            else if (isDelete)
            {
                var cacheKey = this.GetCacheKey(request);
                await this.UncacheAsync(request.ResourceType, cacheKey, cancellationToken).ConfigureAwait(false);
            }

            // Execute request and get result
            var result = await chain.ExecuteAsync(request, logger, cancellationToken).ConfigureAwait(false);

            //todo edge cases:
            // - remove account from cache on email verification token

            bool possibleCustomDataUpdate =
                (request.Action == ResourceAction.Create ||
                request.Action == ResourceAction.Update) &&
                AbstractExtendableInstanceResource.IsExtendable(request.ResourceType);
            if (possibleCustomDataUpdate)
                await this.CacheNestedCustomDataUpdatesAsync(request.Uri.ResourcePath.ToString(), request.Properties, cancellationToken).ConfigureAwait(false);

            if (IsCacheable(request, result))
                await this.CacheAsync(result.Type, result.Body, cancellationToken).ConfigureAwait(false);

            //todo handle nested custom data

            return result;
        }

        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            // TODO
            throw new NotImplementedException();
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

                var asNestedResource = value as ExpandedProperty;
                var asNestedArray = value as IEnumerable<IDictionary<string, object>>;

                if (asNestedResource != null && IsResource(asNestedResource.Data))
                {
                    var nestedType = this.resourceTypes.GetInterface(item.Key);
                    if (nestedType == null)
                        throw new ApplicationException($"Cannot cache nested item. Item type for '{item.Key}' unknown.");

                    await this.CacheAsync(nestedType, asNestedResource.Data, cancellationToken).ConfigureAwait(false);
                    value = ToCanonicalReference(asNestedResource.Data);
                }
                else if (asNestedArray != null)
                {
                    // This is a CollectionResponsePage<T>.Items property. Find the type of objects to expect
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

        private async Task CacheNestedCustomDataUpdatesAsync(string parentHref, IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            object customDataObj = null;
            IDictionary<string, object> customData = null;

            if (!data.TryGetValue(AbstractExtendableInstanceResource.CustomDataPropertyName, out customDataObj))
                return;

            customData = customDataObj as IDictionary<string, object>;
            if (customData.IsNullOrEmpty())
                return;

            var customDataHref = parentHref + "/customData";
            var updatedDataToCache = await this.GetCachedValueAsync(customDataHref, typeof(ICustomData), cancellationToken).ConfigureAwait(false);
            if (updatedDataToCache.IsNullOrEmpty())
                updatedDataToCache = new Dictionary<string, object>();

            foreach (var updatedItem in customData)
            {
                updatedDataToCache[updatedItem.Key] = updatedItem.Value;
            }

            await this.CacheAsync(typeof(ICustomData), updatedDataToCache, cancellationToken).ConfigureAwait(false);
        }

        private async Task UncacheAsync(Type resourceType, string cacheKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cacheKey))
                throw new ArgumentNullException(nameof(cacheKey));
            if (resourceType == null)
                throw new ArgumentNullException(nameof(resourceType));

            var cache = await this.GetCacheAsync(resourceType, cancellationToken).ConfigureAwait(false);
            await cache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }

        private async Task UncacheCustomDataPropertyAsync(Uri resourceUri, CancellationToken cancellationToken)
        {
            var href = resourceUri.ToString();
            var propertyName = href.Substring(href.LastIndexOf('/') + 1);
            href = href.Substring(0, href.LastIndexOf('/'));

            if (string.IsNullOrEmpty(propertyName) ||
                string.IsNullOrEmpty(href))
                throw new ApplicationException("Could not update cache for removed custom data entry.");

            var cache = await this.GetCacheAsync(typeof(ICustomData), cancellationToken).ConfigureAwait(false);
            var cacheKey = this.GetCacheKey(href);

            var existingData = await cache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            if (existingData.IsNullOrEmpty())
                return;

            existingData.Remove(propertyName);
            await cache.PutAsync(cacheKey, existingData, cancellationToken).ConfigureAwait(false);
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
            if (data == null)
                return false;

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
