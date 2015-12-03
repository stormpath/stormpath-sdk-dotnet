// <copyright file="DefaultNonceStore.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.IdSite;
using Stormpath.SDK.Impl.Cache;

namespace Stormpath.SDK.Impl.IdSite
{
    internal sealed class DefaultNonceStore : ISynchronousNonceStore, IAsynchronousNonceStore
    {
        private ICacheResolver cacheResolver;

        public DefaultNonceStore(ICacheResolver cacheResolver)
        {
            if (cacheResolver == null)
            {
                throw new ArgumentNullException(nameof(cacheResolver));
            }

            this.cacheResolver = cacheResolver;
        }

        bool INonceStore.IsAsynchronousSupported => this.cacheResolver.IsAsynchronousSupported;

        bool INonceStore.IsSynchronousSupported => this.cacheResolver.IsSynchronousSupported;

        bool ISynchronousNonceStore.ContainsNonce(string nonce)
        {
            if (string.IsNullOrEmpty(nonce))
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            var cache = this.cacheResolver.GetSyncCache(typeof(INonce));
            var value = cache.Get(nonce);

            return value != null;
        }

        async Task<bool> IAsynchronousNonceStore.ContainsNonceAsync(string nonce, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(nonce))
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            var cache = this.cacheResolver.GetAsyncCache(typeof(INonce));
            var value = await cache.GetAsync(nonce).ConfigureAwait(false);

            return value != null;
        }

        void ISynchronousNonceStore.PutNonce(string nonce)
        {
            if (string.IsNullOrEmpty(nonce))
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            var cache = this.cacheResolver.GetSyncCache(typeof(INonce));

            var nonceObject = new DefaultNonce(nonce);
            cache.Put(nonce, nonceObject.GetProperties());
        }

        async Task IAsynchronousNonceStore.PutNonceAsync(string nonce, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(nonce))
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            var cache = this.cacheResolver.GetAsyncCache(typeof(INonce));

            var nonceObject = new DefaultNonce(nonce);
            await cache.PutAsync(nonce, nonceObject.GetProperties(), cancellationToken).ConfigureAwait(false);
        }
    }
}
