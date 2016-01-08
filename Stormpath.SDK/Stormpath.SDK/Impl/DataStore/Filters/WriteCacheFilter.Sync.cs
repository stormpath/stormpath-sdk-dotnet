// <copyright file="WriteCacheFilter.Sync.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Account;
using Stormpath.SDK.CustomData;
using Stormpath.SDK.Impl.Extensions;
using Stormpath.SDK.Impl.Resource;
using Stormpath.SDK.Logging;
using Map = System.Collections.Generic.IDictionary<string, object>;

namespace Stormpath.SDK.Impl.DataStore.Filters
{
    internal sealed partial class WriteCacheFilter
    {
        public override IResourceDataResult Filter(IResourceDataRequest request, ISynchronousFilterChain chain, ILogger logger)
        {
            bool cacheEnabled = this.cacheResolver.IsSynchronousSupported;
            if (!cacheEnabled)
            {
                return chain.Filter(request, logger);
            }

            bool isDelete = request.Action == ResourceAction.Delete;
            bool isCustomDataPropertyRequest = request.Uri.ResourcePath.ToString().Contains("/customData/");

            if (isCustomDataPropertyRequest && isDelete)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is a custom data property delete, deleting cached property name if exists", "WriteCacheFilter.FilterAsync");
                this.UncacheCustomDataProperty(request.Uri.ResourcePath, logger);
            }
            else if (isDelete)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is a resource deletion, purging from cache if exists", "WriteCacheFilter.Filter");
                var cacheKey = this.GetCacheKey(request);
                this.Uncache(request.Type, cacheKey);
            }

            var result = chain.Filter(request, logger);

            bool isEmailVerificationResponse = result.Type == typeof(IEmailVerificationToken);
            if (isEmailVerificationResponse)
            {
                logger.Trace($"Request {request.Action} {request.Uri} is an email verification request, purging account from cache if exists", "WriteCacheFilter.Filter");
                this.UncacheAccountOnEmailVerification(result);
            }

            bool possibleCustomDataUpdate = (request.Action == ResourceAction.Create || request.Action == ResourceAction.Update) &&
                AbstractExtendableInstanceResource.IsExtendable(request.Type);
            if (possibleCustomDataUpdate)
            {
                this.CacheNestedCustomDataUpdates(request, result, logger);
            }

            if (IsCacheable(request, result))
            {
                logger.Trace($"Caching request {request.Action} {request.Uri}", "WriteCacheFilter.Filter");
                this.Cache(result.Type, result.Body, logger);
            }

            return result;
        }

        private void Cache(Type resourceType, Map data, ILogger logger)
        {
            string href = data[AbstractResource.HrefPropertyName].ToString();

            var cacheData = new Dictionary<string, object>();

            bool isCustomData = resourceType == typeof(ICustomData);
            if (isCustomData)
            {
                logger.Trace($"Response {href} is a custom data resource, caching directly", "WriteCacheFilter.Cache");

                var cache = this.GetSyncCache(resourceType);
                cache.Put(this.GetCacheKey(href), data);
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
                    logger.Trace($"Attribute {key} on response {href} is an expanded resource, caching recursively", "WriteCacheFilter.Cache");

                    var nestedType = this.typeLookup.GetInterfaceByPropertyName(item.Key);
                    if (nestedType == null)
                    {
                        logger.Warn($"Cannot cache nested item. Item type for '{key}' unknown. '{href}' will not be cached.");
                        isCacheable = false; // gracefully disable caching
                        break;
                    }

                    this.Cache(nestedType, asNestedResource.Data, logger);
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
                                this.Cache(nestedType, resourceElement, logger);
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
                logger.Trace($"Caching {href} as type {resourceType.Name}", "WriteCacheFilter.Cache");

                var cache = this.GetSyncCache(resourceType);
                if (cache == null)
                {
                    return;
                }

                var cacheKey = this.GetCacheKey(href);
                cache.Put(cacheKey, cacheData);
            }
        }

        private void CacheNestedCustomDataUpdates(IResourceDataRequest request, IResourceDataResult result, ILogger logger)
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

            var dataToCache = this.GetCachedValue(typeof(ICustomData), customDataHref);
            if (!creating && dataToCache == null)
            {
                logger.Trace($"Request {request.Uri} has nested custom data updates, but no authoritative cached custom data exists; aborting", "WriteCacheFilter.CacheNestedCustomDataUpdates");
                return;
            }

            logger.Trace($"Request {request.Uri} has nested custom data updates, updating cached custom data", "WriteCacheFilter.CacheNestedCustomDataUpdates");

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

            this.Cache(typeof(ICustomData), dataToCache, logger);
        }

        private void Uncache(Type resourceType, string cacheKey)
        {
            if (string.IsNullOrEmpty(cacheKey))
            {
                throw new ArgumentNullException(nameof(cacheKey));
            }

            if (resourceType == null)
            {
                throw new ArgumentNullException(nameof(resourceType));
            }

            var cache = this.GetSyncCache(resourceType);
            cache.Remove(cacheKey);
        }

        private void UncacheCustomDataProperty(Uri resourceUri, ILogger logger)
        {
            var href = resourceUri.ToString();
            var propertyName = href.Substring(href.LastIndexOf('/') + 1);
            href = href.Substring(0, href.LastIndexOf('/'));

            if (string.IsNullOrEmpty(propertyName) ||
                string.IsNullOrEmpty(href))
            {
                throw new ApplicationException("Could not update cache for removed custom data entry.");
            }

            var cache = this.GetSyncCache(typeof(ICustomData));
            var cacheKey = this.GetCacheKey(href);

            var existingData = cache.Get(cacheKey);
            if (existingData.IsNullOrEmpty())
            {
                return;
            }

            logger.Trace($"Deleting custom data property '{propertyName}' from resource {href}", "WriteCacheFilter.UncacheCustomDataProperty");

            existingData.Remove(propertyName);
            cache.Put(cacheKey, existingData);
        }

        private void UncacheAccountOnEmailVerification(IResourceDataResult result)
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

            var cache = this.GetSyncCache(typeof(IAccount));
            cache.Remove(this.GetCacheKey(accountHref));
        }
    }
}
