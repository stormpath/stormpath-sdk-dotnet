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
    /// <summary>
    /// Base class for cache provider implementations.
    /// </summary>
    public abstract class AbstractCacheProvider : IAsynchronousCacheProvider, ISynchronousCacheProvider
    {
        private readonly bool syncSupported;
        private readonly bool asyncSupported;

        private readonly ConcurrentDictionary<string, ICacheConfiguration> cacheConfigs;
        private readonly ConcurrentDictionary<string, object> caches;

        private TimeSpan? defaultTimeToLive;
        private TimeSpan? defaultTimeToIdle;

        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractCacheProvider"/> class.
        /// </summary>
        /// <param name="syncSupported">Determines whether a synchronous execution path is supported.</param>
        /// <param name="asyncSupported">Determines whether an asynchronous execution path is supported.</param>
        public AbstractCacheProvider(bool syncSupported, bool asyncSupported)
        {
            this.syncSupported = syncSupported;
            this.asyncSupported = asyncSupported;

            this.cacheConfigs = new ConcurrentDictionary<string, ICacheConfiguration>();
            this.caches = new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Gets the default time to live (TTL) of new caches.
        /// </summary>
        /// <value>The default time to live of new caches.</value>
        public TimeSpan? DefaultTimeToLive => this.defaultTimeToLive;

        /// <summary>
        /// Gets the default time to idle (TTI) of new caches.
        /// </summary>
        /// <value>The default time to idle of new caches.</value>
        public TimeSpan? DefaultTimeToIdle => this.defaultTimeToIdle;

        /// <inheritdoc/>
        bool ICacheProvider.IsAsynchronousSupported => this.syncSupported;

        /// <inheritdoc/>
        bool ICacheProvider.IsSynchronousSupported => this.asyncSupported;

        /// <summary>
        /// Throws an error if the cache provider has been disposed.
        /// </summary>
        /// <exception cref="ApplicationException">The cache provider has been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ApplicationException("This cache provider has been disposed.");
        }

        /// <summary>
        /// Creates a new synchronous cache.
        /// </summary>
        /// <param name="name">The cache name.</param>
        /// <param name="ttl">The time to live value, or <c>null</c> to use <see cref="DefaultTimeToLive"/>.</param>
        /// <param name="tti">The time to idle value, or <c>null</c> to use <see cref="DefaultTimeToIdle"/>.</param>
        /// <returns>A new <see cref="ISynchronousCache"/> instance.</returns>
        protected abstract ISynchronousCache CreateSyncCache(string name, TimeSpan? ttl, TimeSpan? tti);

        /// <summary>
        /// Creates a new asynchronous cache.
        /// </summary>
        /// <param name="name">The cache name.</param>
        /// <param name="ttl">The time to live value, or <c>null</c> to use <see cref="DefaultTimeToLive"/>.</param>
        /// <param name="tti">The time to idle value, or <c>null</c> to use <see cref="DefaultTimeToIdle"/>.</param>
        /// <returns>A new <see cref="IAsynchronousCache"/> instance.</returns>
        protected abstract IAsynchronousCache CreateAsyncCache(string name, TimeSpan? ttl, TimeSpan? tti);

        /// <inheritdoc/>
        ISynchronousCache ISynchronousCacheProvider.GetSyncCache(string name)
        {
            this.ThrowIfDisposed();

            if (!this.syncSupported)
                throw new ApplicationException("This cache provider does not support a synchronous execution path.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Func<string, TimeSpan?, TimeSpan?, object> callback =
                (n, ttl, tti) => this.CreateSyncCache(n, ttl, tti);

            return this.caches.GetOrAdd(
                name,
                region => this.CreateCache(region, callback)) as ISynchronousCache;
        }

        /// <inheritdoc/>
        IAsynchronousCache IAsynchronousCacheProvider.GetAsyncCache(string name)
        {
            this.ThrowIfDisposed();

            if (!this.asyncSupported)
                throw new ApplicationException("This cache provider does not support an asynchronous execution path.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Func<string, TimeSpan?, TimeSpan?, object> callback =
                (n, ttl, tti) => this.CreateAsyncCache(n, ttl, tti);

            return this.caches.GetOrAdd(
                name,
                region => this.CreateCache(region, callback)) as IAsynchronousCache;
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

        /// <summary>
        /// Sets the default time to live (TTL) for new caches.
        /// </summary>
        /// <param name="defaultTimeToLive">The default time to live.</param>
        /// <exception cref="ApplicationException">The cache provider has been disposed.</exception>
        public void SetDefaultTimeToLive(TimeSpan defaultTimeToLive)
        {
            this.ThrowIfDisposed();

            if (defaultTimeToLive.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToLive));

            this.defaultTimeToLive = defaultTimeToLive;
        }

        /// <summary>
        /// Sets the default time to live (TTI) for new caches.
        /// </summary>
        /// <param name="defaultTimeToIdle">The default time to idle.</param>
        /// <exception cref="ApplicationException">The cache provider has been disposed.</exception>
        public void SetDefaultTimeToIdle(TimeSpan defaultTimeToIdle)
        {
            this.ThrowIfDisposed();

            if (defaultTimeToIdle.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToIdle));

            this.defaultTimeToIdle = defaultTimeToIdle;
        }

        /// <summary>
        /// Sets per-cache configurations.
        /// </summary>
        /// <param name="configs">The cache configurations to add.</param>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this object is currently disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                if (disposing)
                {
                    foreach (KeyValuePair<string, object> entry in this.caches)
                    {
                        (entry.Value as IDisposable)?.Dispose();
                    }

                    this.caches.Clear();
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
