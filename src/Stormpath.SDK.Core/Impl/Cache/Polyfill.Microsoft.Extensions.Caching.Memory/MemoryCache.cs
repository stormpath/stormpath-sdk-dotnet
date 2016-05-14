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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stormpath.SDK.Impl.Cache.Polyfill.Microsoft.Extensions.Caching.Memory
{
    internal class MemoryCache : IMemoryCache
    {
        private readonly Dictionary<object, CacheEntry> _entries;
        private readonly ReaderWriterLockSlim _entryLock;
        private bool _disposed;

        private readonly Action<CacheEntry> _entryExpirationNotification;

        private TimeSpan _expirationScanFrequency;
        private DateTimeOffset _lastExpirationScan;

        /// <summary>
        /// Creates a new MemoryCache instance.
        /// </summary>
        public MemoryCache(MemoryCacheOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _entries = new Dictionary<object, CacheEntry>();
            _entryLock = new ReaderWriterLockSlim();
            _entryExpirationNotification = EntryExpired;
            if (options.CompactOnMemoryPressure)
            {
                GcNotification.Register(DoMemoryPreassureCollection, state: null);
            }
            _expirationScanFrequency = options.ExpirationScanFrequency;
            _lastExpirationScan = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Cleans up the background collection events.
        /// </summary>
        ~MemoryCache()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the count of the current entries for diagnostic purposes.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        public IEntryLink CreateLinkingScope()
        {
            return EntryLinkHelpers.CreateLinkingScope();
        }

        public object Set(object key, object value, MemoryCacheEntryOptions cacheEntryOptions)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            CheckDisposed();
            CacheEntry priorEntry = null;
            var utcNow = DateTimeOffset.UtcNow;

            DateTimeOffset? absoluteExpiration = null;
            if (cacheEntryOptions.AbsoluteExpirationRelativeToNow.HasValue)
            {
                absoluteExpiration = utcNow + cacheEntryOptions.AbsoluteExpirationRelativeToNow;
            }
            else if (cacheEntryOptions.AbsoluteExpiration.HasValue)
            {
                if (cacheEntryOptions.AbsoluteExpiration <= utcNow)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(MemoryCacheEntryOptions.AbsoluteExpiration),
                        cacheEntryOptions.AbsoluteExpiration.Value,
                        "The absolute expiration value must be in the future.");
                }

                absoluteExpiration = cacheEntryOptions.AbsoluteExpiration;
            }

            var entry = new CacheEntry(
                key,
                value,
                utcNow,
                absoluteExpiration,
                cacheEntryOptions,
                _entryExpirationNotification);

            var link = EntryLinkHelpers.ContextLink;
            if (link != null)
            {
                // Copy expiration tokens and AbsoluteExpiration to the link.
                // We do this regardless of it gets cached because the tokens are associated with the value we'll return.
                if (absoluteExpiration.HasValue)
                {
                    link.SetAbsoluteExpiration(absoluteExpiration.Value);
                }
            }

            bool added = false;

            _entryLock.EnterWriteLock();
            try
            {
                if (_entries.TryGetValue(key, out priorEntry))
                {
                    _entries.Remove(key);
                    priorEntry.SetExpired(EvictionReason.Replaced);
                }

                if (!entry.CheckExpired(utcNow))
                {
                    _entries[key] = entry;
                    added = true;
                }
            }
            finally
            {
                _entryLock.ExitWriteLock();
            }
            if (priorEntry != null)
            {
            }
            if (!added)
            {
            }

            StartScanForExpiredItems();

            return value;
        }

        public bool TryGetValue(object key, out object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            value = null;
            CacheEntry expiredEntry = null;
            bool found = false;
            CheckDisposed();
            _entryLock.EnterReadLock();
            try
            {
                CacheEntry entry;
                if (_entries.TryGetValue(key, out entry))
                {
                    // Check if expired due to expiration tokens, timers, etc. and if so, remove it.
                    if (entry.CheckExpired(DateTimeOffset.UtcNow))
                    {
                        expiredEntry = entry;
                    }
                    else
                    {
                        // Refresh sliding expiration, etc.
                        entry.LastAccessed = DateTimeOffset.UtcNow;
                        value = entry.Value;
                        found = true;

                        var link = EntryLinkHelpers.ContextLink;
                        if (link != null)
                        {
                            // Copy expiration tokens and AbsoluteExpiration to the link
                            if (entry.Options.AbsoluteExpiration.HasValue)
                            {
                                link.SetAbsoluteExpiration(entry.Options.AbsoluteExpiration.Value);
                            }
                        }
                    }
                }
            }
            finally
            {
                _entryLock.ExitReadLock();
            }

            if (expiredEntry != null)
            {
                // TODO: For efficiency queue this up for batch removal
                RemoveEntry(expiredEntry);
            }

            StartScanForExpiredItems();

            return found;
        }

        public void Remove(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            CheckDisposed();
            CacheEntry entry;
            _entryLock.EnterReadLock();
            try
            {
                if (_entries.TryGetValue(key, out entry))
                {
                    entry.SetExpired(EvictionReason.Removed);
                }
            }
            finally
            {
                _entryLock.ExitReadLock();
            }

            if (entry != null)
            {
                // TODO: For efficiency consider processing these removals in batches.
                RemoveEntry(entry);
            }

            StartScanForExpiredItems();
        }

        private void RemoveEntry(CacheEntry entry)
        {
            _entryLock.EnterWriteLock();
            try
            {
                // Only remove it if someone hasn't modified it since our lookup
                CacheEntry currentEntry;
                if (_entries.TryGetValue(entry.Key, out currentEntry)
                    && object.ReferenceEquals(currentEntry, entry))
                {
                    _entries.Remove(entry.Key);
                }
            }
            finally
            {
                _entryLock.ExitWriteLock();
            }
        }

        private void RemoveEntries(List<CacheEntry> entries)
        {
            _entryLock.EnterWriteLock();
            try
            {
                foreach (var entry in entries)
                {
                    // Only remove it if someone hasn't modified it since our lookup
                    CacheEntry currentEntry;
                    if (_entries.TryGetValue(entry.Key, out currentEntry)
                        && object.ReferenceEquals(currentEntry, entry))
                    {
                        _entries.Remove(entry.Key);
                    }
                }
            }
            finally
            {
                _entryLock.ExitWriteLock();
            }

            foreach (var entry in entries)
            {
            }
        }

        private void EntryExpired(CacheEntry entry)
        {
            // TODO: For efficiency consider processing these expirations in batches.
            RemoveEntry(entry);
            StartScanForExpiredItems();
        }

        // Called by multiple actions to see how long it's been since we last checked for expired items.
        // If sufficient time has elapsed then a scan is initiated on a background task.
        private void StartScanForExpiredItems()
        {
            var now = DateTimeOffset.UtcNow;
            if (_expirationScanFrequency < now - _lastExpirationScan)
            {
                _lastExpirationScan = now;
                Task.Factory.StartNew(state => ScanForExpiredItems((MemoryCache)state), this,
                    CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
            }
        }

        private static void ScanForExpiredItems(MemoryCache cache)
        {
            List<CacheEntry> expiredEntries = new List<CacheEntry>();

            cache._entryLock.EnterReadLock();
            try
            {
                var now = DateTimeOffset.UtcNow;
                foreach (var entry in cache._entries.Values)
                {
                    if (entry.CheckExpired(now))
                    {
                        expiredEntries.Add(entry);
                    }
                }
            }
            finally
            {
                cache._entryLock.ExitReadLock();
            }

            cache.RemoveEntries(expiredEntries);
        }

        /// This is called after a Gen2 garbage collection. We assume this means there was memory pressure.
        /// Remove at least 10% of the total entries (or estimated memory?).
        private bool DoMemoryPreassureCollection(object state)
        {
            if (_disposed)
            {
                return false;
            }

            Compact(0.10);

            return true;
        }

        /// Remove at least the given percentage (0.10 for 10%) of the total entries (or estimated memory?), according to the following policy:
        /// 1. Remove all expired items.
        /// 2. Bucket by CacheItemPriority.
        /// ?. Least recently used objects.
        /// ?. Items with the soonest absolute expiration.
        /// ?. Items with the soonest sliding expiration.
        /// ?. Larger objects - estimated by object graph size, inaccurate.
        public void Compact(double percentage)
        {
            List<CacheEntry> expiredEntries = new List<CacheEntry>();
            List<CacheEntry> lowPriEntries = new List<CacheEntry>();
            List<CacheEntry> normalPriEntries = new List<CacheEntry>();
            List<CacheEntry> highPriEntries = new List<CacheEntry>();
            List<CacheEntry> neverRemovePriEntries = new List<CacheEntry>();

            _entryLock.EnterReadLock();
            try
            {
                // Sort items by expired & priority status
                var now = DateTimeOffset.UtcNow;
                foreach (var entry in _entries.Values)
                {
                    if (entry.CheckExpired(now))
                    {
                        expiredEntries.Add(entry);
                    }
                    else
                    {
                        switch (entry.Options.Priority)
                        {
                            case CacheItemPriority.Low:
                                lowPriEntries.Add(entry);
                                break;
                            case CacheItemPriority.Normal:
                                normalPriEntries.Add(entry);
                                break;
                            case CacheItemPriority.High:
                                highPriEntries.Add(entry);
                                break;
                            case CacheItemPriority.NeverRemove:
                                neverRemovePriEntries.Add(entry);
                                break;
                            default:
                                System.Diagnostics.Debug.Assert(false, "Not implemented: " + entry.Options.Priority);
                                break;
                        }
                    }
                }

                int totalEntries = expiredEntries.Count + lowPriEntries.Count + normalPriEntries.Count + highPriEntries.Count + neverRemovePriEntries.Count;
                int removalCountTarget = (int)(totalEntries * percentage);

                ExpirePriorityBucket(removalCountTarget, expiredEntries, lowPriEntries);
                ExpirePriorityBucket(removalCountTarget, expiredEntries, normalPriEntries);
                ExpirePriorityBucket(removalCountTarget, expiredEntries, highPriEntries);
            }
            finally
            {
                _entryLock.ExitReadLock();
            }

            RemoveEntries(expiredEntries);
        }

        /// Policy:
        /// ?. Least recently used objects.
        /// ?. Items with the soonest absolute expiration.
        /// ?. Items with the soonest sliding expiration.
        /// ?. Larger objects - estimated by object graph size, inaccurate.
        private void ExpirePriorityBucket(int removalCountTarget, List<CacheEntry> expiredEntries, List<CacheEntry> priorityEntries)
        {
            // Do we meet our quota by just removing expired entries?
            if (removalCountTarget <= expiredEntries.Count)
            {
                // No-op, we've met quota
                return;
            }
            if (expiredEntries.Count + priorityEntries.Count <= removalCountTarget)
            {
                // Expire all of the entries in this bucket
                foreach (var entry in priorityEntries)
                {
                    entry.SetExpired(EvictionReason.Capacity);
                }
                expiredEntries.AddRange(priorityEntries);
                return;
            }

            // Expire enough entries to reach our goal
            // TODO: Refine policy

            // LRU
            foreach (var entry in priorityEntries.OrderBy(entry => entry.LastAccessed))
            {
                entry.SetExpired(EvictionReason.Capacity);
                expiredEntries.Add(entry);
                if (removalCountTarget <= expiredEntries.Count)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }

                _disposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(typeof(MemoryCache).FullName);
            }
        }
    }
}
#endif