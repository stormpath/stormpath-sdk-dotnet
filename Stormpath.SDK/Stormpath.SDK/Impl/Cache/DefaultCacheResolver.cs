// <copyright file="DefaultCacheResolver.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Logging;

namespace Stormpath.SDK.Impl.Cache
{
    internal class DefaultCacheResolver : ICacheResolver
    {
        private readonly ILogger logger;
        private readonly ICacheProvider cacheProvider;
        private readonly ISynchronousCacheProvider syncCacheProvider;
        private readonly IAsynchronousCacheProvider asyncCacheProvider;

        public DefaultCacheResolver(ICacheProvider cacheProvider, ILogger logger)
        {
            if (cacheProvider == null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            this.cacheProvider = cacheProvider;
            this.syncCacheProvider = cacheProvider as ISynchronousCacheProvider;
            this.asyncCacheProvider = cacheProvider as IAsynchronousCacheProvider;
            this.logger = logger;
        }

        bool ICacheResolver.IsSynchronousSupported => this.cacheProvider.IsAsynchronousSupported;

        bool ICacheResolver.IsAsynchronousSupported => this.cacheProvider.IsSynchronousSupported;

        private string GetCacheRegionName(Type type)
        {
            var iface = new ResourceTypeLookup().GetInterface(type);

            if (iface == null)
            {
                throw new ApplicationException($"Could not locate a cache region for resource type {type.Name}. Resource type unknown.");
            }

            return iface.Name;
        }

        ISynchronousCache ICacheResolver.GetSyncCache(Type resourceType)
        {
            if (!this.cacheProvider.IsSynchronousSupported || this.syncCacheProvider == null)
            {
                return null;
            }

            var cacheRegionName = string.Empty;
            try
            {
                cacheRegionName = this.GetCacheRegionName(resourceType);
            }
            catch (Exception e)
            {
                this.logger.Warn($"Could not get synchronous cache for type {resourceType.Name}: {e.Message} (Source: {e.Source})", "DefaultCacheResolver.GetSyncCache");
            }

            return string.IsNullOrEmpty(cacheRegionName)
                ? null
                : this.syncCacheProvider.GetSyncCache(cacheRegionName);
        }

        IAsynchronousCache ICacheResolver.GetAsyncCache(Type resourceType)
        {
            if (!this.cacheProvider.IsAsynchronousSupported || this.asyncCacheProvider == null)
            {
                return null;
            }

            var cacheRegionName = string.Empty;
            try
            {
                cacheRegionName = this.GetCacheRegionName(resourceType);
            }
            catch (Exception e)
            {
                this.logger.Warn($"Could not get asynchronous cache for type {resourceType.Name}: {e.Message} (Source: {e.Source})", "DefaultCacheResolver.GetAsyncCache");
            }

            return string.IsNullOrEmpty(cacheRegionName)
                ? null
                : this.asyncCacheProvider.GetAsyncCache(cacheRegionName);
        }
    }
}
