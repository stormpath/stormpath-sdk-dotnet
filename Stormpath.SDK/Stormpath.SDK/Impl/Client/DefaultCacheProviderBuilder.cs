// <copyright file="DefaultCacheProviderBuilder.cs" company="Stormpath, Inc.">
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

using Stormpath.SDK.Cache;
using Stormpath.SDK.Impl.Cache;

namespace Stormpath.SDK.Impl.Client
{
    internal sealed class DefaultCacheProviderBuilder : ICacheProviderBuilder
    {
        private bool useCache;
        private ICacheProvider cacheProvider;

        public DefaultCacheProviderBuilder()
        {
            this.useCache = false;
            this.cacheProvider = null;
        }

        internal bool UseCache => this.useCache;

        internal ICacheProvider Provider => this.cacheProvider;

        ICacheProviderBuilder ICacheProviderBuilder.UseCache(bool useCache)
        {
            this.useCache = useCache;
            return this;
        }

        ICacheProviderBuilder ICacheProviderBuilder.UseProvider(ICacheProvider cacheProvider)
        {
            this.cacheProvider = cacheProvider;
            return this;
        }

        ICacheProvider ICacheProviderBuilder.Build()
        {
            if (!this.useCache)
                return new NullCacheProvider();

            if (this.cacheProvider == null)
                return new InMemoryCacheProvider();

            return this.cacheProvider;
        }
    }
}
