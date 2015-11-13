// <copyright file="CacheEntry.cs" company="Stormpath, Inc.">
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

namespace Stormpath.SDK.Extensions.Cache.Redis
{
    internal sealed class CacheEntry
    {
        private static readonly string Separator = ";;";

        public string Data { get; private set; }

        public CacheEntry(string data, DateTimeOffset created)
        {
            this.Data = data;
            this.CreatedAt = created;
        }

        public static CacheEntry Parse(string serialized)
        {
            var tokens = serialized.Split(new string[] { Separator }, StringSplitOptions.None);

            if (tokens.Length != 2)
                throw new ArgumentException("Cached data is invalid.", nameof(serialized));

            return new CacheEntry(
                tokens[1],
                DateTimeOffset.Parse(tokens[0]));
        }

        public DateTimeOffset CreatedAt { get; private set; }

        public override string ToString()
        {
            var data = new string[]
            {
                this.CreatedAt.ToString("o"),
                this.Data
            };

            return string.Join(Separator, data);
        }
    }
}
