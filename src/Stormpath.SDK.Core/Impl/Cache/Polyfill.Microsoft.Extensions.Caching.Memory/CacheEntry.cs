// <copyright file="MemoryCache.cs" company="Stormpath, Inc.">
// Copyright (c) 2016 Stormpath, Inc.
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
//
// Contains code modified from aspnet/Caching. Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

#if NET45 || NET451
using System;
using System.Collections.Generic;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    internal class CacheEntry
    {
        private readonly Action<CacheEntry> _notifyCacheOfExpiration;

        private readonly DateTimeOffset? _absoluteExpiration;

        internal readonly object _lock = new object();

        internal CacheEntry(
            object key,
            object value,
            DateTimeOffset utcNow,
            DateTimeOffset? absoluteExpiration,
            MemoryCacheEntryOptions options,
            Action<CacheEntry> notifyCacheOfExpiration)
        {
            Key = key;
            Value = value;
            LastAccessed = utcNow;
            Options = options;
            _notifyCacheOfExpiration = notifyCacheOfExpiration;
            _absoluteExpiration = absoluteExpiration;
        }

        internal MemoryCacheEntryOptions Options { get; private set; }

        internal object Key { get; private set; }

        internal object Value { get; private set; }

        private bool IsExpired { get; set; }

        internal EvictionReason EvictionReason { get; private set; }

        internal IList<IDisposable> ExpirationTokenRegistrations { get; set; }

        internal DateTimeOffset LastAccessed { get; set; }

        internal bool CheckExpired(DateTimeOffset now)
        {
            return IsExpired || CheckForExpiredTime(now);
        }

        internal void SetExpired(EvictionReason reason)
        {
            IsExpired = true;
            if (EvictionReason == EvictionReason.None)
            {
                EvictionReason = reason;
            }
            DetachTokens();
        }

        private bool CheckForExpiredTime(DateTimeOffset now)
        {
            if (_absoluteExpiration.HasValue && _absoluteExpiration.Value <= now)
            {
                SetExpired(EvictionReason.Expired);
                return true;
            }

            if (Options.SlidingExpiration.HasValue
                && (now - LastAccessed) >= Options.SlidingExpiration)
            {
                SetExpired(EvictionReason.Expired);
                return true;
            }

            return false;
        }

        private void DetachTokens()
        {
            lock (_lock)
            {
                var registrations = ExpirationTokenRegistrations;
                if (registrations != null)
                {
                    ExpirationTokenRegistrations = null;
                    for (int i = 0; i < registrations.Count; i++)
                    {
                        var registration = registrations[i];
                        registration.Dispose();
                    }
                }
            }
        }
    }
}
#endif