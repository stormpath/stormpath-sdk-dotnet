// <copyright file="WriteCacheFilter.Async.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.Auth;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.Account;
using Stormpath.SDK.Impl.Cache;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Stormpath.SDK.Provider;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed partial class WriteCacheFilter : AbstractCacheFilter, IAsynchronousFilter, ISynchronousFilter
    {
        private readonly IResourceFactory resourceFactory;
        private readonly ResourceTypeLookup typeLookup;

        public WriteCacheFilter(ICacheResolver cacheResolver, IResourceFactory resourceFactory)
            : base(cacheResolver)
        {
            if (resourceFactory == null)
            {
                throw new ArgumentNullException(nameof(resourceFactory));
            }

            this.resourceFactory = resourceFactory;
            this.typeLookup = new ResourceTypeLookup();
        }

        public override async Task<IResourceDataResult> FilterAsync(IResourceDataRequest request, IAsynchronousFilterChain chain, ILogger logger, CancellationToken cancellationToken)
        {
            bool cacheEnabled = this.cacheResolver.IsAsynchronousSupported;
            if (!cacheEnabled)
            {
                return await chain.FilterAsync(request, logger, cancellationToken).ConfigureAwait(false);
            }

            bool isDelete = request.Action == ResourceAction.Delete;
            bool isCustomDataPropertyRequest = request.Uri.ResourcePath.ToString().Contains("/customData/");

            if (isCustomDataPropertyRequest && isDelete)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is a custom data property delete, deleting cached property name if exists", "WriteCacheFilter.FilterAsync");
                await this.UncacheCustomDataPropertyAsync(request.Uri.ResourcePath, logger, cancellationToken).ConfigureAwait(false);
            }
            else if (isDelete)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is a resource deletion, purging from cache if exists", "WriteCacheFilter.FilterAsync");
                var cacheKey = this.GetCacheKey(request);
                await this.UncacheAsync(request.Type, cacheKey, cancellationToken).ConfigureAwait(false);
            }

            var result = await chain.FilterAsync(request, logger, cancellationToken).ConfigureAwait(false);

            bool isEmailVerificationResponse = result.Type == typeof(IEmailVerificationToken);
            if (isEmailVerificationResponse)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is an email verification request, purging account from cache if exists", "WriteCacheFilter.FilterAsync");
                await this.UncacheAccountOnEmailVerificationAsync(result, cancellationToken).ConfigureAwait(false);
            }

            bool possibleCustomDataUpdate = (request.Action == ResourceAction.Create || request.Action == ResourceAction.Update) &&
                AbstractExtendableInstanceResource.IsExtendable(request.Type);
            if (possibleCustomDataUpdate)
            {
                await this.CacheNestedCustomDataUpdatesAsync(request, result, logger, cancellationToken).ConfigureAwait(false);
            }

            if (IsCacheable(request, result))
            {
                logger.Trace($"Caching request {request.Action} {request.Uri}", "WriteCacheFilter.FilterAsync");
                await this.CacheAsync(result.Type, result.Body, logger, cancellationToken).ConfigureAwait(false);
            }

            return result;
        }

        private async Task CacheAsync(Type resourceType, Map data, ILogger logger, CancellationToken cancellationToken)
        {
            string href = data[AbstractResource.HrefPropertyName].ToString();

            var cacheData = new Dictionary<string, object>();

            bool isCustomData = resourceType == typeof(ICustomData);
            if (isCustomData)
            {
                logger.Trace($"Response {href} is a custom data resource, caching directly", "WriteCacheFilter.CacheAsync");

                var cache = this.GetAsyncCache(resourceType);
                await cache.PutAsync(this.GetCacheKey(href), data, cancellationToken).ConfigureAwait(false);
                return; // simple! return early
            }

            bool isCacheable = true;

            foreach (var item in data)
            {
                string key = item.Key;
                object value = item.Value;

                // TODO DefaultModelMap edge case
                // TODO ApiEncryptionMetadata edge case
                var asNestedResource = value as ExpandedProperty;
                var asNestedMapArray = value as IEnumerable<Map>;
                var asNestedScalarArray = value as IEnumerable<object>;

                if (asNestedResource != null && IsResource(asNestedResource.Data))
                {
                    logger.Trace($"Attribute {key} on response {href} is an expanded resource, caching recursively", "WriteCacheFilter.CacheAsync");

                    var nestedType = this.typeLookup.GetInterfaceByPropertyName(key);
                    if (nestedType == null)
                    {
                        logger.Warn($"Cannot cache nested item. Item type for '{key}' unknown. '{href}' will not be cached.");
                        isCacheable = false; // gracefully disable caching
                        break;
                    }

                    await this.CacheAsync(nestedType, asNestedResource.Data, logger, cancellationToken).ConfigureAwait(false);
                    value = ToCanonicalReference(key, asNestedResource.Data);
                }
                else if (asNestedMapArray != null)
                {
                    logger.Trace($"Attribute {key} on response {href} is an array, caching items recursively", "WriteCacheFilter.CacheAsync");

                    var nestedType = this.typeLookup.GetInnerCollectionInterface(resourceType);
                    if (nestedType == null)
                    {
                        logger.Warn($"Can not cache array '{key}'. Item type for '{resourceType.Name}' unknown. '{href}' will not be cached.");
                        isCacheable = false; // gracefully disable caching
                        break;
                    }

                    // This is a CollectionResponsePage<T>.Items property.
                    // Recursively cache nested resources and create a new collection that only has references
                    var canonicalList = new List<object>();
                    foreach (var element in asNestedMapArray)
                    {
                        object canonicalElement = element;
                        var resourceElement = canonicalElement as Map;
                        if (resourceElement != null)
                        {
                            if (IsResource(resourceElement))
                            {
                                await this.CacheAsync(nestedType, resourceElement, logger, cancellationToken).ConfigureAwait(false);
                                canonicalElement = ToCanonicalReference(key, resourceElement);
                            }
                        }

                        canonicalList.Add(canonicalElement);
                    }

                    value = canonicalList;
                }
                else if (asNestedScalarArray != null)
                {
                    var nestedType = this.typeLookup.GetInterfaceByPropertyName(key);
                    if (nestedType == null)
                    {
                        logger.Warn($"Can not cache array '{key}'. Item type for '{resourceType.Name}' unknown. '{href}' will not be cached.");
                        isCacheable = false; // gracefully disable caching
                        break;
                    }

                    // This is an array of primitives, such as string[].
                    // (Not currently supported. Will be supported in the future.)
                    logger.Warn($"Can not cache array '{key}'. Array caching is currently unsupported. '{href}' will not be cached.");
                    isCacheable = false; // gracefully disable caching
                    break;
                }

                if (!IsSensitive(key))
                {
                    cacheData.Add(key, value);
                }
            }

            if (isCacheable && !this.typeLookup.IsCollectionResponse(resourceType))
            {
                logger.Trace($"Caching {href} as type {resourceType.Name}", "WriteCacheFilter.CacheAsync");

                var cache = this.GetAsyncCache(resourceType);
                if (cache == null)
                {
                    return;
                }

                var cacheKey = this.GetCacheKey(href);
                await cache.PutAsync(cacheKey, cacheData, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task CacheNestedCustomDataUpdatesAsync(IResourceDataRequest request, IResourceDataResult result, ILogger logger, CancellationToken cancellationToken)
        {
            object customDataObj = null;
            Map customData = null;

            if (!request.Properties.TryGetValue(AbstractExtendableInstanceResource.CustomDataPropertyName, out customDataObj))
            {
                return;
            }

            customData = customDataObj as Map;
            if (customData.IsNullOrEmpty())
            {
                return;
            }

            bool creating = request.Action == ResourceAction.Create;

            var parentHref = request.Uri.ResourcePath.ToString();
            if (creating && !result.Body.TryGetValueAsString(AbstractResource.HrefPropertyName, out parentHref))
            {
                return;
            }

            var customDataHref = parentHref + "/customData";

            var dataToCache = await this.GetCachedValueAsync(typeof(ICustomData), customDataHref, cancellationToken).ConfigureAwait(false);
            if (!creating && dataToCache == null)
            {
                logger.Trace($"Request {request.Uri} has nested custom data updates, but no authoritative cached custom data exists; aborting", "WriteCacheFilter.CacheNestedCustomDataUpdatesAsync");
                return;
            }

            logger.Trace($"Request {request.Uri} has nested custom data updates, updating cached custom data", "WriteCacheFilter.CacheNestedCustomDataUpdatesAsync");

            if (dataToCache.IsNullOrEmpty())
            {
                dataToCache = new Dictionary<string, object>();
            }

            foreach (var updatedItem in customData)
            {
                dataToCache[updatedItem.Key] = updatedItem.Value;
            }

            // Ensure the href property exists
            dataToCache[AbstractResource.HrefPropertyName] = customDataHref;

            await this.CacheAsync(typeof(ICustomData), dataToCache, logger, cancellationToken).ConfigureAwait(false);
        }

        private async Task UncacheAsync(Type resourceType, string cacheKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (resourceType == null)
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            var cache = this.GetAsyncCache(resourceType);
            await cache.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }

        private async Task UncacheCustomDataPropertyAsync(Uri resourceUri, ILogger logger, CancellationToken cancellationToken)
        {
            var href = resourceUri.ToString();
            var propertyName = href.Substring(href.LastIndexOf('/') + 1);
            href = href.Substring(0, href.LastIndexOf('/'));

            if (string.IsNullOrEmpty(propertyName) ||
                string.IsNullOrEmpty(href))
            {
                throw new ApplicationException("Could not update cache for removed custom data entry.");
            }

            var cache = this.GetAsyncCache(typeof(ICustomData));
            var cacheKey = this.GetCacheKey(href);

            var existingData = await cache.GetAsync(cacheKey, cancellationToken).ConfigureAwait(false);
            if (existingData.IsNullOrEmpty())
            {
                return;
            }

            logger.Trace($"Deleting custom data property '{propertyName}' from resource {href}", "WriteCacheFilter.UncacheCustomDataPropertyAsync");

            existingData.Remove(propertyName);
            await cache.PutAsync(cacheKey, existingData, cancellationToken).ConfigureAwait(false);
        }

        private async Task UncacheAccountOnEmailVerificationAsync(IResourceDataResult result, CancellationToken cancellationToken)
        {
            object accountHrefRaw = null;
            string accountHref = null;
            if (!result.Body.TryGetValue(AbstractResource.HrefPropertyName, out accountHrefRaw))
            {
                return;
            }

            accountHref = accountHrefRaw.ToString();
            if (string.IsNullOrEmpty(accountHref))
            {
                return;
            }

            var cache = this.GetAsyncCache(typeof(IAccount));
            await cache.RemoveAsync(this.GetCacheKey(accountHref), cancellationToken).ConfigureAwait(false);
        }

        private static bool IsCacheable(IResourceDataRequest request, IResourceDataResult result)
        {
            bool hasData = !result.Body.IsNullOrEmpty();

            return

                // Must be a resource
                IsResource(result?.Body) &&

                // Don't cache password reset or email verification requests
                result.Type != typeof(IPasswordResetToken) &&
                result.Type != typeof(IEmailVerificationToken) &&
                result.Type != typeof(IEmailVerificationRequest) &&

                // Don't cache login attempts
                result.Type != typeof(IAuthenticationResult) &&

                // ProviderAccountResults look like IAccounts but should not be cached either
                result.Type != typeof(IProviderAccountResult);
        }

        private static bool IsResource(Map data)
        {
            if (data == null)
            {
                return false;
            }

            bool hasItems = data.Count > 1;
            bool hasHref = data.ContainsKey(AbstractResource.HrefPropertyName);

            return hasHref && hasItems;
        }

        private static object ToCanonicalReference(string propertyName, Map resourceData)
        {
            if (IsResource(resourceData))
            {
                return new LinkProperty(resourceData[AbstractResource.HrefPropertyName].ToString());
            }

            throw new ApplicationException($"Could not convert embedded resource '{propertyName}' to a canonical reference.");
        }

        private static bool IsSensitive(string propertyName)
        {
            return

                // Don't cache passwords ever
                DefaultAccount.PasswordPropertyName.Equals(propertyName);
        }
    }
}
