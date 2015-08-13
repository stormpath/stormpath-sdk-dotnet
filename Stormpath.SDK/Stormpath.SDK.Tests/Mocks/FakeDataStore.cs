// <copyright file="FakeDataStore.cs" company="Stormpath, Inc.">
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Stormpath.SDK.Impl.DataStore;
using Stormpath.SDK.Impl.Resource;

namespace Stormpath.SDK.Tests.Mocks
{
    // TODO: Make this an actual server with valid responses
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements must appear in the correct order", Justification = "Compiled regex fields near methods that use them")]
    public class FakeDataStore<TType> : IDataStore
    {
        private static int defaultLimit = 25;
        private static int defaultOffset = 0;

        private readonly List<string> calls = new List<string>();

        public FakeDataStore(IEnumerable<TType> items)
        {
            this.Items = items.ToList();
        }

        public List<TType> Items { get; private set; }

        public IEnumerable<string> GetCalls()
        {
            return calls;
        }

        async Task<CollectionResponsePageDto<T>> IDataStore.GetCollectionAsync<T>(string href, CancellationToken cancellationToken)
        {
            bool typesMatch = typeof(T) == typeof(TType);
            if (!typesMatch)
                throw new ArgumentException("Requested type must match type of fake data.");

            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
            calls.Add(href);

            var limit = GetLimitFromUrlString(href) ?? defaultLimit;
            var offset = GetOffsetFromUrlString(href) ?? defaultOffset;

            return new CollectionResponsePageDto<T>()
            {
                Href = href,
                Items = this.Items.OfType<T>().Skip(offset).Take(limit).ToList(),
                Limit = limit,
                Offset = offset,
                Size = this.Items.Count
            };
        }

        private static Regex limitRegex = new Regex(@"limit=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetLimitFromUrlString(string href)
        {
            var match = limitRegex.Match(href);
            if (!match.Success) return null;
            if (string.IsNullOrEmpty(match.Groups?[1].Value)) return null;

            return int.Parse(match.Groups[1].Value);
        }

        private static Regex offsetRegex = new Regex(@"offset=(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static int? GetOffsetFromUrlString(string href)
        {
            var match = offsetRegex.Match(href);
            if (!match.Success) return null;
            if (string.IsNullOrEmpty(match.Groups?[1].Value)) return null;

            return int.Parse(match.Groups[1].Value);
        }
    }
}
