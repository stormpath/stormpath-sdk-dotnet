// <copyright file="DefaultCacheConfiguration.cs" company="Stormpath, Inc.">
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
using Stormpath.SDK.Cache;

namespace Stormpath.SDK.Impl.Cache
{
    internal sealed class DefaultCacheConfiguration : ICacheConfiguration
    {
        private readonly string name;
        private readonly TimeSpan? ttl;
        private readonly TimeSpan? tti;

        public DefaultCacheConfiguration(string name, TimeSpan? timeToLive, TimeSpan? timeToIdle)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.name = name;
            this.ttl = timeToLive;
            this.tti = timeToIdle;
        }

        string ICacheConfiguration.Name => this.name;

        TimeSpan? ICacheConfiguration.TimeToIdle => this.tti;

        TimeSpan? ICacheConfiguration.TimeToLive => this.ttl;
    }
}
