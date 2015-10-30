// <copyright file="NullCacheProvider.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class NullCacheProvider : ISynchronousCacheProvider, IAsynchronousCacheProvider
    {
        private bool disposed = false;

        bool ICacheProvider.IsAsynchronousSupported => false;

        bool ICacheProvider.IsSynchronousSupported => false;

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ApplicationException("This cache provider has been disposed.");
        }

        ISynchronousCache<K, V> ISynchronousCacheProvider.GetCache<K, V>(string name)
        {
            throw new NotImplementedException();
        }

        Task<IAsynchronousCache<K, V>> IAsynchronousCacheProvider.GetCacheAsync<K, V>(string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return @"{ ""name"": ""NullCacheProvider"" }";
        }

        public void Dispose()
        {
            this.disposed = true;
        }
    }
}
