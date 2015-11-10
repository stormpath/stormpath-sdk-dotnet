// <copyright file="AbstractCacheProvider.cs" company="Stormpath, Inc.">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stormpath.SDK.Cache
{
    public abstract class AbstractCacheProvider : IAsynchronousCacheProvider, ISynchronousCacheProvider
    {
        private readonly bool syncSupported;
        private readonly bool asyncSupported;

        private readonly ConcurrentDictionary<string, ICacheConfiguration> cacheConfigs;
        private readonly ConcurrentDictionary<string, object> caches;

        private TimeSpan? defaultTimeToLive;
        private TimeSpan? defaultTimeToIdle;

        private bool disposed = false;

        public AbstractCacheProvider(bool syncSupported, bool asyncSupported)
        {
            this.syncSupported = syncSupported;
            this.asyncSupported = asyncSupported;

            this.cacheConfigs = new ConcurrentDictionary<string, ICacheConfiguration>();
            this.caches = new ConcurrentDictionary<string, object>();
        }

        public TimeSpan? DefaultTimeToLive => this.defaultTimeToLive;

        public TimeSpan? DefaultTimeToIdle => this.defaultTimeToIdle;

        bool ICacheProvider.IsAsynchronousSupported => this.syncSupported;

        bool ICacheProvider.IsSynchronousSupported => this.asyncSupported;

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ApplicationException("This cache provider has been disposed.");
        }

        protected abstract ISynchronousCache<K, V> CreateSyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti);

        protected abstract IAsynchronousCache<K, V> CreateAsyncCache<K, V>(string name, TimeSpan? ttl, TimeSpan? tti);

        ISynchronousCache<K, V> ISynchronousCacheProvider.GetSyncCache<K, V>(string name)
        {
            this.ThrowIfDisposed();

            if (!this.syncSupported)
                throw new ApplicationException("This cache provider does not support a synchronous execution path.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Func<string, TimeSpan?, TimeSpan?, object> callback =
                (n, ttl, tti) => this.CreateSyncCache<K, V>(n, ttl, tti);

            return this.caches.GetOrAdd(
                name,
                region => this.CreateCache(region, callback)) as ISynchronousCache<K, V>;
        }

        IAsynchronousCache<K, V> IAsynchronousCacheProvider.GetAsyncCache<K, V>(string name)
        {
            this.ThrowIfDisposed();

            if (!this.asyncSupported)
                throw new ApplicationException("This cache provider does not support an asynchronous execution path.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Func<string, TimeSpan?, TimeSpan?, object> callback =
                (n, ttl, tti) => this.CreateAsyncCache<K, V>(n, ttl, tti);

            return this.caches.GetOrAdd(
                name,
                region => this.CreateCache(region, callback)) as IAsynchronousCache<K, V>;
        }

        private object CreateCache(string name, Func<string, TimeSpan?, TimeSpan?, object> delegated)
        {
            this.ThrowIfDisposed();

            var ttl = this.defaultTimeToLive;
            var tti = this.defaultTimeToIdle;

            ICacheConfiguration config = null;
            if (this.cacheConfigs.TryGetValue(name, out config))
            {
                if (config.TimeToLive.HasValue)
                    ttl = config.TimeToLive;

                if (config.TimeToIdle.HasValue)
                    tti = config.TimeToIdle;
            }

            return delegated(name, ttl, tti);
        }

        public void SetDefaultTimeToLive(TimeSpan defaultTimeToLive)
        {
            this.ThrowIfDisposed();

            if (defaultTimeToLive.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToLive));

            this.defaultTimeToLive = defaultTimeToLive;
        }

        public void SetDefaultTimeToIdle(TimeSpan defaultTimeToIdle)
        {
            this.ThrowIfDisposed();

            if (defaultTimeToIdle.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToIdle));

            this.defaultTimeToIdle = defaultTimeToIdle;
        }

        public void SetCacheConfigurations(ICollection<ICacheConfiguration> configs)
        {
            this.ThrowIfDisposed();

            if (configs == null)
                throw new ArgumentNullException(nameof(configs));

            this.cacheConfigs.Clear();
            foreach (var config in configs)
            {
                if (!this.cacheConfigs.TryAdd(config.Name, config))
                    throw new ApplicationException("Unable to load cache configuration.");
            }
        }

        public override string ToString()
        {
            this.ThrowIfDisposed();

            var caches = this.caches.Values;
            var builder = new StringBuilder();

            var indent = "    ";
            builder.AppendLine("{");
            builder.AppendLine($@"{indent}""name"": {this.GetType().Name},");
            builder.AppendLine($@"{indent}""cacheCount"": {caches.Count},");
            builder.AppendLine($@"{indent}""defaultTimeToLive"": ""{PrettyDuration(this.defaultTimeToLive)}"",");
            builder.AppendLine($@"{indent}""defaultTimeToIdle"": ""{PrettyDuration(this.defaultTimeToIdle)}"",");
            builder.Append($@"{indent}""caches"": [");

            if (caches.Any())
            {
                builder.Append(Environment.NewLine);

                builder.Append(string.Join(
                    "," + Environment.NewLine,
                    caches.Select(x => $"{indent}{indent}{x.ToString()}")));

                builder.Append(Environment.NewLine);
                builder.Append(indent);
            }

            builder.AppendLine($"]");
            builder.Append("}");

            return builder.ToString();
        }

        private static string PrettyDuration(TimeSpan? duration)
        {
            return duration.HasValue
                ? duration.Value.ToString()
                : "indefinite";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (disposing)
                {
                    foreach (var cacheKey in this.caches.Keys)
                    {
                        object cache = null;
                        if (this.caches.TryRemove(cacheKey, out cache))
                            (cache as IDisposable).Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);
        }
    }
}
