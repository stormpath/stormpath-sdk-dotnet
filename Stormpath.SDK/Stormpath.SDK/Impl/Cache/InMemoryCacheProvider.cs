// <copyright file="InMemoryCacheProvider.cs" company="Stormpath, Inc.">
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
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    internal class InMemoryCacheProvider : ISynchronousCacheProvider, IAsynchronousCacheProvider
    {
        private readonly ConcurrentDictionary<string, ICacheConfiguration> cacheConfigs;
        private readonly ConcurrentDictionary<string, object> caches;

        private TimeSpan? defaultTimeToLive;
        private TimeSpan? defaultTimeToIdle;

        public InMemoryCacheProvider()
        {
            this.cacheConfigs = new ConcurrentDictionary<string, ICacheConfiguration>();
            this.caches = new ConcurrentDictionary<string, object>();
        }

        ISynchronousCache<K, V> ISynchronousCacheProvider.GetCache<K, V>(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var cache = this.caches.GetOrAdd(name, n => this.CreateCache<K, V>(n)) as ISynchronousCache<K, V>;
            return cache;
        }

        Task<IAsynchronousCache<K, V>> IAsynchronousCacheProvider.GetCacheAsync<K, V>(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var cache = this.caches.GetOrAdd(name, n => this.CreateCache<K, V>(n)) as IAsynchronousCache<K, V>;
            return Task.FromResult(cache);
        }

        private ISynchronousCacheProvider AsSyncInterface => this;

        public TimeSpan? DefaultTimeToLive => this.defaultTimeToLive;

        public TimeSpan? DefaultTimeToIdle => this.defaultTimeToIdle;

        bool ICacheProvider.IsAsynchronousSupported => false; // Currently unsupported

        bool ICacheProvider.IsSynchronousSupported => false; // Currently unsupported

        private object CreateCache<K, V>(string name)
        {
            var ttl = this.defaultTimeToLive;
            var tti = this.defaultTimeToIdle;

            ICacheConfiguration config = null;
            if (!this.cacheConfigs.TryGetValue(name, out config))
            {
                if (config.TimeToLive.HasValue)
                    ttl = config.TimeToLive;

                if (config.TimeToIdle.HasValue)
                    tti = config.TimeToIdle;
            }

            return new InMemoryCache<K, V>(name, ttl, tti);
        }

        public void SetDefaultTimeToLive(TimeSpan defaultTimeToLive)
        {
            if (defaultTimeToLive.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToLive));

            this.defaultTimeToLive = defaultTimeToLive;
        }

        public void SetDefaultTimeToIdle(TimeSpan defaultTimeToIdle)
        {
            if (defaultTimeToIdle.TotalMilliseconds <= 0)
                throw new ArgumentOutOfRangeException("TTL duration must be greater than zero.", nameof(defaultTimeToIdle));

            this.defaultTimeToIdle = defaultTimeToIdle;
        }

        public void SetCacheConfigurations(ICollection<ICacheConfiguration> configs)
        {
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
            var caches = this.caches.Values;
            var builder = new StringBuilder();

            var indent = "    ";
            builder.AppendLine("{");
            builder.AppendLine($@"{indent}""cacheCount"": ""{caches.Count}"",");
            builder.AppendLine($@"{indent}""defaultTimeToLive"": ""{PrettyDuration(this.defaultTimeToLive)}"",");
            builder.AppendLine($@"{indent}""defaultTimeToIdle"": ""{PrettyDuration(this.defaultTimeToIdle)}"",");
            builder.AppendLine($@"{indent}""caches"": [");

            if (caches.Any())
            {
                builder.AppendLine(string.Empty);

                var added = 0;
                foreach (var cache in caches)
                {
                    if (added++ > 0)
                        builder.AppendLine(",");
                    builder.AppendLine($"{indent}{indent}{cache.ToString()}");
                }

                builder.Append($"{indent}");
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
    }
}
