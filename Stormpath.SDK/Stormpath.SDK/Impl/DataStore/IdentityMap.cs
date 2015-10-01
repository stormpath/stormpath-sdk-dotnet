// <copyright file="IdentityMap.cs" company="Stormpath, Inc.">
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

using System;
using System.Collections.Concurrent;
using System.Threading;
using Stormpath.SDK.Impl.Extensions;

namespace Stormpath.SDK.Impl.DataStore
{
    internal class IdentityMap<TKey, TItem>
        where TItem : class
    {
        private static readonly int MinCompactThreshold = 100;

        private readonly object compactLock = new object();
        private int itemsAliveDuringLastCompact;
        private int itemsAddedSinceLastCompact;
        private long compactedTimes;

        private ConcurrentDictionary<TKey, WeakReference<TItem>> map;

        public IdentityMap()
        {
            this.map = new ConcurrentDictionary<TKey, WeakReference<TItem>>();
        }

        private bool ShouldCompact()
            => this.itemsAddedSinceLastCompact > Math.Max(MinCompactThreshold, this.itemsAliveDuringLastCompact + MinCompactThreshold);

        public int Count => this.itemsAliveDuringLastCompact + this.itemsAddedSinceLastCompact;

        public void Compact()
        {
            if (Monitor.TryEnter(this.compactLock, 0))
            {
                try
                {
                    var alive = 0;
                    var itemsAddedBeforeCompact = this.itemsAddedSinceLastCompact;

                    foreach (var kvp in this.map)
                    {
                        TItem item = null;

                        bool valueIsEmpty =
                            kvp.Value == null ||
                            !kvp.Value.TryGetTarget(out item) ||
                            item == null;

                        bool removed = false;
                        if (valueIsEmpty)
                            removed = this.map.TryRemove(kvp.Key, kvp.Value);

                        if (!removed)
                            alive++;
                    }

                    this.itemsAliveDuringLastCompact = alive;
                    this.compactedTimes++;

                    Interlocked.Add(ref this.itemsAddedSinceLastCompact, -itemsAddedBeforeCompact);
                }
                finally
                {
                    Monitor.Exit(this.compactLock);
                }
            }
        }

        public TItem GetOrAdd(TKey key, Func<TItem> itemFactory)
        {
            if (this.ShouldCompact())
                this.Compact();

            bool added = false;

            TItem item = null;
            var reference = this.map.GetOrAdd(key, _ =>
            {
                added = true;
                item = itemFactory();
                return new WeakReference<TItem>(item);
            });

            if (added)
            {
                Interlocked.Increment(ref this.itemsAddedSinceLastCompact);
                return item;
            }
            else
            {
                reference.TryGetTarget(out item);
                return item;
            }
        }
    }
}
